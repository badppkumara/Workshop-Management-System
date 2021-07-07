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

namespace UILAB.Controllers.Crm
{
    [ValidateAdminSession]
    public class SupplierController : Controller
    {
        SupplierTB objsupplier = new SupplierTB();
        AddressBookTB objaddressbook = new AddressBookTB();
        IUser _IUser;

        public SupplierController()
        {
            _IUser = new UserConcrete();
        }

        [HttpGet]
        public ActionResult List()
        {
            ViewBag.Current = "User";
            ViewBag.CurrentSub = "Supplier";
            TempData["Error"] = "";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.Suppliers where data.SegmentID == _Segment orderby data.SupplierID descending select data).ToList();
                return View(list);
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Current = "User";
            ViewBag.CurrentSub = "Supplier";
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
        public ActionResult Create(SupplierTB item, FormCollection collection)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var result = db.Suppliers.SingleOrDefault(b => b.UserName == item.UserName && b.SegmentID == _Segment);

                if (result != null)
                {
                    TempData["ErrorMessage"] = "Username  Exists ...!";
                    return RedirectToAction("Create", "Supplier");
                }
                else
                {
                    var password = EncryptionLibrary.EncryptText("password");

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

                    objsupplier.SegmentID = _Segment;
                    objsupplier.FirstName = _FirstName;
                    objsupplier.LastName = _LastName;
                    objsupplier.MiddleName = _MiddleName;
                    objsupplier.OtherName = _OtherName;
                    objsupplier.GenderID = item.GenderID;
                    objsupplier.Company = _Company;
                    objsupplier.BuisnessNo = _BusinessNo;
                    objsupplier.GSTNo = _GSTNo;
                    objsupplier.UserName = item.UserName;
                    objsupplier.Email = item.Email.ToString();
                    objsupplier.Mobile = item.Mobile.ToString();
                    objsupplier.Fax = _Fax;
                    objsupplier.Phone = _Phone;
                    objsupplier.Password = password;
                    objsupplier.ConfirmPassword = "";
                    objsupplier.Address1 = _Address1;
                    objsupplier.Address2 = _Address2;
                    objsupplier.Address3 = _Address3;
                    objsupplier.PostalNo = _PostalNo;
                    objsupplier.CountryID = _CountryID;
                    objsupplier.RoleID = 5;
                    objsupplier.AddressID = -1;
                    objsupplier.Flagged = false;
                    objsupplier.IsActive = _IsActive;
                    objsupplier.LastModifyDate = DateTime.Now;
                    objsupplier.LastModifyUser = _User;
                    int _SupplierID = _IUser.InsertSupplier(objsupplier); // Save and Return ID

                    if (_Address1 != "" && _Address2 != "" && _Address3 != "")
                    {
                        // ----------------------------- Save Address -----------------------------
                        objaddressbook.SegmentID = _Segment;
                        objaddressbook.EntryID = _SupplierID;
                        objaddressbook.RoleID = 5;
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
                        var result2 = db.Suppliers.SingleOrDefault(b => b.SupplierID == _SupplierID);
                        result2.AddressID = _AddressID;
                        db.SaveChanges();
                        //------------------------------------------------------------------------------
                    }
                    //------------------------------------------------------------------------------
                    TempData["Success"] = "Success";
                    return RedirectToAction("List", "Supplier");
                }
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            ViewBag.Current = "User";
            ViewBag.CurrentSub = "Supplier";

            using (var db = new DatabaseContext())
            {
                var result = db.Suppliers.SingleOrDefault(b => b.SupplierID == id);

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

                    return View(db.Suppliers.Where(x => x.SupplierID == id).FirstOrDefault<SupplierTB>());
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SupplierTB user, FormCollection collection)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var result = (from item in db.Suppliers
                              where item.Company == user.Company && item.SupplierID != user.SupplierID && item.SegmentID == _Segment
                              select item).Count();

