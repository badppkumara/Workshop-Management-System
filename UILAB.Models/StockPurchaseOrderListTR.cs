using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("StockPurchaseOrderListTR")]
    public class StockPurchaseOrderListTR
    {
        public Nullable<int> SegmentID { get; set; }
        [Key]
        public int POListID { get; set; }
        public int POID { get; set; }
        public Nullable<int> StockID { get; set; }
        public Nullable<int> UnitID { get; set; }
        public Double Qty { get; set; }
        public Double UnitAmount { get; set; }
        public Double Total { get; set; }
        public Double Discount { get; set; }
        public Double TaxAmount { get; set; }
        public Double TotalAmount { get; set; }
        public string Remark { get; set; }
        public bool Flagged { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
    }
}
