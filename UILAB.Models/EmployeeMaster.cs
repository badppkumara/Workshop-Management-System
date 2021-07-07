using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("EmployeeMaster")]
    public class EmployeeMaster
    {
        public int SegmentID { get; set; }

        [Key]
        public int EmployeeNo { get; set; }
        public Nullable<int> DesignationID { get; set; }
        public Nullable<int> EmpLevelID { get; set; }
        public string DocumentEmployeeNo { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string OtherName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string DrivingLicenceNo { get; set; }
        public Nullable<int> GenderID { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public Nullable<System.DateTime> DateJoined { get; set; }
        public Nullable<System.DateTime> DateLeft { get; set; }
        public Nullable<bool> IsCitizen { get; set; }
        public string TaxFileNo { get; set; }
        public Nullable<int> BankID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public Nullable<int> RoleID { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
        public Nullable<bool> IsContract { get; set; }
        public Nullable<int> AddressID { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string PostalNo { get; set; }
        public Nullable<int> CountryID { get; set; }
        public Nullable<bool> Flagged { get; set; }
    }
}
