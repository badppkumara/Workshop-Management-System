using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UILAB.Concrete;
using UILAB.Filters;
using UILAB.Interface;
using UILAB.Models;
using System.Net;
using System.Web.UI;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;
using System.Text;
using System.Web.UI.WebControls;

namespace UILAB.Controllers.Wms
{
    [ValidateAdminSession]
    public class JobController : Controller
    {
        JobTypeTB objtype = new JobTypeTB();
        JobTR objjob = new JobTR();
        JobTaskTR objjobtask = new JobTaskTR();
        JobTasksTB objjobtaskmaster = new JobTasksTB();
        JobAssignEmpTR objjobemployee = new JobAssignEmpTR();
        VehicleTR objvehicles = new VehicleTR();
        VehicleMakeTB objvehiclemake = new VehicleMakeTB();
        VehicleModelTB objvehiclemodel = new VehicleModelTB();
        SalesInvoiceTR objinvoice = new SalesInvoiceTR();
        SalesInvoiceListTR objinvoicelist = new SalesInvoiceListTR();
        CustomerTB objcustomer = new CustomerTB();
        VehicleMileageTR objmileage = new VehicleMileageTR();
        ApprovalHeaderTR objapprovalheader = new ApprovalHeaderTR();
        ApprovalDetailTR objapprovaldetail = new ApprovalDetailTR();
        FileVehicle objfilevehicle = new FileVehicle();

        IJob _IJob;
        ISales _ISales;
        IVehicle _IVehicle;
        IUser _IUser;
        IApproval _IApproval;

        public JobController()
        {
            _IJob = new JobConcrete();
            _ISales = new SalesConcerete();
            _IVehicle = new VehicleConcrete();
            _IUser = new UserConcrete();
            _IApproval = new ApprovalConcrete();
        }

        [HttpGet]
        public ActionResult List()
        {
            ViewBag.Current = "Jobs";
            ViewBag.CurrentSub = "Job";
            TempData["Error"] = "";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.vw_JobTRs where data.SegmentID == _Segment orderby data.JobTRID descending select data).ToList();
                return View(list.ToList());
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Current = "Jobs";
            ViewBag.CurrentSub = "Job";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var vehicle = (from data in db.VehicleTRs orderby data.PlateNo ascending select data).ToList();
                ViewBag.VehicleList = new SelectList(vehicle, "VehicleID", "PlateNo");

                var jobtype = (from data in db.JobPackages where data.SegmentID == _Segment && data.PackageID != 1 orderby data.PackageName ascending select data).ToList();
                ViewBag.TypeList = new SelectList(jobtype, "PackageID", "PackageName");

                var task = (from data in db.JobTasksTBs where data.SegmentID == _Segment orderby data.TaskName ascending select data).ToList();
                ViewBag.TaskList = new SelectList(task, "TaskID", "TaskName");

                var make = (from data in db.VehicleMakes orderby data.Make ascending select data).ToList();
                ViewBag.MakeList = new SelectList(make, "MakeID", "Make");

                var model = (from data in db.VehicleModels orderby data.ModelName ascending select data).ToList();
                ViewBag.ModelList = new SelectList(model, "ModelID", "ModelName");

                //var package = (from data in db.JobPackages orderby data.PackageName ascending select data).ToList();
                //ViewBag.PackageList = new SelectList(package, "PackageID", "PackageName");

                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(JobTR item, FormCollection collection, HttpPostedFileBase[] FileUpload)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var jobtype = db.JobTypes.SingleOrDefault(b => b.JobTypeName == "Custom");
                var vehicle = db.VehicleTRs.SingleOrDefault(b => b.VehicleID == item.VehicleID);

                string _JobTRID = "";
                int _StatusID = item.StatusID == null ? -1 : int.Parse(item.StatusID.ToString());
                int _JobTypeID = jobtype.JobTypeID.ToString() == null ? -1 : int.Parse(jobtype.JobTypeID.ToString());
                int _JobPackageID = item.PackageID == null ? -1 : int.Parse(item.PackageID.ToString());
                int _VehicleID = item.VehicleID == null ? -1 : int.Parse(item.VehicleID.ToString());
                int _ApprovalSettingID = jobtype.ApprovalSettingID == null ? -1 : int.Parse(jobtype.ApprovalSettingID.ToString());
                string _Milage = collection["mileage"] == null ? string.Empty : collection["mileage"].Trim();
                string _Hubo = collection["hubo"] == null ? string.Empty : collection["hubo"].Trim();
                string _RUC = collection["ruc"] == null ? string.Empty : collection["ruc"].Trim();
                string _Remark = item.Remark == null ? string.Empty : item.Remark.Trim();
                string _Description = item.Description == null ? string.Empty : item.Description.Trim();
                DateTime _JobStartDate = collection["startdate"] == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(collection["startdate"].ToString());
                DateTime _JobFinishDate = collection["finishdate"] == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(collection["finishdate"].ToString());
                DateTime _RegoExpiryDate = collection["regodate"] == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(collection["regodate"].ToString());
                DateTime _WOFExpiryDate = collection["wofdate"] == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(collection["wofdate"].ToString());

                string newvehiclecheck = collection["checkall"];
                bool _IsNewVehicle = Convert.ToBoolean(newvehiclecheck);

                //string jobcomplete = collection["jobcomplete"];
                //bool _IsComplete = Convert.ToBoolean(jobcomplete);

