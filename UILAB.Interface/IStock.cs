using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UILAB.Models;

namespace UILAB.Interface
{
    public interface IStock
    {
        // Stock Type
        void InsertType(StockTypeTB typetb);
        void UpdateType(StockTypeTB typetb);
        int DeleteType(int typetb);

        // Stock Sub Type
        void InsertSubType(StockSubTypeTB subtypetb);
        void UpdateSubType(StockSubTypeTB subtypetb);
        int DeleteSubType(int subtypetb);

        // Warehouse
        int InsertWarehouse(StockWarehouse warehousetb);
        void UpdateWarehouse(StockWarehouse warehousetb);
        int DeleteWarehouse(int warehousetb);

        // Stock Brand
        int InsertBrand(StockBrandTB brandtb);
        void UpdateBrand(StockBrandTB brandtb);
        int DeleteBrand(int brandtb);

        // Brand Image
        int InsertBrandImg(FileProduct brandimgtb);
        void UpdateBrandImg(FileProduct brandimgtb);
        int DeleteBrandImg(int brandimgtb);

        // Purchase Order
        int InsertPO(StockPurchaseOrderTR stockpo);
        void UpdatePO(StockPurchaseOrderTR stockpo);
        int DeletePO(int stockpo);
    }
}