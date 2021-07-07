using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("OrganizationSchemeInfo")]
    public class OrganizationSchemeInfo
    {
        [Key]
        public int SegmentInfoID { get; set; }
        public string SegmentName { get; set; }
        public string BuisnessNo { get; set; }
        public string GSTNo { get; set; }
        public string Email1 { get; set; }
        public string Email2 { get; set; }
        public string Email3 { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }

        public string Mobile { get; set; }
        public string BankAccount1 { get; set; }
        public string BankAccount2 { get; set; }
        public string BankAccount3 { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string PostalNo { get; set; }
        public Nullable<int> CountryID { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
        public Nullable<int> SegmentID { get; set; }
    }
}
