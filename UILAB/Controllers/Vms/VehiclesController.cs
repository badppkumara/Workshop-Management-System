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
    public class VehiclesController : Controller
    {
        VehicleTR objvehicles = new VehicleTR();
        VehicleMileageTR objmileage = new VehicleMileageTR();
        VehicleMakeTB objvehiclemake = new VehicleMakeTB();
        VehicleModelTB objvehiclemodel = new VehicleModelTB();
        CustomerTB objcustomer = new CustomerTB();

        IVehicle _IVehicle;
        IUser _IUser;

        public VehiclesController()
        {
            _IVehicle = new VehicleConcrete();
            _IUser = new UserConcrete();
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
        public ActionResult Create()
        {
            ViewBag.Current = "Vehicle";
            ViewBag.CurrentSub = "Vehicles";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var make = (from data in db.VehicleMakes orderby data.Make ascending select data).ToList();
                ViewBag.MakeList = new SelectList(make, "MakeID", "Make");

                var customer = (from data in db.Customers where data.SegmentID == _Segment orderby data.FullName ascending select data).ToList();
                ViewBag.CustomerList = new SelectList(customer, "CustomerID", "FullName");

                var model = (from data in db.VehicleModels orderby data.ModelName ascending select data).ToList();
                ViewBag.ModelList = new SelectList(model, "ModelID", "ModelName");

                var fuel = (from data in db.VehicleFuelTypes orderby data.FuelType ascending select data).ToList();
                ViewBag.FuelList = new SelectList(fuel, "FuelTypeID", "FuelType");

                var trans = (from data in db.VehicleTransTypes orderby data.TransType ascending select data).ToList();
                ViewBag.TransList = new SelectList(trans, "TransTypeID", "TransType");

                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(VehicleTR item, FormCollection collection, HttpPostedFileBase[] FileUpload)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var itemdata = db.VehicleTRs.SingleOrDefault(b => b.PlateNo == item.PlateNo);

                if (itemdata != null)
                {
                    TempData["ErrorMessage"] = itemdata.PlateNo;
                    return RedirectToAction("Create", "Vehicles");
                }
                else
                {
                    string _PlateNo = item.PlateNo == null ? string.Empty : item.PlateNo.Trim();
                    int _MakeID = item.MakeID == null ? -1 : int.Parse(item.MakeID.ToString());
                    int _CustomerID = item.CustomerID == null ? -1 : int.Parse(item.CustomerID.ToString());
                    int _ModelID = item.ModelID == null ? -1 : int.Parse(item.ModelID.ToString());
                    string _ChassisNo = item.ChassisNo == null ? string.Empty : item.ChassisNo.Trim();
                    string _EngineCC = item.EngineCC == null ? string.Empty : item.EngineCC.Trim();
                    string _Year = item.Year == null ? string.Empty : item.Year.Trim();
                    string _EngineNo = item.EngineNo == null ? string.Empty : item.EngineNo.Trim();
                    string _ModelNo = item.ModelNo == null ? string.Empty : item.ModelNo.Trim();
                    string _TyreSize = item.TyreSize == null ? string.Empty : item.TyreSize.Trim();
                    string _Color = item.Color == null ? string.Empty : item.Color.Trim();
                    int _TransTypeID = item.TransTypeID == null ? -1 : int.Parse(item.TransTypeID.ToString());
                    int _FuleType = item.FuelTypeID == null ? -1 : int.Parse(item.FuelTypeID.ToString());
                    string _Milage = collection["mileage"] == null ? string.Empty : collection["mileage"].Trim();
                    string _Hubo = collection["hubo"] == null ? string.Empty : collection["hubo"].Trim();
                    string _RUC = collection["ruc"] == null ? string.Empty : collection["ruc"].Trim();
                    string _Remark = item.Remark == null ? string.Empty : item.Remark.Trim();
                    DateTime _RegoExpiryDate = collection["regodate"] == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(collection["regodate"].ToString());
                    DateTime _WOFExpiryDate = collection["wofdate"] == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(collection["wofdate"].ToString());
                    int _RegisterID = collection["registertype"] == null ? -1 : int.Parse(collection["registertype"].ToString());

                    // New Items
                    string _NewMake = collection["newmake"] == null ? string.Empty : collection["newmake"].Trim();
                    string _NewModel = collection["newmodel"] == null ? string.Empty : collection["newmodel"].Trim();
                    int _NewMakeID = collection["newmakeid"] == null ? -1 : int.Parse(collection["newmakeid"]);
                    string _NewFName = collection["firstname"] == null ? string.Empty : collection["firstname"].Trim();
                    string _NewLName = collection["lastname"] == null ? string.Empty : collection["lastname"].Trim();
                    string _NewMobile = collection["mobile"] == null ? string.Empty : collection["mobile"].Trim();


                    string active = collection["checkall"];
                    bool _IsChecked = Convert.ToBoolean(active);

                    string active2 = collection["checkall2"];
                    bool _IsChecked2 = Convert.ToBoolean(active2);

                    objvehicles.SegmentID = _Segment;
                    objvehicles.PlateNo = _PlateNo;

                    if (_IsChecked == true)
                    {
                        if (_NewMake != "")
                        {
                            var makename = db.VehicleMakes.SingleOrDefault(b => b.Make == _NewMake);
                            if (makename == null)
                            {
                                objvehiclemake.Make = _NewMake;
                                objvehiclemake.FileID = -1;
                                objvehiclemake.Flagged = false;
                                objvehiclemake.LastModifyDate = DateTime.Now;
                                objvehiclemake.LastModifyUser = _User;
                                int _ReturnMakeID = _IVehicle.InsertMake(objvehiclemake);
                                Session["NewMake"] = Convert.ToString(_ReturnMakeID);
                                objvehicles.MakeID = int.Parse(Session["NewMake"].ToString());
                            }
                            else
                            {
                                Session["ExistingMake"] = Convert.ToString(makename.MakeID);
                                objvehicles.MakeID = int.Parse(Session["ExistingMake"].ToString());
                            }
                        }
                        else
                        {
                            objvehicles.MakeID = _NewMakeID;
                        }


                        if (_NewModel != "")
                        {
                            var modelname = db.VehicleModels.SingleOrDefault(b => b.ModelName == _NewModel && b.MakeID == objvehicles.MakeID);
                            if (modelname == null)
                            {
                                objvehiclemodel.MakeID = objvehicles.MakeID;
                                objvehiclemodel.ModelName = _NewModel;
                                objvehiclemodel.ModelTypeID = -1;
                                objvehiclemodel.Flagged = false;
                                objvehiclemodel.LastModifyDate = DateTime.Now;
                                objvehiclemodel.LastModifyUser = _User;
                                int _ReturnModelID = _IVehicle.InsertModel(objvehiclemodel);
                                Session["NewModel"] = Convert.ToString(_ReturnModelID);
                                objvehicles.ModelID = int.Parse(Session["NewModel"].ToString());
                            }
                            else
                            {
                                Session["ExistingModel"] = Convert.ToString(modelname.ModelID);
                                objvehicles.ModelID = int.Parse(Session["ExistingModel"].ToString());
                            }
                        }
                        else
                        {
                            objvehicles.ModelID = -1;
                        }
                    }
                    else
                    {
                        objvehicles.MakeID = _MakeID;
                        objvehicles.ModelID = _ModelID;
                    }

                    if (_IsChecked2 == true)
                    {
                        if (_NewFName != "")
                        {
                            var customer = db.Customers.SingleOrDefault(b => b.Mobile == _NewMobile && b.SegmentID == _Segment);

                            if (customer != null)
                            {
                                Session["ExistingCustomer"] = Convert.ToString(customer.CustomerID);
                                objvehicles.CustomerID = int.Parse(Session["ExistingCustomer"].ToString());
                            }
                            else
                            {
                                objcustomer.FirstName = _NewFName;
                                objcustomer.LastName = _NewLName;
                                objcustomer.Mobile = _NewMobile;
                                objcustomer.SegmentID = _Segment;
                                objcustomer.MiddleName = "";
                                objcustomer.OtherName = "";
                                objcustomer.GenderID = -1;
                                objcustomer.Company = "";
                                objcustomer.BuisnessNo = "";
                                objcustomer.GSTNo = "";
                                objcustomer.UserName = "";
                                objcustomer.Email = "";
                                objcustomer.Fax = "";
                                objcustomer.Phone = "";
                                objcustomer.Password = "";
                                objcustomer.ConfirmPassword = "";
                                objcustomer.Address1 = "";
                                objcustomer.Address2 = "";
                                objcustomer.Address3 = "";
                                objcustomer.DrivingLicenceNo = "";
                                objcustomer.PostalNo = "";
                                objcustomer.CountryID = -1;
                                objcustomer.RoleID = 4;
                                objcustomer.AddressID = -1;
                                objcustomer.Flagged = false;
                                objcustomer.IsActive = true;
                                objcustomer.IsContract = false;
                                objcustomer.LastModifyDate = DateTime.Now;
                                objcustomer.LastModifyUser = _User;
                                objcustomer.FullName = _NewFName + ' ' + _NewLName;
                                int _ReturnCustomerID = _IUser.InsertCustomer(objcustomer);
                                Session["NewCustomer"] = Convert.ToString(_ReturnCustomerID);
                                objvehicles.CustomerID = int.Parse(Session["NewCustomer"].ToString());
                            }
                        }
                        else
                        {
                            objvehicles.CustomerID = -1;
                        }
                    }
                    else
                    {
                        objvehicles.CustomerID = _CustomerID;
                    }

                    objvehicles.ChassisNo = _ChassisNo;
                    objvehicles.EngineNo = _EngineNo;
                    objvehicles.EngineCC = _EngineCC;
                    objvehicles.Year = _Year;
                    objvehicles.ModelNo = _ModelNo;
                    objvehicles.RegisterTypeID = _RegisterID;
                    objvehicles.TransTypeID = _TransTypeID;
                    objvehicles.TyreSize = _TyreSize;
                    objvehicles.Color = _Color;
                    objvehicles.FuelTypeID = _FuleType;
                    objvehicles.Milage = _Milage;
                    objvehicles.Hubo = _Hubo;
                    objvehicles.RUC = _RUC;
                    objvehicles.MileageID = -1;
                    objvehicles.Rego = "Rego";
                    objvehicles.RegoExpiryDate = _RegoExpiryDate;
                    objvehicles.WOF = "WOF";
                    objvehicles.WOFExpiryDate = _WOFExpiryDate;
                    objvehicles.Remark = _Remark;
                    objvehicles.Flagged = false;
                    objvehicles.LastModifyDate = DateTime.Now;
                    objvehicles.LastModifyUser = _User;
                    int _VehicleID = _IVehicle.InsertVehicle(objvehicles);

                    if (_Milage != "" || _Hubo != "" || _RUC != "")
                    {
                        objmileage.SegmentID = _Segment;
                        objmileage.Mileage = _Milage;
                        objmileage.VehicleID = _VehicleID;
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
                        var updatemileage = db.VehicleTRs.SingleOrDefault(b => b.VehicleID == _VehicleID);
                        if (updatemileage != null)
                        {
                            updatemileage.MileageID = _MileageID;
                            updatemileage.LastModifyDate = DateTime.Now;
                            updatemileage.LastModifyUser = _User;
                            db.SaveChanges();
                        }
                    }

                    // ----------------------------- Update Flag Vehicle Model  -----------------------------
                    var update = db.VehicleModels.SingleOrDefault(b => b.ModelID == _ModelID);
                    if (update.Flagged == false)
                    {
                        update.Flagged = true;
                        update.LastModifyDate = DateTime.Now;
                        update.LastModifyUser = _User;
                        db.SaveChanges();
                    }
                    //------------------------------------------------------------------------------

                    // ----------------------------- Update Flag Customer  -----------------------------
                    var updatecustomer = db.Customers.SingleOrDefault(b => b.CustomerID == _CustomerID);
                    if (updatecustomer != null)
                    {
                        if (update.Flagged == false)
                        {
                            update.Flagged = true;
                            update.LastModifyDate = DateTime.Now;
                            update.LastModifyUser = _User;
                            db.SaveChanges();
                        }
                    }
                    //------------------------------------------------------------------------------

                    TempData["Success"] = "Success";
                    return RedirectToAction("List", "Vehicles");
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

                    var customer = (from data in db.Customers where data.SegmentID == _Segment orderby data.FullName ascending select data).ToList();
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
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var itemdata = db.VehicleTRs.SingleOrDefault(b => b.PlateNo == item.PlateNo && b.VehicleID != item.VehicleID);

                if (itemdata != null)
                {
                    TempData["ErrorMessage"] = itemdata.PlateNo;
                    return RedirectToAction("Edit", "Vehicles");
                }
                else
                {
                    string _PlateNo = item.PlateNo == null ? string.Empty : item.PlateNo.Trim();
                    int _MakeID = item.MakeID == null ? -1 : int.Parse(item.MakeID.ToString());
                    int _CustomerID = item.CustomerID == null ? -1 : int.Parse(item.CustomerID.ToString());
                    int _ModelID = item.ModelID == null ? -1 : int.Parse(item.ModelID.ToString());
                    string _ChassisNo = item.ChassisNo == null ? string.Empty : item.ChassisNo.Trim();
                    string _EngineCC = item.EngineCC == null ? string.Empty : item.EngineCC.Trim();
                    string _Year = item.Year == null ? string.Empty : item.Year.Trim();
                    string _EngineNo = item.EngineNo == null ? string.Empty : item.EngineNo.Trim();
                    string _ModelNo = item.ModelNo == null ? string.Empty : item.ModelNo.Trim();
                    string _TyreSize = item.TyreSize == null ? string.Empty : item.TyreSize.Trim();
                    string _Color = item.Color == null ? string.Empty : item.Color.Trim();
                    int _TransTypeID = item.TransTypeID == null ? -1 : int.Parse(item.TransTypeID.ToString());
                    int _FuleType = item.FuelTypeID == null ? -1 : int.Parse(item.FuelTypeID.ToString());
                    string _Milage = collection["mileage"] == null ? string.Empty : collection["mileage"].Trim();
                    string _Hubo = collection["hubo"] == null ? string.Empty : collection["hubo"].Trim();
                    string _RUC = collection["ruc"] == null ? string.Empty : collection["ruc"].Trim();
                    string _Remark = item.Remark == null ? string.Empty : item.Remark.Trim();
                    DateTime _RegoExpiryDate = collection["regodate"] == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(collection["regodate"].ToString());
                    DateTime _WOFExpiryDate = collection["wofdate"] == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(collection["wofdate"].ToString());
                    int _RegisterID = collection["registertype"] == null ? -1 : int.Parse(collection["registertype"].ToString());

                    // New Items
                    string _NewMake = collection["newmake"] == null ? string.Empty : collection["newmake"].Trim();
                    string _NewModel = collection["newmodel"] == null ? string.Empty : collection["newmodel"].Trim();
                    int _NewMakeID = collection["newmakeid"] == null ? -1 : int.Parse(collection["newmakeid"]);
                    string _NewFName = collection["firstname"] == null ? string.Empty : collection["firstname"].Trim();
                    string _NewLName = collection["lastname"] == null ? string.Empty : collection["lastname"].Trim();
                    string _NewMobile = collection["mobile"] == null ? string.Empty : collection["mobile"].Trim();


                    string active = collection["checkall"];
                    bool _IsChecked = Convert.ToBoolean(active);

                    string active2 = collection["checkall2"];
                    bool _IsChecked2 = Convert.ToBoolean(active2);

                    var updatedata = db.VehicleTRs.SingleOrDefault(b => b.VehicleID == item.VehicleID);
                    if (updatedata == null)
                    {
                        TempData["Error"] = "Vehicle Update Failed...! Please Try Again";
                        return View(List());
                    }
                    else
                    {
                        if (_IsChecked == true)
                        {
                            if (_NewMake != "")
                            {
                                var makename = db.VehicleMakes.SingleOrDefault(b => b.Make == _NewMake);
                                if (makename == null)
                                {
                                    objvehiclemake.Make = _NewMake;
                                    objvehiclemake.FileID = -1;
                                    objvehiclemake.Flagged = false;
                                    objvehiclemake.LastModifyDate = DateTime.Now;
                                    objvehiclemake.LastModifyUser = _User;
                                    int _ReturnMakeID = _IVehicle.InsertMake(objvehiclemake);
                                    Session["NewMake"] = Convert.ToString(_ReturnMakeID);
                                    updatedata.MakeID = int.Parse(Session["NewMake"].ToString());
                                }
                                else
                                {
                                    Session["ExistingMake"] = Convert.ToString(makename.MakeID);
                                    updatedata.MakeID = int.Parse(Session["ExistingMake"].ToString());
                                }
                            }
                            else
                            {
                                updatedata.MakeID = _NewMakeID;
                            }


                            if (_NewModel != "")
                            {
                                var modelname = db.VehicleModels.SingleOrDefault(b => b.ModelName == _NewModel && b.MakeID == objvehicles.MakeID);
                                if (modelname == null)
                                {
                                    objvehiclemodel.MakeID = objvehicles.MakeID;
                                    objvehiclemodel.ModelName = _NewModel;
                                    objvehiclemodel.ModelTypeID = -1;
                                    objvehiclemodel.Flagged = false;
                                    objvehiclemodel.LastModifyDate = DateTime.Now;
                                    objvehiclemodel.LastModifyUser = _User;
                                    int _ReturnModelID = _IVehicle.InsertModel(objvehiclemodel);
                                    Session["NewModel"] = Convert.ToString(_ReturnModelID);
                                    updatedata.ModelID = int.Parse(Session["NewModel"].ToString());
                                }
                                else
                                {
                                    Session["ExistingModel"] = Convert.ToString(modelname.ModelID);
                                    updatedata.ModelID = int.Parse(Session["ExistingModel"].ToString());
                                }
                            }
                            else
                            {
                                updatedata.ModelID = -1;
                            }
                        }
                        else
                        {
                            updatedata.MakeID = _MakeID;
                            updatedata.ModelID = _ModelID;
                        }

                        if (_IsChecked2 == true)
                        {
                            if (_NewFName != "")
                            {
                                objcustomer.FirstName = _NewFName;
                                objcustomer.LastName = _NewLName;
                                objcustomer.Mobile = _NewMobile;
                                objcustomer.SegmentID = _Segment;
                                objcustomer.MiddleName = "";
                                objcustomer.OtherName = "";
                                objcustomer.GenderID = -1;
                                objcustomer.Company = "";
                                objcustomer.BuisnessNo = "";
                                objcustomer.GSTNo = "";
                                objcustomer.UserName = "";
                                objcustomer.Email = "";
                                objcustomer.Fax = "";
                                objcustomer.Phone = "";
                                objcustomer.Password = "";
                                objcustomer.ConfirmPassword = "";
                                objcustomer.Address1 = "";
                                objcustomer.Address2 = "";
                                objcustomer.Address3 = "";
                                objcustomer.DrivingLicenceNo = "";
                                objcustomer.PostalNo = "";
                                objcustomer.CountryID = -1;
                                objcustomer.RoleID = 4;
                                objcustomer.AddressID = -1;
                                objcustomer.Flagged = false;
                                objcustomer.IsActive = true;
                                objcustomer.IsContract = false;
                                objcustomer.LastModifyDate = DateTime.Now;
                                objcustomer.LastModifyUser = _User;
                                objcustomer.FullName = _NewFName + ' ' + _NewLName;
                                int _ReturnCustomerID = _IUser.InsertCustomer(objcustomer);
                                Session["NewCustomer"] = Convert.ToString(_ReturnCustomerID);
                                objvehicles.CustomerID = int.Parse(Session["NewCustomer"].ToString());
                            }
                            else
                            {
                                objvehicles.CustomerID = -1;
                            }
                        }


                        updatedata.PlateNo = _PlateNo;
                        //updatedata.MakeID = _MakeID;
                        updatedata.CustomerID = _CustomerID;
                        updatedata.RegisterTypeID = _RegisterID;
                        //updatedata.ModelID = _ModelID;
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
                        updatedata.Color = _Color;
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
                        return RedirectToAction("List", "Vehicles");
                    }
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

                var data = _IVehicle.DeleteVehicle(Convert.ToInt32(id));

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
        public ActionResult Details(int id)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

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

        public ActionResult FillModel(int brand)
        {
            using (var db = new DatabaseContext())
            {
                var vehmodels = (from data in db.VehicleModels where data.MakeID == brand orderby data.ModelName ascending select data).ToList();
                return Json(vehmodels, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult DetailJobList(int id)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.vw_JobTRs where data.VehicleID == id && data.SegmentID == _Segment select data).ToList();
                return View(list);
            }
        }

        [HttpGet]
        public ActionResult DetailInvoiceList(int id)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.vw_SalesInvoiceTRs where data.VehicleID == id && data.SegmentID == _Segment select data).ToList();
                return View(list);
            }
        }
    }
}