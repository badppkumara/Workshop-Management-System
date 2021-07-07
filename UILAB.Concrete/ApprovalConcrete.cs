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
    public class ApprovalConcrete : IApproval
    {
        // Approval Scheme
        public void InsertScheme(ApprovalSchemeMaster approvalttb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.ApprovalSchemeMasters.Add(approvalttb);
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateScheme(ApprovalSchemeMaster approvalttb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(approvalttb).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int DeleteScheme(int approvalttb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.ApprovalSchemeMasters
                                 where item.ApprovalSchemeID == approvalttb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        //Remove Segment
                        _context.ApprovalSchemeMasters.Remove(deleteitem);
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

        // Approval Groups
        public void InsertGroup(ApprovalGroupTR grouptb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.ApprovalGroups.Add(grouptb);
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateGroup(ApprovalGroupTR grouptb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(grouptb).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int DeleteGroup(int grouptb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.ApprovalGroups
                                      where item.ApprovalGroupID == grouptb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        //Remove Segment
                        _context.ApprovalGroups.Remove(deleteitem);
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

        // Approval Header
        public int InsertHeader(ApprovalHeaderTR appheadertb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.ApprovalHeaderTRs.Add(appheadertb);
                    _context.SaveChanges();
                    int id = appheadertb.ApprovalID;
                    return id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateHeader(ApprovalHeaderTR appheadertb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(appheadertb).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int DeleteHeader(int appheadertb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.ApprovalHeaderTRs
                                      where item.ApprovalID == appheadertb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        //Remove Segment
                        _context.ApprovalHeaderTRs.Remove(deleteitem);
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

        // Approval Detail
        public void InsertDetail(ApprovalDetailTR appdetailtb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.ApprovalDetailTRs.Add(appdetailtb);
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateDetail(ApprovalDetailTR appdetailtb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(appdetailtb).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int DeleteDetail(int appdetailtb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.ApprovalDetailTRs
                                      where item.ApprovalDetailID == appdetailtb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        //Remove Segment
                        _context.ApprovalDetailTRs.Remove(deleteitem);
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
