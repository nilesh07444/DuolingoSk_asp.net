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
    
    public partial class tbl_ExamResultDetails
    {
        public long ExamResultDetailId { get; set; }
        public Nullable<long> ExamId { get; set; }
        public Nullable<long> StudentId { get; set; }
        public string QuestionText { get; set; }
        public string QuestionOptionValue { get; set; }
        public Nullable<int> QuestionTypeId { get; set; }
        public string QuestionOptiont_Ans { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
}
