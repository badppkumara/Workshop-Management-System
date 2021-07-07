using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("JobTaskTR")]

    public class JobTaskTR
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
        public Nullable<int> StatusID { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> FinishDate { get; set; }
        public Nullable<int> CompletedBy { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
        public bool Flagged { get; set; }
    }
}
