using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DNTBreadCrumb.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FODLSystem.Models;
using FODLSystem.Models.View_Model;
using System.DirectoryServices;
using FODLSystem.Extension;
using DirectoryEntry = System.DirectoryServices.DirectoryEntry;

namespace FODLSystem.Controllers
{
    public class UsersController : Controller
    {
        private void ResetContextState() => _context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);
        private readonly FODLSystemContext _context;

        public UsersController(FODLSystemContext context)
        {
            _context = context;
        }

        [BreadCrumb(Title = "Index", Order = 1, IgnoreAjaxRequests = true)]
        public IActionResult Index(string domain)
        {
            string status = "Active,Default";
            string[] stat = status.Split(',').Select(n => n).ToArray();
            this.SetCurrentBreadCrumbTitle("Users");
            ViewBag.Domain = domain;
            ViewData["RoleId"] = new SelectList(_context.Set<Role>(), "Id", "Name");
            ViewData["DepartmentId"] = new SelectList(_context.Departments.Where(a => a.Status == "Active"), "Code", "Name");

            ////custom select
            //var lube = _context.LubeTrucks.Where(a => stat.Contains(a.Status)).Select(a => new
            //{
            //    a.Id,
            //    Text = a.No + " - " + a.Description
            //});
            //ViewData["LubeTruckId"] = new SelectList(lube.OrderBy(a => a.Text), "ID", "Text");



            //ViewData["LubeTruckId"] = new SelectList(_context.LubeTrucks.Where(a => stat.Contains(a.Status)), "Id", "Description");



            var lube = _context.LubeTrucks.Where(a => stat.Contains(a.Status)).Select(a => new
            {
                a.No,
                Text = a.No + " | " + a.Description
            });
            ViewData["LubeTruckId"] = new SelectList(lube.OrderBy(a => a.Text), "No", "Text");






            ViewData["DispenserId"] = new SelectList(_context.Dispensers.Where(a => stat.Contains(a.Status)), "No", "Name");


            return View();
        }
        public IActionResult GetUserList(string domain)
        {

            //NEW
            string status = "";
            string message = "";
            var lst = new List<UserViewModel>();
            string stat = "";
            int id = 0;
            string roles = "";
            string domainName = "";
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);
            PrincipalContext ctx2 = new PrincipalContext(ContextType.Domain);
            try
            {
                var user = _context.Users.Where(u => u.Status == "1")
                    .Select(r => new UserViewModel {
                        id = r.Id,
                        Username = r.Username,
                        Firstname = r.FirstName,
                        Name = r.Name,
                        Lastname = r.LastName,
                        mail = r.Email,
                        sysid = r.Id.ToString(),
                        domain = r.Domain,
                        status = "Enabled",
                        Roles = roles
                    });

                var mdl = new
                {

                    status= "success",
                    message,
                    data = user
                };

                return Json(mdl);
            }
            catch (Exception e)
            {
                status = "fail";
                message = e.Message;
                throw;
            }






