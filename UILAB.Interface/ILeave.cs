using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UILAB.Models;

namespace UILAB.Interface
{
    public interface ILeave
    {
        // Leave Scheme
        void InsertScheme(UserSecurity admintb);
        void UpdateScheme(UserSecurity admintb);
        int DeleteScheme(int admintb, int segment);

        // Leave Types
        int InsertTypes(EmployeeMaster employeetb);
        void UpdateTypes(EmployeeMaster employeetb);
        int DeleteTypes(int employeetb, int segment);

        // Leave Scheme Items
        int InsertSchemeItem(CustomerTB customertb);
        void UpdateSchemeItem(CustomerTB customertb);
        int DeleteSchemeItem(int customertb, int segment);

    }
}