using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("UserRoles")]
    public class UserRoles
    {
        [Key]
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> Flagged { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }       
    }
}

// 1 - Super Administrator
// 2 - Administrator
// 3 - Employee
// 4 - Customer
// 5 - Supplier