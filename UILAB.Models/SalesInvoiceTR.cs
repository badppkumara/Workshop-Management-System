using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("SalesInvoiceTR")]
    public class SalesInvoiceTR
    {
        public Nullable<int> SegmentID { get; set; }
        [Key]
        public int InvoiceID { get; set; }
        public string InvoiceNo { get; set; }
        public Nullable<int> JobID { get; set; }
        public Nullable<int> POID { get; set; }
        public Nullable<int> VehicleID { get; set; }
        public string InvoiceStatus { get; set; }
        public Double SubTotal { get; set; }
        public Double GST { get; set; }
        public Double Total { get; set; }
        public Double TotalNet { get; set; }
        public Nullable<int> CurrencyID { get; set; }
        public Nullable<int> PrefixType { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
        public Nullable<bool> Flagged { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public Nullable<int> PreparedBy { get; set; }
        public string Comment { get; set; }
        public Nullable<int> ApprovalID { get; set; }
        public Nullable<int> ApprovalSchemeID { get; set; }
        public Nullable<int> StatusID { get; set; }
        public Double CustomFloat1 { get; set; }
        public Double CustomFloat2 { get; set; }
        public Double CustomFloat3 { get; set; }
    }
}
