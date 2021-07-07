using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("vw_SalesInvoiceTR")]

    public partial class vw_SalesInvoiceTR
    {
        public Nullable<int> SegmentID { get; set; }
        [Key]
        public int InvoiceID { get; set; }
        public string InvoiceNo { get; set; }
        public Nullable<int> JobID { get; set; }
        public Nullable<int> POID { get; set; }
        public string InvoiceStatus { get; set; }
        public Double SubTotal { get; set; }
        public Double GST { get; set; }
        public Double Total { get; set; }
        public Double TotalNet { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public Nullable<int> VehicleID { get; set; }
        public string PlateNo { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string PostalNo { get; set; }
        public Nullable<int> CustomerID { get; set; }
        public Nullable<int> MakeID { get; set; }
        public string Make { get; set; }
        public Nullable<int> ModelID { get; set; }
        public string ModelName { get; set; }
        public Nullable<int> ModelTypeID { get; set; }
        public string ModelType { get; set; }
        public string ModelNo { get; set; }
        public string Year { get; set; }
        public string ChassisNo { get; set; }
        public string EngineNo { get; set; }
        public string EngineCC { get; set; }
        public Nullable<int> MileageID { get; set; }
        public string Milage { get; set; }
        public string Hubo { get; set; }
        public string RUC { get; set; }
        public Nullable<System.DateTime> RegoExpiryDate { get; set; }
        public Nullable<System.DateTime> WOFExpiryDate { get; set; }
        public string JobTRID { get; set; }
        public Nullable<int> JobTypeID { get; set; }
        public string JobTypeName { get; set; }
        public Nullable<int> JobTaskTypeID { get; set; }
        public Nullable<System.DateTime> JobStartDate { get; set; }
        public Nullable<System.DateTime> JobFinishDate { get; set; }
        public Nullable<int> ApprovalID { get; set; }
        public Nullable<int> ApprovalSchemeID { get; set; }
        public Nullable<int> PreparedBy { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public string Comment { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
        public Nullable<int> StatusID { get; set; }
        public string StatusName { get; set; }
    }
}
