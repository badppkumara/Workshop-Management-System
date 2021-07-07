using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("ResourceType")]
    public class ResourceType
    {
        public Nullable<int> SegmentID { get; set; }

        [Key]
        public int TypeID { get; set; }
        public string TypeName { get; set; }
        public Nullable<bool> Flagged { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }       
    }
}
