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
    public class MasterConcrete : IMaster
    {
        // Segment
        public int InsertSegment(OrganizationSchemeTR segmenttb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.OrganizationSchemeTRs.Add(segmenttb);
                    _context.SaveChanges();
                    int id = segmenttb.SegmentID;
                    return id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateSegment(OrganizationSchemeTR segmenttb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(segmenttb).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int DeleteSegment(int segmenttb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.OrganizationSchemeTRs
                                 where item.SegmentID == segmenttb
                                 select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        //Remove Segment
                        _context.OrganizationSchemeTRs.Remove(deleteitem);
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

        // Segment Information
        public int InsertInfo(OrganizationSchemeInfo segmentinfotb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.OrganizationSchemeInfos.Add(segmentinfotb);
                    _context.SaveChanges();
                    int id = segmentinfotb.SegmentInfoID;
                    return id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateInfo(OrganizationSchemeInfo segmentinfotb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(segmentinfotb).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int DeleteInfo(int segmentinfotb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.OrganizationSchemeInfos
                                      where item.SegmentID == segmentinfotb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        _context.OrganizationSchemeInfos.Remove(deleteitem);
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

        // Segment Types
        public void InsertSegmentType(SegmentTypeMaster segmenttypetb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.SegmentTypeMasters.Add(segmenttypetb);
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateSegmentType(SegmentTypeMaster segmenttypetb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(segmenttypetb).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int DeleteSegmentType(int segmenttypetb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.SegmentTypeMasters
                                      where item.SegmentTypeID == segmenttypetb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        _context.SegmentTypeMasters.Remove(deleteitem);
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
