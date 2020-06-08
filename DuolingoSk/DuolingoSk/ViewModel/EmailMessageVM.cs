using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuolingoSk
{
    public class EmailMessageVM
    {
        public EmailMessageVM()
        {
            this.SendEmailTo = new List<string>();
            this.SendEmailCc = new List<string>();
            this.SendEmailBcc = new List<string>();
        }
        public List<string> SendEmailTo { get; private set; }
        public List<string> SendEmailCc { get; private set; }
        public List<string> SendEmailBcc { get; private set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string CommaSeperatedEmailToList
        {
            get
            {
                return string.Join(",", this.SendEmailTo);
            }
        }
    }
}