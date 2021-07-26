using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UILAB.Concrete;
using UILAB.Filters;
using UILAB.Models;
using UILAB.Interface;
using System.Net;
using System.IO;
using UILAB.Library;

namespace UILAB.Controllers.Crm
{
    [ValidateSAdminSession]
    public class AdministratorSAController : Controller
    {
        UserSecurity objadmin = new UserSecurity();

        IUser _IUser;
        public AdministratorSAController()
        {
            _IUser = new UserConcrete();
        }

        [HttpGet]
        public ActionResult List()
        {
            int _User = Convert.ToInt32(HttpContext.Session["SuperAdmin"]);

            ViewBag.Current = "User";
            TempData["Error"] = "";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.vw_UserSecurities orderby data.SegmentID ascending select data).ToList();
                return View(list);
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Current = "User";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var user = (from data in db.vw_EmployeeTB where data.SegmentID == _Segment orderby data.FullName ascending select data).ToList();
                ViewBag.EmployeeList = new SelectList(user, "EmployeeNo", "FullName");

                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserSecurity item, FormCollection collection)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["SuperAdmin"]);

            using (var db = new DatabaseContext())
            {
                var user = (from data in db.UserSecurities where data.UserName == item.UserName && data.SegmentID == _Segment select data).ToList();

                if (user.Count > 0)
                {
                    TempData["ErrorMessage"] = "Username  Exists ...!";
                    return RedirectToAction("Create", "Administrator");
                }
                else
                {
                    var _Password = EncryptionLibrary.EncryptText(item.Password);
                    string active = collection["isactive"];
                    bool _IsActive = Convert.ToBoolean(active);

                    string _UserName = item.UserName == null ? string.Empty : item.UserName.Trim();
                    int _Employee = item.EmployeeNo == null ? -1 : int.Parse(item.EmployeeNo.ToString());

                    objadmin.SegmentID = _Segment;
                    objadmin.UserName = _UserName;
                    objadmin.Password = _Password;
                    objadmin.IsActive = _IsActive;
                    objadmin.EmployeeNo = _Employee;
                    objadmin.ConfirmPassword = _Password;
                    objadmin.RoleID = 2; //Administrator
                    objadmin.Flagged = false;
                    objadmin.LastModifyDate = DateTime.Now;
                    objadmin.LastModifyUser = _User;
                    _IUser.InsertAdmin(objadmin);

                    if (_Employee != -1)
                    {
                        // ----------------------------- Update Employee -----------------------------
                        var update = db.EmployeeMasters.SingleOrDefault(b => b.EmployeeNo == _Employee);
                        if (update.Flagged != true)
                        {
                            update.Flagged = true;
                            db.SaveChanges();
                        }
                        //------------------------------------------------------------------------------
                    }

                    TempData["Success"] = "Success";
                    return RedirectToAction("List", "Administrator");
                }
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            ViewBag.Current = "User";
            int _User = Convert.ToInt32(HttpContext.Session["SuperAdmin"]);
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var result = db.UserSecurities.SingleOrDefault(b => b.UserID == id);

                if (result == null)
                {
                    return View(List());
                }
                else
                {
                    TempData["user"] = _User;

                    var employee = (from data in db.vw_EmployeeTB where data.SegmentID == _Segment orderby data.FullName ascending select data).ToList();
                    ViewBag.EmployeeList = new SelectList(employee, "EmployeeNo", "FullName");

                    return View(db.UserSecurities.Where(x => x.UserID == id).FirstOrDefault<UserSecurity>());
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserSecurity item, FormCollection collection)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);
            bool _IsActive = true;

            using (var db = new DatabaseContext())
            {
                var result = (from data in db.UserSecurities
                              where data.UserName == item.UserName && data.UserID != item.UserID && data.SegmentID == _Segment
                              select data).ToList();

                if (result.Count > 0)
                {
                    TempData["ErrorMessage"] = "Username Exists ...!";
                    return RedirectToAction("Edit", "Administrator");
                }
                else
                {
                    if (item.UserID == _User)
                    {
                        _IsActive = true;
                    }
                    else
                    {
                        string active = collection["isactive"];
                        _IsActive = Convert.ToBoolean(active);
                    }

                    string _UserName = item.UserName == null ? string.Empty : item.UserName.Trim();
                    int _Employee = item.EmployeeNo == null ? -1 : int.Parse(item.EmployeeNo.ToString());

                    var update = db.UserSecurities.SingleOrDefault(b => b.UserID == item.UserID);
                    if (update == null)
                    {
                        TempData["Error"] = "Administrator Update Failed...! Please Try Again";
                        return RedirectToAction("List", "Administrator");
                    }
                    else
                    {
                        update.UserName = _UserName;
                        //update.Password = _Password;
                        update.EmployeeNo = _Employee;
                        update.IsActive = _IsActive;
                        update.LastModifyDate = DateTime.Now;
                        update.LastModifyUser = _User;
                        _IUser.UpdateAdmin(update);

                        if (_Employee != -1)
                        {
                            // ----------------------------- Update Employee -----------------------------
                            var employee = db.EmployeeMasters.SingleOrDefault(b => b.EmployeeNo == _Employee);
                            if (employee.Flagged != true)
                            {
                                employee.Flagged = true;
                                db.SaveChanges();
                            }
                            //------------------------------------------------------------------------------
                        }

                        TempData["Updated"] = "Updated";
                        return RedirectToAction("List", "Administrator");
                    }
                }
            }
        }

        [HttpPost]
        public JsonResult Delete(string id)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(id)))
                {
                    return Json("Error", JsonRequestBehavior.AllowGet);
                }

                var data = _IUser.DeleteAdmin(Convert.ToInt32(id), _Segment);

                if (data > 0)
                {
                    TempData["Deleted"] = "Deleted";
                    return Json(data: true, behavior: JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(data: false, behavior: JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult Detail(int id)
        {
            ViewBag.Current = "User";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var result = db.UserSecurities.SingleOrDefault(b => b.UserID == id);

                if (result == null)
                {
                    return View(List());
                }
                else
                {
                    TempData["username"] = result.UserName;

                    return View(db.vw_UserSecurities.Where(x => x.UserID == id).FirstOrDefault<vw_UserSecurity>());
                }
            }
        }

        [HttpGet]
        public ActionResult AdminInfo(int id)
        {
            ViewBag.Current = "User";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var result = db.UserSecurities.SingleOrDefault(b => b.UserID == id);

                if (result == null)
                {
                    return View(List());
                }
                else
                {
                    TempData["username"] = result.UserName;

                    return View(db.vw_UserSecurities.Where(x => x.UserID == id).FirstOrDefault<vw_UserSecurity>());
                }
            }
        }

        [HttpGet]
        public ActionResult Security(int id)
        {
            ViewBag.Current = "User";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var result = db.UserSecurities.SingleOrDefault(b => b.UserID == id);

                if (result == null)
                {
                    return View(List());
                }
                else
                {
                    TempData["username"] = result.UserName;

                    return View(db.vw_UserSecurities.Where(x => x.UserID == id).FirstOrDefault<vw_UserSecurity>());
                }
            }
        }

        [HttpGet]
        public ActionResult Logs(int id)
        {
            ViewBag.Current = "User";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.SecurityActiveUser where data.SegmentID == _Segment && data.UserID == id && data.UserType == 2 orderby data.LoginInstance descending select data).ToList();
                return View(list);
            }
        }
    }
}