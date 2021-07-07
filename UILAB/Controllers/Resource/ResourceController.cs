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
    public class ResourceController : Controller
    {
        ResourceTR objresource = new ResourceTR();
        IResource _IResource;

        public ResourceController()
        {
            _IResource = new ResourceConcrete();
        }

        [HttpGet]
        public ActionResult List()
        {
            ViewBag.Current = "Resource";
            ViewBag.CurrentSub = "Assets";
            TempData["Error"] = "";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.vw_ResourceTRs where data.SegmentID == _Segment orderby data.Resource ascending select data).ToList();
                return View(list);
            }
        }

        [HttpGet]
        public ActionResult Create(int id = 0)
        {
            ViewBag.Current = "Resource";
            ViewBag.CurrentSub = "Assets";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            if (id == 0)
            {
                ViewBag.SubmitValue = "Save";
                ViewBag.SubmitHeader = "Create";

                using (var db = new DatabaseContext())
                {
                    var type = (from data in db.ResourceTypes where data.SegmentID == _Segment orderby data.TypeName ascending select data).ToList();
                    ViewBag.TypeList = new SelectList(type, "TypeID", "TypeName");

                    var brand = (from data in db.ResourceBrands orderby data.Brand ascending select data).ToList();
                    ViewBag.BrandList = new SelectList(brand, "BrandID", "Brand");

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

                    var type = (from data in db.ResourceTypes where data.SegmentID == _Segment orderby data.TypeName ascending select data).ToList();
                    ViewBag.TypeList = new SelectList(type, "TypeID", "TypeName");

                    var brand = (from data in db.ResourceBrands orderby data.Brand ascending select data).ToList();
                    ViewBag.BrandList = new SelectList(brand, "BrandID", "Brand");

                    return View(db.ResourceTRs.Where(x => x.TypeID == id).FirstOrDefault<ResourceTR>());
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ResourceTR item)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var itemdata = db.ResourceTRs.SingleOrDefault(b => b.Resource == item.Resource && b.SegmentID == _Segment && b.TypeID == item.TypeID);

                if (itemdata != null)
                {
                    TempData["ErrorMessage"] = itemdata.Resource;
                    return RedirectToAction("Create", "Resource");
                }
                else
                {
                    string _TypeName = item.Resource == null ? string.Empty : item.Resource.Trim();

                    if (item.ResourceID != 0)
                    {
                        var updatedata = db.ResourceTRs.SingleOrDefault(b => b.ResourceID == item.ResourceID);

                        //updatedata.SubTypeName = _SubTypeName;
                        //updatedata.TypeID = _TypeID;
                        //updatedata.LastModifyDate = DateTime.Now;
                        //updatedata.LastModifyUser = _User;
                        //_IResource.UpdateSubType(updatedata);

                        //TempData["successMsg"] = " " + Resource + " Updated";
                    }
                    else
                    {
                        objresource.SegmentID = _Segment;
                        objresource.Flagged = false;
                        objresource.LastModifyDate = DateTime.Now;
                        objresource.LastModifyUser = _User;
                        _IResource.InsertResource(objresource);

                        //TempData["successMsg"] = " " + Resource + " Created";
                    }

                    TempData["Success"] = "Success";
                    return RedirectToAction("List", "Resource");
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

                var data = _IResource.DeleteResource(Convert.ToInt32(id));

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

        public ActionResult FillModel(int brand)
        {
            using (var db = new DatabaseContext())
            {
                var vehmodels = (from data in db.ResourceSubTypes where data.TypeID == brand orderby data.SubTypeName ascending select data).ToList();
                return Json(vehmodels, JsonRequestBehavior.AllowGet);
            }
        }
    }
}