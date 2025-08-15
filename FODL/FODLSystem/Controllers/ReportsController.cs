using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

using FODLSystem.Models;
using FODLSystem.Models.View_Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ReportService;

namespace FODLSystem.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IHostingEnvironment env;
        private readonly FODLSystemContext _context;
        public ReportsController(FODLSystemContext context, IHostingEnvironment Ienv)
        {
            _context = context;
            env = Ienv;
        }
        private static string DBServer()
        {
            return "192.168.0.229";
        }
        private static string DBUSer()
        {
            return "ict";
        }
        private static string DBUserPwd()
        {
            return "ict@ictdept";
        }
        public IActionResult Index()
        {
            

            string status = "Active,Default";
            string[] stat = status.Split(',').Select(n => n).ToArray();
            var lube = _context.LubeTrucks.Where(a => stat.Contains(a.Status)).Select(a => new
            {
                a.No,
                Text = a.No + " - " + a.Description
            });
            ViewData["LubeTruckId"] = new SelectList(lube.OrderBy(a => a.No), "No", "Text");


           
            var disp = _context.Dispensers.Where(a => stat.Contains(a.Status)).Select(a => new
            {
                a.No,
                Text = a.No + " - " + a.Name
            });
            ViewData["DispenserId"] = new SelectList(disp.OrderBy(a => a.No), "No", "Text");



            return View();
        }
        [HttpPost]
        public ActionResult getDataSummary(DateTime strStart, DateTime end,string lube,string disp)
        {
            string status = "";

            string fstatus = "Active,Posted,Transferred";
            string[] fstat = fstatus.Split(',').Select(n => n).ToArray();


            try
            {

                //DateTime startDate = DateTime.ParseExact(strStart, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                //DateTime endDate = DateTime.ParseExact(end, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                string STR1 = strStart.ToString("MM/dd/yyyy") + " 12:00:01 AM";
                string STR2 = end.ToString("MM/dd/yyyy") + " 11:59:59 PM";
                DateTime dt1 = Convert.ToDateTime(STR1);
                DateTime dt2 = Convert.ToDateTime(STR2);

                var model = _context.FuelOilSubDetails
                     .Where(a => a.FuelOilDetails.FuelOils.CreatedDate >= dt1 && a.FuelOilDetails.FuelOils.CreatedDate <= dt2)
                     .Where(a => fstat.Contains(a.Status));

                if (lube == "na" && disp == "na")
                {
                  
                }
                else
                {
                    model = model
                    .Where(a => a.FuelOilDetails.FuelOils.LubeTruckCode == lube)
                    .Where(a => a.FuelOilDetails.FuelOils.DispenserCode == disp);
                   
                }
          

                var v =
                model
                .Where(a => fstat.Contains(a.FuelOilDetails.Status))
                .Where(a => fstat.Contains(a.FuelOilDetails.FuelOils.Status))
                  .Select(a => new
                  {
                      a.FuelOilDetails.FuelOils.ReferenceNo,

                      a.FuelOilDetails.FuelOils.Shift
                      ,SourceNo = a.FuelOilDetails.FuelOils.LubeTrucks==null ?"": a.FuelOilDetails.FuelOils.LubeTrucks.No == "na" ? a.FuelOilDetails.FuelOils.Dispensers.Name : a.FuelOilDetails.FuelOils.LubeTrucks.No
                      ,EquipmentNo = a.FuelOilDetails.Equipments==null?"": a.FuelOilDetails.Equipments.No
                      ,EntryType = "Negative Adjmt.",
                      ItemNo = a.Items.No,
                      PostingDate = a.FuelOilDetails.FuelOils.TransactionDate,
                      DocumentDate = a.FuelOilDetails.FuelOils.CreatedDate,
                      Qty = a.VolumeQty,
                      EquipmentCode = a.FuelOilDetails.Equipments==null?"": a.FuelOilDetails.Equipments.No,
                      OfficeCode = a.FuelOilDetails.Locations==null?"": a.FuelOilDetails.Locations.OfficeCode,
                      FuelCode = a.Items.TypeFuel == "OIL-LUBE" ? a.FuelOilDetails.Equipments.FuelCodeOil : a.FuelOilDetails.Equipments.FuelCodeDiesel,
                      LocationCode = "SMPC-SITE",
                      DepartmentCode  = a.Items.TypeFuel == "OIL-LUBE" ? "342" : a.FuelOilDetails.Equipments.DepartmentCode,
                      a.Id,
                      a.Status,
                      a.FuelOilDetailId
                      ,a.FuelOilDetails.FuelOils.LubeTruckCode
                      ,a.FuelOilDetails.FuelOils.DispenserCode
                      ,Drivers = a.FuelOilDetails.Drivers==null?"": a.FuelOilDetails.Drivers.Name
                  });

              

                status = "success";
                var x = v.ToList();

                var models = new
                {
                 status
                 ,data = v
                };
                return new JsonResult(models);
            }
            catch (Exception ex)
            {
                var models = new
                {
                    status = "fail"
                 ,
                    message = ex.Message
                };
                return Json(models);
            }
        }
        public IActionResult printReport(ReportViewModel rvm)
        {

            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = new HttpResponseMessage();
                byte[] bytes = null;
                string xstring = JsonConvert.SerializeObject(rvm);



              string urilive = "http://localhost/FODLApi/api/printreport?rvm=";


                //string urilive = "http://localhost:59455/api/printreport?rvm=";

                response = client.GetAsync(urilive + xstring).Result;
                string byteToString = response.Content.ReadAsStringAsync().Result.Replace("\"", string.Empty);
                bytes = Convert.FromBase64String(byteToString);

                string rpttype = "";
                string _name = "";
                switch (rvm.rptType)
                {
                    case "PDF":
                        rpttype = "application/pdf";
                        break;
                    case "Excel":
                        rpttype = "application/vnd.ms-excel";
                        _name = "PrintReport.xls";
                        break;
                    default:
                        break;
                }


                return File(bytes, rpttype, _name);
            }
            catch (Exception e)
            {

                throw;
            }

        }
        public async Task<IActionResult> printReport2(ReportViewModel rvm)
        {

            //  try
            // {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = new HttpResponseMessage();
            string xstring = JsonConvert.SerializeObject(rvm);

            try
            {
                DataSet ds = new DataSet();

                var o = JsonConvert.DeserializeObject(xstring);
                ReportViewModel rptVM = JsonConvert.DeserializeObject<ReportViewModel>(xstring);
                string report = rptVM.Report;

                //LocalReport localReport = new LocalReport(path);
                string status = "Active,Posted,Transferred,Partially Transferred";
                string[] stat = status.Split(',').Select(n => n).ToArray();


                DateTime def = new DateTime(1, 1, 1);

                List<ReportDataSet> reportDataSets = new List<ReportDataSet>();
                var client2 = new Service1Client();

                //   string mimtype = "";
                Report _report = new Report
                {
                    FileName = report + ".rdlc",
                    FolderName = "FODL",

                };
                Database database = new Database
                {
                    DbServer = DBServer(),
                    DbName = "FODL",
                    DbUser = DBUSer(),
                    DbPwd = DBUserPwd()
                };

                if (report == "rptLiquidation")
                {

                    //FOR SQL QUERY

                    string commandText = "SELECT * FROM jFuel_Lube_Issuance "
                                     + "WHERE FuelOilId = " + rptVM.ReferenceId;

                    ReportDataSet SQLQueryDataset = new ReportDataSet
                    {
                        DataSetName = "DailyFuel",
                        SQLQuery = commandText

                    };
                    reportDataSets.Add(SQLQueryDataset);

                    //PDF
                    await client2.SetReportAsync(_report);
                    byte[] bytes = await client2.GeneratePDFNewAsync(reportDataSets.ToArray(), database);

                    return File(bytes, "application/pdf");
                }
                else
                {

                    DateTime dt1 = Convert.ToDateTime(rptVM.fromDate);
                    DateTime dt2 = Convert.ToDateTime(rptVM.toDate);

                    //FOR SQL QUERY

                    string commandText = "SELECT * FROM jFuel_Lube_Issuance WHERE CreatedDate BETWEEN '" + dt1 + "' AND '" + dt2 + "'";

                    if (!string.IsNullOrEmpty(rptVM.shift))
                    {
                        commandText += " AND Shift = '" + rptVM.shift + "'";
                    }
                    if (!string.IsNullOrEmpty(rptVM.disp))
                    {
                        commandText += " AND DispenserCode = '" + rptVM.disp + "'";
                    }
                    if (!string.IsNullOrEmpty(rptVM.lube))
                    {
                        commandText += " AND LubeTruckCode = '" + rptVM.lube + "'";
                    }


                    ReportDataSet SQLQueryDataset = new ReportDataSet
                    {
                        DataSetName = "DailyFuel",
                        SQLQuery = commandText

                    };
                    reportDataSets.Add(SQLQueryDataset);

                    _report.ReportParameters = new ReportParameter[4];
                    List<ReportParameter> param = new List<ReportParameter>();
                    ReportParameter reportParameter = new ReportParameter
                    {
                        Name = "period",
                        Value = dt1.ToString("MM/dd/yyyy") + " - " + dt2.ToString("MM/dd/yyyy")
                    };

                    _report.ReportParameters[0] = reportParameter;

                    ReportParameter reportParameter2 = new ReportParameter
                    {
                        Name = "lube",
                        Value = rptVM.lube
                    };

                    _report.ReportParameters[1] = reportParameter2;

                    ReportParameter reportParameter3 = new ReportParameter
                    {
                        Name = "dispenser",
                        Value = rptVM.disp
                    };

                    _report.ReportParameters[2] = reportParameter3;

                    ReportParameter reportParameter4 = new ReportParameter
                    {
                        Name = "shift",
                        Value = rptVM.shift
                    };

                    _report.ReportParameters[3] = reportParameter4;

                    //PDF
                    await client2.SetReportAsync(_report);
                }
                //client2.Endpoint.Binding.SendTimeout = new TimeSpan(0, 1, 30);
                if (rvm.rptType != "PDF")
                {
                    byte[] bytes = await client2.GenerateExcelNewAsync(reportDataSets.ToArray(), database);
                    return File(bytes, "application/vnd.ms-excel", "FuelConsumptionSummary.xls");
                    // return File(bytes, "application/msexcel",);
                }
                else
                {

                    byte[] bytes = await client2.GeneratePDFNewAsync(reportDataSets.ToArray(), database);

                    return File(bytes, "application/pdf");
                }

            }
            catch (Exception e)
            {
                (string.Format("Error in report : " + e.Message)).WriteLog();
                throw;
            }

        }

        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
    }
}