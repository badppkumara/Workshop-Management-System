using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UILAB.Concrete;
using UILAB.Filters;
using UILAB.Interface;
using UILAB.Models;

namespace UILAB.Controllers.Wms
{
    [ValidateAdminSession]
    public class JobStatusController : Controller
    {
        JobStatusTB objstatus = new JobStatusTB();
        IJob _IJob;

        public JobStatusController()
        {
            _IJob = new JobConcrete();
        }

        [HttpGet]
        public ActionResult List()
        {
            ViewBag.Current = "Jobs";
            ViewBag.CurrentSub = "Status";
            TempData["Error"] = "";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.JobStatuses where data.SegmentID == _Segment orderby data.StatusName ascending select data).ToList();
                return View(list);
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Current = "Jobs";
            ViewBag.CurrentSub = "Status";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(JobStatusTB item)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var itemdata = db.JobStatuses.SingleOrDefault(b => b.StatusName == item.StatusName && b.SegmentID == _Segment);

                if (itemdata != null)
                {
                    TempData["ErrorMessage"] = itemdata.StatusName;
                    return RedirectToAction("Create", "JobStatus");
                }
                else
                {
                    string _Name = item.StatusName == null ? string.Empty : item.StatusName.Trim();

                    objstatus.SegmentID = _Segment;
                    objstatus.StatusName = _Name;
                    objstatus.StatusType = 1;
                    objstatus.Flagged = false;
                    objstatus.LastModifyDate = DateTime.Now;
                    objstatus.LastModifyUser = _User;
                    _IJob.InsertStatus(objstatus);

                    TempData["Success"] = "Success";
                    return RedirectToAction("List", "JobStatus");
                }
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            ViewBag.Current = "Jobs";
            ViewBag.CurrentSub = "Status";

            using (var db = new DatabaseContext())
            {
                var result = db.JobStatuses.SingleOrDefault(b => b.StatusID == id);

                if (result == null)
                {
                    return RedirectToAction("List", "JobStatus");
                }
                else
                {
                    return View(db.JobStatuses.Where(x => x.StatusID == id).FirstOrDefault<JobStatusTB>());
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(JobStatusTB item)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var result = db.JobStatuses.SingleOrDefault(b => b.StatusName == item.StatusName && b.SegmentID == _Segment && b.StatusID != item.StatusID);

                if (result != null)
                {
                    TempData["ErrorMessage"] = result.StatusName;
                    return RedirectToAction("Edit", "JobStatus");
                }
                else
                {
                    string _Name = item.StatusName == null ? string.Empty : item.StatusName.Trim();

                    var updatedata = db.JobStatuses.SingleOrDefault(b => b.StatusID == item.StatusID);

                    if (updatedata == null)
                    {
                        TempData["Error"] = "Warehouse Update Failed...! Please Try Again";
                        return View(List());
                    }
                    else
                    {
                        updatedata.StatusName = _Name;
                        updatedata.LastModifyDate = DateTime.Now;
                        updatedata.LastModifyUser = _User;
                        _IJob.UpdateStatus(updatedata);

                        TempData["Updated"] = "Updated";
                        return RedirectToAction("List", "JobStatus");
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

                var data = _IJob.DeleteStatus(Convert.ToInt32(id));

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