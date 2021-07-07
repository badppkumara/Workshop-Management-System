using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("JobPackageListTB")]

    public class JobPackageListTB
    {
        public Nullable<int> SegmentID { get; set; }
        [Key]
        public int PackageListID { get; set; }
        public Nullable<int> TaskID { get; set; }
        public Nullable<int> PackageID { get; set; }
        public Nullable<int> JobTypeID { get; set; }
        public bool Flagged { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
    }
}
