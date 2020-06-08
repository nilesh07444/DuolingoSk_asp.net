using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuolingoSk
{
    public class PwdVM
    {
        public string NewPassword { get; set; }
        public string OldPassword { get; set; }
        public string MobileNumber { get; set; }
        public string UserType { get; set; }
        public string ClientUserId { get; set; }
    }
}