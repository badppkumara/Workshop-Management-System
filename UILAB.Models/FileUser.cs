using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("FileUser")]

    public class FileUser
    {
        public Nullable<int> SegmentID { get; set; }

        [Key]
        public int FileID { get; set; }
        public Nullable<int> EntryID { get; set; }
        public Nullable<int> FileTypeID { get; set; }
        public Nullable<int> TransactionID { get; set; }
        public string FileName { get; set; }
        public byte[] FileBitStreem { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string FileTypeDescription { get; set; }
        public Nullable<bool> IsPrimaryPicture { get; set; }
        public string FilePath { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
    }
}

// 3 - Employee Profile Image
// 4 - Customer Profile Image
// 5 - Supplier Profile Image
