using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UILAB.Concrete;
using UILAB.Filters;
using UILAB.Interface;
using UILAB.Models;

namespace UILAB.Controllers.Settings
{
    public class SettingApprovalController : Controller
    {
        ApprovalGroupTR objapprovalgroup = new ApprovalGroupTR();
        ApprovalSchemeMaster objapprovalscheme = new ApprovalSchemeMaster();
        IApproval _IApproval;

        public SettingApprovalController()
        {
            _IApproval = new ApprovalConcrete();
        }

        [HttpGet]
        public ActionResult List()
        {
            ViewBag.Current = "Setting";
            ViewBag.CurrentSub = "Approval";

            return View();
        }

        [HttpGet]
        public ActionResult Approvals()
        {
            ViewBag.Current = "Setting";
            ViewBag.CurrentSub = "Approval";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.ApprovalSettings where data.SegmentID == _Segment select data).ToList();
                return View(list);
            }
        }

        #region --> Approval Scheme

        [HttpGet]
        public ActionResult SchemeInfo()
        {
            ViewBag.Current = "Setting";
            ViewBag.CurrentSub = "Approval";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.ApprovalSchemeMasters where data.SegmentID == _Segment select data).ToList();
                return View(list);
            }
        }

        [HttpGet]
        public ActionResult CreateScheme(int id = 0)
        {
            ViewBag.Current = "Setting";
            ViewBag.CurrentSub = "Approval";

            if (id == 0)
            {
                ViewBag.SubmitValue = "Save";
                ViewBag.SubmitHeader = "Create Scheme";
                return View();
            }
            else
            {
                using (var db = new DatabaseContext())
                {
                    ViewBag.SubmitValue = "Update";
                    ViewBag.SubmitHeader = "Update Scheme";
                    TempData["buttonMsg"] = "ClearButton";
                    return View(db.ApprovalSchemeMasters.Where(x => x.ApprovalSchemeID == id).FirstOrDefault<ApprovalSchemeMaster>());
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateScheme(ApprovalSchemeMaster item)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var itemdata = db.ApprovalSchemeMasters.SingleOrDefault(b => b.SchemeName == item.SchemeName && b.SegmentID == _Segment && b.ApprovalSchemeID != item.ApprovalSchemeID);

                if (itemdata != null)
                {
                    TempData["ErrorMessage"] = itemdata.SchemeName;
                    return RedirectToAction("CreateScheme", "SettingApproval");
                }
                else
                {
                    string _SchemeName = item.SchemeName == null ? string.Empty : item.SchemeName.Trim();
                    string _SchemeCode = item.Code == null ? string.Empty : item.Code.Trim();

                    if (item.ApprovalSchemeID != 0)
                    {
                        item.SegmentID = _Segment;
                        item.SchemeName = _SchemeName;
                        item.Code = _SchemeCode;
                        item.LastModifyDate = DateTime.Now;
                        item.LastModifyUser = _User;
                        _IApproval.UpdateScheme(item);
                        TempData["successMsg"] = "Approval Scheme " + _SchemeName + " Updated..!";
                    }
                    else
                    {
                        objapprovalscheme.SegmentID = _Segment;
                        objapprovalscheme.SchemeName = _SchemeName;
                        objapprovalscheme.Code = _SchemeCode;
                        objapprovalscheme.Flagged = false;
                        objapprovalscheme.LastModifyDate = DateTime.Now;
                        objapprovalscheme.LastModifyUser = _User;
                        _IApproval.InsertScheme(objapprovalscheme);
                        TempData["successMsg"] = "Approval Scheme " + _SchemeName + " Created..!";
                    }
                    TempData["Success"] = "Success";
                    return RedirectToAction("List", "SettingApproval");
                }
            }
        }

        [HttpPost]
        public JsonResult DeleteScheme(string id)
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

        #endregion

        #region --> Approval Assign

        [HttpGet]
        public ActionResult Assign()
        {
            ViewBag.Current = "Setting";
            ViewBag.CurrentSub = "Approval";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var scheme = (from data in db.ApprovalSchemeMasters orderby data.SchemeName ascending select data).ToList();
                ViewBag.SchemeList = new SelectList(scheme, "ApprovalSchemeID", "SchemeName");

                var list = (from data in db.ApprovalSettings where data.SegmentID == _Segment select data).ToList();
                return View(list);
            }
        }

        [HttpGet]
        public ActionResult UpdateAssign(int id = 0)
        {
            ViewBag.Current = "Setting";
            ViewBag.CurrentSub = "Approval";

            if (id == 0)
            {
                ViewBag.SubmitValue = "Save";
                ViewBag.SubmitHeader = "Create Scheme";
                return View();
            }
            else
            {
                using (var db = new DatabaseContext())
                {
                    ViewBag.SubmitValue = "Update";
                    ViewBag.SubmitHeader = "Update Scheme";
                    TempData["buttonMsg"] = "ClearButton";
                    return View(db.ApprovalSchemeMasters.Where(x => x.ApprovalSchemeID == id).FirstOrDefault<ApprovalSchemeMaster>());
                }
            }
        }

        #endregion

        #region --> Approval Groups

        [HttpGet]
        public ActionResult Groups()
        {
            ViewBag.Current = "Setting";
            ViewBag.CurrentSub = "Approval";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                ViewBag.ActiveTab = "Group";
                var list = (from data in db.vw_ApprovalGroupTRs where data.SegmentID == _Segment select data).ToList();
                return View(list);
            }
        }

        [HttpGet]
        public ActionResult CreateGroup(int id = 0)
        {
            ViewBag.Current = "Setting";
            ViewBag.CurrentSub = "Approval";

            if (id == 0)
            {
                using (var db = new DatabaseContext())
                {
                    var scheme = (from data in db.ApprovalSchemeMasters orderby data.SchemeName ascending select data).ToList();
                    ViewBag.SchemeList = new SelectList(scheme, "ApprovalSchemeID", "SchemeName");

                    ViewBag.SubmitValue = "Save";
                    ViewBag.SubmitHeader = "Create Group";
                    return View();
                }
            }
            else
            {
                using (var db = new DatabaseContext())
                {
                    var scheme = (from data in db.ApprovalSchemeMasters orderby data.SchemeName ascending select data).ToList();
                    ViewBag.SchemeList = new SelectList(scheme, "ApprovalSchemeID", "SchemeName");

                    ViewBag.SubmitValue = "Update";
                    ViewBag.SubmitHeader = "Update Group";
                    TempData["buttonMsg"] = "ClearButton";
                    return View(db.ApprovalGroups.Where(x => x.ApprovalGroupID == id).FirstOrDefault<ApprovalGroupTR>());
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateGroup(ApprovalGroupTR item, FormCollection collection)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var itemdata = db.ApprovalGroups.SingleOrDefault(b => b.GroupName == item.GroupName && b.SegmentID == _Segment && b.ApprovalSchemeID == item.ApprovalSchemeID);

                if (itemdata != null)
                {
                    TempData["ErrorMessage"] = itemdata.GroupName;
                    return RedirectToAction("CreateGroup", "SettingApproval");
                }
                else
                {
                    string _GroupName = item.GroupName == null ? string.Empty : item.GroupName.Trim();
                    int _SchemeID = item.ApprovalSchemeID == null ? -1 : int.Parse(item.ApprovalSchemeID.ToString());


                    #region --> Add Selected Employee

                    if (!string.IsNullOrEmpty(collection["ID"]))
                    {
                        string[] ids = collection["ID"].Split(new char[] { ',' });
                        string[] number = collection["NumberID"].Split(new char[] { ',' });
                        string[] final = collection["IDFinal"].Split(new char[] { ',' });

                        if (ids != null)
                        {
                            foreach (string id in ids)
                            {
                                objapprovalgroup.SegmentID = _Segment;
                                objapprovalgroup.GroupName = _GroupName;
                                objapprovalgroup.ApprovalSchemeID = _SchemeID;
                                objapprovalgroup.UserID = int.Parse(id);

                                var designation = db.EmployeeMasters.SingleOrDefault(b => b.EmployeeNo == objapprovalgroup.UserID);

                                if (designation == null)
                                {
                                    objapprovalgroup.DesignationID = -1;
                                }
                                else
                                {
                                    objapprovalgroup.DesignationID = designation.DesignationID;
                                }

                                //string _NumberOrder = collection["textorder"] == null ? string.Empty : collection["textorder"].Trim();
                                //objapprovalgroup.LevelID = int.Parse(_NumberOrder);

                                //string finalapprover = collection["checkBoxFinal"];
                                //bool _FinalApprover = Convert.ToBoolean(finalapprover);

                                //objapprovalgroup.FinalApprover = _FinalApprover;
                                objapprovalgroup.Flagged = false;
                                objapprovalgroup.IsActive = true;
                                objapprovalgroup.LastModifyDate = DateTime.Now;
                                objapprovalgroup.LastModifyUser = _User;
                                _IApproval.InsertGroup(objapprovalgroup);
                                TempData["successMsg"] = "Approval Group " + _GroupName + " Created..!";

                            }
                        }
                    }

                    #endregion

                    //string _SchemeName = item.GroupName == null ? string.Empty : item.GroupName.Trim();
                    //string _SchemeCode = item.Code == null ? string.Empty : item.Code.Trim();

                    //if (item.ApprovalSchemeID != 0)
                    //{
                    //    item.SegmentID = _Segment;
                    //    item.SchemeName = _SchemeName;
                    //    item.Code = _SchemeCode;
                    //    item.LastModifyDate = DateTime.Now;
                    //    item.LastModifyUser = _User;
                    //    _IApproval.UpdateScheme(item);
                    //    TempData["successMsg"] = "Approval Scheme " + _SchemeName + " Updated..!";
                    //}
                    //else
                    //{
                    //    objapprovalscheme.SegmentID = _Segment;
                    //    objapprovalscheme.SchemeName = _SchemeName;
                    //    objapprovalscheme.Code = _SchemeCode;
                    //    objapprovalscheme.Flagged = false;
                    //    objapprovalscheme.LastModifyDate = DateTime.Now;
                    //    objapprovalscheme.LastModifyUser = _User;
                    //    _IApproval.InsertScheme(objapprovalscheme);
                    //    TempData["successMsg"] = "Approval Scheme " + _SchemeName + " Created..!";
                    //}
                    TempData["Success"] = "Success";
                    ViewBag.ActiveTab = "Group";
                    return RedirectToAction("List", "SettingApproval", ViewBag.ActiveTab = "Group");
                }
            }
        }

        [HttpPost]
        public JsonResult DeleteGroup(string id)
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


        #endregion
    }
}