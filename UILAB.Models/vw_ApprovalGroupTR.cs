using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("vw_ApprovalGroupTR")]
    public class vw_ApprovalGroupTR
    {
        public int SegmentID { get; set; }

        [Key]
        public int ApprovalGroupID { get; set; }
        public string GroupName { get; set; }        
        public Nullable<int> ApprovalSchemeID { get; set; }
        public string SchemeName { get; set; }
        public string Code { get; set; }
        public Nullable<int> UserID { get; set; }
        public string FullName { get; set; }
        public Nullable<int> DesignationID { get; set; }
        public string DesignationName { get; set; }
        public Nullable<int> levelID { get; set; }
        public Nullable<bool> FinalApprover { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
        public Nullable<bool> Flagged { get; set; }
    }
}
