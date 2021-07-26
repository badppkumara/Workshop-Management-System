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

namespace UILAB.Controllers.Stock
{
    [ValidateAdminSession]
    public class StockPOController : Controller
    {
        StockPurchaseOrderTR objstockpo = new StockPurchaseOrderTR();
        FileProduct objfileproduct = new FileProduct();
        SupplierTB objsupplier = new SupplierTB();
        ApprovalHeaderTR objapprovalheader = new ApprovalHeaderTR();
        ApprovalDetailTR objapprovaldetail = new ApprovalDetailTR();
        StockWarehouse objwarehouse = new StockWarehouse();

        IStock _IStock;
        IUser _IUser;
        IApproval _IApproval;

        public StockPOController()
        {
            _IStock = new StockConcrete();
            _IUser = new UserConcrete();
            _IApproval = new ApprovalConcrete();
        }

        [HttpGet]
        public ActionResult List()
        {
            ViewBag.Current = "Stock";
            ViewBag.CurrentSub = "POrder";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var list = (from data in db.StockPurchaseOrders orderby data.PONO descending select data).ToList();
                return View(list);
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Current = "Stock";
            ViewBag.CurrentSub = "POrder";
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);

            using (var db = new DatabaseContext())
            {
                var warehouse = (from data in db.StockWarehouses orderby data.Warehouse ascending select data).ToList();
                ViewBag.WarehouseList = new SelectList(warehouse, "WarehouseID", "Warehouse");

                var supplier = (from data in db.Suppliers orderby data.Company ascending select data).ToList();
                ViewBag.SupplierList = new SelectList(supplier, "SupplierID", "Company");

                var status = (from data in db.StockStatusMasters where data.StatusType == 1 orderby data.StatusName ascending select data).ToList();
                ViewBag.StatusList = new SelectList(status, "StatusID", "StatusName");

                var poprefix = db.SettingGenerals.SingleOrDefault(b => b.PrefixType == 3);

                if (poprefix == null)
                {
                    TempData["ErrorMessage"] = "Purchase Order Prefix Not Setup ...!";
                }
                return View();
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StockPurchaseOrderTR item, FormCollection collection, HttpPostedFileBase[] FileUpload)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var approvalsetting = db.ApprovalSettings.SingleOrDefault(b => b.Code == "POA");

                string _PONo = "";
                DateTime _PODate = collection["PODate"] == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(collection["PODate"].ToString());
                int _SupplierID = item.SupplierID == null ? -1 : int.Parse(item.SupplierID.ToString());
                int _WarehouseID = item.WarehouseID == null ? -1 : int.Parse(item.WarehouseID.ToString());
                int _StatusID = item.StatusID == null ? -1 : int.Parse(item.StatusID.ToString());
                string _Remark = item.Remark == null ? string.Empty : item.Remark.Trim();
                string _Referance = item.Referance == null ? string.Empty : item.Referance.Trim();
                string _POStatus = "";

                string newsuppleir = collection["newsupplier"];
                bool _IsNewSupplier = Convert.ToBoolean(newsuppleir);

                string newwarehouse = collection["nwarehouse"];
                bool _IsNewWarehouse = Convert.ToBoolean(newwarehouse);

                #region -- Create New Supplier

                if (_IsNewSupplier == true)
                {
                    string _Company = collection["company"] == null ? string.Empty : collection["company"].Trim();
                    string _Mobile = collection["mobile"] == null ? string.Empty : collection["mobile"].Trim();
                    string _FName = collection["firstname"] == null ? string.Empty : collection["firstname"].Trim();
                    string _LName = collection["lastname"] == null ? string.Empty : collection["lastname"].Trim();

                    var supplier = db.Suppliers.SingleOrDefault(b => b.Company == _Company);

                    if (supplier != null)
                    {
                        TempData["errorMessage"] = supplier.Company;
                        return RedirectToAction("Create", "StockPO");
                    }
                    else
                    {
                        objsupplier.SegmentID = _Segment;
                        objsupplier.RoleID = 5;
                        objsupplier.FirstName = _FName;
                        objsupplier.MiddleName = "";
                        objsupplier.LastName = _LName;
                        objsupplier.OtherName = "";
                        objsupplier.GenderID = -1;
                        objsupplier.Company = _Company;
                        objsupplier.BuisnessNo = "";
                        objsupplier.GSTNo = "";
                        objsupplier.Email = "";
                        objsupplier.Mobile = _Mobile;
                        objsupplier.Phone = "";
                        objsupplier.Fax = "";
                        objsupplier.UserName = "";
                        objsupplier.Password = "";
                        objsupplier.ConfirmPassword = "";
                        objsupplier.IsActive = true;
                        objsupplier.AddressID = -1;
                        objsupplier.Address1 = "";
                        objsupplier.Address2 = "";
                        objsupplier.Address3 = "";
                        objsupplier.PostalNo = "";
                        objsupplier.CountryID = -1;
                        objsupplier.Flagged = true;
                        objsupplier.LastModifyDate = DateTime.Now;
                        objsupplier.LastModifyUser = _User;
                        int _NewSupplierID = _IUser.InsertSupplier(objsupplier);
                        Session["NewSupplier"] = Convert.ToString(_NewSupplierID);
                    }
                }

