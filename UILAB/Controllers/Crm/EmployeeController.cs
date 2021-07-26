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
using System.Data.OleDb;
using System.Data;
using LinqToExcel;
using System.Data.Entity.Validation;

namespace UILAB.Controllers.Crm
{
    [ValidateAdminSession]
    public class EmployeeController : Controller
    {
        EmployeeMaster objemployee = new EmployeeMaster();
        AddressBookTB objaddressbook = new AddressBookTB();

        IUser _IUser;
        public EmployeeController()
        {
            _IUser = new UserConcrete();
        }

        [HttpGet]
        public ActionResult List()
        {
            ViewBag.Current = "User";
            ViewBag.CurrentSub = "Employee";
            TempData["Error"] = "";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.vw_EmployeeTB where data.SegmentID == _Segment orderby data.UserName ascending select data).ToList();
                return View(list);
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Current = "User";
            ViewBag.CurrentSub = "Employee";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var gender = (from data in db.GenderMasters orderby data.Gender ascending select data).ToList();
                ViewBag.GenderList = new SelectList(gender, "GenderID", "Gender");

                var country = (from data in db.CountryMasters orderby data.CountryName ascending select data).ToList();
                ViewBag.CountryList = new SelectList(country, "CountryID", "CountryName");

                var designation = (from data in db.DesignationMasters where data.SegmentID == _Segment orderby data.DesignationName ascending select data).ToList();
                ViewBag.DesignationList = new SelectList(designation, "DesignationID", "DesignationName");

                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EmployeeMaster item, FormCollection collection)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var result = db.EmployeeMasters.SingleOrDefault(b => b.UserName == item.UserName && b.SegmentID == _Segment);