                if (_IsNewVehicle == true)
                {
                    // New Items
                    string _NewPlateNo = collection["newplate"] == null ? string.Empty : collection["newplate"].Trim();
                    string _NewMake = collection["newmake"] == null ? string.Empty : collection["newmake"].Trim();
                    string _NewModel = collection["newmodel"] == null ? string.Empty : collection["newmodel"].Trim();
                    string _NewYear = collection["newyear"] == null ? string.Empty : collection["newyear"].Trim();
                    string _NewFName = collection["firstname"] == null ? string.Empty : collection["firstname"].Trim();
                    string _NewLName = collection["lastname"] == null ? string.Empty : collection["lastname"].Trim();
                    string _NewMobile = collection["mobile"] == null ? string.Empty : collection["mobile"].Trim();
                    int _MakeID = collection["newmakeid"] == null ? -1 : int.Parse(collection["newmakeid"]);
                    int _ModelID = collection["newmodelid"] == null ? -1 : int.Parse(collection["newmodelid"]);
                    int _CustomerID = collection["customerid"] == null ? -1 : int.Parse(collection["customerid"]);

                    if (_VehicleID == -1)
                    {
                        var plateno = db.VehicleTRs.SingleOrDefault(b => b.PlateNo == _NewPlateNo);

                        if (plateno != null)
                        {
                            TempData["ErrorMessage"] = "Plate No Allready Exist";
                            return RedirectToAction("Create", "Job");
                        }
                        else
                        {
                            #region --> Create New Make and Model

                            if (_MakeID == -1 && _NewMake == "")
                            {
                                TempData["ErrorMessage"] = "Vehicle Make cannot be Empty";
                                return RedirectToAction("Create", "Job");
                            }
                            else
                            {
                                // Make Selected
                                if (_MakeID != -1)
                                {
                                    var make = db.VehicleMakes.SingleOrDefault(b => b.MakeID == _MakeID);

                                    if (make.Make != _NewMake)
                                    {
                                        if (_NewMake != "")
                                        {
                                            objvehiclemake.Make = _NewMake;
                                            objvehiclemake.FileID = -1;
                                            objvehiclemake.Flagged = true;
                                            objvehiclemake.LastModifyDate = DateTime.Now;
                                            objvehiclemake.LastModifyUser = _User;
                                            int _NewMakeID = _IVehicle.InsertMake(objvehiclemake);
                                            Session["NewMake"] = Convert.ToString(_NewMakeID);

                                            if (_NewModel != "")
                                            {
                                                objvehiclemodel.ModelName = _NewModel;
                                                objvehiclemodel.MakeID = _NewMakeID;
                                                objvehiclemodel.ModelTypeID = -1;
                                                objvehiclemodel.LastModifyDate = DateTime.Now;
                                                objvehiclemodel.LastModifyUser = _User;
                                                int _NewModelID = _IVehicle.InsertModel(objvehiclemodel);
                                                Session["NewModel"] = Convert.ToString(_NewModelID);
                                            }
                                        }
                                        else
                                        {
                                            if (_NewModel != "")
                                            {
                                                var model = db.VehicleModels.SingleOrDefault(b => b.ModelName == _NewModel && b.MakeID == _MakeID);

                                                if (model != null)
                                                {
                                                    Session["NewModel"] = Convert.ToString(model.ModelID);
                                                }
                                                else
                                                {
                                                    objvehiclemodel.ModelName = _NewModel;
                                                    objvehiclemodel.MakeID = _MakeID;
                                                    objvehiclemodel.ModelTypeID = -1;
                                                    objvehiclemodel.LastModifyDate = DateTime.Now;
                                                    objvehiclemodel.LastModifyUser = _User;
                                                    int _NewModelID = _IVehicle.InsertModel(objvehiclemodel);
                                                    Session["NewModel"] = Convert.ToString(_NewModelID);

                                                    if (make.Flagged == false)
                                                    {
                                                        make.Flagged = true;
                                                        make.LastModifyDate = DateTime.Now;
                                                        make.LastModifyUser = _User;
                                                        db.SaveChanges();
                                                    }

                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var model = db.VehicleModels.SingleOrDefault(b => b.ModelID == _ModelID);

                                        if (model != null)
                                        {
                                            if (model.ModelName != _NewModel)
                                            {
                                                objvehiclemodel.ModelName = _NewModel;
                                                objvehiclemodel.MakeID = _MakeID;
                                                objvehiclemodel.ModelTypeID = -1;
                                                objvehiclemodel.LastModifyDate = DateTime.Now;
                                                objvehiclemodel.LastModifyUser = _User;
                                                int _NewModelID = _IVehicle.InsertModel(objvehiclemodel);
                                                Session["NewModel"] = Convert.ToString(_NewModelID);

                                                if (make.Flagged == false)
                                                {
                                                    make.Flagged = true;
                                                    make.LastModifyDate = DateTime.Now;
                                                    make.LastModifyUser = _User;
                                                    db.SaveChanges();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            objvehiclemodel.ModelName = _NewModel;
                                            objvehiclemodel.MakeID = _MakeID;
                                            objvehiclemodel.ModelTypeID = -1;
                                            objvehiclemodel.LastModifyDate = DateTime.Now;
                                            objvehiclemodel.LastModifyUser = _User;
                                            int _NewModelID = _IVehicle.InsertModel(objvehiclemodel);
                                            Session["NewModel"] = Convert.ToString(_NewModelID);
                                        }
                                    }
                                }
                                else
                                {
                                    var make = db.VehicleMakes.SingleOrDefault(b => b.Make == _NewMake);

                                    if (make != null)
                                    {
                                        Session["NewMake"] = Convert.ToString(make.MakeID);

                                        var model = db.VehicleModels.SingleOrDefault(b => b.ModelName == _NewModel && b.MakeID == make.MakeID);

                                        if (model != null)
                                        {
                                            Session["NewModel"] = Convert.ToString(model.ModelID);
                                        }
                                        else
                                        {
                                            objvehiclemodel.ModelName = _NewModel;
                                            objvehiclemodel.MakeID = make.MakeID;
                                            objvehiclemodel.ModelTypeID = -1;
                                            objvehiclemodel.LastModifyDate = DateTime.Now;
                                            objvehiclemodel.LastModifyUser = _User;
                                            int _NewModelID = _IVehicle.InsertModel(objvehiclemodel);
                                            Session["NewModel"] = Convert.ToString(_NewModelID);

                                            if (make.Flagged == false)
                                            {
                                                make.Flagged = true;
                                                make.LastModifyDate = DateTime.Now;
                                                make.LastModifyUser = _User;
                                                db.SaveChanges();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        objvehiclemake.Make = _NewMake;
                                        objvehiclemake.FileID = -1;
                                        objvehiclemake.Flagged = true;
                                        objvehiclemodel.LastModifyDate = DateTime.Now;
                                        objvehiclemodel.LastModifyUser = _User;
                                        int _NewMakeID = _IVehicle.InsertMake(objvehiclemake);
                                        Session["NewMake"] = Convert.ToString(_NewMakeID);

                                        if (_NewModel != "")
                                        {
                                            objvehiclemodel.ModelName = _NewModel;
                                            objvehiclemodel.MakeID = _NewMakeID;
                                            objvehiclemodel.ModelTypeID = -1;
                                            objvehiclemodel.LastModifyDate = DateTime.Now;
                                            objvehiclemodel.LastModifyUser = _User;
                                            int _NewModelID = _IVehicle.InsertModel(objvehiclemodel);
                                            Session["NewModel"] = Convert.ToString(_NewModelID);
                                        }
                                    }
                                }
                            }

                            #endregion

                            #region --> Create New Customer

                            if (_CustomerID == -1)
                            {
                                if (_NewFName != "")
                                {
                                    objcustomer.SegmentID = _Segment;
                                    objcustomer.FirstName = _NewFName;
                                    objcustomer.LastName = _NewLName;
                                    objcustomer.MiddleName = "";
                                    objcustomer.OtherName = "";
                                    objcustomer.GenderID = -1;
                                    objcustomer.Company = "";
                                    objcustomer.BuisnessNo = "";
                                    objcustomer.GSTNo = "";
                                    objcustomer.UserName = "";
                                    objcustomer.Email = "";
                                    objcustomer.Mobile = _NewMobile;
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
                                    int _rtCustomerID = _IUser.InsertCustomer(objcustomer);
                                    Session["NewCustomer"] = Convert.ToString(_rtCustomerID);
                                }
                            }
                            else
                            {
                                var customer = db.Customers.SingleOrDefault(b => b.CustomerID == _CustomerID);

                                if (customer.Mobile != _NewMobile)
                                {
                                    var newcustomer = db.Customers.SingleOrDefault(b => b.Mobile == _NewMobile);

                                    if (newcustomer == null)
                                    {
                                        if (_NewFName != "")
                                        {
                                            objcustomer.SegmentID = _Segment;
                                            objcustomer.FirstName = _NewFName;
                                            objcustomer.LastName = _NewLName;
                                            objcustomer.MiddleName = "";
                                            objcustomer.OtherName = "";
                                            objcustomer.GenderID = -1;
                                            objcustomer.Company = "";
                                            objcustomer.BuisnessNo = "";
                                            objcustomer.GSTNo = "";
                                            objcustomer.UserName = "";
                                            objcustomer.Email = "";
                                            objcustomer.Mobile = _NewMobile;
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
                                            int _rtCustomerID = _IUser.InsertCustomer(objcustomer);
                                            Session["NewCustomer"] = Convert.ToString(_rtCustomerID);
                                        }
                                    }
                                    else
                                    {
                                        Session["exCustomer"] = Convert.ToString(newcustomer.CustomerID);
                                    }
                                }
                                else
                                {
                                    Session["exCustomer"] = Convert.ToString(customer.CustomerID);
                                }
                            }

                            #endregion

                            #region --> Create New Vehicle

                            if (_NewPlateNo == "")
                            {
                                TempData["ErrorMessage"] = "Plate No Cannot be Empty";
                                return RedirectToAction("Create", "Job");

                            }
                            else
                            {
                                objvehicles.SegmentID = _Segment;
                                objvehicles.PlateNo = _NewPlateNo;

                                if (_MakeID == -1)
                                {
                                    objvehicles.MakeID = Convert.ToInt32(HttpContext.Session["NewMake"]);
                                }
                                else
                                {
                                    objvehicles.MakeID = _MakeID;
                                }

                                if (_CustomerID == -1)
                                {
                                    objvehicles.CustomerID = Convert.ToInt32(HttpContext.Session["NewCustomer"]);
                                }
                                else
                                {
                                    objvehicles.CustomerID = Convert.ToInt32(HttpContext.Session["exCustomer"]);
                                }

                                if (_NewModel != "")
                                {
                                    objvehicles.ModelID = Convert.ToInt32(HttpContext.Session["NewModel"]);

                                    var updatemake = db.VehicleModels.SingleOrDefault(b => b.ModelID == objvehicles.ModelID);
                                    if (updatemake != null)
                                    {
                                        if (updatemake.Flagged == false)
                                        {
                                            updatemake.Flagged = true;
                                            updatemake.LastModifyDate = DateTime.Now;
                                            updatemake.LastModifyUser = _User;
                                            db.SaveChanges();
                                        }
                                    }
                                }
                                else
                                {
                                    objvehicles.ModelID = _ModelID;

                                    var updatemake = db.VehicleModels.SingleOrDefault(b => b.ModelID == _ModelID);
                                    if (updatemake != null)
                                    {
                                        if (updatemake.Flagged == false)
                                        {
                                            updatemake.Flagged = true;
                                            updatemake.LastModifyDate = DateTime.Now;
                                            updatemake.LastModifyUser = _User;
                                            db.SaveChanges();
                                        }
                                    }
                                }

                                objvehicles.ChassisNo = "";
                                objvehicles.EngineNo = "";
                                objvehicles.EngineCC = "";
                                objvehicles.Year = _NewYear;
                                objvehicles.ModelNo = "";
                                objvehicles.RegisterTypeID = -1;
                                objvehicles.TransTypeID = -1;
                                objvehicles.TyreSize = "";
                                objvehicles.Color = "";
                                objvehicles.FuelTypeID = -1;
                                objvehicles.Milage = _Milage;
                                objvehicles.Hubo = _Hubo;
                                objvehicles.RUC = _RUC;
                                objvehicles.MileageID = -1;
                                objvehicles.Milage = "0";
                                objvehicles.Hubo = "0";
                                objvehicles.RUC = "0";
                                objvehicles.Rego = "Rego";
                                objvehicles.RegoExpiryDate = _RegoExpiryDate;
                                objvehicles.WOF = "WOF";
                                objvehicles.WOFExpiryDate = _WOFExpiryDate;
                                objvehicles.Remark = _Remark;
                                objvehicles.Flagged = false;
                                objvehicles.LastModifyDate = DateTime.Now;
                                objvehicles.LastModifyUser = _User;
                                int _NewVehicleID = _IVehicle.InsertVehicle(objvehicles);
                                Session["NewVehicle"] = Convert.ToString(_NewVehicleID);

                                if (_Milage != "" || _Hubo != "" || _RUC != "")
                                {
                                    objmileage.SegmentID = _Segment;
                                    objmileage.Mileage = _Milage;
                                    objmileage.VehicleID = Convert.ToInt32(HttpContext.Session["NewVehicle"]);
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
                                    var updatemileage = db.VehicleTRs.SingleOrDefault(b => b.VehicleID == _NewVehicleID);
                                    if (updatemileage != null)
                                    {
                                        updatemileage.MileageID = _MileageID;
                                        updatemileage.Milage = _Milage;
                                        updatemileage.Hubo = _Hubo;
                                        updatemileage.RUC = _RUC;
                                        updatemileage.RegoExpiryDate = _RegoExpiryDate;
                                        updatemileage.WOFExpiryDate = _WOFExpiryDate;
                                        updatemileage.LastModifyDate = DateTime.Now;
                                        updatemileage.LastModifyUser = _User;
                                        db.SaveChanges();
                                    }
                                }
                            }

                            #endregion
                        }
                    }

                }
                else
                {
                    #region --> Add and Update Mileage

                    if (_Milage != "" || _Hubo != "" || _RUC != "")
                    {
                        var archivemileage = (from data in db.VehicleMileageTRs where data.VehicleID == _VehicleID && data.Updated == true select data).ToList();

                        if (archivemileage.Count > 0)
                        {
                            foreach (var mile in archivemileage)
                            {
                                mile.Updated = false;
                                mile.LastModifyDate = DateTime.Now;
                                mile.LastModifyUser = _User;
                                _IVehicle.UpdateMileage(mile);
                            }
                        }

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
                            updatemileage.Milage = _Milage;
                            updatemileage.Hubo = _Hubo;
                            updatemileage.RUC = _RUC;
                            updatemileage.RegoExpiryDate = _RegoExpiryDate;
                            updatemileage.WOFExpiryDate = _WOFExpiryDate;
                            updatemileage.LastModifyDate = DateTime.Now;
                            updatemileage.LastModifyUser = _User;
                            db.SaveChanges();
                        }
                    }

                    #endregion
                }

                #region -- > Generate Job No.

                var jobprefix = db.SettingGenerals.SingleOrDefault(b => b.PrefixType == 1);

                if (jobprefix == null)
                {
                    TempData["Prefix"] = "Job Prefix Not Setup ...!";
                }
                else
                {
                    string _JobPrefix = jobprefix.Prefix;
                    string _StartNo = jobprefix.StartNo;

                    List<JobTR> startlist = db.JobTRs.ToList();

                    if (startlist.Count == 0)
                    {
                        _JobTRID = _JobPrefix + _StartNo;
                    }
                    else
                    {
                        // ----------- Split and Next Job ID ----------------
                        var lastNumber = startlist.Last().JobTRID;

                        Regex re = new Regex(@"([a-zA-Z]+)(\d+)");
                        Match match = re.Match(lastNumber);

                        string alphaPart = match.Groups[1].Value;
                        string numberPart = match.Groups[2].Value;
                        string _Format = numberPart.Length.ToString();

                        string zero = "{0:D" + _Format + "}".ToString();
                        int split = int.Parse(numberPart) + 1;
                        string value = String.Format(zero, split);
                        _JobTRID = alphaPart + value;
                        // --------------------------------------------------
                    }
                }

                #endregion

                #region --> Create Job

                objjob.SegmentID = _Segment;

                if (_VehicleID == -1)
                {
                    objjob.VehicleID = Convert.ToInt32(HttpContext.Session["NewVehicle"]);

                    var newvehicle = db.VehicleTRs.SingleOrDefault(b => b.VehicleID == objjob.VehicleID);
                    objjob.PlateNo = newvehicle.PlateNo;
                }
                else
                {
                    objjob.VehicleID = _VehicleID;
                    objjob.PlateNo = vehicle.PlateNo;
                }

                objjob.JobTRID = _JobTRID;
                objjob.JobTypeID = _JobTypeID;
                objjob.JobTaskTypeID = _JobPackageID;
                objjob.PackageID = _JobPackageID;
                objjob.JobStartDate = _JobStartDate;
                objjob.JobFinishDate = _JobFinishDate;
                objjob.StatusID = 1;
                objjob.Flagged = false;
                objjob.LastModifyDate = DateTime.Now;
                objjob.LastModifyUser = _User;
                objjob.Remark = _Remark;
                objjob.Description = _Description;
                objjob.ApprovalID = -1;
                objjob.Approved = false;
                objjob.ApprovalSchemeID = jobtype.ApprovalSchemeID;
                objjob.ApprovedDate = DateTime.Parse("1900-01-01");
                objjob.Rejected = false;
                objjob.RejectedDate = DateTime.Parse("1900-01-01");
                objjob.Reason = "";
                objjob.InvoiceID = -1;
                int returnJobID = _IJob.InsertJobTR(objjob);

                #endregion

                #region --> Add Selected Tasks

                if (!string.IsNullOrEmpty(collection["ID"]))
                {
                    string[] ids = collection["ID"].Split(new char[] { ',' });

                    if (ids != null)
                    {
                        foreach (string id in ids)
                        {
                            objjobtask.SegmentID = _Segment;
                            objjobtask.JobID = returnJobID;
                            objjobtask.JobTRID = _JobTRID;

                            if (_VehicleID == -1)
                            {
                                int _NewVehicleID = Convert.ToInt32(HttpContext.Session["NewVehicle"]);

                                var newvehicle = db.VehicleTRs.SingleOrDefault(b => b.VehicleID == _NewVehicleID);
                                objjobtask.PlateNo = newvehicle.PlateNo;
                                objjobtask.VehicleID = _NewVehicleID;
                            }
                            else
                            {
                                objjobtask.PlateNo = vehicle.PlateNo;
                                objjobtask.VehicleID = _VehicleID;
                            }

                            objjobtask.JobTaskTypeID = _JobPackageID;
                            objjobtask.JobTaskID = int.Parse(id);
                            objjobtask.JobTypeID = _JobTypeID;
                            objjobtask.StatusID = 1;
                            objjobtask.Remarks = "";
                            objjobtask.StartDate = _JobStartDate;
                            objjobtask.FinishDate = _JobFinishDate;
                            objjobtask.CompletedBy = -1;
                            objjobtask.Flagged = false;
                            objjobtask.LastModifyDate = DateTime.Now;
                            objjobtask.LastModifyUser = _User;
                            _IJob.InsertTaskTR(objjobtask);
                        }
                    }
                }

                #endregion

                #region --> Add New Tasks

                // ----------------------------  Add New Tasks --------------------------------
                if (!string.IsNullOrEmpty(collection["taskname"]))
                {
                    string[] names = collection["taskname"].Split(char.Parse(","));

                    if (names != null)
                    {
                        foreach (string task in names)
                        {
                            if (task != "")
                            {
                                var newtask = db.JobTasksTBs.SingleOrDefault(b => b.TaskName == task);

                                if (newtask != null)
                                {
                                    // ---------------------------- Create Job Task TR --------------------------------
                                    objjobtask.SegmentID = _Segment;
                                    objjobtask.JobID = returnJobID;
                                    objjobtask.JobTRID = _JobTRID;

                                    if (_VehicleID == -1)
                                    {
                                        int _NewVehicleID = Convert.ToInt32(HttpContext.Session["NewVehicle"]);

                                        var newvehicle = db.VehicleTRs.SingleOrDefault(b => b.VehicleID == _NewVehicleID);
                                        objjobtask.PlateNo = newvehicle.PlateNo;
                                        objjobtask.VehicleID = _NewVehicleID;
                                    }
                                    else
                                    {
                                        objjobtask.PlateNo = vehicle.PlateNo;
                                        objjobtask.VehicleID = _VehicleID;
                                    }

                                    objjobtask.JobTaskTypeID = _JobPackageID;
                                    objjobtask.JobTaskID = newtask.TaskID;
                                    objjobtask.JobTypeID = _JobTypeID;
                                    objjobtask.StatusID = 1;
                                    objjobtask.Remarks = "";
                                    objjobtask.StartDate = _JobStartDate;
                                    objjobtask.FinishDate = _JobFinishDate;
                                    objjobtask.CompletedBy = -1;
                                    objjobtask.Flagged = false;
                                    objjobtask.LastModifyDate = DateTime.Now;
                                    objjobtask.LastModifyUser = _User;
                                    _IJob.InsertTaskTR(objjobtask);
                                }
                                else
                                {
                                    // ---------------------------- Create Job Task Master --------------------------------
                                    objjobtaskmaster.SegmentID = _Segment;
                                    objjobtaskmaster.TaskName = task;
                                    objjobtaskmaster.Flagged = false;
                                    objjobtaskmaster.LastModifyDate = DateTime.Now;
                                    objjobtaskmaster.LastModifyUser = _User;
                                    int returnTaskID = _IJob.InsertTasks(objjobtaskmaster);

                                    // ---------------------------- Create Job Task TR --------------------------------
                                    objjobtask.SegmentID = _Segment;
                                    objjobtask.JobID = returnJobID;
                                    objjobtask.JobTRID = _JobTRID;

                                    if (_VehicleID == -1)
                                    {
                                        int _NewVehicleID = Convert.ToInt32(HttpContext.Session["NewVehicle"]);

                                        var newvehicle = db.VehicleTRs.SingleOrDefault(b => b.VehicleID == _NewVehicleID);
                                        objjobtask.PlateNo = newvehicle.PlateNo;
                                        objjobtask.VehicleID = _NewVehicleID;
                                    }
                                    else
                                    {
                                        objjobtask.PlateNo = vehicle.PlateNo;
                                        objjobtask.VehicleID = _VehicleID;
                                    }

                                    objjobtask.JobTaskTypeID = _JobPackageID;
                                    objjobtask.JobTaskID = returnTaskID;
                                    objjobtask.JobTypeID = _JobTypeID;
                                    objjobtask.StatusID = 1;
                                    objjobtask.Remarks = "";
                                    objjobtask.StartDate = _JobStartDate;
                                    objjobtask.FinishDate = _JobFinishDate;
                                    objjobtask.CompletedBy = -1;
                                    objjobtask.Flagged = false;
                                    objjobtask.LastModifyDate = DateTime.Now;
                                    objjobtask.LastModifyUser = _User;
                                    _IJob.InsertTaskTR(objjobtask);
                                }
                            }
                        }
                    }
                }

                #endregion

                #region --> Assign Employee

                if (!string.IsNullOrEmpty(collection["IDEmployee"]))
                {
                    string[] ids = collection["IDEmployee"].Split(new char[] { ',' });

                    if (ids != null)
                    {
                        foreach (string id in ids)
                        {
                            objjobemployee.SegmentID = _Segment;
                            objjobemployee.JobID = returnJobID;
                            objjobemployee.EmployeeNo = int.Parse(id);
                            objjobemployee.Flagged = false;
                            objjobemployee.LastModifyDate = DateTime.Now;
                            objjobemployee.LastModifyUser = _User;
                            _IJob.InsertJobEmployee(objjobemployee);
                        }
                    }
                }

                #endregion

                #region --> Create Invoice

                //if (_IsComplete == true)
                //{
                //    var gst = db.SalesTaxes.SingleOrDefault(b => b.Code == "GST" && b.IsActive == true);
                //    var employee = db.JobAssignEmpTRs.SingleOrDefault(b => b.JobID == returnJobID);
                //    string _RepairNote = collection["repairnote"] == null ? string.Empty : collection["repairnote"].Trim();
                //    string _PartNote = collection["partnote"] == null ? string.Empty : collection["partnote"].Trim();
                //    string _MaterialNote = collection["materialnote"] == null ? string.Empty : collection["materialnote"].Trim();
                //    string _TaskNote = collection["tasknote"] == null ? string.Empty : collection["tasknote"].Trim();
                //    string _Status = collection["status"] == null ? string.Empty : collection["status"].Trim();
                //    Double _Repair = Convert.ToDouble(collection["repair"]) == 0 ? 0 : Convert.ToDouble(collection["repair"]);
                //    Double _PartAmount = Convert.ToDouble(collection["partamount"]) == 0 ? 0 : Convert.ToDouble(collection["partamount"]);
                //    Double _Material = Convert.ToDouble(collection["material"]) == 0 ? 0 : Convert.ToDouble(collection["material"]);
                //    Double _AddFreight = Convert.ToDouble(collection["addfreight"]) == 0 ? 0 : Convert.ToDouble(collection["addfreight"]);
                //    Double _Freight = Convert.ToDouble(collection["freight"]) == 0 ? 0 : Convert.ToDouble(collection["freight"]);
                //    Double _Excess = Convert.ToDouble(collection["excess"]) == 0 ? 0 : Convert.ToDouble(collection["excess"]);
                //    Double _Travel = Convert.ToDouble(collection["travel"]) == 0 ? 0 : Convert.ToDouble(collection["travel"]);
                //    DateTime _DueDate = collection["duedate"] == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(collection["duedate"].ToString());

                //    Double _Misc = _AddFreight + _Freight;
                //    Double _GrandTotal = _PartAmount + _Misc + _Repair + _Material + _Travel;
                //    Double _GST = _GrandTotal * gst.Percentage / 100;
                //    Double _TotalAmount = _GrandTotal + _GST;
                //    Double _TotalDue = _TotalAmount - _Excess;

                //    string _InvoiceNo = "";
                //    var invoiceprefix = db.SettingGenerals.SingleOrDefault(b => b.PrefixType == 2);

                //    string _InvoicePrefix = invoiceprefix.Prefix;
                //    string _StartNo = invoiceprefix.StartNo;

                //    List<SalesInvoiceTR> startlist = db.SalesInvoiceTRs.ToList();

                //    if (startlist.Count == 0)
                //    {
                //        _InvoiceNo = _InvoicePrefix + _StartNo;
                //    }
                //    else
                //    {
                //        var lastNumber = startlist.Last().InvoiceNo;

                //        Regex re = new Regex(@"([a-zA-Z]+)(\d+)");
                //        Match re2 = re.Match(lastNumber);

                //        string alphaPart = re2.Groups[1].Value;
                //        string numberPart = re2.Groups[2].Value;
                //        string _Format = numberPart.Length.ToString();

                //        string zero = "{0:D" + _Format + "}".ToString();
                //        int split = int.Parse(numberPart) + 1;
                //        string value = String.Format(zero, split);
                //        _InvoiceNo = alphaPart + value;
                //    }

                //    objinvoice.SegmentID = _Segment;
                //    objinvoice.JobID = returnJobID;
                //    objinvoice.InvoiceNo = _InvoiceNo;
                //    objinvoice.POID = -1;
                //    objinvoice.SubTotal = Convert.ToDouble(_GrandTotal.ToString("0.00"));
                //    objinvoice.GST = Convert.ToDouble(_GST.ToString("0.00"));
                //    objinvoice.Total = Convert.ToDouble(_TotalAmount.ToString("0.00"));
                //    objinvoice.TotalNet = Convert.ToDouble(_TotalDue.ToString("0.00"));
                //    objinvoice.InvoiceStatus = _Status;
                //    objinvoice.StatusID = int.Parse(_Status);
                //    objinvoice.CurrencyID = invoiceprefix.CurrencyID;
                //    objinvoice.PrefixType = invoiceprefix.PrefixType;
                //    objinvoice.Flagged = false;
                //    objinvoice.DueDate = DateTime.Now;
                //    objinvoice.ApprovalID = -1;
                //    objinvoice.ApprovalSchemeID = -1;
                //    objinvoice.PreparedBy = _User;
                //    objinvoice.ApprovedBy = -1;
                //    objinvoice.Comment = "";
                //    objinvoice.CustomFloat1 = 0;
                //    objinvoice.CustomFloat2 = 0;
                //    objinvoice.CustomFloat3 = 0;
                //    objinvoice.LastModifyDate = DateTime.Now;
                //    objinvoice.LastModifyUser = _User;
                //    int ReturnInvoiceID = _ISales.InsertInvoice(objinvoice);

                //    //Create Invoice List
                //    objinvoicelist.SegmentID = _Segment;
                //    objinvoicelist.InvoiceID = ReturnInvoiceID;
                //    objinvoicelist.InvoiceNo = _InvoiceNo;
                //    objinvoicelist.CustomVar1 = _RepairNote;
                //    objinvoicelist.CustomVar2 = _PartNote;
                //    objinvoicelist.CustomVar3 = _MaterialNote;
                //    objinvoicelist.CustomVar4 = "";
                //    objinvoicelist.CustomVar5 = "";
                //    objinvoicelist.CustomFloat1 = Convert.ToDouble(_Repair);
                //    objinvoicelist.CustomFloat2 = Convert.ToDouble(_PartAmount);
                //    objinvoicelist.CustomFloat3 = Convert.ToDouble(_Material);
                //    objinvoicelist.CustomFloat4 = Convert.ToDouble(_AddFreight);
                //    objinvoicelist.CustomFloat5 = Convert.ToDouble(_Freight);
                //    objinvoicelist.CustomFloat6 = Convert.ToDouble(_Excess);
                //    objinvoicelist.CustomFloat7 = Convert.ToDouble(_Travel);
                //    objinvoicelist.CustomFloat8 = 0;
                //    objinvoicelist.CustomFloat9 = 0;
                //    objinvoicelist.CustomFloat10 = 0;
                //    objinvoicelist.LastModifyDate = DateTime.Now;
                //    objinvoicelist.LastModifyUser = _User;
                //    _ISales.InsertInvoiceList(objinvoicelist);

                //    var jobupdate = db.JobTRs.SingleOrDefault(b => b.JobID == returnJobID);
                //    if (jobupdate != null)
                //    {
                //        jobupdate.InvoiceID = ReturnInvoiceID;
                //        jobupdate.JobFinishDate = DateTime.Now;
                //        jobupdate.LastModifyDate = DateTime.Now;
                //        jobupdate.LastModifyUser = _User;
                //        jobupdate.StatusID = 2;
                //        db.SaveChanges();
                //    }

                //    var updatetask = (from data in db.JobTaskTRs where data.JobID == item.JobID select data).ToList();
                //    if (updatetask.Count > 0)
                //    {
                //        foreach (var task in updatetask)
                //        {
                //            task.LastModifyDate = DateTime.Now;
                //            task.LastModifyUser = _User;
                //            task.StatusID = 2;
                //            task.CompletedBy = employee.EmployeeNo;
                //            db.SaveChanges();
                //        }
                //    }

                //}

                #endregion

                #region --> Assign Part Types

                //if (!String.IsNullOrEmpty(collection["IDPart"]))
                //{
                //    string[] ids = collection["IDPart"].Split(new char[] { ',' });

                //    if (ids != null)
                //    {
                //        foreach (string id in ids)
                //        {
                //            var parts = new JobTaskPartDetailTR
                //            {
                //                SegmentID = _Segment,
                //                JobTaskTRID = -1,
                //                TaskPartID = int.Parse(id),
                //                JobID = returnJobID,
                //                SubTypeID = -1,
                //                StockID = -1,
                //                Qty = 0,
                //                Remark = "",
                //                Flagged = false,
                //                LastModifyDate = DateTime.Now,
                //                LastModifyUser = _User
                //            };
                //            db.JobTaskPartDetailTRs.Add(parts);
                //            db.SaveChanges();
                //        }
                //    }
                //}

                #endregion

                #region --> Approval Process

                //objapprovalheader.SegmentID = _Segment;
                //objapprovalheader.TransactionID = returnJobID;
                //objapprovalheader.ApprovalSchemeID = jobtype.ApprovalSchemeID;
                //objapprovalheader.Approved = false;
                //objapprovalheader.ApprovalDate = DateTime.Parse("1900-01-01");
                //objapprovalheader.Rejected = false;
                //objapprovalheader.RejectedDate = DateTime.Parse("1900-01-01");
                //objapprovalheader.TextNaration = "Job_Transaction";
                //objapprovalheader.Flagged = false;
                //objapprovalheader.LastModifyDate = DateTime.Now;
                //objapprovalheader.LastModifyUser = _User;
                //int returnAppovalID = _IApproval.InsertHeader(objapprovalheader);

                //var approvalgroup = (from data in db.ApprovalGroups where data.ApprovalSchemeID == jobtype.ApprovalSchemeID && data.SegmentID == _Segment select data).ToList();
                //if (approvalgroup.Count > 0)
                //{
                //    foreach (var user in approvalgroup)
                //    {

                //        objapprovaldetail.SegmentID = _Segment;
                //        objapprovaldetail.ApprovalID = returnAppovalID;
                //        objapprovaldetail.TransactionID = returnJobID;
                //        objapprovaldetail.GroupID = user.ApprovalGroupID;
                //        objapprovaldetail.ApprovalEmployeeNo = user.UserID;
                //        objapprovaldetail.ApprovalDesignationID = user.DesignationID;
                //        objapprovaldetail.ApprovalUserID = -1;
                //        objapprovaldetail.ApprovalParentDesignationID = -1;
                //        objapprovaldetail.ApprovalParentEmployeeNo = -1;
                //        objapprovaldetail.ApprovalSchemeID = jobtype.ApprovalSchemeID;
                //        objapprovaldetail.Approved = false;
                //        objapprovaldetail.ApprovalTime = DateTime.Parse("1900-01-01");
                //        objapprovaldetail.Rejected = false;
                //        objapprovaldetail.RejectedTime = DateTime.Parse("1900-01-01");
                //        objapprovaldetail.Reason = "";
                //        objapprovaldetail.FinalApproval = user.FinalApprover;
                //        objapprovaldetail.ImmedateApproval = false;
                //        objapprovaldetail.ImmediateReject = false;
                //        objapprovaldetail.Flagged = false;
                //        objapprovaldetail.LastModifyDate = DateTime.Now;
                //        objapprovaldetail.LastModifyUser = _User;
                //        _IApproval.InsertDetail(objapprovaldetail);
                //    }
                //}

                //var updatejob = db.JobTRs.SingleOrDefault(b => b.JobID == returnJobID);
                //if (updatejob != null)
                //{
                //    updatejob.ApprovalID = returnAppovalID;
                //    db.SaveChanges();
                //}

                #endregion

                #region --> Vehicle Images

                if (Request.Files.Count > 0)
                {
                    foreach (HttpPostedFileBase file in FileUpload)
                    {
                        if (file != null)
                        {
                            if (_VehicleID == -1)
                            {
                                int _NewVehicleID = Convert.ToInt32(HttpContext.Session["NewVehicle"]);

                                var newvehicle = db.VehicleTRs.SingleOrDefault(b => b.VehicleID == _NewVehicleID);

                                // Save file in Folder
                                string filename = newvehicle.PlateNo + "_" + file.FileName;
                                string physicalPath = Server.MapPath("~/Files/Vehicles/" + filename);
                                file.SaveAs(physicalPath);

                                var image = new FileVehicle
                                {
                                    SegmentID = Convert.ToInt32(HttpContext.Session["Segment"]),
                                    TransactionID = returnJobID,
                                    FileTypeID = 1,
                                    FileName = filename,
                                    FileBitStreem = new byte[file.ContentLength],
                                    CreateDate = DateTime.Now,
                                    FileTypeDescription = "",
                                    FilePath = Server.MapPath("~/Files/Vehicles/" + filename),
                                    LastModifyDate = DateTime.Now,
                                    LastModifyUser = Convert.ToInt32(HttpContext.Session["AdminUser"])
                                };
                                db.FileVehicles.Add(image);
                                db.SaveChanges();
                            }
                            else
                            {
                                // Save file in Folder
                                string filename = vehicle.PlateNo + "_" + file.FileName;
                                string physicalPath = Server.MapPath("~/Files/Vehicles/" + filename);
                                file.SaveAs(physicalPath);

                                var image = new FileVehicle
                                {
                                    SegmentID = Convert.ToInt32(HttpContext.Session["Segment"]),
                                    TransactionID = returnJobID,
                                    FileTypeID = 1,
                                    FileName = filename,
                                    FileBitStreem = new byte[file.ContentLength],
                                    CreateDate = DateTime.Now,
                                    FileTypeDescription = "",
                                    FilePath = Server.MapPath("~/Files/Vehicles/" + filename),
                                    LastModifyDate = DateTime.Now,
                                    LastModifyUser = Convert.ToInt32(HttpContext.Session["AdminUser"])
                                };
                                db.FileVehicles.Add(image);
                                db.SaveChanges();
                            }
                        }
                    }
                }

                #endregion

                TempData["Success"] = "Success";
                return RedirectToAction("List", "Job");
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            ViewBag.Current = "Jobs";
            ViewBag.CurrentSub = "Job";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var result = db.JobTRs.SingleOrDefault(b => b.JobID == id);

                if (result == null)
                {
                    return View(List());
                }
                else
                {
                    if (Convert.ToDateTime(result.JobStartDate).ToString("yyyy-MM-dd") == "1900-01-01")
                    {
                        TempData["startdate"] = "";
                    }
                    else
                    {
                        TempData["startdate"] = Convert.ToDateTime(result.JobStartDate).ToString("dd-MM-yyyy");
                    }

                    if (Convert.ToDateTime(result.JobFinishDate).ToString("yyyy-MM-dd") == "1900-01-01")
                    {
                        TempData["finishdate"] = "";
                    }
                    else
                    {
                        TempData["finishdate"] = Convert.ToDateTime(result.JobFinishDate).ToString("dd-MM-yyyy");
                    }


                    var vehicle = (from data in db.VehicleTRs orderby data.PlateNo ascending select data).ToList();
                    ViewBag.VehicleList = new SelectList(vehicle, "VehicleID", "PlateNo");

                    var jobtype = (from data in db.JobPackages where data.SegmentID == _Segment && data.PackageID != 1 orderby data.PackageName ascending select data).ToList();
                    ViewBag.TypeList = new SelectList(jobtype, "PackageID", "PackageName");

                    var task = (from data in db.JobTasksTBs where data.SegmentID == _Segment orderby data.TaskName ascending select data).ToList();
                    ViewBag.TaskList = new SelectList(task, "TaskID", "TaskName");

                    var make = (from data in db.VehicleMakes orderby data.Make ascending select data).ToList();
                    ViewBag.MakeList = new SelectList(make, "MakeID", "Make");

                    var customer = (from data in db.Customers where data.SegmentID == _Segment orderby data.FullName ascending select data).ToList();
                    ViewBag.CustomerList = new SelectList(customer, "CustomerID", "FullName");

                    var model = (from data in db.VehicleModels orderby data.ModelName ascending select data).ToList();
                    ViewBag.ModelList = new SelectList(model, "ModelID", "ModelName");

                    var package = (from data in db.JobPackages orderby data.PackageName ascending select data).ToList();
                    ViewBag.PackageList = new SelectList(package, "PackageID", "PackageName");

                    var mileage = db.VehicleMileageTRs.SingleOrDefault(b => b.VehicleID == result.VehicleID && b.Updated == true);
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

                    //var invoicelist = db.SalesInvoiceLists.SingleOrDefault(b => b.InvoiceID == result.InvoiceID);
                    //if (invoicelist != null)
                    //{
                    //    TempData["repairnote"] = invoicelist.CustomVar1;
                    //    TempData["partnote"] = invoicelist.CustomVar2;
                    //    TempData["materialnote"] = invoicelist.CustomVar3;

                    //    TempData["repair"] = invoicelist.CustomFloat1;
                    //    TempData["partamount"] = invoicelist.CustomFloat2;
                    //    TempData["material"] = invoicelist.CustomFloat3;
                    //    TempData["addfreight"] = invoicelist.CustomFloat4;
                    //    TempData["freight"] = invoicelist.CustomFloat5;
                    //    TempData["travel"] = invoicelist.CustomFloat6;
                    //    TempData["excess"] = invoicelist.CustomFloat7;
                    //}

                    //var invoice = db.SalesInvoiceTRs.SingleOrDefault(b => b.InvoiceID == result.InvoiceID);
                    //if (invoice != null)
                    //{
                    //    if (Convert.ToDateTime(invoice.DueDate).ToString("yyyy-MM-dd") == "1900-01-01")
                    //    {
                    //        TempData["duedate"] = "";
                    //    }
                    //    else
                    //    {
                    //        TempData["duedate"] = Convert.ToDateTime(invoice.DueDate).ToString("dd-MM-yyyy");
                    //    }
                    //}

                    return View(db.JobTRs.Where(x => x.JobID == id).FirstOrDefault<JobTR>());
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(JobTR item, FormCollection collection, HttpPostedFileBase[] FileUpload)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var jobtype = db.JobTypes.SingleOrDefault(b => b.JobTypeName == "Custom");
                var jobtask = db.JobPackages.SingleOrDefault(b => b.PackageName == "Custom");
                var vehicle = db.VehicleTRs.SingleOrDefault(b => b.VehicleID == item.VehicleID);
                var job = db.JobTRs.SingleOrDefault(b => b.JobID == item.JobID);

                int _StatusID = item.StatusID == null ? -1 : int.Parse(item.StatusID.ToString());
                int _JobTypeID = jobtype.JobTypeID.ToString() == null ? -1 : int.Parse(jobtype.JobTypeID.ToString());
                int _JobPackageID = item.PackageID == null ? -1 : int.Parse(item.PackageID.ToString());
                int _VehicleID = item.VehicleID == null ? -1 : int.Parse(item.VehicleID.ToString());
                //int _ApprovalSettingID = jobtype.ApprovalSettingID == null ? -1 : int.Parse(jobtype.ApprovalSettingID.ToString());
                string _Milage = collection["mileage"] == null ? string.Empty : collection["mileage"].Trim();
                string _Hubo = collection["hubo"] == null ? string.Empty : collection["hubo"].Trim();
                string _RUC = collection["ruc"] == null ? string.Empty : collection["ruc"].Trim();
                string _Remark = item.Remark == null ? string.Empty : item.Remark.Trim();
                string _Description = item.Description == null ? string.Empty : item.Description.Trim();
                DateTime _JobStartDate = collection["startdate"] == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(collection["startdate"].ToString());
                DateTime _JobFinishDate = collection["finishdate"] == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(collection["finishdate"].ToString());
                DateTime _RegoExpiryDate = collection["regodate"] == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(collection["regodate"].ToString());
                DateTime _WOFExpiryDate = collection["wofdate"] == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(collection["wofdate"].ToString());

                //string jobcomplete = collection["jobcomplete"];
                //bool _IsComplete = Convert.ToBoolean(jobcomplete);


                #region --> Add and Update Mileage

                if (_Milage != "" || _Hubo != "" || _RUC != "")
                {
                    var archivemileage = (from data in db.VehicleMileageTRs where data.VehicleID == _VehicleID && data.Updated == true select data).ToList();

                    if (archivemileage.Count > 0)
                    {
                        foreach (var mile in archivemileage)
                        {
                            mile.Updated = false;
                            mile.LastModifyDate = DateTime.Now;
                            mile.LastModifyUser = _User;
                            _IVehicle.UpdateMileage(mile);
                        }
                    }

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
                        updatemileage.Milage = _Milage;
                        updatemileage.Hubo = _Hubo;
                        updatemileage.RUC = _RUC;
                        updatemileage.RegoExpiryDate = _RegoExpiryDate;
                        updatemileage.WOFExpiryDate = _WOFExpiryDate;
                        updatemileage.LastModifyDate = DateTime.Now;
                        updatemileage.LastModifyUser = _User;
                        db.SaveChanges();
                    }
                }

                #endregion

                #region --> Update Job

                var updatejob = db.JobTRs.SingleOrDefault(b => b.JobID == item.JobID);

                if (updatejob != null)
                {
                    objjob.SegmentID = _Segment;
                    objjob.VehicleID = _VehicleID;
                    objjob.PlateNo = vehicle.PlateNo;
                    //objjob.JobTRID = job.JobTRID;
                    //objjob.JobTypeID = _JobTypeID;
                    //objjob.JobTaskTypeID = _JobPackageID;
                    //objjob.PackageID = _JobPackageID;
                    objjob.JobStartDate = _JobStartDate;
                    objjob.JobFinishDate = _JobFinishDate;
                    objjob.StatusID = 1;
                    objjob.Flagged = false;
                    objjob.LastModifyDate = DateTime.Now;
                    objjob.LastModifyUser = _User;
                    objjob.Remark = _Remark;
                    objjob.Description = _Description;
                    //objjob.ApprovalID = -1;
                    //objjob.Approved = false;
                    //objjob.ApprovalSchemeID = jobtype.ApprovalSchemeID;
                    //objjob.ApprovedDate = DateTime.Parse("1900-01-01");
                    //objjob.Rejected = false;
                    //objjob.RejectedDate = DateTime.Parse("1900-01-01");
                    //objjob.Reason = "";
                    //objjob.InvoiceID = -1;

                    _IJob.UpdateJobTR(updatejob);
                }

                #endregion


                // Remove Job Tasks
                var asstask = (from data in db.JobTaskTRs where data.JobID == item.JobID select data).ToList();
                if (asstask.Count > 0)
                {
                    foreach (var task in asstask)
                    {
                        db.JobTaskTRs.Remove(task);
                        db.SaveChanges();
                    }
                }

                #region --> Add Selected Tasks

                if (!string.IsNullOrEmpty(collection["ID"]))
                {
                    string[] ids = collection["ID"].Split(new char[] { ',' });

                    if (ids != null)
                    {
                        foreach (string id in ids)
                        {
                            objjobtask.SegmentID = _Segment;
                            objjobtask.JobID = item.JobID;
                            objjobtask.JobTRID = job.JobTRID;
                            objjobtask.VehicleID = item.VehicleID;
                            objjobtask.PlateNo = job.PlateNo;
                            objjobtask.JobTaskTypeID = _JobPackageID;
                            objjobtask.JobTaskID = int.Parse(id);
                            objjobtask.JobTypeID = _JobTypeID;
                            objjobtask.StatusID = 1;
                            objjobtask.Remarks = "";
                            objjobtask.StartDate = _JobStartDate;
                            objjobtask.FinishDate = _JobFinishDate;
                            objjobtask.CompletedBy = -1;
                            objjobtask.Flagged = false;
                            objjobtask.LastModifyDate = DateTime.Now;
                            objjobtask.LastModifyUser = _User;
                            _IJob.InsertTaskTR(objjobtask);
                        }
                    }
                }

                #endregion

                #region --> Add New Tasks

                // ----------------------------  Add New Tasks --------------------------------
                if (!string.IsNullOrEmpty(collection["taskname"]))
                {
                    string[] names = collection["taskname"].Split(char.Parse(","));

                    if (names != null)
                    {
                        foreach (string task in names)
                        {
                            if (task != "")
                            {
                                var newtask = db.JobTasksTBs.SingleOrDefault(b => b.TaskName == task);

                                if (newtask != null)
                                {
                                    // ---------------------------- Create Job Task TR --------------------------------
                                    objjobtask.SegmentID = _Segment;
                                    objjobtask.JobID = item.JobID;
                                    objjobtask.JobTRID = job.JobTRID;

                                    if (_VehicleID == -1)
                                    {
                                        int _NewVehicleID = Convert.ToInt32(HttpContext.Session["NewVehicle"]);

                                        var newvehicle = db.VehicleTRs.SingleOrDefault(b => b.VehicleID == _NewVehicleID);
                                        objjobtask.PlateNo = newvehicle.PlateNo;
                                        objjobtask.VehicleID = _NewVehicleID;
                                    }
                                    else
                                    {
                                        objjobtask.PlateNo = vehicle.PlateNo;
                                        objjobtask.VehicleID = _VehicleID;
                                    }

                                    objjobtask.JobTaskTypeID = _JobPackageID;
                                    objjobtask.JobTaskID = newtask.TaskID;
                                    objjobtask.JobTypeID = _JobTypeID;
                                    objjobtask.StatusID = 1;
                                    objjobtask.Remarks = "";
                                    objjobtask.StartDate = _JobStartDate;
                                    objjobtask.FinishDate = _JobFinishDate;
                                    objjobtask.CompletedBy = -1;
                                    objjobtask.Flagged = false;
                                    objjobtask.LastModifyDate = DateTime.Now;
                                    objjobtask.LastModifyUser = _User;
                                    _IJob.InsertTaskTR(objjobtask);
                                }
                                else
                                {
                                    // ---------------------------- Create Job Task Master --------------------------------
                                    objjobtaskmaster.SegmentID = _Segment;
                                    objjobtaskmaster.TaskName = task;
                                    objjobtaskmaster.Flagged = false;
                                    objjobtaskmaster.LastModifyDate = DateTime.Now;
                                    objjobtaskmaster.LastModifyUser = _User;
                                    int returnTaskID = _IJob.InsertTasks(objjobtaskmaster);

                                    // ---------------------------- Create Job Task TR --------------------------------
                                    objjobtask.SegmentID = _Segment;
                                    objjobtask.JobID = item.JobID;
                                    objjobtask.JobTRID = job.JobTRID;

                                    if (_VehicleID == -1)
                                    {
                                        int _NewVehicleID = Convert.ToInt32(HttpContext.Session["NewVehicle"]);

                                        var newvehicle = db.VehicleTRs.SingleOrDefault(b => b.VehicleID == _NewVehicleID);
                                        objjobtask.PlateNo = newvehicle.PlateNo;
                                        objjobtask.VehicleID = _NewVehicleID;
                                    }
                                    else
                                    {
                                        objjobtask.PlateNo = vehicle.PlateNo;
                                        objjobtask.VehicleID = _VehicleID;
                                    }

                                    objjobtask.JobTaskTypeID = _JobPackageID;
                                    objjobtask.JobTaskID = returnTaskID;
                                    objjobtask.JobTypeID = _JobTypeID;
                                    objjobtask.StatusID = 1;
                                    objjobtask.Remarks = "";
                                    objjobtask.StartDate = _JobStartDate;
                                    objjobtask.FinishDate = _JobFinishDate;
                                    objjobtask.CompletedBy = -1;
                                    objjobtask.Flagged = false;
                                    objjobtask.LastModifyDate = DateTime.Now;
                                    objjobtask.LastModifyUser = _User;
                                    _IJob.InsertTaskTR(objjobtask);
                                }
                            }
                        }
                    }
                }

                #endregion

                #region --> Assign Employee

                var removeemp = (from data in db.JobAssignEmpTRs where data.JobID == item.JobID select data).ToList();
                if (removeemp.Count > 0)
                {
                    foreach (var emp in removeemp)
                    {
                        db.JobAssignEmpTRs.Remove(emp);
                        db.SaveChanges();
                    }
                }

                if (!string.IsNullOrEmpty(collection["IDEmployee"]))
                {
                    string[] ids = collection["IDEmployee"].Split(new char[] { ',' });

                    if (ids != null)
                    {
                        foreach (string id in ids)
                        {
                            objjobemployee.SegmentID = _Segment;
                            objjobemployee.JobID = item.JobID;
                            objjobemployee.EmployeeNo = int.Parse(id);
                            objjobemployee.Flagged = false;
                            objjobemployee.LastModifyDate = DateTime.Now;
                            objjobemployee.LastModifyUser = _User;
                            _IJob.InsertJobEmployee(objjobemployee);
                        }
                    }
                }

                #endregion

                #region --> Create Invoice

                //if (_IsComplete == true)
                //{
                //    var gst = db.SalesTaxes.SingleOrDefault(b => b.Code == "GST" && b.IsActive == true);

                //    string _RepairNote = collection["repairnote"] == null ? string.Empty : collection["repairnote"].Trim();
                //    string _PartNote = collection["partnote"] == null ? string.Empty : collection["partnote"].Trim();
                //    string _MaterialNote = collection["materialnote"] == null ? string.Empty : collection["materialnote"].Trim();
                //    string _TaskNote = collection["tasknote"] == null ? string.Empty : collection["tasknote"].Trim();
                //    string _Status = collection["status"] == null ? string.Empty : collection["status"].Trim();
                //    Double _Repair = Convert.ToDouble(collection["repair"]) == 0 ? 0 : Convert.ToDouble(collection["repair"]);
                //    Double _PartAmount = Convert.ToDouble(collection["partamount"]) == 0 ? 0 : Convert.ToDouble(collection["partamount"]);
                //    Double _Material = Convert.ToDouble(collection["material"]) == 0 ? 0 : Convert.ToDouble(collection["material"]);
                //    Double _AddFreight = Convert.ToDouble(collection["addfreight"]) == 0 ? 0 : Convert.ToDouble(collection["addfreight"]);
                //    Double _Freight = Convert.ToDouble(collection["freight"]) == 0 ? 0 : Convert.ToDouble(collection["freight"]);
                //    Double _Excess = Convert.ToDouble(collection["excess"]) == 0 ? 0 : Convert.ToDouble(collection["excess"]);
                //    Double _Travel = Convert.ToDouble(collection["travel"]) == 0 ? 0 : Convert.ToDouble(collection["travel"]);
                //    DateTime _DueDate = collection["duedate"] == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(collection["duedate"].ToString());

                //    Double _Misc = _AddFreight + _Freight;
                //    Double _GrandTotal = _PartAmount + _Misc + _Repair + _Material + _Travel;
                //    Double _GST = _GrandTotal * gst.Percentage / 100;
                //    Double _TotalAmount = _GrandTotal + _GST;
                //    Double _TotalDue = _TotalAmount - _Excess;

                //    if (item.InvoiceID == -1)
                //    {
                //        string _InvoiceNo = "";
                //        var invoiceprefix = db.SettingGenerals.SingleOrDefault(b => b.PrefixType == 2);

                //        string _InvoicePrefix = invoiceprefix.Prefix;
                //        string _StartNo = invoiceprefix.StartNo;

                //        List<SalesInvoiceTR> startlist = db.SalesInvoiceTRs.ToList();

                //        if (startlist.Count == 0)
                //        {
                //            _InvoiceNo = _InvoicePrefix + _StartNo;
                //        }
                //        else
                //        {
                //            var lastNumber = startlist.Last().InvoiceNo;

                //            Regex re = new Regex(@"([a-zA-Z]+)(\d+)");
                //            Match re2 = re.Match(lastNumber);

                //            string alphaPart = re2.Groups[1].Value;
                //            string numberPart = re2.Groups[2].Value;
                //            string _Format = numberPart.Length.ToString();

                //            string zero = "{0:D" + _Format + "}".ToString();
                //            int split = int.Parse(numberPart) + 1;
                //            string value = String.Format(zero, split);
                //            _InvoiceNo = alphaPart + value;
                //        }

                //        objinvoice.SegmentID = _Segment;
                //        objinvoice.JobID = job.JobID;
                //        objinvoice.InvoiceNo = _InvoiceNo;
                //        objinvoice.POID = -1;
                //        objinvoice.SubTotal = Convert.ToDouble(_GrandTotal.ToString("0.00"));
                //        objinvoice.GST = Convert.ToDouble(_GST.ToString("0.00"));
                //        objinvoice.Total = Convert.ToDouble(_TotalAmount.ToString("0.00"));
                //        objinvoice.TotalNet = Convert.ToDouble(_TotalDue.ToString("0.00"));
                //        objinvoice.InvoiceStatus = _Status;
                //        objinvoice.StatusID = int.Parse(_Status);
                //        objinvoice.CurrencyID = invoiceprefix.CurrencyID;
                //        objinvoice.PrefixType = invoiceprefix.PrefixType;
                //        objinvoice.Flagged = false;
                //        objinvoice.DueDate = DateTime.Now;
                //        objinvoice.ApprovalID = -1;
                //        objinvoice.ApprovalSchemeID = -1;
                //        objinvoice.PreparedBy = _User;
                //        objinvoice.ApprovedBy = -1;
                //        objinvoice.Comment = "";
                //        objinvoice.CustomFloat1 = 0;
                //        objinvoice.CustomFloat2 = 0;
                //        objinvoice.CustomFloat3 = 0;
                //        objinvoice.LastModifyDate = DateTime.Now;
                //        objinvoice.LastModifyUser = _User;
                //        int ReturnInvoiceID = _ISales.InsertInvoice(objinvoice);

                //        //Create Invoice List
                //        objinvoicelist.SegmentID = _Segment;
                //        objinvoicelist.InvoiceID = ReturnInvoiceID;
                //        objinvoicelist.InvoiceNo = _InvoiceNo;
                //        objinvoicelist.CustomVar1 = _RepairNote;
                //        objinvoicelist.CustomVar2 = _PartNote;
                //        objinvoicelist.CustomVar3 = _MaterialNote;
                //        objinvoicelist.CustomVar4 = "";
                //        objinvoicelist.CustomVar5 = "";
                //        objinvoicelist.CustomFloat1 = Convert.ToDouble(_Repair);
                //        objinvoicelist.CustomFloat2 = Convert.ToDouble(_PartAmount);
                //        objinvoicelist.CustomFloat3 = Convert.ToDouble(_Material);
                //        objinvoicelist.CustomFloat4 = Convert.ToDouble(_AddFreight);
                //        objinvoicelist.CustomFloat5 = Convert.ToDouble(_Freight);
                //        objinvoicelist.CustomFloat6 = Convert.ToDouble(_Excess);
                //        objinvoicelist.CustomFloat7 = Convert.ToDouble(_Travel);
                //        objinvoicelist.CustomFloat8 = 0;
                //        objinvoicelist.CustomFloat9 = 0;
                //        objinvoicelist.CustomFloat10 = 0;
                //        objinvoicelist.LastModifyDate = DateTime.Now;
                //        objinvoicelist.LastModifyUser = _User;
                //        _ISales.InsertInvoiceList(objinvoicelist);

                //        var jobupdate = db.JobTRs.SingleOrDefault(b => b.JobID == job.JobID);
                //        if (jobupdate != null)
                //        {
                //            jobupdate.InvoiceID = ReturnInvoiceID;
                //            jobupdate.JobFinishDate = DateTime.Now;
                //            jobupdate.LastModifyDate = DateTime.Now;
                //            jobupdate.LastModifyUser = _User;
                //            jobupdate.StatusID = 2;
                //            db.SaveChanges();
                //        }

                //        var updatetask = (from data in db.JobTaskTRs where data.JobID == item.JobID select data).ToList();
                //        if (updatetask.Count > 0)
                //        {
                //            foreach (var task in updatetask)
                //            {
                //                task.LastModifyDate = DateTime.Now;
                //                task.LastModifyUser = _User;
                //                task.StatusID = 2;
                //                db.SaveChanges();
                //            }
                //        }
                //    }
                //    else
                //    {
                //        var updateinvoice = db.SalesInvoiceTRs.SingleOrDefault(b => b.InvoiceID == item.InvoiceID);
                //        if (updateinvoice != null)
                //        {
                //            updateinvoice.SubTotal = Convert.ToDouble(_GrandTotal.ToString("0.00"));
                //            updateinvoice.GST = Convert.ToDouble(_GST.ToString("0.00"));
                //            updateinvoice.Total = Convert.ToDouble(_TotalAmount.ToString("0.00"));
                //            updateinvoice.TotalNet = Convert.ToDouble(_TotalDue.ToString("0.00"));
                //            updateinvoice.InvoiceStatus = _Status;
                //            updateinvoice.StatusID = int.Parse(_Status);
                //            updateinvoice.DueDate = DateTime.Now;
                //            updateinvoice.LastModifyDate = DateTime.Now;
                //            updateinvoice.LastModifyUser = _User;
                //            _ISales.UpdateInvoice(updateinvoice);

                //        }

                //        var updateinvoicelist = db.SalesInvoiceLists.SingleOrDefault(b => b.InvoiceID == item.InvoiceID);
                //        if (updateinvoicelist != null)
                //        {
                //            updateinvoicelist.CustomVar1 = _RepairNote;
                //            updateinvoicelist.CustomVar2 = _PartNote;
                //            updateinvoicelist.CustomVar3 = _MaterialNote;
                //            updateinvoicelist.CustomVar4 = "";
                //            updateinvoicelist.CustomVar5 = "";
                //            updateinvoicelist.CustomFloat1 = Convert.ToDouble(_Repair);
                //            updateinvoicelist.CustomFloat2 = Convert.ToDouble(_PartAmount);
                //            updateinvoicelist.CustomFloat3 = Convert.ToDouble(_Material);
                //            updateinvoicelist.CustomFloat4 = Convert.ToDouble(_AddFreight);
                //            updateinvoicelist.CustomFloat5 = Convert.ToDouble(_Freight);
                //            updateinvoicelist.CustomFloat6 = Convert.ToDouble(_Excess);
                //            updateinvoicelist.CustomFloat7 = Convert.ToDouble(_Travel);
                //            updateinvoicelist.CustomFloat8 = 0;
                //            updateinvoicelist.CustomFloat9 = 0;
                //            updateinvoicelist.CustomFloat10 = 0;
                //            updateinvoicelist.LastModifyDate = DateTime.Now;
                //            updateinvoicelist.LastModifyUser = _User;
                //            _ISales.UpdateInvoiceList(updateinvoicelist);
                //        }
                //    }
                //}

                #endregion

                #region --> Assign Parts

                //var removepart = (from data in db.JobTaskPartDetailTRs where data.JobID == item.JobID select data).ToList();
                //if (removepart.Count > 0)
                //{
                //    foreach (var part in removepart)
                //    {
                //        db.JobTaskPartDetailTRs.Remove(part);
                //        db.SaveChanges();
                //    }
                //}

                //if (!String.IsNullOrEmpty(collection["IDPart"]))
                //{
                //    string[] ids = collection["IDPart"].Split(new char[] { ',' });

                //    if (ids != null)
                //    {
                //        foreach (string id in ids)
                //        {
                //            var parts = new JobTaskPartDetailTR
                //            {
                //                SegmentID = _Segment,
                //                JobTaskTRID = -1,
                //                TaskPartID = int.Parse(id),
                //                JobID = item.JobID,
                //                SubTypeID = -1,
                //                StockID = -1,
                //                Qty = 0,
                //                Remark = "",
                //                Flagged = false,
                //                LastModifyDate = DateTime.Now,
                //                LastModifyUser = _User
                //            };
                //            db.JobTaskPartDetailTRs.Add(parts);
                //            db.SaveChanges();
                //        }
                //    }
                //}

                #endregion

                #region --> File Upload

                if (Request.Files.Count > 0)
                {
                    foreach (HttpPostedFileBase file in FileUpload)
                    {
                        if (file != null)
                        {
                            // Save file in Folder
                            string filename = job.PlateNo + "_" + file.FileName;
                            string physicalPath = Server.MapPath("~/Files/Vehicles/" + filename);
                            file.SaveAs(physicalPath);

                            var image = new FileVehicle
                            {
                                SegmentID = Convert.ToInt32(HttpContext.Session["Segment"]),
                                TransactionID = job.JobID,
                                FileTypeID = 1,
                                FileName = filename,
                                FileBitStreem = new byte[file.ContentLength],
                                CreateDate = DateTime.Now,
                                FileTypeDescription = "",
                                FilePath = Server.MapPath("~/Files/Vehicles/" + filename),
                                LastModifyDate = DateTime.Now,
                                LastModifyUser = Convert.ToInt32(HttpContext.Session["AdminUser"])
                            };
                            db.FileVehicles.Add(image);
                            db.SaveChanges();
                        }
                    }
                }

                #endregion

            }

            TempData["Updated"] = "Updated";
            return RedirectToAction("List", "Job");
        }

        [HttpGet]
        public ActionResult StartJob(int id)
        {
            ViewBag.Current = "Jobs";
            ViewBag.CurrentSub = "Job";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);
            Session["JobID"] = Convert.ToString(id);

            using (var db = new DatabaseContext())
            {
                return View(db.JobTRs.Where(x => x.JobID == id).FirstOrDefault<JobTR>());
            }
        }

        [HttpGet]
        public ActionResult UpdateJobTask()
        {
            ViewBag.Current = "Jobs";
            ViewBag.CurrentSub = "Job";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);
            int _JobID = Convert.ToInt32(HttpContext.Session["JobID"]);

            using (var db = new DatabaseContext())
            {
                var task = (from data in db.vw_JobTasksTRs where data.JobID == _JobID && data.SegmentID == _Segment orderby data.TaskName select data).ToList();
                ViewBag.Tasklist = new SelectList(task, "JobTaskTRID", "TaskName");

                var empassign = (from data in db.vw_JobAssignEmpTRs where data.JobID == _JobID && data.SegmentID == _Segment orderby data.FullName select data).ToList();
                ViewBag.employeelist = new SelectList(empassign, "EmployeeNo", "FullName");

                var status = (from data in db.JobStatuses where data.StatusType == 1 && data.SegmentID == _Segment orderby data.StatusName select data).ToList();
                ViewBag.statuslist = new SelectList(status, "StatusID", "StatusName");

                var result = db.JobTRs.SingleOrDefault(b => b.JobID == _JobID);

                if (result != null)
                {
                    //TempData["tasknote"] = result.Description;

                    if (Convert.ToDateTime(result.JobStartDate).ToString("yyyy-MM-dd") == "1900-01-01")
                    {
                        TempData["startdate"] = "";
                    }
                    else
                    {
                        TempData["startdate"] = Convert.ToDateTime(result.JobStartDate).ToString("dd-MM-yyyy");
                    }

                    if (Convert.ToDateTime(result.JobFinishDate).ToString("yyyy-MM-dd") == "1900-01-01")
                    {
                        TempData["finishdate"] = "";
                    }
                    else
                    {
                        TempData["finishdate"] = Convert.ToDateTime(result.JobFinishDate).ToString("dd-MM-yyyy");
                    }
                }

                return View(db.JobTaskTRs.Where(x => x.JobID == _JobID).FirstOrDefault<JobTaskTR>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddJobTask(JobTaskTR item, FormCollection collection)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);
            int _JobID = Convert.ToInt32(HttpContext.Session["JobID"]);

            using (var db = new DatabaseContext())
            {
                string active = collection["checkall"];
                bool _IsActive = Convert.ToBoolean(active);

                int _Status = item.StatusID == 0 ? -1 : int.Parse(item.StatusID.ToString());
                int _Employee = item.CompletedBy == 0 ? -1 : int.Parse(item.CompletedBy.ToString());
                DateTime _JobStartDate = collection["startdate"] == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(collection["startdate"].ToString());
                DateTime _JobFinishDate = collection["finishdate"] == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(collection["finishdate"].ToString());
                string _Remark = item.Remarks == null ? string.Empty : item.Remarks.Trim();
                string _TaskNote = collection["tasknote"] == null ? string.Empty : collection["tasknote"].Trim();

                if (_IsActive == true)
                {
                    var alltasks = (from data in db.JobTaskTRs where data.JobID == item.JobID select data).ToList();
                    if (alltasks.Count > 0)
                    {
                        foreach (var x in alltasks)
                        {
                            x.StatusID = _Status;
                            x.CompletedBy = _Employee;
                            x.StartDate = _JobStartDate;
                            x.FinishDate = _JobFinishDate;
                            x.Remarks = _Remark;
                            x.LastModifyDate = DateTime.Now;
                            x.LastModifyUser = _User;
                            _IJob.UpdateTaskTR(x);
                        }
                    }

                    var job = db.JobTRs.SingleOrDefault(b => b.JobID == _JobID && b.SegmentID == _Segment);

                    if (job != null)
                    {
                        job.Description = _TaskNote;
                        job.JobFinishDate = _JobFinishDate;
                        _IJob.UpdateJobTR(job);
                    }
                }
                else
                {
                    var task = db.JobTaskTRs.SingleOrDefault(b => b.JobID == _JobID && b.JobTaskTRID == item.JobTaskTRID && b.SegmentID == _Segment);
                    if (task != null)
                    {
                        task.StatusID = _Status;
                        task.CompletedBy = _Employee;
                        task.StartDate = _JobStartDate;
                        task.FinishDate = _JobFinishDate;
                        task.Remarks = _Remark;
                        task.LastModifyDate = DateTime.Now;
                        task.LastModifyUser = _User;
                        _IJob.UpdateTaskTR(task);

                        TempData["Success"] = "Success";
                    }
                }

                TempData["Success"] = "Success";
                return RedirectToAction("StartJob", "Job", new { id = _JobID });
            }
        }

        [HttpGet]
        public ActionResult OrderList(int id)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _JobID = Convert.ToInt32(HttpContext.Session["JobID"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.vw_JobTasksTRs where data.JobID == _JobID && data.SegmentID == _Segment orderby data.TaskName descending select data).ToList();
                var complete = (from data in db.vw_JobTasksTRs where data.JobID == _JobID && data.Code == "COMPLETE" && data.SegmentID == _Segment select data).ToList();

                int _list = list.Count;
                int _Completed = complete.Count;

                if (_Completed == _list)
                {
                    TempData["Completed"] = "4";
                }
                else
                {
                    TempData["Completed"] = "1";
                }

                return View(list);
            }
        }

        [HttpGet]
        public ActionResult CompleteJob(int id)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _JobID = Convert.ToInt32(HttpContext.Session["JobID"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);
            Session["JobID"] = Convert.ToString(id);

            ViewBag.Current = "Jobs";
            ViewBag.CurrentSub = "Job";

            using (var db = new DatabaseContext())
            {

                var status = (from data in db.SalesStatusTBs where data.StatusType == 1 orderby data.StatusName select data).ToList();
                ViewBag.statuslist = new SelectList(status, "StatusID", "StatusName");

                TempData["repairnote"] = "(Listed below description)";
                TempData["partnote"] = "Parts";
                TempData["materialnote"] = "Materials";


                return View(db.JobTRs.Where(x => x.JobID == id).FirstOrDefault<JobTR>());
            }
        }

        [HttpPost]
        public ActionResult CompleteJob(JobTR item, FormCollection collection)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);
            int _JobID = Convert.ToInt32(HttpContext.Session["JobID"]);

            string active = collection["jobcomplete"];
            bool _Checked = Convert.ToBoolean(active);


            using (var db = new DatabaseContext())
            {

                // ------------------- Update Job Transaction ----------------------
                var jobtr = db.JobTRs.SingleOrDefault(b => b.JobID == _JobID);

                if (jobtr != null)
                {
                    jobtr.StatusID = 2;
                    jobtr.JobFinishDate = DateTime.Now;
                    jobtr.LastModifyDate = DateTime.Now;
                    jobtr.LastModifyUser = _User;
                    db.SaveChanges();
                }

                // ------------------- Create Invoice ----------------------
                if (_Checked == true)
                {
                    var gst = db.SalesTaxes.SingleOrDefault(b => b.Code == "GST" && b.IsActive == true);
                    string _RepairNote = collection["repairnote"] == null ? string.Empty : collection["repairnote"].Trim();
                    string _PartNote = collection["partnote"] == null ? string.Empty : collection["partnote"].Trim();
                    string _MaterialNote = collection["materialnote"] == null ? string.Empty : collection["materialnote"].Trim();
                    string _TaskNote = collection["tasknote"] == null ? string.Empty : collection["tasknote"].Trim();
                    int _Status = item.StatusID == 0 ? -1 : int.Parse(item.StatusID.ToString());
                    string _Repair = collection["repair"] == "" ? "0" : collection["repair"].Trim();
                    string _PartAmount = collection["partamount"] == "" ? "0" : collection["partamount"].Trim();
                    string _Material = collection["material"] == "" ? "0" : collection["material"].Trim();
                    string _AddFreight = collection["addfreight"] == "" ? "0" : collection["addfreight"].Trim();
                    string _Freight = collection["freight"] == "" ? "0" : collection["freight"].Trim();
                    string _Excess = collection["excess"] == "" ? "0" : collection["excess"].Trim();
                    string _Travel = collection["travel"] == "" ? "0" : collection["travel"].Trim();
                    DateTime _DueDate = collection["duedate"] == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(collection["duedate"].ToString());

                    Double _Misc = Convert.ToDouble(_AddFreight) + Convert.ToDouble(_Freight);
                    Double _GrandTotal = Convert.ToDouble(_PartAmount) + _Misc + Convert.ToDouble(_Repair) + Convert.ToDouble(_Material) + Convert.ToDouble(_Travel);
                    Double _GST = _GrandTotal * gst.Percentage / 100;
                    Double _TotalAmount = _GrandTotal + _GST;
                    Double _TotalDue = _TotalAmount - Convert.ToDouble(_Excess);

                    // ------------------- Generate Invoice Number ----------------------
                    string _InvoiceNo = "";
                    var invoiceprefix = db.SettingGenerals.SingleOrDefault(b => b.PrefixType == 2);

                    string _InvoicePrefix = invoiceprefix.Prefix;
                    string _StartNo = invoiceprefix.StartNo;

                    List<SalesInvoiceTR> startlist = db.SalesInvoiceTRs.ToList();

                    if (startlist.Count == 0)
                    {
                        _InvoiceNo = _InvoicePrefix + _StartNo;
                    }
                    else
                    {
                        var lastNumber = startlist.Last().InvoiceNo;

                        Regex re = new Regex(@"([a-zA-Z]+)(\d+)");
                        Match re2 = re.Match(lastNumber);

                        string alphaPart = re2.Groups[1].Value;
                        string numberPart = re2.Groups[2].Value;
                        string _Format = numberPart.Length.ToString();

                        string zero = "{0:D" + _Format + "}".ToString();
                        int split = int.Parse(numberPart) + 1;
                        string value = String.Format(zero, split);
                        _InvoiceNo = alphaPart + value;
                    }
                    // ----------------------------------------------------------------------

                    objinvoice.SegmentID = _Segment;
                    objinvoice.JobID = _JobID;
                    objinvoice.InvoiceNo = _InvoiceNo;
                    objinvoice.POID = -1;
                    objinvoice.SubTotal = Convert.ToDouble(_GrandTotal.ToString("0.00"));
                    objinvoice.GST = Convert.ToDouble(_GST.ToString("0.00"));
                    objinvoice.Total = Convert.ToDouble(_TotalAmount.ToString("0.00"));
                    objinvoice.TotalNet = Convert.ToDouble(_TotalDue.ToString("0.00"));
                    objinvoice.InvoiceStatus = "";
                    objinvoice.StatusID = _Status;
                    objinvoice.CurrencyID = invoiceprefix.CurrencyID;
                    objinvoice.PrefixType = invoiceprefix.PrefixType;
                    objinvoice.Flagged = false;
                    objinvoice.DueDate = DateTime.Now;
                    objinvoice.ApprovalID = -1;
                    objinvoice.ApprovalSchemeID = -1;
                    objinvoice.PreparedBy = _User;
                    objinvoice.ApprovedBy = -1;
                    objinvoice.Comment = "";
                    objinvoice.CustomFloat1 = 0;
                    objinvoice.CustomFloat2 = 0;
                    objinvoice.CustomFloat3 = 0;
                    objinvoice.LastModifyDate = DateTime.Now;
                    objinvoice.LastModifyUser = _User;
                    int ReturnInvoiceID = _ISales.InsertInvoice(objinvoice);

                    objinvoicelist.SegmentID = _Segment;
                    objinvoicelist.InvoiceID = ReturnInvoiceID;
                    objinvoicelist.InvoiceNo = _InvoiceNo;
                    objinvoicelist.CustomVar1 = _RepairNote;
                    objinvoicelist.CustomVar2 = _PartNote;
                    objinvoicelist.CustomVar3 = _MaterialNote;
                    objinvoicelist.CustomVar4 = "";
                    objinvoicelist.CustomVar5 = "";
                    objinvoicelist.CustomFloat1 = Convert.ToDouble(_Repair);
                    objinvoicelist.CustomFloat2 = Convert.ToDouble(_PartAmount);
                    objinvoicelist.CustomFloat3 = Convert.ToDouble(_Material);
                    objinvoicelist.CustomFloat4 = Convert.ToDouble(_AddFreight);
                    objinvoicelist.CustomFloat5 = Convert.ToDouble(_Freight);
                    objinvoicelist.CustomFloat6 = Convert.ToDouble(_Excess);
                    objinvoicelist.CustomFloat7 = Convert.ToDouble(_Travel);
                    objinvoicelist.CustomFloat8 = 0;
                    objinvoicelist.CustomFloat9 = 0;
                    objinvoicelist.CustomFloat10 = 0;
                    objinvoicelist.LastModifyDate = DateTime.Now;
                    objinvoicelist.LastModifyUser = _User;
                    _ISales.InsertInvoiceList(objinvoicelist);

                    var job = db.JobTRs.SingleOrDefault(b => b.JobID == _JobID);
                    if (job != null)
                    {
                        job.InvoiceID = ReturnInvoiceID;
                        job.JobFinishDate = DateTime.Now;
                        job.LastModifyDate = DateTime.Now;
                        job.LastModifyUser = _User;
                        db.SaveChanges();
                    }
                }
                TempData["Success"] = "Success";
                return RedirectToAction("List", "Job");
            }
        }

