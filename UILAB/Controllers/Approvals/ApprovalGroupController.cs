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
    public class ApprovalGroupController : Controller
    {
        ApprovalGroupTR objapprovalgroup = new ApprovalGroupTR();

        IApproval _IApproval;
        public ApprovalGroupController()
        {
            _IApproval = new ApprovalConcrete();
        }

        [HttpGet]
        public ActionResult List()
        {
            ViewBag.Current = "Approval";
            ViewBag.CurrentSub = "ApprovalGroup";
            TempData["Error"] = "";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.ApprovalGroups where data.SegmentID == _Segment orderby data.ApprovalGroupID ascending select data).ToList();
                return View(list);
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Current = "Approval";
            ViewBag.CurrentSub = "ApprovalGroup";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var scheme = (from data in db.ApprovalSchemeMasters where data.SegmentID == _Segment orderby data.SchemeName ascending select data).ToList();
                ViewBag.SchemeList = new SelectList(scheme, "ApprovalSchemeID", "SchemeName");

                var employee = (from data in db.vw_EmployeeMasters where data.SegmentID == _Segment orderby data.FullName ascending select data).ToList();
                ViewBag.EmployeeList = new SelectList(employee, "EmployeeNo", "FullName");

                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ApprovalGroupTR item, FormCollection collection)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var itemdata = db.ApprovalGroups.SingleOrDefault(b => b.GroupName == item.GroupName && b.SegmentID == _Segment);

                if (itemdata != null)
                {
                    TempData["ErrorMessage"] = itemdata.GroupName;
                    return RedirectToAction("Create", "ApprovalGroup");
                }
                else
                {
                    string _Name = item.GroupName == null ? string.Empty : item.GroupName.Trim();
                    int _SchemeID = item.ApprovalSchemeID == null ? -1 : int.Parse(item.ApprovalSchemeID.ToString());
                    int _EmployeeID= item.UserID == null ? -1 : int.Parse(item.UserID.ToString());
                    int _LevelID = collection["level"] == null ? -1 : int.Parse(collection["level"].ToString());

                    string active = collection["isactive"];
                    bool _IsActive = Convert.ToBoolean(active);

                    var designation = db.EmployeeMasters.SingleOrDefault(b => b.EmployeeNo == _EmployeeID);

                    if (designation != null)
                    {                        
                        objapprovalgroup.SegmentID = _Segment;
                        objapprovalgroup.GroupName = _Name;
                        objapprovalgroup.ApprovalSchemeID = _SchemeID;
                        objapprovalgroup.UserID = _EmployeeID;
                        objapprovalgroup.DesignationID = designation.DesignationID;
                        objapprovalgroup.LevelID = _LevelID;
                        objapprovalgroup.Flagged = false;
                        objapprovalgroup.FinalApprover = _IsActive;
                        objapprovalgroup.LastModifyDate = DateTime.Now;
                        objapprovalgroup.LastModifyUser = _User;
                        _IApproval.InsertGroup(objapprovalgroup);

                        TempData["Success"] = "Success";
                        return RedirectToAction("List", "ApprovalGroup");
                    }
                    else
                    {
                        TempData["Error"] = "Designation not Assigned to User...!";
                        return RedirectToAction("Create", "ApprovalGroup");
                    }   
                }
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            ViewBag.Current = "Approval";
            ViewBag.CurrentSub = "ApprovalGroup";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var result = db.ApprovalGroups.SingleOrDefault(b => b.ApprovalGroupID == id);

                if (result == null)
                {
                    return RedirectToAction("List", "ApprovalGroup");
                }
                else
                {
                    var scheme = (from data in db.ApprovalSchemeMasters where data.SegmentID == _Segment orderby data.SchemeName ascending select data).ToList();
                    ViewBag.SchemeList = new SelectList(scheme, "ApprovalSchemeID", "SchemeName");

                    var employee = (from data in db.vw_EmployeeMasters where data.SegmentID == _Segment orderby data.FullName ascending select data).ToList();
                    ViewBag.EmployeeList = new SelectList(employee, "EmployeeNo", "FullName");

                    return View(db.ApprovalGroups.Where(x => x.ApprovalGroupID == id).FirstOrDefault<ApprovalGroupTR>());
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ApprovalGroupTR item, FormCollection collection)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var result = db.ApprovalGroups.SingleOrDefault(b => b.GroupName == item.GroupName && b.SegmentID == _Segment && b.ApprovalGroupID != item.ApprovalGroupID);

                if (result != null)
                {
                    TempData["ErrorMessage"] = result.GroupName;
                    return RedirectToAction("Edit", "ApprovalGroup");
                }
                else
                {
                    string _Name = item.GroupName == null ? string.Empty : item.GroupName.Trim();
                    //int _SchemeID = item.ApprovalSchemeID == null ? -1 : int.Parse(item.ApprovalSchemeID.ToString());
                    //int _EmployeeID = item.UserID == null ? -1 : int.Parse(item.UserID.ToString());
                    //int _LevelID = collection["level"] == null ? -1 : int.Parse(collection["level"].ToString());

                    string active = collection["isactive"];
                    bool _IsActive = Convert.ToBoolean(active);

                    var updatedata = db.ApprovalGroups.SingleOrDefault(b => b.ApprovalGroupID == item.ApprovalGroupID);

                    if (updatedata == null)
                    {
                        TempData["Error"] = "Group Update Failed...! Please Try Again";
                        return View(List());
                    }
                    else
                    {
                        updatedata.GroupName = _Name;
                        updatedata.FinalApprover = _IsActive;
                        updatedata.LastModifyDate = DateTime.Now;
                        updatedata.LastModifyUser = _User;
                        _IApproval.UpdateGroup(updatedata);

                        TempData["Updated"] = "Updated";
                        return RedirectToAction("List", "ApprovalGroup");
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

                var data = _IApproval.DeleteGroup(Convert.ToInt32(id));

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