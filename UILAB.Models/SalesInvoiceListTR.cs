using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("SalesInvoiceListTR")]
    public class SalesInvoiceListTR
    {
        public Nullable<int> SegmentID { get; set; }
        [Key]
        public int SalesInvoiceListID { get; set; }
        public int InvoiceID { get; set; }
        public int StatusID { get; set; }
        public string InvoiceNo { get; set; }
        public string CustomVar1 { get; set; }
        public string CustomVar2 { get; set; }
        public string CustomVar3 { get; set; }
        public string CustomVar4 { get; set; }
        public string CustomVar5 { get; set; }
        public Double CustomFloat1 { get; set; }
        public Double CustomFloat2 { get; set; }
        public Double CustomFloat3 { get; set; }
        public Double CustomFloat4 { get; set; }
        public Double CustomFloat5 { get; set; }
        public Double CustomFloat6 { get; set; }
        public Double CustomFloat7 { get; set; }
        public Double CustomFloat8 { get; set; }
        public Double CustomFloat9 { get; set; }
        public Double CustomFloat10 { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
    }
}
