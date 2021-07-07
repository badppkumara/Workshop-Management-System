using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("DesignationMaster")]

    public class DesignationMaster
    {
        public Nullable<int> SegmentID { get; set; }

        [Key]
        public int DesignationID { get; set; }
        public string DesignationName { get; set; }
        public bool Flagged { get; set; }
        public DateTime LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
    }
}
