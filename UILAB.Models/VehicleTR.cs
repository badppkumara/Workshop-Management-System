using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("VehicleTR")]
    public class VehicleTR
    {
        public Nullable<int> SegmentID { get; set; }
        [Key]
        public int VehicleID { get; set; }
        public string PlateNo { get; set; }
        public Nullable<int> MakeID { get; set; }
        public Nullable<int> ModelID { get; set; }
        public Nullable<int> CustomerID { get; set; }
        public Nullable<int> RegisterTypeID { get; set; }
        public string ChassisNo { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
        public string EngineNo { get; set; }
        public string EngineCC { get; set; }
        public string ModelNo { get; set; }
        public string TyreSize { get; set; }
        public string Color { get; set; }
        public Nullable<int> TransTypeID { get; set; }
        public Nullable<int> FuelTypeID { get; set; }
        public bool Flagged { get; set; }
        public Nullable<int> MileageID { get; set; }
        public string Rego { get; set; }
        public Nullable<System.DateTime> RegoExpiryDate { get; set; }
        public string WOF { get; set; }
        public Nullable<System.DateTime> WOFExpiryDate { get; set; }
        public string Milage { get; set; }
        public string Hubo { get; set; }
        public string RUC { get; set; }
        public string Year { get; set; }
        public string Remark { get; set; }
    }
}

// Register Type ID
// 1 - Company
// 2 - Contractor
// 3 - Customer
