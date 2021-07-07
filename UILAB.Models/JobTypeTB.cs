using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("JobTypeTB")]
    public class JobTypeTB
    {
        public Nullable<int> SegmentID { get; set; }
        [Key]
        public int JobTypeID { get; set; }
        public string JobTypeName { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
        public bool Flagged { get; set; }
        public Nullable<int> ApprovalSchemeID { get; set; }
        public Nullable<int> ApprovalSettingID { get; set; }
    }
}
