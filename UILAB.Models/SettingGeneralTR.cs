using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("SettingGeneralTR")]
    public class SettingGeneralTR
    {
        public Nullable<int> SegmentID { get; set; }

        [Key]
        public int SettingGenID { get; set; }
        public string Prefix { get; set; }
        public Nullable<int> PrefixType { get; set; }
        public string StartNo { get; set; }
        public string Format { get; set; }
        public Nullable<int> CurrencyID { get; set; }
        public string Currency { get; set; }
        public string Symbol { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }

    }
}
