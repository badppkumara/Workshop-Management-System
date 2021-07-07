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
    [ValidateEmployeeSession]
    public class EmpController : Controller
    {
        IEmp _IEmp;
        public EmpController()
        {
            _IEmp = new EmpConcrete();
        }

        [HttpGet]
        public ActionResult Dashboard()
        {
            ViewBag.Current = "Dashboard";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {

                TempData["employees"] = _IEmp.GetEmployeesCount(_Segment);
                TempData["customers"] = _IEmp.GetCustomersCount(_Segment);
                TempData["suppliers"] = _IEmp.GetSuppliersCount(_Segment);
                TempData["vehicles"] = _IEmp.GetVehiclesCount(_Segment);

                //TempData["jobcomplete"] = _IEmp.GetJobComplete(_Segment);
                //TempData["jobpending"] = _IEmp.GetJobPending(_Segment);
                //TempData["jobopen"] = _IEmp.GetJobOpen(_Segment);

                return View();
            }
        }
    }
}