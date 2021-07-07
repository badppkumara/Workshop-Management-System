using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("StockWarehouseTB")]
    public class StockWarehouseTB
    {
        public Nullable<int> SegmentID { get; set; }

        [Key]
        public int WarehouseID { get; set; }
        public string Warehouse { get; set; }
        public Nullable<bool> Flagged { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
    }
}
