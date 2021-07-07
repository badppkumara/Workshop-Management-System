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
    public class VehicleConcrete : IVehicle
    {
        #region ---> Vehicle
        // Vehicle
        public int InsertVehicle(VehicleTR vehicletb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.VehicleTRs.Add(vehicletb);
                    _context.SaveChanges();
                    int id = vehicletb.VehicleID;
                    return id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateVehicle(VehicleTR vehicletb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(vehicletb).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int DeleteVehicle(int vehicletb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.VehicleTRs
                                      where item.VehicleID == vehicletb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        _context.VehicleTRs.Remove(deleteitem);
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

        #region ---> Make
        // Make
        public int InsertMake(VehicleMakeTB maketb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.VehicleMakes.Add(maketb);
                    _context.SaveChanges();
                    int id = maketb.MakeID;
                    return id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateMake(VehicleMakeTB maketb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(maketb).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int DeleteMake(int maketb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.VehicleMakes
                                      where item.MakeID == maketb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        _context.VehicleMakes.Remove(deleteitem);
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

        public int InsertMakeImg(FileVehicle makeimgtb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.FileVehicles.Add(makeimgtb);
                    _context.SaveChanges();
                    int id = makeimgtb.FileID;
                    return id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateMakeImg(FileVehicle makeimgtb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(makeimgtb).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int DeleteMakeImg(int makeimgtb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.FileVehicles
                                      where item.FileID == makeimgtb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        _context.FileVehicles.Remove(deleteitem);
                        int result = _context.SaveChanges();

                        string ImageName = System.IO.Path.GetFileName(deleteitem.FileName);
                        string physicalPath = System.Web.HttpContext.Current.Server.MapPath("~/Files/Make/" + ImageName);
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

        #endregion

        #region ---> Model
        // Model
        public int InsertModel(VehicleModelTB modeltb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.VehicleModels.Add(modeltb);
                    _context.SaveChanges();
                    int id = modeltb.ModelID;
                    return id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateModel(VehicleModelTB modeltb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(modeltb).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int DeleteModel(int modeltb, int user)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.VehicleModels
                                      where item.ModelID == modeltb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        var update = (from updateitem in _context.VehicleMakes
                                      where updateitem.MakeID == deleteitem.MakeID
                                      select updateitem).SingleOrDefault();

                        if (update != null)
                        {
                            if (update.Flagged == true)
                            {
                                update.Flagged = false;
                                update.LastModifyDate = DateTime.Now;
                                update.LastModifyUser = user;
                                _context.SaveChanges();
                            }
                        }

                        _context.VehicleModels.Remove(deleteitem);
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

        #region ---> Model Type
        // Model Type
        public int InsertModelType(VehicleTypeTB modeltypetb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.VehicleTypes.Add(modeltypetb);
                    _context.SaveChanges();
                    int id = modeltypetb.ModelTypeID;
                    return id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateModelType(VehicleTypeTB modeltypetb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(modeltypetb).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int DeleteModelType(int modeltypetb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.VehicleTypes
                                      where item.ModelTypeID == modeltypetb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        _context.VehicleTypes.Remove(deleteitem);
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

        #region ---> Mileage
        // Mileage
        public int InsertMileage(VehicleMileageTR mileagetb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.VehicleMileageTRs.Add(mileagetb);
                    _context.SaveChanges();
                    int id = mileagetb.MileageID;
                    return id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateMileage(VehicleMileageTR mileagetb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(mileagetb).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int DeleteMilage(int mileagetb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.VehicleMileageTRs
                                      where item.MileageID == mileagetb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        _context.VehicleMileageTRs.Remove(deleteitem);
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

        #region ---> Driver
        // Mileage
        public int InsertDriver(VehicleDriverTR drivertb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    _context.VehicleDriverTRs.Add(drivertb);
                    _context.SaveChanges();
                    int id = drivertb.DriverID;
                    return id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateDriver(VehicleDriverTR drivertb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.Entry(drivertb).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public int DeleteDriver(int drivertb)
        {
            try
            {
                using (var _context = new DatabaseContext())
                {
                    var deleteitem = (from item in _context.VehicleDriverTRs
                                      where item.DriverID == drivertb
                                      select item).SingleOrDefault();

                    if (deleteitem != null)
                    {
                        _context.VehicleDriverTRs.Remove(deleteitem);
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
