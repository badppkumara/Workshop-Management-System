using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UILAB.Concrete;
using UILAB.Filters;
using UILAB.Interface;
using UILAB.Models;

namespace UILAB.Controllers
{
    [ValidateAdminSession]
    public class AdminController : Controller
    {
        //Ad objapprovalscheme = new ApprovalSchemeMaster();

        IAdmin _IAdmin;
        public AdminController()
        {
            _IAdmin = new AdminConcrete();
        }

        [HttpGet]
        public ActionResult Dashboard()
        {
            ViewBag.Current = "Dashboard";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {

                TempData["employees"] = _IAdmin.GetEmployeesCount(_Segment);
                TempData["customers"] = _IAdmin.GetCustomersCount(_Segment);
                TempData["suppliers"] = _IAdmin.GetSuppliersCount(_Segment);
                TempData["vehicles"] = _IAdmin.GetVehiclesCount(_Segment);

                TempData["jobcomplete"] = _IAdmin.GetJobComplete(_Segment);
                TempData["jobpending"] = _IAdmin.GetJobPending(_Segment);
                TempData["jobopen"] = _IAdmin.GetJobOpen(_Segment);

                return View();
            }
        }
    }
}