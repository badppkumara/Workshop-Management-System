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
    public class UserConcrete : IUser
    {
        // Administrator
        public void InsertAdmin(UserSecurity admintb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.UserSecurities.Add(admintb);
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateAdmin(UserSecurity admintb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(admintb).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int DeleteAdmin(int admintb, int segment)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.UserSecurities
                                    where item.UserID == admintb
                                    select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        //Remove Admin
                        _context.UserSecurities.Remove(deleteitem);
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

        //Employee
        public int InsertEmployee(EmployeeMaster employeetb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.EmployeeMasters.Add(employeetb);
                    _context.SaveChanges();
                    int id = employeetb.EmployeeNo;
                    return id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateEmployee(EmployeeMaster employeetb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(employeetb).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int DeleteEmployee(int employeetb, int segment)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.EmployeeMasters
                                    where item.EmployeeNo == employeetb
                                    select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        //Remove Employee
                        _context.EmployeeMasters.Remove(deleteitem);
                        int result = _context.SaveChanges();

                        //Remove Profile Image
                        var image = (from data in _context.FileUsers where data.EntryID == employeetb && data.FileTypeID == 3 && data.SegmentID == segment select data).ToList();
                        if (image.Count > 0)
                        {
                            foreach (var imageitem in image)
                            {
                                _context.FileUsers.Remove(imageitem);
                                _context.SaveChanges();
                            }                           
                        }

                        //Remove Address
                        var address = (from data in _context.AddressBook where data.EntryID == employeetb && data.RoleID == 3 && data.SegmentID == segment select data).ToList();
                        if (address.Count > 0)
                        {
                            foreach (var addressitem in address)
                            {
                                _context.AddressBook.Remove(addressitem);
                                _context.SaveChanges();
                            }
                        }

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

        //Customer
        public int InsertCustomer(CustomerTB customertb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.Customers.Add(customertb);
                    _context.SaveChanges();
                    int id = customertb.CustomerID;
                    return id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateCustomer(CustomerTB customertb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(customertb).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int DeleteCustomer(int customertb, int segment)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.Customers
                                      where item.CustomerID == customertb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        //Remove Employee
                        _context.Customers.Remove(deleteitem);
                        int result = _context.SaveChanges();

                        //Remove Profile Image
                        var image = (from data in _context.FileUsers where data.EntryID == customertb && data.FileTypeID == 4 && data.SegmentID == segment select data).ToList();
                        if (image.Count > 0)
                        {
                            foreach (var imageitem in image)
                            {
                                _context.FileUsers.Remove(imageitem);
                                _context.SaveChanges();
                            }
                        }

                        //Remove Address
                        var address = (from data in _context.AddressBook where data.EntryID == customertb && data.RoleID == 4 && data.SegmentID == segment select data).ToList();
                        if (address.Count > 0)
                        {
                            foreach (var addressitem in address)
                            {
                                _context.AddressBook.Remove(addressitem);
                                _context.SaveChanges();
                            }
                        }

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

        //Supplier
        public int InsertSupplier(SupplierTB suppliertb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.Suppliers.Add(suppliertb);
                    _context.SaveChanges();
                    int id = suppliertb.SupplierID;
                    return id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateSupplier(SupplierTB suppliertb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(suppliertb).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int DeleteSupplier(int suppliertb, int segment)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.Suppliers
                                      where item.SupplierID == suppliertb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        //Remove Employee
                        _context.Suppliers.Remove(deleteitem);
                        int result = _context.SaveChanges();

                        //Remove Profile Image
                        var image = (from data in _context.FileUsers where data.EntryID == suppliertb && data.FileTypeID == 5 && data.SegmentID == segment select data).ToList();
                        if (image.Count > 0)
                        {
                            foreach (var imageitem in image)
                            {
                                _context.FileUsers.Remove(imageitem);
                                _context.SaveChanges();
                            }
                        }

                        //Remove Address
                        var address = (from data in _context.AddressBook where data.EntryID == suppliertb && data.RoleID == 5 && data.SegmentID == segment select data).ToList();
                        if (address.Count > 0)
                        {
                            foreach (var addressitem in address)
                            {
                                _context.AddressBook.Remove(addressitem);
                                _context.SaveChanges();
                            }
                        }

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

        //Address Book
        public int InsertAddress(AddressBookTB addresstb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.AddressBook.Add(addresstb);
                    _context.SaveChanges();
                    int id = addresstb.AddressID;
                    return id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateAddress(AddressBookTB addresstb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(addresstb).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }
       
    }
}
