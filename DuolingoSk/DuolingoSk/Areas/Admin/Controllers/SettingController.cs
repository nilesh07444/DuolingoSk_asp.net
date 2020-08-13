using DuolingoSk.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DuolingoSk.Models;

namespace DuolingoSk.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class SettingController : Controller
    {
        private readonly DuolingoSk_Entities _db;
        public SettingController()
        {
            _db = new DuolingoSk_Entities();
        }
        public ActionResult Index()
        {

            GeneralSettingVM objGenSetting = (from s in _db.tbl_GeneralSetting
                                              select new GeneralSettingVM
                                              {
                                                  GeneralSettingId = s.GeneralSettingId,
                                                  SMTPHost = s.SMTPHost,
                                                  SMTPPort = s.SMTPPort,
                                                  EnableSSL = s.EnableSSL,
                                                  SMTPEmail = s.SMTPEmail,
                                                  SMTPPwd = s.SMTPPwd,
                                                  AdminSMSNumber = s.AdminSMSNumber,
                                                  AdminEmail = s.AdminEmail,
                                                  FromEmail = s.FromEmail, 
                                              }).FirstOrDefault();

            return View(objGenSetting);
        }

        [HttpPost]
        public string SaveGeneralSetting(FormCollection frm)
        {
            string ReturnMessage = "";
            try
            {

                string txtSmtpHost = frm["txtSmtpHost"];
                string txtSmtpPort = frm["txtSmtpPort"];
                string txtSmtpEmail = frm["txtSmtpEmail"];
                string txtSmtpPwd = frm["txtSmtpPwd"];
                string txtadminemail = frm["txtadminemail"];
                string txtfrommail = frm["txtfrommail"];
                string txtsmsmobil = frm["txtsmsmobil"];
                bool EnableSSL = false;

                decimal txtRegistrationFee = Convert.ToDecimal(frm["txtRegistrationFee"]);
                decimal txtRenewFee = Convert.ToDecimal(frm["txtRenewFee"]);
                int txtTotalExamAttempt = Convert.ToInt32(frm["txtTotalExamAttempt"]);

                int txtMaxQuestionLevel = Convert.ToInt32(frm["txtMaxQuestionLevel"]);
                int txtFeeExpiryInDays = Convert.ToInt32(frm["txtFeeExpiryInDays"]);

                if (frm["rdbSSL"].ToString() == "Yes")
                {
                    EnableSSL = true;
                }

                tbl_GeneralSetting objGenSetting = _db.tbl_GeneralSetting.FirstOrDefault();

                objGenSetting.SMTPEmail = txtSmtpEmail;
                objGenSetting.SMTPHost = txtSmtpHost;
                objGenSetting.SMTPPort = txtSmtpPort;
                objGenSetting.SMTPPwd = txtSmtpPwd;
                objGenSetting.AdminEmail = txtadminemail;
                objGenSetting.AdminSMSNumber = txtsmsmobil.ToString();
                objGenSetting.FromEmail = txtfrommail;
                objGenSetting.EnableSSL = EnableSSL; 

                _db.SaveChanges();
                return "Success";
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