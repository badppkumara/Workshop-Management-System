using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("StockUnitTB")]
    public class StockUnitTB
    {
        [Key]
        public int UnitID { get; set; }
        public string Unit { get; set; }
        public Nullable<bool> Flagged { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
    }
}
