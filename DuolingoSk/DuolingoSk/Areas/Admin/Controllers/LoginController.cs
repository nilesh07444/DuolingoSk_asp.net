using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DuolingoSk.Helper;
using DuolingoSk.Model;
using DuolingoSk.Models;

namespace DuolingoSk.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        private readonly DuolingoSk_Entities _db;
        public LoginController()
        {
            _db = new DuolingoSk_Entities();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(LoginVM userLogin)
        {
            try
            {
                string EncyptedPassword = userLogin.Password; // Encrypt(userLogin.Password);

                var data = _db.tbl_AdminUsers.Where(x => x.MobileNo == userLogin.MobileNo && x.Password == EncyptedPassword && !x.IsDeleted).FirstOrDefault();

                if (data != null)
                {


                    if (!data.IsActive)
                    {
                        TempData["LoginError"] = "Your Account is not active. Please contact administrator.";
                        return View();
                    }


                    clsAdminSession.SessionID = Session.SessionID;
                    clsAdminSession.UserID = data.AdminUserId;
                    clsAdminSession.RoleID = data.AdminRoleId;
                    
                    clsAdminSession.UserName = data.FirstName + " " + data.LastName;
                    clsAdminSession.ImagePath = data.ProfilePicture;
                    clsAdminSession.MobileNumber = data.MobileNo;

                    if(clsAdminSession.RoleID == (int)AdminRoles.AdminUser)
                    {
                        clsAdminSession.RoleName = "Super Admin";
                    }
                    else
                    {
                        clsAdminSession.RoleName = "Agent";
                    }

                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    TempData["LoginError"] = "Invalid Mobile or Password";
                    return View();
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

            return View();
        }

        public string SendOTP(string MobileNumber)
        {
            try
            {
                tbl_AdminUsers objtbl_AdminUsers = _db.tbl_AdminUsers.Where(o => o.MobileNo.ToLower() == MobileNumber.ToLower() && !o.IsDeleted).FirstOrDefault();
                if (objtbl_AdminUsers == null)
                {
                    return "NotExist";
                }
                if (!objtbl_AdminUsers.IsActive)
                {
                    return "InActiveAccount";
                }

                using (WebClient webClient = new WebClient())
                {
                    Random random = new Random();
                    int num = random.Next(555555, 999999);
                    string msg = "Your Otp code for Login is " + num;
                    string url = "http://sms.unitechcenter.com/sendSMS?username=skacademy&message=" + msg + "&sendername=SKANAD&smstype=TRANS&numbers=" + MobileNumber + "&apikey=0b9c3015-bbcd-4ad8-b9ac-30f28451ebe6";
                    var json = webClient.DownloadString(url);
                    if (json.Contains("invalidnumber"))
                    {
                        return "InvalidNumber";
                    }
                    else
                    {
                        string msg1 = "Your Otp code for Login is " + num;
                        return num.ToString();

                    }

                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }
        public ActionResult Signout()
        {
            clsAdminSession.SessionID = "";
            clsAdminSession.UserID = 0;
            return RedirectToAction("Index");
        }
    }
}