using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuolingoSk
{
    public class ExamVM
    {
        public long Exam_Id { get; set; }
        public long? StudentId { get; set; }
        public DateTime? ExamDate { get; set; }
        public long? AgentId { get; set; }
        public string Score { get; set; }
        public string ResultText { get; set; } 
        public long? QuestionLevelId { get; set; }
        public int? ResultStatus { get; set; }

        // other fields
        public string StudentName { get; set; }
        public string AgentName { get; set; }
        public string LevelName { get; set; } 
    }
}