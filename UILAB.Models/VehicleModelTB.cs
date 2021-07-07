using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace UILAB.Models
{
    [Table("VehicleModelTB")]
    public class VehicleModelTB
    {
        [Key]
        public int ModelID { get; set; }
        public string ModelName { get; set; }
        public Nullable<int> ModelTypeID { get; set; }
        public Nullable<int> MakeID { get; set; }
        public bool Flagged { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }

    }
}
