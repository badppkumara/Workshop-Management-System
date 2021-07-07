using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("ResourceTR")]
    public class ResourceTR
    {
        [Key]
        public int ResourceID { get; set; }
        public Nullable<int> SegmentID { get; set; }
        public Nullable<int> TypeID { get; set; }
        public Nullable<int> SubTypeID { get; set; }
        public Nullable<int> BrandID { get; set; }
        public string Code { get; set; }
        public string SerialNo { get; set; }
        public string Resource { get; set; }
        public string Description { get; set; }
        public Double Qty { get; set; }
        public Double UnitPrice { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
        public bool Flagged { get; set; }
        public Nullable<int> FileID { get; set; }
        public Nullable<int> AssignedID { get; set; }
    }

}