                #endregion

                #region Create New Warehouse

                if (_IsNewWarehouse == true)
                {
                    string _Warehouse = collection["warehouse"] == null ? string.Empty : collection["warehouse"].Trim();

                    var warehouse = db.StockWarehouses.SingleOrDefault(b => b.Warehouse == _Warehouse);

                    if (warehouse != null)
                    {
                        TempData["errorMessage"] = warehouse.Warehouse;
                        return RedirectToAction("Create", "StockPO");
                    }
                    else
                    {
                        objwarehouse.SegmentID = _Segment;
                        objwarehouse.Warehouse = _Warehouse;
                        objwarehouse.Flagged = true;
                        objwarehouse.LastModifyDate = DateTime.Now;
                        objwarehouse.LastModifyUser = _User;
                        int _NewWarehouseID = _IStock.InsertWarehouse(objwarehouse);
                        Session["NewWarehouse"] = Convert.ToString(_NewWarehouseID);
                    }
                }

                #endregion

                #region -- > Generate PO No.

                var prefixtype = db.SettingGenerals.SingleOrDefault(b => b.PrefixType == 3);

                if (prefixtype == null)
                {
                    TempData["Prefix"] = "Job Prefix Not Setup ...!";
                }
                else
                {
                    string _Prefix = prefixtype.Prefix;
                    string _StartNo = prefixtype.StartNo;

                    List<StockPurchaseOrderTR> startlist = db.StockPurchaseOrders.ToList();

                    if (startlist.Count == 0)
                    {
                        _PONo = _Prefix + _StartNo;
                    }
                    else
                    {
                        // ----------- Split and Next PO ID ----------------
                        var lastNumber = startlist.Last().PONO;

                        Regex re = new Regex(@"([a-zA-Z]+)(\d+)");
                        Match match = re.Match(lastNumber);

                        string alphaPart = match.Groups[1].Value;
                        string numberPart = match.Groups[2].Value;
                        string _Format = numberPart.Length.ToString();

                        string zero = "{0:D" + _Format + "}".ToString();
                        int split = int.Parse(numberPart) + 1;
                        string value = String.Format(zero, split);
                        _PONo = alphaPart + value;
                        // --------------------------------------------------
                    }
                }

                #endregion

                #region --> Create Purchase Order

                objstockpo.SegmentID = _Segment;
                objstockpo.PONO = _PONo;
                objstockpo.POTypeID = -1;
                objstockpo.PODate = _PODate;

                if (_IsNewSupplier == true)
                {
                    objstockpo.SupplierID = Convert.ToInt32(HttpContext.Session["NewSupplier"]);
                }
                else
                {
                    objstockpo.SupplierID = _SupplierID;
                }

                if (_IsNewWarehouse == true)
                {
                    objstockpo.WarehouseID = Convert.ToInt32(HttpContext.Session["NewWarehouse"]);
                }
                else
                {
                    objstockpo.WarehouseID = _WarehouseID;
                }

                objstockpo.WarehouseID = _WarehouseID;
                objstockpo.StatusID = _StatusID;
                objstockpo.Flagged = false;
                objstockpo.Remark = _Remark;
                objstockpo.Referance = _Referance;
                objstockpo.POStatus = _POStatus;
                objstockpo.Discount = 0;
                objstockpo.TaxAmount = 0;
                objstockpo.Comment = "";
                objstockpo.ApprovedBy = -1;
                objstockpo.PreparedBy = -1;
                objstockpo.Items = -1;
                objstockpo.ApprovalID = -1;
                objstockpo.ApprovalSchemeID = approvalsetting.ApprovalSchemeID;
                objstockpo.TotalAmount = 0;
                objstockpo.LastModifyDate = DateTime.Now;
                objstockpo.LastModifyUser = _User;
                int _ReturnPOID = _IStock.InsertPO(objstockpo);
                Session["PurchaseOrder"] = Convert.ToString(_ReturnPOID);

