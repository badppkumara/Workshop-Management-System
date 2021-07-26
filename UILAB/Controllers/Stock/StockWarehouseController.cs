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
    public class StockWarehouseController : Controller
    {
        StockWarehouse objstockwarehouse = new StockWarehouse();
        IStock _IStock;

        public StockWarehouseController()
        {
            _IStock = new StockConcrete();
        }

        [HttpGet]
        public ActionResult List()
        {
            ViewBag.Current = "Stock";
            ViewBag.CurrentSub = "Warehouse";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.StockWarehouses where data.SegmentID == _Segment orderby data.Warehouse ascending select data).ToList();
                return View(list);
            }
        }

        [HttpGet]
        public ActionResult Create(int id = 0)
        {
            ViewBag.Current = "Stock";
            ViewBag.CurrentSub = "Warehouse";
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

                    return View(db.StockWarehouses.Where(x => x.WarehouseID == id).FirstOrDefault<StockWarehouse>());
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StockWarehouse item)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var itemdata = db.StockWarehouses.SingleOrDefault(b => b.Warehouse == item.Warehouse && b.SegmentID == _Segment && b.WarehouseID != item.WarehouseID);

                if (itemdata != null)
                {
                    TempData["ErrorMessage"] = itemdata.Warehouse;
                    return RedirectToAction("List", "StockWarehouse");
                }
                else
                {
                    string _Name = item.Warehouse == null ? string.Empty : item.Warehouse.Trim();

                    if (item.WarehouseID != 0)
                    {
                        var updatedata = db.StockWarehouses.SingleOrDefault(b => b.WarehouseID == item.WarehouseID);

                        updatedata.Warehouse = _Name;
                        updatedata.LastModifyDate = DateTime.Now;
                        updatedata.LastModifyUser = _User;
                        _IStock.UpdateWarehouse(updatedata);

                        TempData["successMsg"] = " " + _Name + " Updated";
                    }
                    else
                    {
                        objstockwarehouse.SegmentID = _Segment;
                        objstockwarehouse.Warehouse = _Name;
                        objstockwarehouse.Flagged = false;
                        objstockwarehouse.LastModifyDate = DateTime.Now;
                        objstockwarehouse.LastModifyUser = _User;
                        _IStock.InsertWarehouse(objstockwarehouse);
                        TempData["successMsg"] = " " + _Name + " Created";
                    }
                    
                    TempData["Success"] = "Success";
                    return RedirectToAction("List", "StockWarehouse");
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

                var data = _IStock.DeleteWarehouse(Convert.ToInt32(id));

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