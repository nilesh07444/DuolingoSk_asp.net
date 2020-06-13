using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuolingoSk.ViewModel
{
    public class Mp3OptionsVM
    {
        public long Mp3OptionId { get; set; }
        public long QuestionId { get; set; }
        public string Mp3FileName { get; set; }
        public string Mp3Text { get; set; }
        public bool IsCorrect { get; set; }
    }
}