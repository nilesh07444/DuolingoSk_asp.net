//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DuolingoSk.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbl_StudentFee
    {
        public int StudentFeeId { get; set; }
        public int StudentId { get; set; }
        public string FeeStatus { get; set; }
        public decimal FeeAmount { get; set; }
        public int TotalExamAttempt { get; set; }
        public bool IsDeleted { get; set; }
        public System.DateTime RequestedDate { get; set; }
        public Nullable<System.DateTime> MarkCompleteDate { get; set; }
        public Nullable<int> MarkCompleteBy { get; set; }
        public Nullable<bool> IsAttemptUsed { get; set; }
    }
}