                if (result > 0)
                {
                    TempData["ErrorMessage"] = "Company Exists ...!";
                    return RedirectToAction("Edit", "Supplier");
                }
                else
                {
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

                    var result2 = db.Suppliers.SingleOrDefault(b => b.SupplierID == user.SupplierID);
                    result2.FirstName = _FirstName;
                    result2.LastName = _LastName;
                    result2.MiddleName = _MiddleName;
                    result2.OtherName = _OtherName;
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
                    result2.LastModifyDate = DateTime.Now;
                    result2.LastModifyUser = _User;
                    _IUser.UpdateSupplier(result2);

                    if (_Address1 != "" && _Address2 != "" && _Address3 != "")
                    {
                        var addressedit = db.AddressBook.SingleOrDefault(b => b.EntryID == user.SupplierID && b.IsActive == true && b.RoleID == 5 && b.SegmentID == _Segment);
                        if (addressedit != null)
                        {
                            addressedit.Address1 = _Address1;
                            addressedit.Address2 = _Address2;
                            addressedit.Address3 = _Address3;
                            addressedit.PostalNo = _PostalNo;
                            addressedit.CountryID = _CountryID;
                            addressedit.LastModifyDate = DateTime.Now;
                            addressedit.LastModifyUser = _User;
                            _IUser.UpdateAddress(addressedit);
                        }
                        else
                        {
                            objaddressbook.SegmentID = _Segment;
                            objaddressbook.EntryID = result2.SupplierID;
                            objaddressbook.RoleID = 5;
                            objaddressbook.Address1 = _Address1;
                            objaddressbook.Address2 = _Address2;
                            objaddressbook.Address3 = _Address3;
                            objaddressbook.PostalNo = _PostalNo;
                            objaddressbook.CountryID = _CountryID;
                            objaddressbook.IsActive = true;
                            objaddressbook.LastModifyDate = DateTime.Now;
                            objaddressbook.LastModifyUser = _User;
                            int _AddressID = _IUser.InsertAddress(objaddressbook);

                            // ----------------------------- Update Address ID -----------------------------
                            int addressID = _AddressID;
                            var employee = db.Suppliers.SingleOrDefault(b => b.SupplierID == user.SupplierID);
                            result2.AddressID = addressID;
                            db.SaveChanges();
                            //------------------------------------------------------------------------------
                        }
                    }
                    TempData["Updated"] = "Updated";
                    return RedirectToAction("List", "Supplier");
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

                var data = _IUser.DeleteSupplier(Convert.ToInt32(id), _Segment);

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
            ViewBag.CurrentSub = "Supplier";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var result = db.Suppliers.SingleOrDefault(b => b.SupplierID == id);

                if (result == null)
                {
                    return View(List());
                }
                else
                {
                    TempData["username"] = result.UserName;
                    TempData["company"] = result.Company;

                    return View(db.Suppliers.Where(x => x.SupplierID == id).FirstOrDefault<SupplierTB>());
                }
            }
        }

        [HttpGet]
        public ActionResult AdminInfo(int id)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var result = db.Suppliers.SingleOrDefault(b => b.SupplierID == id);

                if (result == null)
                {
                    return View(List());
                }
                else
                {
                    TempData["username"] = result.UserName;
                    TempData["company"] = result.Company;

                    return View(db.Suppliers.Where(x => x.SupplierID == id).FirstOrDefault<SupplierTB>());
                }
            }
        }

        [HttpGet]
        public ActionResult Security(int id)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var result = db.Suppliers.SingleOrDefault(b => b.SupplierID == id);

                if (result == null)
                {
                    return View(List());
                }
                else
                {
                    TempData["username"] = result.UserName;
                    TempData["company"] = result.Company;

                    return View(db.Suppliers.Where(x => x.SupplierID == id).FirstOrDefault<SupplierTB>());
                }
            }
        }

        [HttpGet]
        public ActionResult Logs(int id)
        {
            ViewBag.Current = "User";
            ViewBag.CurrentSub = "Supplier";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.SecurityActiveUser where data.SegmentID == _Segment && data.UserID == id && data.UserType == 5 orderby data.LoginInstance ascending select data).ToList();
                return View(list);
            }
        }
    }
}