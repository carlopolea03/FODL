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
    public class LocationsController : Controller
    {
        private readonly FODLSystemContext _context;

        public LocationsController(FODLSystemContext context)
        {
            _context = context;
        }

        [BreadCrumb(Title = "Index", Order = 1, IgnoreAjaxRequests = true)]
        public IActionResult Index()
        {
            this.SetCurrentBreadCrumbTitle("Location");
            return View();
        }
        [HttpPost]
        public ActionResult SaveItem(int id, string No, string List, string OfficeCode)
        {
            string status = "";
            string message = "";
            try
            {
                var item = _context.Locations.FirstOrDefault(r=>r.Id==id);
                if (item != null)
                {
                    item.No = No;
                    item.List = List;
                    //item.OfficeCode = OfficeCode;
                    _context.Entry(item).State = EntityState.Modified;
                    _context.SaveChanges();
                }
                else
                {
                    var locations = new Location
                    {
                        No = No,
                        List = List,
                        Status = "Active",
                        OfficeCode = "0"
                    };
            
                    //item.OfficeCode = OfficeCode;
                    _context.Entry(locations).State = EntityState.Added;
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


                string[] fields = new string[] { "No", "List", "Id" };
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



                var ReqList = _context.Locations
                .Where(a => a.Status == "Active")
              .Select(a => new
              {
                  a.No,
                  a.List,
                  a.Id,
                  a.Status
              });

                recordsTotal = ReqList.Count();

                if (!string.IsNullOrEmpty(strFilter))
                {
                    ReqList = ReqList.Where(strFilter);
                }



                int recFilter = ReqList.Count();


                var sortfield = fields[int.Parse(order)];
                ReqList = ReqList.OrderByField(sortfield, orderDir);

                ReqList = ReqList.Skip(start).Take(length);
                var data = ReqList.ToList();

                var jsonData = new
                {
                    draw = Convert.ToInt32(draw),
                    recordsTotal,
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
    }
}