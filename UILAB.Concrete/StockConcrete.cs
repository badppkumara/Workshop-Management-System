using System;
using System.Data.Entity;
using System.Linq;
using System.Web.UI.WebControls;
using UILAB.Interface;
using UILAB.Models;
using System.Web;
using System.Web.Mvc;

namespace UILAB.Concrete
{
    public class StockConcrete : IStock
    {
        // Stock Type
        public void InsertType(StockTypeTB typetb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.StockTypes.Add(typetb);
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateType(StockTypeTB typetb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(typetb).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int DeleteType(int typetb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.StockTypes
                                      where item.TypeID == typetb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        _context.StockTypes.Remove(deleteitem);
                        int result = _context.SaveChanges();
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Stock Sub Type
        public void InsertSubType(StockSubTypeTB subtypetb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.StockSubTypes.Add(subtypetb);
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateSubType(StockSubTypeTB subtypetb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(subtypetb).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int DeleteSubType(int subtypetb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.StockSubTypes
                                      where item.TypeID == subtypetb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        _context.StockSubTypes.Remove(deleteitem);
                        int result = _context.SaveChanges();
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Warehouse
        public int InsertWarehouse(StockWarehouseTB warehousetb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.StockWarehouses.Add(warehousetb);
                    _context.SaveChanges();
                    int id = warehousetb.WarehouseID;
                    return id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateWarehouse(StockWarehouseTB warehousetb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(warehousetb).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int DeleteWarehouse(int warehousetb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.StockWarehouses
                                      where item.WarehouseID == warehousetb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        _context.StockWarehouses.Remove(deleteitem);
                        int result = _context.SaveChanges();
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Brands
        public int InsertBrand(StockBrandTB brandtb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.StockBrands.Add(brandtb);
                    _context.SaveChanges();
                    int id = brandtb.BrandID;
                    return id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateBrand(StockBrandTB brandtb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(brandtb).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int DeleteBrand(int brandtb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.StockBrands
                                      where item.BrandID == brandtb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        _context.StockBrands.Remove(deleteitem);
                        int result = _context.SaveChanges();
                            return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Brand Image
        public int InsertBrandImg(FileProduct brandimgtb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.FileProducts.Add(brandimgtb);
                    _context.SaveChanges();
                    int id = brandimgtb.FileID;
                    return id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateBrandImg(FileProduct brandimgtb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(brandimgtb).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int DeleteBrandImg(int brandimgtb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.FileProducts
                                      where item.TransactionID == brandimgtb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        _context.FileProducts.Remove(deleteitem);
                        int result = _context.SaveChanges();

                        string ImageName = System.IO.Path.GetFileName(deleteitem.FileName);
                        string physicalPath = System.Web.HttpContext.Current.Server.MapPath("~/Files/StockBrand/" + ImageName);
                        System.IO.File.Delete(physicalPath);

                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Purchase Order
        public int InsertPO(StockPurchaseOrderTR stockpo)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.StockPurchaseOrders.Add(stockpo);
                    _context.SaveChanges();
                    int id = stockpo.POID;
                    return id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void UpdatePO(StockPurchaseOrderTR stockpo)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(stockpo).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int DeletePO(int stockpo)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.StockPurchaseOrders
                                      where item.POID == stockpo
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        _context.StockPurchaseOrders.Remove(deleteitem);
                        int result = _context.SaveChanges();
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
