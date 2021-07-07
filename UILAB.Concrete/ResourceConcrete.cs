using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UILAB.Interface;
using UILAB.Models;

namespace UILAB.Concrete
{
    public class ResourceConcrete : IResource
    {
        // Resources
        public int InsertResource(ResourceTR resourcetb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.ResourceTRs.Add(resourcetb);
                    _context.SaveChanges();
                    int id = resourcetb.ResourceID;
                    return id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateResource(ResourceTR resourcetb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(resourcetb).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int DeleteResource(int resourcetb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.ResourceTRs
                                      where item.ResourceID == resourcetb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        _context.ResourceTRs.Remove(deleteitem);
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

        // Resource Type
        public void InsertType(ResourceType typetb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.ResourceTypes.Add(typetb);
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateType(ResourceType typetb)
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
                    var deleteitem = (from item in _context.ResourceTypes
                                      where item.TypeID == typetb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        _context.ResourceTypes.Remove(deleteitem);
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

        // Resource Sub Type
        public void InsertSubType(ResourceSubType subtypetb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.ResourceSubTypes.Add(subtypetb);
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateSubType(ResourceSubType subtypetb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(subtypetb).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int DeleteSubType(int subtypetb, int user)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.ResourceSubTypes
                                      where item.SubTypeID == subtypetb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        _context.ResourceSubTypes.Remove(deleteitem);
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
        public int InsertBrand(ResourceBrandTB brandtb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.ResourceBrands.Add(brandtb);
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

        public void UpdateBrand(ResourceBrandTB brandtb)
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
                    var deleteitem = (from item in _context.ResourceBrands
                                      where item.BrandID == brandtb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        _context.ResourceBrands.Remove(deleteitem);
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
        public int InsertBrandImg(FileResourceTR brandimgtb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.FileResourceTRs.Add(brandimgtb);
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

        public void UpdateBrandImg(FileResourceTR brandimgtb)
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
                    var deleteitem = (from item in _context.FileResourceTRs
                                      where item.TransactionID == brandimgtb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        _context.FileResourceTRs.Remove(deleteitem);
                        int result = _context.SaveChanges();

                        string ImageName = System.IO.Path.GetFileName(deleteitem.FileName);
                        string physicalPath = System.Web.HttpContext.Current.Server.MapPath("~/Files/ResourceBrand/" + ImageName);
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

        // Resources
        public int InsertResourceUser(ResourceUserTR resourceusertb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.ResourceUserTRs.Add(resourceusertb);
                    _context.SaveChanges();
                    int id = resourceusertb.AssignedID;
                    return id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateResourceUser(ResourceUserTR resourceusertb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(resourceusertb).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int DeleteResourceUser(int resourceusertb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.ResourceUserTRs
                                      where item.AssignedID == resourceusertb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        _context.ResourceUserTRs.Remove(deleteitem);
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
        public int DeleteResourceItem(int resourceusertb, int user)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.ResourceUserTRs
                                      where item.AssignedID == resourceusertb && item.UserID == user
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        _context.ResourceUserTRs.Remove(deleteitem);
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
