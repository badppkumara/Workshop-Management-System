using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("ApprovalSchemeMaster")]
    public class ApprovalSchemeMaster
    {
        public Nullable<int> SegmentID { get; set; }
        [Key]
        public int ApprovalSchemeID { get; set; }
        public string SchemeName { get; set; }
        public string Code { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
        public Nullable<bool> Flagged { get; set; }
    }
}
