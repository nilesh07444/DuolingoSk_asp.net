using DuolingoSk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DuolingoSk.Areas.Client.Controllers
{
    public class ForgetPasswordController : Controller
    {
        private readonly DuolingoSk_Entities _db;
        public ForgetPasswordController()
        {
            _db = new DuolingoSk_Entities();
        }
        public ActionResult Index()
        {
            return View();
        }

        public string SendOTP(string MobileNumber)
        {
            try
            {
                tbl_Students objClientUsr = _db.tbl_Students.Where(o => (o.Email.ToLower() == MobileNumber || o.MobileNo.ToLower() == MobileNumber.ToLower()) && !o.IsDeleted).FirstOrDefault();
                if (objClientUsr == null)
                {
                    return "NotExist";
                }
                if (!objClientUsr.IsActive)
                {
                    return "InActiveAccount";
                }

                using (WebClient webClient = new WebClient())
                {
                    Random random = new Random();
                    int num = random.Next(555555, 999999);
                    string msg = "Your Otp code for Forget Password is " + num;
                    string url = "http://sms.unitechcenter.com/sendSMS?username=skacademy&message=" + msg + "&sendername=SKANAD&smstype=TRANS&numbers=" + objClientUsr.MobileNo + "&apikey=0b9c3015-bbcd-4ad8-b9ac-30f28451ebe6";
                    var json = webClient.DownloadString(url);
                    if (json.Contains("invalidnumber"))
                    {
                        return "InvalidNumber";
                    }
                    else
                    {
                        return num.ToString();
                    }

                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        public string ResetNewPassword(string MobileNumber, string NewPassword)
        {
            string message = string.Empty;

            try
            {
                tbl_Students objClientUsr = _db.tbl_Students.Where(o => o.MobileNo.ToLower() == MobileNumber.ToLower()).FirstOrDefault();
                if (objClientUsr != null)
                {
                    objClientUsr.Password = NewPassword;
                    _db.SaveChanges();

                    message = "Success";
                } 
            }
            catch (Exception ex)
            {
                message = "error";
            }

            

            return message;

        }

    }
}