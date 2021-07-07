using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UILAB.Models;

namespace UILAB.Interface
{
    public interface ILogin
    {
        UserSecurity ValidateAdmin(string username, string password);
        EmployeeMaster ValidateEmployeeName(string username, string password);
        EmployeeMaster ValidateEmployeeEmail(string email, string password);
        //bool UpdatePassword(string NewPassword, int UserID);
        //string GetPasswordbyUserID(int UserID);
    }
}
