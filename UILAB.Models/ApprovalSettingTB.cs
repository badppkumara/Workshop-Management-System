using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("ApprovalSettingTB")]
    public class ApprovalSettingTB
    {
        public Nullable<int> SegmentID { get; set; }

        [Key]
        public int ApprovalSettingID { get; set; }
        public Nullable<int> ApprovalSchemeID { get; set; }
        public string ApprovalItem { get; set; }
        public string Code { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
        public bool Flagged { get; set; }

    }
}
