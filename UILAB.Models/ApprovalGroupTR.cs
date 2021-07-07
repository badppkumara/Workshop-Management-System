using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("ApprovalGroupTR")]
    public class ApprovalGroupTR
    {
        public Nullable<int> SegmentID { get; set; }
        [Key]
        public int ApprovalGroupID { get; set; }
        public string GroupName { get; set; }
        public Nullable<int> ApprovalSchemeID { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<int> DesignationID { get; set; }
        public Nullable<int> LevelID { get; set; }
        public Nullable<bool> FinalApprover { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
        public Nullable<bool> Flagged { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}
