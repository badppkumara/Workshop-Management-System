using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("SalesTaxTB")]
    public class SalesTaxTB
    {
        public Nullable<int> SegmentID { get; set; }
        [Key]
        public int SalesTaxID { get; set; }
        public string TaxName { get; set; }
        public Double Percentage { get; set; }
        public Double Amount { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string Code { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
    }
}