                #endregion

                #region --> Create Approval Header

                objapprovalheader.SegmentID = _Segment;
                objapprovalheader.TransactionID = _ReturnPOID;
                objapprovalheader.ApprovalSchemeID = approvalsetting.ApprovalSchemeID;
                objapprovalheader.Approved = false;
                objapprovalheader.ApprovalDate = DateTime.Parse("1900-01-01");
                objapprovalheader.Rejected = false;
                objapprovalheader.RejectedDate = DateTime.Parse("1900-01-01");
                objapprovalheader.TextNaration = "PO_Transaction";
                objapprovalheader.Flagged = false;
                objapprovalheader.LastModifyDate = DateTime.Now;
                objapprovalheader.LastModifyUser = _User;
                int _ApprovalID = _IApproval.InsertHeader(objapprovalheader);

                #endregion

                #region --> Create Approval Detail

                //var approvalgroup = (from data in db.ApprovalGroups where data.ApprovalSchemeID == scheme.ApprovalSchemeID && data.SegmentID == _Segment select data).ToList();
                //if (approvalgroup.Count > 0)
                //{
                //    foreach (var user in approvalgroup)
                //    {
                //        var approvaldetail = new ApprovalDetailTR
                //        {
                //            SegmentID = _Segment,
                //            ApprovalID = returnAppovalID,
                //            TransactionID = returnID,
                //            GroupID = user.ApprovalGroupID,
                //            ApprovalEmployeeNo = user.UserID,
                //            ApprovalDesignationID = user.DesignationID,
                //            ApprovalUserID = -1,
                //            ApprovalParentDesignationID = -1,
                //            ApprovalParentEmployeeNo = -1,
                //            ApprovalSchemeID = _ApprovalSchemeID,
                //            Approved = false,
                //            ApprovalTime = DateTime.Parse("1900-01-01"),
                //            Rejected = false,
                //            RejectedTime = DateTime.Parse("1900-01-01"),
                //            Reason = "",
                //            FinalApproval = user.FinalApprover,
                //            ImmedateApproval = false,
                //            ImmediateReject = false,
                //            Flagged = false,
                //            LastModifyDate = DateTime.Now,
                //            LastModifyUser = _User
                //        };
                //        db.ApprovalDetailTRs.Add(approvaldetail);
                //        db.SaveChanges();
                //    }
                //}

                #endregion

                #region --> File Upload

                if (Request.Files.Count > 0)
                {
                    foreach (HttpPostedFileBase file in FileUpload)
                    {
                        if (file != null)
                        {
                            // Save file in Folder
                            string filename = _PONo + "_" + file.FileName;
                            string physicalPath = Server.MapPath("~/Files/PO/" + filename);
                            file.SaveAs(physicalPath);

                            var image = new FileProduct
                            {
                                SegmentID = _Segment,
                                TransactionID = _ReturnPOID,
                                FileTypeID = 2,
                                FileName = filename,
                                FileBitStreem = new byte[file.ContentLength],
                                CreateDate = DateTime.Now,
                                FileTypeDescription = "",
                                FilePath = Server.MapPath("~/Files/PO/" + filename),
                                LastModifyDate = DateTime.Now,
                                LastModifyUser = _User
                            };
                            db.FileProducts.Add(image);
                            db.SaveChanges();
                        }
                    }

                    #endregion
                }

                TempData["Success"] = "Success";
                return RedirectToAction("List", "StockPO");
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            using (var db = new DatabaseContext())
            {
                var result = db.StockPurchaseOrders.SingleOrDefault(b => b.POID == id);

                if (result == null)
                {
                    return View(List());
                }
                else
                {
                    var warehouse = (from data in db.StockWarehouses orderby data.Warehouse ascending select data).ToList();
                    ViewBag.WarehouseList = new SelectList(warehouse, "WarehouseID", "Warehouse");

                    var supplier = (from data in db.Suppliers orderby data.Company ascending select data).ToList();
                    ViewBag.SupplierList = new SelectList(supplier, "SupplierID", "Company");

                    var status = (from data in db.StockStatusMasters where data.StatusType == 1 orderby data.StatusName ascending select data).ToList();
                    ViewBag.StatusList = new SelectList(status, "StatusID", "StatusName");

                    return View(db.StockPurchaseOrders.Where(x => x.POID == id).FirstOrDefault<StockPurchaseOrderTR>());
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StockPurchaseOrderTR item, FormCollection collection, HttpPostedFileBase[] FileUpload)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);

