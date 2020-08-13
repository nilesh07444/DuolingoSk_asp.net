using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DuolingoSk.Model;
using DuolingoSk.Models;

namespace DuolingoSk.Areas.Client.Controllers
{
    public class StudentLoginController : Controller
    {
        private readonly DuolingoSk_Entities _db;
        public StudentLoginController()
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
                    string msg = "Your Otp code for Login is " + num;
                    string url = "http://sms.unitechcenter.com/sendSMS?username=skacademy&message=" + msg + "&sendername=SKANAD&smstype=TRANS&numbers=" + objClientUsr.MobileNo + "&apikey=0b9c3015-bbcd-4ad8-b9ac-30f28451ebe6";
                    var json = webClient.DownloadString(url);
                    if (json.Contains("invalidnumber"))
                    {
                        return "InvalidNumber";
                    }
                    else
                    {
                        //tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
                        //string FromEmail = objGensetting.FromEmail;
                        //string msg1 = "Your Otp code for Login is " + num;
                        //try
                        //{
                        //    clsCommon.SendEmail(objClientUsr.Email, FromEmail, "OTP Code for Login - Duolingo Sk", msg1);
                        //}
                        //catch (Exception e)
                        //{
                        //    string ErrorMessage = e.Message.ToString();
                        //}

                        return num.ToString();

                    }

                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        public ActionResult CheckLogin(FormCollection frm)
        {
            string mobilenumber = frm["emailmobile"];
            string password = frm["password"];
            try
            {
                string EncyptedPassword = password; // clsCommon.EncryptString(password); // Encrypt(userLogin.Password);                
                var data = _db.tbl_Students.Where(x => (x.Email.ToLower() == mobilenumber.ToLower() || x.MobileNo.ToLower() == mobilenumber.ToLower()) && x.Password == EncyptedPassword && !x.IsDeleted).FirstOrDefault();

                if (data != null)
                {

                    if (!data.IsActive)
                    {
                        TempData["LoginError"] = "Your Account is not active. Please contact administrator.";
                        return RedirectToAction("Index");
                    }

                    clsClientSession.SessionID = Session.SessionID;
                    clsClientSession.UserID = data.StudentId; 
                    clsClientSession.FullName = data.FullName; 
                    clsClientSession.ImagePath = data.ProfilePicture;
                    clsClientSession.Email = data.Email;
                    clsClientSession.MobileNumber = data.MobileNo;
                    clsClientSession.IsDemoUsed = data.IsDemoUsed.HasValue ? data.IsDemoUsed.Value : false;
                    return RedirectToAction("Index", "MyExams");

                }
                else
                {
                    TempData["LoginError"] = "Invalid username or password";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                TempData["LoginError"] = ErrorMessage;
            }

            return RedirectToAction("Index");

        }

        public ActionResult Signout()
        {
            clsClientSession.SessionID = "";
            clsClientSession.UserID = 0;
            clsClientSession.FullName = ""; 
            clsClientSession.Email = "";
            Session.RemoveAll();
            Session.Clear();
            Session.Abandon();
            string GuidNew = Guid.NewGuid().ToString();
            Response.Cookies["sessionkeyval"].Value = GuidNew;
            Response.Cookies["sessionkeyval"].Expires = DateTime.Now.AddDays(30);
            return RedirectToAction("Index", "StudentLogin");
        }

    }
}