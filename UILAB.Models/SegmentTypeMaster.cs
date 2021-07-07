using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("SegmentTypeMaster")]
    public class SegmentTypeMaster
    {
        [Key]
        public int SegmentTypeID { get; set; }
        public string SegmentTypeName { get; set; }
        public bool IsMaster { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
    }
}
