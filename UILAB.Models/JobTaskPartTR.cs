using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("JobTaskPartTR")]

    public class JobTaskPartTR
    {
        public Nullable<int> SegmentID { get; set; }
        [Key]
        public int TaskPartID { get; set; }
        public int JobTaskTRID { get; set; }
        public Nullable<int> JobID { get; set; }
        public Nullable<int> ApprovalID { get; set; }
        public Nullable<int> ApprovalSchemeID { get; set; }
        public string Remark { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
        public bool Flagged { get; set; }
        public string Status { get; set; }
        public Double Items { get; set; }
        public Double Total { get; set; }

    }
}
