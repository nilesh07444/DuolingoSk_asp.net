using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuolingoSk
{
    public class StudentWebinarVM
    {
        public long StudentWebinarId { get; set; }
        public int WebinarId { get; set; }
        public long StudentId { get; set; }
        public long StudentFeeId { get; set; }
        public long PackageId { get; set; }
        public DateTime CreatedDate { get; set; }

        // 
        public string StudentName { get; set; }
        public string MobileNo { get; set; }
        public decimal TotalWebinarUsed { get; set; }

    }
}