            var model = new
            {

                status,
                message,
                data = lst
            };
            return Json(model);



        }

        // GET: Users/Create
        public IActionResult CreateWindowsAccount()
        {
            string status = "Active,Default";
            string[] stat = status.Split(',').Select(n => n).ToArray();
            ViewData["RoleId"] = new SelectList(_context.Set<Role>(), "Id", "Name");
            ViewData["DepartmentId"] = new SelectList(_context.Departments.Where(a => a.Status == "Active"), "Code", "Name");
            //ViewData["LubeTruckId"] = new SelectList(_context.LubeTrucks.Where(a => stat.Contains(a.Status)), "Id", "Description");
            //custom select
            var lube = _context.LubeTrucks.Where(a => stat.Contains(a.Status)).Select(a => new
            {
                a.No,
                Text = a.No + " | " + a.Description
            });
            ViewData["LubeTruckId"] = new SelectList(lube.OrderBy(a => a.Text), "No", "Text");
            ViewData["DispenserId"] = new SelectList(_context.Dispensers.Where(a => stat.Contains(a.Status)), "No", "Name");

            var domain = new List<SelectListItem>
            {
                new SelectListItem { Text = "SMCDACON", Value = "SMCDACON" },
                new SelectListItem { Text = "SEMCALACA", Value = "SEMCALACA" },
                new SelectListItem { Text = "SEMIRARAMINING", Value = "SEMIRARAMINING" }
            };
            ViewData["Domain"] = new SelectList(domain, "Value", "Text");
            return View(new DomainUserViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateWindowsAccount([Bind]DomainUserViewModel userView, string[] lubetags, string[] dispensertags)
        {
            string lubeaccess = string.Join(",", lubetags);
            string dispenseraccess = string.Join(",", dispensertags);

            if (ModelState.IsValid)
            {
                try
                {
                    User user = new User
                    {
                        RoleId = userView.RoleId,
                        Username = userView.Username,
                        Status = "1",
                        Domain = userView.Domain,
                        FirstName = userView.Firstname,
                        LastName = userView.Lastname,
                        Name = userView.Firstname + " " + userView.Lastname,
                        DepartmentCode = userView.DepartmentCode,
                        CompanyAccess = "1",
                        LubeAccess = lubeaccess,
                        DispenserAccess = dispenseraccess,
                        DateModified = DateTime.Now,
                        Email = userView.Email
                    };
                    _context.Add(user);
                    await _context.SaveChangesAsync();


                    Log log = new Log
                    {
                        Descriptions = "New User - " + user.Id,
                        Action = "Add",
                        Status = "success",
                        UserId = User.Identity.Name
                    };
                    _context.Add(log);
                    await _context.SaveChangesAsync();
                    TempData["APPSuccess"] = "Account has been successfully registered.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    ResetContextState();
                    Log log = new Log
                    {
                        Descriptions = "New User - " + e.InnerException.Message,
                        Action = "Add",
                        Status = "fail",
                        UserId = User.Identity.Name
                    };
                    _context.Add(log);
                    await _context.SaveChangesAsync();
                    ModelState.AddModelError("", e.InnerException.Message);
                }

            }


            ViewData["RoleId"] = new SelectList(_context.Set<Role>(), "Id", "Name", userView.RoleId);
            ViewData["DepartmentId"] = new SelectList(_context.Set<Department>(), "Code", "Name", userView.DepartmentId);
            return View(userView);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            string status = "Active,Default";
            string[] stat = status.Split(',').Select(n => n).ToArray();
            ViewData["RoleId"] = new SelectList(_context.Set<Role>(), "Id", "Name");
            ViewData["DepartmentId"] = new SelectList(_context.Departments.Where(a => a.Status == "Active"), "Code", "Name");
            //ViewData["LubeTruckId"] = new SelectList(_context.LubeTrucks.Where(a => stat.Contains(a.Status)), "Id", "Description");
            //custom select
            var lube = _context.LubeTrucks.Where(a => stat.Contains(a.Status)).Select(a => new
            {
                a.No,
                Text = a.No + " | " + a.Description
            });
            ViewData["LubeTruckId"] = new SelectList(lube.OrderBy(a => a.Text), "No", "Text");
            ViewData["DispenserId"] = new SelectList(_context.Dispensers.Where(a => stat.Contains(a.Status)), "No", "Name");
            return View(new LocalUserViewModel());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind]LocalUserViewModel userView, string[] lubetags, string[] dispensertags)
        {
            string lubeaccess = string.Join(",", lubetags);
            string dispenseraccess = string.Join(",", dispensertags);



            int cnt = _context.Users.Where(a => a.Username == userView.Username).Count();
            if (cnt > 0)
            {

                ModelState.AddModelError("", "UserName already existing");
                return View(userView);
            }


            if (ModelState.IsValid)
            {
                try
                {
                    User user = new User();
                    user.Password = GetSHA1HashData(userView.Password);
                    user.RoleId = userView.RoleId;
                    user.Username = userView.Username;
                    user.Status = "1";
                    user.Domain = "Local";
                    user.FirstName = userView.Firstname;
                    user.LastName = userView.Lastname;
                    user.Name = userView.Firstname + " " + userView.Lastname;
                    user.DepartmentCode = userView.DepartmentCode;
                    user.CompanyAccess = "1";
                    user.LubeAccess = lubeaccess;
                    user.DispenserAccess = dispenseraccess;
                    user.DateModified = DateTime.Now;
                    _context.Add(user);
                    await _context.SaveChangesAsync();


                    Log log = new Log
                    {
                        Descriptions = "New User - " + user.Id,
                        Action = "Add",
                        Status = "success",
                        UserId = User.Identity.Name
                    };
                    _context.Add(log);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    ResetContextState();
                    Log log = new Log
                    {
                        Descriptions = "New User - " + e.InnerException.Message,
                        Action = "Add",
                        Status = "fail",
                        UserId = User.Identity.Name
                    };
                    _context.Add(log);
                    await _context.SaveChangesAsync();
                    ModelState.AddModelError("", e.InnerException.Message);
                }

            }






            ViewData["RoleId"] = new SelectList(_context.Set<Role>(), "Id", "Name", userView.RoleId);
            ViewData["DepartmentId"] = new SelectList(_context.Set<Department>(), "Code", "Name", userView.DepartmentId);
            return View(userView);
        }


        private string GetSHA1HashData(string data)
        {
            //create new instance of md5
            SHA1 sha1 = SHA1.Create();

            //convert the input text to array of bytes
            byte[] hashData = sha1.ComputeHash(Encoding.Default.GetBytes(data));

            //create new instance of StringBuilder to save hashed data
            StringBuilder returnValue = new StringBuilder();

            //loop for each byte and add it to StringBuilder
            for (int i = 0; i < hashData.Length; i++)
            {
                returnValue.Append(hashData[i].ToString());
            }

            // return hexadecimal string
            return returnValue.ToString();
        }
        public IActionResult CreateFODUser(string domain)
        {
            string status = "Active,Default";
            string[] stat = status.Split(',').Select(n => n).ToArray();
            this.SetCurrentBreadCrumbTitle("Users");
            ViewBag.Domain = domain;
            ViewData["RoleId"] = new SelectList(_context.Set<Role>(), "Id", "Name");
            ViewData["DepartmentId"] = new SelectList(_context.Departments.Where(a => a.Status == "Active"), "Code", "Name");


            var lube = _context.LubeTrucks.Where(a => stat.Contains(a.Status)).Select(a => new
            {
                a.No,
                Text = a.No + " | " + a.Description
            });
            ViewData["LubeTruckId"] = new SelectList(lube.OrderBy(a => a.Text), "No", "Text");






            ViewData["DispenserId"] = new SelectList(_context.Dispensers.Where(a => stat.Contains(a.Status)), "No", "Name");


            return View();
        }
        public IActionResult getData(string domain)
        {


            string search = Request.Form["search[value]"].FirstOrDefault();
            string draw = Request.Form["draw"].FirstOrDefault();
            string order = Request.Form["order[0][column]"].FirstOrDefault();
            string orderDir = Request.Form["order[0][dir]"];
            int startRec = Convert.ToInt32(Request.Form["start"]);
            int pageSize = Convert.ToInt32(Request.Form["length"]);
            var lst = new List<UserViewModel>();

                string roles = "";
                string domainName = "";
                int totalRecords = 0;
                string OU2 = "";
                int ADUserCount = 0;
                if (domain == "SEMIRARAMINING")
                {
                    domainName = "SEMIRARAMINING";
                    domain = "SEMIRARAMINING";
                    OU2 = "OU=SEMIRARA MINESITE,DC=semiraramining,DC=net";
                }
                else
                {
                    domainName = "smcdacon";
                    OU2 = "OU=MAKATI HEAD OFFICE,DC=smcdacon,DC=com";
                }

                var aduser = domainName.ToLower() + "\\qmaster";

                using (var context = new PrincipalContext(ContextType.Domain, domain.ToLower(), OU2, aduser, "M@st3rQ###"))
                {
                    using (var searcher = new PrincipalSearcher(new UserPrincipal(context)))
                    {

                        if (!string.IsNullOrEmpty(search))
                        {

                            searcher.QueryFilter.DisplayName = "*" + search + "*";
                            // searcher.QueryFilter.DisplayName.Contains(search);
                            totalRecords = searcher.FindAll().Count();
                        }
                        else
                        {
                            totalRecords = searcher.FindAll().Count();
                        }

                        totalRecords = searcher.FindAll().Count();

                        ((DirectorySearcher)searcher.GetUnderlyingSearcher()).PageSize = 0;
                        ((DirectorySearcher)searcher.GetUnderlyingSearcher()).Sort = new SortOption("cn", SortDirection.Ascending);
                        //  ((DirectorySearcher)searcher.GetUnderlyingSearcher()).VirtualListView = new DirectoryVirtualListView(startRec, pageSize, draw);

                        ADUserCount = searcher.FindAll().Count();
                        foreach (var result in searcher.FindAll())
                        {
                            System.DirectoryServices.DirectoryEntry de = result.GetUnderlyingObject() as System.DirectoryServices.DirectoryEntry;
                            string Mail = "";
                            if (de.Properties["mail"].Value != null) Mail = de.Properties["mail"].Value.ToString();


                            var user = _context.Users.Where(u => u.Username == result.SamAccountName.ToString()).Where(u => u.Domain == domainName).Where(u => u.Status == "1").FirstOrDefault();

                            string stat = "";
                            int id = 0;
                            if (user != null)
                            {
                                stat = "Enabled";
                                id = user.Id;

                            }
                            else
                            {
                                stat = "Disabled";
                                id = 0;

                            }
                            var adUser = new UserViewModel()
                            {
                                id = id,
                                Username = result.SamAccountName,
                                Firstname = de.Properties["givenName"].Value.ToString(),
                                Name = result.DisplayName,
                                Lastname = de.Properties["sn"].Value.ToString(),
                                mail = Mail,
                                sysid = result.Guid.GetValueOrDefault().ToString(),
                                domain = domain,
                                status = stat,
                                Roles = roles
                            };
                            lst.Add(adUser);

                        }
                    
                    }
                }


            var jsonData = new
            {
                draw = Convert.ToInt32(draw),
                recordsTotal = totalRecords,
                recordsFiltered = lst.Count(),
                data = lst.ToList(),
            };

            return new JsonResult(jsonData);



        }
        public IActionResult getDataLocal()
        {

            string status = "";
            var v =

                _context.Users.Where(a=>a.Domain == "Local").Select(a => new {


                    a.Username
                      ,
                    a.Name
                     
                        ,

                    a.Id
                    ,a.Status
                    , a.DepartmentCode
                    , Department = a.Departments.Name
                    , Role = a.Roles.Name
                    , a.RoleId
                    , a.FirstName
                    , a.LastName

                    ,
                    LubeAccess = a.LubeAccess
                    ,
                    DispenserAccess = a.DispenserAccess

                    , StatusName = a.Status == "1" ? "Enabled" : "Disabled"
                });
            status = "success";






            var model = new
            {
                status
                ,
                data = v.ToList()
            };
            return Json(model);



        }


        [HttpPost]
        public IActionResult EnableDisableUser(string Domain, string UserName, string Email, string Status, string Name, string UserType,int Id)
        {

            string status = "";
            string message = "";

            try
            {
                //if (Status == "Enabled")
                //{
                    var result = _context.Users.FirstOrDefault(r=>r.Id==Id);
                    if (result != null)
                    {
                      //  var _dept = _context.Departments.FirstOrDefault(r=>r.ID == result.de);
                        //result.Email = Email;
                        result.Status = "0";
                        result.DateModified = DateTime.Now;
                        _context.Entry(result).State = EntityState.Modified;
                        _context.SaveChanges();
                        status = "success";
                    }
                    //else
                    //{
                    //    User user = new User
                    //    {
                    //        DepartmentCode = "NA", //Not set
                    //        Username = UserName,
                    //        Domain = Domain,
                    //        Name = Name,
                    //        Email = Email,
                    //        Status = "1",
                    //        DispenserAccess = "1",
                    //        LubeAccess = "1",
                    //        RoleId = 2,
                    //        UserType = UserType,
                    //        DateModified = DateTime.Now
                    //    };
                    //    _context.Users.Add(user);
                    //    _context.SaveChanges();
                    //    status = "success";
                    //}


                //}
                //else
                //{

                //    var result = _context.Users.Where(b => b.Username == UserName).Where(b => b.Domain == Domain).FirstOrDefault();
                //    if (result != null)
                //    {
                //        result.Status = "0";
                //        result.DateModified = DateTime.Now;
                //        _context.Entry(result).State = EntityState.Modified;
                //        _context.SaveChanges();

                //    }
                //    else
                //    {
                //        User user = new User
                //        {
                //            DepartmentCode = "NA", //Not set
                //            Username = UserName,
                //            Domain = Domain,
                //            Name = Name,
                //            Status = "1",
                //            RoleId = 2,
                //            CompanyAccess = "1",
                //            DispenserAccess = "1",
                //            LubeAccess = "1",
                //            UserType = UserType,
                //            DateModified = DateTime.Now
                //        };
                //        _context.Users.Add(user);
                //        _context.SaveChanges();

                //    }


                //    status = "success";

                //}

            }
            catch (Exception e)
            {
                status = "fail";
                message = e.InnerException.InnerException.Message.ToString();

            }
            var model = new
            {
                status,
                message

            };



            return Json(model);
        }



        [BreadCrumb(Title = "Edit", Order = 2, IgnoreAjaxRequests = true)]
        public IActionResult Edit(int? id)
        {
            string status = "Active,Default";
            string[] stat = status.Split(',').Select(n => n).ToArray();

            this.AddBreadCrumb(new BreadCrumb
            {
                Title = "Users",
                Url = string.Format(Url.Action("Index", "Users")),
                Order = 1
            });
            if (id == null)
            {
                return NotFound();
            }
            var user = _context.Users.Include(a => a.Departments).Where(a => a.Id == id).FirstOrDefault();
            if (user == null)
            {
                return NotFound();
            }

            string deptname = user.Departments ==null ?"" : user.Departments.Name;
            int CompanyId = user.Departments == null ? 0 : user.Departments.CompanyId;
            ViewData["Department"] = new SelectList(_context.Departments.Where(a => a.Status == "Active"), "Code", "Name", user.DepartmentCode);
            ViewData["Company"] = new SelectList(_context.Companies.Where(a => a.Status == "Active"), "ID", "Name", CompanyId);
            ViewData["Roles"] = new SelectList(_context.Roles, "Id", "Name", user.RoleId);

            //custom select
            var lube = _context.LubeTrucks.Where(a => stat.Contains(a.Status)).Select(a => new
            {
                a.No,
                Text = a.No + " | " + a.Description
            });
            ViewData["LubeTruckId"] = new SelectList(lube.OrderBy(a => a.Text), "No", "Text");
            //ViewData["LubeTruckId"] = new SelectList(_context.LubeTrucks.Where(a => stat.Contains(a.Status)), "Id", "Description");
            ViewData["DispenserId"] = new SelectList(_context.Dispensers.Where(a => stat.Contains(a.Status)), "No", "Name");
            return View(user);
        }

        [HttpPost]
        public IActionResult ReloadDepartment(int? id)
        {
            var dept = new SelectList(_context.Departments.Where(a => a.Status == "Active").Where(a => a.CompanyId == id), "ID", "Name");

            return Json(dept);
        }

        [HttpPost]
        public IActionResult Edit(User u, string[] companytags,string[] lubetags,string[] dispensertags)
        {

            //u.CompanyAccess = companytags.ToString();
            string companyaccess = string.Join(",", companytags);
            string lubeaccess = string.Join(",", lubetags);
            string dispenseraccess = string.Join(",", dispensertags);



            if (ModelState.IsValid)
            {
                var user = _context.Users.Find(u.Id);
                user.CompanyAccess = companyaccess;
                user.RoleId = u.RoleId;
                user.DepartmentCode = u.DepartmentCode;
                user.UserType = u.UserType;
                user.LubeAccess = lubeaccess;
                user.DispenserAccess = dispenseraccess;
                user.DateModified = DateTime.Now;
                _context.Entry(user).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(u);
        }
        [HttpPost]
        public IActionResult Delete(int? id)
        {
            string status = "";
            string message = "";

            try
            {
                User detail = _context.Users.Find(id);
                detail.Status = "0_" + DateTime.Now.Ticks;
                _context.Entry(detail).State = EntityState.Modified;
                _context.SaveChanges();
                status = "success";
            }
            catch (Exception ex)
            {
                message = ex.InnerException.InnerException.Message;
                status = "failed";
            }
            var model = new
            {
                status,
                message

            };
            return Json(model);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        public JsonResult SelectUserList(string q)
        {

            var model = _context.Users.Where(b => b.Status == "1").Select(b => new
            {

                id = b.Id,
                text = b.Name,
            });

            if (!string.IsNullOrEmpty(q))
            {
                model = model.Where(b => b.text.Contains(q));
            }

            var modelUser = new
            {
                total_count = model.Count(),
                incomplete_results = false,
                items = model.ToList(),
            };
            return Json(modelUser);
        }
        [HttpPost]
        public ActionResult SaveLocal(LocalUserViewModel luser, string[] lubetags, string[] dispensertags)
        {
            string lubeaccess = string.Join(",", lubetags);
            string dispenseraccess = string.Join(",", dispensertags);


            string status = "";
            string message = "";
            try
            {

                
                var item = _context.Users.Find(luser.Id);
                item.Username = luser.Username;
                item.LastName = luser.Lastname;
                item.FirstName = luser.Firstname;
                item.DepartmentCode = luser.DepartmentCode;
                item.Name = luser.Firstname + " " + luser.Lastname;
                item.RoleId = luser.RoleId;
                item.CompanyAccess = "1";
                item.LubeAccess = lubeaccess;
                item.DispenserAccess = dispenseraccess;
                item.DateModified = DateTime.Now;
                item.Status = luser.Status;
                _context.Entry(item).State = EntityState.Modified;
                _context.SaveChanges();
               


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
                message
            };

            return Json(model);
        }

        public JsonResult GetDomainUser()
        {
            List<Select2Model> ADUsers = new List<Select2Model>();
            string search = Request.Query["q"];
            string domain = Request.Query["d"];

            // Total record count.
            int totalRecords = 0;
            string OU2 = "";
            string OU3 = "";

            if (domain.ToUpper().Trim() == "SMCDACON")
            {
                OU2 = "DC=smcdacon,DC=com";
            }
            else if (domain.ToUpper() == "SEMIRARAMINING")
            {
                OU2 = "DC=semiraramining,DC=net";

            }
            else if (domain.ToUpper() == "SEMCALACA")
            {
                OU3 = "DC=semcalaca,DC=com";
            }
            int ADUserCount = 0;

            var aduser = domain.ToLower() + "\\qmaster";

            try
            {
                if (domain.ToUpper().Trim() == "SMCDACON" || domain.ToUpper().Trim() == "SEMIRARAMINING")
                {
                    using (var context = new PrincipalContext(ContextType.Domain, domain.ToLower(), OU2, aduser, "M@st3rQ###"))
                    {
                        using (var searcher = new PrincipalSearcher(new UserPrincipal(context)))
                        {

                            if (!string.IsNullOrEmpty(search))
                            {

                                searcher.QueryFilter.SamAccountName = "*" + search + "*";
                                // searcher.QueryFilter.DisplayName.Contains(search);
                                totalRecords = searcher.FindAll().Count();
                            }
                            else
                            {
                                totalRecords = searcher.FindAll().Count();
                            }

                            totalRecords = searcher.FindAll().Count();

                            ((DirectorySearcher)searcher.GetUnderlyingSearcher()).PageSize = 0;
                            ((DirectorySearcher)searcher.GetUnderlyingSearcher()).Sort = new SortOption("cn", SortDirection.Ascending);
                            //  ((DirectorySearcher)searcher.GetUnderlyingSearcher()).VirtualListView = new DirectoryVirtualListView(startRec, pageSize, draw);

                            ADUserCount = searcher.FindAll().Count();
                            foreach (var result in searcher.FindAll())
                            {
                                DirectoryEntry de = result.GetUnderlyingObject() as DirectoryEntry;
                                string Mail = "";
                                if (de.Properties["mail"].Value != null) Mail = de.Properties["mail"].Value.ToString();

                                string sn = "";
                                if (de.Properties["sn"].Value != null) sn = de.Properties["sn"].Value.ToString();

                                string givenName = "";
                                if (de.Properties["givenName"].Value != null) givenName = de.Properties["givenName"].Value.ToString();

                                var user = _context.Users.Where(u => u.Username.ToUpper() == result.SamAccountName.ToUpper() && u.Domain.ToUpper() == domain.ToUpper()).FirstOrDefault();
                                if (user == null)
                                {
                                    //_user_id = user.Id;
                                    //   _status = "Enabled";
                                    ADUsers.Add(new Select2Model { text = result.SamAccountName, id = result.SamAccountName, title = result.DisplayName +  "^" + sn + "^" + givenName + "^" + Mail + "^" + result.Guid.GetValueOrDefault() });
                                }
                            }
                        }
                    }
                }
                else if (domain == "SEMCALACA")
                {

                    using (var context = new PrincipalContext(ContextType.Domain, domain.ToLower(), OU2, aduser, "M@st3rQ###"))
                    {
                        using (var searcher = new PrincipalSearcher(new UserPrincipal(context)))
                        {
                            if (!string.IsNullOrEmpty(search))
                            {

                                searcher.QueryFilter.SamAccountName = "*" + search + "*";
                                // searcher.QueryFilter.DisplayName.Contains(search);
                                totalRecords = searcher.FindAll().Count();
                            }
                            else
                            {
                                totalRecords = searcher.FindAll().Count();
                            }

                            ((DirectorySearcher)searcher.GetUnderlyingSearcher()).PageSize = 0;
                            ((DirectorySearcher)searcher.GetUnderlyingSearcher()).Sort = new SortOption("cn", SortDirection.Ascending);
                            // ((DirectorySearcher)searcher.GetUnderlyingSearcher()).VirtualListView = new DirectoryVirtualListView(startRec, pageSize, draw);

                            ADUserCount = searcher.FindAll().Count();
                            foreach (var result in searcher.FindAll())
                            {
                                System.DirectoryServices.DirectoryEntry de = result.GetUnderlyingObject() as DirectoryEntry;
                                string Mail = "";
                                if (de.Properties["mail"].Value != null) Mail = de.Properties["mail"].Value.ToString();

                                string sn = "";
                                if (de.Properties["sn"].Value != null) sn = de.Properties["sn"].Value.ToString();

                                string givenName = "";
                                if (de.Properties["givenName"].Value != null) givenName = de.Properties["givenName"].Value.ToString();

                                var user = _context.Users.Where(u => u.Username.ToUpper() == result.SamAccountName.ToUpper() && u.Domain.ToUpper() == domain.ToUpper()).FirstOrDefault();
                                if (user == null)
                                {
                                    ADUsers.Add(new Select2Model { text = result.SamAccountName, id = result.SamAccountName, title = result.DisplayName + "^" + Mail + "^" + sn + "^" + givenName + "^" + result.Guid.GetValueOrDefault() });
                                }


                            }
                        }
                    }
                }

            }
            catch (Exception x)
            {
                var err = x.Message;
            }


            return new JsonResult(ADUsers);
        }

    }
}
