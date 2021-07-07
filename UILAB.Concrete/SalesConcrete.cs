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
    public class SalesConcerete : ISales
    {
        // Invoice Jobs
        public int InsertInvoice(SalesInvoiceTR invoicetb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.SalesInvoiceTRs.Add(invoicetb);
                    _context.SaveChanges();
                    int id = invoicetb.InvoiceID;
                    return id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateInvoice(SalesInvoiceTR invoicetb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(invoicetb).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int DeleteInvoice(int invoicetb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.SalesInvoiceTRs
                                 where item.InvoiceID == invoicetb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        //Remove Segment
                        _context.SalesInvoiceTRs.Remove(deleteitem);
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

        // Invoice List Jobs

        public void InsertInvoiceList(SalesInvoiceListTR invoicelisttb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.SalesInvoiceLists.Add(invoicelisttb);
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateInvoiceList(SalesInvoiceListTR invoicelisttb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(invoicelisttb).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int DeleteInvoiceList(int invoicelisttb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.SalesInvoiceLists
                                      where item.SalesInvoiceListID == invoicelisttb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        _context.SalesInvoiceLists.Remove(deleteitem);
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

        // Invoice Status
        public void InsertStatus(SalesStatusTB statustb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.SalesStatusTBs.Add(statustb);
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateStatus(SalesStatusTB statustb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(statustb).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int DeleteStatus(int statustb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.SalesStatusTBs
                                      where item.StatusID == statustb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        _context.SalesStatusTBs.Remove(deleteitem);
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
