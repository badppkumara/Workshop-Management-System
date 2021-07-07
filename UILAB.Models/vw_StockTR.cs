using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("vw_StockTR")]
    public class vw_StockTR
    {
        [Key]
        public int StockID { get; set; }
        public Nullable<int> SegmentID { get; set; }
        public Nullable<int> TypeID { get; set; }
        public string TypeName { get; set; }
        public Nullable<int> SubTypeID { get; set; }
        public string SubTypeName { get; set; }
        public Nullable<int> BrandID { get; set; }
        public string Brand { get; set; }
        public Nullable<int> WarehouseID { get; set; }
        public string Warehouse { get; set; }
        public string Code { get; set; }
        public string PartNo { get; set; }
        public string Product { get; set; }
        public string Description { get; set; }
        public Double Qty { get; set; }
        public Double UnitPrice { get; set; }
        public Double InStock { get; set; }
        public Double Used { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
        public bool Flagged { get; set; }
        public byte[] Barcode { get; set; }
        public Double AlertQty { get; set; }

    }
}
