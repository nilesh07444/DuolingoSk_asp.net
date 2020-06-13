using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuolingoSk.ViewModel
{
    public class QuestionVM
    {
        public long QuestionId { get; set; }
        public string QuestionText { get; set; }
        public int QuestionTime { get; set; }
        public int QuestionTypeId { get; set; }
        public string QuestionOptionText { get; set; }
        public string Words { get; set; }
        public string Mp3FileName { get; set; }
        public string Mp3FileText { get; set; }
        public int MaxReplay { get; set; }
        public string ImageName { get; set; }
        public int NoOfWords { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public string QuestionTypeText { get; set; }
        public List<Mp3OptionsVM> Mp3Options { get; set; }
    }
}