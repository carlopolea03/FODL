using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using DNTBreadCrumb.Core;
using FODLSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FODLSystem.Controllers
{
    public class DriversController : Controller
    {
        private readonly FODLSystemContext _context;

        public DriversController(FODLSystemContext context)
        {
            _context = context;
        }

        [BreadCrumb(Title = "Index", Order = 1, IgnoreAjaxRequests = true)]
        public IActionResult Index()
        {
            this.SetCurrentBreadCrumbTitle("Item");
            return View();
        }
        
        [HttpPost]
        public ActionResult SaveItem(int ID,string IdNumber, string Name,string Position, string Status)
        {
            string status = "";
            string message = "";
            int itemid = 0;
            try
            {
                var _Drivers = _context.Drivers.FirstOrDefault(r => r.IdNumber == IdNumber);
                if (_Drivers == null)
                {

                    var item = new Driver
                    {
                        IdNumber = IdNumber,
                        Name = Name,
                        Position = Position,
                        Status = "Enabled",

                    };
                    
                    _context.Add(item);
                    _context.SaveChanges();
                    itemid = item.ID;
                }
                else
                {
                    _Drivers.IdNumber = IdNumber;
                    _Drivers.Name = Name;
                    _Drivers.Position = Position;
                    _Drivers.Status = Status;
        
                    _Drivers.DateModified = DateTime.Now;
                    _context.Entry(_Drivers).State = EntityState.Modified;
                    _context.SaveChanges();

                    itemid = ID;
                }


               

                status = "success";
            }
            catch (Exception ex)
            {
                status = "fail";
                message = ex.InnerException.Message;
                
            }

            var model = new
            {
                status,
                message,
                itemid
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
                        string colSearch = Request.Form["columns[" + (i-1) + "][name]"];



                        if (strFilter == "")
                        {

                            strFilter = colSearch + ".ToUpper().Contains(" + "\"" + colval + "\"" + ")";

                        }
                        else
                        {
                            strFilter = strFilter + " && " + colSearch + ".ToUpper().Contains(" + "\"" + colval + "\"" + ")";
                        }

                    }
                }

                var v =
                    _context.Drivers
                    .Select(a => new
                    {
                      a.IdNumber,
                      a.Name,
                      a.Position,
                      a.Status,
                      a.ID
                    });
                int recCount = v.Count();
       
                if (!string.IsNullOrEmpty(strFilter))
                {
                    v = v.Where(strFilter);
                }





                int recFilter = v.Count();





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
                var jsonData = new { draw = draw, recordsFiltered = recFilter, recordsTotal = recordsTotal, data = data};
                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }
    }
    
}