using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("VehicleFuelTypeTB")]
    public class VehicleFuelTypeTB
    {
        [Key]
        public int FuelTypeID { get; set; }
        public string FuelType { get; set; }
        public bool Flagged { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
    }
}
