using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("OrganizationSchemeTR")]
    public class OrganizationSchemeTR
    {
        [Key]
        public int SegmentID { get; set; }
        public Nullable<int> SegmentInfoID { get; set; }
        public string SegmentName { get; set; }
        public Nullable<int> SegmentTypeID { get; set; }
        public Nullable<int> ParentSegmentID { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public DateTime LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
    }
}
