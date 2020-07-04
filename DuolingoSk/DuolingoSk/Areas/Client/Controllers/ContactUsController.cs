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

                string bodyMsg = "";

                bodyMsg += "Hi Admin, \n";
                bodyMsg += "We have received contact inquiry from below details: \n\n";

                bodyMsg += "Name: " + Name + " \n";
                bodyMsg += "Email Id: " + EmailId + " \n";
                bodyMsg += "Mobile No: " + MobileNo + " \n";
                bodyMsg += "Message: " + Message + " \n\n";

                bodyMsg += "Thanks, \n";
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