using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("SecurityUserDevices")]
    public class SecurityUserDevices
    {
        [Key]
        public int DeviceID { get; set; }
        public int? SegmentID { get; set; }
        public int? UserID { get; set; }
        public int? UserType { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string WorkStationName { get; set; }
        public string ComputerIP { get; set; }
        public bool ConnectionLive { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
    }
}
