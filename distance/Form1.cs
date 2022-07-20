using ClosedXML.Excel;
using distance.entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace distance
{
    public partial class FrmMain : Form
    {

        private List<Result> results = new List<Result>();
        private static Logger errorsLogger = LogManager.GetLogger("errors");
        private static Logger rejectionsLogger = LogManager.GetLogger("rejections");
        private static string ROUTE_QUERY_TYPE = "shortest";
        private static string ROUTE_QUERY_OUTPUT = "simple";
        const int DISTANCE_THRESHOLD = 500;
        private static string DISTANCE_TRESHOLD_WARN = "Отклонен аудит  c ID {0} Отклонение от объекта {1} метров при лимите {2}. Аудитор: {3}";
        private static string DATE_DIFF_WARN = "Отклонен аудит  c ID {0} Отклонение даты начала заполнения {1} от даты окончания {2}. Аудитор: {3}";
        private static string INTRADAY_RETURN = "Отклонен аудит  c ID {0}. Повторное посещение точки {1} {2} внутри одного дня. Аудитор: {3}";
        private static List<string> headers = new List<string> { "Аудитор", "Дата посещения", "Количество ТТ", "Расстояние (км)" };
        const string TWO_GIS_API_URL = "https://catalog.api.2gis.com/carrouting/6.0.0/global?key=rurbbn3446";
        private static string DONE_YANDEX_DATALENS = "Готово! Выгрузка в CSV завершена";

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

                    int audit_id = current.Cell("A").GetValue<Int32>();
                    string user_full_name = current.Cell("AA").GetString();
                    DateTime audit_last_update = current.Cell("S").GetDateTime();//.Date;
                    DateTime filling_start;
                    DateTime filling_end;
                    int tracking_deviation_max = 0;
                    string point_name = current.Cell("AM").GetString().Trim();
                    string point_address = current.Cell("AN").GetString().Trim();
                    string point_longitude = current.Cell("AP").GetString().Trim().Replace(',', '.');
                    string point_latitude = current.Cell("AO").GetString().Trim().Replace(',', '.');

                    dataRow = rawData.NewRow();
                    dataRow["audit_id"] = audit_id;
                    dataRow["user_full_name"] = user_full_name;
                    dataRow["audit_last_update"] = audit_last_update;
                    if (current.Cell("BD").IsEmpty())
                    { continue; }
                    else
                    {
                        filling_start = current.Cell("BD").GetDateTime().Date;
                        dataRow["filling_start"] = filling_start;
                    }
                    if (current.Cell("BF").IsEmpty())
                    { continue; }
                    else
                    {
                        filling_end = current.Cell("BF").GetDateTime().Date;
                        dataRow["filling_end"] = filling_end;
                    }


                    if (current.Cell("BA").IsEmpty())
                    {
                        dataRow["tracking_deviation_max"] = 0;
                    }
                    else
                    {
                        tracking_deviation_max = current.Cell("BA").GetValue<Int32>();
                        dataRow["tracking_deviation_max"] = tracking_deviation_max;
                    }

                    if (tracking_deviation_max > DISTANCE_THRESHOLD)
                    {
                        this.addLogRecord(audit_id,
                            audit_last_update,
                            "DISTANCE",
                            user_full_name,
                            String.Format(DISTANCE_TRESHOLD_WARN,
                            audit_id,
                            tracking_deviation_max,
                            DISTANCE_THRESHOLD,
                            user_full_name));
                        continue;
                    }

                    if (filling_start.Date != filling_end.Date)
                    {
                        this.addLogRecord(audit_id,
                            audit_last_update,
                            "DATE",
                            user_full_name,
                            String.Format(DATE_DIFF_WARN,
                            audit_id,
                            filling_start.Date,
                            filling_end.Date,
                            user_full_name));
                        continue;
                    }

                    dataRow["point_name"] = point_name;
                    dataRow["point_address"] = point_address;
                    dataRow["point_longitude"] = point_longitude;
                    dataRow["point_latitude"] = point_latitude;

                    rawData.Rows.Add(dataRow);
                    pBar.Value = current.RowNumber();
                    statusLabel.Text = String.Format("Запись {0} из {1}", pBar.Value, rowCount);
                }
            }

            statusLabel.Text = "Сортировка...";

            rawData.DefaultView.Sort = "user_full_name ASC, audit_last_update ASC";
            rawData = rawData.DefaultView.ToTable();
        }

        private void addLogRecord(int audit_id, DateTime audit_last_update, string reason, string user_full_name, string details)
        {
            DataRow logRecord = dtLogs.NewRow();
            logRecord["audit_id"] = audit_id;
            logRecord["audit_date"] = audit_last_update.Date;
            logRecord["reason"] = reason;
            logRecord["auditor"] = user_full_name;
            logRecord["details"] = details;
            dtLogs.Rows.Add(logRecord);
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
                    if (current.Cell("B").IsEmpty() || current.Cell("C").IsEmpty())
                    {
                        continue;
                    }
                    dataRow["office_longitude"] = current.Cell("B").GetString().Trim().Replace(',', '.');
                    dataRow["office_latitude"] = current.Cell("C").GetString().Trim().Replace(',', '.');
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
            chk_ExportToYDL.Enabled = true;
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
            if (chk_ExportToYDL.Checked)
            {
                ExportRawDataToDatalensFormat();
            }
            else
            {
                saveFileDialog1.ShowDialog();
                if (saveFileDialog1.FileName != "")
                {
                    var watch = System.Diagnostics.Stopwatch.StartNew();
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
                        int tracking_deviation_max = (int)row["tracking_deviation_max"];
                        int audit_id = (int)row["audit_id"];
                        int shopId = (int)row["point_name"];
                        DateTime filling_start = (DateTime)row["filling_start"];
                        DateTime filling_end = (DateTime)row["filling_end"];

                        //Писалось на скорую руку, надо отрефакторить
                        //Все эти проверки надо вынести на этап импорта из Excel
                        //Если расстояние по координатам отличается от координат магазина более чем на DISTANCE_THRESHOLD,
                        //то аудит отбрасывается, ошибка пишется в лог
                        //if (tracking_deviation_max > DISTANCE_THRESHOLD)
                        //{
                        //    rejectionsLogger.Warn(String.Format(DISTANCE_TRESHOLD_WARN,
                        //        audit_id,
                        //        tracking_deviation_max,
                        //        DISTANCE_THRESHOLD,
                        //        (string)row["user_full_name"]));
                        //    continue;
                        //}

                        //Если дата начала заполнения аудита отличается от даты окончания,
                        //то аудит отбрасывается, ошибка пишется в лог
                        //if (filling_start.Date != filling_end.Date)
                        //{
                        //    rejectionsLogger.Warn(String.Format(DATE_DIFF_WARN,
                        //        audit_id,
                        //        filling_start.Date,
                        //        filling_end.Date,
                        //        (string)row["user_full_name"]));
                        //    continue;
                        //}

                        if (currentUser != row["user_full_name"].ToString())
                        {
                            currentUser = row["user_full_name"].ToString();
                            currentDate = (DateTime)row["audit_last_update"];
                            Office currentOffice = this.getCurrentOffice(shopId);
                            if (currentOffice.ShopId == -1)
                                continue;
                            DataRow[] rows = this.getCurrentAudits(currentUser, currentDate);
                            call2GISApi(rows,
                                        currentUser,
                                        currentDate,
                                        currentOffice);
                        }
                        else
                        {
                            if (currentDate.Date != ((DateTime)row["audit_last_update"]).Date)
                            {
                                currentDate = (DateTime)row["audit_last_update"];
                                DataRow[] rows = this.getCurrentAudits(currentUser, currentDate);
                                Office currentOffice = this.getCurrentOffice(shopId);
                                if (currentOffice.ShopId == -1)
                                    continue;
                                call2GISApi(rows,
                                            currentUser,
                                            currentDate,
                                            currentOffice);
                            }
                        }
                        pBar.Value++;
                        statusLabel.Text = String.Format("Обработано {0} из {1}", pBar.Value, pBar.Maximum);
                        Application.DoEvents();
                    }
                    ExportResultsToExcel(saveFileDialog1.FileName);

                    pBar.Visible = false;
                    btnExport.Enabled = false;
                    watch.Stop();
                    statusLabel.Text = String.Format("Готово! Выполнение {0} минут", watch.ElapsedMilliseconds / 60000);
                    btnLogs.Enabled = true;
                }
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


            //Если в текущий день только одно посещение, то маршрут начинается из офиса
            //В противном случае от первого магазина 
            if (rows.Length == 1)
            {
                routeQuery = this.addOfficeRoutes(routeQuery, currentOffice);
            }

            foreach (DataRow row in rows)
            {
                if (point_name != (int)row["point_name"])
                {
                    point_name = (int)row["point_name"];
                }
                else
                {
                    this.addLogRecord((int)row["audit_id"],
                        (DateTime)row["audit_last_update"],
                        "DATE",
                        (string)row["user_full_name"],
                        String.Format(INTRADAY_RETURN,
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
            //if (RETURN_TO_OFFICE)
            //{
            //    routeQuery = this.addOfficeRoutes(routeQuery, currentOffice);
            //}

            //errorsLogger.Info(String.Format("{0},{1}, {2}", rows[0]["user_full_name"], rows[0]["audit_last_update"], JsonConvert.SerializeObject(routeQuery)));
            
            //Если после всех проверок точек маршрута осталось только 1, то метод 2GIS не вызывается
            //Невозможно построить маршрут по 1 точке.
            if (routeQuery.points.Count < 2) { return; } else
            {
                routeQuery.points.First().type = "stop";
                routeQuery.points.Last().type = "stop";
            }
            
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
                    result.currentDate = currentDate.Date;
                    //result.duration = ((int)jsonObject["result"][0]["duration"])/60;
                    int pointsCount = routeQuery.points.Count;
                    if (pointsCount == 2)
                    {
                        result.uniqueVisits = 1;
                    }
                    else
                    {
                        result.uniqueVisits = pointsCount;
                    }

                    result.length = ((float)jsonObject["result"][0]["length"]) / 1000;
                    results.Add(result);
                    //watch.Stop();
                    //errorsLogger.Debug(String.Format("Execution: {0} ms, payload: {1}", watch.ElapsedMilliseconds, payload));
                    //errorsLogger.Debug(String.Format("Audit Date {0}, Payload: {1}", currentDate, payload));
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
            var ws = wb.Worksheets.Add("Аудиты");
            int i = 1;
            //headers
            foreach (string header in headers)
            {
                ws.Cell(1, i).Value = header;
                i++;
            }
            //Results
            ws.Cell(2, 1).InsertData(results);
            ws.Columns().AdjustToContents();

            ws = wb.Worksheets.Add("Отклоненные аудиты");
            i = 1;
            //headers
            foreach (DataColumn column in dtLogs.Columns)
            {
                ws.Cell(1, i).Value = column.Caption;
                i++;
            }
            //Results
            ws.Cell(2, 1).InsertData(dtLogs.Rows);
            ws.Columns().AdjustToContents();

            wb.SaveAs(excelFilePath);
        }

        private void btnLogs_Click(object sender, EventArgs e)
        {
            string logsFilePath = String.Format(@"{0}\logs\", Application.StartupPath);
            Process.Start("explorer.exe", logsFilePath);
        }

        private void ExportRawDataToDatalensFormat()
        {
            StringBuilder csv = new StringBuilder();
            string header = String.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8}",
                    "audit_id",
                    "audit_last_update",
                    "user_full_name",
                    "point_name",
                    "point_address",
                    "coordinates",
                    "filling_start",
                    "filling_end",
                    "tracking_deviation_max");
            csv.AppendLine(header);
            //audit_id;audit_last_update;user_full_name;point_name;point_address;point_latitude;point_longitude;filling_start;filling_end;tracking_deviation_max
            foreach (DataRow row in rawData.Rows)
            {
                string line = String.Format("{0};{1};{2};{3};{4};\"[{5},{6}]\";{7};{8};{9}",
                    row["audit_id"],
                    (DateTime)row["audit_last_update"],
                    row["user_full_name"],
                    row["point_name"],
                    Regex.Replace((string)row["point_address"], @"\t|\n|\r", ""),
                    row["point_latitude"],
                    row["point_longitude"],
                    (DateTime)row["filling_start"],
                    (DateTime)row["filling_end"],
                    row["tracking_deviation_max"]);
                csv.AppendLine(line);
            }
            File.WriteAllText(@"c:\temp\datalens.csv", csv.ToString());
            statusLabel.Text = DONE_YANDEX_DATALENS;
        }
    }
}
