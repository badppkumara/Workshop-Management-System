using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UILAB.Concrete;
using UILAB.Filters;
using UILAB.Interface;
using UILAB.Models;

namespace UILAB.Controllers.Masters
{
    [ValidateAdminSession]
    public class OrganizationSchemeController : Controller
    {
        OrganizationSchemeTR objorganization = new OrganizationSchemeTR();
        OrganizationSchemeInfo objorganizationinfo = new OrganizationSchemeInfo();

        IMaster _IMaster;
        public OrganizationSchemeController()
        {
            _IMaster = new MasterConcrete();
        }

        [HttpGet]
        public ActionResult List()
        {
            ViewBag.Current = "Setting";
            ViewBag.CurrentSub = "Segment";
            TempData["Error"] = "";

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.OrganizationSchemeTRs orderby data.SegmentID ascending select data).ToList();
                return View(list);
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Current = "Setting";
            ViewBag.CurrentSub = "Segment";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var type = (from data in db.SegmentTypeMasters orderby data.SegmentTypeName ascending select data).ToList();
                ViewBag.TypeList = new SelectList(type, "SegmentTypeID", "SegmentTypeName");

                var parent = (from data in db.OrganizationSchemeTRs orderby data.SegmentName ascending select data).ToList();
                ViewBag.ParentList = new SelectList(type, "SegmentID", "SegmentName");

