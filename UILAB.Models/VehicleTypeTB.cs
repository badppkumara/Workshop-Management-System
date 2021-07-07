using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("VehicleTypeTB")]
    public class VehicleTypeTB
    {
        [Key]
        public int ModelTypeID { get; set; }
        public string ModelType { get; set; }
        public bool Flagged { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
    }
}
