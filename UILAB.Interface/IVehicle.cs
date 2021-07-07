using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UILAB.Models;

namespace UILAB.Interface
{
    public interface IVehicle
    {
        // Vehicle
        int InsertVehicle(VehicleTR vehicletb);
        void UpdateVehicle(VehicleTR vehicletb);
        int DeleteVehicle(int vehicletb);

        // Make
        int InsertMake(VehicleMakeTB maketb);
        void UpdateMake(VehicleMakeTB maketb);
        int DeleteMake(int maketb);

        // Make Image
        int InsertMakeImg(FileVehicle makeimgtb);
        void UpdateMakeImg(FileVehicle makeimgtb);
        int DeleteMakeImg(int makeimgtb);

        // Model
        int InsertModel(VehicleModelTB modeltb);
        void UpdateModel(VehicleModelTB modeltb);
        int DeleteModel(int modeltb, int user);

        // Model Types
        int InsertModelType(VehicleTypeTB modeltypetb);
        void UpdateModelType(VehicleTypeTB modeltypetb);
        int DeleteModelType(int modeltypetb);

        // Mileage
        int InsertMileage(VehicleMileageTR mileagetb);
        void UpdateMileage(VehicleMileageTR mileagetb);
        int DeleteMilage(int mileagetb);

        // Driver
        int InsertDriver(VehicleDriverTR drivertb);
        void UpdateDriver(VehicleDriverTR drivertb);
        int DeleteDriver(int drivertb);
    }
}