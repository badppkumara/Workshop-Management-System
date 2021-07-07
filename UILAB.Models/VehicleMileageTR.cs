using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("VehicleMileageTR")]
    public class VehicleMileageTR
    {
        public Nullable<int> SegmentID { get; set; }
        [Key]
        public int MileageID { get; set; }
        public string Mileage { get; set; }
        public int VehicleID { get; set; }
        public string Rego { get; set; }
        public Nullable<System.DateTime> RegoExpiryDate { get; set; }
        public string WOF { get; set; }
        public Nullable<System.DateTime> WOFExpiryDate { get; set; }
        public Nullable<bool> Updated { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
        public string Hubo { get; set; }
        public string RUC { get; set; }
        public bool Flagged { get; set; }
    }
}
