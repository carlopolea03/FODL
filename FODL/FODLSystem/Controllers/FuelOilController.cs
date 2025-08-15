using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Net.NetworkInformation;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using DNTBreadCrumb.Core;
using FODLSystem.Interface;
using FODLSystem.Models;
using FODLSystem.Models.View_Model;
using JobCardList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
//all codes modified by JRV
namespace FODLSystem.Controllers
{
    [Authorize]
    public class FuelOilController : Controller
    {
        private readonly string NAVUserName = @"Semiraramining\HANDSHAKE";
        private readonly string NAVPassword = "M1ntch0c0l@t3";
        private readonly FODLSystemContext _context;
        private readonly IGlobalnterface _globalnterface;
        private static string url = "http://Bacchus.semiraramining.net:7047/bc2019_smpc/WS/Semirara/";
        //private static string url = "http://APRODITE.semiraramining.net:7057/BC130_SMPC_TEST/WS/Semirara/";
        private static TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
        private static string errorMessage = "";
        private static int errId = 0;
        public FuelOilController(FODLSystemContext context, IGlobalnterface iinterface)
        {
            _context = context;
            _globalnterface = iinterface;

        }

        [BreadCrumb(Title = "Index", Order = 1, IgnoreAjaxRequests = true)]
        public IActionResult Index()
        {
            this.SetCurrentBreadCrumbTitle("Fuel Oil Liquidation");
         

                return View();
        }
        [BreadCrumb(Title = "Summary", Order = 2, IgnoreAjaxRequests = true)]
        public IActionResult Summary()
        {
            this.AddBreadCrumb(new BreadCrumb
            {
                Title = "Fuel Oil Liquidation",
                Url = string.Format(Url.Action("Index", "FuelOil")),
                Order = 1
            });
            return View();
        }
        public IActionResult signUrl(int id)
        {
            string imagedata = "";

            var fueloil = _context.FuelOilDetails.Where(a => a.Id == id).FirstOrDefault().Signature;
            if (string.IsNullOrEmpty(fueloil))
            {
                imagedata = "";
            }
            else
            {
                imagedata = fueloil;
            }
            var model = new
            {

                imagedata
            };

            return Json(model);

        }
        [HttpPost]
        public IActionResult DigitalSignature(int id, string DriverId)
        {

            string filename = "";
            string status = "";
            string message = "";
            string imageurl = "";
            int cnt = 0;


            try
            {

                cnt = _context.FuelOilSubDetails.Where(a => a.FuelOilDetailId == id).Count();
                if (cnt < 1)
                {
                    message = "Items should be added first before signing. If items has been added, try saving it first before signing";
                    status = "fail";

                    var modelErr = new
                    {

                        message,
                        status,
                        imageurl = ""
                    };

                    return Json(modelErr);


                }

                var fueloil = _context.FuelOilDetails
                    .Where(a => a.Id == id)
                    .FirstOrDefault();

                fueloil.Signature = "SIGNED";
                fueloil.DriverIdNumber = DriverId;
                _context.Update(fueloil);
                _context.SaveChanges();

                status = "success";
            }
            catch (Exception e)
            {

                status = "fail";
                message = e.Message;
                e.Message.WriteLog();

            }

            var model = new
            {

                message,
                status,
                imageurl
            };

            return Json(model);
        }

        [HttpPost]
        public IActionResult saveSnapShot(int id, string imgData)
        {

            string filename = "";
            string status = "";
            string message = "";
            string imageurl = "";
            int cnt = 0;

            try
            {
                cnt = _context.FuelOilSubDetails.Where(a => a.FuelOilDetailId == id).Count();
                if (cnt < 1)
                {
                    message = "Items should be added first before signing. If items has been added, try saving it first before signing";
                    status = "fail";

                    var modelErr = new
                    {

                        message,
                        status,
                        imageurl = ""
                    };

                    return Json(modelErr);


                }



                var fueloil = _context.FuelOilDetails.Where(a => a.Id == id).FirstOrDefault();
                fueloil.Signature = imgData;
                _context.Update(fueloil);
                _context.SaveChanges();




                imageurl = imgData;
                status = "success";
            }
            catch (Exception e)
            {

                status = "fail";
                message = e.Message;
                e.Message.WriteLog();

            }

            var model = new
            {

                message,
                status,
                imageurl
            };

            return Json(model);
        }
        [BreadCrumb(Title = "Create", Order = 2, IgnoreAjaxRequests = true)]
        // GET: Companies/Create
        public IActionResult Create()
        {
            try
            {
                string lubeAccess = User.Identity.GetLubeAccess();
                string dispenserAccess = User.Identity.GetDispenserAccess();


                string[] lubeId = lubeAccess.Split(',').Select(n => Convert.ToString(n)).ToArray();
                string[] dispenserId = dispenserAccess.Split(',').Select(n => Convert.ToString(n)).ToArray();

                string status = "Active,Default";
                string[] stat = status.Split(',').Select(n => n).ToArray();

                this.AddBreadCrumb(new BreadCrumb
                {
                    Title = "Fuel Oil Liquidation",
                    Url = string.Format(Url.Action("Index")),
                    Order = 1
                });

                ViewData["CreatedDate"] = DateTime.Now;
                ViewData["Signature"] = "";
                ViewData["Status"] = "Active";
                ViewData["Id"] = 0;

                ViewBag.defaultLocationCode = "";

              var LocationId = _context.Locations.OrderBy(r => r.No).Where(a => a.Status == "Active").Select(r => new {
                    Id = r.No,
                    Text = r.No + " | " + r.List
                });
                ViewData["LocationNo"] = new SelectList(LocationId, "Id", "Text");

                var EquipmentId = _context.Equipments.OrderBy(r => r.No).Where(a => a.Status == "Active").Select(r => new {
                    Id = r.No,
                    Text = r.No + " | " + r.Name
                });

                ViewData["EquipmentNo"] = new SelectList(EquipmentId, "Id", "Text");


                var drivers = _context.Drivers.OrderBy(r => r.IdNumber).Where(a => a.Status == "Enabled").Select(r => new {
                    Id = r.IdNumber,
                    Text = r.IdNumber + " | " + r.Name
                });

                ViewData["DriverIdNumber"] = new SelectList(drivers, "Id", "Text");

                var disp = _context.Dispensers.Where(a => a.Status == "Active");

                if (User.Identity.GetRoleName() != "Admin")
                {
                    disp = disp.Where(a => dispenserId.Contains(a.No));
                }

                var DispenserId = disp.Select(r => new {
                    Id = r.No,
                    Text = r.NewName
                });

                ViewData["DispenserId"] = new SelectList(DispenserId, "Id", "Text");

                var lube = _context.LubeTrucks.OrderBy(r => r.No)
                     .Where(a => a.Status == "Active");


                if (User.Identity.GetRoleName() != "Admin")
                {
                    lube = lube.Where(a => lubeId.Contains(a.No));

                }

                var LubeTruckId = lube.Select(r => new {
                    Id = r.No,
                    Text = r.No + " | " + r.Description
                });

                ViewData["LubeTruckId"] = new SelectList(LubeTruckId, "Id", "Text");

                var components = _context.Components.OrderBy(r => r.Description).Where(a => a.Status == "Active").Select(r => new {
                    Id = r.Code,
                    Text = r.Description
                });
                ViewData["components"] = new SelectList(components, "Id", "Text");

                //ViewData["Id"] = 0 ;
                var fuel = new FuelOil
                {
                    CreatedDate = DateTime.Now
                };

                ViewData["GeneratedfromBSmart"] = false;
                ViewBag.defaultLocationCode = 0;

                return View(fuel);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                return View("Index");
               
            }
            
        }

        [BreadCrumb(Title = "Edit", Order = 2, IgnoreAjaxRequests = true)]
        public IActionResult Edit(int id)
        {
            string lubeAccess = User.Identity.GetLubeAccess();
            string dispenserAccess = User.Identity.GetDispenserAccess();


            string[] lubeId = lubeAccess.Split(',').Select(n => Convert.ToString(n)).ToArray();
            string[] dispenserId = dispenserAccess.Split(',').Select(n => Convert.ToString(n)).ToArray();

            this.AddBreadCrumb(new BreadCrumb
            {
                Title = "Fuel Oil Liquidation",
                Url = string.Format(Url.Action("Index")),
                Order = 1
            });
            var model = _context.FuelOils.Where(a => a.Id == id).FirstOrDefault();
            if (model == null)
            {
                ModelState.AddModelError("", "Record not found!");
                return View("Index");
            }
            ViewData["GeneratedfromBSmart"] = model.GeneratedfromBSmart;
            ViewData["CreatedDate"] = model.CreatedDate;
            ViewData["Id"] = model.Id;
            ViewData["Status"] = model.Status;

            ViewBag.defaultLocationCode = _context.Dispensers.FirstOrDefault(r => r.No == model.DispenserCode).LocationCode;

            var LocationId = _context.Locations.OrderBy(r => r.No).Where(a => a.Status == "Active").Select(r => new {
                Id = r.No,
                Text = r.No + " | " + r.List
            });
            ViewData["LocationNo"] = new SelectList(LocationId, "Id", "Text");

            var EquipmentId = _context.Equipments.OrderBy(r => r.No).Where(a => a.Status == "Active").Select(r => new {
                Id = r.No,
                Text = r.No + " | " + r.Name
            });

            ViewData["EquipmentNo"] = new SelectList(EquipmentId, "Id", "Text");


            var drivers = _context.Drivers.OrderBy(r=>r.IdNumber).Where(a => a.Status == "Enabled").Select(r=>new {
                Id=r.IdNumber,
                Text = r.IdNumber + " | " + r.Name
            });

            ViewData["DriverIdNumber"] = new SelectList(drivers, "Id", "Text");

            var disp = _context.Dispensers.Where(a => a.Status == "Active");

            if (User.Identity.GetRoleName() != "Admin")
            {
                disp = disp.Where(a => dispenserId.Contains(a.No));
            }               
                
            var DispenserId = disp.Select(r => new {
                Id = r.No,
                Text =string.IsNullOrEmpty(model.DispenserName) ? r.Name:r.NewName
            });

            ViewData["DispenserId"] = new SelectList(DispenserId, "Id", "Text", model.DispenserCode);

            var lube = _context.LubeTrucks.OrderBy(r=>r.No)
                 .Where(a => a.Status == "Active");


            if (User.Identity.GetRoleName() != "Admin")
            {
                lube = lube.Where(a => lubeId.Contains(a.No));

            }

            var LubeTruckId = lube.Select(r => new {
                Id = r.No,
                Text = r.No + " | " + r.Description
            });

            ViewData["LubeTruckId"] = new SelectList(LubeTruckId, "Id", "Text", model.LubeTruckCode);

            var components = _context.Components.OrderBy(r => r.Description).Where(a => a.Status == "Active").Select(r => new {
                Id = r.Code,
                Text =  r.Description
            });
            ViewData["components"] = new SelectList(components, "Id", "Text");


            return View("Create", model);
        }

