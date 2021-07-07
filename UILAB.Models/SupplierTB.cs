using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("SupplierTB")]
    public class SupplierTB
    {
        public Nullable<int> SegmentID { get; set; }
        [Key]
        public int SupplierID { get; set; }

        public Nullable<int> RoleID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string OtherName { get; set; }
        public string Company { get; set; }
        public Nullable<int> GenderID { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
        public Nullable<bool> Flagged { get; set; }
        public Nullable<int> AddressID { get; set; }
        public string BuisnessNo { get; set; }
        public string GSTNo { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string PostalNo { get; set; }
        public Nullable<int> CountryID { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
    }
}
