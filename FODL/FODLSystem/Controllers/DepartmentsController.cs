using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DNTBreadCrumb.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FODLSystem.Models;
using System.ServiceModel;
using NAVDimension;
using System.Linq.Dynamic.Core;

namespace FODLSystem.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly FODLSystemContext _context;

        public DepartmentsController(FODLSystemContext context)
        {
            _context = context;
        }
        [BreadCrumb(Title = "Index", Order = 1, IgnoreAjaxRequests = true)]
        // GET: Departments
        public async Task<IActionResult> Index()
        {
            this.SetCurrentBreadCrumbTitle("Departments");
            var FODLSystemContext = _context.Departments.Include(d => d.Companies);
            return View(await FODLSystemContext.ToListAsync());
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


                for (int i = 0; i < 2; i++)
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

                var v = _context.Departments.Where(a => a.Status != "Deleted").Select(a => new {
                    a.Code, a.Name,CompanyName = a.Companies.Name, a.ID });

                int recCount = v.Count();

                if (!string.IsNullOrEmpty(strFilter))
                {
                    v = v.Where(strFilter);
                }


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
        // GET: Departments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(d => d.Companies)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        [BreadCrumb(Title = "Create", Order = 2, IgnoreAjaxRequests = true)]
        // GET: Departments/Create
        public IActionResult Create()
        {
            this.AddBreadCrumb(new BreadCrumb
            {
                Title = "Department",
                Url = string.Format(Url.Action("Index", "Departments")),
                Order = 1
            });

            ViewData["CompanyId"] = new SelectList(_context.Companies, "ID", "Name");
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Code,Name,Status,CompanyId")] Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Add(department);
                await _context.SaveChangesAsync();


                Log log = new Log
                {
                    Descriptions = "Create Department - " + department.ID,
                    Action = "Create",
                    Status = "success",
                    UserId = User.Identity.GetUserName()
                };
                _context.Add(log);

                _context.SaveChanges();



                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "ID", "Name", department.CompanyId);
            return View(department);
        }
        [BreadCrumb(Title = "Edit", Order = 2, IgnoreAjaxRequests = true)]
        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            this.AddBreadCrumb(new BreadCrumb
            {
                Title = "Department",
                Url = string.Format(Url.Action("Index", "Departments")),
                Order = 1
            });

            var department = await _context.Departments.FirstOrDefaultAsync(r=>r.ID==id);
            if (department == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "ID", "Name", department.CompanyId);
            return View(department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Code,Name,Status,CompanyId")] Department department)
        {
            if (id != department.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    
                    _context.Update(department);
                    await _context.SaveChangesAsync();

                    Log log = new Log
                    {
                        Descriptions = "Update Department - " + department.ID,
                        Action = "Edit",
                        Status = "success",
                        UserId = User.Identity.GetUserName()
                    };
                    _context.Add(log);

                    _context.SaveChanges();


                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "ID", "Name", department.CompanyId);
            return View(department);
        }

        [BreadCrumb(Title = "Delete", Order = 2, IgnoreAjaxRequests = true)]
        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            this.AddBreadCrumb(new BreadCrumb
            {
                Title = "Department",
                Url = string.Format(Url.Action("Index", "Departments")),
                Order = 1
            });
            var department = await _context.Departments
                .Include(d => d.Companies)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.Departments.FindAsync(id);
            model.Status = "Deleted";
            _context.Update(model);
            await _context.SaveChangesAsync();


            Log log = new Log
            {
                Descriptions = "Delete Department - " + id,
                Action = "Delete",
                Status = "success",
                UserId = User.Identity.GetUserName()
            };
            _context.Add(log);
            _context.SaveChanges();


            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.ID == id);
        }

        public async Task<IActionResult> GetDimension()
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
                var endpoint = new EndpointAddress(new Uri("http://APRODITE.semiraramining.net:7057/BC130_SMPC_TEST/WS/Semirara/Page/DimensionValue"));

                DimensionValue_PortClient _PortClientSpare = new DimensionValue_PortClient(binding, endpoint);
                _PortClientSpare.ClientCredentials.UserName.UserName = NAVUserName;
                _PortClientSpare.ClientCredentials.UserName.Password = NAVPassword;

                List<DimensionValue_Filter> filterArray2 = new List<DimensionValue_Filter>();
                DimensionValue_Filter _code = new DimensionValue_Filter
                {
                    Field = DimensionValue_Fields.Dimension_Code,
                    Criteria = "DEPT"
                };
                filterArray2.Add(_code);


                ReadMultiple_Result fncResult = new ReadMultiple_Result();
                fncResult = await _PortClientSpare.ReadMultipleAsync(filterArray2.ToArray(), null, 10000);

                for (var i = 0; i < fncResult.ReadMultiple_Result1.Count(); i++)
                {
                    var _eq = _context.Departments.OrderByDescending(r => r.DateModified).FirstOrDefault(r => r.Code == fncResult.ReadMultiple_Result1[i].Code);
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

                        _eq.Name = fncResult.ReadMultiple_Result1[i].Name;
                        _eq.DateModified = DateTime.Now;

                        _context.Entry(_eq).State = EntityState.Modified;
                        _context.SaveChanges();

                    }
                    else
                    {
                        var eq = new Department
                        {
                            Code = fncResult.ReadMultiple_Result1[i].Code,
                            Name = fncResult.ReadMultiple_Result1[i].Name,
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
