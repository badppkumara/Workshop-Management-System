using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UILAB.Concrete;
using UILAB.Filters;
using UILAB.Models;
using UILAB.Interface;
using System.Net;
using System.Web.UI;
using System.Text.RegularExpressions;
using System.IO;
namespace UILAB.Controllers.Wms
{
    [ValidateEmployeeSession]
    public class EmpJobController : Controller
    {
        JobTypeTB objtype = new JobTypeTB();
        SalesInvoiceTR objinvoice = new SalesInvoiceTR();
        IJob _IJob;
        ISales _ISales;

        public EmpJobController()
        {
            _IJob = new JobConcrete();
            _ISales = new SalesConcerete();
        }

        [HttpGet]
        public ActionResult List()
        {
            ViewBag.Current = "Jobs";
            ViewBag.CurrentSub = "Job";
            TempData["Error"] = "";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["Employee"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.vw_JobAssignEmpTRs where data.SegmentID == _Segment && data.EmployeeNo == _User orderby data.JobID descending select data).ToList();
                return View(list.ToList());
            }
        }

        [HttpGet]
        public ActionResult EditCustom(int id)
        {
            ViewBag.Current = "Jobs";
            ViewBag.CurrentSub = "Job";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var result = db.JobTRs.SingleOrDefault(b => b.JobID == id);

                if (result == null)
                {
                    return View(List());
                }
                else
                {
                    if (Convert.ToDateTime(result.JobStartDate).ToString("yyyy-MM-dd") == "1900-01-01")
                    {
                        TempData["startdate"] = "";
                    }
                    else
                    {
                        TempData["startdate"] = Convert.ToDateTime(result.JobStartDate).ToString("dd-MM-yyyy");
                    }

                    if (Convert.ToDateTime(result.JobFinishDate).ToString("yyyy-MM-dd") == "1900-01-01")
                    {
                        TempData["finishdate"] = "";
                    }
                    else
                    {
                        TempData["finishdate"] = Convert.ToDateTime(result.JobFinishDate).ToString("dd-MM-yyyy");
                    }


                    var vehicle = (from data in db.VehicleTRs orderby data.PlateNo ascending select data).ToList();
                    ViewBag.VehicleList = new SelectList(vehicle, "VehicleID", "PlateNo");

                    var mileage = db.VehicleMileageTRs.SingleOrDefault(b => b.VehicleID == result.VehicleID && b.Updated == true);
                    if (mileage != null)
                    {
                        TempData["mileage"] = mileage.Mileage;
                        TempData["hubo"] = mileage.Hubo;
                        TempData["ruc"] = mileage.RUC;

                        if (Convert.ToDateTime(mileage.RegoExpiryDate).ToString("yyyy-MM-dd") == "1900-01-01")
                        {
                            TempData["regodate"] = "";
                        }
                        else
                        {
                            TempData["regodate"] = Convert.ToDateTime(mileage.RegoExpiryDate).ToString("dd-MM-yyyy");
                        }

                        if (Convert.ToDateTime(mileage.WOFExpiryDate).ToString("yyyy-MM-dd") == "1900-01-01")
                        {
                            TempData["wofdate"] = "";
                        }
                        else
                        {
                            TempData["wofdate"] = Convert.ToDateTime(mileage.WOFExpiryDate).ToString("dd-MM-yyyy");
                        }
                    }

                    return View(db.JobTRs.Where(x => x.JobID == id).FirstOrDefault<JobTR>());
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCustom(JobTR item, FormCollection collection, HttpPostedFileBase[] FileUpload)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var vehicle = db.VehicleTRs.SingleOrDefault(b => b.VehicleID == item.VehicleID);
                if (vehicle == null)
                {
                    TempData["ErrorMessage"] = "Vehicle Cannot be Empty";
                    return RedirectToAction("EditCustom", "Job");
                }
                else
                {
                    string _PlateNo = vehicle.PlateNo == null ? string.Empty : vehicle.PlateNo.Trim();
                    int _VehicleID = item.VehicleID == null ? -1 : int.Parse(item.VehicleID.ToString());
                    string _Milage = collection["mileage"] == null ? string.Empty : collection["mileage"].Trim();
                    string _Hubo = collection["hubo"] == null ? string.Empty : collection["hubo"].Trim();
                    string _RUC = collection["ruc"] == null ? string.Empty : collection["ruc"].Trim();
                    string _Remark = item.Remark == null ? string.Empty : item.Remark.Trim();
                    DateTime _JobStartDate = collection["startdate"] == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(collection["startdate"].ToString());
                    DateTime _JobFinishDate = collection["finishdate"] == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(collection["finishdate"].ToString());
                    DateTime _RegoExpiryDate = collection["regodate"] == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(collection["regodate"].ToString());
                    DateTime _WOFExpiryDate = collection["wofdate"] == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(collection["wofdate"].ToString());

                    // Update Vehicle 
                    var result2 = db.JobTRs.SingleOrDefault(b => b.JobID == item.JobID);
                    result2.JobStartDate = _JobStartDate;
                    result2.VehicleID = _VehicleID;
                    result2.JobFinishDate = _JobFinishDate;
                    result2.Remark = _Remark;
                    result2.LastModifyDate = DateTime.Now;
                    result2.LastModifyUser = _User;
                    db.SaveChanges();

                    var asstask = (from data in db.JobTaskTRs where data.JobID == item.JobID select data).ToList();
                    if (asstask.Count > 0)
                    {
                        foreach (var task in asstask)
                        {
                            db.JobTaskTRs.Remove(task);
                            db.SaveChanges();
                        }
                    }

                    if (!string.IsNullOrEmpty(collection["ID"]))
                    {
                        string[] ids = collection["ID"].Split(new char[] { ',' });

                        if (ids != null)
                        {
                            foreach (string id in ids)
                            {
                                var tasks = new JobTaskTR
                                {
                                    SegmentID = _Segment,
                                    VehicleID = _VehicleID,
                                    JobID = item.JobID,
                                    JobTypeID = 1,
                                    JobTRID = result2.JobTRID,
                                    JobTaskID = int.Parse(id),
                                    JobTaskTypeID = 1,
                                    PlateNo = _PlateNo,
                                    StartDate = _JobStartDate,
                                    FinishDate = _JobFinishDate,
                                    CompletedBy = -1,
                                    StatusID = 1,
                                    Remarks = _Remark,
                                    Flagged = false,
                                    LastModifyDate = DateTime.Now,
                                    LastModifyUser = _User
                                };
                                db.JobTaskTRs.Add(tasks);
                                db.SaveChanges();
                            }
                        }
                    }

                    var removeemp = (from data in db.JobAssignEmpTRs where data.JobID == item.JobID select data).ToList();
                    if (removeemp.Count > 0)
                    {
                        foreach (var emp in removeemp)
                        {
                            db.JobAssignEmpTRs.Remove(emp);
                            db.SaveChanges();
                        }
                    }

                    if (!string.IsNullOrEmpty(collection["IDEmployee"]))
                    {
                        string[] ids = collection["IDEmployee"].Split(new char[] { ',' });

                        if (ids != null)
                        {
                            foreach (string id in ids)
                            {
                                var emp = new JobAssignEmpTR
                                {
                                    SegmentID = _Segment,
                                    JobID = item.JobID,
                                    EmployeeNo = int.Parse(id),
                                    Flagged = false,
                                    LastModifyDate = DateTime.Now,
                                    LastModifyUser = _User
                                };
                                db.JobAssignEmpTRs.Add(emp);
                                db.SaveChanges();
                            }
                        }
                    }

                    var removepart = (from data in db.JobTaskPartDetailTRs where data.JobID == item.JobID select data).ToList();
                    if (removepart.Count > 0)
                    {
                        foreach (var part in removepart)
                        {
                            db.JobTaskPartDetailTRs.Remove(part);
                            db.SaveChanges();
                        }
                    }

                    if (!String.IsNullOrEmpty(collection["IDPart"]))
                    {
                        string[] ids = collection["IDPart"].Split(new char[] { ',' });

                        if (ids != null)
                        {
                            foreach (string id in ids)
                            {
                                var parts = new JobTaskPartDetailTR
                                {
                                    SegmentID = _Segment,
                                    JobTaskTRID = -1,
                                    TaskPartID = int.Parse(id),
                                    JobID = item.JobID,
                                    StockID = -1,
                                    Qty = 0,
                                    Remark = "",
                                    Flagged = false,
                                    LastModifyDate = DateTime.Now,
                                    LastModifyUser = _User
                                };
                                db.JobTaskPartDetailTRs.Add(parts);
                                db.SaveChanges();
                            }
                        }
                    }

                    if (Request.Files.Count > 0)
                    {
                        foreach (HttpPostedFileBase file in FileUpload)
                        {
                            if (file != null)
                            {
                                // Save file in Folder
                                string filename = result2.PlateNo + "_" + file.FileName;
                                string physicalPath = Server.MapPath("~/Files/Vehicles/" + filename);
                                file.SaveAs(physicalPath);

                                var image = new FileVehicle
                                {
                                    SegmentID = Convert.ToInt32(HttpContext.Session["Segment"]),
                                    TransactionID = result2.JobID,
                                    FileTypeID = 1,
                                    FileName = filename,
                                    FileBitStreem = new byte[file.ContentLength],
                                    CreateDate = DateTime.Now,
                                    FileTypeDescription = "",
                                    FilePath = Server.MapPath("~/Files/Vehicles/" + filename),
                                    LastModifyDate = DateTime.Now,
                                    LastModifyUser = Convert.ToInt32(HttpContext.Session["AdminUser"])
                                };
                                db.FileVehicles.Add(image);
                                db.SaveChanges();
                            }
                        }
                    }



                    // Check Milage true or false
                    var premileage = (from data in db.VehicleMileageTRs
                                      where data.VehicleID == item.VehicleID && data.Updated == true
                                      select data).ToList();

                    if (premileage.Count >= 1)
                    {
                        foreach (var items in premileage)
                        {
                            items.Updated = false;
                            items.LastModifyDate = DateTime.Now;
                            items.LastModifyUser = _User;
                            db.SaveChanges();
                        }
                    }

                    // Create Vehicle Milage
                    var mileage = new VehicleMileageTR
                    {
                        SegmentID = _Segment,
                        Mileage = _Milage,
                        VehicleID = int.Parse(result2.VehicleID.ToString()),
                        Rego = "Rego",
                        RegoExpiryDate = _RegoExpiryDate,
                        WOF = "WOF",
                        WOFExpiryDate = _WOFExpiryDate,
                        Hubo = _Hubo,
                        RUC = _RUC,
                        Updated = true,
                        LastModifyDate = DateTime.Now,
                        LastModifyUser = _User
                    };
                    db.VehicleMileageTRs.Add(mileage);
                    db.SaveChanges();

                    TempData["Updated"] = "Updated";
                    return RedirectToAction("List", "EmpJob");
                }
            }
        }

