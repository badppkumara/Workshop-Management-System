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
    public class SegmentTypeController : Controller
    {
        SegmentTypeMaster objsegmentType = new SegmentTypeMaster();
        IMaster  _IMaster;

        public SegmentTypeController()
        {
            _IMaster = new MasterConcrete();
        }

        [HttpGet]
        public ActionResult List()
        {
            ViewBag.Current = "Setting";
            ViewBag.CurrentSub = "SegmentType";
            TempData["Error"] = "";

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.SegmentTypeMasters orderby data.SegmentTypeName ascending select data).ToList();
                return View(list);
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Current = "Setting";
            ViewBag.CurrentSub = "SegmentType";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SegmentTypeMaster item)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var itemdata = db.SegmentTypeMasters.SingleOrDefault(b => b.SegmentTypeName == item.SegmentTypeName);

                if (itemdata != null)
                {
                    TempData["ErrorMessage"] = itemdata.SegmentTypeName;
                    return RedirectToAction("Create", "SegmentType");
                }
                else
                {
                    string _Name = item.SegmentTypeName == null ? string.Empty : item.SegmentTypeName.Trim();

                    objsegmentType.SegmentTypeName = _Name;
                    objsegmentType.LastModifyDate = DateTime.Now;
                    objsegmentType.IsMaster = false;
                    objsegmentType.LastModifyUser = _User;
                    _IMaster.InsertSegmentType(objsegmentType);

                    TempData["Success"] = "Success";
                    return RedirectToAction("List", "SegmentType");
                }
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            ViewBag.Current = "Setting";
            ViewBag.CurrentSub = "SegmentType";

            using (var db = new DatabaseContext())
            {
                var result = db.SegmentTypeMasters.SingleOrDefault(b => b.SegmentTypeID == id);

                if (result == null)
                {
                    return RedirectToAction("List", "SegmentType");
                }
                else
                {
                    return View(db.SegmentTypeMasters.Where(x => x.SegmentTypeID == id).FirstOrDefault<SegmentTypeMaster>());
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SegmentTypeMaster item)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var result = db.SegmentTypeMasters.SingleOrDefault(b => b.SegmentTypeName == item.SegmentTypeName);

                if (result != null)
                {
                    TempData["ErrorMessage"] = result.SegmentTypeName;
                    return RedirectToAction("Edit", "SegmentType");
                }
                else
                {
                    string _Name = item.SegmentTypeName == null ? string.Empty : item.SegmentTypeName.Trim();

                    var updatedata = db.SegmentTypeMasters.SingleOrDefault(b => b.SegmentTypeID == item.SegmentTypeID);

                    if (updatedata == null)
                    {
                        TempData["Error"] = "Segment Type Update Failed...! Please Try Again";
                        return View(List());
                    }
                    else
                    {
                        updatedata.SegmentTypeName = _Name;
                        updatedata.LastModifyDate = DateTime.Now;
                        updatedata.LastModifyUser = _User;
                        _IMaster.UpdateSegmentType(updatedata);

                        TempData["Updated"] = "Updated";
                        return RedirectToAction("List", "SegmentType");
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

                var data = _IMaster.DeleteSegmentType(Convert.ToInt32(id));

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