using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("SecurityActiveUsers")]
    public class SecurityActiveUsers
    {
        [Key]
        public int LoginInstance { get; set; }
        public int? SegmentID { get; set; }
        public DateTime? LoginDate { get; set; }
        public DateTime? LogOutDate { get; set; }
        public bool ConnectionAlive { get; set; }
        public int? UserID { get; set; }
        public string WorkStationName { get; set; }
        public string ComputerIP { get; set; }
        public int? UserType { get; set; }

    }
}
