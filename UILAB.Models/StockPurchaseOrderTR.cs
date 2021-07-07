using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("StockPurchaseOrderTR")]
    public class StockPurchaseOrderTR
    {
        public Nullable<int> SegmentID { get; set; }
        [Key]
        public int POID { get; set; }
        public string PONO { get; set; }
        public Nullable<int> SupplierID { get; set; }
        public Nullable<int> POTypeID { get; set; }
        public Nullable<int> StatusID { get; set; }
        public Nullable<int> WarehouseID { get; set; }
        public Nullable<System.DateTime> PODate { get; set; }
        public bool Flagged { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
        public string Remark { get; set; }
        public string Referance { get; set; }
        public string POStatus { get; set; }
        public Double Discount { get; set; }
        public Double TaxAmount { get; set; }
        public Double TotalAmount { get; set; }
        public Double Total { get; set; }
        public Double Items { get; set; }
        public string Comment { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public Nullable<int> PreparedBy { get; set; }
        public Nullable<int> ApprovalSchemeID { get; set; }
        public Nullable<int> ApprovalID { get; set; }

    }
}
