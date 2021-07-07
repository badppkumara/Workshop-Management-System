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
    public class ResourceSubTypeController : Controller
    {
        ResourceSubType objresourcesubtype = new ResourceSubType();

        IResource _IResource;
        public ResourceSubTypeController()
        {
            _IResource = new ResourceConcrete();
        }

        [HttpGet]
        public ActionResult List()
        {
            ViewBag.Current = "Resource";
            ViewBag.CurrentSub = "SubType";
            TempData["Error"] = "";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.ResourceSubTypes where data.SegmentID == _Segment orderby data.SubTypeName ascending select data).ToList();
                return View(list);
            }
        }

        [HttpGet]
        public ActionResult Create(int id = 0)
        {
            ViewBag.Current = "Resource";
            ViewBag.CurrentSub = "SubType";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            if (id == 0)
            {
                ViewBag.SubmitValue = "Save";
                ViewBag.SubmitHeader = "Create";

                using (var db = new DatabaseContext())
                {
                    var user = (from data in db.ResourceTypes where data.SegmentID == _Segment orderby data.TypeName ascending select data).ToList();
                    ViewBag.List = new SelectList(user, "TypeID", "TypeName");

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

                    var user = (from data in db.ResourceTypes where data.SegmentID == _Segment orderby data.TypeName ascending select data).ToList();
                    ViewBag.List = new SelectList(user, "TypeID", "TypeName");

                    return View(db.ResourceSubTypes.Where(x => x.SubTypeID == id).FirstOrDefault<ResourceSubType>());
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ResourceSubType item)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var itemdata = db.ResourceSubTypes.SingleOrDefault(b => b.SubTypeName == item.SubTypeName && b.SegmentID == _Segment && b.SubTypeID != item.SubTypeID);

                if (itemdata != null)
                {
                    TempData["ErrorMessage"] = itemdata.SubTypeName;
                    return RedirectToAction("List", "ResourceSubType");
                }
                else
                {
                    string _SubTypeName = item.SubTypeName == null ? string.Empty : item.SubTypeName.Trim();
                    int _TypeID = item.TypeID == null ? -1 : int.Parse(item.TypeID.ToString());

                    if (item.SubTypeID != 0)
                    {
                        var updatedata = db.ResourceSubTypes.SingleOrDefault(b => b.SubTypeID == item.SubTypeID);

                        updatedata.SubTypeName = _SubTypeName;
                        updatedata.TypeID = _TypeID;
                        updatedata.LastModifyDate = DateTime.Now;
                        updatedata.LastModifyUser = _User;
                        _IResource.UpdateSubType(updatedata);

                        TempData["successMsg"] = " " + _SubTypeName + " Updated";
                    }
                    else
                    {
                        objresourcesubtype.SegmentID = _Segment;
                        objresourcesubtype.SubTypeName = _SubTypeName;
                        objresourcesubtype.TypeID = _TypeID;
                        objresourcesubtype.Flagged = false;
                        objresourcesubtype.LastModifyDate = DateTime.Now;
                        objresourcesubtype.LastModifyUser = _User;
                        _IResource.InsertSubType(objresourcesubtype);
                        TempData["successMsg"] = " " + _SubTypeName + " Created";
                    }

                    // ----------------------------- Update Resource Type -----------------------------
                    var update = db.ResourceTypes.SingleOrDefault(b => b.TypeID == _TypeID);
                    if (update.Flagged == false)
                    {
                        update.Flagged = true;
                        update.LastModifyDate = DateTime.Now;
                        update.LastModifyUser = _User;
                        db.SaveChanges();
                    }                    
                    //------------------------------------------------------------------------------

                    TempData["Success"] = "Success";
                    return RedirectToAction("List", "ResourceSubType");
                }
            }
        }

        [HttpPost]
        public JsonResult Delete(string id)
        {
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(id)))
                {
                    return Json("Error", JsonRequestBehavior.AllowGet);
                }

                var data = _IResource.DeleteSubType(Convert.ToInt32(id), _User);

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