
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.ServiceModel;
using System.Threading.Tasks;
using DNTBreadCrumb.Core;
using EQList;
using FODLSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FODLSystem.Controllers
{
    public class EquipmentsController : Controller
    {
        private readonly FODLSystemContext _context;

        public EquipmentsController(FODLSystemContext context)
        {
            _context = context;
        }

        [BreadCrumb(Title = "Index", Order = 1, IgnoreAjaxRequests = true)]
        public IActionResult Index()
        {
            this.SetCurrentBreadCrumbTitle("Equipment");
            return View();
        }
        public JsonResult SearchEquipment()
        {
            var model = _context.Equipments
                .Where(a => a.Status == "Active")
                //.Where(a => a.Name.ToUpper().Contains(q.ToUpper()) || a.No.ToUpper().Contains(q.ToUpper()))
                .Select(b => new
                {
                    id = b.Id,
                    text = b.No ,

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
        public ActionResult SaveItem(int id,string itemNo,string oldCode,string itemName, string itemModelNo, string DepartmentCode, string FuelCodeDiesel, string FuelCodeOil)
        {
            string status = "";
            string message = "";
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var item = _context.Equipments.FirstOrDefault(r => r.No == itemNo);
                    if (item == null)
                    {
                        item = new Equipment
                        {
                            No = itemNo,
                            Name = itemName,
                            ModelNo = itemModelNo,
                            DepartmentCode = DepartmentCode,
                            FuelCodeDiesel = FuelCodeDiesel,
                            FuelCodeOil = FuelCodeOil,
                            DateModified = DateTime.Now,
                            Status = "Active"
                    };

                        _context.Entry(item).State = EntityState.Added;
                        _context.SaveChanges();

                        if (!string.IsNullOrEmpty(oldCode))
                        {
                            var _FuelOilDetails = _context.FuelOilDetails.Where(r => r.EquipmentNo == oldCode).ToList();
                            if (_FuelOilDetails != null)
                            {
                                foreach(var det in _FuelOilDetails)
                                {
                                    det.EquipmentNo = itemNo;
                                    _context.Entry(det).State = EntityState.Modified;
                                }

                                var _eq = _context.Equipments.FirstOrDefault(r=>r.No==oldCode);
                                if (_eq != null)
                                {
                                    _eq.Status = "Inactive";
                                    _eq.DateModified = DateTime.Now;
                                    _context.Entry(_eq).State = EntityState.Modified;

                                }
                                else
                                {
                                    transaction.Rollback();
                                    status = "fail";
                                    message = "Old Code does not exist.";

                                    var model2 = new
                                    {
                                        status,
                                        message
                                    };
                                    return Json(model2);
                                }
                                _context.SaveChanges();
                            }
                        }

                    }
                    else
                    {
                        item.Name = itemName;
                        item.ModelNo = itemModelNo;
                        item.DepartmentCode = DepartmentCode;
                        item.FuelCodeDiesel = FuelCodeDiesel;
                        item.FuelCodeOil = FuelCodeOil;
                        item.DateModified = DateTime.Now;
                        item.Status = "Active";
                        _context.Entry(item).State =  EntityState.Modified;
                        _context.SaveChanges();
                    }


                    transaction.Commit();
                    status = "success";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    status = "fail";
                    message = ex.Message;
                }
            }
            var model = new
            {
                status,
                message
            };

            return Json(model);
        }

        [HttpPost]
        public ActionResult getData()
        {
            string strFilter = "";
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


                for (int i = 0; i < 6; i++)
                {
                    string colval = Request.Form["columns[" + i + "][search][value]"];
                    if (colval != "")
                    {
                        colval = colval.ToUpper();
                        string colSearch = Request.Form["columns[" + i + "][name]"];



                        if (strFilter == "")
                        {

                            strFilter = colSearch + ".Contains(" + "\"" + colval + "\"" + ")";

                        }
                        else
                        {
                            strFilter = strFilter + " && " + colSearch + ".Contains(" + "\"" + colval + "\"" + ")";
                        }

                    }
                }
                var v = _context.Equipments
                     .Select(a => new
                     {
                         a.No,
                         a.Name,
                         a.ModelNo,
                         a.Status,
                         a.DepartmentCode,
                         a.FuelCodeDiesel,
                         a.FuelCodeOil,
                         a.Id
                     });
                recordsTotal = v.Count();
                if (!string.IsNullOrEmpty(strFilter))
                {
                    v = v.Where(strFilter);
                }

          
                int recFilter = v.Count();



               

              //.OrderBy(a => a.FileDate).ThenBy(a => a.Hour)
               v=v.Skip(skip).Take(pageSize);




                bool desc = false;
                if (sortColumnDirection == "desc")
                {
                    desc = true;
                }
                v = v.OrderBy(sortColumn + (desc ? " descending" : ""));



                if (pageSize < 0)
                {
                    pageSize = recordsTotal;
                }


                var data = v;
                var jsonData = new { draw = draw, recordsFiltered = recFilter, recordsTotal = recordsTotal, data = data };
                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }

        public async Task<IActionResult> GetEquipment()
        {
            try
            {
                 string NAVUserName = Properties.Resources.ResourceManager.GetString("NavUser"); ;
                 string NAVPassword = Properties.Resources.ResourceManager.GetString("NavPass"); ;
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding(System.ServiceModel.BasicHttpSecurityMode.TransportCredentialOnly)
                {
                    MaxReceivedMessageSize = 10485760
                };

                binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;
                var endpoint = new EndpointAddress(new Uri("http://APRODITE.semiraramining.net:7057/BC130_SMPC_TEST/WS/Semirara/Page/EquipmentList"));

                EquipmentList_PortClient _PortClientSpare = new EquipmentList_PortClient(binding, endpoint);
                _PortClientSpare.ClientCredentials.UserName.UserName = NAVUserName;
                _PortClientSpare.ClientCredentials.UserName.Password = NAVPassword;

                List<EquipmentList_Filter> filterArray2 = new List<EquipmentList_Filter>();

                EQList.ReadMultiple_Result fncResult = new EQList.ReadMultiple_Result();
                fncResult = await _PortClientSpare.ReadMultipleAsync(filterArray2.ToArray(), null,10000);

                for (var i = 0; i < fncResult.ReadMultiple_Result1.Count(); i++)
                {
                    var _eq = _context.Equipments.OrderByDescending(r=>r.DateModified).FirstOrDefault(r => r.No == fncResult.ReadMultiple_Result1[i].No);
                    if (_eq != null)
                    {
                        if (fncResult.ReadMultiple_Result1[i].Blocked == true)
                        {
                            _eq.Status = "Inactive";
                        }
                        else
                        {
                            _eq.Status = "Active";
                        }

                        _eq.Name = fncResult.ReadMultiple_Result1[i].Name;
                        _eq.DepartmentCode = fncResult.ReadMultiple_Result1[i].Dept_Code;
                        _eq.ModelNo = fncResult.ReadMultiple_Result1[i].Model_No;
                        _eq.DateModified = DateTime.Now;
                        _context.Entry(_eq).State = EntityState.Modified;
                        _context.SaveChanges();

                    }
                    else
                    {
                        var eq = new Equipment
                        {
                            No = fncResult.ReadMultiple_Result1[i].No,
                            Name = fncResult.ReadMultiple_Result1[i].Name,
                            DepartmentCode = fncResult.ReadMultiple_Result1[i].Dept_Code,
                            ModelNo = fncResult.ReadMultiple_Result1[i].Model_No,
                            Status = "Active",
                            DateModified = DateTime.Now
                        };

                        _context.Entry(eq).State = EntityState.Added;
                        _context.SaveChanges();

                    }
                }


                var arr = new
                {
                    Status = "OK"
                };
                return new JsonResult(arr);
            }
            catch (Exception ex)
            {
                var arr = new
                {
                    Status = "NOT OK",
                    ex.Message
                };
                return new JsonResult(arr);
            }


        }
    }
}

