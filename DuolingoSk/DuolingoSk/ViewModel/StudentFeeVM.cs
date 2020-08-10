using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuolingoSk
{
    public class StudentFeeVM
    {
        public int StudentFeeId { get; set; }
        public int StudentId { get; set; }
        public string FeeStatus { get; set; }
        public decimal FeeAmount { get; set; }
        public int TotalExamAttempt { get; set; }
        public int? TotalWebinarAttempt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime? MarkCompleteDate { get; set; }
        public int? MarkCompleteBy { get; set; }

        // Additional Fields
        public string StudentName { get; set; }
        public string AgentName { get; set; }
        public string MarkCompleteByUserName { get; set; }
        public DateTime? FeeExpiryDate { get; set; }
        public bool? IsAttemptUsed { get; set; }
        public int UsedTotalAttempts { get; set; }

    }
}