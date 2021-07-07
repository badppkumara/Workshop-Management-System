using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("vw_JobTasksTR")]
    public partial class vw_JobTasksTR
    {
        public Nullable<int> SegmentID { get; set; }

        [Key]
        public int JobTaskTRID { get; set; }
        public int JobID { get; set; }
        public string JobTRID { get; set; }
        public string PlateNo { get; set; }
        public Nullable<int> JobTaskTypeID { get; set; }
        public Nullable<int> JobTaskID { get; set; }
        public Nullable<int> VehicleID { get; set; }
        public Nullable<int> JobTypeID { get; set; }
        public string JobTypeName { get; set; }
        public string StatusName { get; set; }
        public Nullable<int> StatusID { get; set; }
        public string Code { get; set; }
        public string Remarks { get; set; }
        public string TaskName { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> FinishDate { get; set; }
        public Nullable<int> CompletedBy { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
        public bool Flagged { get; set; }
        public string FullName { get; set; }

    }
}
