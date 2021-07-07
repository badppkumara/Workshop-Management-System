using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("vw_JobTaskPartDetailTmpTR")]
    public partial class vw_JobTaskPartDetailTmpTR
    {
        public Nullable<int> SegmentID { get; set; }

        [Key]
        public int TaskPartDetailTmpID { get; set; }
        public Nullable<int> JobTaskTRID { get; set; }
        public string TaskName { get; set; }
        public Nullable<int> TaskPartID { get; set; }
        public Nullable<int> JobID { get; set; }
        public Nullable<int> StockID { get; set; }
        public string Product { get; set; }
        public string Code { get; set; }
        public string PartNo { get; set; }
        public string Remark { get; set; }
        public Nullable<int> SubTypeID { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
        public string SubTypeName { get; set; }
        public Nullable<bool> Flagged { get; set; }
        public Double Qty { get; set; }
    }
}