        public JsonResult SearchItem(string q)
        {

            
            var model = _context.Items
                .Where(a => a.Status == "Active")
                .Where(a => a.DescriptionLiquidation2.ToUpper().Contains(q.ToUpper()) || a.No.ToUpper().Contains(q.ToUpper())
                ).Select(b => new
                {
                    id = b.No,
                    text = b.No + " | " + b.DescriptionLiquidation2.ToUpper()

                });

         
            var modelItem = new
            {
                total_count = model.Count(),
                incomplete_results = false,
                items = model.ToList(),
            };
            return Json(modelItem);
        }
        public JsonResult SearchComponent()
        {


            string status = "Active,Default";
            string[] stat = status.Split(',').Select(n => n).ToArray();

            var model = _context.Components
                .Where(a => stat.Contains(a.Status))
                .Select(b => new
                {
                    id = b.Code,
                    text = b.Description,

                });

            var modelItem = new
            {
                total_count = model.Count(),
                incomplete_results = false,
                items = model.ToList(),
            };
            return Json(modelItem);
        }
        [HttpPost]
        public async Task<JsonResult>SaveFormDetail(FuelOilViewModel fvm, DateTime CDate, List<FuelOilSubDetail> fvdet)
        {
            string status = "";
            string message = "";
            int refId = 0;


            //CDate.ToString().WriteLog();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (string.IsNullOrEmpty(fvm.SMR.ToString()) || fvm.SMR ==0)
                    {
                        transaction.Rollback();
                        var model1 = new
                        {
                            status = "fail",
                            message = "SMR is required!"
                        };
                        return new JsonResult(model1);
                    }
                    //if (string.IsNullOrEmpty(fvm.DriverIdNumber))
                    //{
                    //    transaction.Rollback();
                    //    var model1 = new
                    //    {
                    //        status = "fail",
                    //        message = "Driver Id Number is required!"
                    //    };
                    //    return new JsonResult(model1);
                    //}
                    if (string.IsNullOrEmpty(fvm.EquipmentNo))
                    {
                        transaction.Rollback();
                        var model1 = new
                        {
                            status = "fail",
                            message = "Equipment No. is required!"
                        };
                        return new JsonResult(model1);
                    }

                    if (string.IsNullOrEmpty(fvm.LocationNo))
                    {
                        transaction.Rollback();
                        var model1 = new
                        {
                            status = "fail",
                            message = "Location No. is required!"
                        };
                        return new JsonResult(model1);
                    }

                    string[] _dispenserMandatory = { "PMS-NRR","PMS-MWS","PMS-MTL","PMS-MLV" };

                    if (string.IsNullOrEmpty(fvm.JobCardNo) && _dispenserMandatory.Contains(fvm.DispenserCode))
                    {
                        transaction.Rollback();
                        var model1 = new
                        {
                            status = "fail",
                            message = "Job Card No. is mandatory for the following Dispenser : PMS- NRR, PMS-MWS, PMS-MTL, PMS-MLV!"
                        };
                        return new JsonResult(model1);
                    }

                    if (fvm.Id == 0)
                    {
                        string refno = await _globalnterface.NextNoSeries("FOD Code");
                        if (string.IsNullOrEmpty(refno))
                        {
                            transaction.Rollback();
                            var model1 = new
                            {
                                status = "fail",
                                message = "Unable to generate No. Series."
                            };
                            return new JsonResult(model1);
                        }



                        FuelOilDetail fod = new FuelOilDetail
                        {
                            DetailNo = refno,
                            LocationNo = fvm.LocationNo,
                            EquipmentNo = fvm.EquipmentNo,
                            FuelOilId = fvm.FuelOilId
                            ,CreatedDate = CDate
                            ,SMR = fvm.SMR
                            ,DriverIdNumber = fvm.DriverIdNumber
                            ,JobCardNo = fvm.JobCardNo,
                           Signature = "SIGNED"
                        };
                        _context.Add(fod);
                        _context.SaveChanges();

                        refId = fod.Id;
                    }
                    else
                    {
                        var fod = _context.FuelOilDetails.FirstOrDefault(r=>r.Id == fvm.Id);
                        if (fod != null)
                        {
                            fod.LocationNo = fvm.LocationNo;
                            fod.EquipmentNo = fvm.EquipmentNo;
                            fod.FuelOilId = fvm.FuelOilId;
                            fod.CreatedDate = CDate;
                            fod.SMR = fvm.SMR;
                            fod.DriverIdNumber = fvm.DriverIdNumber;
                            fod.JobCardNo = fvm.JobCardNo;

                            _context.Entry(fod).State = EntityState.Modified;
                            _context.SaveChanges();

                            refId = fod.Id;
                        }
                    }

                    string[] listofGoals = fvdet.Where(r => r.Id > 0).Select(i => i.Id.ToString()).ToArray();
                    //comm status
                    _context.FuelOilSubDetails.RemoveRange(_context.FuelOilSubDetails.Where(x => !listofGoals.Contains(x.Id.ToString()) && x.FuelOilDetailId == refId));

                    if (fvdet!=null && fvdet.Count() > 0)
                    {
                        foreach(var det in fvdet)
                        {
                            if (string.IsNullOrEmpty(det.ItemNo))
                            {
                                transaction.Rollback();
                                var model1 = new
                                {
                                    status = "fail",
                                    message = "Item No. is required!"
                                };
                                return new JsonResult(model1);
                            }
                            if (string.IsNullOrEmpty(det.VolumeQty.ToString()) || det.VolumeQty==0)
                            {
                                transaction.Rollback();
                                var model1 = new
                                {
                                    status = "fail",
                                    message = "Volume Qty is required!"
                                };
                                return new JsonResult(model1);
                            }
                            //if (det.ItemNo != "FO000001" && det.ItemNo != "FO000287" && det.ItemNo != "FO000106")
                            //{
                            //    if (string.IsNullOrEmpty(det.ComponentCode))
                            //    {
                            //        transaction.Rollback();
                            //        var model1 = new
                            //        {
                            //            status = "fail",
                            //            message = "Components is required for item -" + det.ItemNo
                            //        };
                            //        return new JsonResult(model1);
                            //    }
                            //}
                            if (det.Id == 0)
                            {
                                var sub = new FuelOilSubDetail
                                {
                                    ItemNo = det.ItemNo,
                                    ComponentCode = det.ComponentCode==""?null:det.ComponentCode,
                                    VolumeQty = det.VolumeQty,
                                    TimeInput = DateTime.Now,
                                    FuelOilDetailId = refId
                                };
                                _context.Add(sub);
                            }
                            else
                            {
                                var jDet = _context.FuelOilSubDetails.FirstOrDefault(r=>r.Id == det.Id);
                                if (jDet != null)
                                {
                                    jDet.ItemNo = det.ItemNo;
                                    jDet.ComponentCode = det.ComponentCode == "" ? null : det.ComponentCode;
                                    jDet.VolumeQty = det.VolumeQty;
                                    _context.Entry(jDet).State = EntityState.Modified;
                                }
                            }
                            _context.SaveChanges();
                        }
                    }

                    transaction.Commit();
                    status = "success";
                    message = refId.ToString();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    status = "fail";
                    message = e.Message;

                }
            }

