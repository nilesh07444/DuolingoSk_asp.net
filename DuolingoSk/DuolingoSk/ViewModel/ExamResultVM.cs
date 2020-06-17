using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuolingoSk.ViewModel
{
    public class ExamResultVM
    {
        public int QueId { get; set; }
        public string QuestionTxt { get; set; }
        public int QuestionType { get; set; }
        public string Que { get; set; }
        public string Ans { get; set; }
    }
}