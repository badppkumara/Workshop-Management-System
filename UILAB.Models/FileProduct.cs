using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace UILAB.Models
{
    [Table("FileProduct")]
    public class FileProduct
    {
        public Nullable<int> SegmentID { get; set; }

        [Key]
        public int FileID { get; set; }
        public Nullable<int> FileTypeID { get; set; }
        public Nullable<int> TransactionID { get; set; }
        public string FileName { get; set; }
        public byte[] FileBitStreem { get; set; }
        public DateTime CreateDate { get; set; }
        public string FileTypeDescription { get; set; }
        public string FilePath { get; set; }
        public DateTime LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
    }
}
