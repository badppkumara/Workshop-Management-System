using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("StockBrandTB")]
    public class StockBrandTB
    {
        public Nullable<int> SegmentID { get; set; }
        public Nullable<int> FileID { get; set; }

        [Key]
        public int BrandID { get; set; }
        public string Brand { get; set; }
        public Nullable<bool> Flagged { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
    }
}
