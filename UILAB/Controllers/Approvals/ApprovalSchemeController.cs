using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UILAB.Concrete;
using UILAB.Filters;
using UILAB.Interface;
using UILAB.Models;

namespace UILAB.Controllers.Approvals
{
    [ValidateAdminSession]
    public class ApprovalSchemeController : Controller
    {
        ApprovalSchemeMaster objapprovalscheme = new ApprovalSchemeMaster();

        IApproval _IApproval;
        public ApprovalSchemeController()
        {
            _IApproval = new ApprovalConcrete();
        }

        [HttpGet]
        public ActionResult List()
        {
            ViewBag.Current = "Approval";
            ViewBag.CurrentSub = "ApprovalScheme";
            TempData["Error"] = "";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.ApprovalSchemeMasters where data.SegmentID == _Segment orderby data.SchemeName ascending select data).ToList();
                return View(list);
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Current = "Approval";
            ViewBag.CurrentSub = "ApprovalScheme";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ApprovalSchemeMaster item)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var itemdata = db.ApprovalSchemeMasters.SingleOrDefault(b => b.SchemeName == item.SchemeName && b.SegmentID == _Segment);

                if (itemdata != null)
                {
                    TempData["ErrorMessage"] = itemdata.SchemeName;
                    return RedirectToAction("Create", "ApprovalScheme");
                }
                else
                {
                    string _SchemeName = item.SchemeName == null ? string.Empty : item.SchemeName.Trim();
                    string _SchemeCode = item.Code == null ? string.Empty : item.Code.Trim();

                    objapprovalscheme.SegmentID = _Segment;
                    objapprovalscheme.SchemeName = _SchemeName;
                    objapprovalscheme.Code = _SchemeCode;
                    objapprovalscheme.Flagged = false;
                    objapprovalscheme.LastModifyDate = DateTime.Now;
                    objapprovalscheme.LastModifyUser = _User;
                    _IApproval.InsertScheme(objapprovalscheme);

                    TempData["Success"] = "Success";
                    return RedirectToAction("List", "ApprovalScheme");
                }
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            ViewBag.Current = "Approval";
            ViewBag.CurrentSub = "ApprovalScheme";

            using (var db = new DatabaseContext())
            {
                var result = db.ApprovalSchemeMasters.SingleOrDefault(b => b.ApprovalSchemeID == id);

                if (result == null)
                {
                    return RedirectToAction("List", "ApprovalScheme");
                }
                else
                {
                    return View(db.ApprovalSchemeMasters.Where(x => x.ApprovalSchemeID == id).FirstOrDefault<ApprovalSchemeMaster>());
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ApprovalSchemeMaster item)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var result = db.ApprovalSchemeMasters.SingleOrDefault(b => b.SchemeName == item.SchemeName && b.SegmentID == _Segment);

                if (result != null)
                {
                    TempData["ErrorMessage"] = result.SchemeName;
                    return RedirectToAction("Edit", "ApprovalScheme");
                }
                else
                {
                    string _SchemeName = item.SchemeName == null ? string.Empty : item.SchemeName.Trim();
                    string _SchemeCode = item.Code == null ? string.Empty : item.Code.Trim();

                    var updatedata = db.ApprovalSchemeMasters.SingleOrDefault(b => b.ApprovalSchemeID == item.ApprovalSchemeID);

                    if (updatedata == null)
                    {
                        TempData["Error"] = "Scheme Update Failed...! Please Try Again";
                        return View(List());
                    }
                    else
                    {
                        updatedata.SchemeName = _SchemeName;
                        updatedata.Code = _SchemeCode;
                        updatedata.LastModifyDate = DateTime.Now;
                        updatedata.LastModifyUser = _User;
                        _IApproval.UpdateScheme(updatedata);

                        TempData["Updated"] = "Updated";
                        return RedirectToAction("List", "ApprovalScheme");
                    }
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

                var data = _IApproval.DeleteScheme(Convert.ToInt32(id));

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