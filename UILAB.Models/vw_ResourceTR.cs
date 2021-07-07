using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("vw_ResourceTR")]
    public class vw_ResourceTR
    {
        public Nullable<int> SegmentID { get; set; }

        [Key]
        public int ResourceID { get; set; }
        public string Resource { get; set; }
        public Nullable<int> TypeID { get; set; }
        public string TypeName { get; set; }
        public Nullable<int> SubTypeID { get; set; }
        public string SubTypeName { get; set; }
        public string Code { get; set; }
        public Double Qty { get; set; }
        public Double UnitPrice { get; set; }
        public Nullable<int> BrandID { get; set; }
        public string SerialNo { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
        public bool Flagged { get; set; }
        public Nullable<int> FileID { get; set; }
        public Nullable<int> AssignedID { get; set; }

    }
}
