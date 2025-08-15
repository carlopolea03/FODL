using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using DNTBreadCrumb.Core;
using FODLSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FODLSystem.Controllers
{
    public class DispensersController : Controller
    {
        private readonly FODLSystemContext _context;

        public DispensersController(FODLSystemContext context)
        {
            _context = context;
        }

        [BreadCrumb(Title = "Index", Order = 1, IgnoreAjaxRequests = true)]
        public IActionResult Index()
        {

            this.SetCurrentBreadCrumbTitle("Dispenser");
            ViewBag.LocationCode = new SelectList(_context.Locations.Where(r => r.Status == "Active"), "No", "List");
            return View();
        }
        public JsonResult SearchDispenser(string q)
        {
            var model = _context.Dispensers
                .Where(a => a.Status != "Deleted")
                .Where(a => a.Name.ToUpper().Contains(q.ToUpper())).Select(b => new
                {
                    id = b.Id,
                    text =  b.Name,

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
        public ActionResult SaveItem(int id, string No, string Name, string NewName, string OfficeCode,string LocationCode,string status)
        {
            
            string message = "";
            try
            {

                var item = _context.Dispensers.FirstOrDefault(r=>r.No== No);
                if (item != null)
                {
                    item.No = No;
                    item.Name = Name;
                    item.NewName = NewName;
                    item.Status = status;
                    item.DateModified = DateTime.Now.Date;
                    item.LocationCode = LocationCode;
                    _context.Entry(item).State = EntityState.Modified;
                    _context.SaveChanges();
                }
                else
                {
                    var _dis = new Dispenser
                    {
                        No = No,
                        Name = Name,
                        NewName = NewName,
                        Status = "Active",
                        LocationCode = LocationCode
                };
                    _context.Entry(_dis).State = EntityState.Added;
                    _context.SaveChanges();
                }


                status = "success";
            }
            catch (Exception ex)
            {
                status = "fail";
                message = ex.Message;
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


                for (int i = 0; i < 3; i++)
                {
                    string colval = Request.Form["columns[" + i + "][search][value]"];
                    if (colval != "")
                    {
                        colval = colval.ToUpper();
                        string colSearch = Request.Form["columns[" + (i) + "][name]"];



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

                var v = _context.Dispensers.Include(r => r.Location)
                    
                    .Select(a => new
                    {
                    a.No,
                    a.Name,
                    a.NewName,
                    a.LocationCode,
                    LocationCodeName = a.Location == null ? "" : a.Location.List,
                    a.Status,
                    a.Id
                    });
                if (!string.IsNullOrEmpty(strFilter))
                {
                    v = v.Where(strFilter);
                }



                int recCount =v.Count();

                recordsTotal = recCount;
                int recFilter = recCount;





                //.OrderBy(a => a.FileDate).ThenBy(a => a.Hour)
                v = v.Skip(skip).Take(pageSize);
              




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
    }
}