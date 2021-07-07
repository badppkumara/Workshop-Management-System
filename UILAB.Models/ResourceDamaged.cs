using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("ResourceDamaged")]
    public class ResourceDamaged
    {
        public Nullable<int> SegmentID { get; set; }

        [Key]
        public int DamagedID { get; set; }
        public Nullable<int> ResourceID { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<bool> Flagged { get; set; }
        public string Description { get; set; }
        public Nullable<int> ReturnToStock { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }       
    }
}
