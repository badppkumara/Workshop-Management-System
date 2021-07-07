using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("vw_VehicleTR")]
    public class vw_VehicleTR
    {
        public int SegmentID { get; set; }

        [Key]
        public int VehicleID { get; set; }
        public string PlateNo { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }
        public string DrivingLicenceNo { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string PostalNo { get; set; }
        public int CustomerID { get; set; }              
        public Nullable<int> MakeID { get; set; }
        public string Make { get; set; }
        public Nullable<int> ModelID { get; set; }
        public string ModelName { get; set; }
        public Nullable<int> ModelTypeID { get; set; }
        public string ModelNo { get; set; }
        public string Year { get; set; }
        public string ChassisNo { get; set; }
        public string EngineNo { get; set; }
        public string EngineCC { get; set; }
        public Nullable<int> MileageID { get; set; }
        public string Milage { get; set; }
        public string Hubo { get; set; }
        public string RUC { get; set; }
        public string Rego { get; set; }
        public Nullable<System.DateTime> RegoExpiryDate { get; set; }
        public string WOF { get; set; }
        public Nullable<System.DateTime> WOFExpiryDate { get; set; }
        public Nullable<bool> Flagged { get; set; }
        public string Remark { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public string ModelType { get; set; }
        public string Color { get; set; }
        public string TyreSize { get; set; }
    }
}