        [HttpGet]
        public ActionResult StartJob(int id)
        {
            ViewBag.Current = "Jobs";
            ViewBag.CurrentSub = "Job";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["Employee"]);
            Session["JobID"] = Convert.ToString(id);

            using (var db = new DatabaseContext())
            {
                return View(db.JobTRs.Where(x => x.JobID == id).FirstOrDefault<JobTR>());
            }
        }

        [HttpGet]
        public ActionResult UpdateJobTask()
        {
            ViewBag.Current = "Jobs";
            ViewBag.CurrentSub = "Job";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["Emplpoyee"]);
            int _JobID = Convert.ToInt32(HttpContext.Session["JobID"]);

            using (var db = new DatabaseContext())
            {
                var task = (from data in db.vw_JobTasksTRs where data.JobID == _JobID && data.SegmentID == _Segment orderby data.TaskName select data).ToList();
                ViewBag.Tasklist = new SelectList(task, "JobTaskTRID", "TaskName");

                var empassign = (from data in db.vw_JobAssignEmpTRs where data.JobID == _JobID && data.SegmentID == _Segment orderby data.FullName select data).ToList();
                ViewBag.employeelist = new SelectList(empassign, "EmployeeNo", "FullName");

                var status = (from data in db.JobStatuses where data.StatusType == 1 && data.SegmentID == _Segment orderby data.StatusName select data).ToList();
                ViewBag.statuslist = new SelectList(status, "StatusID", "StatusName");

                var result = db.JobTRs.SingleOrDefault(b => b.JobID == _JobID);
                if (Convert.ToDateTime(result.JobStartDate).ToString("yyyy-MM-dd") == "1900-01-01")
                {
                    TempData["startdate"] = "";
                }
                else
                {
                    TempData["startdate"] = Convert.ToDateTime(result.JobStartDate).ToString("dd-MM-yyyy");
                }

                if (Convert.ToDateTime(result.JobFinishDate).ToString("yyyy-MM-dd") == "1900-01-01")
                {
                    TempData["finishdate"] = "";
                }
                else
                {
                    TempData["finishdate"] = Convert.ToDateTime(result.JobFinishDate).ToString("dd-MM-yyyy");
                }

                return View(db.JobTaskTRs.Where(x => x.JobID == _JobID).FirstOrDefault<JobTaskTR>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddJobTask(JobTaskTR item, FormCollection collection)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["Employee"]);
            int _JobID = Convert.ToInt32(HttpContext.Session["JobID"]);

            using (var db = new DatabaseContext())
            {
                string active = collection["checkall"];
                bool _IsActive = Convert.ToBoolean(active);

                int _Status = item.StatusID == 0 ? -1 : int.Parse(item.StatusID.ToString());
                int _Employee = item.CompletedBy == 0 ? -1 : int.Parse(item.CompletedBy.ToString());
                DateTime _JobStartDate = collection["startdate"] == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(collection["startdate"].ToString());
                DateTime _JobFinishDate = collection["finishdate"] == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(collection["finishdate"].ToString());
                string _Remark = item.Remarks == null ? string.Empty : item.Remarks.Trim();

                if (_IsActive == true)
                {
                    var alltasks = (from data in db.JobTaskTRs where data.JobID == item.JobID select data).ToList();
                    if (alltasks.Count > 0)
                    {
                        foreach (var x in alltasks)
                        {
                            x.StatusID = _Status;
                            x.CompletedBy = _Employee;
                            x.StartDate = _JobStartDate;
                            x.FinishDate = _JobFinishDate;
                            x.Remarks = _Remark;
                            x.LastModifyDate = DateTime.Now;
                            x.LastModifyUser = _User;
                            _IJob.UpdateTaskTR(x);
                        }
                    }
                    TempData["Success"] = "Success";
                }
                else
                {
                    var task = db.JobTaskTRs.SingleOrDefault(b => b.JobID == _JobID && b.JobTaskTRID == item.JobTaskTRID && b.SegmentID == _Segment);
                    if (task != null)
                    {
                        task.StatusID = _Status;
                        task.CompletedBy = _Employee;
                        task.StartDate = _JobStartDate;
                        task.FinishDate = _JobFinishDate;
                        task.Remarks = _Remark;
                        task.LastModifyDate = DateTime.Now;
                        task.LastModifyUser = _User;
                        _IJob.UpdateTaskTR(task);

                        TempData["Success"] = "Success";
                    }
                }
                return RedirectToAction("StartJob", "EmpJob", new { id = _JobID });
            }
        }

        [HttpGet]
        public ActionResult OrderList(int id)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _JobID = Convert.ToInt32(HttpContext.Session["JobID"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.vw_JobTasksTRs where data.JobID == _JobID && data.SegmentID == _Segment orderby data.TaskName descending select data).ToList();
                var complete = (from data in db.vw_JobTasksTRs where data.JobID == _JobID && data.StatusID == 4 && data.SegmentID == _Segment select data).ToList();

                int _list = list.Count;
                int _Completed = complete.Count;

                if (_Completed == _list)
                {
                    TempData["Completed"] = "4";
                }
                else
                {
                    TempData["Completed"] = "1";
                }

                return View(list);
            }
        }

        [HttpGet]
        public ActionResult EmployeeList(int id)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _JobID = Convert.ToInt32(HttpContext.Session["JobID"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.vw_JobAssignEmpTRs where data.JobID == _JobID && data.SegmentID == _Segment orderby data.FullName descending select data).ToList();
                return View(list);
            }
        }

        [HttpGet]
        public ActionResult Detail(int id)
        {
            ViewBag.Current = "Jobs";
            ViewBag.CurrentSub = "Job";

            using (var db = new DatabaseContext())
            {
                var result = db.vw_JobTRs.SingleOrDefault(b => b.JobID == id);

                if (result == null)
                {
                    return View(List());
                }
                else
                {
                    //------------------------------ Customer Address ----------------------------------
                    var address = db.AddressBook.SingleOrDefault(b => b.EntryID == result.CustomerID && b.RoleID == 4);

                    if (address == null)
                    {
                        TempData["Address1"] = string.Empty;
                        TempData["Address2"] = string.Empty;
                        TempData["Address3"] = string.Empty;
                        TempData["PostalNo"] = string.Empty;
                    }
                    else
                    {
                        TempData["Address1"] = address.Address1;
                        TempData["Address2"] = address.Address2;
                        TempData["Address3"] = address.Address3;
                        TempData["PostalNo"] = address.PostalNo;
                    }
                    //------------------------------------------------------------------------------------

                    //------------------------------ Vehicle Mileage ----------------------------------
                    var mileage = db.VehicleMileageTRs.SingleOrDefault(b => b.VehicleID == result.VehicleID && b.Updated == true);

                    if (mileage == null)
                    {
                        TempData["Mileage"] = string.Empty;
                    }
                    else
                    {
                        TempData["Mileage"] = mileage.Mileage;
                    }
                    //------------------------------------------------------------------------------------

                    return View(db.vw_JobTRs.Where(x => x.JobID == id).FirstOrDefault<vw_JobTR>());
                }
            }
        }

        [HttpGet]
        public ActionResult DetailJobList(int id)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.vw_JobTasksTRs where data.JobID == id && data.SegmentID == _Segment select data).ToList();
                return View(list);
            }
        }

        [HttpGet]
        public ActionResult ViewParts(int id)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.vw_JobTaskPartDetailTRs where data.JobID == id && data.SegmentID == _Segment orderby data.JobID descending select data).ToList();
                return View(list.ToList());
            }
        }
    }
}