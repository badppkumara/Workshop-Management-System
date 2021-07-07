using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UILAB.Concrete;
using UILAB.Filters;
using UILAB.Interface;
using UILAB.Models;

namespace UILAB.Controllers.Stock
{
    [ValidateAdminSession]
    public class JobTypeController : Controller
    {
        JobTypeTB objtype = new JobTypeTB();
        IJob _IJob;
        IApproval _Apprroval;

        public JobTypeController()
        {
            _IJob = new JobConcrete();
            _Apprroval = new ApprovalConcrete();
        }

        [HttpGet]
        public ActionResult List()
        {
            ViewBag.Current = "Jobs";
            ViewBag.CurrentSub = "Type";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.JobTypes where data.SegmentID == _Segment select data).ToList();
                return View(list);
            }
        }

        [HttpGet]
        public ActionResult Create(int id = 0)
        {
            ViewBag.Current = "Jobs";
            ViewBag.CurrentSub = "Type";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            TempData["Segment"] = _Segment;

            if (id == 0)
            {
                ViewBag.SubmitValue = "Save";
                ViewBag.SubmitHeader = "Create Category";

                using (var db = new DatabaseContext())
                {
                    var scheme = (from data in db.ApprovalSettings where data.SegmentID == _Segment & data.Code == "JOB" select data).ToList();
                    ViewBag.List = new SelectList(scheme, "ApprovalSettingID", "ApprovalItem");

                    return View();
                }
            }
            else
            {
                using (var db = new DatabaseContext())
                {
                    ViewBag.SubmitValue = "Update";
                    ViewBag.SubmitHeader = "Update Category";
                    TempData["buttonMsg"] = "ClearButton";

                    var scheme = (from data in db.ApprovalSettings where data.SegmentID == _Segment & data.Code == "JOB" select data).ToList();
                    ViewBag.List = new SelectList(scheme, "ApprovalSettingID", "ApprovalItem");

                    return View(db.JobTypes.Where(x => x.JobTypeID == id).FirstOrDefault<JobTypeTB>());
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(JobTypeTB item)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var itemdata = db.JobTypes.SingleOrDefault(b => b.JobTypeName == item.JobTypeName && b.SegmentID == _Segment && b.JobTypeID != item.JobTypeID);

                if (itemdata != null)
                {
                    TempData["ErrorMessage"] = itemdata.JobTypeName;
                    return RedirectToAction("List", "JobType");
                }
                else
                {
                    string _TypeName = item.JobTypeName == null ? string.Empty : item.JobTypeName.Trim();
                    int _SchemeID = item.ApprovalSettingID == null ? -1 : int.Parse(item.ApprovalSettingID.ToString());

                    var approvalscheme = db.ApprovalSettings.SingleOrDefault(b => b.ApprovalSettingID == _SchemeID);

                    if (item.JobTypeID != 0)
                    {
                        item.SegmentID = _Segment;
                        item.JobTypeName = _TypeName;
                        item.ApprovalSettingID = _SchemeID;
                        item.ApprovalSchemeID = approvalscheme.ApprovalSchemeID;
                        item.LastModifyDate = DateTime.Now;
                        item.LastModifyUser = _User;
                        _IJob.UpdateType(item);

                        TempData["successMsg"] = "Category " + _TypeName + " Updated";
                    }
                    else
                    {
                        objtype.SegmentID = _Segment;
                        objtype.JobTypeName = _TypeName;
                        objtype.ApprovalSettingID = _SchemeID;
                        objtype.Flagged = false;
                        objtype.ApprovalSchemeID = approvalscheme.ApprovalSchemeID;
                        objtype.LastModifyDate = DateTime.Now;
                        objtype.LastModifyUser = _User;
                        _IJob.InsertType(objtype);

                        var result = db.ApprovalSchemeMasters.SingleOrDefault(b => b.ApprovalSchemeID == _SchemeID);
                        if (result != null)
                        {
                            result.Flagged = true;
                            result.LastModifyDate = DateTime.Now;
                            result.LastModifyUser = _User;
                            _Apprroval.UpdateScheme(result);
                        }
                        TempData["successMsg"] = "Category " + _TypeName + " Created";
                    }

                    TempData["Success"] = "Success";
                    return RedirectToAction("List", "JobType");
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

                var data = _IJob.DeleteType(Convert.ToInt32(id));

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