                var country = (from data in db.CountryMasters orderby data.CountryName ascending select data).ToList();
                ViewBag.CountryList = new SelectList(country, "CountryID", "CountryName");

                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OrganizationSchemeTR item, FormCollection collection)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["SuperAdmin"]);

            using (var db = new DatabaseContext())
            {
                var result = db.OrganizationSchemeTRs.SingleOrDefault(b => b.SegmentName == item.SegmentName);

                if (result != null)
                {
                    TempData["ErrorMessage"] = "Segment Name  Exists ...!";
                    return RedirectToAction("Create", "OrganizationScheme");
                }
                else
                {
                    string active = collection["isactive"];
                    bool _IsActive = Convert.ToBoolean(active);

                    string _SegmentName = item.SegmentName == null ? string.Empty : item.SegmentName.Trim();
                    int _SegmentType = item.SegmentTypeID == null ? -1 : int.Parse(item.SegmentTypeID.ToString());
                    int _ParentSegment = item.ParentSegmentID == null ? -1 : int.Parse(item.ParentSegmentID.ToString());
                    string _BusinessNo = collection["business"] == null ? string.Empty : collection["business"].Trim();
                    string _GSTNo = collection["gst"] == null ? string.Empty : collection["gst"].Trim();
                    string _Email1 = collection["email1"] == null ? string.Empty : collection["email1"].Trim();
                    string _Email2 = collection["email2"] == null ? string.Empty : collection["email2"].Trim();
                    string _Email3 = collection["email3"] == null ? string.Empty : collection["email3"].Trim();
                    string _Phone = collection["phone"] == null ? string.Empty : collection["phone"].Trim();
                    string _Fax = collection["fax"] == null ? string.Empty : collection["fax"].Trim();
                    string _Mobile = collection["mobile"] == null ? string.Empty : collection["mobile"].Trim();
                    string _Bank1 = collection["bank1"] == null ? string.Empty : collection["bank1"].Trim();
                    string _Bank2 = collection["bank2"] == null ? string.Empty : collection["bank2"].Trim();
                    string _Bank3 = collection["bank3"] == null ? string.Empty : collection["bank3"].Trim();
                    string _Address1 = collection["address1"] == null ? string.Empty : collection["address1"].Trim();
                    string _Address2 = collection["address2"] == null ? string.Empty : collection["address2"].Trim();
                    string _Address3 = collection["address3"] == null ? string.Empty : collection["address3"].Trim();
                    string _PostalNo = collection["postal"] == null ? string.Empty : collection["postal"].Trim();
                    int _CountryID = collection["country"] == null ? -1 : int.Parse(collection["country"].Trim());

                    objorganization.ParentSegmentID = _ParentSegment;
                    objorganization.SegmentTypeID = _SegmentType;
                    objorganization.SegmentName = _SegmentName;
                    objorganization.IsActive = _IsActive;
                    objorganization.LastModifyDate = DateTime.Now;
                    objorganization.LastModifyUser = _User;
                    int _SegmentID = _IMaster.InsertSegment(objorganization); // Save and Return ID

                    objorganizationinfo.SegmentID = _SegmentID;
                    objorganizationinfo.SegmentName = _SegmentName;
                    objorganizationinfo.BuisnessNo = _BusinessNo;
                    objorganizationinfo.GSTNo = _GSTNo;
                    objorganizationinfo.Email1 = _Email1;
                    objorganizationinfo.Email2 = _Email2;
                    objorganizationinfo.Email3 = _Email3;
                    objorganizationinfo.Phone = _Phone;
                    objorganizationinfo.Fax = _Fax;
                    objorganizationinfo.Mobile = _Mobile;
                    objorganizationinfo.BankAccount1 = _Bank1;
                    objorganizationinfo.BankAccount2 = _Bank2;
                    objorganizationinfo.BankAccount3 = _Bank3;
                    objorganizationinfo.Address1 = _Address1;
                    objorganizationinfo.Address2 = _Address2;
                    objorganizationinfo.Address3 = _Address3;
                    objorganizationinfo.PostalNo = _PostalNo;
                    objorganizationinfo.CountryID = _CountryID;
                    objorganizationinfo.LastModifyDate = DateTime.Now;
                    objorganizationinfo.LastModifyUser = _User;
                    int _InfoID = _IMaster.InsertInfo(objorganizationinfo);

                    // ----------------------------- Update Segment ID -----------------------------
                    var result2 = db.OrganizationSchemeTRs.SingleOrDefault(b => b.SegmentID == _SegmentID);
                    result2.SegmentInfoID = _InfoID;
                    db.SaveChanges();
                    //------------------------------------------------------------------------------

                    TempData["Success"] = "Success";
                    return RedirectToAction("List", "OrganizationScheme");
                }
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            ViewBag.Current = "Setting";
            ViewBag.CurrentSub = "Segment";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var result = db.OrganizationSchemeTRs.SingleOrDefault(b => b.SegmentID == id);

                if (result == null)
                {
                    return View(List());
                }
                else
                {
                    var type = (from data in db.SegmentTypeMasters orderby data.SegmentTypeName ascending select data).ToList();
                    ViewBag.TypeList = new SelectList(type, "SegmentTypeID", "SegmentTypeName");

                    var parent = (from data in db.OrganizationSchemeTRs orderby data.SegmentName ascending select data).ToList();
                    ViewBag.ParentList = new SelectList(type, "SegmentID", "SegmentName");

                    return View(db.OrganizationSchemeTRs.Where(x => x.SegmentID == id).FirstOrDefault<OrganizationSchemeTR>());
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(OrganizationSchemeTR item, FormCollection collection)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["SuperAdmin"]);

            using (var db = new DatabaseContext())
            {
                var result = (from data in db.OrganizationSchemeTRs
                              where data.SegmentName == item.SegmentName && item.SegmentID != item.SegmentID select item).Count();

                if (result > 0)
                {
                    TempData["ErrorMessage"] = "Segment Name Exists ...!";
                    return RedirectToAction("Edit", "OrganizationScheme");
                }
                else
                {
                    string active = collection["isactive"];
                    bool _IsActive = Convert.ToBoolean(active);

                    string _SegmentName = item.SegmentName == null ? string.Empty : item.SegmentName.Trim();
                    int _SegmentType = item.SegmentTypeID == null ? -1 : int.Parse(item.SegmentTypeID.ToString());
                    int _ParentSegment = item.ParentSegmentID == null ? -1 : int.Parse(item.ParentSegmentID.ToString());
                    string _BusinessNo = collection["business"] == null ? string.Empty : collection["business"].Trim();
                    string _GSTNo = collection["gst"] == null ? string.Empty : collection["gst"].Trim();
                    string _Email1 = collection["email1"] == null ? string.Empty : collection["email1"].Trim();
                    string _Email2 = collection["email2"] == null ? string.Empty : collection["email2"].Trim();
                    string _Email3 = collection["email3"] == null ? string.Empty : collection["email3"].Trim();
                    string _Phone = collection["phone"] == null ? string.Empty : collection["phone"].Trim();
                    string _Fax = collection["fax"] == null ? string.Empty : collection["fax"].Trim();
                    string _Mobile = collection["mobile"] == null ? string.Empty : collection["mobile"].Trim();
                    string _Bank1 = collection["bank1"] == null ? string.Empty : collection["bank1"].Trim();
                    string _Bank2 = collection["bank2"] == null ? string.Empty : collection["bank2"].Trim();
                    string _Bank3 = collection["bank3"] == null ? string.Empty : collection["bank3"].Trim();
                    string _Address1 = collection["address1"] == null ? string.Empty : collection["address1"].Trim();
                    string _Address2 = collection["address2"] == null ? string.Empty : collection["address2"].Trim();
                    string _Address3 = collection["address3"] == null ? string.Empty : collection["address3"].Trim();
                    string _PostalNo = collection["postal"] == null ? string.Empty : collection["postal"].Trim();
                    int _CountryID = collection["country"] == null ? -1 : int.Parse(collection["country"].Trim());


                    var updatedata = db.OrganizationSchemeTRs.SingleOrDefault(b => b.SegmentID == item.SegmentID);

                    if (updatedata == null)
                    {
                        TempData["Error"] = "Segment Update Failed...! Please Try Again";
                        return RedirectToAction("List", "OrganizationScheme");
                    }
                    else
                    {
                        updatedata.ParentSegmentID = _ParentSegment;
                        updatedata.SegmentTypeID = _SegmentType;
                        updatedata.SegmentName = _SegmentName;
                        updatedata.IsActive = _IsActive;
                        updatedata.LastModifyDate = DateTime.Now;
                        updatedata.LastModifyUser = _User;
                        _IMaster.UpdateSegment(updatedata);

                        var updateinfo = db.OrganizationSchemeInfos.SingleOrDefault(b => b.SegmentInfoID == item.SegmentInfoID);

                        if (updateinfo != null)
                        {

                            updateinfo.SegmentName = _SegmentName;
                            updateinfo.BuisnessNo = _BusinessNo;
                            updateinfo.GSTNo = _GSTNo;
                            updateinfo.Email1 = _Email1;
                            updateinfo.Email2 = _Email2;
                            updateinfo.Email3 = _Email3;
                            updateinfo.Phone = _Phone;
                            updateinfo.Fax = _Fax;
                            updateinfo.Mobile = _Mobile;
                            updateinfo.BankAccount1 = _Bank1;
                            updateinfo.BankAccount2 = _Bank2;
                            updateinfo.BankAccount3 = _Bank3;
                            updateinfo.Address1 = _Address1;
                            updateinfo.Address2 = _Address2;
                            updateinfo.Address3 = _Address3;
                            updateinfo.PostalNo = _PostalNo;
                            updateinfo.CountryID = _CountryID;
                            updateinfo.LastModifyDate = DateTime.Now;
                            updateinfo.LastModifyUser = _User;
                            _IMaster.InsertInfo(updateinfo);
                        }
                        else
                        {
                            TempData["Error"] = "Segment Information Update Failed.";
                        }
                        
                        TempData["Updated"] = "Updated";
                        return RedirectToAction("List", "OrganizationScheme");
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

                var data = _IMaster.DeleteSegment(Convert.ToInt32(id));

                if (data > 0)
                {
                    var datainfo = _IMaster.DeleteInfo(Convert.ToInt32(id));

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