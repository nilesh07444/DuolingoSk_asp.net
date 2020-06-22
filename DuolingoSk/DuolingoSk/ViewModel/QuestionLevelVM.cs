using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DuolingoSk
{
    public class QuestionLevelVM
    {
        public long Level_Id { get; set; }
        [Required]
        [MaxLength(50), Display(Name = "Level Name *")]
        public string LevelName { get; set; }
    }
}