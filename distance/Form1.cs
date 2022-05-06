using ClosedXML.Excel;
using distance.entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace distance
{
    public partial class FrmMain : Form
    {

        private List<Result> results = new List<Result>();

        public FrmMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                //Get the path of specified file
                excelFilePath.Text = fileDialog.FileName;
            }
        }

        private void ReadFromExcelToRawDataTable(string filePath)
        {
            rawData.Rows.Clear();
            DataRow dataRow;

            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            Excel.Range range;

            xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Open(@filePath, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            range = xlWorkSheet.UsedRange;
            int rowCount = range.Rows.Count;

            pBar.Visible = true;
            pBar.Minimum = 0;
            pBar.Maximum = rowCount;

            statusLabel.Visible = true;
            statusLabel.Text = "";

            for (int i = 2; i <= rowCount; i++)
            {
                //audit_id;audit_start_date;user_full_name;point_name;point_address;point_longitude;point_latitude
                dataRow = rawData.NewRow();
                dataRow["audit_id"] = (string)(range.Cells[i, "A"] as Excel.Range).Value2;
                dataRow["user_full_name"] = (string)(range.Cells[i, "AA"] as Excel.Range).Value2;
                dataRow["audit_start_date"] = DateTime.Parse((range.Cells[i, "P"] as Excel.Range).Value2);
                dataRow["point_name"] = (string)(range.Cells[i, "AM"] as Excel.Range).Value2;
                dataRow["point_address"] = (string)(range.Cells[i, "AN"] as Excel.Range).Value2;
                dataRow["point_longitude"] = ((string)(range.Cells[i, "AP"] as Excel.Range).Value2).Replace(',', '.');
                dataRow["point_latitude"] = ((string)(range.Cells[i, "AO"] as Excel.Range).Value2).Replace(',', '.');

                rawData.Rows.Add(dataRow);
                pBar.Value = i;
                statusLabel.Text = String.Format("Запись {0} из {1}", i, rowCount);
            }

            statusLabel.Text = "Сортировка...";

            rawData.DefaultView.Sort = "user_full_name ASC, audit_start_date DESC";
            rawData = rawData.DefaultView.ToTable();

            xlWorkBook.Close(true, null, null);
            xlApp.Quit();

            Marshal.ReleaseComObject(xlWorkSheet);
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp);

            pBar.Visible = false;
            statusLabel.Text = "Готово! Нажмите 'Export'";
        }

        private void ImportFromExcelToToRawDataTable(string filePath)
        {
            button3.Enabled = false;
            rawData.Rows.Clear();
            DataRow dataRow;
            Application.DoEvents();

            using (var excelWorkbook = new XLWorkbook(filePath))
            {
                var nonEmptyDataRows = excelWorkbook.Worksheet(1).RowsUsed();
                int rowCount = nonEmptyDataRows.Count();
                pBar.Visible = true;
                pBar.Minimum = 0;
                pBar.Maximum = rowCount;
                statusLabel.Visible = true;
                statusLabel.Text = "Импорт из Excel...";

                foreach (var current in nonEmptyDataRows)
                {
                    if (current.RowNumber() == 1) continue;
                    //audit_id;audit_start_date;user_full_name;point_name;point_address;point_longitude;point_latitude
                    dataRow = rawData.NewRow();
                    dataRow["audit_id"] = current.Cell("A").GetValue<Int32>();
                    dataRow["user_full_name"] = current.Cell("AA").GetString();
                    dataRow["audit_start_date"] = current.Cell("P").GetDateTime();
                    dataRow["point_name"] = current.Cell("AM").GetString();
                    dataRow["point_address"] = current.Cell("AN").GetString();
                    dataRow["point_longitude"] = current.Cell("AP").GetString().Replace(',', '.');
                    dataRow["point_latitude"] = current.Cell("AO").GetString().Replace(',', '.');

                    rawData.Rows.Add(dataRow);
                    pBar.Value = current.RowNumber();
                    statusLabel.Text = String.Format("Запись {0} из {1}", pBar.Value, rowCount);
                }
            }

            statusLabel.Text = "Сортировка...";

            rawData.DefaultView.Sort = "user_full_name ASC, audit_start_date ASC, point_name ASC";
            rawData = rawData.DefaultView.ToTable();

            pBar.Visible = false;
            statusLabel.Text = "Готово! Нажмите 'Export'";
            button3.Enabled = true;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            //ReadFromExcelToRawDataTable(excelFilePath.Text);
            ImportFromExcelToToRawDataTable(excelFilePath.Text);
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            button3.Enabled = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {

                pBar.Visible = true;
                pBar.Value = 0;
                pBar.Minimum = 0;
                pBar.Maximum = rawData.Rows.Count;
                statusLabel.Visible = true;
                statusLabel.Text = "Запросы в 2GIS...";

                //audit_id;audit_start_date;user_full_name;point_name;point_address;point_longitude;point_latitude
                string currentUser = "";
                DateTime currentDate = new DateTime();
                results.Clear();

                foreach (DataRow row in rawData.Rows)
                {
                    if (currentUser != row["user_full_name"].ToString())
                    {
                        currentUser = row["user_full_name"].ToString();
                        currentDate = new DateTime();
                    }
                    else
                    {
                        if (currentDate != (DateTime)row["audit_start_date"])
                        {
                            currentDate = (DateTime)row["audit_start_date"];
                            DataRow[] rows = rawData.Select(String.Format("user_full_name = '{0}' AND audit_start_date='{1}'", currentUser, currentDate));
                            call2GISApi(rows, currentUser, currentDate);
                            continue;
                        }
                    }
                    pBar.Value++;
                }
                ExportResultsToExcel(saveFileDialog1.FileName);

                pBar.Visible = false;
                button3.Enabled = false;
                statusLabel.Text = "Готово! Результат сохранен";
            }
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private async void call2GISApi(DataRow[] rows, string currentUser, DateTime currentDate)
        {
            RouteQuery routeQuery = new RouteQuery();
            Result result = new Result();
            routeQuery.type = "shortest";
            routeQuery.output = "simple";
            routeQuery.points = new List<RoutePoint>();
            int point_name = -1;
            foreach (DataRow row in rows)
            {
                if (point_name != (int)row["point_name"])
                {
                    point_name = (int)row["point_name"];
                }
                else
                {
                    continue;
                }
                RoutePoint point = new RoutePoint
                {
                    x = (string)row["point_longitude"],
                    y = (string)row["point_latitude"],
                    type = "stop"
                };
                routeQuery.points.Add(point);
                point_name = (int)row["point_name"];
            }
            //Console.WriteLine(String.Format("{0},{1}, {2}", rows[0]["user_full_name"], rows[0]["audit_start_date"], JsonConvert.SerializeObject(routeQuery)));
            string payload = JsonConvert.SerializeObject(routeQuery);

            using (var client = new HttpClient())
            {
                var res = client.PostAsync("https://catalog.api.2gis.com/carrouting/6.0.0/global?key=rurbbn3446",
                  new StringContent(payload, Encoding.UTF8, "application/json"));
                try
                {
                    res.Result.EnsureSuccessStatusCode();
                    var content = await res.Result.Content.ReadAsStringAsync();
                    JObject jsonObject = JObject.Parse(content);
                    result.currentUser = currentUser;
                    result.currentDate = currentDate;
                    //result.duration = ((int)jsonObject["result"][0]["duration"])/60;
                    result.uniqueVisits = routeQuery.points.Count;
                    result.length = ((int)jsonObject["result"][0]["length"])/1000;
                    results.Add(result);
                    //Console.WriteLine(String.Format("Duration: {0}; user: {1}; response: {2}", jsonObject["result"][0]["duration"], rows[0]["user_full_name"], content));
                }
                catch (Exception e)
                {
                    Console.WriteLine(String.Format("User {0}, payload {1}", rows[0]["user_full_name"], payload));
                    Console.WriteLine(e.ToString());
                }
            }
        }

        private void ExportResultsToExcel(string excelFilePath)
        {
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Results of audits");
            
            //headers
            
            PropertyInfo[] properties = typeof(Result).GetProperties();
            List<string> headerNames = properties.Select(prop => prop.Name).ToList();
            for (int i = 0; i < headerNames.Count; i++)
            {
                ws.Cell(1, i + 1).Value = headerNames[i];
            }
            //Results
            ws.Cell(2, 1).InsertData(results);
            ws.Columns().AdjustToContents();
            wb.SaveAs(excelFilePath);
        }
    }
}
