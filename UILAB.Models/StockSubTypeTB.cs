using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("StockSubTypeTB")]
    public class StockSubTypeTB
    {
        public Nullable<int> SegmentID { get; set; }

        [Key]
        public int SubTypeID { get; set; }
        public string SubTypeName { get; set; }
        public Nullable<int> TypeID { get; set; }
        public Nullable<bool> Flagged { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }       
    }
}
