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
    public class JobTaskController : Controller
    {
        JobTasksTB objtasks = new JobTasksTB();
        IJob _IJob;

        public JobTaskController()
        {
            _IJob = new JobConcrete();
        }

        [HttpGet]
        public ActionResult List()
        {
            ViewBag.Current = "Jobs";
            ViewBag.CurrentSub = "Task";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.JobTasksTBs where data.SegmentID == _Segment select data).ToList();
                return View(list);
            }
        }

        [HttpGet]
        public ActionResult Create(int id = 0)
        {
            ViewBag.Current = "Jobs";
            ViewBag.CurrentSub = "Task";

            if (id == 0)
            {
                ViewBag.SubmitValue = "Save";
                ViewBag.SubmitHeader = "Create Job Task";
                return View();
            }
            else
            {
                using (var db = new DatabaseContext())
                {
                    ViewBag.SubmitValue = "Update";
                    ViewBag.SubmitHeader = "Update Job Task";
                    TempData["buttonMsg"] = "ClearButton";
                    return View(db.JobTasksTBs.Where(x => x.TaskID == id).FirstOrDefault<JobTasksTB>());
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(JobTasksTB item)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var itemdata = db.JobTasksTBs.SingleOrDefault(b => b.TaskName == item.TaskName && b.SegmentID == _Segment);

                if (itemdata != null)
                {
                    TempData["ErrorMessage"] = itemdata.TaskName;
                    TempData["buttonMsg"] = "ClearButton";
                    return RedirectToAction("List", "JobTask");
                }
                else
                {
                    string _TypeName = item.TaskName == null ? string.Empty : item.TaskName.Trim();

                    if (item.TaskID != 0)
                    {
                        item.SegmentID = _Segment;
                        item.TaskName = _TypeName;
                        item.LastModifyDate = DateTime.Now;
                        item.LastModifyUser = _User;
                        _IJob.UpdateTasks(item);

                        TempData["successMsg"] = "Job Task " + _TypeName + " Updated";
                    }
                    else
                    {
                        objtasks.SegmentID = _Segment;
                        objtasks.TaskName = _TypeName;
                        objtasks.Flagged = false;
                        objtasks.LastModifyDate = DateTime.Now;
                        objtasks.LastModifyUser = _User;
                        _IJob.InsertTasks(objtasks);

                        TempData["successMsg"] = "Job Task " + _TypeName + " Created";
                    }

                    TempData["Success"] = "Success";
                    return RedirectToAction("List", "JobTask");
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

                var data = _IJob.DeleteTasks(Convert.ToInt32(id));

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