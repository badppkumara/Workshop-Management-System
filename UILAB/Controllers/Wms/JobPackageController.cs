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
    public class JobPackageController : Controller
    {
        JobPackageTB objpackage = new JobPackageTB();
        JobPackageListTB objpackagelist = new JobPackageListTB();
        JobTaskTR objjobtask = new JobTaskTR();
        JobTasksTB objjobtaskmaster = new JobTasksTB();
        IJob _IJob;

        public JobPackageController()
        {
            _IJob = new JobConcrete();
        }

        [HttpGet]
        public ActionResult List()
        {
            ViewBag.Current = "Jobs";
            ViewBag.CurrentSub = "Package";
            TempData["Error"] = "";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.JobPackages where data.SegmentID == _Segment orderby data.PackageName descending select data).ToList();
                return View(list);
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Current = "Jobs";
            ViewBag.CurrentSub = "Package";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            ViewBag.SubmitValue = "Save";
            ViewBag.SubmitHeader = "Create Package";

            using (var db = new DatabaseContext())
            {
                var types = (from data in db.JobTypes where data.SegmentID == _Segment && data.JobTypeName != "Custom" orderby data.JobTypeName ascending select data).ToList();
                ViewBag.List = new SelectList(types, "JobTypeID", "JobTypeName");

                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(JobPackageTB item, FormCollection collection)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var itemdata = db.JobPackages.SingleOrDefault(b => b.PackageName == item.PackageName && b.SegmentID == _Segment && b.TypeID == item.TypeID && b.PackageID != item.PackageID);

                if (itemdata != null)
                {
                    TempData["ErrorMessage"] = itemdata.PackageName;
                    return RedirectToAction("Create", "JobPackage");
                }
                else
                {
                    string _TypeName = item.PackageName == null ? string.Empty : item.PackageName.Trim();
                    int _TypeID = item.TypeID == null ? -1 : int.Parse(item.TypeID.ToString());

                    objpackage.SegmentID = _Segment;
                    objpackage.PackageName = _TypeName;
                    objpackage.Flagged = false;
                    objpackage.TypeID = _TypeID;
                    objpackage.LastModifyDate = DateTime.Now;
                    objpackage.LastModifyUser = _User;
                    int _JobPackageID = _IJob.InsertPackage(objpackage);

                    var result = db.JobTypes.SingleOrDefault(b => b.JobTypeID == _TypeID);

                    if (result.Flagged == false)
                    {
                        result.Flagged = true;
                        result.LastModifyDate = DateTime.Now;
                        result.LastModifyUser = _User;
                        _IJob.UpdateType(result);
                    }

                    #region --> Add Selected Tasks

                    if (!string.IsNullOrEmpty(collection["ID"]))
                    {
                        string[] ids = collection["ID"].Split(new char[] { ',' });

                        if (ids != null)
                        {
                            foreach (string id in ids)
                            {
                                objpackagelist.SegmentID = _Segment;
                                objpackagelist.TaskID = int.Parse(id);
                                objpackagelist.PackageID = _JobPackageID;
                                objpackagelist.JobTypeID = _TypeID;
                                objpackagelist.Flagged = false;
                                objpackagelist.LastModifyDate = DateTime.Now;
                                objpackagelist.LastModifyUser = _User;
                                _IJob.InsertPackageList(objpackagelist);
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
                                // ---------------------------- Create Job Task Master --------------------------------
                                objjobtaskmaster.SegmentID = _Segment;
                                objjobtaskmaster.TaskName = task;
                                objjobtaskmaster.Flagged = false;
                                objjobtaskmaster.LastModifyDate = DateTime.Now;
                                objjobtaskmaster.LastModifyUser = _User;
                                int returnTaskID = _IJob.InsertTasks(objjobtaskmaster);

                                objpackagelist.SegmentID = _Segment;
                                objpackagelist.TaskID = returnTaskID;
                                objpackagelist.PackageID = _JobPackageID;
                                objpackagelist.JobTypeID = _TypeID;
                                objpackagelist.Flagged = false;
                                objpackagelist.LastModifyDate = DateTime.Now;
                                objpackagelist.LastModifyUser = _User;
                                _IJob.InsertPackageList(objpackagelist);
                            }
                        }
                    }

                    #endregion
                    TempData["successMsg"] = "Package " + _TypeName + " Created";
                    TempData["Success"] = "Success";
                    return RedirectToAction("List", "JobPackage");
                }
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            ViewBag.Current = "Jobs";
            ViewBag.CurrentSub = "Package";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            ViewBag.SubmitValue = "Update";
            ViewBag.SubmitHeader = "Update Category";
            TempData["buttonMsg"] = "ClearButton";

            using (var db = new DatabaseContext())
            {
                var result = db.JobPackages.SingleOrDefault(b => b.PackageID == id);

                if (result == null)
                {
                    return RedirectToAction("List", "JobPackage");
                }
                else
                {
                    var types = (from data in db.JobTypes where data.SegmentID == _Segment && data.JobTypeName != "Custom" orderby data.JobTypeName ascending select data).ToList();
                    ViewBag.List = new SelectList(types, "JobTypeID", "JobTypeName");

                    return View(db.JobPackages.Where(x => x.PackageID == id).FirstOrDefault<JobPackageTB>());
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(JobPackageTB item, FormCollection collection)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var result = db.JobPackages.SingleOrDefault(b => b.PackageName == item.PackageName && b.SegmentID == _Segment && b.TypeID == item.TypeID && b.PackageID != item.PackageID);

                if (result != null)
                {
                    TempData["ErrorMessage"] = result.PackageName;
                    return RedirectToAction("Edit", "JobPackage");
                }
                else
                {
                    string _TypeName = item.PackageName == null ? string.Empty : item.PackageName.Trim();
                    int _TypeID = item.TypeID == null ? -1 : int.Parse(item.TypeID.ToString());

                    var updatedata = db.JobPackages.SingleOrDefault(b => b.PackageID == item.PackageID);

                    if (updatedata == null)
                    {
                        TempData["Error"] = "Package Update Failed...! Please Try Again";
                        return View(List());
                    }
                    else
                    {
                        updatedata.PackageName = _TypeName;
                        updatedata.TypeID = _TypeID;
                        updatedata.LastModifyDate = DateTime.Now;
                        updatedata.LastModifyUser = _User;
                        _IJob.UpdatePackage(updatedata);

                        var packagelist = (from data in db.JobPackageLists where data.PackageID == item.PackageID select data).ToList();
                        if (packagelist.Count > 0)
                        {
                            foreach (var task in packagelist)
                            {
                                db.JobPackageLists.Remove(task);
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
                                    objpackagelist.SegmentID = _Segment;
                                    objpackagelist.TaskID = int.Parse(id);
                                    objpackagelist.PackageID = updatedata.PackageID;
                                    objpackagelist.JobTypeID = _TypeID;
                                    objpackagelist.Flagged = false;
                                    objpackagelist.LastModifyDate = DateTime.Now;
                                    objpackagelist.LastModifyUser = _User;
                                    _IJob.InsertPackageList(objpackagelist);
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
                                    objjobtaskmaster.SegmentID = _Segment;
                                    objjobtaskmaster.TaskName = task;
                                    objjobtaskmaster.Flagged = false;
                                    objjobtaskmaster.LastModifyDate = DateTime.Now;
                                    objjobtaskmaster.LastModifyUser = _User;
                                    int returnTaskID = _IJob.InsertTasks(objjobtaskmaster);

                                    objpackagelist.SegmentID = _Segment;
                                    objpackagelist.TaskID = returnTaskID;
                                    objpackagelist.PackageID = updatedata.PackageID;
                                    objpackagelist.JobTypeID = _TypeID;
                                    objpackagelist.Flagged = false;
                                    objpackagelist.LastModifyDate = DateTime.Now;
                                    objpackagelist.LastModifyUser = _User;
                                    _IJob.InsertPackageList(objpackagelist);
                                }
                            }
                        }

                        #endregion

                        TempData["Updated"] = "Updated";
                        return RedirectToAction("List", "JobTaskType");
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

                var data = _IJob.DeletePackage(Convert.ToInt32(id));

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