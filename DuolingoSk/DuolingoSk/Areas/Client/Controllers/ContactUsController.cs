using DuolingoSk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DuolingoSk.Areas.Client.Controllers
{
    public class ContactUsController : Controller
    {
        private readonly DuolingoSk_Entities _db;

        public ContactUsController()
        {
            _db = new DuolingoSk_Entities();
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string SubmitContactForm(FormCollection frm)
        {
            string ReturnMessage = "";
            try
            {
                string Name = frm["name"];
                string EmailId = frm["emailid"];
                string MobileNo = frm["mobileno"];
                string Message = frm["message"];

                // Save in DB
                tbl_ContactForm objContact = new tbl_ContactForm();
                objContact.FullName = Name;
                objContact.EmailId = EmailId;
                objContact.MobileNo = MobileNo;
                objContact.Message = Message;
                objContact.CreatedDate = DateTime.UtcNow;
                _db.tbl_ContactForm.Add(objContact);
                _db.SaveChanges();

                ReturnMessage = "SUCCESS";

                /*
                string bodyMsg = "";

                bodyMsg += "Hi Admin, \r\n";
                bodyMsg += "We have received contact inquiry from below details: \r\n\r\n";

                bodyMsg += "Name: " + Name + " \r\n";
                bodyMsg += "Email Id: " + EmailId + " \r\n";
                bodyMsg += "Mobile No: " + MobileNo + " \r\n";
                bodyMsg += "Message: " + Message + " \r\n\r\n";

                bodyMsg += "Thanks, <br>";
                bodyMsg += "Duolingo Sk";

                tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();

                bool IsSentEmail = clsCommon.SendEmail(objGensetting.AdminEmail, objGensetting.SMTPEmail, "Contact Form - Duolingo Sk", bodyMsg);

                if (IsSentEmail)
                {
                    ReturnMessage = "SUCCESS";
                }
                else
                {
                    ReturnMessage = "ERROR";
                }
                */
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                ReturnMessage = "ERROR";
            }

            return ReturnMessage;
        }


    }
}