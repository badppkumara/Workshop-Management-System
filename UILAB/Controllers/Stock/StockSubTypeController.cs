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
    public class StockSubTypeController : Controller
    {
        StockSubTypeTB objstocksubtype = new StockSubTypeTB();
        IStock _IStock;

        public StockSubTypeController()
        {
            _IStock = new StockConcrete();
        }

        [HttpGet]
        public ActionResult List()
        {
            ViewBag.Current = "Stock";
            ViewBag.CurrentSub = "SubType";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.StockSubTypes where data.SegmentID == _Segment orderby data.SubTypeName ascending select data).ToList();
                return View(list);
            }
        }

        [HttpGet]
        public ActionResult Create(int id = 0)
        {
            ViewBag.Current = "Stock";
            ViewBag.CurrentSub = "SubType";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            if (id == 0)
            {
                ViewBag.SubmitValue = "Save";
                ViewBag.SubmitHeader = "Create";

                using (var db = new DatabaseContext())
                {
                    var user = (from data in db.StockTypes where data.SegmentID == _Segment orderby data.TypeName ascending select data).ToList();
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

                    var user = (from data in db.StockTypes where data.SegmentID == _Segment orderby data.TypeName ascending select data).ToList();
                    ViewBag.List = new SelectList(user, "TypeID", "TypeName");

                    return View(db.StockSubTypes.Where(x => x.SubTypeID == id).FirstOrDefault<StockSubTypeTB>());
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StockSubTypeTB item)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var itemdata = db.StockSubTypes.SingleOrDefault(b => b.SubTypeName == item.SubTypeName && b.SegmentID == _Segment && b.SubTypeID != item.SubTypeID);

                if (itemdata != null)
                {
                    TempData["ErrorMessage"] = itemdata.SubTypeName;
                    return RedirectToAction("List", "StockSubType");
                }
                else
                {
                    string _SubTypeName = item.SubTypeName == null ? string.Empty : item.SubTypeName.Trim();
                    int _TypeID = item.TypeID == null ? -1 : int.Parse(item.TypeID.ToString());

                    if (item.SubTypeID != 0)
                    {
                        var updatedata = db.StockSubTypes.SingleOrDefault(b => b.SubTypeID == item.SubTypeID);

                        updatedata.SubTypeName = _SubTypeName;
                        updatedata.LastModifyDate = DateTime.Now;
                        updatedata.LastModifyUser = _User;
                        _IStock.UpdateSubType(updatedata);

                        TempData["successMsg"] = " " + _SubTypeName + " Updated";
                    }
                    else
                    {
                        objstocksubtype.SegmentID = _Segment;
                        objstocksubtype.SubTypeName = _SubTypeName;
                        objstocksubtype.TypeID = _TypeID;
                        objstocksubtype.Flagged = false;
                        objstocksubtype.LastModifyDate = DateTime.Now;
                        objstocksubtype.LastModifyUser = _User;
                        _IStock.InsertSubType(objstocksubtype);
                        TempData["successMsg"] = " " + _SubTypeName + " Created";
                    }

                    // ----------------------------- Update Resource Type -----------------------------
                    var update = db.StockTypes.SingleOrDefault(b => b.TypeID == _TypeID);
                    if (update.Flagged == false)
                    {
                        update.Flagged = true;
                        update.LastModifyDate = DateTime.Now;
                        update.LastModifyUser = _User;
                        db.SaveChanges();
                    }
                    //------------------------------------------------------------------------------

                    TempData["Success"] = "Success";
                    return RedirectToAction("List", "StockSubType");
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

                var data = _IStock.DeleteSubType(Convert.ToInt32(id));

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