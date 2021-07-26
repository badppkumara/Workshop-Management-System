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
    public class ResourceAssignController : Controller
    {
        ResourceUserTR objresourceuser = new ResourceUserTR();
        IResource _IResource;

        public ResourceAssignController()
        {
            _IResource = new ResourceConcrete();
        }

        [HttpGet]
        public ActionResult List()
        {
            ViewBag.Current = "Resource";
            ViewBag.CurrentSub = "Assign";
            TempData["Error"] = "";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.ResourceUserTRs where data.SegmentID == _Segment orderby data.AssignedID ascending select data).ToList();
                return View(list);
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Current = "Resource";
            ViewBag.CurrentSub = "Assign";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var resource = (from data in db.ResourceTRs where data.SegmentID == _Segment orderby data.Resource ascending select data).ToList();
                ViewBag.ResourceList = new SelectList(resource, "ResourceID", "Resource");

                var user = (from data in db.vw_EmployeeTB where data.SegmentID == _Segment orderby data.FullName ascending select data).ToList();
                ViewBag.UserList = new SelectList(user, "EmployeeNo", "FullName");

                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ResourceUserTR item)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var itemdata = db.ResourceUserTRs.SingleOrDefault(b => b.AssignedID == item.AssignedID && b.SegmentID == _Segment && b.ResourceID == item.ResourceID);

                if (itemdata != null)
                {
                    TempData["ErrorMessage"] = "Resource Allready Assigned";
                    return RedirectToAction("Create", "ResourceAssign");
                }
                else
                {
                    int _ResourceID = item.ResourceID == null ? -1 : int.Parse(item.ResourceID.ToString());
                    int _EmployeeID = item.UserID == null ? -1 : int.Parse(item.UserID.ToString());

                    objresourceuser.SegmentID = _Segment;
                    objresourceuser.ResourceID = _ResourceID;
                    objresourceuser.UserID = _EmployeeID;
                    objresourceuser.Flagged = false;
                    objresourceuser.LastModifyDate = DateTime.Now;
                    objresourceuser.LastModifyUser = _User;
                    int _ReturnID = _IResource.InsertResourceUser(objresourceuser);

                    Session["PurchaseOrder"] = Convert.ToString(_ReturnID);
                    Session["Employee"] = Convert.ToString(_EmployeeID);

                    TempData["Success"] = "Success";
                    return RedirectToAction("Create", "ResourceAssign");
                }
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            ViewBag.Current = "Resource";
            ViewBag.CurrentSub = "Assign";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var result = db.ResourceUserTRs.SingleOrDefault(b => b.AssignedID == id);

                if (result == null)
                {
                    return RedirectToAction("List", "ResourceAssign");
                }
                else
                {
                    var resource = (from data in db.ResourceTRs where data.SegmentID == _Segment && data.AssignedID == -1 orderby data.Resource ascending select data).ToList();
                    ViewBag.ResourceList = new SelectList(resource, "ResourceID", "Resource");

                    var user = (from data in db.vw_EmployeeTB where data.SegmentID == _Segment orderby data.FullName ascending select data).ToList();
                    ViewBag.ResourceList = new SelectList(user, "EmployeeNo", "FullName");

                    return View(db.ResourceUserTRs.Where(x => x.AssignedID == id).FirstOrDefault<ResourceUserTR>());
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ResourceUserTR item)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var itemdata = db.ResourceUserTRs.SingleOrDefault(b => b.ResourceID == item.ResourceID && b.SegmentID == _Segment && b.AssignedID == item.AssignedID);

                if (itemdata != null)
                {
                    TempData["ErrorMessage"] = "Resource Allready Assigned";
                    return RedirectToAction("Create", "ResourceAssign");
                }
                else
                {
                    int _ResourceID = item.ResourceID == null ? -1 : int.Parse(item.ResourceID.ToString());
                    int _EmployeeID = item.UserID == null ? -1 : int.Parse(item.UserID.ToString());

                    var updatedata = db.ResourceUserTRs.SingleOrDefault(b => b.AssignedID == item.AssignedID);

                    if (updatedata == null)
                    {
                        TempData["Error"] = "Resource Assign Update Failed...! Please Try Again";
                        return View(List());
                    }
                    else
                    {
                        updatedata.ResourceID = _ResourceID;
                        updatedata.UserID = _EmployeeID;
                        updatedata.LastModifyDate = DateTime.Now;
                        updatedata.LastModifyUser = _User;
                        _IResource.UpdateResourceUser(updatedata);

                        TempData["Updated"] = "Updated";
                        return RedirectToAction("List", "ResourceAssign");
                    }
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

                var data = _IResource.DeleteResourceUser(Convert.ToInt32(id));

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

        [HttpGet]
        public ActionResult AssignedList()
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _EmployeeID = Convert.ToInt32(HttpContext.Session["Employee"]);

            ViewBag.Current = "Resource";
            ViewBag.CurrentSub = "Assign";
            TempData["Error"] = "";

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.ResourceTRs where data.SegmentID == _Segment orderby data.AssignedID ascending select data).ToList();
                return View(list);
            }
        }

        [HttpPost]
        public JsonResult DeleteList(string id)
        {
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);
            int _EmployeeID = Convert.ToInt32(HttpContext.Session["Employee"]);

            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(id)))
                {
                    return Json("Error", JsonRequestBehavior.AllowGet);
                }

                var data = _IResource.DeleteResourceItem(Convert.ToInt32(id), _EmployeeID);

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