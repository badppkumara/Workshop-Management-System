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
    [ValidateAdminSession]
    public class VehicleBodyTypeController : Controller
    {
        VehicleTypeTB objbodytype = new VehicleTypeTB();
        IVehicle _IVehicle;

        public VehicleBodyTypeController()
        {
            _IVehicle = new VehicleConcrete();
        }

        [HttpGet]
        public ActionResult List()
        {
            ViewBag.Current = "Vehicle";
            ViewBag.CurrentSub = "BodyType";

            TempData["Error"] = "";

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.VehicleTypes orderby data.ModelType descending select data).ToList();
                return View(list);
            }
        }

        [HttpGet]
        public ActionResult Create(int id = 0)
        {
            ViewBag.Current = "Vehicle";
            ViewBag.CurrentSub = "BodyType";

            if (id == 0)
            {
                ViewBag.SubmitValue = "Save";
                ViewBag.SubmitHeader = "Create";
                return View();
            }
            else
            {
                using (var db = new DatabaseContext())
                {
                    ViewBag.SubmitValue = "Update";
                    ViewBag.SubmitHeader = "Update";
                    TempData["buttonMsg"] = "ClearButton";
                    return View(db.VehicleTypes.Where(x => x.ModelTypeID == id).FirstOrDefault<VehicleTypeTB>());
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(VehicleTypeTB item)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var itemdata = db.VehicleTypes.SingleOrDefault(b => b.ModelType == item.ModelType && b.ModelTypeID != item.ModelTypeID);

                if (itemdata != null)
                {
                    TempData["errorMessage"] = itemdata.ModelType;
                    return RedirectToAction("List", "VehicleBodyType");
                }
                else
                {
                    string _ModelType = item.ModelType == null ? string.Empty : item.ModelType.Trim();

                    if (item.ModelTypeID != 0)
                    {
                        var updatedata = db.VehicleTypes.SingleOrDefault(b => b.ModelTypeID == item.ModelTypeID);
                        updatedata.ModelType = _ModelType;
                        updatedata.LastModifyDate = DateTime.Now;
                        updatedata.LastModifyUser = _User;
                        _IVehicle.UpdateModelType(updatedata);
                        TempData["successMsg"] = " " + _ModelType + " Updated..!";
                    }
                    else
                    {
                        objbodytype.ModelType = _ModelType;
                        objbodytype.Flagged = false;
                        objbodytype.LastModifyDate = DateTime.Now;
                        objbodytype.LastModifyUser = _User;
                        int _ModelTypeID = _IVehicle.InsertModelType(objbodytype);
                        TempData["successMsg"] = " " + _ModelType + " Created..!";
                    }
                    TempData["Success"] = "Success";
                    
                    return RedirectToAction("List", "VehicleBodyType");
                }
            }
        }

        [HttpPost]
        public JsonResult Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(id)))
                {
                    return Json("Error", JsonRequestBehavior.AllowGet);
                }

                var data = _IVehicle.DeleteModelType(Convert.ToInt32(id));

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