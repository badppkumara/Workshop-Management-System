using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UILAB.Models;

namespace UILAB.Interface
{
    public interface IAdmin
    {
        // WMS
        int GetJobPending(int segment);
        int GetJobOpen(int segment);
        int GetJobComplete(int segment);
        int GetVehiclesCount(int segment);

        // Users
        int GetEmployeesCount(int segment);
        int GetCustomersCount(int segment);
        int GetSuppliersCount(int segment);
    }
}