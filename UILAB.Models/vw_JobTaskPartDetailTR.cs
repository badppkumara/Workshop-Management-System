using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("vw_JobTaskPartDetailTR")]
    public partial class vw_JobTaskPartDetailTR
    {
        public Nullable<int> SegmentID { get; set; }

        [Key]
        public int TaskPartDetailID { get; set; }
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
        public Double UnitPrice { get; set; }
        public string TypeName { get; set; }
    }
}
