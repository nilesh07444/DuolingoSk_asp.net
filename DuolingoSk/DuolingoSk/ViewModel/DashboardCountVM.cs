using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuolingoSk
{
    public class DashboardCountVM
    {
        public int TotalAgents { get; set; }
        public int TotalStudents { get; set; }
        public decimal TotalPendingFees { get; set; }
        public int TotalPendingExams { get; set; }        

    }
}