            using (var db = new DatabaseContext())
            {
                var result = db.StockPurchaseOrders.SingleOrDefault(b => b.POID == item.POID);

                result.SupplierID = item.SupplierID;
                result.WarehouseID = item.WarehouseID;
                result.PODate = item.PODate;
                result.StatusID = item.StatusID;
                result.Remark = item.Remark;
                result.Referance = item.Referance;
                result.LastModifyDate = DateTime.Now;
                result.LastModifyUser = _User;
                db.SaveChanges();

                if (Request.Files.Count > 0)
                {
                    foreach (HttpPostedFileBase file in FileUpload)
                    {
                        if (file != null)
                        {
                            // Save file in Folder
                            string filename = result.PONO + "_" + file.FileName;
                            string physicalPath = Server.MapPath("~/Files/PO/" + filename);
                            file.SaveAs(physicalPath);

                            var image = new FileProduct
                            {
                                SegmentID = Convert.ToInt32(HttpContext.Session["Segment"]),
                                TransactionID = item.POID,
                                FileTypeID = 2,
                                FileName = filename,
                                FileBitStreem = new byte[file.ContentLength],
                                CreateDate = DateTime.Now,
                                FileTypeDescription = "",
                                FilePath = Server.MapPath("~/Files/PO/" + filename),
                                LastModifyDate = DateTime.Now,
                                LastModifyUser = Convert.ToInt32(HttpContext.Session["AdminUser"])
                            };
                            db.FileProducts.Add(image);
                            db.SaveChanges();
                        }
                    }
                }

                TempData["Updated"] = "Updated";
                return RedirectToAction("List", "StockPO");
            }
        }

        [HttpGet]
        public ActionResult CreateOrder(int id)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            Session["PurchaseOrder"] = Convert.ToString(id);

            using (var db = new DatabaseContext())
            {
                var purchaseorder = (from data in db.StockPurchaseOrders where data.POID == id select data);

                if (purchaseorder == null)
                {
                    return View(List());
                }
                else
                {
                    var product = (from data in db.StockTRs where data.SegmentID == _Segment orderby data.Product ascending select data).ToList();
                    ViewBag.ProductList = new SelectList(product, "StockID", "Product");

                    var unit = (from data in db.StockUnits orderby data.Unit ascending select data).ToList();
                    ViewBag.UnitList = new SelectList(unit, "UnitID", "Unit");

                    return View();
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrder(StockPurchaseOrderListTR item)
        {
            int _Segment = Convert.ToInt32(HttpContext.Session["Segment"]);
            int _User = Convert.ToInt32(HttpContext.Session["AdminUser"]);
            int _POID = Convert.ToInt32(HttpContext.Session["PurchaseOrder"]);

            using (var db = new DatabaseContext())
            {
                int _ProductID = item.StockID == 0 ? -1 : int.Parse(item.StockID.ToString());
                int _UnitID = item.UnitID == 0 ? -1 : int.Parse(item.UnitID.ToString());
                Double _Qty = item.Qty == 0 ? 0 : item.Qty;
                Double _UnitAmount = item.UnitAmount == 0 ? 0 : item.UnitAmount;
                Double _Total = _Qty * _UnitAmount;
                Double _Discount = item.Discount == 0 ? 0 : item.Discount;
                Double _TaxAmount = item.TaxAmount == 0 ? 0 : item.TaxAmount;
                Double _TotalAmount = (_Qty * _UnitAmount) + _TaxAmount - _Discount;
                string _Remark = item.Remark == null ? string.Empty : item.Remark.Trim();



                var item2 = new StockPurchaseOrderListTR
                {
                    SegmentID = _Segment,
                    POID = _POID,
                    StockID = _ProductID,
                    UnitID = _UnitID,
                    Qty = _Qty,
                    UnitAmount = _UnitAmount,
                    Total = _Total,
                    Discount = _Discount,
                    TaxAmount = _TaxAmount,
                    TotalAmount = _TotalAmount,
                    Flagged = false,
                    Remark = _Remark,
                    LastModifyDate = DateTime.Now,
                    LastModifyUser = _User

                };
                db.StockPurchaseOrderLists.Add(item2);
                db.SaveChanges();

                var update1 = db.StockTRs.SingleOrDefault(b => b.StockID == _ProductID);
                if (update1 != null)
                {
                    if (update1.Flagged == false)
                    {
                        update1.Flagged = true;
                        update1.LastModifyDate = DateTime.Now;
                        update1.LastModifyUser = _User;
                        db.SaveChanges();
                    }
                }

                TempData["Success"] = "Success";
                return RedirectToAction("CreateOrder", "StockPO");
            }
        }


    }
}