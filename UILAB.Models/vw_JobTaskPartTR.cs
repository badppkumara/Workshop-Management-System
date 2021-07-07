using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("vw_JobTaskPartTR")]

    public partial class vw_JobTaskPartTR
    {
        public Nullable<int> SegmentID { get; set; }

        [Key]
        public int TaskPartID { get; set; }
        public int JobTaskTRID { get; set; }
        public Nullable<int> JobID { get; set; }
        public string JobTRID { get; set; }
        public string Remark { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
        public string PlateNo { get; set; }
        public Nullable<int> JobTypeID { get; set; }
        public Nullable<int> VehicleID { get; set; }
        public Nullable<System.DateTime> JobStartDate { get; set; }
        public Nullable<System.DateTime> JobFinishDate { get; set; }
        public Nullable<int> StatusID { get; set; }
        public Nullable<int> ApprovalID { get; set; }
        public Nullable<int> ApprovalSchemeID { get; set; }
        public Nullable<bool> Flagged { get; set; }
        public string Status { get; set; }
        public Double Items { get; set; }
        public Double Total { get; set; }
        public string BrandName { get; set; }
        public string ModelName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Company { get; set; }
        public string Mobile { get; set; }
        public string JobTypeName { get; set; }
    }
}
