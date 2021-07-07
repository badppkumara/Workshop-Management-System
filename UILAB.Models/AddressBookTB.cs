using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("AddressBookTB")]
    public class AddressBookTB
    {
        public Nullable<int> SegmentID { get; set; }

        [Key]
        public int AddressID { get; set; }
        public Nullable<int> EntryID { get; set; }
        public Nullable<int> RoleID { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string PostalNo { get; set; }
        public Nullable<int> CountryID { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
    }
}
