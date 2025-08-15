using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.ServiceModel;
using System.Threading.Tasks;
using DNTBreadCrumb.Core;
using FODLSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NAVItems;

namespace FODLSystem.Controllers
{
    public class ItemsController : Controller
    {
        private readonly FODLSystemContext _context;

        public ItemsController(FODLSystemContext context)
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
        public ActionResult SaveItem(int id,string TypeFuel,string DescLiq, string DescLiq2)
        {
            string status = "";
            string message = "";
            try
            {
                var item = _context.Items.FirstOrDefault(r=>r.Id==id);
                item.DescriptionLiquidation = DescLiq;
                item.DescriptionLiquidation2 = DescLiq2;
                item.TypeFuel = TypeFuel;
                item.DateModified = DateTime.Now.Date;
                _context.Entry(item).State = EntityState.Modified;
                _context.SaveChanges();

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


                for (int i = 0; i < 5; i++)
                {
                    string colval = Request.Form["columns[" + i + "][search][value]"];
                    if (colval != "")
                    {
                        colval = colval.ToUpper();
                        string colSearch = Request.Form["columns[" + i + "][name]"];



                        if (strFilter == "")
                        {

                            strFilter = colSearch + ".ToString().ToUpper().Contains(" + "\"" + colval + "\"" + ")";

                        }
                        else
                        {
                            strFilter = strFilter + " && " + colSearch + ".ToString().ToUpper().Contains(" + "\"" + colval + "\"" + ")";
                        }

                    }
                }


                if (strFilter == "")
                {
                    strFilter = "true";
                }



                int recCount =

                _context.Items
                .Where(a => a.Status == "Active")
               
                .Where(strFilter)
                .Count();

                recordsTotal = recCount;
                int recFilter = recCount;



                var v =

               _context.Items
                .Where(a => a.Status == "Active")
              .Where(strFilter)
              
              //.OrderBy(a => a.FileDate).ThenBy(a => a.Hour)
              .Skip(skip).Take(pageSize)
              .Select(a => new
              {
                  a.No,
                  a.Description,
                  a.Description2,
                  a.DescriptionLiquidation,
                  a.DescriptionLiquidation2,
                  a.TypeFuel,
                  a.Id



              });


               

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

        public async Task<IActionResult> GetItem()
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
                var endpoint = new EndpointAddress(new Uri("http://APRODITE.semiraramining.net:7057/BC130_SMPC_TEST/WS/Semirara/Page/Items"));

                Items_PortClient _PortClientSpare = new Items_PortClient(binding, endpoint);
                _PortClientSpare.ClientCredentials.UserName.UserName = NAVUserName;
                _PortClientSpare.ClientCredentials.UserName.Password = NAVPassword;

                List<Items_Filter> filterArray2 = new List<Items_Filter>();
                Items_Filter _no = new Items_Filter
                {
                    Field = Items_Fields.No,
                    Criteria = "FO*"
                };
                filterArray2.Add(_no);

                Items_Filter tag1 = new Items_Filter
                {
                    Field = Items_Fields.tag1,
                    Criteria = "<>DEL"
                };
                filterArray2.Add(tag1);

                Items_Filter tag2 = new Items_Filter
                {
                    Field = Items_Fields.tag2,
                    Criteria = "<>DEL-NO TRANS&<>FOR BLOCKING"
                };
                filterArray2.Add(tag2);

                ReadMultiple_Result fncResult = new ReadMultiple_Result();
                fncResult = await _PortClientSpare.ReadMultipleAsync(filterArray2.ToArray(), null, 20000);

                for (var i = 0; i < fncResult.ReadMultiple_Result1.Count(); i++)
                {
                    var _eq = _context.Items.OrderByDescending(r => r.DateModified).FirstOrDefault(r => r.No == fncResult.ReadMultiple_Result1[i].No);
                    if (_eq != null)
                    {
                        if (fncResult.ReadMultiple_Result1[i].Blocked == true)
                        {
                            _eq.Status = "Deleted";
                        }
                        else
                        {
                            _eq.Status = "Active";
                        }

                        _eq.Description = fncResult.ReadMultiple_Result1[i].Description;
                        _eq.Description2 = fncResult.ReadMultiple_Result1[i].Description_2;
                        _eq.DateModified = DateTime.Now;

                        if(fncResult.ReadMultiple_Result1[i].Item_Category_Code== "FUELLUBE")
                        {
                            _eq.TypeFuel = "OIL-LUBE";
                        }
                        _context.Entry(_eq).State = EntityState.Modified;
                        _context.SaveChanges();

                    }
                    else
                    {
                        var eq = new Item
                        {
                            No = fncResult.ReadMultiple_Result1[i].No,
                            Description = fncResult.ReadMultiple_Result1[i].Description,
                            Description2 = fncResult.ReadMultiple_Result1[i].Description_2,
                            Status= "Active",
                            DateModified = DateTime.Now
                        };
                        if (fncResult.ReadMultiple_Result1[i].Item_Category_Code == "FUELLUBE")
                        {
                            _eq.TypeFuel = "OIL-LUBE";
                        }
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