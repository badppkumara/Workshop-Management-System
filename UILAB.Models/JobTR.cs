using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("JobTR")]
    public class JobTR
    {
        public Nullable<int> SegmentID { get; set; }
        [Key]
        public int JobID { get; set; }
        public Nullable<int> VehicleID { get; set; }

        public string JobTRID { get; set; }

        public Nullable<int> JobTypeID { get; set; }
        public Nullable<int> JobTaskTypeID { get; set; }

        public string PlateNo { get; set; }
        public Nullable<System.DateTime> JobStartDate { get; set; }
        public Nullable<System.DateTime> JobFinishDate { get; set; }
        public Nullable<int> StatusID { get; set; }
        public bool Flagged { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
        public string Remark { get; set; }
        public string Description { get; set; }
        public Nullable<int> ApprovalID { get; set; }
        public Nullable<int> ApprovalSchemeID { get; set; }
        public bool Approved  { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }
        public bool Rejected { get; set; }
        public Nullable<System.DateTime> RejectedDate { get; set; }
        public string Reason { get; set; }
        public Nullable<int> InvoiceID { get; set; }
        public Nullable<int> PackageID { get; set; }
    }
}
