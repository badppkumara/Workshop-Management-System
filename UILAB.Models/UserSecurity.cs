using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("UserSecurity")]
    public class UserSecurity
    {
        [Key]
        public int UserID { get; set; }
        public int? SegmentID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public int? RoleID { get; set; }
        public bool Flagged { get; set; }
        public bool IsActive { get; set; }
        public int? EmployeeNo { get; set; }
        public DateTime? LastModifyDate { get; set; }
        public int? LastModifyUser { get; set; }
    }
}
