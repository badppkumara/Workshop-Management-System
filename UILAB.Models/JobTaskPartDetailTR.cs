using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("JobTaskPartDetailTR")]
    public class JobTaskPartDetailTR
    {
        public Nullable<int> SegmentID { get; set; }
        [Key]
        public int TaskPartDetailID { get; set; }
        public int JobTaskTRID { get; set; }
        public Nullable<int> TaskPartID { get; set; }
        public Nullable<int> JobID { get; set; }
        public Nullable<int> StockID { get; set; }
        public Nullable<int> SubTypeID { get; set; }
        public Double Qty { get; set; }
        public string Remark { get; set; }
        public bool Flagged { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
               
    }
}
