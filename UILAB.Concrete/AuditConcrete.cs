using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UILAB.Interface;
using UILAB.Models;

namespace UILAB.Concrete
{
    public class AuditConcrete : IAudit
    {
        public void InsertAuditData(AuditUserLog audittb)
        {
            using (var _context = new DatabaseContext())
            {
                _context.AuditUserLogs.Add(audittb);
                _context.SaveChanges();
            }
        }
    }
}