        [HttpGet]
        public ActionResult EmployeeList(int id)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _JobID = Convert.ToInt32(HttpContext.Session["JobID"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.vw_JobAssignEmpTRs where data.JobID == _JobID && data.SegmentID == _Segment orderby data.FullName descending select data).ToList();
                return View(list);
            }
        }

        [HttpPost]
        public JsonResult DeleteTask(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(id)))
                {
                    return Json("Error", JsonRequestBehavior.AllowGet);
                }

                var data = _IJob.DeleteTaskTR(Convert.ToInt32(id));

                if (data > 0)
                {
                    TempData["Deleted"] = "Deleted";
                    int _JobID = Convert.ToInt32(HttpContext.Session["JobID"]);
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

                var data = _IJob.DeleteJobTR(Convert.ToInt32(id), _User);

                if (data > 0)
                {
                    TempData["Canceled"] = "Canceled";
                    int _JobID = Convert.ToInt32(HttpContext.Session["JobID"]);
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
            ViewBag.Current = "Jobs";
            ViewBag.CurrentSub = "Job";

            using (var db = new DatabaseContext())
            {
                var result = db.vw_JobTRs.SingleOrDefault(b => b.JobID == id);

                if (result == null)
                {
                    return View(List());
                }
                else
                {
                    //------------------------------ Customer Address ----------------------------------
                    var address = db.AddressBook.SingleOrDefault(b => b.EntryID == result.CustomerID && b.RoleID == 4);

                    if (address == null)
                    {
                        TempData["Address1"] = string.Empty;
                        TempData["Address2"] = string.Empty;
                        TempData["Address3"] = string.Empty;
                        TempData["PostalNo"] = string.Empty;
                    }
                    else
                    {
                        TempData["Address1"] = address.Address1;
                        TempData["Address2"] = address.Address2;
                        TempData["Address3"] = address.Address3;
                        TempData["PostalNo"] = address.PostalNo;
                    }
                    //------------------------------------------------------------------------------------

                    //------------------------------ Vehicle Mileage ----------------------------------
                    var mileage = db.VehicleMileageTRs.SingleOrDefault(b => b.VehicleID == result.VehicleID && b.Updated == true);

                    if (mileage == null)
                    {
                        TempData["Mileage"] = string.Empty;
                    }
                    else
                    {
                        TempData["Mileage"] = mileage.Mileage;
                    }
                    //------------------------------------------------------------------------------------

                    return View(db.vw_JobTRs.Where(x => x.JobID == id).FirstOrDefault<vw_JobTR>());
                }
            }
        }

        [HttpGet]
        public ActionResult DetailJobList(int id)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.vw_JobTasksTRs where data.JobID == id && data.SegmentID == _Segment select data).ToList();
                return View(list);
            }
        }

        [HttpGet]
        public ActionResult ViewParts(int id)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.vw_JobTaskPartDetailTRs where data.JobID == id && data.SegmentID == _Segment orderby data.JobID descending select data).ToList();
                return View(list.ToList());
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
        public ActionResult InvoiceDetail(int id)
        {
            ViewBag.Current = "Jobs";
            ViewBag.CurrentSub = "Job";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var result = db.vw_SalesInvoiceTRs.SingleOrDefault(b => b.InvoiceID == id);

                if (result == null)
                {
                    return View(List());
                }
                else
                {
                    TempData["tasknote"] = string.Join(", ", from datas in db.vw_JobTasksTRs where datas.JobID == id select datas.TaskName);

                    var invoice = db.vw_SalesInvoiceTRs.SingleOrDefault(b => b.InvoiceID == result.InvoiceID);

                    if (invoice != null)
                    {
                        var organization = db.OrganizationSchemeInfos.SingleOrDefault(b => b.SegmentID == _Segment);

                        if (organization != null)
                        {
                            TempData["address1"] = organization.Address1;
                            TempData["address2"] = organization.Address2;
                            TempData["address3"] = organization.Address3;
                            TempData["postal"] = organization.PostalNo;
                            TempData["phone"] = organization.Phone;
                            TempData["email"] = organization.Email1;
                            TempData["gst"] = organization.GSTNo;
                            TempData["acc"] = organization.BankAccount1;

                        }
                        else
                        {
                            TempData["address1"] = "";
                            TempData["address2"] = "";
                            TempData["address3"] = "";
                            TempData["postal"] = "";
                            TempData["phone"] = "";
                            TempData["email"] = "";
                            TempData["gst"] = "";
                            TempData["acc"] = "";
                        }

                        var invoicelist = db.SalesInvoiceLists.SingleOrDefault(b => b.SegmentID == _Segment && b.InvoiceID == id);

                        if (invoicelist != null)
                        {
                            TempData["repairnote"] = invoicelist.CustomVar1.ToString();
                            TempData["partnote"] = invoicelist.CustomVar2.ToString();
                            TempData["materialnote"] = invoicelist.CustomVar3.ToString();
                            TempData["miscellnote"] = invoicelist.CustomVar4.ToString();
                            TempData["repair"] = invoicelist.CustomFloat1.ToString("#,##0.00");
                            TempData["partamount"] = invoicelist.CustomFloat2.ToString("#,##0.00");
                            TempData["material"] = invoicelist.CustomFloat3.ToString("#,##0.00");
                            TempData["miscellamount"] = invoicelist.CustomFloat4.ToString("#,##0.00");
                            TempData["freight"] = invoicelist.CustomFloat5.ToString("#,##0.00");
                            TempData["miscubtotal"] = Convert.ToDouble(invoicelist.CustomFloat4 + invoicelist.CustomFloat5).ToString("#,##0.00");
                        }
                    }
                    else
                    {
                        return View(List());
                    }

                    return View(db.vw_SalesInvoiceTRs.Where(x => x.InvoiceID == id).FirstOrDefault<vw_SalesInvoiceTR>());
                }
            }
        }

        [HttpGet]
        public ActionResult UpdateInvoice(int id)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            Session["Invoice"] = Convert.ToString(id);

            using (var db = new DatabaseContext())
            {
                var job = db.JobTRs.SingleOrDefault(b => b.JobID == id && b.SegmentID == _Segment);

                if (job == null)
                {
                    return View(List());
                }
                else
                {
                    TempData["tasknote"] = job.Description;

                    var invoice = db.vw_SalesInvoiceTRs.SingleOrDefault(b => b.JobID == id && b.SegmentID == _Segment);

                    if (invoice == null)
                    {
                        TempData["duedate"] = "";
                    }
                    else
                    {
                        TempData["duedate"] = invoice.DueDate;
                    }

                    var invoicelist = db.SalesInvoiceLists.SingleOrDefault(b => b.InvoiceID == job.InvoiceID && b.SegmentID == _Segment);

                    if (invoicelist == null)
                    {
                        TempData["invoiceno"] = invoice.InvoiceNo;
                        TempData["repairnote"] = "";
                        TempData["partnote"] = "";
                        TempData["materialnote"] = "";
                        TempData["tasknote"] = "";
                        TempData["repair"] = 0;
                        TempData["partamount"] = 0;
                        TempData["material"] = 0;
                        TempData["addfreight"] = 0;
                        TempData["freight"] = 0;
                        TempData["excess"] = 0;
                        TempData["travel"] = 0;
                    }
                    else
                    {
                        TempData["invoiceno"] = invoicelist.InvoiceNo;
                        TempData["repairnote"] = invoicelist.CustomVar1;
                        TempData["partnote"] = invoicelist.CustomVar2;
                        TempData["materialnote"] = invoicelist.CustomVar3;
                        TempData["tasknote"] = invoicelist.CustomVar5;
                        TempData["repair"] = invoicelist.CustomFloat1.ToString("#,##0.00");
                        TempData["partamount"] = invoicelist.CustomFloat2.ToString("#,##0.00");
                        TempData["material"] = invoicelist.CustomFloat3.ToString("#,##0.00");
                        TempData["addfreight"] = invoicelist.CustomFloat4.ToString("#,##0.00");
                        TempData["freight"] = invoicelist.CustomFloat5.ToString("#,##0.00");
                        TempData["excess"] = invoicelist.CustomFloat6.ToString("#,##0.00");
                        TempData["travel"] = invoicelist.CustomFloat7.ToString("#,##0.00");
                    }

                    return View();
                }
            }
        }

        [HttpPost]
        public ActionResult UpdateInvoice(SalesInvoiceListTR item, FormCollection collection)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);
            int _InvoiceID = Convert.ToInt32(HttpContext.Session["Invoice"]);

            using (var db = new DatabaseContext())
            {
                var scheme = db.ApprovalSettings.SingleOrDefault(b => b.Code == "INVJOB");
                int _ApprovalSchemeID = scheme.ApprovalSchemeID == null ? -1 : int.Parse(scheme.ApprovalSchemeID.ToString());
                var gst = db.SalesTaxes.SingleOrDefault(b => b.Code == "GST" && b.IsActive == true);

                var invoice = db.SalesInvoiceTRs.SingleOrDefault(b => b.InvoiceID == _InvoiceID);
                string _RepairNote = collection["repairnote"] == null ? string.Empty : collection["repairnote"].Trim();
                string _PartNote = collection["partnote"] == null ? string.Empty : collection["partnote"].Trim();
                string _MaterialNote = collection["materialnote"] == null ? string.Empty : collection["materialnote"].Trim();
                string _TaskNote = collection["tasknote"] == null ? string.Empty : collection["tasknote"].Trim();
                string _Status = collection["status"] == null ? string.Empty : collection["status"].Trim();
                Double _Repair = Convert.ToDouble(collection["repair"]) == 0 ? 0 : Convert.ToDouble(collection["repair"]);
                Double _PartAmount = Convert.ToDouble(collection["partamount"]) == 0 ? 0 : Convert.ToDouble(collection["partamount"]);
                Double _Material = Convert.ToDouble(collection["material"]) == 0 ? 0 : Convert.ToDouble(collection["material"]);
                Double _AddFreight = Convert.ToDouble(collection["addfreight"]) == 0 ? 0 : Convert.ToDouble(collection["addfreight"]);
                Double _Freight = Convert.ToDouble(collection["freight"]) == 0 ? 0 : Convert.ToDouble(collection["freight"]);
                Double _Excess = Convert.ToDouble(collection["excess"]) == 0 ? 0 : Convert.ToDouble(collection["excess"]);
                Double _Travel = Convert.ToDouble(collection["travel"]) == 0 ? 0 : Convert.ToDouble(collection["travel"]);
                DateTime _DueDate = collection["duedate"] == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(collection["duedate"].ToString());

                Double _Misc = _AddFreight + _Freight;
                Double _GrandTotal = _PartAmount + _Misc + _Repair + _Material + _Travel;
                Double _GST = _GrandTotal * gst.Percentage / 100;
                Double _TotalAmount = _GrandTotal + _GST;
                Double _TotalDue = _TotalAmount - _Excess;

                invoice.SubTotal = Convert.ToDouble(_GrandTotal.ToString("0.00"));
                invoice.StatusID = int.Parse(_Status);
                invoice.GST = Convert.ToDouble(_GST.ToString("0.00"));
                invoice.Total = Convert.ToDouble(_TotalAmount.ToString("0.00"));
                invoice.TotalNet = Convert.ToDouble(_TotalDue.ToString("0.00"));
                invoice.LastModifyDate = DateTime.Now;
                invoice.LastModifyUser = _User;

                if (_Status != null)
                {
                    if (_Status == "1")
                    {
                        invoice.InvoiceStatus = "Paid";
                    }
                    if (_Status == "2")
                    {
                        invoice.InvoiceStatus = "Partial";
                    }
                    if (_Status == "3")
                    {
                        invoice.InvoiceStatus = "UnPaid";
                    }
                    if (_Status == "4")
                    {
                        invoice.InvoiceStatus = "Withdrawn";
                    }
                    if (_Status == "5")
                    {
                        invoice.InvoiceStatus = "Credit";
                    }
                }


                _ISales.UpdateInvoice(invoice);

                var invoicelist = db.SalesInvoiceLists.SingleOrDefault(b => b.InvoiceID == invoice.InvoiceID);

                if (invoicelist != null)
                {
                    invoicelist.CustomVar1 = _RepairNote;
                    invoicelist.CustomVar2 = _PartNote;
                    invoicelist.CustomVar3 = _MaterialNote;
                    invoicelist.CustomVar4 = "";
                    invoicelist.CustomVar5 = _TaskNote;
                    invoicelist.CustomFloat1 = Convert.ToDouble(_Repair.ToString("0.00"));
                    invoicelist.CustomFloat2 = Convert.ToDouble(_PartAmount.ToString("0.00"));
                    invoicelist.CustomFloat3 = Convert.ToDouble(_Material.ToString("0.00"));
                    invoicelist.CustomFloat4 = Convert.ToDouble(_AddFreight.ToString("0.00"));
                    invoicelist.CustomFloat5 = Convert.ToDouble(_Freight.ToString("0.00"));
                    invoicelist.CustomFloat6 = Convert.ToDouble(_Excess.ToString("0.00"));
                    invoicelist.CustomFloat7 = Convert.ToDouble(_Travel.ToString("0.00"));
                    invoicelist.CustomFloat8 = 0;
                    invoicelist.CustomFloat9 = 0;
                    invoicelist.CustomFloat10 = 0;
                    invoicelist.LastModifyDate = DateTime.Now;
                    invoicelist.LastModifyUser = _User;
                    _ISales.UpdateInvoiceList(invoicelist);
                }
                else
                {
                    objinvoicelist.SegmentID = _Segment;
                    objinvoicelist.InvoiceID = invoice.InvoiceID;
                    objinvoicelist.InvoiceNo = invoice.InvoiceNo;
                    objinvoicelist.CustomVar1 = _RepairNote;
                    objinvoicelist.CustomVar2 = _PartNote;
                    objinvoicelist.CustomVar3 = _MaterialNote;
                    objinvoicelist.CustomVar4 = "";
                    objinvoicelist.CustomVar5 = _TaskNote;
                    objinvoicelist.CustomFloat1 = Convert.ToDouble(_Repair.ToString("0.00"));
                    objinvoicelist.CustomFloat2 = Convert.ToDouble(_PartAmount.ToString("0.00"));
                    objinvoicelist.CustomFloat3 = Convert.ToDouble(_Material.ToString("0.00"));
                    objinvoicelist.CustomFloat4 = Convert.ToDouble(_AddFreight.ToString("0.00"));
                    objinvoicelist.CustomFloat5 = Convert.ToDouble(_Freight.ToString("0.00"));
                    objinvoicelist.CustomFloat6 = Convert.ToDouble(_Excess.ToString("0.00"));
                    objinvoicelist.CustomFloat7 = Convert.ToDouble(_Travel.ToString("0.00"));
                    objinvoicelist.CustomFloat8 = 0;
                    objinvoicelist.CustomFloat9 = 0;
                    objinvoicelist.CustomFloat10 = 0;
                    objinvoicelist.LastModifyDate = DateTime.Now;
                    objinvoicelist.LastModifyUser = _User;
                    _ISales.InsertInvoiceList(objinvoicelist);
                }
                TempData["Success"] = "Success";
                return RedirectToAction("List", "Job");
            }
        }

    }
}