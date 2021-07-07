using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("ApprovalHeaderTR")]

    public class ApprovalHeaderTR
    {
        public int SegmentID { get; set; }

        [Key]
        public int ApprovalID { get; set; }
        public int TransactionID { get; set; }
        public Nullable<int> ApprovalSchemeID { get; set; }
        public Nullable<bool> Approved { get; set; }
        public Nullable<System.DateTime> ApprovalDate { get; set; }
        public Nullable<bool> Rejected { get; set; }
        public Nullable<System.DateTime> RejectedDate { get; set; }
        public string TextNaration { get; set; }
        public bool Flagged { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
    }
}
