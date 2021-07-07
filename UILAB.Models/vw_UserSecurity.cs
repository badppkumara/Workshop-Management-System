using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("vw_UserSecurity")]
    public class vw_UserSecurity
    {
        public Nullable<int> SegmentID { get; set; }
        [Key]
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public Nullable<int> RoleID { get; set; }
        public string RoleName { get; set; }
        public Nullable<bool> Flagged { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> EmployeeNo { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
        public string EmployeeName { get; set; }
        public Nullable<int> DesignationID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string OtherName { get; set; }
        public string DocumentEmployeeNo { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public Nullable<int> GenderID { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public Nullable<System.DateTime> DateJoined { get; set; }
        public Nullable<bool> IsCitizen { get; set; }
        public string TaxFileNo { get; set; }
        public string EmployeeUserName { get; set; }
        public string SegmentName { get; set; }
        public string Gender { get; set; }
    }
}
