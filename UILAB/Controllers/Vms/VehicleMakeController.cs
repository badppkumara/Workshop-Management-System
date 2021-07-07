using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UILAB.Concrete;
using UILAB.Filters;
using UILAB.Interface;
using UILAB.Models;

namespace UILAB.Controllers.Vms
{
    [ValidateAdminSession]
    public class VehicleMakeController : Controller
    {
        VehicleMakeTB objvehiclemake = new VehicleMakeTB();
        FileVehicle objfilevehicle = new FileVehicle();
        IVehicle _IVehicle;

        public VehicleMakeController()
        {
            _IVehicle = new VehicleConcrete();
        }

        [HttpGet]
        public ActionResult List()
        {
            ViewBag.Current = "Vehicle";
            ViewBag.CurrentSub = "Make";

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.VehicleMakes select data).ToList();
                return View(list);
            }
        }

        [HttpGet]
        public ActionResult Create(int id = 0)
        {
            ViewBag.Current = "Vehicle";
            ViewBag.CurrentSub = "Make";

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

                    var makeimage = db.FileVehicles.SingleOrDefault(b => b.FileID == id && b.FileTypeID == 1);

                    if (makeimage != null)
                    {
                        ViewBag.Path = String.Format("/Files/Make/{0}", makeimage.FileName.Replace('+', '_'));
                        TempData["viewImg"] = makeimage.FileName;
                    }


                    return View(db.VehicleMakes.Where(x => x.MakeID == id).FirstOrDefault<VehicleMakeTB>());
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(VehicleMakeTB item, HttpPostedFileBase File1)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var itemdata = db.VehicleMakes.SingleOrDefault(b => b.Make == item.Make && b.MakeID != item.MakeID);

                if (itemdata != null)
                {
                    TempData["ErrorMessage"] = itemdata.Make;
                    return RedirectToAction("List", "VehicleMake");
                }
                else
                {
                    string _Make = item.Make == null ? string.Empty : item.Make.Trim();

                    if (item.MakeID != 0)
                    {
                        var updatedata = db.VehicleMakes.SingleOrDefault(b => b.MakeID == item.MakeID);
                        updatedata.Make = _Make;
                        updatedata.LastModifyDate = DateTime.Now;
                        updatedata.LastModifyUser = _User;
                        _IVehicle.UpdateMake(updatedata);

                        if (File1 != null && File1.ContentLength > 0)
                        {
                            var file = (from data in db.FileVehicles where data.TransactionID == item.MakeID && data.FileTypeID == 1 select data).ToList();
                            if (file.Count > 0)
                            {
                                foreach (var img in file)
                                {
                                    db.FileVehicles.Remove(img);
                                    db.SaveChanges();

                                    string imgfile = System.IO.Path.GetFileName(img.FileName);
                                    string imgPath = Server.MapPath("~/Files/Make/" + imgfile);
                                    System.IO.File.Delete(imgPath);
                                }
                            }


                            string ImageName = System.IO.Path.GetFileName(File1.FileName);
                            string physicalPath = Server.MapPath("/Files/Make/" + ImageName);
                            File1.SaveAs(physicalPath);
                            ViewBag.ImageURL = physicalPath;

                            objfilevehicle.SegmentID = _Segment;
                            objfilevehicle.FileTypeID = 1;
                            objfilevehicle.TransactionID = item.MakeID;
                            objfilevehicle.FileName = System.IO.Path.GetFileName(File1.FileName);
                            objfilevehicle.FileBitStreem = new byte[File1.ContentLength];
                            objfilevehicle.CreateDate = DateTime.Now;
                            objfilevehicle.FileTypeDescription = _Make;
                            objfilevehicle.FilePath = "/Files/Make/" + ImageName;
                            objfilevehicle.IsPrimaryPicture = true;
                            objfilevehicle.LastModifyDate = DateTime.Now;
                            objfilevehicle.LastModifyUser = _User;
                            int _FileID = _IVehicle.InsertMakeImg(objfilevehicle);

                            var updateID = db.VehicleMakes.SingleOrDefault(b => b.MakeID == item.MakeID);
                            if (updateID != null)
                            {
                                updateID.FileID = _FileID;
                                updateID.LastModifyDate = DateTime.Now;
                                updateID.LastModifyUser = _User;
                                db.SaveChanges();
                            }
                        }

                        TempData["successMsg"] = " " + _Make + " Updated";
                    }
                    else
                    {
                        objvehiclemake.Make = _Make;
                        objvehiclemake.FileID = -1;
                        objvehiclemake.LastModifyDate = DateTime.Now;
                        objvehiclemake.LastModifyUser = _User;
                        int _MakeID = _IVehicle.InsertMake(objvehiclemake);

                        if (File1 != null && File1.ContentLength > 0)
                        {
                            string ImageName = System.IO.Path.GetFileName(File1.FileName);
                            string physicalPath = Server.MapPath("~/Files/Make/" + ImageName);
                            File1.SaveAs(physicalPath);

                            objfilevehicle.SegmentID = _Segment;
                            objfilevehicle.FileTypeID = 1;
                            objfilevehicle.TransactionID = _MakeID;
                            objfilevehicle.FileName = System.IO.Path.GetFileName(File1.FileName);
                            objfilevehicle.FileBitStreem = new byte[File1.ContentLength];
                            objfilevehicle.CreateDate = DateTime.Now;
                            objfilevehicle.FileTypeDescription = _Make;
                            objfilevehicle.FilePath = "/Files/Make/" + ImageName;
                            objfilevehicle.IsPrimaryPicture = true;
                            objfilevehicle.LastModifyDate = DateTime.Now;
                            objfilevehicle.LastModifyUser = _User;
                            int _FileID = _IVehicle.InsertMakeImg(objfilevehicle);

                            var updateID = db.VehicleMakes.SingleOrDefault(b => b.MakeID == _MakeID);
                            if (updateID != null)
                            {
                                updateID.FileID = _FileID;
                                updateID.LastModifyDate = DateTime.Now;
                                updateID.LastModifyUser = _User;
                                db.SaveChanges();
                            }
                        }

                        TempData["successMsg"] = " " + _Make + " Created";
                    }

                    TempData["Success"] = "Success";
                    return RedirectToAction("List", "VehicleMake");
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

                var data = _IVehicle.DeleteMake(Convert.ToInt32(id));
                var img = _IVehicle.DeleteMakeImg(Convert.ToInt32(id));

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