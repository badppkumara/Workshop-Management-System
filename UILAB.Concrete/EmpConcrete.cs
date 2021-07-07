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
    public class EmpConcrete : IEmp
    {
        public int GetVehiclesCount(int segment)
        {
            using (var _context = new DatabaseContext())
            {
                var count = (from item in _context.VehicleTRs where item.SegmentID == segment select item).ToList();

                if (count.Count > 0)
                {
                    int result = count.Count;
                    return result;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int GetEmployeesCount(int segment)
        {
            using (var _context = new DatabaseContext())
            {
                var count = (from item in _context.EmployeeMasters where item.SegmentID == segment && item.IsActive == true select item).ToList();

                if (count.Count > 0)
                {
                    int result = count.Count;
                    return result;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int GetCustomersCount(int segment)
        {
            using (var _context = new DatabaseContext())
            {
                var count = (from item in _context.Customers where item.SegmentID == segment && item.IsActive == true select item).ToList();

                if (count.Count > 0)
                {
                    int result = count.Count;
                    return result;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int GetSuppliersCount(int segment)
        {
            using (var _context = new DatabaseContext())
            {
                var count = (from item in _context.Suppliers where item.SegmentID == segment && item.IsActive == true select item).ToList();

                if (count.Count > 0)
                {
                    int result = count.Count;
                    return result;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int GetJobComplete(int user, int segment)
        {
            using (var _context = new DatabaseContext())
            {
                var count = (from item in _context.JobAssignEmpTRs where item.SegmentID == segment && item.EmployeeNo == user select item).ToList();

                foreach(var item in count)
                {
                    var job = (from data in _context.JobTRs where item.SegmentID == segment && item.JobID == item.JobID select item).ToList();

                }


                

                if (count.Count > 0)
                {
                    int result = count.Count;
                    return result;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int GetJobPending(int user, int segment)
        {
            using (var _context = new DatabaseContext())
            {
                var count = (from item in _context.JobTRs where item.SegmentID == segment && item.StatusID == 1 select item).ToList();

                if (count.Count > 0)
                {
                    int result = count.Count;
                    return result;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int GetJobOpen(int user, int segment)
        {
            using (var _context = new DatabaseContext())
            {
                var count = (from item in _context.JobTRs where item.SegmentID == segment && item.StatusID == 3 select item).ToList();

                if (count.Count > 0)
                {
                    int result = count.Count;
                    return result;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
