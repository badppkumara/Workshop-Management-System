using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("VehicleMakeTB")]
    public class VehicleMakeTB
    {
        [Key]
        public int MakeID { get; set; }
        public string Make { get; set; }
        public bool Flagged { get; set; }
        public int FileID { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
    }
}
