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

namespace UILAB.Controllers.Sales
{
    [ValidateAdminSession]
    public class SaleInvoiceJobController : Controller
    {
        SalesInvoiceTR objinvoice = new SalesInvoiceTR();
        SalesInvoiceListTR objinvoicelist = new SalesInvoiceListTR();
        VehicleTR objvehicles = new VehicleTR();
        VehicleMakeTB objvehiclemake = new VehicleMakeTB();
        VehicleModelTB objvehiclemodel = new VehicleModelTB();
        VehicleMileageTR objmileage = new VehicleMileageTR();
        ApprovalHeaderTR objapprovalheader = new ApprovalHeaderTR();
        ApprovalDetailTR objapprovaldetail = new ApprovalDetailTR();
        JobTR objjob = new JobTR();
        CustomerTB objcustomer = new CustomerTB();
        JobAssignEmpTR objjobemployee = new JobAssignEmpTR();

        IJob _IJob;
        ISales _ISales;
        IVehicle _IVehicle;
        IUser _IUser;
        IApproval _IApproval;

        public SaleInvoiceJobController()
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
            ViewBag.Current = "Sales";
            ViewBag.CurrentSub = "InvoiceJob";
            TempData["Error"] = "";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.vw_SalesInvoiceTRs where data.SegmentID == _Segment orderby data.InvoiceNo descending select data).ToList();
                return View(list.ToList());
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Current = "Sales";
            ViewBag.CurrentSub = "InvoiceJob";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var vehicle = (from data in db.VehicleTRs orderby data.PlateNo ascending select data).ToList();
                ViewBag.VehicleList = new SelectList(vehicle, "VehicleID", "PlateNo");

                var status = (from data in db.SalesStatusTBs where data.StatusType == 1 orderby data.StatusName select data).ToList();
                ViewBag.statuslist = new SelectList(status, "StatusID", "StatusName");

                var make = (from data in db.VehicleMakes orderby data.Make ascending select data).ToList();
                ViewBag.MakeList = new SelectList(make, "MakeID", "Make");

                var model = (from data in db.VehicleModels orderby data.ModelName ascending select data).ToList();
                ViewBag.ModelList = new SelectList(model, "ModelID", "ModelName");

