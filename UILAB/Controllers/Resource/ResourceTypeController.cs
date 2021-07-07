using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UILAB.Concrete;
using UILAB.Filters;
using UILAB.Interface;
using UILAB.Models;


namespace UILAB.Controllers.Resource
{
    [ValidateAdminSession]
    public class ResourceTypeController : Controller
    {
        ResourceType objresourcetype = new ResourceType();
        IResource _IResource;

        public ResourceTypeController()
        {
            _IResource = new ResourceConcrete();
        }

        [HttpGet]
        public ActionResult List()
        {
            ViewBag.Current = "Resource";
            ViewBag.CurrentSub = "Type";
            TempData["Error"] = "";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.ResourceTypes where data.SegmentID == _Segment orderby data.TypeName ascending select data).ToList();
                return View(list);
            }
        }

        [HttpGet]
        public ActionResult Create(int id = 0)
        {
            ViewBag.Current = "Resource";
            ViewBag.CurrentSub = "Type";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            if (id == 0)
            {
                ViewBag.SubmitValue = "Save";
                ViewBag.SubmitHeader = "Create";

                using (var db = new DatabaseContext())
                {
                    return View();
                }
            }
            else
            {
                using (var db = new DatabaseContext())
                {
                    ViewBag.SubmitValue = "Update";
                    ViewBag.SubmitHeader = "Update";
                    TempData["buttonMsg"] = "ClearButton";

                    return View(db.ResourceTypes.Where(x => x.TypeID == id).FirstOrDefault<ResourceType>());
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ResourceType item)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var itemdata = db.ResourceTypes.SingleOrDefault(b => b.TypeName == item.TypeName && b.SegmentID == _Segment && b.TypeID != item.TypeID);

                if (itemdata != null)
                {
                    TempData["ErrorMessage"] = itemdata.TypeName;
                    return RedirectToAction("List", "ResourceType");
                }
                else
                {
                    string _TypeName = item.TypeName == null ? string.Empty : item.TypeName.Trim();

                    if (item.TypeID != 0)
                    {
                        var updatedata = db.ResourceTypes.SingleOrDefault(b => b.TypeID == item.TypeID);

                        updatedata.TypeName = _TypeName;
                        updatedata.LastModifyDate = DateTime.Now;
                        updatedata.LastModifyUser = _User;
                        _IResource.UpdateType(updatedata);

                        TempData["successMsg"] = " " + _TypeName + " Updated";
                    }
                    else
                    {
                        objresourcetype.SegmentID = _Segment;
                        objresourcetype.TypeName = _TypeName;
                        objresourcetype.Flagged = false;
                        objresourcetype.LastModifyDate = DateTime.Now;
                        objresourcetype.LastModifyUser = _User;
                        _IResource.InsertType(objresourcetype);
                        TempData["successMsg"] = " " + _TypeName + " Created";
                    }
                    TempData["Success"] = "Success";
                    return RedirectToAction("List", "ResourceType");
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

                var data = _IResource.DeleteType(Convert.ToInt32(id));

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