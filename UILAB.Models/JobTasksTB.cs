using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("JobTasksTB")]
    public class JobTasksTB
    {
        public Nullable<int> SegmentID { get; set; }
        [Key]
        public int TaskID { get; set; }
        public string TaskName { get; set; }
        public bool Flagged { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
    }
}
