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
    [ValidateEmployeeSession]
    public class EmpVehiclesController : Controller
    {
        VehicleTR objvehicles = new VehicleTR();
        VehicleMileageTR objmileage = new VehicleMileageTR();
        IVehicle _IVehicle;

        public EmpVehiclesController()
        {
            _IVehicle = new VehicleConcrete();
        }

        [HttpGet]
        public ActionResult List()
        {
            ViewBag.Current = "Vehicle";
            ViewBag.CurrentSub = "Vehicles";
            TempData["Error"] = "";

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.vw_VehicleTRs orderby data.PlateNo descending select data).ToList();
                return View(list);
            }
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var result = db.vw_VehicleTRs.SingleOrDefault(b => b.VehicleID == id);

                if (result == null)
                {
                    return View(List());
                }
                else
                {
                    return View(db.vw_VehicleTRs.Where(x => x.VehicleID == id).FirstOrDefault<vw_VehicleTR>());
                }
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            ViewBag.Current = "Vehicle";
            ViewBag.CurrentSub = "Vehicles";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var result = db.VehicleTRs.SingleOrDefault(b => b.VehicleID == id);

                if (result == null)
                {
                    return View(List());
                }
                else
                {
                    var make = (from data in db.VehicleMakes orderby data.Make ascending select data).ToList();
                    ViewBag.MakeList = new SelectList(make, "MakeID", "Make");

                    var customer = (from data in db.Customers where data.SegmentID == _Segment orderby data.FirstName ascending select data).ToList();
                    ViewBag.CustomerList = new SelectList(customer, "CustomerID", "FullName");

                    var model = (from data in db.VehicleModels orderby data.ModelName ascending select data).ToList();
                    ViewBag.ModelList = new SelectList(model, "ModelID", "ModelName");

                    var fuel = (from data in db.VehicleFuelTypes orderby data.FuelType ascending select data).ToList();
                    ViewBag.FuelList = new SelectList(fuel, "FuelTypeID", "FuelType");

                    var trans = (from data in db.VehicleTransTypes orderby data.TransType ascending select data).ToList();
                    ViewBag.TransList = new SelectList(trans, "TransTypeID", "TransType");

                    var mileage = db.VehicleMileageTRs.SingleOrDefault(b => b.MileageID == result.MileageID && b.Updated == true);
                    if (mileage != null)
                    {
                        TempData["mileage"] = mileage.Mileage;
                        TempData["hubo"] = mileage.Hubo;
                        TempData["ruc"] = mileage.RUC;

                        if (Convert.ToDateTime(mileage.RegoExpiryDate).ToString("yyyy-MM-dd") == "1900-01-01")
                        {
                            TempData["regodate"] = "";
                        }
                        else
                        {
                            TempData["regodate"] = Convert.ToDateTime(mileage.RegoExpiryDate).ToString("dd-MM-yyyy");
                        }

                        if (Convert.ToDateTime(mileage.WOFExpiryDate).ToString("yyyy-MM-dd") == "1900-01-01")
                        {
                            TempData["wofdate"] = "";
                        }
                        else
                        {
                            TempData["wofdate"] = Convert.ToDateTime(mileage.WOFExpiryDate).ToString("dd-MM-yyyy");
                        }
                    }
                    else
                    {
                        TempData["mileage"] = "0";
                        TempData["hubo"] = "0";
                        TempData["ruc"] = "0";
                        TempData["regodate"] = "";
                        TempData["wofdate"] = "";

                    }
                    return View(db.VehicleTRs.Where(x => x.VehicleID == id).FirstOrDefault<VehicleTR>());
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(VehicleTR item, FormCollection collection)
        {
            int _User = Convert.ToInt32(HttpContext.Session["Employee"]);
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var itemdata = db.VehicleTRs.SingleOrDefault(b => b.PlateNo == item.PlateNo && b.VehicleID != item.VehicleID);

                if (itemdata != null)
                {
                    TempData["ErrorMessage"] = itemdata.PlateNo;
                    return RedirectToAction("Edit", "EmpVehicles");
                }
                else
                {
                    //string _PlateNo = item.PlateNo == null ? string.Empty : item.PlateNo.Trim();
                    int _MakeID = item.MakeID == null ? -1 : int.Parse(item.MakeID.ToString());
                    int _CustomerID = item.CustomerID == null ? -1 : int.Parse(item.CustomerID.ToString());
                    int _ModelID = item.ModelID == null ? -1 : int.Parse(item.ModelID.ToString());
                    string _ChassisNo = item.ChassisNo == null ? string.Empty : item.ChassisNo.Trim();
                    string _EngineCC = item.EngineCC == null ? string.Empty : item.EngineCC.Trim();
                    string _Year = item.Year == null ? string.Empty : item.Year.Trim();
                    string _EngineNo = item.EngineNo == null ? string.Empty : item.EngineNo.Trim();
                    string _ModelNo = item.ModelNo == null ? string.Empty : item.ModelNo.Trim();
                    string _TyreSize = item.TyreSize == null ? string.Empty : item.TyreSize.Trim();
                    int _TransTypeID = item.TransTypeID == null ? -1 : int.Parse(item.TransTypeID.ToString());
                    int _FuleType = item.FuelTypeID == null ? -1 : int.Parse(item.FuelTypeID.ToString());
                    string _Milage = collection["mileage"] == null ? string.Empty : collection["mileage"].Trim();
                    string _Hubo = collection["hubo"] == null ? string.Empty : collection["hubo"].Trim();
                    string _RUC = collection["ruc"] == null ? string.Empty : collection["ruc"].Trim();
                    string _Remark = item.Remark == null ? string.Empty : item.Remark.Trim();
                    DateTime _RegoExpiryDate = collection["regodate"] == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(collection["regodate"].ToString());
                    DateTime _WOFExpiryDate = collection["wofdate"] == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(collection["wofdate"].ToString());
                    //int _RegisterID = collection["registertype"] == null ? -1 : int.Parse(collection["registertype"].ToString());

                    var updatedata = db.VehicleTRs.SingleOrDefault(b => b.VehicleID == item.VehicleID);
                    if (updatedata == null)
                    {
                        TempData["Error"] = "Vehicle Update Failed...! Please Try Again";
                        return View(List());
                    }
                    else
                    {
                        //updatedata.PlateNo = _PlateNo;
                        updatedata.MakeID = _MakeID;
                        updatedata.CustomerID = _CustomerID;
                        updatedata.RegisterTypeID = -1;
                        updatedata.ModelID = _ModelID;
                        updatedata.ChassisNo = _ChassisNo;
                        updatedata.EngineNo = _EngineNo;
                        updatedata.EngineCC = _EngineCC;
                        updatedata.Year = _Year;
                        updatedata.FuelTypeID = _FuleType;
                        updatedata.TransTypeID = _TransTypeID;
                        updatedata.ModelNo = _ModelNo;
                        updatedata.Milage = _Milage;
                        updatedata.Hubo = _Hubo;
                        updatedata.TyreSize = _TyreSize;
                        updatedata.RUC = _RUC;
                        updatedata.Remark = _Remark;
                        updatedata.RegoExpiryDate = _RegoExpiryDate;
                        updatedata.WOFExpiryDate = _WOFExpiryDate;
                        updatedata.LastModifyDate = DateTime.Now;
                        updatedata.LastModifyUser = _User;
                        _IVehicle.UpdateVehicle(updatedata);

                        var updatemileage = db.VehicleMileageTRs.SingleOrDefault(b => b.MileageID == updatedata.MileageID && b.Updated == true);

                        if (updatemileage != null)
                        {
                            updatemileage.Mileage = _Milage;
                            updatemileage.Hubo = _Hubo;
                            updatemileage.RUC = _RUC;
                            updatemileage.RegoExpiryDate = _RegoExpiryDate;
                            updatemileage.WOFExpiryDate = _WOFExpiryDate;
                            updatemileage.Updated = true;
                            _IVehicle.UpdateMileage(updatemileage);
                        }
                        else
                        {
                            objmileage.SegmentID = _Segment;
                            objmileage.Mileage = _Milage;
                            objmileage.VehicleID = updatedata.VehicleID;
                            objmileage.Hubo = _Hubo;
                            objmileage.RUC = _RUC;
                            objmileage.Rego = "Rego";
                            objmileage.RegoExpiryDate = _RegoExpiryDate;
                            objmileage.WOF = "WOF";
                            objmileage.WOFExpiryDate = _WOFExpiryDate;
                            objmileage.Updated = true;
                            objmileage.Flagged = false;
                            objmileage.LastModifyDate = DateTime.Now;
                            objmileage.LastModifyUser = _User;
                            int _MileageID = _IVehicle.InsertMileage(objmileage);

                            // ----------------------------- Update Mileage ID -----------------------------
                            var mileageupdate = db.VehicleTRs.SingleOrDefault(b => b.VehicleID == updatedata.VehicleID);
                            if (mileageupdate != null)
                            {
                                mileageupdate.MileageID = _MileageID;
                                mileageupdate.LastModifyDate = DateTime.Now;
                                mileageupdate.LastModifyUser = _User;
                                db.SaveChanges();
                            }
                        }

                        TempData["Updated"] = "Updated";
                        return RedirectToAction("List", "EmpVehicles");
                    }
                }
            }
        }
    }
}