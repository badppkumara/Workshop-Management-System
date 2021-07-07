using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UILAB.Models;

namespace UILAB.Interface
{
    public interface IJob
    {
        // Job Status
        void InsertStatus(JobStatusTB statustb);
        void UpdateStatus(JobStatusTB statustb);
        int DeleteStatus(int statustb);

        // Job Types
        void InsertType(JobTypeTB typetb);
        void UpdateType(JobTypeTB typetb);
        int DeleteType(int typetb);

        // Job Packages
        int InsertPackage(JobPackageTB packagetb);
        void UpdatePackage(JobPackageTB packagetb);
        int DeletePackage(int packagetb);

        // Job Task Type List
        void InsertPackageList(JobPackageListTB packagelisttb);
        void UpdatePackageList(JobPackageListTB packagelisttb);
        int DeletePackageList(int packagelisttb);

        // Job Tasks
        int InsertTasks(JobTasksTB taskstb);
        void UpdateTasks(JobTasksTB taskstb);
        int DeleteTasks(int taskstb);

        // Job Task Transaction
        void InsertTaskTR(JobTaskTR taskstrtb);
        void UpdateTaskTR(JobTaskTR taskstrtb);
        int DeleteTaskTR(int taskstrtb);

        // Jobs
        int InsertJobTR(JobTR jobtrtb);
        void UpdateJobTR(JobTR jobtrtb);
        int DeleteJobTR(int jobtrtb, int user);

        // Job Employee
        void InsertJobEmployee(JobAssignEmpTR jobemptb);
        void UpdateJobEmployee(JobAssignEmpTR jobemptb);
        int DeleteJobEmployee(int jobemptb);

        // Job Parts
        void InsertJobPart(JobTaskPartDetailTR jobparttb);
        void UpdateJobPart(JobTaskPartDetailTR jobparttb);
        int DeleteJobPart(int jobparttb);
    }
}