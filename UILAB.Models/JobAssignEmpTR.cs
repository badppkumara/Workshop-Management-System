using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UILAB.Models
{
    [Table("JobAssignEmpTR")]

    public class JobAssignEmpTR
    {
        public Nullable<int> SegmentID { get; set; }

        [Key]
        public int JobAssignEmpID { get; set; }
        public int JobID { get; set; }
        public Nullable<int> EmployeeNo { get; set; }
        public bool Flagged { get; set; }
        public Nullable<System.DateTime> LastModifyDate { get; set; }
        public Nullable<int> LastModifyUser { get; set; }
    }
}
