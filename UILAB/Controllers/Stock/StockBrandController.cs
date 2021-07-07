using System;
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
    public class StockBrandController : Controller
    {
        StockBrandTB objstockbrand = new StockBrandTB();
        FileProduct objfileproduct = new FileProduct();
        IStock _IStock;

        public StockBrandController()
        {
            _IStock = new StockConcrete();
        }

        [HttpGet]
        public ActionResult List()
        {
            ViewBag.Current = "Stock";
            ViewBag.CurrentSub = "Brand";
            TempData["Error"] = "";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.StockBrands orderby data.Brand ascending select data).ToList();
                return View(list);
            }
        }

        [HttpGet]
        public ActionResult Create(int id = 0)
        {
            ViewBag.Current = "Stock";
            ViewBag.CurrentSub = "Brand";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            if (id == 0)
            {
                ViewBag.SubmitValue = "Save";
                ViewBag.SubmitHeader = "Create";
                return View();
            }
            else
            {
                using (var db = new DatabaseContext())
                {
                    ViewBag.SubmitValue = "Update";
                    ViewBag.SubmitHeader = "Update";
                    TempData["buttonMsg"] = "ClearButton";

                    var brandimage = db.FileProducts.SingleOrDefault(b => b.TransactionID == id && b.FileTypeID == 1);

                    if (brandimage != null)
                    {
                        //string ImageName = System.IO.Path.GetFileName(brandimage.FileName);
                        ViewBag.Path = String.Format("/Files/StockBrand/{0}", brandimage.FileName.Replace('+', '_'));
                        TempData["viewImg"] = brandimage.FileName;
                    }

                    return View(db.StockBrands.Where(x => x.BrandID == id).FirstOrDefault<StockBrandTB>());
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StockBrandTB item, HttpPostedFileBase File1)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var itemdata = db.StockBrands.SingleOrDefault(b => b.Brand == item.Brand && b.BrandID != item.BrandID);

                if (itemdata != null)
                {
                    TempData["ErrorMessage"] = itemdata.Brand;
                    return RedirectToAction("List", "StockBrand");
                }
                else
                {
                    string _Brand = item.Brand == null ? string.Empty : item.Brand.Trim();

                    if (item.BrandID != 0)
                    {
                        var updatedata = db.StockBrands.SingleOrDefault(b => b.BrandID == item.BrandID);

                        updatedata.Brand = _Brand;
                        updatedata.LastModifyDate = DateTime.Now;
                        updatedata.LastModifyUser = _User;
                        _IStock.UpdateBrand(updatedata);

                        if (File1 != null && File1.ContentLength > 0)
                        {
                            var file = (from data in db.FileProducts where data.TransactionID == item.BrandID && data.FileTypeID == 1 select data).ToList();
                            if (file.Count > 0)
                            {
                                foreach (var img in file)
                                {
                                    db.FileProducts.Remove(img);
                                    db.SaveChanges();

                                    string imgfile = System.IO.Path.GetFileName(img.FileName);
                                    string imgPath = Server.MapPath("~/Files/StockBrand/" + imgfile);
                                    System.IO.File.Delete(imgPath);
                                }
                            }

                            string ImageName = System.IO.Path.GetFileName(File1.FileName);
                            string physicalPath = Server.MapPath("~/Files/StockBrand/" + ImageName);
                            File1.SaveAs(physicalPath);
                            ViewBag.ImageURL = physicalPath;

                            objfileproduct.SegmentID = _Segment;
                            objfileproduct.FileTypeID = 1;
                            objfileproduct.TransactionID = item.BrandID;
                            objfileproduct.FileName = System.IO.Path.GetFileName(File1.FileName);
                            objfileproduct.FileBitStreem = new byte[File1.ContentLength];
                            objfileproduct.CreateDate = DateTime.Now;
                            objfileproduct.FileTypeDescription = _Brand;
                            objfileproduct.FilePath = "/Files/StockBrand/" + ImageName;
                            //objfileproduct.IsPrimaryPicture = true;
                            objfileproduct.LastModifyDate = DateTime.Now;
                            objfileproduct.LastModifyUser = _User;
                            int _FileID = _IStock.InsertBrandImg(objfileproduct);

                            var updateID = db.StockBrands.SingleOrDefault(b => b.BrandID == item.BrandID);
                            if (updateID != null)
                            {
                                updateID.FileID = _FileID;
                                updateID.LastModifyDate = DateTime.Now;
                                updateID.LastModifyUser = _User;
                                db.SaveChanges();
                            }
                        }

                        TempData["successMsg"] = " " + _Brand + " Updated";
                    }
                    else
                    {
                        objstockbrand.SegmentID = _Segment;
                        objstockbrand.Brand = _Brand;
                        objstockbrand.FileID = -1;
                        objstockbrand.Flagged = false;
                        objstockbrand.LastModifyDate = DateTime.Now;
                        objstockbrand.LastModifyUser = _User;
                        int _BrandID = _IStock.InsertBrand(objstockbrand);
                        TempData["successMsg"] = " " + _Brand + " Created";

                        if (File1 != null && File1.ContentLength > 0)
                        {
                            string ImageName = System.IO.Path.GetFileName(File1.FileName);
                            string physicalPath = Server.MapPath("~/Files/StockBrand/" + ImageName);
                            File1.SaveAs(physicalPath);

                            objfileproduct.SegmentID = _Segment;
                            objfileproduct.FileTypeID = 1;
                            objfileproduct.TransactionID = _BrandID;
                            objfileproduct.FileName = System.IO.Path.GetFileName(File1.FileName);
                            objfileproduct.FileBitStreem = new byte[File1.ContentLength];
                            objfileproduct.CreateDate = DateTime.Now;
                            objfileproduct.FileTypeDescription = _Brand;
                            objfileproduct.FilePath = "/Files/StockBrand/" + ImageName;
                            //objfileproduct.IsPrimaryPicture = true;
                            objfileproduct.LastModifyDate = DateTime.Now;
                            objfileproduct.LastModifyUser = _User;
                            int _FileID = _IStock.InsertBrandImg(objfileproduct);

                            var updateID = db.StockBrands.SingleOrDefault(b => b.BrandID == _BrandID);
                            if (updateID != null)
                            {
                                updateID.FileID = _FileID;
                                updateID.LastModifyDate = DateTime.Now;
                                updateID.LastModifyUser = _User;
                                db.SaveChanges();
                            }
                        }
                    }
                    TempData["Success"] = "Success";
                    return RedirectToAction("List", "StockBrand");
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

                var data = _IStock.DeleteBrand(Convert.ToInt32(id));
                var img = _IStock.DeleteBrandImg(Convert.ToInt32(id));

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