                //TempData["repairnote"] = "(Listed below description)";
                TempData["partnote"] = "Parts";
                TempData["materialnote"] = "Materials";
                TempData["miscellnote"] = "WOF";

                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SalesInvoiceTR item, FormCollection collection, HttpPostedFileBase[] FileUpload)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var gst = db.SalesTaxes.SingleOrDefault(b => b.Code == "GST" && b.IsActive == true);
                               
                var jobtype = db.JobTypes.SingleOrDefault(b => b.JobTypeName == "Custom");

                string _JobTRID = "";
                string _RepairNote = collection["repairnote"] == null ? string.Empty : collection["repairnote"].Trim();
                string _PartNote = collection["partnote"] == null ? string.Empty : collection["partnote"].Trim();
                string _MaterialNote = collection["materialnote"] == null ? string.Empty : collection["materialnote"].Trim();
                string _MiscellNote = collection["miscellnote"] == null ? string.Empty : collection["miscellnote"].Trim();
                int _Status = item.StatusID == null ? -1 : int.Parse(item.StatusID.ToString());
                int _VehicleID = item.VehicleID == null ? -1 : int.Parse(item.VehicleID.ToString());
                string _Repair = collection["repair"] == "" ? "0" : collection["repair"].Trim();
                string _PartAmount = collection["partamount"] == "" ? "0" : collection["partamount"].Trim();
                string _Material = collection["material"] == "" ? "0" : collection["material"].Trim();
                string _Freight = collection["freight"] == "" ? "0" : collection["freight"].Trim();
                string _Miscell = collection["miscellamount"] == "" ? "0" : collection["miscellamount"].Trim();
                string _Milage = collection["mileage"] == null ? string.Empty : collection["mileage"].Trim();
                string _Hubo = collection["hubo"] == null ? string.Empty : collection["hubo"].Trim();
                string _RUC = collection["ruc"] == null ? string.Empty : collection["ruc"].Trim();
                DateTime _DueDate = collection["duedate"] == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(collection["duedate"].ToString());
                DateTime _JobStartDate = collection["startdate"] == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(collection["startdate"].ToString());
                DateTime _JobFinishDate = collection["finishdate"] == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(collection["finishdate"].ToString());
                //DateTime _RegoExpiryDate = collection["regodate"] == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(collection["regodate"].ToString());
                //DateTime _WOFExpiryDate = collection["wofdate"] == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(collection["wofdate"].ToString());

                Double _GrandTotal = Convert.ToDouble(_PartAmount) + Convert.ToDouble(_Miscell) + Convert.ToDouble(_Repair) + Convert.ToDouble(_Material);
                Double _GST = _GrandTotal * gst.Percentage / 100;
                Double _TotalAmount = _GrandTotal + _GST;

                string newvehiclecheck = collection["checkall"];
                bool _IsNewVehicle = Convert.ToBoolean(newvehiclecheck);

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

                    var plateno = db.VehicleTRs.SingleOrDefault(b => b.PlateNo == _NewPlateNo);

                    if (plateno != null)
                    {
                        TempData["ErrorMessage"] = "Plate No Allready Exist";
                        return RedirectToAction("Create", "SaleInvoiceJob");
                    }
                    else
                    {
                        #region --> Create New Make and Model

                        if (_MakeID == -1 && _NewMake == "")
                        {
                            TempData["ErrorMessage"] = "Vehicle Make cannot be Empty";
                            return RedirectToAction("Create", "SaleInvoiceJob");
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
                            return RedirectToAction("Create", "SaleInvoiceJob");

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
                            objvehicles.MileageID = -1;
                            objvehicles.Milage = _Milage;
                            objvehicles.Hubo = _Hubo;
                            objvehicles.RUC = _RUC;
                            objvehicles.Rego = "Rego";
                            objvehicles.RegoExpiryDate = DateTime.Parse("1900-01-01");
                            objvehicles.WOF = "WOF";
                            objvehicles.WOFExpiryDate = DateTime.Parse("1900-01-01");
                            objvehicles.Remark = "";
                            objvehicles.Flagged = false;
                            objvehicles.LastModifyDate = DateTime.Now;
                            objvehicles.LastModifyUser = _User;
                            int _NewVehicleID = _IVehicle.InsertVehicle(objvehicles);
                            Session["NewVehicle"] = Convert.ToString(_NewVehicleID);

                            if (_Milage != "" || _Hubo != "" || _RUC != "")
                            {
                                objmileage.SegmentID = _Segment;
                                objmileage.Mileage = _Milage;
                                objmileage.VehicleID = _NewVehicleID;
                                objmileage.Hubo = _Hubo;
                                objmileage.RUC = _RUC;
                                objmileage.Rego = "Rego";
                                objmileage.RegoExpiryDate = DateTime.Parse("1900-01-01");
                                objmileage.WOF = "WOF";
                                objmileage.WOFExpiryDate = DateTime.Parse("1900-01-01");
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
                                    updatemileage.LastModifyDate = DateTime.Now;
                                    updatemileage.LastModifyUser = _User;
                                    _IVehicle.UpdateVehicle(updatemileage);
                                }
                            }
                        }

                        #endregion
                    }
                }

                #region --> Update Mileage 

                if (_Milage != "" || _Hubo != "" || _RUC != "")
                {
                    objmileage.SegmentID = _Segment;
                    objmileage.Mileage = _Milage;
                    objmileage.VehicleID = _VehicleID;
                    objmileage.Hubo = _Hubo;
                    objmileage.RUC = _RUC;
                    objmileage.Rego = "Rego";
                    objmileage.RegoExpiryDate = DateTime.Parse("1900-01-01");
                    objmileage.WOF = "WOF";
                    objmileage.WOFExpiryDate = DateTime.Parse("1900-01-01");
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
                        updatemileage.LastModifyDate = DateTime.Now;
                        updatemileage.LastModifyUser = _User;
                        _IVehicle.UpdateVehicle(updatemileage);
                    }
                }

                #endregion

                #region --> Generate Invoice Number

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

                #endregion

                #region --> Generate Job No.

                var jobprefix = db.SettingGenerals.SingleOrDefault(b => b.PrefixType == 1);

                if (jobprefix == null)
                {
                    TempData["Prefix"] = "Job Prefix Not Setup ...!";
                }
                else
                {
                    string _JobPrefix = jobprefix.Prefix;
                    string _StartJobNo = jobprefix.StartNo;

                    List<JobTR> startjoblist = db.JobTRs.ToList();

                    if (startlist.Count == 0)
                    {
                        _JobTRID = _JobPrefix + _StartNo;
                    }
                    else
                    {
                        // ----------- Split and Next Job ID ----------------
                        var lastNumber = startjoblist.Last().JobTRID;

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
                    var vehicle = db.VehicleTRs.SingleOrDefault(b => b.VehicleID == item.VehicleID);
                    objjob.VehicleID = _VehicleID;
                    objjob.PlateNo = vehicle.PlateNo;
                }

                objjob.JobTRID = _JobTRID;
                objjob.JobTypeID = jobtype.JobTypeID;
                objjob.JobTaskTypeID = -1;
                objjob.PackageID = -1;
                objjob.JobStartDate = _JobStartDate;
                objjob.JobFinishDate = _JobFinishDate;
                objjob.StatusID = 1;
                objjob.Flagged = false;
                objjob.LastModifyDate = DateTime.Now;
                objjob.LastModifyUser = _User;
                objjob.Remark = "";
                objjob.Description = "";
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

                #region --> Update Vehicle Flag

                var vehicleflag = db.VehicleTRs.SingleOrDefault(b => b.VehicleID == _VehicleID);
                if (vehicleflag.Flagged != true)
                {
                    vehicleflag.Flagged = true;
                    vehicleflag.LastModifyDate = DateTime.Now;
                    vehicleflag.LastModifyUser = _User;
                    db.SaveChanges();
                }
                else
                {
                    var newvehicleflag = db.VehicleTRs.SingleOrDefault(b => b.VehicleID == Convert.ToInt32(HttpContext.Session["NewVehicle"]));

                    if (newvehicleflag.Flagged != true)
                    {
                        newvehicleflag.Flagged = true;
                        newvehicleflag.LastModifyDate = DateTime.Now;
                        newvehicleflag.LastModifyUser = _User;
                        db.SaveChanges();
                    }

                }

                #endregion

                #region --> Create Invoice

                var employee = db.JobAssignEmpTRs.SingleOrDefault(b => b.JobID == returnJobID);

                objinvoice.SegmentID = _Segment;
                objinvoice.JobID = returnJobID;
                objinvoice.InvoiceNo = _InvoiceNo;
                objinvoice.POID = -1;

                if (_VehicleID == -1)
                {
                    objinvoice.VehicleID = Convert.ToInt32(HttpContext.Session["NewVehicle"]);
                }
                else
                {
                    objinvoice.VehicleID = _VehicleID;
                }

                objinvoice.SubTotal = Convert.ToDouble(_GrandTotal.ToString("0.00"));
                objinvoice.GST = Convert.ToDouble(_GST.ToString("0.00"));
                objinvoice.Total = Convert.ToDouble(_TotalAmount.ToString("0.00"));
                objinvoice.TotalNet = Convert.ToDouble(_TotalAmount.ToString("0.00")); 
                
                if (_Status == -1)
                {
                    objinvoice.StatusID = 3;
                }
                else
                {
                    objinvoice.StatusID = _Status;
                }
                               
                var status = db.SalesStatusTBs.SingleOrDefault(b => b.StatusID == _Status);

                if (status == null)
                {
                    objinvoice.InvoiceStatus = "UnPaid";
                }
                else
                {
                    objinvoice.InvoiceStatus = status.StatusName;
                }

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

                //Create Invoice List
                objinvoicelist.SegmentID = _Segment;
                objinvoicelist.InvoiceID = ReturnInvoiceID;
                objinvoicelist.InvoiceNo = _InvoiceNo;

                if (_Status == -1)
                {
                    objinvoice.StatusID = 3;
                }
                else
                {
                    objinvoice.StatusID = _Status;
                }

                objinvoicelist.CustomVar1 = _RepairNote;
                objinvoicelist.CustomVar2 = _PartNote;
                objinvoicelist.CustomVar3 = _MaterialNote;
                objinvoicelist.CustomVar4 = _MiscellNote;
                objinvoicelist.CustomVar5 = "";
                objinvoicelist.CustomFloat1 = Convert.ToDouble(_Repair);
                objinvoicelist.CustomFloat2 = Convert.ToDouble(_PartAmount);
                objinvoicelist.CustomFloat3 = Convert.ToDouble(_Material);
                objinvoicelist.CustomFloat4 = Convert.ToDouble(_Miscell);
                objinvoicelist.CustomFloat5 = Convert.ToDouble(_Freight);
                objinvoicelist.CustomFloat6 = 0;
                objinvoicelist.CustomFloat7 = 0;
                objinvoicelist.CustomFloat8 = 0;
                objinvoicelist.CustomFloat9 = 0;
                objinvoicelist.CustomFloat10 = 0;
                objinvoicelist.LastModifyDate = DateTime.Now;
                objinvoicelist.LastModifyUser = _User;
                _ISales.InsertInvoiceList(objinvoicelist);

                var jobupdate = db.JobTRs.SingleOrDefault(b => b.JobID == returnJobID);
                if (jobupdate != null)
                {
                    jobupdate.InvoiceID = ReturnInvoiceID;
                    jobupdate.JobFinishDate = DateTime.Now;
                    jobupdate.LastModifyDate = DateTime.Now;
                    jobupdate.LastModifyUser = _User;
                    jobupdate.StatusID = 2;
                    db.SaveChanges();
                }

                var updatetask = (from data in db.JobTaskTRs where data.JobID == item.JobID select data).ToList();
                if (updatetask.Count > 0)
                {
                    foreach (var task in updatetask)
                    {
                        task.LastModifyDate = DateTime.Now;
                        task.LastModifyUser = _User;
                        task.StatusID = 2;
                        task.CompletedBy = employee.EmployeeNo;
                        db.SaveChanges();
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

                TempData["Success"] = "Success";
                return RedirectToAction("Details", "SaleInvoiceJob", new { id = ReturnInvoiceID });
            }
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            ViewBag.Current = "Sales";
            ViewBag.CurrentSub = "InvoiceJob";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            using (var db = new DatabaseContext())
            {
                var result = db.vw_SalesInvoiceTRs.SingleOrDefault(b => b.InvoiceID == id && b.SegmentID == _Segment);

                if (result == null)
                {
                    return View(List());
                }
                else
                {
                    return View(db.vw_SalesInvoiceTRs.Where(x => x.InvoiceID == id).FirstOrDefault<vw_SalesInvoiceTR>());
                }
            }
        }

        [HttpGet]
        public ActionResult InvoiceDetail(int id)
        {
            ViewBag.Current = "Sales";
            ViewBag.CurrentSub = "InvoiceJob";
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
                var invoice = db.vw_SalesInvoiceTRs.SingleOrDefault(b => b.InvoiceID == id && b.SegmentID == _Segment);

                if (invoice == null)
                {
                    return View(List());
                }
                else
                {
                    var status = (from data in db.SalesStatusTBs orderby data.StatusName select data).ToList();
                    ViewBag.statuslist = new SelectList(status, "StatusID", "StatusName");

                    TempData["duedate"] = invoice.DueDate;

                    var invoicelist = db.SalesInvoiceLists.SingleOrDefault(b => b.InvoiceID == id && b.SegmentID == _Segment);

                    if (invoicelist == null)
                    {
                        TempData["invoiceno"] = invoice.InvoiceNo;
                        TempData["repairnote"] = "";
                        TempData["partnote"] = "";
                        TempData["materialnote"] = "";
                        TempData["miscellnote"] = "";
                        TempData["tasknote"] = "";
                        TempData["repair"] = 0;
                        TempData["partamount"] = 0;
                        TempData["material"] = 0;
                        TempData["miscellamount"] = 0;
                        TempData["freight"] = 0;
                    }
                    else
                    {
                        TempData["invoiceno"] = invoicelist.InvoiceNo;
                        TempData["repairnote"] = invoicelist.CustomVar1;
                        TempData["partnote"] = invoicelist.CustomVar2;
                        TempData["materialnote"] = invoicelist.CustomVar3;
                        TempData["miscellnote"] = invoicelist.CustomVar4;
                        TempData["tasknote"] = invoicelist.CustomVar5;
                        TempData["repair"] = invoicelist.CustomFloat1.ToString("#,##0.00");
                        TempData["partamount"] = invoicelist.CustomFloat2.ToString("#,##0.00");
                        TempData["material"] = invoicelist.CustomFloat3.ToString("#,##0.00");
                        TempData["miscellamount"] = invoicelist.CustomFloat4.ToString("#,##0.00");
                        TempData["freight"] = invoicelist.CustomFloat5.ToString("#,##0.00");
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
                //var scheme = db.ApprovalSettings.SingleOrDefault(b => b.Code == "INVJOB");
                //int _ApprovalSchemeID = scheme.ApprovalSchemeID == null ? -1 : int.Parse(scheme.ApprovalSchemeID.ToString());
                var gst = db.SalesTaxes.SingleOrDefault(b => b.Code == "GST" && b.IsActive == true);


                string _RepairNote = collection["repairnote"] == null ? string.Empty : collection["repairnote"].Trim();
                string _PartNote = collection["partnote"] == null ? string.Empty : collection["partnote"].Trim();
                string _MaterialNote = collection["materialnote"] == null ? string.Empty : collection["materialnote"].Trim();
                string _MiscellNote = collection["miscellnote"] == null ? string.Empty : collection["miscellnote"].Trim();
                int _Status = item.StatusID == 0 ? -1 : int.Parse(item.StatusID.ToString());
                string _Repair = collection["repair"] == "" ? "0" : collection["repair"].Trim();
                string _PartAmount = collection["partamount"] == "" ? "0" : collection["partamount"].Trim();
                string _Material = collection["material"] == "" ? "0" : collection["material"].Trim();
                string _Freight = collection["freight"] == "" ? "0" : collection["freight"].Trim();
                string _Miscell = collection["miscellamount"] == "" ? "0" : collection["miscellamount"].Trim();
                DateTime _DueDate = collection["duedate"] == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(collection["duedate"].ToString());

                Double _GrandTotal = Convert.ToDouble(_PartAmount) + Convert.ToDouble(_Miscell) + Convert.ToDouble(_Repair) + Convert.ToDouble(_Material);
                Double _GST = _GrandTotal * gst.Percentage / 100;
                Double _TotalAmount = _GrandTotal + _GST;

                var invoice = db.SalesInvoiceTRs.SingleOrDefault(b => b.InvoiceID == _InvoiceID);

                if (invoice != null)
                {
                    invoice.SubTotal = Convert.ToDouble(_GrandTotal.ToString("0.00"));
                    invoice.GST = Convert.ToDouble(_GST.ToString("0.00"));
                    invoice.Total = Convert.ToDouble(_TotalAmount.ToString("0.00"));
                    invoice.TotalNet = Convert.ToDouble(_TotalAmount.ToString("0.00"));
                    invoice.Flagged = false;
                    invoice.DueDate = DateTime.Now;
                    invoice.LastModifyDate = DateTime.Now;
                    invoice.LastModifyUser = _User;
                    invoice.StatusID = _Status;

                    var status = db.SalesStatusTBs.SingleOrDefault(b => b.StatusID == _Status);

                    if (status != null)
                    {
                        invoice.InvoiceStatus = status.StatusName;
                    }
                    else
                    {
                        invoice.InvoiceStatus = "UnPaid";
                    }
                    _ISales.UpdateInvoice(invoice);
                }


                var invoicelist = db.SalesInvoiceLists.SingleOrDefault(b => b.InvoiceID == invoice.InvoiceID);

                if (invoicelist != null)
                {
                    invoicelist.SegmentID = _Segment;
                    invoicelist.StatusID = _Status;
                    invoicelist.CustomVar1 = _RepairNote;
                    invoicelist.CustomVar2 = _PartNote;
                    invoicelist.CustomVar3 = _MaterialNote;
                    invoicelist.CustomVar4 = _MiscellNote;
                    invoicelist.CustomVar5 = "";
                    invoicelist.CustomFloat1 = Convert.ToDouble(_Repair);
                    invoicelist.CustomFloat2 = Convert.ToDouble(_PartAmount);
                    invoicelist.CustomFloat3 = Convert.ToDouble(_Material);
                    invoicelist.CustomFloat4 = Convert.ToDouble(_Miscell);
                    invoicelist.CustomFloat5 = Convert.ToDouble(_Freight);
                    invoicelist.CustomFloat6 = 0;
                    invoicelist.CustomFloat7 = 0;
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
                    objinvoicelist.CustomVar4 = _MiscellNote;
                    objinvoicelist.CustomVar5 = "";
                    objinvoicelist.CustomFloat1 = Convert.ToDouble(_Repair);
                    objinvoicelist.CustomFloat2 = Convert.ToDouble(_PartAmount);
                    objinvoicelist.CustomFloat3 = Convert.ToDouble(_Material);
                    objinvoicelist.CustomFloat4 = Convert.ToDouble(_Miscell);
                    objinvoicelist.CustomFloat5 = Convert.ToDouble(_Freight);
                    objinvoicelist.CustomFloat6 = 0;
                    objinvoicelist.CustomFloat7 = 0;
                    objinvoicelist.CustomFloat8 = 0;
                    objinvoicelist.CustomFloat9 = 0;
                    objinvoicelist.CustomFloat10 = 0;
                    objinvoicelist.LastModifyDate = DateTime.Now;
                    objinvoicelist.LastModifyUser = _User;
                    _ISales.InsertInvoiceList(objinvoicelist);
                }

                TempData["Success"] = "Success";
                return RedirectToAction("Details", "SaleInvoiceJob", new { id = invoice.InvoiceID });
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

        public ActionResult FillInfo(int info)
        {
            using (var db = new DatabaseContext())
            {
                var vehicleinfo = db.vw_VehicleTRs.SingleOrDefault(b => b.VehicleID == info);

                //TempData["makeselect"] = vehicleinfo.Make;
                //TempData["modelselect"] = vehicleinfo.ModelName;
                //TempData["customerselect"] = vehicleinfo.FirstName;
                //TempData["mileage"] = vehicleinfo.Milage;

                return Json(vehicleinfo, JsonRequestBehavior.AllowGet);
            }
        }

    }
}