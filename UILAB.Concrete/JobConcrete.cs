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
    public class JobConcrete : IJob
    {
        #region ---> Job Status
        // Job Status
        public void InsertStatus(JobStatusTB statustb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.JobStatuses.Add(statustb);
                    _context.SaveChanges();
                    //int id = statustb.StatusID;
                    //return id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateStatus(JobStatusTB statustb)
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
                    var deleteitem = (from item in _context.JobStatuses
                                      where item.StatusID == statustb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        _context.JobStatuses.Remove(deleteitem);
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

        #endregion

        #region ---> Job Types
        // Stock Type
        public void InsertType(JobTypeTB typetb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.JobTypes.Add(typetb);
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateType(JobTypeTB typetb)
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
                    var deleteitem = (from item in _context.JobTypes
                                      where item.JobTypeID == typetb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        _context.JobTypes.Remove(deleteitem);
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

        #endregion

        #region ---> Job Packages
        // Task Type
        public int InsertPackage(JobPackageTB packagetb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.JobPackages.Add(packagetb);
                    _context.SaveChanges();
                    int id = packagetb.PackageID;
                    return id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdatePackage(JobPackageTB packagetb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(packagetb).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int DeletePackage(int packagetb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.JobPackages
                                      where item.PackageID == packagetb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        _context.JobPackages.Remove(deleteitem);
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

        #endregion

        #region ---> Job Package List
        // Task Type
        public void InsertPackageList(JobPackageListTB packagelisttb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.JobPackageLists.Add(packagelisttb);
                    _context.SaveChanges();
                    //int id = tasktyplistetb.JobTaskListID;
                    //return id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdatePackageList(JobPackageListTB packagelisttb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(packagelisttb).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int DeletePackageList(int packagelisttb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.JobPackageLists
                                      where item.PackageListID == packagelisttb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        _context.JobPackageLists.Remove(deleteitem);
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

        #endregion

        #region ---> Job Tasks

        public int InsertTasks(JobTasksTB taskstb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.JobTasksTBs.Add(taskstb);
                    _context.SaveChanges();
                    int id = taskstb.TaskID;
                    return id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateTasks(JobTasksTB taskstb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(taskstb).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int DeleteTasks(int taskstb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.JobTasksTBs
                                      where item.TaskID == taskstb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        _context.JobTasksTBs.Remove(deleteitem);
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

        #endregion

        #region ---> Job Task Transaction

        public void InsertTaskTR(JobTaskTR taskstrtb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.JobTaskTRs.Add(taskstrtb);
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateTaskTR(JobTaskTR taskstrtb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(taskstrtb).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int DeleteTaskTR(int taskstrtb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.JobTaskTRs
                                      where item.JobTaskTRID == taskstrtb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        _context.JobTaskTRs.Remove(deleteitem);
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

        #endregion

        #region ---> Jobs

        public int InsertJobTR(JobTR jobtrtb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.JobTRs.Add(jobtrtb);
                    _context.SaveChanges();
                    int id = jobtrtb.JobID;
                    return id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateJobTR(JobTR jobtrtb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(jobtrtb).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int DeleteJobTR(int jobtrtb, int user)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.JobTRs
                                      where item.JobID == jobtrtb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        var result = _context.JobTRs.SingleOrDefault(b => b.JobID == jobtrtb);

                        if (result != null)
                        {
                            result.StatusID = 5;
                            result.JobFinishDate = DateTime.Now;
                            result.LastModifyDate = DateTime.Now;
                            result.LastModifyUser = user;
                            _context.SaveChanges();
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

        #endregion

        #region ---> Job Assign Employee

        public void InsertJobEmployee(JobAssignEmpTR jobemptb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.JobAssignEmpTRs.Add(jobemptb);
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateJobEmployee(JobAssignEmpTR jobemptb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(jobemptb).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int DeleteJobEmployee(int jobemptb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.JobAssignEmpTRs
                                      where item.JobAssignEmpID == jobemptb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        _context.JobAssignEmpTRs.Remove(deleteitem);
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

        #endregion

        #region ---> Job Assign Employee

        public void InsertJobPart(JobTaskPartDetailTR jobparttb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.JobTaskPartDetailTRs.Add(jobparttb);
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateJobPart(JobTaskPartDetailTR jobparttb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(jobparttb).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int DeleteJobPart(int jobparttb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.JobTaskPartDetailTRs
                                      where item.TaskPartDetailID == jobparttb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        _context.JobTaskPartDetailTRs.Remove(deleteitem);
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

        #endregion
    }
}
