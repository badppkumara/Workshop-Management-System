using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UILAB.Concrete;
using UILAB.Filters;
using UILAB.Interface;
using UILAB.Models;
using System.Net;
using System.Web.UI;
using System.Text.RegularExpressions;
using System.IO;

namespace UILAB.Controllers.Stock
{
    [ValidateAdminSession]
    public class StockController : Controller
    {
        public ActionResult List()
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.vw_StockTRs orderby data.Product descending select data).ToList();
                return View(list);
            }
        }

        public ActionResult Create()
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            using (var db = new DatabaseContext())
            {
                var warehouse = (from data in db.StockWarehouses orderby data.Warehouse ascending select data).ToList();
                ViewBag.WarehouseList = new SelectList(warehouse, "WarehouseID", "Warehouse");

                var types = (from data in db.StockTypes orderby data.TypeName ascending select data).ToList();
                ViewBag.TypeList = new SelectList(types, "TypeID", "TypeName");

                var brand = (from data in db.StockBrands orderby data.Brand ascending select data).ToList();
                ViewBag.BrandList = new SelectList(brand, "BrandID", "Brand");

                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StockTR item, HttpPostedFileBase[] FileUpload)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var result = db.StockTRs.SingleOrDefault(b => b.SegmentID == _Segment && b.Code == item.Code);

                if (result != null)
                {
                    TempData["ErrorMessage"] = "Code Exists ...!";
                    return RedirectToAction("Create", "Stock");
                }
                else
                {
                    string _Code = item.Code == null ? string.Empty : item.Code.Trim();
                    int _TypeID = item.TypeID == null ? -1 : int.Parse(item.TypeID.ToString());
                    int _SubTypeID = item.SubTypeID == null ? -1 : int.Parse(item.SubTypeID.ToString());
                    int _WarehouseID = item.WarehouseID == null ? -1 : int.Parse(item.WarehouseID.ToString());
                    int _BrandID = item.BrandID == null ? -1 : int.Parse(item.BrandID.ToString());
                    string _PartNo = item.PartNo == null ? string.Empty : item.PartNo.Trim();
                    string _Product = item.Product == null ? string.Empty : item.Product.Trim();
                    string _Description = item.Description == null ? string.Empty : item.Description.Trim();
                    Double _Qty = item.Qty == 0 ? 0 : item.Qty;
                    Double _UnitPrice = item.UnitPrice == 0 ? 0 : item.UnitPrice;
                    Double _InStock = item.InStock == 0 ? 0 : item.InStock;
                    Double _Used = item.Used == 0 ? 0 : item.Used;
                    Double _AlertQty = item.AlertQty == 0 ? 0 : item.AlertQty;

                    var item2 = new StockTR
                    {
                        SegmentID = _Segment,
                        TypeID = _TypeID,
                        SubTypeID = _SubTypeID,
                        BrandID = _BrandID,
                        Code = _Code,
                        WarehouseID = _WarehouseID,
                        PartNo = _PartNo,
                        Product = _Product,
                        Description = _Description,
                        Qty = _Qty,
                        UnitPrice = _UnitPrice,
                        InStock = _InStock,
                        Used = _Used,
                        AlertQty = _AlertQty,
                        Flagged = false,
                        LastModifyDate = DateTime.Now,
                        LastModifyUser = _User
                    };
                    db.StockTRs.Add(item2);
                    db.SaveChanges();

                    int returnID = item2.StockID;

                    if (Request.Files.Count > 0)
                    {
                        foreach (HttpPostedFileBase file in FileUpload)
                        {
                            if (file != null)
                            {
                                // Save file in Folder
                                string filename = _Code + "_" + file.FileName;
                                string physicalPath = Server.MapPath("~/Files/Products/" + filename);
                                file.SaveAs(physicalPath);

                                var image = new FileProduct
                                {
                                    SegmentID = Convert.ToInt32(HttpContext.Session["Segment"]),
                                    TransactionID = returnID,
                                    FileTypeID = 3,
                                    FileName = filename,
                                    FileBitStreem = new byte[file.ContentLength],
                                    CreateDate = DateTime.Now,
                                    FileTypeDescription = "",
                                    FilePath = Server.MapPath("~/Files/Products/" + filename),
                                    LastModifyDate = DateTime.Now,
                                    LastModifyUser = Convert.ToInt32(HttpContext.Session["AdminUser"])
                                };
                                db.FileProducts.Add(image);
                                db.SaveChanges();
                            }
                        }
                    }


                    var update1 = db.StockTypes.SingleOrDefault(b => b.TypeID == _TypeID);
                    if (update1 != null)
                    {
                        if (update1.Flagged == false)
                        {
                            update1.Flagged = true;
                            update1.LastModifyDate = DateTime.Now;
                            update1.LastModifyUser = Convert.ToInt32(HttpContext.Session["AdminUser"]);
                            db.SaveChanges();
                        }
                    }

                    var update2 = db.StockSubTypes.SingleOrDefault(b => b.SubTypeID == _SubTypeID);
                    if (update2 != null)
                    {
                        if (update2.Flagged == false)
                        {
                            update2.Flagged = true;
                            update2.LastModifyDate = DateTime.Now;
                            update2.LastModifyUser = Convert.ToInt32(HttpContext.Session["AdminUser"]);
                            db.SaveChanges();
                        }
                    }

                    var update3 = db.StockWarehouses.SingleOrDefault(b => b.WarehouseID == _WarehouseID);
                    if (update3 != null)
                    {
                        if (update3.Flagged == false)
                        {
                            update3.Flagged = true;
                            update3.LastModifyDate = DateTime.Now;
                            update3.LastModifyUser = Convert.ToInt32(HttpContext.Session["AdminUser"]);
                            db.SaveChanges();
                        }
                    }

                    TempData["Success"] = "Success";
                    return RedirectToAction("List", "Stock");
                }
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var result = db.StockTRs.SingleOrDefault(b => b.StockID == id);

                if (result == null)
                {
                    return View(List());
                }
                else
                {
                    var warehouse = (from data in db.StockWarehouses orderby data.Warehouse ascending select data).ToList();
                    ViewBag.WarehouseList = new SelectList(warehouse, "WarehouseID", "Warehouse");

                    var types = (from data in db.StockTypes orderby data.TypeName ascending select data).ToList();
                    ViewBag.TypeList = new SelectList(types, "TypeID", "TypeName");

                    var brand = (from data in db.StockBrands orderby data.Brand ascending select data).ToList();
                    ViewBag.BrandList = new SelectList(brand, "BrandID", "Brand");

                    var subtypes = (from data in db.StockSubTypes where data.SubTypeID == result.SubTypeID orderby data.SubTypeName ascending select data).ToList();
                    ViewBag.SubTypeList = new SelectList(subtypes, "SubTypeID", "SubTypeName");

                    return View(db.StockTRs.Where(x => x.StockID == id).FirstOrDefault<StockTR>());
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StockTR item, HttpPostedFileBase[] FileUpload)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var result = (from data in db.StockTRs
                              where data.Code == item.Code && item.SegmentID == _Segment && data.StockID != item.StockID && data.TypeID == item.TypeID
                              select data).Count();

                if (result > 0)
                {
                    TempData["ErrorMessage"] = "Code Exists ...!";
                    return RedirectToAction("Edit", "Stock");
                }
                else
                {
                    string _Code = item.Code == null ? string.Empty : item.Code.Trim();

                    string _PartNo = item.PartNo == null ? string.Empty : item.PartNo.Trim();
                    string _Product = item.Product == null ? string.Empty : item.Product.Trim();
                    Double _Qty = item.Qty == 0 ? 0 : item.Qty;
                    Double _UnitPrice = item.UnitPrice == 0 ? 0 : item.UnitPrice;
                    Double _InStock = item.InStock == 0 ? 0 : item.InStock;
                    Double _Used = item.Used == 0 ? 0 : item.Used;
                    Double _AlertQty = item.AlertQty == 0 ? 0 : item.AlertQty;
                    int _BrandID = item.BrandID == null ? -1 : int.Parse(item.BrandID.ToString());
                    string _Description = item.Description == null ? string.Empty : item.Description.Trim();

                    // Update Product 
                    var result2 = db.StockTRs.SingleOrDefault(b => b.StockID == item.StockID);
                    if (result2 != null)
                    {
                        result2.BrandID = _BrandID;
                        result2.Code = _Code;
                        result2.PartNo = _PartNo;
                        result2.Product = _Product;
                        result2.Description = _Description;
                        result2.Qty = _Qty;
                        result2.UnitPrice = _UnitPrice;
                        result2.AlertQty = _AlertQty;
                        result2.LastModifyDate = DateTime.Now;
                        result2.LastModifyUser = _User;
                        db.SaveChanges();

                        if (Request.Files.Count > 0)
                        {
                            foreach (HttpPostedFileBase file in FileUpload)
                            {
                                if (file != null)
                                {
                                    // Save file in Folder
                                    string filename = _Code + "_" + file.FileName;
                                    string physicalPath = Server.MapPath("~/Files/Products/" + filename);
                                    file.SaveAs(physicalPath);

                                    var image = new FileProduct
                                    {
                                        SegmentID = _Segment,
                                        TransactionID = item.StockID,
                                        FileTypeID = 3,
                                        FileName = filename,
                                        FileBitStreem = new byte[file.ContentLength],
                                        CreateDate = DateTime.Now,
                                        FileTypeDescription = "",
                                        FilePath = Server.MapPath("~/Files/Products/" + filename),
                                        LastModifyDate = DateTime.Now,
                                        LastModifyUser = _User
                                    };
                                    db.FileProducts.Add(image);
                                    db.SaveChanges();
                                }
                            }
                        }
                        TempData["Updated"] = "Updated";
                    }
                    return RedirectToAction("List", "Stock");
                }
            }
        }

        public ActionResult FillModel(int brand)
        {
            using (var db = new DatabaseContext())
            {
                var vehmodels = (from data in db.StockSubTypes where data.TypeID == brand orderby data.SubTypeName ascending select data).ToList();
                return Json(vehmodels, JsonRequestBehavior.AllowGet);
            }
        }
    }
}