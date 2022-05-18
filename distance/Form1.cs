using ClosedXML.Excel;
using distance.entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;

namespace distance
{
    public partial class FrmMain : Form
    {

        private List<Result> results = new List<Result>();
        private static Logger errorsLogger = LogManager .GetLogger("errors");
        private static Logger rejectionsLogger = LogManager.GetLogger("rejections");
        private static string ROUTE_QUERY_TYPE = "statistic";
        private static string ROUTE_QUERY_OUTPUT = "simple";
        const int DISTANCE_THRESHOLD = 500;
        private static List<string> headers = new List<string> {"Аудитор", "Дата посещения", "Количество ТТ", "Расстояние (км)"};
        const string TWO_GIS_API_URL = "https://catalog.api.2gis.com/carrouting/6.0.0/global?key=rurbbn3446";

        public FrmMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                //Get the path of specified file
                auditFilePath.Text = fileDialog.FileName;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                //Get the path of specified file
                mappingsFilePath.Text = fileDialog.FileName;
            }
        }

        private void ImportAuditsFromExcelToDataTable(string filePath)
        {
            btnExport.Enabled = false;
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
                statusLabel.Text = "Импорт аудитов из Excel...";

                foreach (var current in nonEmptyDataRows)
                {
                    Application.DoEvents();
                    if (current.RowNumber() == 1) continue;
                    //audit_id;audit_last_update;user_full_name;point_name;point_address;point_longitude;point_latitude
                    dataRow = rawData.NewRow();
                    dataRow["audit_id"] = current.Cell("A").GetValue<Int32>();
                    dataRow["user_full_name"] = current.Cell("AA").GetString();
                    dataRow["audit_last_update"] = current.Cell("S").GetDateTime();//.Date;
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

            rawData.DefaultView.Sort = "user_full_name ASC, audit_last_update ASC";
            rawData = rawData.DefaultView.ToTable();
        }

        private void ImportMappingsFromExcelToDataTable(string mappingsFilePath)
        {
            DataRow dataRow;
            mappingsDT.Rows.Clear();
            Application.DoEvents();

            using (var excelWorkbook = new XLWorkbook(mappingsFilePath))
            {
                var nonEmptyDataRows = excelWorkbook.Worksheet(1).RowsUsed();
                int rowCount = nonEmptyDataRows.Count();
                pBar.Visible = true;
                pBar.Minimum = 0;
                pBar.Maximum = rowCount;
                statusLabel.Visible = true;
                statusLabel.Text = "Импорт мэппингов из Excel...";
                foreach (var current in nonEmptyDataRows)
                {
                    Application.DoEvents();
                    if (current.RowNumber() == 1) continue;
                    //shop_id;office_longitude;office_latitude
                    dataRow = mappingsDT.NewRow();
                    dataRow["shop_id"] = current.Cell("A").GetValue<Int32>();
                    dataRow["office_longitude"] = current.Cell("B").GetString();
                    dataRow["office_latitude"] = current.Cell("C").GetString();
                    mappingsDT.Rows.Add(dataRow);
                }
            }

            //rawData.DefaultView.Sort = "user_full_name ASC, audit_last_update ASC, point_name ASC";
            //rawData = rawData.DefaultView.ToTable();
            mappingsDT.DefaultView.Sort = "shop_id ASC";
            mappingsDT = mappingsDT.DefaultView.ToTable();

            pBar.Visible = false;
            statusLabel.Text = "Готово! Нажмите 'Export'";
            btnExport.Enabled = true;
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            //ReadFromExcelToRawDataTable(excelFilePath.Text);
            this.ImportAuditsFromExcelToDataTable(auditFilePath.Text);
            this.ImportMappingsFromExcelToDataTable(mappingsFilePath.Text);
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {

        }

        private void btnExport_Click(object sender, EventArgs e)
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
                    int shopId = (int)row["point_name"];
                    DateTime filling_start = (DateTime)row["filling_start"];
                    DateTime filling_end = (DateTime)row["filling_end"];

                    if (tracking_deviation_max > DISTANCE_THRESHOLD)
                    {
                        rejectionsLogger.Warn(String.Format("Отклонен аудит  c ID {0}. Отклонение от объекта {1} метров при лимите {2}. Аудитор: {3}",
                            audit_id,
                            tracking_deviation_max,
                            DISTANCE_THRESHOLD,
                            (string)row["user_full_name"]));
                        continue;
                    }

                    if (filling_start.Date != filling_end.Date)
                    {
                        rejectionsLogger.Warn(String.Format("Отклонен аудит  c ID {0}. Отклонение даты начала заполнения {1} от даты окончания {2}",
                            audit_id,
                            filling_start.Date,
                            filling_end.Date,
                            (string)row["user_full_name"]));
                        continue;
                    }

                    if (currentUser != row["user_full_name"].ToString())
                    {
                        currentUser = row["user_full_name"].ToString();
                        currentDate = (DateTime)row["audit_last_update"];
                        DataRow[] rows = this.getCurrentAudits(currentUser, currentDate);
                        call2GISApi(rows,
                                    currentUser,
                                    currentDate,
                                    this.getCurrentOffice(shopId));
                    }
                    else
                    {
                        if (currentDate.Date != ((DateTime)row["audit_last_update"]).Date)
                        {
                            currentDate = (DateTime)row["audit_last_update"];
                            DataRow[] rows = this.getCurrentAudits(currentUser, currentDate);
                            call2GISApi(rows,
                                        currentUser,
                                        currentDate,
                                        this.getCurrentOffice(shopId));
                        }
                    }
                    pBar.Value++;
                }
                ExportResultsToExcel(saveFileDialog1.FileName);

                pBar.Visible = false;
                btnExport.Enabled = false;
                statusLabel.Text = "Готово! Результат сохранен";
                btnLogs.Enabled = true;
            }
        }

        private DataRow[] getCurrentAudits(string currentUser, DateTime currentDate)
        {
            string filter = String.Format("user_full_name = '{0}' AND audit_last_update>='{1}' AND audit_last_update < '{2}'", currentUser, currentDate.Date, currentDate.Date.AddDays(1));
            return rawData.Select(filter);
           
        }

        //Для каждого магазина есть отправная точка - региональный офис.
        //Данная функция возвращает привязанный к магазину офис
        private Office getCurrentOffice(int shopId)
        {
            Office currentOffice = new Office(-1);
            DataRow[] office = mappingsDT.Select(String.Format("shop_id = {0}", shopId));
            if (office != null && office.Length > 0)
            {
                currentOffice.ShopId = (int)office[0]["shop_id"];
                currentOffice.OfficeLongitude = (string)office[0]["office_longitude"];
                currentOffice.OfficeLatitude = (string)office[0]["office_latitude"];
            }
            else
            {
                errorsLogger.Error(String.Format("Для магазина {0} не найден офис. Расчет расстояний будет неверным", shopId));
            }
            return currentOffice;
        }

        private RouteQuery addOfficeRoutes(RouteQuery routeQuery, Office currentOffice)
        {
            if (currentOffice.ShopId != -1)
            {
                RoutePoint point = new RoutePoint
                {
                    x = currentOffice.OfficeLongitude,
                    y = currentOffice.OfficeLatitude,
                    type = "stop"
                };
                routeQuery.points.Add(point);
            }
            return routeQuery;
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private async void call2GISApi(DataRow[] rows, string currentUser, DateTime currentDate, Office currentOffice)
        {
            //var watch = System.Diagnostics.Stopwatch.StartNew();

            RouteQuery routeQuery = new RouteQuery();
            Result result = new Result();
            routeQuery.type = ROUTE_QUERY_TYPE;
            routeQuery.output = ROUTE_QUERY_OUTPUT;
            routeQuery.points = new List<RoutePoint>();
            int point_name = -1;

            //Путь начинается из офиса
            routeQuery = this.addOfficeRoutes(routeQuery, currentOffice);

            foreach (DataRow row in rows)
            {
                if (point_name != (int)row["point_name"])
                {
                    point_name = (int)row["point_name"];
                }
                else
                {
                    rejectionsLogger.Warn(String.Format("Отклонен аудит  c ID {0}. Повторное посещение точки {1} {2} внутри одного дня. Аудитор: {3}",
                        (int)row["audit_id"],
                        (int)row["point_name"],
                        (string)row["point_address"],
                        (string)row["user_full_name"]));
                    continue;
                }
                RoutePoint point = new RoutePoint
                {
                    x = (string)row["point_longitude"],
                    y = (string)row["point_latitude"],
                    type = "pref"
                };
                routeQuery.points.Add(point);
                point_name = (int)row["point_name"];
            }
            //И заканчивается маршрут в офисе
            routeQuery = this.addOfficeRoutes(routeQuery, currentOffice);

            //errorsLogger.Info(String.Format("{0},{1}, {2}", rows[0]["user_full_name"], rows[0]["audit_last_update"], JsonConvert.SerializeObject(routeQuery)));
            string payload = JsonConvert.SerializeObject(routeQuery);

            var handler = new HttpClientHandler
            {
                DefaultProxyCredentials = CredentialCache.DefaultCredentials
            };

            using (var client = new HttpClient(handler))
            {
                var res = client.PostAsync(TWO_GIS_API_URL,
                  new StringContent(payload, Encoding.UTF8, "application/json"));
                try
                {
                    //var watch = System.Diagnostics.Stopwatch.StartNew();
                    res.Result.EnsureSuccessStatusCode();
                    var content = await res.Result.Content.ReadAsStringAsync();
                    JObject jsonObject = JObject.Parse(content);
                    result.currentUser = currentUser;
                    result.currentDate = currentDate;
                    //result.duration = ((int)jsonObject["result"][0]["duration"])/60;
                    result.uniqueVisits = routeQuery.points.Count-2;
                    result.length = ((float)jsonObject["result"][0]["length"]) / 1000;
                    results.Add(result);
                    //watch.Stop();
                    //errorsLogger.Debug(String.Format("Execution: {0} ms, payload: {1}", watch.ElapsedMilliseconds, payload));
                    //Console.WriteLine(String.Format("Length: {0}; user: {1}; response: {2}", (int)jsonObject["result"][0]["length"], rows[0]["user_full_name"], content));
                }
                catch (Exception e)
                {
                    errorsLogger.Error(String.Format("Аудитор: {0}, Дата аудита: {1}, payload: {2}, system: {3}", currentUser, currentDate, payload, e.Message));
                }
            }

            //watch.Stop();
            //errorsLogger.Debug(String.Format("Execution: {0} ms, payload: {1}", watch.ElapsedMilliseconds, payload));
        }

        private void ExportResultsToExcel(string excelFilePath)
        {
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Results of audits");
            int i = 1;
            //headers
            foreach(string header in headers)
            {
                ws.Cell(1, i).Value = header;
                i++;
            }
            //Results
            ws.Cell(2, 1).InsertData(results);
            ws.Columns().AdjustToContents();
            wb.SaveAs(excelFilePath);
        }

        private void btnLogs_Click(object sender, EventArgs e)
        {
            string logsFilePath = String.Format(@"{0}\logs\", Application.StartupPath);
            Process.Start("explorer.exe", logsFilePath);
        }

    }
}
