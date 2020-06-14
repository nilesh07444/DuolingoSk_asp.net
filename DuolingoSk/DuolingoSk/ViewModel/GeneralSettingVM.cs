using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DuolingoSk
{
    public class GeneralSettingVM
    {
        public long GeneralSettingId { get; set; } 
        public string SMTPHost { get; set; }
        public string SMTPPort { get; set; }
        public bool? EnableSSL { get; set; }
        public string SMTPEmail { get; set; }
        public string SMTPPwd { get; set; }
        public string AdminSMSNumber { get; set; }
        public string AdminEmail { get; set; }
        public string FromEmail { get; set; }
        public decimal? RegistrationFee { get; set; }
        public decimal? RenewFee { get; set; }
        public int? TotalExamAttempt { get; set; }
    }
}