            var modelItem = new
            {
                status,
                message,
                fvm.FuelOilId
            };
            return new JsonResult(modelItem);
        }
        private bool testNetworkConnection() {
            try
            {
                Ping myPing = new Ping();
                String host = "192.168.70.231";
                byte[] buffer = new byte[32];
                int timeout = 1000;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                return (reply.Status == IPStatus.Success);
               
                
            }
            catch (Exception)
            {
                return false;
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveForm(FuelOilViewModel fvm)
        {
            string status = "";
            string message = "";
            string refno = "";
            string refid = "0";
            string isNew = "true";
            int messagenumber = 0;
            DateTime dt = new DateTime();





            if (string.IsNullOrEmpty(fvm.LubeTruckCode))
            {
                var model = new
                {
                    status = "fail",
                    message = "Lube Truck is required."
                };
                return new JsonResult(model);
            }
            if (string.IsNullOrEmpty(fvm.DispenserCode))
            {
                var model = new
                {
                    status = "fail",
                    message = "Dispenser Code is required."
                };
                return new JsonResult(model);
            }

            var dispensers = _context.Dispensers.FirstOrDefault(r => r.No == fvm.DispenserCode);

            //MODIFIED BY JRV
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (string.IsNullOrEmpty(fvm.ReferenceNo))
                    {


                        refno = await _globalnterface.NextNoSeries("FuelOil");
                        if (string.IsNullOrEmpty(refno))
                        {
                            transaction.Rollback();
                            var model1 = new
                            {
                                status = "fail",
                                message = "Unable to generate No. Series."
                            };
                            return new JsonResult(model1);
                        }

                        var nSeries = await _context.NoSeries.FirstOrDefaultAsync(r => r.Code == "FuelOil");
                        if (nSeries != null)
                        {
                            nSeries.LastNoUsed = refno;
                            _context.Entry(nSeries).State = EntityState.Modified;
                        }

                        var fo = new FuelOil
                        {
                            ReferenceNo = refno,
                            Shift = fvm.Shift,
                            CreatedDate =  fvm.CreatedDate,
                            CreatedBy = User.Identity.GetFullName(),
                            TransactionDate = DateTime.Now.Date
                            ,DispenserCode = fvm.DispenserCode
                            ,DispenserName = dispensers==null?"" :dispensers.NewName ,
                            LubeTruckCode = fvm.LubeTruckCode
                            ,SourceReferenceNo = refno
                            ,OriginalDate = DateTime.Now,
                         
                        };

                        _context.Add(fo);
                        _context.SaveChanges();

                        message = refno;
                        refid = fo.Id.ToString();


                        status = "success";

                        dt = DateTime.Now;
                    }
                    else
                    {
                        isNew = "false";
                        var fo = _context.FuelOils.Where(a => a.ReferenceNo == fvm.ReferenceNo).FirstOrDefault();
                        if (fo != null)
                        {
                            fo.Shift = fvm.Shift;

                            fo.CreatedDate = fvm.CreatedDate;
                            fo.CreatedBy = User.Identity.GetFullName();
                            fo.DispenserCode = fvm.DispenserCode;
                            fo.LubeTruckCode = fvm.LubeTruckCode;

                            if (fo.Status != "Posted")
                            {
                                fo.OriginalDate = fvm.CreatedDate;
                            }


                            _context.Update(fo);
                            _context.SaveChanges();


                            refid = fo.Id.ToString();

                            dt = fvm.CreatedDate;

                            status = "success";
                            message = fo.ReferenceNo;
                        }
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    status = "fail";
                    message = ex.InnerException==null? ex.Message: ex.InnerException.Message;
                    messagenumber = ex.InnerException == null ? ex.HResult: ex.InnerException.HResult;
                }

            }

            var modelItem = new
            {
                status,
                message,
                refid,
                isNew,
                messagenumber,
                dt,
                LocationCode = dispensers==null?"":dispensers.LocationCode
            };
            return new JsonResult(modelItem);
        }
        [HttpPost]

        public ActionResult DeleteEquipment(int id)
        {
            string message = "";
            string status = "";
            try
            {
                FuelOilDetail item = _context.FuelOilDetails.Find(id);
                item.Status = "Deleted_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                _context.Entry(item).State = EntityState.Modified;
                _context.SaveChanges();

                status = "success";
            }
            catch (Exception e)
            {

                status = "fail";
                message = e.InnerException.Message;
            }

            var res = new
            {
                message,
                status
            };
            return Json(res);
        }
        public IActionResult Delete(int id)
        {
            string status = "";
            string message = "";
            try
            {
                var items = _context.FuelOils.Find(id);

                if (items.Status != "Active")
                {
                    var model1 = new
                    {

                        status = "fail",
                        message = "Only item with Active status can be deleted"
                    };
                    return Json(model1);
                }

                items.Status = "Deleted";
                _context.Entry(items).State = EntityState.Modified;
                _context.SaveChanges();





                Log log = new Log
                {
                    Descriptions = "Delete FuelOil - " + id,
                    Action = "Delete",
                    Status = "success",
                    UserId = User.Identity.GetUserName()
                };
                _context.Add(log);


                _context.SaveChangesAsync();


                status = "success";
            }
            catch (Exception e)
            {

                message = e.InnerException.ToString();

            }

            var model = new
            {

                status,
                message
            };
            return Json(model);







        }
        [HttpPost]
        public IActionResult PostForm(string referenceNo)
        {
            string status = "";
            string message = "";
            try
            {


                var fdetail = _context.FuelOilDetails.Where(a => a.FuelOils.ReferenceNo == referenceNo).Where(a=>a.Status == "Active");
                foreach (var item in fdetail)
                {

                    if (item.EquipmentNo=="001" || item.EquipmentNo == "IBUTTON")
                    {
                        var model = new
                        {
                            status = "fail",
                            message = "Equipment No.(001 and IBUTTON) are not allowed to Post."
                        };
                        return Json(model);

                    };
                }

               


                var fo = _context.FuelOils.Where(a => a.ReferenceNo == referenceNo)
                    .FirstOrDefault();
                fo.Status = "Posted";
                _context.Update(fo);
                _context.SaveChanges();

                status = "success";
                message = fo.ReferenceNo;

            }
            catch (Exception ex)
            {
                status = "fail";
                message = ex.Message;
            }

            var modelItem = new
            {
                status,
                message
            };
            return Json(modelItem);
        }
        [HttpPost]
        public ActionResult getData(int columnCount)
        {
            string status = "Active,Posted,Transferred,Partially Transferred";
            string[] stat = status.Split(',').Select(n => n).ToArray();

           
            try
            {

                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                string order = Request.Form["order[0][column]"].FirstOrDefault();
                string orderDir = Request.Form["order[0][dir]"];


                var ReqList = _context.FuelOils
               //.Where(a => a.CreatedBy == User.Identity.GetFullName())
              .Where(a => stat.Contains(a.Status))
             // .Where(strFilter)
              .OrderByDescending(a => a.ReferenceNo)
              //.Skip(skip).Take(pageSize)
              .Select(a => new
              {
                  a.CreatedBy,
                  a.ReferenceNo,
                  a.CreatedDate,
                  a.Shift,
                  LubeTruckName = a.LubeTrucks==null?"": a.LubeTrucks.No + " / " + a.LubeTrucks.Description,
                  DispenserName = string.IsNullOrEmpty(a.DispenserName) ? a.Dispensers==null ?"" : a.Dispensers.Name : a.DispenserName,
                  a.Id,
                  a.Status,
                  a.SourceReferenceNo,
                  a.BatchName
              });

            if (User.Identity.GetRoleName() == "User")
            {
                  ReqList = ReqList.Where(a => a.CreatedBy == User.Identity.GetFullName());
            }


             recordsTotal = ReqList.Count();

            string[] fields = new string[] { "ReferenceNo", "CreatedDate", "Shift", "LubeTruckName", "DispenserName", "SourceReferenceNo", "SourceReferenceNo", "Status" };
            string searchByCol = "";
            for (var scol = 0; scol <= fields.Count(); scol++)
            {
                string ColSearchValue = Request.Form["columns[" + scol + "][search][value]"].FirstOrDefault();
                string Col = Request.Form["columns[" + scol + "][data]"];
                if (!string.IsNullOrEmpty(ColSearchValue))
                {
                    if (searchByCol == "")
                    {
                        searchByCol = Col + ".Contains(\"" + ColSearchValue + "\")";
                    }
                    else
                    {
                        searchByCol = searchByCol + " && " + Col + ".Contains(\"" + ColSearchValue + "\")";
                    }
                }
            }
            if (!string.IsNullOrEmpty(searchByCol))
            {
                ReqList = ReqList.Where(searchByCol);
            }

            int recFilter = ReqList.Count();

           // bool desc = false;
           // if (sortColumnDirection == "desc")
            //{
             //   desc = true;
          //  }

            //ReqList = ReqList.OrderBy(sortColumn + (desc ? " descending" : ""));
            var sortfield = fields[int.Parse(order)];
            ReqList = ReqList.OrderByField(sortfield, orderDir);
            ReqList = ReqList.Skip(skip).Take(pageSize);

                //if (pageSize < 0)
                //{
                //    pageSize = recordsTotal;
                //}

                //if (!desc)
                //{
                //    ReqList.OrderByDescending(a => a.ReferenceNo);
                //}

                var data = ReqList.ToList(); 

                var jsonData = new { draw, recordsFiltered = recFilter, recordsTotal, data };
                return new JsonResult(jsonData);
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }
        public IActionResult getDataDetails(int? id)
        {
            string status = "Active,Transferred";
            string[] stat = status.Split(',').Select(n => n).ToArray();
           // string strFilter = "";
            try
            {


                var draw = Request.Form["draw"].FirstOrDefault();
                int start = Convert.ToInt32(Request.Form["start"]);
                var length = Convert.ToInt32(Request.Form["length"]);
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                //int pageSize = length != null ? Convert.ToInt32(length) : 0;
                //int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                var ReqList = _context.FuelOilDetails
                  .Where(a => a.FuelOilId == id)
                  //.Where(a => a.Status != "Deleted")
                  .Where(a => stat.Contains(a.Status))
                  //.Where(strFilter)
                  //.Skip(skip).Take(pageSize)
                  .Select(a => new
                  {
                      EquipmentName = a.Equipments==null ?"" : a.Equipments.No,
                      LocationName = a.Locations==null?"": a.Locations.List,
                      SMR = a.SMR ==null ? 0 : a.SMR,
                      a.CreatedDate,
                      a.Status,
                      a.Id,
                      a.LocationNo,
                      a.EquipmentNo,
                      SignStatus = string.IsNullOrEmpty(a.Signature) ? "" : "Signed",
                      DocumentStatus = a.FuelOils.Status,
                      a.DriverIdNumber,
                      a.JobCardNo
                  });

                recordsTotal = ReqList.Count();

                string[] fields = new string[] { "CreatedDate","EquipmentName", "LocationName", "SMR", "CreatedDate", "Status" };
                string searchByCol = "";
                for (var scol = 0; scol <= fields.Count(); scol++)
                {
                    string ColSearchValue = Request.Form["columns[" + scol + "][search][value]"].FirstOrDefault();
                    string Col = Request.Form["columns[" + scol + "][data]"];
                    if (!string.IsNullOrEmpty(ColSearchValue))
                    {
                        if (searchByCol == "")
                        {
                            searchByCol = Col + ".Contains(\"" + ColSearchValue + "\")";
                        }
                        else
                        {
                            searchByCol = searchByCol + " && " + Col + ".Contains(\"" + ColSearchValue + "\")";
                        }
                    }
                }
                if (!string.IsNullOrEmpty(searchByCol))
                {
                    ReqList = ReqList.Where(searchByCol);
                }


                int recFilter = ReqList.Count();


                bool desc = false;
                if (sortColumnDirection == "desc")
                {
                    desc = true;
                }
                ReqList = ReqList.OrderBy(sortColumn + (desc ? " descending" : ""));

                ReqList = ReqList.Skip(start).Take(length);

                //if (pageSize < 0)
               // {
                //    pageSize = recordsTotal;
                //}


                var data = ReqList.ToList();
                var jsonData = new {  draw, recordsFiltered = recFilter,  recordsTotal, data };
                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                return Json(ex);
            }



        }
        [HttpPost]
        public ActionResult getDataReferenceNo()
        {
            
            string status = "Posted,Partially Transferred";
            string[] stat = status.Split(',').Select(n => n).ToArray();


            try
            {

                var v = _context.FuelOils.Include(r => r.FuelOilDetail)
                    .Where(y => y.Status == "Posted" || y.Status == "Partially Transferred")
                    .Select(
                    a => new
                    {

                        a.Id,
                        fuelStatus = a.Status,
                        a.ReferenceNo
                    });



                //var v = _context.FuelOilSubDetails.Include(r => r.FuelOilDetails).ThenInclude(r => r.FuelOils)
                //  //.Where(a => fuelid.Contains(a.FuelOilDetails.FuelOilId))
                //  .Where(a => a.ItemNo != "" && a.ItemNo != null)
                //  // .Where(a => a.FuelOilDetails.Status == "Active")
                //  .GroupBy(r => r.FuelOilDetails.FuelOilId)
                //    .Select(
                //    a => new
                //    {

                //        Id = a.Key,
                //        fuelStatus = a.Select(r => r.FuelOilDetails.FuelOils.Status).FirstOrDefault(),
                //        ReferenceNo = a.Select(r => r.FuelOilDetails.FuelOils.ReferenceNo).FirstOrDefault()
                //    })
                //    .Where(y=>y.fuelStatus=="Posted" || y.fuelStatus== "Partially Transferred");

                //var v = _context.FuelOils
                //    //.Where(a => a.Status == "Posted")
                //    .Where(a => stat.Contains(a.Status))
                //    .Select( 
                //        a => new {
                //            a.Id,
                //            a.ReferenceNo
                //        } 
                //    );




                status = "success";

                var model = new
                {
                    status
                 ,
                    data = v
                };
                return Json(model);
            }
            catch (Exception ex)
            {
                var model = new
                {
                    status = "fail"
                 ,
                    message = ex.Message
                };
                return Json(model);
            }
        }
        [HttpPost]
        public ActionResult getDataSummary(string refid)
        {
            string status = "Posted,Partially Transferred";
            string[] stat = status.Split(',').Select(n => n).ToArray();


            int[] fuelid;
           
            try
            {
                if (string.IsNullOrEmpty(refid))
                {

                    fuelid = new int[0];
                }
                else
                {
                    fuelid = refid.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                }



                var v = _context.FuelOilSubDetails
                  .Where(a=> fuelid.Contains(a.FuelOilDetails.FuelOilId))
                  .Where(a => a.Status == "Active" &&  a.ItemNo!="" && a.ItemNo !=null)
                  .Where(a => a.FuelOilDetails.Status == "Active")
                  .Select(a => new
                  {
                      a.FuelOilDetails.FuelOils.ReferenceNo,
                      EntryType = "Negative Adjmt.",
                      ItemNo = a.Items.No,
                      PostingDate = a.FuelOilDetails.FuelOils.TransactionDate,
                      DocumentDate = a.FuelOilDetails.FuelOils.CreatedDate,
                      Qty = a.VolumeQty,
                      EquipmentCode = a.FuelOilDetails.Equipments==null?"": a.FuelOilDetails.Equipments.No,
                      OfficeCode = a.FuelOilDetails.Locations==null ?"" : a.FuelOilDetails.Locations.OfficeCode,
                      FuelCode = a.Items.TypeFuel == "OIL-LUBE" ? a.FuelOilDetails.Equipments==null?"" : a.FuelOilDetails.Equipments.FuelCodeOil : a.FuelOilDetails.Equipments.FuelCodeDiesel,
                      LocationCode = "SMPC-SITE",
                      //a.FuelOilDetails.Equipments.DepartmentCode,
                      DepartmentCode = a.Items.TypeFuel == "OIL-LUBE" ? "342" : a.FuelOilDetails.Equipments==null ?"" : a.FuelOilDetails.Equipments.DepartmentCode,
                      a.FuelOilDetails.JobCardNo,
                      a.Id,
                      a.Status,
                      a.FuelOilDetailId
                  });

               
                status = "success";

                var model = new
                {
                  status
                 ,data = v
                };
                return new JsonResult(model);
            }
            catch (Exception ex)
            {
                var model = new
                {
                  status = "fail"
                 ,message = ex.Message
                };
                return new JsonResult(model);
            }
        }

        public IActionResult getDataSubDetails(int id)
        {
            //string strFilter = "";
            string status = "Active,Transferred";
            string[] stat = status.Split(',').Select(n => n).ToArray();
            try
            {

                var v = _context.FuelOilSubDetails
               .Where(a => a.FuelOilDetailId == id)
              //.Where(a => a.Status != "Deleted")
              .Where(a => status.Contains(a.Status))
              .Select(a => new
              {
                  a.Items.No,
                  ItemName = a.Items==null?"": a.Items.Description,
                  ComponentId = a.Components ==null?"":a.Components.Code,
                  ComponentName = a.Components == null ? "" : a.Components.Description,
                  a.VolumeQty,
                  a.Id

              });


                var model = new
                {
                    data = v.ToList()
                };




                return Json(model);
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }
        [HttpPost]
        public IActionResult SaveFormSubDetail(FuelOilViewModel fvm)
        {
            string status = "";
            string message = "";
            string series = "";
            string refno = "";
            string refid = "0";

            



            try
            {
                var d = _context.FuelOilSubDetails.Where(a => a.FuelOilDetailId == fvm.Id).Count();
                if (d == 0)
                {
                    for (int i = 0; i < fvm.no.Length; i++)
                    {

                        var sub = new FuelOilSubDetail
                        {
                            ItemNo = fvm.no[i],
                            ComponentCode = fvm.component[i],
                            VolumeQty = Convert.ToInt32(fvm.volume[i]),
                            TimeInput = DateTime.Now,
                            FuelOilDetailId = fvm.Id
                        };
                        _context.Add(sub);

                    }
                    _context.SaveChanges();
                    status = "success";


                }
                else
                {

                    _context.FuelOilSubDetails
                          .Where(a => a.FuelOilDetailId == fvm.Id)
                          .ToList().ForEach(a => a.Status = "Deleted");


                    for (int i = 0; i < fvm.no.Length; i++)
                    {
                        var sub = new FuelOilSubDetail
                        {
                            ItemNo = fvm.no[i],
                            ComponentCode = fvm.component[i],
                            VolumeQty = Convert.ToInt32(fvm.volume[i]),
                            TimeInput = DateTime.Now,
                            FuelOilDetailId = fvm.Id
                        };
                        _context.Add(sub);

                    }
                    _context.SaveChanges();
                    status = "success";
                }
            }
            catch (Exception ex)
            {

                status = "fail";
                message = ex.Message;
            }

            var modelItem = new
            {
                status,
                message,
                refid
            };
            return Json(modelItem);
        }
      
        public class Author
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }
        public JsonResult Checkifposted() {

            string message = "";
            string status = "";
            int cnt = 0;
            cnt = _context.FuelOils
                    //.Where(a => a.TransactionDate == DateTime.Now.Date)
                    .Where(a => a.Status == "Active").Count();

            int cntPosted = _context.FuelOils
                    .Where(a => a.Status == "Posted").Count();

            if (cnt > 0)
            {
                message = "Not all input has been posted. Continue with downloading of posted data?";
                status = "success";
            }
            else if (cntPosted == 0) 
            {
                message = "No data to be download. Please try refreshing the page.";
                status = "fail";
            }
            else
            {
                cnt = -1;
                message = "This will download all data and will be move to archived. Continue?";
                status = "success";
            }

            var model = new
            {
                status,
                message

            };

            return Json(model);

        }

        public IActionResult DownloadExcel()
        {
           

            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = "fodl_" + DateTime.Now.ToString("MMddyyyyHHmmss") + ".xlsx";

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // .ThenInclude<FuelOil, FuelOilDetail, FuelOilSubDetail>(e=>e.FuelOilSubDetail)
                    //.ThenInclude(r=>r.Select(y=>y.FuelOilSubDetail))
                    var fodlheader = new List<FuelOil>();
                    var fodldetail = new List<FuelOilDetail>();
                    var fodlsub = new List<FuelOilSubDetail>();
                    int[] foid = null;
                    int[] fodetailid = null;

                    fodlheader = _context.FuelOils.Include(r=>r.FuelOilDetail)
                        .OrderByDescending(r => r.Id).Where(a => a.Status == "Posted")
                        .ToList();

                    if (fodlheader != null && fodlheader.Count() > 0)
                    {
                        foid = fodlheader.Select(a => a.Id).ToArray();

                        fodldetail = _context.FuelOilDetails.Include(a => a.Equipments).Where(a => foid.Contains(a.FuelOilId) && a.DetailNo != null)
                            .Where(a => a.Status == "Active")
                            .ToList();
                    }

                    if (fodldetail != null && fodldetail.Count() > 0)
                    {
                        fodetailid = fodldetail.Select(a => a.Id).ToArray();

                        fodlsub = _context.FuelOilSubDetails.Include(r => r.FuelOilDetails).Where(a => fodetailid.Contains(a.FuelOilDetailId) && a.FuelOilDetails.DetailNo != null)
                            .Where(a => a.Status == "Active")
                            .ToList();
                    }



                    // var  fodData = _context.FuelOils.I.Where(a => a.Status == "Posted");

                    using (var workbook = new XLWorkbook())
                    {


                        IXLWorksheet worksheet =
                        workbook.Worksheets.Add("Header");
                        worksheet.Cell(1, 1).Value = "ReferenceNo";
                        worksheet.Cell(1, 2).Value = "Shift";
                        worksheet.Cell(1, 3).Value = "CreatedDate";
                        worksheet.Cell(1, 4).Value = "CreatedBy";
                        worksheet.Cell(1, 5).Value = "Status";
                        worksheet.Cell(1, 6).Value = "DispenserCode";
                        worksheet.Cell(1, 7).Value = "LubeTruckCode";
                        worksheet.Cell(1, 8).Value = "TransactionDate";
                        worksheet.Cell(1, 9).Value = "Id";
                        worksheet.Cell(1, 10).Value = "OriginalDate";
                       // worksheet.Cell(1, 11).Value = "OriginalDate";
                        int index = 1;

                        foreach (var item in fodlheader)
                        {
                            if (item.FuelOilDetail != null && item.FuelOilDetail.Count() > 0)
                            {
                                worksheet.Cell(index + 1, 1).Value = "'" + item.ReferenceNo;
                                worksheet.Cell(index + 1, 2).Value = item.Shift;
                                worksheet.Cell(index + 1, 3).Value = item.CreatedDate.ToString("MM/dd/yyyy HH:mm");
                                worksheet.Cell(index + 1, 4).Value = item.CreatedBy;
                                worksheet.Cell(index + 1, 5).Value = item.Status;
                                worksheet.Cell(index + 1, 6).Value = "'" + item.DispenserCode;
                                worksheet.Cell(index + 1, 7).Value = "'" + item.LubeTruckCode;
                                worksheet.Cell(index + 1, 8).Value = item.TransactionDate.ToString("MM/dd/yyyy HH:mm");
                                worksheet.Cell(index + 1, 9).Value = item.Id;
                                worksheet.Cell(index + 1, 10).Value = item.OriginalDate.ToString("MM/dd/yyyy HH:mm");
                                index++;
                            }
                        }



                        IXLWorksheet worksheet2 =
                         workbook.Worksheets.Add("Detail");
                        worksheet2.Cell(1, 1).Value = "CreatedDate";
                        worksheet2.Cell(1, 2).Value = "EquipmentNo";
                        worksheet2.Cell(1, 3).Value = "LocationNo";
                        worksheet2.Cell(1, 4).Value = "FuelOilNo";
                        worksheet2.Cell(1, 5).Value = "Status";
                        worksheet2.Cell(1, 6).Value = "SMR";
                        worksheet2.Cell(1, 7).Value = "Signature";
                        worksheet2.Cell(1, 8).Value = "Id";
                        worksheet2.Cell(1, 9).Value = "CreatedBy";
                        worksheet2.Cell(1, 10).Value = "DriverIdNumber";
                        worksheet2.Cell(1, 11).Value = "OldDetailNo";
                        // worksheet2.Cell(1, 11).Value = "EquipmentNo";
                        index = 1;

                        foreach (var item in fodldetail)
                        {
                            worksheet2.Cell(index + 1, 1).Value = item.CreatedDate;
                            worksheet2.Cell(index + 1, 2).Value = "'" + item.EquipmentNo;
                            worksheet2.Cell(index + 1, 3).Value = "'" + item.LocationNo;
                            worksheet2.Cell(index + 1, 4).Value = "'" + item.FuelOils.ReferenceNo;
                            worksheet2.Cell(index + 1, 5).Value = item.Status;
                            worksheet2.Cell(index + 1, 6).Value = string.IsNullOrEmpty(item.SMR.ToString())?0: item.SMR;
                            worksheet2.Cell(index + 1, 7).Value = item.Signature;
                            worksheet2.Cell(index + 1, 8).Value = item.Id;
                            worksheet2.Cell(index + 1, 9).Value = item.FuelOils.CreatedBy;
                            worksheet2.Cell(index + 1, 10).Value = "'" + item.DriverIdNumber;
                            worksheet2.Cell(index + 1, 11).Value = "'" + item.DetailNo;
                            index++;
                        }


                        IXLWorksheet worksheet3 =
                         workbook.Worksheets.Add("SubDetail");
                        worksheet3.Cell(1, 1).Value = "TimeInput";
                        worksheet3.Cell(1, 2).Value = "ItemId";
                        worksheet3.Cell(1, 3).Value = "ComponentId";
                        worksheet3.Cell(1, 4).Value = "VolumeQty";
                        worksheet3.Cell(1, 5).Value = "FuelOilDetailNo";
                        worksheet3.Cell(1, 6).Value = "Status";
                        worksheet3.Cell(1, 7).Value = "Id";
                        worksheet3.Cell(1, 8).Value = "CreatedBy";


                        index = 1;

                        foreach (var item in fodlsub)
                        {
                            worksheet3.Cell(index + 1, 1).Value = item.TimeInput;
                            worksheet3.Cell(index + 1, 2).Value = "'" + item.ItemNo;
                            worksheet3.Cell(index + 1, 3).Value = "'" + item.ComponentCode;
                            worksheet3.Cell(index + 1, 4).Value = item.VolumeQty;
                            worksheet3.Cell(index + 1, 5).Value = "'" + item.FuelOilDetails.DetailNo;
                            worksheet3.Cell(index + 1, 6).Value = item.Status;
                            worksheet3.Cell(index + 1, 7).Value = item.Id;
                            worksheet3.Cell(index + 1, 8).Value = item.FuelOilDetails.FuelOils.CreatedBy;

                            index++;
                        }


                        string dateticks = DateTime.Now.Ticks.ToString();
                        _context.FuelOils.Where(a => a.Status == "Posted").ToList().ForEach(a => a.Status = "Archived_" + dateticks);

                        if (foid != null)
                        {
                            _context.FuelOilDetails.Where(a => foid.Contains(a.FuelOilId)).Where(a => a.Status == "Archived").ToList().ForEach(a => a.Status = "Deleted");
                        }

                        if (fodetailid != null)
                        {
                            _context.FuelOilSubDetails.Where(a => fodetailid.Contains(a.FuelOilDetailId)).Where(a => a.Status == "Archived").ToList().ForEach(a => a.Status = "Deleted");

                        }

                        _context.SaveChanges();

                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            var content = stream.ToArray();

                            transaction.Commit();
                            return File(content, contentType, fileName);
                        }


                    }


                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ex.Message.WriteLog();
                    return null;
                }
            }
        }

        public async Task<IActionResult>UploadExcel()
        {
            string filePath = "";
            string message = "";
            string status = "";
            IFormFile file = Request.Form.Files[0];
            try
            {

                string strFilename = Path.GetFileNameWithoutExtension(file.FileName);

                int cntRec = _context.FileUploads.Where(a => a.FileName == strFilename).Count();

                if (cntRec > 0)
                {
                    var err = new
                    {
                        status = "failed",
                        message = "File already uploaded. Please check file type or filename."
                    };

                   
                    return Json(err);
                }
                
                StringBuilder sb = new StringBuilder();
                if (file.Length > 0)
                {
                    string sFileExtension = Path.GetExtension(file.FileName).ToLower();
                    ISheet sheet;
                    ISheet sheet2;
                    ISheet sheet3;

                    string fullPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\fileuploads\", file.FileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                        stream.Position = 0;
                        if (sFileExtension == ".xls")
                        {
                            HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
                           
                            sheet = hssfwb.GetSheet("Header"); //get first sheet from workbook 
                            sheet2 = hssfwb.GetSheet("Detail");
                            sheet3 = hssfwb.GetSheet("SubDetail");
                        }
                        else
                        {
                            XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
                           
                            sheet = hssfwb.GetSheet("Header"); //get first sheet from workbook 
                            sheet2 = hssfwb.GetSheet("Detail");
                            sheet3 = hssfwb.GetSheet("SubDetail");
                        }

                      
                        //string strTest = UploadExcelTest(sheet, sheet2, sheet3);

                        //if (strTest != "Ok")
                        //{
                        //    var models = new
                        //    {
                        //        status = "failed",
                        //        message = strTest
                        //    };

                        //    filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\fileuploads\" + file.FileName);
                        //    System.IO.File.Delete(filePath);
                        //    return Json(models);
                        //}

                        var transferExcel = await UploadExcelFinal(sheet, sheet2, sheet3,strFilename);
                        if(transferExcel!= "Ok")
                        {
                            var err = new
                            {
                                status = "failed",
                                message = transferExcel
                            };

                            filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\fileuploads\" + file.FileName);
                            System.IO.File.Delete(filePath);

                            return Json(err);
                        }                       
                    }
                }
                status = "success";
                message = "Uploaded successfully!";
            }
            catch (Exception e)
            {

                status = "failed";
                message = e.Message.ToString();
            }

            var model = new
            {
                status,
                message
            };

            filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\fileuploads\" + file.FileName);
            System.IO.File.Delete(filePath);

            return Json(model);
        }

        public async Task<string>UploadExcelFinal(ISheet sheet, ISheet sheet2, ISheet sheet3,string strFilename)
        {
            try
                {
                    int rowCount = sheet.LastRowNum;
                    int line = 1;

                    //header
                    List<FuelOil> svm = new List<FuelOil>();
                    for (int i = 1; i <= rowCount; i++)
                    {
                        using (var transaction = _context.Database.BeginTransaction())
                        {
                            try
                            {
                                line = 1;
                                IRow headerRow = sheet.GetRow(i); //Get Header Row
                                int cellCount = headerRow.LastCellNum;
                                if (!string.IsNullOrEmpty(headerRow.Cells[0].StringCellValue))
                                {
                                    string refno = await _globalnterface.NextNoSeries("FuelOil");
                                    if (string.IsNullOrEmpty(refno))
                                    {
                                        transaction.Rollback();
                                        return "fail";
                                    }

                                    var tfuel = _context.FuelOils.Count(r => r.ReferenceNo == refno);
                                    if (tfuel > 0)
                                    {
                                        return "Please try again later, system detected duplicate Reference No while saving the data.";
                                    }

                                    var _jOldref = headerRow.Cells[0].StringCellValue;
                                    var jfuel = _context.FuelOils.FirstOrDefault(r => r.SourceReferenceNo == _jOldref);
                                    if (jfuel == null)
                                    {
                                        var nSeries = await _context.NoSeries.FirstOrDefaultAsync(r => r.Code == "FuelOil");
                                        if (nSeries != null)
                                        {
                                            nSeries.LastNoUsed = refno;
                                            _context.Entry(nSeries).State = EntityState.Modified;
                                        }

                                    var dispensers = _context.Dispensers.FirstOrDefault(r => r.No == headerRow.Cells[5].StringCellValue);
                                        FuelOil sv = new FuelOil
                                        {
                                            ReferenceNo = refno,
                                            SourceReferenceNo = headerRow.Cells[0].StringCellValue,
                                            Shift = headerRow.Cells[1].StringCellValue,
                                            CreatedDate = headerRow.Cells[2].DateCellValue,
                                            CreatedBy = headerRow.Cells[3].StringCellValue,
                                            Status = headerRow.Cells[4].StringCellValue,
                                            DispenserCode = dispensers == null ? "" : dispensers.No,
                                            DispenserName = dispensers == null ? "" : dispensers.Name,
                                            LubeTruckCode = headerRow.Cells[6].StringCellValue,
                                            TransactionDate = headerRow.Cells[7].DateCellValue,
                                            OriginalDate = headerRow.Cells[9].DateCellValue,
                                            TransferredBy = User.Identity.GetFullName(),
                                            OldId = Convert.ToInt32(headerRow.Cells[8].NumericCellValue),
                                            TransferDate = DateTime.Now,
                                        };
                                        //svm.Add(sv);
                                        _context.FuelOils.Add(sv);
                                        _context.SaveChanges();
                                        transaction.Commit();
                                    }
                                }
                                // refno = UploadNoSeries(refno);
                                line += 1;
                            }
                            catch (Exception e)
                            {
                                transaction.Rollback();
                                return e.InnerException == null ? e.Message : e.InnerException.Message;
                            }

                        }
                    }


                    //detail
                    rowCount = sheet2.LastRowNum;
                    List<FuelOilDetail> svmDetail = new List<FuelOilDetail>();

                    for (int i = 1; i <= rowCount; i++)
                    {
                        line = 1;
                        IRow detailRow = sheet2.GetRow(i); //Get Detail Row

                        var fuel = _context.FuelOils.FirstOrDefault(a => a.Status == "Posted" && a.SourceReferenceNo == detailRow.Cells[3].StringCellValue);
                        if (fuel != null)
                        {
                            var fDetails = _context.FuelOilDetails.FirstOrDefault(r => r.FuelOilId == fuel.Id && r.OldDetailNo == detailRow.Cells[10].StringCellValue);
                            if (fDetails == null)
                            {
                                string refno = await _globalnterface.NextNoSeries("FOD Code");
                                if (string.IsNullOrEmpty(refno))
                                {
                                    return "fail";
                                }

                                FuelOilDetail sv = new FuelOilDetail
                                {
                                    FuelOilId = fuel.Id,
                                    DetailNo = refno,
                                    CreatedDate = detailRow.Cells[0].DateCellValue,
                                    EquipmentNo = detailRow.Cells[1].StringCellValue,
                                    LocationNo = detailRow.Cells[2].StringCellValue,
                                    Status = detailRow.Cells[4].StringCellValue,
                                    SMR = Convert.ToDecimal(detailRow.Cells[5].NumericCellValue),
                                    Signature = detailRow.Cells[6].StringCellValue,
                                    OldId = Convert.ToInt32(detailRow.Cells[7].NumericCellValue), //FuelOilId
                                    DriverIdNumber = detailRow.Cells[9].StringCellValue == "" ? null : detailRow.Cells[9].StringCellValue,
                                    OldDetailNo = detailRow.Cells[10].StringCellValue
                                };
                                //svmDetail.Add(sv);
                                _context.FuelOilDetails.Add(sv);
                                _context.SaveChanges();
                                line += 1;
                            }
                        }
                    }

                    //_context.SaveChanges();

                    //sub-detail
                    rowCount = sheet3.LastRowNum;
                    List<FuelOilSubDetail> subsvmDetail = new List<FuelOilSubDetail>();
                    for (int i = 1; i <= rowCount; i++)
                    {
                        line = 1;
                        IRow subdetailRow = sheet3.GetRow(i); //Get Detail Row

                        var FODet = _context.FuelOilDetails.FirstOrDefault(a => a.OldDetailNo == subdetailRow.Cells[4].StringCellValue);
                        if (FODet != null)
                        {
                            var OldItemId = Convert.ToInt32(subdetailRow.Cells[6].NumericCellValue);
                            var fItemDet = _context.FuelOilSubDetails.FirstOrDefault(r => r.OldId == OldItemId && r.FuelOilDetailId == FODet.Id);
                            if (fItemDet == null)
                            {
                                FuelOilSubDetail sv = new FuelOilSubDetail
                                {
                                    FuelOilDetailId = FODet.Id,
                                    TimeInput = subdetailRow.Cells[0].DateCellValue,
                                    ItemNo = subdetailRow.Cells[1].StringCellValue,
                                    ComponentCode = string.IsNullOrEmpty(subdetailRow.Cells[2].StringCellValue) ? null : subdetailRow.Cells[2].StringCellValue,
                                    VolumeQty = Convert.ToInt32(subdetailRow.Cells[3].NumericCellValue),
                                    OldFuelOilDetailNo = subdetailRow.Cells[4].StringCellValue,
                                    Status = subdetailRow.Cells[5].StringCellValue,
                                    OldId = OldItemId // Convert.ToInt32(subdetailRow.Cells[6].NumericCellValue)
                                };
                                _context.FuelOilSubDetails.Add(sv);
                                _context.SaveChanges();
                                line += 1;
                            }
                        }
                    }

                    Log log = new Log
                    {
                        Action = "Upload",
                        CreatedDate = DateTime.Now,
                        Descriptions = "Upload Excel File " + strFilename,
                        Status = "success",
                        UserId = User.Identity.GetUserId().ToString()
                    };

                    _context.Add(log);


                    FileUpload fu = new FileUpload
                    {
                        FileName = strFilename,
                        UploadDate = DateTime.Now,
                        UploadBy = User.Identity.GetFullName()
                    };
                    _context.Add(fu);

                   

                    _context.SaveChanges();


                    return "Ok";
                }
                catch (Exception e)
                {
                    return e.InnerException==null? e.Message:e.InnerException.Message;
                }
        }
        public IActionResult DownloadCSV()
        {

            string fileName = "fodl_"+DateTime.Now.ToString("MMddyyyy") +".csv";
            try
            {
                var fodlheader = _context.FuelOils
                    .Where(a => a.Status == "Posted");
                int[] foid = fodlheader.Select(a => a.Id).ToArray();
                var fodldetail = _context.FuelOilDetails.Where(a=> foid.Contains(a.FuelOilId))
                    .Where(a => a.Status == "Active");
                int[] fodetailid = fodldetail.Select(a => a.Id).ToArray();
                var fodlsub = _context.FuelOilSubDetails.Where(a => fodetailid.Contains(a.FuelOilDetailId))
                    .Where(a => a.Status == "Active");

                try
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine("ReferenceNo,Shift,CreatedDate,CreatedBy,Status,DispenserCode,LubeTruckCode,TransactionDate");
                    foreach (var author in fodlheader)
                    {
                        stringBuilder.AppendLine($"{author.ReferenceNo},{ author.Shift},{ author.CreatedDate}," +
                            $"{author.CreatedBy }, {author.Status}, {author.DispenserCode},{author.LubeTruckCode },{author.TransactionDate }");
                    }
                    return File(Encoding.UTF8.GetBytes
                    (stringBuilder.ToString()), "text/csv", fileName);
                }
                catch
                {
                    return null;
                }

            }
            catch (Exception)
            {
                return null;
            }
        }

        public string UploadNoSeries(string nSeries)//JRV
        {
            string LastNoSeries = "";
            var NoFromString = Regex.Match(nSeries, @"\d+").Value;
            var stringFromNo = Regex.Replace(nSeries, @"[0-9]", string.Empty);

            int new_last_no = int.Parse(NoFromString) + 1;
            LastNoSeries = stringFromNo + new_last_no.ToString().PadLeft(NoFromString.Length, '0');


            return LastNoSeries;
        }

        public async Task<JsonResult> GetJobCardNo()//JRV
        {
            string search = Request.Query["q"];
            string machineID = Request.Query["s"];
            List<JobCardViewModel> _JobCardViewModel = new List<JobCardViewModel>();

            try
            {

                string JobCardURI = url + "Page/B2BJobCardList";
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding(System.ServiceModel.BasicHttpSecurityMode.TransportCredentialOnly)
                {
                    MaxReceivedMessageSize = 10485760
                };
                binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;

                var endpoint = new EndpointAddress(new Uri(JobCardURI));

                B2BJobCardList_PortClient _PortClient = new B2BJobCardList_PortClient(binding, endpoint);
                _PortClient.ClientCredentials.UserName.UserName = NAVUserName;
                _PortClient.ClientCredentials.UserName.Password = NAVPassword;

                List< B2BJobCardList_Filter> filterArray = new List<B2BJobCardList_Filter>();

                B2BJobCardList_Filter TypeFilter = new B2BJobCardList_Filter
                {
                    Field = B2BJobCardList_Fields.Type,
                    Criteria = "PMR"
                };

                filterArray.Add(TypeFilter);

                B2BJobCardList_Filter StatusFilter = new B2BJobCardList_Filter
                {
                    Field = B2BJobCardList_Fields.Status,
                    Criteria = Status.Released.ToString()
                };
                filterArray.Add(StatusFilter);

                B2BJobCardList_Filter DateFilter = new B2BJobCardList_Filter
                {
                    Field = B2BJobCardList_Fields.End_Date_Time,
                    Criteria = "=''"
                };
                filterArray.Add(DateFilter);

                B2BJobCardList_Filter AccessGroup = new B2BJobCardList_Filter
                {
                    Field = B2BJobCardList_Fields.Access_Group,
                    Criteria = "MOBILE"
                };
                filterArray.Add(AccessGroup);

                B2BJobCardList_Filter spin = new B2BJobCardList_Filter
                {
                    Field = B2BJobCardList_Fields.Machine_ID,
                    Criteria = machineID
                };
                filterArray.Add(spin);

                if (!string.IsNullOrEmpty(search))
                {
                    B2BJobCardList_Filter nameFilter = new B2BJobCardList_Filter
                    {
                        Field = B2BJobCardList_Fields.No,
                        Criteria = "@*" + search.ToUpper() + "*"
                    };
                    filterArray.Add(nameFilter);
                }

                ReadMultiple_Result fncResult = new ReadMultiple_Result();
                fncResult = await _PortClient.ReadMultipleAsync(filterArray.ToArray(), null, 20);

                for (var i = 0; i < fncResult.ReadMultiple_Result1.Count(); i++)
                {
                    _JobCardViewModel.Add(new JobCardViewModel
                    {
                        text = fncResult.ReadMultiple_Result1[i].No //+ " | " + fncResult.ReadMultiple_Result1[i].Created_Date.ToString("MM/dd/yyyy")
                        ,
                        id = fncResult.ReadMultiple_Result1[i].No
                         ,
                        title = fncResult.ReadMultiple_Result1[i].Work_Center
                         + "^" + fncResult.ReadMultiple_Result1[i].Machine_ID
                         + "^" + fncResult.ReadMultiple_Result1[i].Access_Group
                         + "^" + fncResult.ReadMultiple_Result1[i].Problem_Description
                    });

                }

            }
            catch (Exception)
            {
                HttpContext.Response.StatusCode = 400;
                return null;
            }

            return new JsonResult(_JobCardViewModel);
        }

        public IActionResult BSmart(int f=0)//JRV
        {
            ViewBag.alldata = f;
            return View();
        }
        [HttpPost]
        public ActionResult getBSmartData(int f)//JRV
        {
            
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                string order = Request.Form["order[0][column]"].FirstOrDefault();
                string orderDir = Request.Form["order[0][dir]"];


               var ReqList = _context.BSmartShifts
              .OrderByDescending(a => a.Id)
              .Select(a => new
              {
                  a.CreatedBy,
                  a.ReferenceNo,
                  CreatedDate = a.CreatedDate,
                  TransferDate = a.TransferDate,
                  a.Shift,
                  LubeTruckName = a.LubeTrucks == null ?"":a.LubeTrucks.Description,
                  DispenserName = a.Dispensers==null?"":a.Dispensers.NewName,
                  a.Id,
                  a.SourceReferenceNo
              });

                if (f == 0)
                {
                    ReqList = ReqList.Where(r => r.SourceReferenceNo == "" || r.SourceReferenceNo ==null);
                }

                recordsTotal = ReqList.Count();

                string[] fields = new string[] {"","", "ReferenceNo", "SourceReferenceNo", "CreatedBy", "CreatedDate", "TransferDate", "Shift", "LubeTruckName", "DispenserName" };
                string searchByCol = "";
                for (var scol = 0; scol <= fields.Count(); scol++)
                {
                    string ColSearchValue = Request.Form["columns[" + scol + "][search][value]"].FirstOrDefault();
                    //string Col = fields[scol];// Request.Form["columns[" + scol + "][data]"];
                    if (!string.IsNullOrEmpty(ColSearchValue))
                    {
                        string Col = fields[scol];
                        if (searchByCol == "")
                        {
                           // searchByCol = Col + ".ToString().ToUpper().Contains(" + "\"" + ColSearchValue + "\"" + ")";
                            searchByCol = Col.ToString() + ".Contains(\"" + ColSearchValue + "\")";
                        }
                        else
                        {
                            searchByCol = searchByCol.ToString() + " && " + Col + ".Contains(\"" + ColSearchValue + "\")";
                        }
                    }
                }
                if (!string.IsNullOrEmpty(searchByCol))
                {
                    ReqList = ReqList.Where(searchByCol);
                }

                int recFilter = ReqList.Count();

                var sortfield = fields[int.Parse(order)];
                ReqList = ReqList.OrderByField(sortfield, orderDir);
                ReqList = ReqList.Skip(skip).Take(pageSize);

                var data = ReqList.ToList();

                var jsonData = new { draw, recordsFiltered = recFilter, recordsTotal, data };
                return new JsonResult(jsonData);
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }
        public IActionResult BSmartView(int id)
        {

            this.AddBreadCrumb(new BreadCrumb
            {
                Title = "Fuel Oil Liquidation",
                Url = string.Format(Url.Action("Index")),
                Order = 1
            });
            var model = _context.BSmartShifts.FirstOrDefault(a => a.Id == id);


            var EquipmentId = _context.Equipments.OrderBy(r => r.No).Where(a => a.Status == "Active").Select(r => new {
                Id = r.No,
                Text = r.No + " | " + r.Name
            });

            ViewData["EquipmentNo"] = new SelectList(EquipmentId, "Id", "Text");


            var drivers = _context.Drivers.OrderBy(r => r.IdNumber).Where(a => a.Status == "Enabled").Select(r => new {
                Id = r.IdNumber,
                Text = r.IdNumber + " | " + r.Name
            });

            ViewData["DriverIdNumber"] = new SelectList(drivers, "Id", "Text");

            var disp = _context.Dispensers.Where(a => a.Status != "Deleted");

            var LocationId = _context.Locations.OrderBy(r => r.No).Where(a => a.Status == "Active").Select(r => new {
                Id = r.No,
                Text = r.No + " | " + r.List
            });
            ViewData["LocationNo"] = new SelectList(LocationId, "Id", "Text");

            var DispenserId = disp.Select(r => new {
                Id = r.No,
                Text = r.NewName
            });

            ViewData["DispenserId"] = new SelectList(DispenserId, "Id", "Text", model.DispenserCode);

            var lube = _context.LubeTrucks.OrderBy(r => r.No)
                 .Where(a => a.Status != "Deleted");


            var LubeTruckId = lube.Select(r => new {
                Id = r.No,
                Text = r.No + " | " + r.Description
            });

            ViewData["LubeTruckId"] = new SelectList(LubeTruckId, "Id", "Text", model.LubeTruckCode);

            var components = _context.Components.OrderBy(r => r.Description).Where(a => a.Status == "Active").Select(r => new {
                Id = r.Code,
                Text = r.Description
            });
            ViewData["components"] = new SelectList(components, "Id", "Text");


            return View("BSmartView", model);
        }
        public IActionResult getBSmartDataDetails(int? id)
        {

 
            try
            {


                var draw = Request.Form["draw"].FirstOrDefault();
                int start = Convert.ToInt32(Request.Form["start"]);
                var length = Convert.ToInt32(Request.Form["length"]);
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                //int pageSize = length != null ? Convert.ToInt32(length) : 0;
                //int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                string order = Request.Form["order[0][column]"].FirstOrDefault();
                string orderDir = Request.Form["order[0][dir]"];
                var ReqList = _context.BSmartEquipments.OrderBy(r=>r.EquipmentNo)
                  .Where(a => a.BSmartShiftId == id)

                  .Select(a => new
                  {
                      EquipmentName = a.Equipments == null ? "" : a.Equipments.No,               
                      SMR = a.SMR == null ? 0 : a.SMR,
                      LocationName = a.Locations==null?"":a.Locations.List,
                      a.CreatedDate,
                      a.Status,
                      a.Id,
                      a.EquipmentNo,
                      a.DriverIdNumber,
                      a.LocationNo,
                      a.OldId
                  });

                recordsTotal = ReqList.Count();

                string[] fields = new string[] { "CreatedDate", "EquipmentName", "LocationName", "SMR", "CreatedDate", "Status" };
                string searchByCol = "";
                for (var scol = 0; scol <= fields.Count(); scol++)
                {
                    string ColSearchValue = Request.Form["columns[" + scol + "][search][value]"].FirstOrDefault();
                    string Col = Request.Form["columns[" + scol + "][data]"];
                    if (!string.IsNullOrEmpty(ColSearchValue))
                    {
                        if (searchByCol == "")
                        {
                            searchByCol = Col + ".Contains(\"" + ColSearchValue + "\")";
                        }
                        else
                        {
                            searchByCol = searchByCol + " && " + Col + ".Contains(\"" + ColSearchValue + "\")";
                        }
                    }
                }
                if (!string.IsNullOrEmpty(searchByCol))
                {
                    ReqList = ReqList.Where(searchByCol);
                }


                int recFilter = ReqList.Count();


                //bool desc = false;
                //if (sortColumnDirection == "desc")
                //{
                //    desc = true;
                //}
//ReqList = ReqList.OrderBy(sortColumn + (desc ? " descending" : ""));
                var sortfield = fields[int.Parse(order)];
                ReqList = ReqList.OrderByField(sortfield, orderDir);

                ReqList = ReqList.Skip(start).Take(length);
                var data = ReqList.ToList();

                var jsonData = new
                {
                    draw = Convert.ToInt32(draw),
                    recordsTotal = recordsTotal,
                    recordsFiltered = recFilter,
                    data = ReqList.ToList(),
                };

                return new JsonResult(jsonData);
            }
            catch (Exception ex)
            {
                return Json(ex);
            }



        }
        public IActionResult getBSmartDataSubDetails(int id)
        {
            //string strFilter = "";
            try
            {

                var v = _context.BSmartItems
               .Where(a => a.BSmartEquipmentId == id)
              .Select(a => new
              {
                  a.Items.No,
                  ItemName = a.Items == null ? "" : a.Items.Description,
                  ComponentId = a.Components == null ? "" : a.Components.Code,
                  ComponentName = a.Components == null ? "" : a.Components.Description,
                  a.VolumeQty,
                  a.Id,
                  a.OldId

              });


                var model = new
                {
                    data = v.ToList()
                };




                return Json(model);
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }

        public async Task<JsonResult> TransferDataToDataEntry(int[] id)
        {
            string status = "";
            string message = "";
            foreach (int f in id)
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var refno = await _globalnterface.NextNoSeries("FuelOil");
                        if (string.IsNullOrEmpty(refno))
                        {
                            transaction.Rollback();
                            var model1 = new
                            {
                                status = "fail",
                                message = "Unable to generate No. Series."
                            };
                            return new JsonResult(model1);
                        }

                        var fvm = _context.BSmartShifts.FirstOrDefault(r => r.Id == f);
                        if (fvm != null)
                        {
                            var nSeries = await _context.NoSeries.FirstOrDefaultAsync(r => r.Code == "FuelOil");
                            if (nSeries != null)
                            {
                                nSeries.LastNoUsed = refno;
                                _context.Entry(nSeries).State = EntityState.Modified;
                            }

                            var fo = new FuelOil
                            {
                                ReferenceNo = refno,
                                Shift = fvm.Shift,
                                CreatedDate = fvm.CreatedDate,
                                CreatedBy = fvm.CreatedBy,
                                TransactionDate = fvm.TransactionDate,
                                DispenserCode = fvm.DispenserCode,
                                LubeTruckCode = fvm.LubeTruckCode,
                                SourceReferenceNo = fvm.ReferenceNo,
                                OriginalDate = fvm.OriginalDate,
                                GeneratedfromBSmart = true,
                                DispenserName = fvm.DispenserName
                            };

                            _context.Add(fo);
                            _context.SaveChanges();

                            fvm.SourceReferenceNo = refno;
                            fvm.FuelOilId = fo.Id;
                            _context.Entry(fvm).State = EntityState.Modified;

                            var _det = _context.BSmartEquipments.Where(r => r.BSmartShiftId == fvm.Id).ToList();
                            if (_det.Any())
                            {
                                foreach (var fv in _det)
                                {
                                    string refno2 = await _globalnterface.NextNoSeries("FOD Code");
                                    if (string.IsNullOrEmpty(refno2))
                                    {
                                        transaction.Rollback();
                                        var model1 = new
                                        {
                                            status = "fail",
                                            message = "Unable to generate No. Series."
                                        };
                                        return new JsonResult(model1);
                                    }

                                    FuelOilDetail fod = new FuelOilDetail
                                    {
                                        DetailNo = refno2,
                                        LocationNo = fv.LocationNo,
                                        EquipmentNo = fv.EquipmentNo,
                                        FuelOilId = fo.Id,
                                        CreatedDate = fv.CreatedDate,
                                        SMR = fv.SMR,
                                        DriverIdNumber = fv.DriverIdNumber,
                                        Signature = "SIGNED"
                                    };
                                    _context.Add(fod);
                                    _context.SaveChanges();

                                    var _bItems = _context.BSmartItems.Where(r => r.BSmartEquipmentId == fv.Id).ToList();
                                    if (_bItems.Any())
                                    {
                                        foreach (var det in _bItems)
                                        {
                                            var sub = new FuelOilSubDetail
                                            {
                                                ItemNo = det.ItemNo,
                                                ComponentCode = det.ComponentCode == "" ? null : det.ComponentCode,
                                                VolumeQty = det.VolumeQty,
                                                TimeInput = DateTime.Now,
                                                FuelOilDetailId = fod.Id
                                            };
                                            _context.Add(sub);
                                        }
                                        _context.SaveChanges();
                                    }
                                }
                            }

                            transaction.Commit();
                        }

    
                        status = "OK";
                        message = "Record has been saved.";
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        status = "NOT OK";
                        message = e.Message;
                    }
                }
            }




            var model = new
            {
                status,
                message
            };

            return Json(model);
        }

        public IActionResult getDataReferenceNo2(int? id)
        {


            try
            {


                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                string status = "Posted,Partially Transferred";
                string[] stat = status.Split(',').Select(n => n).ToArray();


                var ReqList = _context.FuelOils.Include(r=>r.Dispensers).Include(r=>r.LubeTrucks)
                    .Where(y => y.Status == "Posted" || y.Status == "Partially Transferred")
                    .Select(
                    a => new
                    {

                        a.Id,
                        fuelStatus = a.Status,
                        a.ReferenceNo,
                        DispenserCode = a.Dispensers.NewName == null ? a.Dispensers.Name: a.Dispensers.NewName + " / " + a.LubeTruckCode
                        
                    });

                recordsTotal = ReqList.Count();

                if (!string.IsNullOrEmpty(searchValue))
                {
                    ReqList = ReqList.Where(r=>r.ReferenceNo == searchValue || r.DispenserCode.Contains(searchValue));
                }


                int recFilter = ReqList.Count();


                bool desc = false;
                if (sortColumnDirection == "desc")
                {
                    desc = true;
                }
                ReqList = ReqList.OrderBy(sortColumn + (desc ? " descending" : ""));



                if (pageSize < 0)
                {
                    pageSize = recordsTotal;
                }

                var data = ReqList.ToList();
                var jsonData = new { draw, recordsFiltered = recFilter, recordsTotal, data };
                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }

        public IActionResult BSmartSynchronizationLogs()
        {
            var d = _context.BSmartErrors.OrderByDescending(r=>r.ErrorDate).Take(90).ToList();
            return View(d);
        }

        #region Manual Sync BSMART
        public JsonResult GetBizMartData()
        {
            try
            {
                // System.IO.StreamWriter wShift = new System.IO.StreamWriter(@"D:\BSmartShiftWOEq.txt");

                // Console.WriteLine("Connecting to BSmart....");
               
                ServicePointManager.ServerCertificateValidationCallback = (s, c, ch, ssl) =>
                {
                    return true;
                };


                //int bShiftId = db.BSmartShifts.Select(p => p.OldId).DefaultIfEmpty(0).Max();

               
                List<Shift> shifts = new List<Shift>();
                WebRequest reqh = WebRequest.Create(@"https://203.177.41.210/ShiftsByFlag?token=ef4372e23ce89967801809d2d6569c4b3719ef3bbe4e73866eb0712932f8693d&flag=0");
                reqh.Method = "GET";
                reqh.Timeout = Timeout.Infinite;
                reqh.Credentials = new NetworkCredential("bsmartfodl", "gu1@72%1A3Pb");
                HttpWebResponse responseObjh = null;
                responseObjh = (HttpWebResponse)reqh.GetResponse();
                string strresultH = null;
                using (Stream stream = responseObjh.GetResponseStream())
                {
                    StreamReader sr = new StreamReader(stream);
                    strresultH = sr.ReadToEnd();

                    sr.Close();

                    shifts = (List<Shift>)JsonConvert.DeserializeObject(strresultH, typeof(List<Shift>));
                }

                //var BSmartEquipmentId = db.BSmartEquipments.Select(p => p.OldId).DefaultIfEmpty(0).Max();

                //Console.WriteLine("Collecting all BSmartEquipments Data....");
                List<BSEquipment> equipment;
                WebRequest req2 = WebRequest.Create(@"https://203.177.41.210/EquipmentsByFlag?token=ef4372e23ce89967801809d2d6569c4b3719ef3bbe4e73866eb0712932f8693d&flag=0");
                req2.Method = "GET";
                reqh.Timeout = Timeout.Infinite;
                req2.Credentials = new NetworkCredential("bsmartfodl", "gu1@72%1A3Pb");
                HttpWebResponse responseObj2 = null;
                responseObj2 = (HttpWebResponse)req2.GetResponse();
                string strresult2 = null;
                using (Stream stream = responseObj2.GetResponseStream())
                {
                    StreamReader sr = new StreamReader(stream);
                    strresult2 = sr.ReadToEnd();

                    sr.Close();

                    equipment = (List<BSEquipment>)JsonConvert.DeserializeObject(strresult2, typeof(List<BSEquipment>));
                }

                //var vc = equipment.FirstOrDefault(r => r.id == 60763);

                // var BSmartItemsId = db.BSmartItems.Select(p => p.OldId).DefaultIfEmpty(0).Max();

                // Console.WriteLine("Collecting all BSmartItems Data....");
                List<BSItem> items;
                WebRequest req = WebRequest.Create(@"https://203.177.41.210/ItemsByFlag?token=ef4372e23ce89967801809d2d6569c4b3719ef3bbe4e73866eb0712932f8693d&flag=0");
                req.Method = "GET";
                req.Credentials = new NetworkCredential("bsmartfodl", "gu1@72%1A3Pb");
                HttpWebResponse responseObj = null;
                reqh.Timeout = Timeout.Infinite;
                responseObj = (HttpWebResponse)req.GetResponse();
                string strresult = null;
                using (Stream stream = responseObj.GetResponseStream())
                {
                    StreamReader sr = new StreamReader(stream);
                    strresult = sr.ReadToEnd();

                    sr.Close();

                    items = (List<BSItem>)JsonConvert.DeserializeObject(strresult, typeof(List<BSItem>));
                }
                // var vcx = items.FirstOrDefault(r => r.id == 60763);

                bool noAdded = true;
                if (shifts != null && shifts.Count() > 0)
                {
                    shifts = shifts.OrderBy(r => r.id).Where(r => r.referenceNo != null).ToList();
                    foreach (var f in shifts)
                    {
                        //errorMessage = "Collecting all BSmart Data....";
                        var bShift = _context.BSmartShifts.FirstOrDefault(r => r.ReferenceNo == f.referenceNo);

                        if (bShift == null)
                        {
                            //using (var transaction = db.Database.BeginTransaction())
                            //{
                            //try
                            //{
                            var dis = _context.Dispensers.FirstOrDefault(r => r.No == f.dispenserCode);
                            var LUB = _context.LubeTrucks.FirstOrDefault(r => r.No == f.lubeTruckCode);

                            //DETAILS
                            int Line = 1;
                            var eq = equipment.OrderBy(r => r.id).Where(r => r.referenceNo == f.referenceNo).ToList();

                            //var _itemCheck = items.OrderBy(r => r.id).Where(r => r.referenceNo == f.referenceNo).ToList();
                            if (eq != null)
                            {
                                noAdded = false;
                                // Console.WriteLine("Saving Fuel Oil No = " + f.referenceNo);

                                var Bfo = new BSmartShift
                                {
                                    ReferenceNo = f.referenceNo,
                                    Shift = myTI.ToTitleCase(f.shift.ToLower()),
                                    CreatedDate = f.createdDate,
                                    CreatedBy = f.createdBy,
                                    TransactionDate = f.transactionDate,
                                    DispenserCode = dis == null ? "N/A" : f.dispenserCode,
                                    DispenserName = dis == null ? "" : dis.NewName,
                                    LubeTruckCode = LUB == null ? "N/A" : f.lubeTruckCode,
                                    OriginalDate = f.originalDate,
                                    TransferDate = DateTime.Now,
                                    OldId = f.id,
                                    OriginalReferenceNo = f.referenceNo
                                };

                                _context.Entry(Bfo).State = EntityState.Added;
                                _context.SaveChanges();

                                //   Console.WriteLine("Updating BSMART Shift" + f.referenceNo);

                                //UPDATE BSMART
                                WebRequest ShiftReq = WebRequest.Create(@"https://203.177.41.210/ShiftsByFlag?token=ef4372e23ce89967801809d2d6569c4b3719ef3bbe4e73866eb0712932f8693d&rowID=" + f.id + "&flag=1");
                                ShiftReq.Method = "POST";
                                ShiftReq.Credentials = new NetworkCredential("bsmartfodl", "gu1@72%1A3Pb");
                                Stream requestStream = ShiftReq.GetRequestStream();
                                requestStream.Close();

                                ShiftReq.GetResponseAsync();
                                //end update shift

                                foreach (var equip in eq)
                                {


                                    var _item = items.OrderBy(r => r.id).Where(r => r.detailNo == equip.detailNo && r.referenceNo == equip.referenceNo).ToList();
                                    if (_item != null && _item.Count() > 0)
                                    {
                                        //   Console.WriteLine("Saving Fuel Oil Details DetailNo = " + equip.detailNo);
                                        //DOWNLOAD COPY
                                        var jEq = _context.Equipments.FirstOrDefault(r => r.No == equip.equipmentNo);
                                        var jdriver = _context.Drivers.FirstOrDefault(r => r.IdNumber == equip.driverIdNumber);

                                        var Bfod = new BSmartEquipment
                                        {
                                            DetailNo = equip.detailNo,
                                            LocationNo = dis == null ? "N/A" : dis.LocationCode,
                                            EquipmentNo = equip.equipmentNo == "IBUTTON" ? "IBUTTON" : jEq == null ? "001" : equip.equipmentNo,
                                            BSmartShiftId = Bfo.Id,
                                            CreatedDate = equip.createdDate.GetValueOrDefault(),
                                            DriverIdNumber = jdriver == null ? "0000000" : equip.driverIdNumber,
                                            OldId = equip.id
                                        };
                                        _context.Entry(Bfod).State = EntityState.Added;
                                        _context.SaveChanges();

                                        //  Console.WriteLine("Updating BSMART Equipment" + equip.detailNo);

                                        //UPDATE BSMART EQUIPMENT
                                        WebRequest EQReq = WebRequest.Create(@"https://203.177.41.210/EquipmentsByFlag?token=ef4372e23ce89967801809d2d6569c4b3719ef3bbe4e73866eb0712932f8693d&rowID=" + equip.id + "&flag=1");
                                        EQReq.Method = "POST";
                                        EQReq.Credentials = new NetworkCredential("bsmartfodl", "gu1@72%1A3Pb");
                                        Stream EQStream = EQReq.GetRequestStream();
                                        EQStream.Close();

                                        EQReq.GetResponseAsync();
                                        //END UPDATE BSMART EQUIPMENT

                                        Line++;

                                        foreach (var i in _item)
                                        {
                                            //  Console.WriteLine("Saving Fuel Oil Details Item ItemNo = " + i.itemNo);
                                            //DOWNLOAD COPY
                                            var jItem = _context.Items.FirstOrDefault(r => r.No == i.itemNo);
                                            var jcOMPcODE = _context.Components.FirstOrDefault(r => r.Code == i.componentCode);
                                            var Bsub = new BSmartItem
                                            {
                                                ItemNo = jItem == null ? "N/A" : i.itemNo,
                                                ComponentCode = jcOMPcODE == null ? "N/A" : i.componentCode,
                                                VolumeQty = i.quantity,
                                                TimeInput = i.timeInput,
                                                BSmartEquipmentId = Bfod.Id,
                                                OldId = i.id
                                            };
                                            _context.Entry(Bsub).State = EntityState.Added;
                                            _context.SaveChanges();

                                            //Console.WriteLine("Updating BSMART Item" + i.itemNo);
                                            //UPDATE BSMART ITEM
                                            WebRequest iTEMReq = WebRequest.Create(@"https://203.177.41.210/ItemsByFlag?token=ef4372e23ce89967801809d2d6569c4b3719ef3bbe4e73866eb0712932f8693d&rowID=" + i.id + "&flag=1");
                                            iTEMReq.Method = "POST";
                                            iTEMReq.Credentials = new NetworkCredential("bsmartfodl", "gu1@72%1A3Pb");
                                            Stream iTEMStream = iTEMReq.GetRequestStream();
                                            iTEMStream.Close();

                                            iTEMReq.GetResponseAsync();
                                            //END UPDATE BSMART ITEM
                                        }
                                    }
                                    else
                                    {
                                        // Console.WriteLine("No Item for  Fuel Oil Details DetailNo = " + equip.detailNo);
                                    }
                                }

                                // transaction.Commit();
                            }
                        }
                        else
                        {
                            //Console.WriteLine("Updating Existing BSMART Shift" + f.referenceNo);
                            //UPDATE BSMART
                            WebRequest ShiftReq = WebRequest.Create(@"https://203.177.41.210/ShiftsByFlag?token=ef4372e23ce89967801809d2d6569c4b3719ef3bbe4e73866eb0712932f8693d&rowID=" + f.id + "&flag=1");
                            ShiftReq.Method = "POST";
                            ShiftReq.Credentials = new NetworkCredential("bsmartfodl", "gu1@72%1A3Pb");
                            Stream requestStream = ShiftReq.GetRequestStream();
                            requestStream.Close();

                            ShiftReq.GetResponse();

                            //end update shift

                            CreateNewRef(equipment.Where(r => r.referenceNo == f.referenceNo).ToList(), items.Where(r => r.referenceNo == f.referenceNo).ToList());

                        }
                    }

                    if (noAdded == true)
                    {
                        errorMessage = "No header data to collect";
                        CreateNewRef(equipment, items);
                    }
                }
                else
                {
                    errorMessage = "No header data to collect";
                    CreateNewRef(equipment, items);
                }

                var bsmartE = new BSmartError
                {
                    ErrorDate = DateTime.Now,
                    ErrMessage = "Manually triggered"
                };
                _context.Entry(bsmartE).State = EntityState.Added;
                _context.SaveChanges();

                errorMessage = "Task Completed";

                var model = new
                {

                    message = errorMessage,
                    status = "OK"
                };

                return Json(model);
            }
            catch (Exception x)
            {
                string err = x.InnerException == null ? x.Message : x.InnerException.InnerException == null ? x.InnerException.Message : x.InnerException.InnerException.Message;
                errorMessage = err;

                var model = new
                {

                    message = err,
                    status ="NOT OK"
                };

                return Json(model);
            }
        }

        [AllowAnonymous]
        public IActionResult GetBizMartData1()
        {
            int jId = 0;
            try
            {
                // System.IO.StreamWriter wShift = new System.IO.StreamWriter(@"D:\BSmartShiftWOEq.txt");

                // Console.WriteLine("Connecting to BSmart....");

                ServicePointManager.ServerCertificateValidationCallback = (s, c, ch, ssl) =>
                {
                    return true;
                };

                var bsmartE = new BSmartError
                {
                    ErrorDate = DateTime.Now,
                    ErrMessage = "Connecting BSMART API"
                };
                _context.Entry(bsmartE).State = EntityState.Added;
                _context.SaveChanges();

                jId = bsmartE.Id;
                //int bShiftId = db.BSmartShifts.Select(p => p.OldId).DefaultIfEmpty(0).Max();


                List<Shift> shifts = new List<Shift>();
                WebRequest reqh = WebRequest.Create(@"https://203.177.41.210/ShiftsByFlag?token=ef4372e23ce89967801809d2d6569c4b3719ef3bbe4e73866eb0712932f8693d&flag=0");
                reqh.Method = "GET";
                reqh.Timeout = Timeout.Infinite;
                reqh.Credentials = new NetworkCredential("bsmartfodl", "gu1@72%1A3Pb");
                HttpWebResponse responseObjh = null;
                responseObjh = (HttpWebResponse)reqh.GetResponse();
                string strresultH = null;
                using (Stream stream = responseObjh.GetResponseStream())
                {
                    StreamReader sr = new StreamReader(stream);
                    strresultH = sr.ReadToEnd();

                    sr.Close();

                    shifts = (List<Shift>)JsonConvert.DeserializeObject(strresultH, typeof(List<Shift>));
                }

                //var BSmartEquipmentId = db.BSmartEquipments.Select(p => p.OldId).DefaultIfEmpty(0).Max();

                //Console.WriteLine("Collecting all BSmartEquipments Data....");
                List<BSEquipment> equipment;
                WebRequest req2 = WebRequest.Create(@"https://203.177.41.210/EquipmentsByFlag?token=ef4372e23ce89967801809d2d6569c4b3719ef3bbe4e73866eb0712932f8693d&flag=0");
                req2.Method = "GET";
                reqh.Timeout = Timeout.Infinite;
                req2.Credentials = new NetworkCredential("bsmartfodl", "gu1@72%1A3Pb");
                HttpWebResponse responseObj2 = null;
                responseObj2 = (HttpWebResponse)req2.GetResponse();
                string strresult2 = null;
                using (Stream stream = responseObj2.GetResponseStream())
                {
                    StreamReader sr = new StreamReader(stream);
                    strresult2 = sr.ReadToEnd();

                    sr.Close();

                    equipment = (List<BSEquipment>)JsonConvert.DeserializeObject(strresult2, typeof(List<BSEquipment>));
                }

                //var vc = equipment.FirstOrDefault(r => r.id == 60763);

                // var BSmartItemsId = db.BSmartItems.Select(p => p.OldId).DefaultIfEmpty(0).Max();

                // Console.WriteLine("Collecting all BSmartItems Data....");
                List<BSItem> items;
                WebRequest req = WebRequest.Create(@"https://203.177.41.210/ItemsByFlag?token=ef4372e23ce89967801809d2d6569c4b3719ef3bbe4e73866eb0712932f8693d&flag=0");
                req.Method = "GET";
                req.Credentials = new NetworkCredential("bsmartfodl", "gu1@72%1A3Pb");
                HttpWebResponse responseObj = null;
                reqh.Timeout = Timeout.Infinite;
                responseObj = (HttpWebResponse)req.GetResponse();
                string strresult = null;
                using (Stream stream = responseObj.GetResponseStream())
                {
                    StreamReader sr = new StreamReader(stream);
                    strresult = sr.ReadToEnd();

                    sr.Close();

                    items = (List<BSItem>)JsonConvert.DeserializeObject(strresult, typeof(List<BSItem>));
                }
                // var vcx = items.FirstOrDefault(r => r.id == 60763);

                bool noAdded = true;
                if (shifts != null && shifts.Count() > 0)
                {
                    shifts = shifts.OrderBy(r => r.id).Where(r => r.referenceNo != null).ToList();
                    foreach (var f in shifts)
                    {
                        //errorMessage = "Collecting all BSmart Data....";
                        var bShift = _context.BSmartShifts.FirstOrDefault(r => r.ReferenceNo == f.referenceNo);

                        if (bShift == null)
                        {
                            //using (var transaction = db.Database.BeginTransaction())
                            //{
                            //try
                            //{
                            var dis = _context.Dispensers.FirstOrDefault(r => r.No == f.dispenserCode);
                            var LUB = _context.LubeTrucks.FirstOrDefault(r => r.No == f.lubeTruckCode);

                            //DETAILS
                            int Line = 1;
                            var eq = equipment.OrderBy(r => r.id).Where(r => r.referenceNo == f.referenceNo).ToList();

                            //var _itemCheck = items.OrderBy(r => r.id).Where(r => r.referenceNo == f.referenceNo).ToList();
                            if (eq != null)
                            {
                                noAdded = false;
                                // Console.WriteLine("Saving Fuel Oil No = " + f.referenceNo);

                                var Bfo = new BSmartShift
                                {
                                    ReferenceNo = f.referenceNo,
                                    Shift = myTI.ToTitleCase(f.shift.ToLower()),
                                    CreatedDate = f.createdDate,
                                    CreatedBy = f.createdBy,
                                    TransactionDate = f.transactionDate,
                                    DispenserCode = dis == null ? "N/A" : f.dispenserCode,
                                    DispenserName = dis == null ? "" : dis.NewName,
                                    LubeTruckCode = LUB == null ? "N/A" : f.lubeTruckCode,
                                    OriginalDate = f.originalDate,
                                    TransferDate = DateTime.Now,
                                    OldId = f.id,
                                    OriginalReferenceNo = f.referenceNo
                                };

                                _context.Entry(Bfo).State = EntityState.Added;
                                _context.SaveChanges();

                                //   Console.WriteLine("Updating BSMART Shift" + f.referenceNo);

                                //UPDATE BSMART
                                WebRequest ShiftReq = WebRequest.Create(@"https://203.177.41.210/ShiftsByFlag?token=ef4372e23ce89967801809d2d6569c4b3719ef3bbe4e73866eb0712932f8693d&rowID=" + f.id + "&flag=1");
                                ShiftReq.Method = "POST";
                                ShiftReq.Credentials = new NetworkCredential("bsmartfodl", "gu1@72%1A3Pb");
                                Stream requestStream = ShiftReq.GetRequestStream();
                                requestStream.Close();

                                ShiftReq.GetResponseAsync();
                                //end update shift

                                foreach (var equip in eq)
                                {


                                    var _item = items.OrderBy(r => r.id).Where(r => r.detailNo == equip.detailNo && r.referenceNo == equip.referenceNo).ToList();
                                    if (_item != null && _item.Count() > 0)
                                    {
                                        //   Console.WriteLine("Saving Fuel Oil Details DetailNo = " + equip.detailNo);
                                        //DOWNLOAD COPY
                                        var jEq = _context.Equipments.FirstOrDefault(r => r.No == equip.equipmentNo);
                                        var jdriver = _context.Drivers.FirstOrDefault(r => r.IdNumber == equip.driverIdNumber);

                                        var Bfod = new BSmartEquipment
                                        {
                                            DetailNo = equip.detailNo,
                                            LocationNo = dis == null ? "N/A" : dis.LocationCode,
                                            EquipmentNo = equip.equipmentNo == "IBUTTON" ? "IBUTTON" : jEq == null ? "001" : equip.equipmentNo,
                                            BSmartShiftId = Bfo.Id,
                                            CreatedDate = equip.createdDate.GetValueOrDefault(),
                                            DriverIdNumber = jdriver == null ? "0000000" : equip.driverIdNumber,
                                            OldId = equip.id
                                        };
                                        _context.Entry(Bfod).State = EntityState.Added;
                                        _context.SaveChanges();

                                        //  Console.WriteLine("Updating BSMART Equipment" + equip.detailNo);

                                        //UPDATE BSMART EQUIPMENT
                                        WebRequest EQReq = WebRequest.Create(@"https://203.177.41.210/EquipmentsByFlag?token=ef4372e23ce89967801809d2d6569c4b3719ef3bbe4e73866eb0712932f8693d&rowID=" + equip.id + "&flag=1");
                                        EQReq.Method = "POST";
                                        EQReq.Credentials = new NetworkCredential("bsmartfodl", "gu1@72%1A3Pb");
                                        Stream EQStream = EQReq.GetRequestStream();
                                        EQStream.Close();

                                        EQReq.GetResponseAsync();
                                        //END UPDATE BSMART EQUIPMENT

                                        Line++;

                                        foreach (var i in _item)
                                        {
                                            //  Console.WriteLine("Saving Fuel Oil Details Item ItemNo = " + i.itemNo);
                                            //DOWNLOAD COPY
                                            var jItem = _context.Items.FirstOrDefault(r => r.No == i.itemNo);
                                            var jcOMPcODE = _context.Components.FirstOrDefault(r => r.Code == i.componentCode);
                                            var Bsub = new BSmartItem
                                            {
                                                ItemNo = jItem == null ? "N/A" : i.itemNo,
                                                ComponentCode = jcOMPcODE == null ? "N/A" : i.componentCode,
                                                VolumeQty = i.quantity,
                                                TimeInput = i.timeInput,
                                                BSmartEquipmentId = Bfod.Id,
                                                OldId = i.id
                                            };
                                            _context.Entry(Bsub).State = EntityState.Added;
                                            _context.SaveChanges();

                                            //Console.WriteLine("Updating BSMART Item" + i.itemNo);
                                            //UPDATE BSMART ITEM
                                            WebRequest iTEMReq = WebRequest.Create(@"https://203.177.41.210/ItemsByFlag?token=ef4372e23ce89967801809d2d6569c4b3719ef3bbe4e73866eb0712932f8693d&rowID=" + i.id + "&flag=1");
                                            iTEMReq.Method = "POST";
                                            iTEMReq.Credentials = new NetworkCredential("bsmartfodl", "gu1@72%1A3Pb");
                                            Stream iTEMStream = iTEMReq.GetRequestStream();
                                            iTEMStream.Close();

                                            iTEMReq.GetResponseAsync();
                                            //END UPDATE BSMART ITEM
                                        }
                                    }
                                    else
                                    {
                                        // Console.WriteLine("No Item for  Fuel Oil Details DetailNo = " + equip.detailNo);
                                    }
                                }

                                // transaction.Commit();
                            }
                        }
                        else
                        {
                            //Console.WriteLine("Updating Existing BSMART Shift" + f.referenceNo);
                            //UPDATE BSMART
                            WebRequest ShiftReq = WebRequest.Create(@"https://203.177.41.210/ShiftsByFlag?token=ef4372e23ce89967801809d2d6569c4b3719ef3bbe4e73866eb0712932f8693d&rowID=" + f.id + "&flag=1");
                            ShiftReq.Method = "POST";
                            ShiftReq.Credentials = new NetworkCredential("bsmartfodl", "gu1@72%1A3Pb");
                            Stream requestStream = ShiftReq.GetRequestStream();
                            requestStream.Close();

                            ShiftReq.GetResponse();

                            //end update shift

                            CreateNewRef(equipment.Where(r => r.referenceNo == f.referenceNo).ToList(), items.Where(r => r.referenceNo == f.referenceNo).ToList());

                        }
                    }

                    if (noAdded == true)
                    {
                        errorMessage = "No header data to collect";
                        CreateNewRef(equipment, items);
                    }
                }
                else
                {
                    errorMessage = "No header data to collect";
                    CreateNewRef(equipment, items);
                }


                var jErr = _context.BSmartErrors.FirstOrDefault(r => r.Id == jId);
                if (jErr != null)
                {
                    jErr.ErrMessage = "Task Completed";
                    jErr.ErrorDate = DateTime.Now;
                    _context.Entry(jErr).State = EntityState.Modified;
                    _context.SaveChanges();
                }
                errorMessage = "Task Completed";

                var model = new
                {

                    message = errorMessage,
                    status = "OK"
                };

                return Json(model);
            }
            catch (Exception x)
            {
                string err = x.InnerException == null ? x.Message : x.InnerException.InnerException == null ? x.InnerException.Message : x.InnerException.InnerException.Message;
                errorMessage = err;

                var jErr = _context.BSmartErrors.FirstOrDefault(r=>r.Id==jId);
                if (jErr != null)
                {
                    jErr.ErrMessage = errorMessage;
                    jErr.ErrorDate = DateTime.Now;
                    _context.Entry(jErr).State = EntityState.Modified;
                    _context.SaveChanges();
                }
                var model = new
                {

                    message = err,
                    status = "NOT OK"
                };

                return Json(model);
            }
        }
        private void CreateNewRef(List<BSEquipment> equipment, List<BSItem> items)
        {
            //DETAILS
            //System.IO.StreamWriter wEq = new System.IO.StreamWriter(@"D:\BSmartShiftWOEq.txt");
            int Line = 1;
            // int JRV = 0;
            var eq = equipment.ToList();
            if (eq != null && eq.Count() > 0 && items.Count() > 0)
            {
                //errorMessage = "Collecting all BSmart Data....";
                foreach (var equip in eq)
                {
                    var TblEquip = _context.BSmartEquipments.FirstOrDefault(r => r.OldId == equip.id);
                    var _item = items.Where(r => r.detailNo == equip.detailNo && r.referenceNo == equip.referenceNo).ToList();
                    if (TblEquip == null && (_item != null && _item.Count() > 0))
                    {
                        var bShift = _context.BSmartShifts.OrderByDescending(r => r.Id).Where(r => r.OriginalReferenceNo == equip.referenceNo).FirstOrDefault();
                        if (bShift != null)
                        {
                            var dataCount = _context.BSmartShifts.Count(r => r.OldId == bShift.OldId) + 1;

                            var dis = _context.Dispensers.FirstOrDefault(r => r.No == bShift.DispenserCode);
                            //DOWNLOAD COPY
                            if (bShift.FuelOilId > 0)
                            {
                                //Console.WriteLine("Saving Fuel Oil No_ = " + equip.referenceNo);
                                bShift = new BSmartShift
                                {
                                    ReferenceNo = bShift.ReferenceNo + "-" + dataCount,
                                    Shift = bShift.Shift,
                                    CreatedDate = bShift.CreatedDate,
                                    CreatedBy = bShift.CreatedBy,
                                    TransactionDate = bShift.TransactionDate,
                                    DispenserCode = bShift.DispenserCode,
                                    DispenserName = dis == null ? "" : dis.NewName,
                                    LubeTruckCode = bShift.LubeTruckCode,
                                    OriginalDate = bShift.OriginalDate,
                                    TransferDate = DateTime.Now,
                                    OldId = bShift.OldId,
                                    OriginalReferenceNo = bShift.ReferenceNo
                                };

                                _context.Entry(bShift).State = EntityState.Added;
                                _context.SaveChanges();
                            }

                            //Console.WriteLine("Saving Fuel Oil Details DetailNo = " + equip.detailNo);

                            //DOWNLOAD COPY
                            var jEq = _context.Equipments.FirstOrDefault(r => r.No == equip.equipmentNo);
                            var jdriver = _context.Drivers.FirstOrDefault(r => r.IdNumber == equip.driverIdNumber);
                            // var jLoc = db.Locations.FirstOrDefault(r => r.No == equip.locationNo);
                            var Bfod = new BSmartEquipment
                            {
                                DetailNo = equip.detailNo,
                                LocationNo = dis == null ? "N/A" : dis.LocationCode,
                                EquipmentNo = equip.equipmentNo == "IBUTTON" ? "IBUTTON" : jEq == null ? "001" : equip.equipmentNo,
                                BSmartShiftId = bShift.Id,
                                CreatedDate = equip.createdDate.GetValueOrDefault(),
                                DriverIdNumber = jdriver == null ? "0000000" : equip.driverIdNumber,
                                OldId = equip.id
                            };
                            _context.Entry(Bfod).State = EntityState.Added;
                            _context.SaveChanges();

                            //UPDATE BSMART EQUIPMENT
                            //Console.WriteLine("Updating BSMART Equipment" + equip.detailNo);
                            WebRequest EQReq = WebRequest.Create(@"https://203.177.41.210/EquipmentsByFlag?token=ef4372e23ce89967801809d2d6569c4b3719ef3bbe4e73866eb0712932f8693d&rowID=" + equip.id + "&flag=1");
                            EQReq.Method = "POST";
                            EQReq.Credentials = new NetworkCredential("bsmartfodl", "gu1@72%1A3Pb");
                            Stream EQStream = EQReq.GetRequestStream();
                            EQStream.Close();

                            EQReq.GetResponse();
                            //END UPDATE BSMART EQUIPMENT

                            Line++;


                            if (_item != null && _item.Count() > 0)
                            {
                                foreach (var i in _item)
                                {
                                    var tblItem = _context.BSmartItems.FirstOrDefault(r => r.OldId == i.id);
                                    if (tblItem == null)
                                    {
                                        // Console.WriteLine("Saving Fuel Oil Details Item ItemNo = " + i.itemNo);
                                        //DOWNLOAD COPY
                                        var jItem = _context.Items.FirstOrDefault(r => r.No == i.itemNo);
                                        var jcOMPcODE = _context.Components.FirstOrDefault(r => r.Code == i.componentCode);
                                        var Bsub = new BSmartItem
                                        {
                                            ItemNo = jItem == null ? "N/A" : i.itemNo,
                                            ComponentCode = jcOMPcODE == null ? "N/A" : i.componentCode,
                                            VolumeQty = i.quantity,
                                            TimeInput = i.timeInput,
                                            BSmartEquipmentId = Bfod.Id,
                                            OldId = i.id
                                        };
                                        _context.Entry(Bsub).State = EntityState.Added;
                                        _context.SaveChanges();

                                        //Console.WriteLine("Updating BSMART Item" + i.itemNo);
                                        //UPDATE BSMART ITEM
                                        WebRequest iTEMReq = WebRequest.Create(@"https://203.177.41.210/ItemsByFlag?token=ef4372e23ce89967801809d2d6569c4b3719ef3bbe4e73866eb0712932f8693d&rowID=" + i.id + "&flag=1");
                                        iTEMReq.Method = "POST";
                                        iTEMReq.Credentials = new NetworkCredential("bsmartfodl", "gu1@72%1A3Pb");
                                        Stream iTEMStream = iTEMReq.GetRequestStream();
                                        iTEMStream.Close();

                                        iTEMReq.GetResponse();
                                        //END UPDATE BSMART ITEM
                                    }
                                    else
                                    {
                                        //Console.WriteLine("Updating Existing BSMART Item" + i.itemNo);
                                        //UPDATE BSMART ITEM
                                        WebRequest iTEMReq = WebRequest.Create(@"https://203.177.41.210/ItemsByFlag?token=ef4372e23ce89967801809d2d6569c4b3719ef3bbe4e73866eb0712932f8693d&rowID=" + i.id + "&flag=1");
                                        iTEMReq.Method = "POST";
                                        iTEMReq.Credentials = new NetworkCredential("bsmartfodl", "gu1@72%1A3Pb");
                                        Stream iTEMStream = iTEMReq.GetRequestStream();
                                        iTEMStream.Close();

                                        iTEMReq.GetResponse();
                                        //END UPDATE BSMART ITEM
                                        // Console.WriteLine("Existing ItemNo = " + i.itemNo);
                                    }
                                }
                            }

                            //  transaction.Commit();

                        }
                    }
                    else
                    {
                        //      JRV= JRV+1;
                        // Console.WriteLine("TOTAL EQUIPMENT" + JRV);
                        //UPDATE BSMART EQUIPMENT
                        // Console.WriteLine("Updating Existing BSMART Equipment" + equip.detailNo);
                        WebRequest EQReq = WebRequest.Create(@"https://203.177.41.210/EquipmentsByFlag?token=ef4372e23ce89967801809d2d6569c4b3719ef3bbe4e73866eb0712932f8693d&rowID=" + equip.id + "&flag=1");
                        EQReq.Method = "POST";
                        EQReq.Credentials = new NetworkCredential("bsmartfodl", "gu1@72%1A3Pb");
                        Stream EQStream = EQReq.GetRequestStream();
                        EQStream.Close();

                        EQReq.GetResponse();
                        //END UPDATE BSMART EQUIPMENT

                        if (_item != null && _item.Count() > 0)
                        {
                            foreach (var i in _item)
                            {
                                var tblItem = _context.BSmartItems.FirstOrDefault(r => r.OldId == i.id);
                                if (tblItem == null)
                                {
                                    // Console.WriteLine("Saving Fuel Oil Details Item ItemNo = " + i.itemNo);
                                    //DOWNLOAD COPY
                                    var jItem = _context.Items.FirstOrDefault(r => r.No == i.itemNo);
                                    var jcOMPcODE = _context.Components.FirstOrDefault(r => r.Code == i.componentCode);
                                    var Bsub = new BSmartItem
                                    {
                                        ItemNo = jItem == null ? "N/A" : i.itemNo,
                                        ComponentCode = jcOMPcODE == null ? "N/A" : i.componentCode,
                                        VolumeQty = i.quantity,
                                        TimeInput = i.timeInput,
                                        BSmartEquipmentId = equip.id,
                                        OldId = i.id
                                    };
                                    _context.Entry(Bsub).State = EntityState.Added;
                                    _context.SaveChanges();

                                    //  Console.WriteLine("Updating BSMART Item" + i.itemNo);
                                    //UPDATE BSMART ITEM
                                    WebRequest iTEMReq = WebRequest.Create(@"https://203.177.41.210/ItemsByFlag?token=ef4372e23ce89967801809d2d6569c4b3719ef3bbe4e73866eb0712932f8693d&rowID=" + i.id + "&flag=1");
                                    iTEMReq.Method = "POST";
                                    iTEMReq.Credentials = new NetworkCredential("bsmartfodl", "gu1@72%1A3Pb");
                                    Stream iTEMStream = iTEMReq.GetRequestStream();
                                    iTEMStream.Close();

                                    iTEMReq.GetResponse();
                                    //END UPDATE BSMART ITEM
                                }
                                else
                                {
                                    // Console.WriteLine("Updating Existing BSMART Item" + i.itemNo);
                                    //UPDATE BSMART ITEM
                                    WebRequest iTEMReq = WebRequest.Create(@"https://203.177.41.210/ItemsByFlag?token=ef4372e23ce89967801809d2d6569c4b3719ef3bbe4e73866eb0712932f8693d&rowID=" + i.id + "&flag=1");
                                    iTEMReq.Method = "POST";
                                    iTEMReq.Credentials = new NetworkCredential("bsmartfodl", "gu1@72%1A3Pb");
                                    Stream iTEMStream = iTEMReq.GetRequestStream();
                                    iTEMStream.Close();

                                    iTEMReq.GetResponse();
                                    //END UPDATE BSMART ITEM
                                    //Console.WriteLine("Existing ItemNo = " + i.itemNo);
                                }
                            }
                        }
                    }
                }
            }
            else if (items != null && items.Count() > 0)
            {
                foreach (var i in items)
                {
                    var tblItem = _context.BSmartItems.FirstOrDefault(r => r.OldId == i.id);
                    if (tblItem == null)
                    {
                        var bsEq = _context.BSmartEquipments.FirstOrDefault(r => r.DetailNo == i.detailNo);
                        if (bsEq != null)
                        {
                            //Console.WriteLine("Saving Fuel Oil Details Item ItemNo = " + i.itemNo);
                            //DOWNLOAD COPY
                            var jItem = _context.Items.FirstOrDefault(r => r.No == i.itemNo);
                            var jcOMPcODE = _context.Components.FirstOrDefault(r => r.Code == i.componentCode);
                            var Bsub = new BSmartItem
                            {
                                ItemNo = jItem == null ? "N/A" : i.itemNo,
                                ComponentCode = jcOMPcODE == null ? "N/A" : i.componentCode,
                                VolumeQty = i.quantity,
                                TimeInput = i.timeInput,
                                BSmartEquipmentId = bsEq.Id,
                                OldId = i.id
                            };
                            _context.Entry(Bsub).State = EntityState.Added;
                            _context.SaveChanges();
                        }


                        //Console.WriteLine("Updating BSMART Item" + i.itemNo);
                        //UPDATE BSMART ITEM
                        WebRequest iTEMReq = WebRequest.Create(@"https://203.177.41.210/ItemsByFlag?token=ef4372e23ce89967801809d2d6569c4b3719ef3bbe4e73866eb0712932f8693d&rowID=" + i.id + "&flag=1");
                        iTEMReq.Method = "POST";
                        iTEMReq.Credentials = new NetworkCredential("bsmartfodl", "gu1@72%1A3Pb");
                        Stream iTEMStream = iTEMReq.GetRequestStream();
                        iTEMStream.Close();

                        iTEMReq.GetResponse();
                        //END UPDATE BSMART ITEM
                    }
                    else
                    {
                        //Console.WriteLine("Updating Existing BSMART Item" + i.itemNo);
                        //UPDATE BSMART ITEM
                        WebRequest iTEMReq = WebRequest.Create(@"https://203.177.41.210/ItemsByFlag?token=ef4372e23ce89967801809d2d6569c4b3719ef3bbe4e73866eb0712932f8693d&rowID=" + i.id + "&flag=1");
                        iTEMReq.Method = "POST";
                        iTEMReq.Credentials = new NetworkCredential("bsmartfodl", "gu1@72%1A3Pb");
                        Stream iTEMStream = iTEMReq.GetRequestStream();
                        iTEMStream.Close();

                        iTEMReq.GetResponse();
                        //END UPDATE BSMART ITEM
                        //Console.WriteLine("Existing ItemNo = " + i.itemNo);
                    }
                }
            }
        }
        #endregion
    }
}