                if (result != null)
                {
                    TempData["ErrorMessage"] = "Username  Exists ...!";
                    return RedirectToAction("Create", "Employee");
                }
                else
                {
                    var password = EncryptionLibrary.EncryptText("password");
                    string contract = collection["iscontract"];
                    bool _IsContract = Convert.ToBoolean(contract);

                    string active = collection["isactive"];
                    bool _IsActive = Convert.ToBoolean(active);

                    string _FirstName = item.FirstName == null ? string.Empty : item.FirstName.Trim();
                    string _MiddleName = item.MiddleName == null ? string.Empty : item.MiddleName.Trim();
                    string _LastName = item.LastName == null ? string.Empty : item.LastName.Trim();
                    string _OtherName = item.OtherName == null ? string.Empty : item.OtherName.Trim();
                    string _Address1 = item.Address1 == null ? string.Empty : item.Address1.Trim();
                    string _Address2 = item.Address2 == null ? string.Empty : item.Address2.Trim();
                    string _Address3 = item.Address3 == null ? string.Empty : item.Address3.Trim();
                    string _DocumentEmployeeNo = item.DocumentEmployeeNo == null ? string.Empty : item.DocumentEmployeeNo.Trim();
                    string _PostalNo = item.PostalNo == null ? string.Empty : item.PostalNo.Trim();
                    int _CountryID = item.CountryID == null ? -1 : int.Parse(item.CountryID.ToString());
                    int _DesignationID = item.DesignationID == null ? -1 : int.Parse(item.DesignationID.ToString());
                    string _DrivingNo = item.DrivingLicenceNo == null ? string.Empty : item.DrivingLicenceNo.Trim();
                    DateTime _DOB = collection["dateofbirth"] == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(collection["dateofbirth"].ToString());
                    DateTime _DateJoined = item.DateJoined == null ? DateTime.Parse("1900-01-01") : DateTime.Parse(item.DateJoined.ToString());
                    DateTime _DateLeft = item.DateLeft == null ? DateTime.Parse("1900-01-01") : DateTime.Parse(item.DateLeft.ToString());

                    objemployee.SegmentID = _Segment;
                    objemployee.FirstName = _FirstName;
                    objemployee.LastName = _LastName;
                    objemployee.MiddleName = _MiddleName;
                    objemployee.OtherName = _OtherName;
                    objemployee.GenderID = item.GenderID;
                    objemployee.UserName = item.UserName;
                    objemployee.DesignationID = _DesignationID;
                    objemployee.DocumentEmployeeNo = _DocumentEmployeeNo;
                    objemployee.Email = item.Email.ToString();
                    objemployee.Mobile = item.Mobile.ToString();
                    objemployee.DOB = _DOB;
                    objemployee.DateJoined = _DateJoined;
                    objemployee.DateLeft = _DateLeft;
                    objemployee.IsCitizen = false;
                    objemployee.EmpLevelID = -1;
                    objemployee.TaxFileNo = "";
                    objemployee.BankID = -1;
                    objemployee.Password = password;
                    objemployee.ConfirmPassword = "";
                    objemployee.Address1 = _Address1;
                    objemployee.Address2 = _Address2;
                    objemployee.Address3 = _Address3;
                    objemployee.DrivingLicenceNo = _DrivingNo;
                    objemployee.PostalNo = _PostalNo;
                    objemployee.CountryID = _CountryID;
                    objemployee.RoleID = 3;
                    objemployee.AddressID = -1;
                    objemployee.Flagged = false;
                    objemployee.IsActive = _IsActive;
                    objemployee.IsContract = _IsContract;
                    objemployee.LastModifyDate = DateTime.Now;
                    objemployee.LastModifyUser = _User;
                    int _EmployeeNo = _IUser.InsertEmployee(objemployee); // Save and Return ID

                    if (_Address1 != "" && _Address2 != "" && _Address3 != "")
                    {
                        // ----------------------------- Save Address -----------------------------
                        objaddressbook.SegmentID = _Segment;
                        objaddressbook.EntryID = _EmployeeNo;
                        objaddressbook.RoleID = 3;
                        objaddressbook.Address1 = _Address1;
                        objaddressbook.Address2 = _Address2;
                        objaddressbook.Address3 = _Address3;
                        objaddressbook.PostalNo = _PostalNo;
                        objaddressbook.CountryID = _CountryID;
                        objaddressbook.IsActive = true;
                        objaddressbook.LastModifyDate = DateTime.Now;
                        objaddressbook.LastModifyUser = _User;
                        int _AddressID = _IUser.InsertAddress(objaddressbook);  // Save and Return ID

                        // ----------------------------- Update Address ID -----------------------------
                        var result2 = db.EmployeeMasters.SingleOrDefault(b => b.EmployeeNo == _EmployeeNo);
                        result2.AddressID = _AddressID;
                        db.SaveChanges();
                        //------------------------------------------------------------------------------
                    }
                    //------------------------------------------------------------------------------
                    TempData["Success"] = "Success";
                    return RedirectToAction("List", "Employee");
                }
            }
        }

        public ActionResult CreateBulk()
        {
            ViewBag.Current = "User";
            ViewBag.CurrentSub = "Employee";

            using (var db = new DatabaseContext())
            {
                return View();
            }
        }

        public FileResult DownloadExcel()
        {
            string path = "/Documents/BulkEmployee.xlsx";
            return File(path, "application/vnd.ms-excel", "BulkEmployee.xlsx");
        }

        [HttpPost]
        public ActionResult CreateBulk(EmployeeMaster users, HttpPostedFileBase FileUpload)
        {
            if (FileUpload == null)
            {
                TempData["ErrorMessage"] = "Select a File to Upload ...!";
                return RedirectToAction("CreateBulk", "Employee");
            }
            else
            {
                string filename = DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss") + "_" + FileUpload.FileName;
                string targetpath = Server.MapPath("~/Files/Uploads/");
                FileUpload.SaveAs(targetpath + filename);
                string pathToExcelFile = targetpath + filename;
                var connectionString = "";
                if (filename.EndsWith(".xls"))
                {
                    connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                }
                else if (filename.EndsWith(".xlsx"))
                {
                    connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                }

                var adapter = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", connectionString);
                var ds = new DataSet();

                adapter.Fill(ds, "ExcelTable");

                DataTable dtable = ds.Tables["ExcelTable"];

                string sheetName = "Sheet1";

                var excelFile = new ExcelQueryFactory(pathToExcelFile);
                var artistAlbums = from item in excelFile.Worksheet<EmployeeMaster>(sheetName) select item;
                var password = EncryptionLibrary.EncryptText("password");


                foreach (var item in artistAlbums)
                {
                    try
                    {
                        string _FirstName = item.FirstName == null ? string.Empty : item.FirstName.Trim();
                        string _MiddleName = item.MiddleName == null ? string.Empty : item.MiddleName.Trim();
                        string _LastName = item.LastName == null ? string.Empty : item.LastName.Trim();
                        string _OtherName = item.OtherName == null ? string.Empty : item.OtherName.Trim();
                        string _Email = item.Email == null ? string.Empty : item.Email.Trim();
                        string _Mobile = item.Mobile == null ? string.Empty : item.Mobile.Trim();
                        string _Address1 = item.Address1 == null ? string.Empty : item.Address1.Trim();
                        string _Address2 = item.Address2 == null ? string.Empty : item.Address2.Trim();
                        string _Address3 = item.Address3 == null ? string.Empty : item.Address3.Trim();
                        string _DocumentEmployeeNo = item.DocumentEmployeeNo == null ? string.Empty : item.DocumentEmployeeNo.Trim();
                        string _PostalNo = item.PostalNo == null ? string.Empty : item.PostalNo.Trim();
                        int _CountryID = item.CountryID == null ? -1 : int.Parse(item.CountryID.ToString());
                        int _GenderID = item.GenderID == null ? -1 : int.Parse(item.GenderID.ToString());
                        int _DesignationID = item.DesignationID == null ? -1 : int.Parse(item.DesignationID.ToString());
                        string _DrivingNo = item.DrivingLicenceNo == null ? string.Empty : item.DrivingLicenceNo.Trim();
                        string _UserName = item.UserName == null ? string.Empty : item.UserName.Trim();
                        DateTime _DOB = item.DOB == null ? DateTime.Parse("1900-01-01") : DateTime.Parse(item.DOB.ToString());
                        DateTime _DateJoined = item.DateJoined == null ? DateTime.Parse("1900-01-01") : DateTime.Parse(item.DateJoined.ToString());
                        DateTime _DateLeft = item.DateLeft == null ? DateTime.Parse("1900-01-01") : DateTime.Parse(item.DateLeft.ToString());

                        using (var db = new DatabaseContext())
                        {
                            EmployeeMaster TU = new EmployeeMaster();

                            TU.SegmentID = Convert.ToInt32(HttpContext.Session["Segment"]);
                            TU.FirstName = _FirstName;
                            TU.LastName = _LastName;
                            TU.MiddleName = _MiddleName;
                            TU.OtherName = _OtherName;
                            TU.GenderID = _GenderID;
                            TU.UserName = _UserName;
                            TU.Email = _Email;
                            TU.Mobile = _Mobile;
                            TU.DesignationID = _DesignationID;
                            TU.DocumentEmployeeNo = _DocumentEmployeeNo;
                            TU.DOB = _DOB;
                            TU.DateJoined = _DateJoined;
                            TU.DateLeft = _DateLeft;
                            TU.IsCitizen = false;
                            TU.TaxFileNo = "";
                            TU.BankID = -1;
                            TU.Password = password;
                            TU.ConfirmPassword = "";
                            TU.Address1 = _Address1;
                            TU.Address2 = _Address2;
                            TU.Address3 = _Address3;
                            TU.DrivingLicenceNo = _DrivingNo;
                            TU.PostalNo = _PostalNo;
                            TU.CountryID = _CountryID;
                            TU.RoleID = 3;
                            TU.AddressID = -1;
                            TU.Flagged = false;
                            TU.IsActive = true;
                            TU.IsContract = false;
                            TU.LastModifyDate = DateTime.Now;
                            TU.LastModifyUser = Convert.ToInt32(HttpContext.Session["AdminUser"]);

                            db.EmployeeMasters.Add(TU);
                            db.SaveChanges();

                            int returnID = TU.EmployeeNo;

                            if (TU.Address1 != "" && TU.Address2 != "" && TU.Address3 != "")
                            {
                                // ----------------------------- Save Address -----------------------------
                                var address = new AddressBookTB
                                {
                                    SegmentID = Convert.ToInt32(HttpContext.Session["Segment"]),
                                    EntryID = returnID,
                                    RoleID = 3,
                                    Address1 = TU.Address1,
                                    Address2 = TU.Address2,
                                    Address3 = TU.Address3,
                                    PostalNo = "",
                                    CountryID = -1,
                                    IsActive = true,
                                    LastModifyDate = DateTime.Now,
                                    LastModifyUser = Convert.ToInt32(HttpContext.Session["AdminUser"])
                                };
                                db.AddressBook.Add(address);
                                db.SaveChanges();

                                // ----------------------------- Update Address ID -----------------------------
                                int addressID = address.AddressID;
                                var result2 = db.EmployeeMasters.SingleOrDefault(b => b.EmployeeNo == returnID);
                                result2.AddressID = addressID;
                                db.SaveChanges();
                                //------------------------------------------------------------------------------
                            }
                        }
                    }
                    catch (DbEntityValidationException ex)
                    {
                        foreach (var entityValidationErrors in ex.EntityValidationErrors)
                        {
                            foreach (var validationError in entityValidationErrors.ValidationErrors)
                            {
                                Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                            }
                        }
                    }
                }
                TempData["Success"] = "Success";
                return RedirectToAction("List", "Employee");
            }
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            ViewBag.Current = "User";
            ViewBag.CurrentSub = "Employee";

            using (var db = new DatabaseContext())
            {
                var result = db.EmployeeMasters.SingleOrDefault(b => b.EmployeeNo == id);

                if (result == null)
                {
                    return View(List());
                }
                else
                {
                    var gender = (from data in db.GenderMasters orderby data.Gender ascending select data).ToList();
                    ViewBag.GenderList = new SelectList(gender, "GenderID", "Gender");

                    var designation = (from data in db.DesignationMasters where data.SegmentID == _Segment orderby data.DesignationName ascending select data).ToList();
                    ViewBag.DesignationList = new SelectList(designation, "DesignationID", "DesignationName");

                    var country = (from data in db.CountryMasters orderby data.CountryName ascending select data).ToList();
                    ViewBag.CountryList = new SelectList(country, "CountryID", "CountryName");

                    if (Convert.ToDateTime(result.DOB).ToString("yyyy-MM-dd") == "1900-01-01")
                    {
                        TempData["dateofbirth"] = "";
                    }
                    else
                    {
                        TempData["dateofbirth"] = Convert.ToDateTime(result.DOB).ToString("dd-MM-yyyy");
                    }

                    return View(db.EmployeeMasters.Where(x => x.EmployeeNo == id).FirstOrDefault<EmployeeMaster>());
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EmployeeMaster user, FormCollection collection)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var result = (from item in db.EmployeeMasters
                              where item.UserName == user.UserName && item.EmployeeNo != user.EmployeeNo && item.SegmentID == _Segment
                              select item).Count();

                if (result > 0)
                {
                    TempData["ErrorMessage"] = "Username Exists ...!";
                    return RedirectToAction("Edit", "Employee");
                }
                else
                {
                    string contract = collection["iscontract"];
                    bool _IsContract = Convert.ToBoolean(contract);

                    string active = collection["isactive"];
                    bool _IsActive = Convert.ToBoolean(active);

                    string _FirstName = user.FirstName == null ? string.Empty : user.FirstName.Trim();
                    string _MiddleName = user.MiddleName == null ? string.Empty : user.MiddleName.Trim();
                    string _LastName = user.LastName == null ? string.Empty : user.LastName.Trim();
                    string _OtherName = user.OtherName == null ? string.Empty : user.OtherName.Trim();
                    string _Email = user.Email == null ? string.Empty : user.Email.Trim();
                    string _Mobile = user.Mobile == null ? string.Empty : user.Mobile.Trim();
                    string _Address1 = user.Address1 == null ? string.Empty : user.Address1.Trim();
                    string _Address2 = user.Address2 == null ? string.Empty : user.Address2.Trim();
                    string _Address3 = user.Address3 == null ? string.Empty : user.Address3.Trim();
                    string _DocumentEmployeeNo = user.DocumentEmployeeNo == null ? string.Empty : user.DocumentEmployeeNo.Trim();
                    string _PostalNo = user.PostalNo == null ? string.Empty : user.PostalNo.Trim();
                    int _CountryID = user.CountryID == null ? -1 : int.Parse(user.CountryID.ToString());
                    int _GenderID = user.GenderID == null ? -1 : int.Parse(user.GenderID.ToString());
                    int _DesignationID = user.DesignationID == null ? -1 : int.Parse(user.DesignationID.ToString());
                    string _DrivingNo = user.DrivingLicenceNo == null ? string.Empty : user.DrivingLicenceNo.Trim();
                    string _UserName = user.UserName == null ? string.Empty : user.UserName.Trim();
                    DateTime _DOB = collection["dateofbirth"] == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(collection["dateofbirth"].ToString());
                    DateTime _DateJoined = user.DateJoined == null ? DateTime.Parse("1900-01-01") : DateTime.Parse(user.DateJoined.ToString());
                    DateTime _DateLeft = user.DateLeft == null ? DateTime.Parse("1900-01-01") : DateTime.Parse(user.DateLeft.ToString());


                    var updatedata = db.EmployeeMasters.SingleOrDefault(b => b.EmployeeNo == user.EmployeeNo);

                    if (updatedata == null)
                    {
                        TempData["Error"] = "Employee Update Failed...! Please Try Again";
                        return RedirectToAction("List", "Employee");
                    }
                    else
                    {
                        updatedata.FirstName = _FirstName;
                        updatedata.LastName = _LastName;
                        updatedata.MiddleName = _MiddleName;
                        updatedata.OtherName = _OtherName;
                        updatedata.DrivingLicenceNo = _DrivingNo;
                        updatedata.GenderID = _GenderID;
                        updatedata.DesignationID = _DesignationID;
                        updatedata.UserName = _UserName;
                        updatedata.DocumentEmployeeNo = _DocumentEmployeeNo;
                        updatedata.Email = user.Email;
                        updatedata.Mobile = user.Mobile;
                        updatedata.Address1 = _Address1;
                        updatedata.Address2 = _Address2;
                        updatedata.Address3 = _Address3;
                        updatedata.PostalNo = _PostalNo;
                        updatedata.DOB = _DOB;
                        updatedata.DateJoined = _DateJoined;
                        updatedata.DateLeft = _DateLeft;
                        updatedata.CountryID = _CountryID;
                        updatedata.IsActive = _IsActive;
                        updatedata.IsContract = _IsContract;
                        updatedata.LastModifyDate = DateTime.Now;
                        updatedata.LastModifyUser = _User;
                        _IUser.UpdateEmployee(updatedata);

                        if (_Address1 != "" && _Address2 != "" && _Address3 != "")
                        {
                            var addressedit = db.AddressBook.SingleOrDefault(b => b.EntryID == user.EmployeeNo && b.IsActive == true && b.RoleID == 3 && b.SegmentID == _Segment);
                            if (addressedit != null)
                            {
                                addressedit.IsActive = false;
                                addressedit.LastModifyDate = DateTime.Now;
                                addressedit.LastModifyUser = Convert.ToInt32(HttpContext.Session["AdminUser"]);
                                _IUser.UpdateAddress(addressedit);
                            }

                            objaddressbook.SegmentID = _Segment;
                            objaddressbook.EntryID = user.EmployeeNo;
                            objaddressbook.RoleID = 3;
                            objaddressbook.Address1 = _Address1;
                            objaddressbook.Address2 = _Address2;
                            objaddressbook.Address3 = _Address3;
                            objaddressbook.PostalNo = _PostalNo;
                            objaddressbook.CountryID = _CountryID;
                            objaddressbook.IsActive = true;
                            objaddressbook.LastModifyDate = DateTime.Now;
                            objaddressbook.LastModifyUser = _User;
                            int _AddressID = _IUser.InsertAddress(objaddressbook);  // Save and Return ID

                            // ----------------------------- Update Address ID -----------------------------
                            int addressID = _AddressID;
                            var employee = db.EmployeeMasters.SingleOrDefault(b => b.EmployeeNo == user.EmployeeNo);
                            employee.AddressID = addressID;
                            db.SaveChanges();
                            //------------------------------------------------------------------------------
                        }
                        else
                        {
                            TempData["Error"] = "Address Update Failed.";
                        }
                        TempData["Updated"] = "Updated";
                        return RedirectToAction("List", "Employee");
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

                var data = _IUser.DeleteEmployee(Convert.ToInt32(id), _Segment);

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
            ViewBag.CurrentSub = "Employee";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var result = db.vw_EmployeeTB.SingleOrDefault(b => b.EmployeeNo == id);

                if (result == null)
                {
                    return View(List());
                }
                else
                {
                    TempData["username"] = result.UserName;
                    TempData["fullname"] = result.FullName;
                    return View(db.vw_EmployeeTB.Where(x => x.EmployeeNo == id).FirstOrDefault<vw_EmployeeTB>());
                }
            }
        }

        [HttpGet]
        public ActionResult EmployeeInfo(int id)
        {
            
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var result = db.EmployeeMasters.SingleOrDefault(b => b.EmployeeNo == id);

                if (result == null)
                {
                    return View(List());
                }
                else
                {
                    TempData["username"] = result.UserName;
                    return View(db.vw_EmployeeTB.Where(x => x.EmployeeNo == id).FirstOrDefault<vw_EmployeeTB>());
                }
            }
        }

        [HttpGet]
        public ActionResult Security(int id)
        {
            
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var result = db.EmployeeMasters.SingleOrDefault(b => b.EmployeeNo == id);

                if (result == null)
                {
                    return View(List());
                }
                else
                {
                    TempData["username"] = result.UserName;
                    return View(db.vw_EmployeeTB.Where(x => x.EmployeeNo == id).FirstOrDefault<vw_EmployeeTB>());
                }
            }
        }

        [HttpGet]
        public ActionResult Logs(int id)
        {
           
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.SecurityActiveUser where data.SegmentID == _Segment && data.UserID == id && data.UserType == 3 orderby data.LoginInstance descending select data).ToList();
                return View(list);
            }
        }
    }
}