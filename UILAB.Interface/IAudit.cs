using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UILAB.Models;

namespace UILAB.Interface
{
    public interface IAudit
    {
        void InsertAuditData(AuditUserLog audittb);
    }
}