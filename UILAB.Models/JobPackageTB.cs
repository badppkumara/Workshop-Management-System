using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("JobPackageTB")]
    public class JobPackageTB
    {
        public Nullable<int> SegmentID { get; set; }
        [Key]
        public int PackageID { get; set; }
        public string PackageName { get; set; }
        public Nullable<int> TypeID { get; set; }
        public bool Flagged { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }              
    }
}
