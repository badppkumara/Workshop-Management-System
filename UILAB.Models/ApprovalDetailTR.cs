using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("ApprovalDetailTR")]

    public class ApprovalDetailTR
    {
        public int SegmentID { get; set; }
        [Key]
        public int ApprovalDetailID { get; set; }
        public int TransactionID { get; set; }
        public Nullable<int> ApprovalSchemeID { get; set; }
        public Nullable<int> GroupID { get; set; }
        public Nullable<int> ApprovalID { get; set; }
        public Nullable<int> ApprovalEmployeeNo { get; set; }
        public Nullable<int> ApprovalDesignationID { get; set; }
        public Nullable<int> ApprovalUserID { get; set; }
        public Nullable<int> ApprovalParentDesignationID { get; set; }
        public Nullable<int> ApprovalParentEmployeeNo { get; set; }
        public Nullable<System.DateTime> ApprovalTime { get; set; }
        public Nullable<bool> Approved { get; set; }
        public Nullable<System.DateTime> RejectedTime { get; set; }
        public Nullable<bool> Rejected { get; set; }
        public string Reason { get; set; }
        public bool Flagged { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
        public Nullable<bool> FinalApproval { get; set; }
        public Nullable<bool> ImmedateApproval { get; set; }
        public Nullable<bool> ImmediateReject { get; set; }

    }
}
