using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using FODLSystem;
using FODLSystem.Models;
using FODLSystem.Models.View_Model;


namespace FODLSystem.Controllers
{
    public class AccountsController : Controller
    {
        private readonly FODLSystemContext _context;

        public AccountsController(FODLSystemContext context)
        {
            _context = context;
        }
        //Get
        //Account/Login
        [AllowAnonymous]

        public ActionResult Login(string returnUrl)
        {
            ViewData["TabLogin"] = "windows";
            ViewBag.ReturnUrl = returnUrl;
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }


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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async Task<IActionResult> LogOff()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Login", "Accounts");
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string loginresult;
            try
            {
                if (model.LogInType == "AD")
                {
                    loginresult = CallAPI("http://192.168.0.153/ADAPI/api/values", model.Domain, model.Username, model.Password);
                    ViewData["TabLogin"] = "windows";
                }
                else
                {
                    loginresult = await LocalLogIn(model);
                    ViewData["TabLogin"] = "local";
                }


                //loginresult = "OK";
                if (loginresult == "OK")
                {
                    User user = new User() { Username = model.Username, Domain = model.Domain };
                    if (model.LogInType != "AD")
                    {
                        user.Domain = "Local";
                    }
                    user = GetUserDetails(user);

                    if (user != null)
                    {
                        var principal = CreatePrincipal(user);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "You are not permitted to Log-In");
                    }
                }
                else
                {
                    ModelState.AddModelError("", loginresult);

                }
            }
            catch(Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);

        }
        private async Task<string>LocalLogIn(LoginViewModel model)
        {
            var pwrd = GetSHA1HashData(model.Password);
            int user = await _context.Users.Where(a => a.Username == model.Username && a.Password == pwrd && a.Status == "1").CountAsync();
            if (user > 0)
            {
                return "OK";
            }
            else
            {
                return "User not known in local log-in";
            }
            
        }
        public string CallAPI(string url, string Domain, string UserName, string Password)

        {

            using (var wb = new WebClient())

            {

                var data = new NameValueCollection();

                data["Domain"] = Domain;

                data["Username"] = UserName;

                data["Password"] = Password;





                var response = wb.UploadValues(url, "POST", data);
                string responseInString = Encoding.UTF8.GetString(response);
                var str2 = JsonConvert.DeserializeObject(responseInString);


                return str2.ToString();

            }



        }



        public User GetUserDetails(User user)
        {
            var users = _context.Users.Include(e => e.Roles)
               .Include(e => e.Departments)
               .Where(u => u.Status == "1")
               .Where(u => u.Username.ToLower() == user.Username.ToLower() &&
               u.Domain == user.Domain)
           .FirstOrDefault();


            return users;



        }
        private ClaimsPrincipal CreatePrincipal(User user)
        {
            var claims = new List<Claim>
                {
                    new Claim("UserId", user.Id.ToString()),
                    new Claim("UserName", user.Username),
                    new Claim("RoleName", user.Roles.Name),
                    new Claim("FullName", user.Name),
                    new Claim("CompanyAccess", user.CompanyAccess),
                    new Claim("LubeAccess", user.LubeAccess),
                    new Claim("DispenserAccess", user.DispenserAccess),
                    new Claim("DepartmentCode", user.DepartmentCode??"" ),
                   
                };

            var principal = new ClaimsPrincipal();
            principal.AddIdentity(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
            return principal;
        }
    }
}