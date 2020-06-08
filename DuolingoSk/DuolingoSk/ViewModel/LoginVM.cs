using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DuolingoSk
{
    public class LoginVM
    {
        [Required]
        public string MobileNo { get; set; }

        [Required]
        public string Password { get; set; }

        public string SessionUniqueId { get; set; }
    }
}