using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UILAB.Concrete;
using UILAB.Interface;
using UILAB.Models;
using UILAB.Library;
using UILAB.Helpers;

namespace UILAB.Controllers
{
    public class LoginController : Controller
    {
        private ILogin _ILogin;
        private ICacheManager _ICacheManager;

        public LoginController()
        {
            _ILogin = new LoginConcrete();
            //_IAssignRoles = new AssignRolesConcrete();
            _ICacheManager = new CacheManager();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel loginViewModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(loginViewModel.Username) && !string.IsNullOrEmpty(loginViewModel.Password))
                {
                    var _UserName = loginViewModel.Username;
                    var _Password = EncryptionLibrary.EncryptText(loginViewModel.Password);
                    var _WorkStationName = System.Environment.MachineName;
                    var _ComputerIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    
                    #region --> Administrator Logins

                    var admin = _ILogin.ValidateAdmin(_UserName, _Password);

                    if (admin != null)
                    {
                        if (admin.IsActive == false)
                        {
                            ViewBag.errormessage = "Account Deactivated";
                            return View(loginViewModel);
                        }
                        else
                        {
                            var _RoleID = admin.RoleID;
                            remove_Anonymous_Cookies(); //Remove Anonymous_Cookies
                            Session["Username"] = Convert.ToString(admin.UserName);

                            if (_RoleID == 1)
                            {
                                Session["SuperAdmin"] = Convert.ToString(admin.UserID);
                                Session["Segment"] = Convert.ToString(admin.SegmentID);

                                return RedirectToAction("Dashboard", "SAdmin");
                            }
                            else
                            {
                                Session["Segment"] = Convert.ToString(admin.SegmentID);
                                Session["Employee"] = Convert.ToString(admin.EmployeeNo);
                                Session["AdminUser"] = Convert.ToString(admin.UserID);

                                using (var db = new DatabaseContext())
                                {
                                    // ----------------- Update Error Logins ------------------
                                    var list = (from data in db.SecurityActiveUser where data.UserID == admin.UserID && data.ConnectionAlive == true && data.UserType == 2 select data).ToList();

                                    if (list.Count > 0)
                                    {
                                        foreach (var sublist in list)
                                        {
                                            sublist.ConnectionAlive = false;
                                            db.SaveChanges();
                                        }
                                    }
                                    // ---------------------------------------------------------

                                    var item = new SecurityActiveUsers
                                    {
                                        WorkStationName = _WorkStationName,
                                        ComputerIP = _ComputerIP,
                                        SegmentID = Convert.ToInt32(HttpContext.Session["Segment"]),
                                        ConnectionAlive = true,
                                        LoginDate = DateTime.Now,
                                        LogOutDate = Convert.ToDateTime("1900-01-01"),
                                        UserID = Convert.ToInt32(HttpContext.Session["AdminUser"]),
                                        UserType = 2
                                    };
                                    db.SecurityActiveUser.Add(item);
                                    db.SaveChanges();

                                    int returnID = item.LoginInstance;
                                    Session["LoginInstance"] = Convert.ToString(returnID);
                                }
                                return RedirectToAction("Dashboard", "Admin");
                            }
                        }
                    }

                    #endregion

                    #region --> Employee Login
                    else
                    {
                        var employeeName = _ILogin.ValidateEmployeeName(_UserName, _Password);

                        if (employeeName != null)
                        {
                            if (employeeName.IsActive == false)
                            {
                                ViewBag.errormessage = "Account Deactivated";
                                return View(loginViewModel);
                            }
                            else
                            {
                                Session["Employee"] = Convert.ToString(employeeName.EmployeeNo);
                                Session["Username"] = Convert.ToString(employeeName.UserName);
                                Session["Segment"] = Convert.ToString(employeeName.SegmentID);

                                using (var db = new DatabaseContext())
                                {
                                    // ----------------- Update Error Logins ------------------
                                    var list = (from data in db.SecurityActiveUser where data.UserID == employeeName.EmployeeNo && data.ConnectionAlive == true && data.UserType == 3 select data).ToList();

                                    if (list.Count > 0)
                                    {
                                        foreach (var sublist in list)
                                        {
                                            sublist.ConnectionAlive = false;
                                            db.SaveChanges();
                                        }
                                    }
                                    // ---------------------------------------------------------

                                    var item = new SecurityActiveUsers
                                    {
                                        WorkStationName = _WorkStationName,
                                        ComputerIP = _ComputerIP,
                                        SegmentID = Convert.ToInt32(HttpContext.Session["Segment"]),
                                        ConnectionAlive = true,
                                        LoginDate = DateTime.Now,
                                        LogOutDate = Convert.ToDateTime("1900-01-01"),
                                        UserID = Convert.ToInt32(HttpContext.Session["Employee"]),
                                        UserType = 3
                                    };
                                    db.SecurityActiveUser.Add(item);
                                    db.SaveChanges();

                                    int returnID = item.LoginInstance;
                                    Session["LoginInstance"] = Convert.ToString(returnID);
                                }

                                return RedirectToAction("Dashboard", "Emp");
                            }
                        }
                        else
                        {
                            var employeeEmail = _ILogin.ValidateEmployeeEmail(_UserName, _Password);

                            if (employeeEmail != null)
                            {
                                if (employeeEmail.IsActive == false)
                                {
                                    ViewBag.errormessage = "Account Deactivated";
                                    return View(loginViewModel);
                                }
                                else
                                {
                                    Session["Employee"] = Convert.ToString(employeeEmail.EmployeeNo);
                                    Session["Username"] = Convert.ToString(employeeEmail.UserName);
                                    Session["Segment"] = Convert.ToString(employeeEmail.SegmentID);

                                    using (var db = new DatabaseContext())
                                    {
                                        // ----------------- Update Error Logins ------------------
                                        var list = (from data in db.SecurityActiveUser where data.UserID == admin.UserID && data.ConnectionAlive == true && data.UserType == 3 select data).ToList();

                                        if (list.Count > 0)
                                        {
                                            foreach (var sublist in list)
                                            {
                                                sublist.ConnectionAlive = false;
                                                db.SaveChanges();
                                            }
                                        }
                                        // ---------------------------------------------------------

                                        var item = new SecurityActiveUsers
                                        {
                                            WorkStationName = _WorkStationName,
                                            ComputerIP = _ComputerIP,
                                            SegmentID = Convert.ToInt32(HttpContext.Session["Segment"]),
                                            ConnectionAlive = true,
                                            LoginDate = DateTime.Now,
                                            LogOutDate = Convert.ToDateTime("1900-01-01"),
                                            UserID = Convert.ToInt32(HttpContext.Session["Employee"]),
                                            UserType = 3
                                        };
                                        db.SecurityActiveUser.Add(item);
                                        db.SaveChanges();

                                        int returnID = item.LoginInstance;
                                        Session["LoginInstance"] = Convert.ToString(returnID);
                                    }

                                    return RedirectToAction("Dashboard", "Emp");
                                }
                            }
                            else
                            {
                                ViewBag.errormessage = "Entered Invalid Username and Password";
                                return View(loginViewModel);
                            }
                        }
                    }

                    #endregion
                }
                return View(loginViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult Logout()
        {
            try
            {
                if (!string.IsNullOrEmpty(Convert.ToString(Session["SuperAdmin"])))
                {
                    //_ICacheManager.Clear("AdminCount");
                    //_ICacheManager.Clear("UsersCount");
                    //_ICacheManager.Clear("ProjectCount");
                }

                string _LogInstance = Convert.ToString(HttpContext.Session["LoginInstance"]);
                int loginstance = int.Parse(_LogInstance);
                int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

                using (var db = new DatabaseContext())
                {
                    var result = db.SecurityActiveUser.SingleOrDefault(b => b.LoginInstance == loginstance);

                    if (result != null)
                    {
                        result.ConnectionAlive = false;
                        result.LogOutDate = DateTime.Now;
                        db.SaveChanges();
                    }

                }

                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
                Response.Cache.SetNoStore();

                HttpCookie Cookies = new HttpCookie("WebTime");
                Cookies.Value = "";
                Cookies.Expires = DateTime.Now.AddHours(-1);
                Response.Cookies.Add(Cookies);
                HttpContext.Session.Clear();
                Session.Abandon();
                return RedirectToAction("Login", "Login");
            }
            catch (Exception)
            {
                throw;
            }
        }

        [NonAction]
        public void remove_Anonymous_Cookies()
        {
            try
            {

                if (Request.Cookies["WebTime"] != null)
                {
                    var option = new HttpCookie("WebTime");
                    option.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(option);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}