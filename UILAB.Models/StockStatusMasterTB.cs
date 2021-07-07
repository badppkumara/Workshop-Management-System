using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("StockStatusMasterTB")]

    public class StockStatusMasterTB
    {
        public Nullable<int> SegmentID { get; set; }
        [Key]
        public int StatusID { get; set; }
        public string StatusName { get; set; }
        public Nullable<int> StatusType { get; set; }
        public bool Flagged { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
    }
}
