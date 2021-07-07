using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("GenderMaster")]

    public class GenderMaster
    {
        [Key]
        public int GenderID { get; set; }
        public string Gender { get; set; }
        public DateTime LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
    }
}
