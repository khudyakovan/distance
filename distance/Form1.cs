using ClosedXML.Excel;
using distance.entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace distance
{
    public partial class FrmMain : Form
    {

        private List<Result> results = new List<Result>();
        private static Logger log = LogManager.GetCurrentClassLogger();
        const int DISTANCE_THRESHOLD = 500;

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
                    Application.DoEvents();
                    if (current.RowNumber() == 1) continue;
                    //audit_id;audit_last_update;user_full_name;point_name;point_address;point_longitude;point_latitude
                    dataRow = rawData.NewRow();
                    dataRow["audit_id"] = current.Cell("A").GetValue<Int32>();
                    dataRow["user_full_name"] = current.Cell("AA").GetString();
                    dataRow["audit_last_update"] = current.Cell("S").GetDateTime().Date;
                    dataRow["filling_start"] = current.Cell("BD").GetDateTime().Date;
                    dataRow["filling_end"] = current.Cell("BF").GetDateTime().Date;
                    if (current.Cell("BA").IsEmpty())
                    {
                        dataRow["tracking_deviation_max"] = 0;
                    }
                    else
                    {
                        dataRow["tracking_deviation_max"] = current.Cell("BA").GetValue<Int32>();
                    }
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

            rawData.DefaultView.Sort = "user_full_name ASC, audit_last_update ASC, point_name ASC";
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

                //audit_id;audit_last_update;user_full_name;point_name;point_address;point_longitude;point_latitude
                string currentUser = "";
                DateTime currentDate = new DateTime();
                results.Clear();

                foreach (DataRow row in rawData.Rows)
                {
                    Application.DoEvents();
                    int tracking_deviation_max = (int)row["tracking_deviation_max"];
                    int audit_id = (int)row["audit_id"];
                    DateTime filling_start = (DateTime)row["filling_start"];
                    DateTime filling_end = (DateTime)row["filling_end"];

                    if (tracking_deviation_max > DISTANCE_THRESHOLD)
                    {
                        log.Warn(String.Format("Отклонен аудит  c ID {0}. Отклонение от объекта {1} метров при лимите {2}",
                            audit_id,
                            tracking_deviation_max,
                            DISTANCE_THRESHOLD));
                        continue;
                    }

                    if (filling_start.Date != filling_end.Date)
                    {
                        log.Warn(String.Format("Отклонен аудит  c ID {0}. Отклонение даты начала заполнения {1} от даты окончания {2}",
                            audit_id,
                            filling_start.Date,
                            filling_end.Date));
                        continue;
                    }

                    if (currentUser != row["user_full_name"].ToString())
                    {
                        currentUser = row["user_full_name"].ToString();
                        currentDate = (DateTime)row["audit_last_update"];
                        DataRow[] rows = rawData.Select(String.Format("user_full_name = '{0}' AND audit_last_update='{1}'", currentUser, currentDate));
                        call2GISApi(rows, currentUser, currentDate);
                    }
                    else
                    {
                        if (currentDate.Date != ((DateTime)row["audit_last_update"]).Date)
                        {
                            currentDate = (DateTime)row["audit_last_update"];
                            DataRow[] rows = rawData.Select(String.Format("user_full_name = '{0}' AND audit_last_update='{1}'", currentUser, currentDate));
                            call2GISApi(rows, currentUser, currentDate);
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
                    log.Warn(String.Format("Отклонен аудит  c ID {0}. Повторное посещение точки {1} {2} внутри одного дня",
                        (int)row["audit_id"],
                        (int)row["point_name"],
                        (string)row["point_address"]));
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
            //Console.WriteLine(String.Format("{0},{1}, {2}", rows[0]["user_full_name"], rows[0]["audit_last_update"], JsonConvert.SerializeObject(routeQuery)));
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
                    result.length = ((int)jsonObject["result"][0]["length"]) / 1000;
                    results.Add(result);
                    //Console.WriteLine(String.Format("Duration: {0}; user: {1}; response: {2}", jsonObject["result"][0]["duration"], rows[0]["user_full_name"], content));
                }
                catch (Exception e)
                {
                    log.Error(String.Format("Аудитор: {0}, Дата аудита: {1}, payload: {2}, system: {3}", currentUser, currentDate, payload, e.Message));
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
