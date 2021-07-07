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
    public class CustomerController : Controller
    {
        CustomerTB objcustomer = new CustomerTB();
        AddressBookTB objaddressbook = new AddressBookTB();

        IUser _IUser;
        public CustomerController()
        {
            _IUser = new UserConcrete();
        }

        [HttpGet]
        public ActionResult List()
        {
            ViewBag.Current = "User";
            ViewBag.CurrentSub = "Customer";
            TempData["Error"] = "";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.Customers where data.SegmentID == _Segment orderby data.CustomerID descending select data).ToList();
                return View(list);
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Current = "User";
            ViewBag.CurrentSub = "Customer";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var gender = (from data in db.GenderMasters orderby data.Gender ascending select data).ToList();
                ViewBag.GenderList = new SelectList(gender, "GenderID", "Gender");

                var country = (from data in db.CountryMasters orderby data.CountryName ascending select data).ToList();
                ViewBag.CountryList = new SelectList(country, "CountryID", "CountryName");

                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CustomerTB item, FormCollection collection)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var result = db.Customers.SingleOrDefault(b => b.UserName == item.UserName && b.SegmentID == _Segment);

                if (result != null)
                {
                    TempData["ErrorMessage"] = "Username  Exists ...!";
                    return RedirectToAction("Create", "Customer");
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
                    string _Company = item.Company == null ? string.Empty : item.Company.Trim();
                    string _BusinessNo = item.BuisnessNo == null ? string.Empty : item.BuisnessNo.Trim();
                    string _GSTNo = item.GSTNo == null ? string.Empty : item.GSTNo.Trim();
                    string _Phone = item.Phone == null ? string.Empty : item.Phone.Trim();
                    string _Fax = item.Fax == null ? string.Empty : item.Fax.Trim();
                    string _Address1 = item.Address1 == null ? string.Empty : item.Address1.Trim();
                    string _Address2 = item.Address2 == null ? string.Empty : item.Address2.Trim();
                    string _Address3 = item.Address3 == null ? string.Empty : item.Address3.Trim();
                    string _PostalNo = item.PostalNo == null ? string.Empty : item.PostalNo.Trim();
                    int _CountryID = item.CountryID == null ? -1 : int.Parse(item.CountryID.ToString());
                    string _DrivingNo = item.DrivingLicenceNo == null ? string.Empty : item.DrivingLicenceNo.Trim();

                    objcustomer.SegmentID = _Segment;
                    objcustomer.FirstName = _FirstName;
                    objcustomer.LastName = _LastName;
                    objcustomer.MiddleName = _MiddleName;
                    objcustomer.OtherName = _OtherName;
                    objcustomer.GenderID = item.GenderID;
                    objcustomer.Company = _Company;
                    objcustomer.BuisnessNo = _BusinessNo;
                    objcustomer.GSTNo = _GSTNo;
                    objcustomer.UserName = item.UserName;
                    objcustomer.Email = item.Email.ToString();
                    objcustomer.Mobile = item.Mobile.ToString();
                    objcustomer.Fax = _Fax;
                    objcustomer.Phone = _Phone;
                    objcustomer.Password = password;
                    objcustomer.ConfirmPassword = "";
                    objcustomer.Address1 = _Address1;
                    objcustomer.Address2 = _Address2;
                    objcustomer.Address3 = _Address3;
                    objcustomer.DrivingLicenceNo = _DrivingNo;
                    objcustomer.PostalNo = _PostalNo;
                    objcustomer.CountryID = _CountryID;
                    objcustomer.RoleID = 4;
                    objcustomer.AddressID = -1;
                    objcustomer.Flagged = false;
                    objcustomer.IsActive = _IsActive;
                    objcustomer.IsContract = _IsContract;
                    objcustomer.LastModifyDate = DateTime.Now;
                    objcustomer.LastModifyUser = _User;
                    objcustomer.FullName = _FirstName + ' ' + _LastName;
                    int _CustomerID = _IUser.InsertCustomer(objcustomer); // Save and Return ID

                    if (_Address1 != "" && _Address2 != "" && _Address3 != "")
                    {
                        // ----------------------------- Save Address -----------------------------
                        objaddressbook.SegmentID = _Segment;
                        objaddressbook.EntryID = _CustomerID;
                        objaddressbook.RoleID = 4;
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
                        var result2 = db.Customers.SingleOrDefault(b => b.CustomerID == _CustomerID);
                        result2.AddressID = _AddressID;
                        db.SaveChanges();
                        //------------------------------------------------------------------------------
                    }
                    //------------------------------------------------------------------------------
                    TempData["Success"] = "Success";
                    return RedirectToAction("List", "Customer");
                }
            }
        }

        public ActionResult CreateBulk()
        {
            ViewBag.Current = "User";
            ViewBag.CurrentSub = "Customer";

            using (var db = new DatabaseContext())
            {
                return View();
            }
        }

        public FileResult DownloadExcel()
        {
            string path = "/Documents/BulkCustomer.xlsx";
            return File(path, "application/vnd.ms-excel", "BulkCustomer.xlsx");
        }

        [HttpPost]
        public ActionResult CreateBulk(CustomerTB users, HttpPostedFileBase FileUpload)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            if (FileUpload == null)
            {
                TempData["ErrorMessage"] = "Select a File to Upload ...!";
                return RedirectToAction("CreateBulk", "Customer");
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
                var artistAlbums = from item in excelFile.Worksheet<CustomerTB>(sheetName) select item;
                var password = EncryptionLibrary.EncryptText("password");


                foreach (var item in artistAlbums)
                {
                    try
                    {
                        string _Company = item.Company == null ? string.Empty : item.Company.Trim();
                        string _FirstName = item.FirstName == null ? string.Empty : item.FirstName.Trim();
                        string _MiddleName = item.MiddleName == null ? string.Empty : item.MiddleName.Trim();
                        string _LastName = item.LastName == null ? string.Empty : item.LastName.Trim();
                        string _OtherName = item.OtherName == null ? string.Empty : item.OtherName.Trim();
                        string _BusinessNo = item.BuisnessNo == null ? string.Empty : item.BuisnessNo.Trim();
                        string _Email = item.Email == null ? string.Empty : item.Email.Trim();
                        string _GSTNo = item.GSTNo == null ? string.Empty : item.GSTNo.Trim();
                        string _Mobile = item.Mobile == null ? string.Empty : item.Mobile.Trim();
                        string _Phone = item.Phone == null ? string.Empty : item.Phone.Trim();
                        string _Fax = item.Fax == null ? string.Empty : item.Fax.Trim();
                        string _Address1 = item.Address1 == null ? string.Empty : item.Address1.Trim();
                        string _Address2 = item.Address2 == null ? string.Empty : item.Address2.Trim();
                        string _Address3 = item.Address3 == null ? string.Empty : item.Address3.Trim();
                        string _PostalNo = item.PostalNo == null ? string.Empty : item.PostalNo.Trim();
                        int _CountryID = item.CountryID == null ? -1 : int.Parse(item.CountryID.ToString());
                        int _GenderID = item.GenderID == null ? -1 : int.Parse(item.GenderID.ToString());
                        string _UserName = item.UserName == null ? string.Empty : item.UserName.Trim();
                        string _DrivingNo = item.DrivingLicenceNo == null ? string.Empty : item.DrivingLicenceNo.Trim();

                        using (var db = new DatabaseContext())
                        {

                            objcustomer.SegmentID = _Segment;
                            objcustomer.Company = _Company;
                            objcustomer.FirstName = _FirstName;
                            objcustomer.LastName = _LastName;
                            objcustomer.MiddleName = _MiddleName;
                            objcustomer.OtherName = _OtherName;
                            objcustomer.GenderID = _GenderID;
                            objcustomer.UserName = _UserName;
                            objcustomer.Email = _Email;
                            objcustomer.Mobile = _Mobile;
                            objcustomer.Fax = _Fax;
                            objcustomer.Phone = _Phone;
                            objcustomer.GSTNo = _GSTNo;
                            objcustomer.BuisnessNo = _BusinessNo;
                            objcustomer.Password = password;
                            objcustomer.ConfirmPassword = "";
                            objcustomer.Address1 = _Address1;
                            objcustomer.Address2 = _Address2;
                            objcustomer.Address3 = _Address3;
                            objcustomer.DrivingLicenceNo = _DrivingNo;
                            objcustomer.PostalNo = _PostalNo;
                            objcustomer.CountryID = _CountryID;
                            objcustomer.RoleID = 4;
                            objcustomer.AddressID = -1;
                            objcustomer.Flagged = false;
                            objcustomer.IsActive = true;
                            objcustomer.IsContract = false;
                            objcustomer.LastModifyDate = DateTime.Now;
                            objcustomer.LastModifyUser = _User;

                            db.Customers.Add(objcustomer);
                            db.SaveChanges();

                            int returnID = objcustomer.CustomerID;

                            if (objcustomer.Address1 != "" && objcustomer.Address2 != "" && objcustomer.Address3 != "")
                            {
                                // ----------------------------- Save Address -----------------------------
                                var address = new AddressBookTB
                                {
                                    SegmentID = _Segment,
                                    EntryID = returnID,
                                    RoleID = 4,
                                    Address1 = objcustomer.Address1,
                                    Address2 = objcustomer.Address2,
                                    Address3 = objcustomer.Address3,
                                    PostalNo = "",
                                    CountryID = -1,
                                    IsActive = true,
                                    LastModifyDate = DateTime.Now,
                                    LastModifyUser = _User
                                };
                                db.AddressBook.Add(address);
                                db.SaveChanges();

                                // ----------------------------- Update Address ID -----------------------------
                                int addressID = address.AddressID;
                                var result2 = db.Customers.SingleOrDefault(b => b.CustomerID == returnID);
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
                return RedirectToAction("List", "Customer");
            }
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            ViewBag.Current = "User";
            ViewBag.CurrentSub = "Customer";

            using (var db = new DatabaseContext())
            {
                var result = db.Customers.SingleOrDefault(b => b.CustomerID == id);

                if (result == null)
                {
                    return View(List());
                }
                else
                {
                    var gender = (from data in db.GenderMasters orderby data.Gender ascending select data).ToList();
                    ViewBag.GenderList = new SelectList(gender, "GenderID", "Gender");

                    var country = (from data in db.CountryMasters orderby data.CountryName ascending select data).ToList();
                    ViewBag.CountryList = new SelectList(country, "CountryID", "CountryName");

                    return View(db.Customers.Where(x => x.CustomerID == id).FirstOrDefault<CustomerTB>());
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CustomerTB user, FormCollection collection)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var result = (from item in db.Customers
                              where item.Company == user.Company && item.CustomerID != user.CustomerID && item.SegmentID == _Segment
                              select item).Count();

                if (result > 0)
                {
                    TempData["ErrorMessage"] = "Company Exists ...!";
                    return RedirectToAction("Edit", "Customer");
                }
                else
                {
                    string contract = collection["iscontract"];
                    bool _IsContract = Convert.ToBoolean(contract);

                    string active = collection["isactive"];
                    bool _IsActive = Convert.ToBoolean(active);

                    string _Company = user.Company == null ? string.Empty : user.Company.Trim();
                    string _FirstName = user.FirstName == null ? string.Empty : user.FirstName.Trim();
                    string _MiddleName = user.MiddleName == null ? string.Empty : user.MiddleName.Trim();
                    string _LastName = user.LastName == null ? string.Empty : user.LastName.Trim();
                    string _OtherName = user.OtherName == null ? string.Empty : user.OtherName.Trim();
                    string _BusinessNo = user.BuisnessNo == null ? string.Empty : user.BuisnessNo.Trim();
                    string _Email = user.Email == null ? string.Empty : user.Email.Trim();
                    string _GSTNo = user.GSTNo == null ? string.Empty : user.GSTNo.Trim();
                    string _Mobile = user.Mobile == null ? string.Empty : user.Mobile.Trim();
                    string _Phone = user.Phone == null ? string.Empty : user.Phone.Trim();
                    string _Fax = user.Fax == null ? string.Empty : user.Fax.Trim();
                    string _Address1 = user.Address1 == null ? string.Empty : user.Address1.Trim();
                    string _Address2 = user.Address2 == null ? string.Empty : user.Address2.Trim();
                    string _Address3 = user.Address3 == null ? string.Empty : user.Address3.Trim();
                    string _PostalNo = user.PostalNo == null ? string.Empty : user.PostalNo.Trim();
                    int _CountryID = user.CountryID == null ? -1 : int.Parse(user.CountryID.ToString());
                    int _GenderID = user.GenderID == null ? -1 : int.Parse(user.GenderID.ToString());
                    string _UserName = user.UserName == null ? string.Empty : user.UserName.Trim();
                    string _DrivingNo = user.DrivingLicenceNo == null ? string.Empty : user.DrivingLicenceNo.Trim();

                    var result2 = db.Customers.SingleOrDefault(b => b.CustomerID == user.CustomerID);
                    result2.FirstName = _FirstName;
                    result2.LastName = _LastName;
                    result2.MiddleName = _MiddleName;
                    result2.OtherName = _OtherName;
                    result2.DrivingLicenceNo = _DrivingNo;
                    result2.GenderID = _GenderID;
                    result2.UserName = _UserName;
                    result2.Company = _Company;
                    result2.BuisnessNo = _BusinessNo;
                    result2.GSTNo = _GSTNo;
                    result2.Email = _Email;
                    result2.Mobile = _Mobile;
                    result2.Fax = _Fax;
                    result2.Phone = _Phone;
                    result2.Address1 = _Address1;
                    result2.Address2 = _Address2;
                    result2.Address3 = _Address3;
                    result2.PostalNo = _PostalNo;
                    result2.CountryID = _CountryID;
                    result2.IsActive = _IsActive;
                    result2.IsContract = _IsContract;
                    result2.LastModifyDate = DateTime.Now;
                    result2.LastModifyUser = _User;
                    _IUser.UpdateCustomer(result2);

                    if (_Address1 != "" && _Address2 != "" && _Address3 != "")
                    {
                        var addressedit = db.AddressBook.SingleOrDefault(b => b.EntryID == user.CustomerID && b.IsActive == true && b.RoleID == 4 && b.SegmentID == _Segment);
                        if (addressedit != null)
                        {
                            addressedit.Address1 = _Address1;
                            addressedit.Address2 = _Address2;
                            addressedit.Address3 = _Address3;
                            addressedit.PostalNo = _PostalNo;
                            addressedit.CountryID = _CountryID;
                            addressedit.LastModifyDate = DateTime.Now;
                            addressedit.LastModifyUser = Convert.ToInt32(HttpContext.Session["AdminUser"]);
                            _IUser.UpdateAddress(addressedit);
                        }
                        else
                        {
                            var address = new AddressBookTB
                            {
                                SegmentID = Convert.ToInt32(HttpContext.Session["Segment"]),
                                EntryID = user.CustomerID,
                                RoleID = 4,
                                Address1 = _Address1,
                                Address2 = _Address2,
                                Address3 = _Address3,
                                PostalNo = _PostalNo,
                                CountryID = _CountryID,
                                IsActive = true,
                                LastModifyDate = DateTime.Now,
                                LastModifyUser = Convert.ToInt32(HttpContext.Session["AdminUser"])
                            };
                            db.AddressBook.Add(address);
                            db.SaveChanges();

                            // ----------------------------- Update Address ID -----------------------------
                            int addressID = address.AddressID;
                            var employee = db.Customers.SingleOrDefault(b => b.CustomerID == user.CustomerID);
                            result2.AddressID = addressID;
                            db.SaveChanges();
                            //------------------------------------------------------------------------------
                        }
                    }
                    TempData["Updated"] = "Updated";
                    return RedirectToAction("List", "Customer");
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

                var data = _IUser.DeleteCustomer(Convert.ToInt32(id), _Segment);

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
            ViewBag.CurrentSub = "Customer";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var result = db.vw_EmployeeMasters.SingleOrDefault(b => b.EmployeeNo == id);

                if (result == null)
                {
                    return View(List());
                }
                else
                {
                    TempData["username"] = result.UserName;
                    TempData["fullname"] = result.FullName;
                    return View(db.vw_EmployeeMasters.Where(x => x.EmployeeNo == id).FirstOrDefault<vw_EmployeeMaster>());
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
                    return View(db.vw_EmployeeMasters.Where(x => x.EmployeeNo == id).FirstOrDefault<vw_EmployeeMaster>());
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
                    return View(db.vw_EmployeeMasters.Where(x => x.EmployeeNo == id).FirstOrDefault<vw_EmployeeMaster>());
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