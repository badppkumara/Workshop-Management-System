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
    public class VehicleModelController : Controller
    {
        VehicleModelTB objvehiclemodel = new VehicleModelTB();
        IVehicle _IVehicle;

        public VehicleModelController()
        {
            _IVehicle = new VehicleConcrete();
        }

        [HttpGet]
        public ActionResult List()
        {
            ViewBag.Current = "Vehicle";
            ViewBag.CurrentSub = "Model";

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.VehicleModels orderby data.ModelName descending select data).ToList();
                return View(list);
            }
        }

        [HttpGet]
        public ActionResult Create(int id = 0)
        {
            ViewBag.Current = "Vehicle";
            ViewBag.CurrentSub = "Model";

            if (id == 0)
            {
                using (var db = new DatabaseContext())
                {
                    ViewBag.SubmitValue = "Save";
                    ViewBag.SubmitHeader = "Create";

                    var make = (from data in db.VehicleMakes orderby data.Make ascending select data).ToList();
                    ViewBag.MakeList = new SelectList(make, "MakeID", "Make");

                    var type = (from data in db.VehicleTypes orderby data.ModelType ascending select data).ToList();
                    ViewBag.TypeList = new SelectList(type, "ModelTypeID", "ModelType");

                    return View();
                }
            }
            else
            {
                using (var db = new DatabaseContext())
                {
                    ViewBag.SubmitValue = "Update";
                    ViewBag.SubmitHeader = "Update";
                    TempData["buttonMsg"] = "ClearButton";

                    var make = (from data in db.VehicleMakes orderby data.Make ascending select data).ToList();
                    ViewBag.MakeList = new SelectList(make, "MakeID", "Make");

                    var type = (from data in db.VehicleTypes orderby data.ModelType ascending select data).ToList();
                    ViewBag.TypeList = new SelectList(type, "ModelTypeID", "ModelType");

                    return View(db.VehicleModels.Where(x => x.ModelID == id).FirstOrDefault<VehicleModelTB>());
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(VehicleModelTB item)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var itemdata = db.VehicleModels.SingleOrDefault(b => b.ModelName == item.ModelName && b.MakeID == item.MakeID && b.ModelID != item.ModelID);

                if (itemdata != null)
                {
                    TempData["ErrorMessage"] = itemdata.ModelName;
                    return RedirectToAction("List", "VehicleModel");
                }
                else
                {
                    string _ModelName = item.ModelName == null ? string.Empty : item.ModelName.Trim();
                    int _MakeID = item.MakeID == null ? -1 : int.Parse(item.MakeID.ToString());
                    int _TypeID = item.ModelTypeID == null ? -1 : int.Parse(item.ModelTypeID.ToString());

                    if (item.ModelID != 0)
                    {
                        item.ModelName = _ModelName;
                        item.MakeID = _MakeID;
                        item.ModelTypeID = _TypeID;
                        item.LastModifyDate = DateTime.Now;
                        item.LastModifyUser = _User;
                        _IVehicle.UpdateModel(item);

                        TempData["successMsg"] = " " + _ModelName + " Updated";
                    }
                    else
                    {
                        objvehiclemodel.ModelName = _ModelName;
                        objvehiclemodel.MakeID = _MakeID;
                        objvehiclemodel.ModelTypeID = _TypeID;
                        objvehiclemodel.LastModifyDate = DateTime.Now;
                        objvehiclemodel.LastModifyUser = _User;
                        int _ModelID = _IVehicle.InsertModel(objvehiclemodel);

                        // ----------------------------- Update Make -----------------------------
                        var update = db.VehicleMakes.SingleOrDefault(b => b.MakeID == _MakeID);
                        if (update.Flagged == false)
                        {
                            update.Flagged = true;
                            update.LastModifyDate = DateTime.Now;
                            update.LastModifyUser = _User;
                            db.SaveChanges();
                        }
                        //------------------------------------------------------------------------------
                        TempData["successMsg"] = " " + _ModelName + " Created";
                    }

                    TempData["Success"] = "Success";
                    return RedirectToAction("List", "VehicleModel");
                }
            }
        }

        [HttpPost]
        public JsonResult Delete(string id)
        {
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(id)))
                {
                    return Json("Error", JsonRequestBehavior.AllowGet);
                }

                var data = _IVehicle.DeleteModel(Convert.ToInt32(id), _User);

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