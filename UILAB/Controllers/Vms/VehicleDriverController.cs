using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UILAB.Concrete;
using UILAB.Filters;
using UILAB.Interface;
using UILAB.Models;

namespace UILAB.Controllers.Vms
{
    public class VehicleDriverController : Controller
    {
        VehicleDriverTR objdriver = new VehicleDriverTR();
        IVehicle _IVehicle;

        [HttpGet]
        public ActionResult List()
        {
            ViewBag.Current = "Trip";
            ViewBag.CurrentSub = "Driver";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.VehicleDriverTRs where data.SegmentID == _Segment orderby data.DriverID ascending select data).ToList();
                return View(list);
            }
        }

        [HttpGet]
        public ActionResult Create(int id = 0)
        {
            ViewBag.Current = "Trip";
            ViewBag.CurrentSub = "Driver";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            if (id == 0)
            {
                ViewBag.SubmitValue = "Save";
                ViewBag.SubmitHeader = "Create Driver";

                using (var db = new DatabaseContext())
                {
                    var employee = (from data in db.vw_EmployeeMasters orderby data.FullName ascending select data).ToList();
                    ViewBag.EmpList = new SelectList(employee, "EmployeeNo", "FullName");

                    var gender = (from data in db.GenderMasters orderby data.Gender ascending select data).ToList();
                    ViewBag.GenderList = new SelectList(gender, "GenderID", "Gender");

                    var country = (from data in db.CountryMasters orderby data.CountryName ascending select data).ToList();
                    ViewBag.CountryList = new SelectList(country, "CountryID", "CountryName");

                    return View();
                }
            }
            else
            {
                using (var db = new DatabaseContext())
                {
                    ViewBag.SubmitValue = "Update";
                    ViewBag.SubmitHeader = "Update Driver";
                    TempData["buttonMsg"] = "ClearButton";

                    var employee = (from data in db.vw_EmployeeMasters orderby data.FullName ascending select data).ToList();
                    ViewBag.EmpList = new SelectList(employee, "EmployeeNo", "FullName");

                    var gender = (from data in db.GenderMasters orderby data.Gender ascending select data).ToList();
                    ViewBag.GenderList = new SelectList(gender, "GenderID", "Gender");

                    var country = (from data in db.CountryMasters orderby data.CountryName ascending select data).ToList();
                    ViewBag.CountryList = new SelectList(country, "CountryID", "CountryName");

                    return View(db.VehicleDriverTRs.Where(x => x.DriverID == id).FirstOrDefault<VehicleDriverTR>());
                }
            }
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(VehicleDriverTR item)
        //{
        //    int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
        //    int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

        //    using (var db = new DatabaseContext())
        //    {

        //    }
        //}

        [HttpPost]
        public JsonResult Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(id)))
                {
                    return Json("Error", JsonRequestBehavior.AllowGet);
                }

                var data = _IVehicle.DeleteDriver(Convert.ToInt32(id));

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
    }
}