using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UILAB.Models;

namespace UILAB.Interface
{
    public interface IEmp
    {
        // WMS
        int GetJobPending(int user, int segment);
        int GetJobOpen(int user, int segment);
        int GetJobComplete(int user, int segment);
        int GetVehiclesCount(int segment);

        // Users
        int GetEmployeesCount(int segment);
        int GetCustomersCount(int segment);
        int GetSuppliersCount(int segment);
    }
}