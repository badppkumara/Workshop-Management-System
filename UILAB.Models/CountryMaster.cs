using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("CountryMaster")]

    public class CountryMaster
    {
        [Key]
        public int CountryID { get; set; }
        public string CountryName { get; set; }
        public DateTime LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
    }
}
