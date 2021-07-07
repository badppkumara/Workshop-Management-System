using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UILAB.Models;

namespace UILAB.Interface
{
    public interface IApproval
    {
        // Approval Scheme
        void InsertScheme(ApprovalSchemeMaster approvaltb);
        void UpdateScheme(ApprovalSchemeMaster approvaltb);
        int DeleteScheme(int approvaltb);

        // Approval Group
        void InsertGroup(ApprovalGroupTR grouptb);
        void UpdateGroup(ApprovalGroupTR grouptb);
        int DeleteGroup(int grouptb);

        // Approval Header
        int InsertHeader(ApprovalHeaderTR appheadertb);
        void UpdateHeader(ApprovalHeaderTR appheadertb);
        int DeleteHeader(int appheadertb);

        // Approval Detail
        void InsertDetail(ApprovalDetailTR appdetailtb);
        void UpdateDetail(ApprovalDetailTR appdetailtb);
        int DeleteDetail(int appdetailtb);
    }
}