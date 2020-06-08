using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace DuolingoSk.Models
{
    public class clsCommon
    {
        private static string _securityKey = "DuolingoSk";

        public static string EncryptString(string PlainText)
        {
            byte[] toEncryptedArray = UTF8Encoding.UTF8.GetBytes(PlainText);
            MD5CryptoServiceProvider objMD5CryptoService = new MD5CryptoServiceProvider();
            byte[] securityKeyArray = objMD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(_securityKey));
            objMD5CryptoService.Clear();
            var objTripleDESCryptoService = new TripleDESCryptoServiceProvider();
            objTripleDESCryptoService.Key = securityKeyArray;
            objTripleDESCryptoService.Mode = CipherMode.ECB;
            objTripleDESCryptoService.Padding = PaddingMode.PKCS7;
            var objCrytpoTransform = objTripleDESCryptoService.CreateEncryptor();
            byte[] resultArray = objCrytpoTransform.TransformFinalBlock(toEncryptedArray, 0, toEncryptedArray.Length);
            objTripleDESCryptoService.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length).Replace("+", "_");
        }
        public static string DecryptString(string CipherText)
        {
            byte[] toEncryptArray = Convert.FromBase64String(CipherText.Replace("_", "+"));
            MD5CryptoServiceProvider objMD5CryptoService = new MD5CryptoServiceProvider();
            byte[] securityKeyArray = objMD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(_securityKey));
            objMD5CryptoService.Clear();
            var objTripleDESCryptoService = new TripleDESCryptoServiceProvider();
            objTripleDESCryptoService.Key = securityKeyArray;
            objTripleDESCryptoService.Mode = CipherMode.ECB;
            objTripleDESCryptoService.Padding = PaddingMode.PKCS7;
            var objCrytpoTransform = objTripleDESCryptoService.CreateDecryptor();
            byte[] resultArray = objCrytpoTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            objTripleDESCryptoService.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
        public static void SendEmail(string To, string from, string subject, string body)
        {
            try
            {
                DuolingoSk_Entities _db = new DuolingoSk_Entities();
                MailMessage mailMessage = new MailMessage(
                      from, // From field
                      To, // Recipient field
                     subject, // Subject of the email message
                      body // Email message body
           );
                mailMessage.From = new MailAddress(from, "Shopping & Saving");

                tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();
                string SMTPHost = objGensetting.SMTPHost;
                string SMTpPort = objGensetting.SMTPPort;
                string SMTPEMail = objGensetting.SMTPEmail;
                string SMTPPwd = objGensetting.SMTPPwd;
                string EnablSSL = "false";
                if (objGensetting.EnableSSL == true)
                {
                    EnablSSL = "true";
                }
                mailMessage.IsBodyHtml = true;
                // System.Net.Mail.MailMessage mailMessage = (System.Net.Mail.MailMessage)mailMsg;

                /* Setting should be kept somewhere so no need to 
                   pass as a parameter (might be in web.config)       */
                using (SmtpClient client = new SmtpClient(SMTPHost, Convert.ToInt32(SMTpPort)))
                {
                    // Configure the client
                    if (EnablSSL == "true")
                    {
                        client.EnableSsl = true;
                    }
                    else
                    {
                        client.EnableSsl = false;
                    }

                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(SMTPEMail, SMTPPwd);

                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    mailMessage.IsBodyHtml = true;
                    client.Send(mailMessage);
                }
            }
            catch (Exception e)
            {

            }
        }
         
        public static EmailMessageVM GetSampleEmailTemplate()
        {
            string HeaderContent = GetFileText("EmailTemplates\\Header.htm");
            string FooterContent = GetFileText("EmailTemplates\\Footer.htm");
            string BodyContent = GetFileText("EmailTemplates\\AccountBlocked.htm");

            string body = HeaderContent + BodyContent + FooterContent;
            string subject = "Test Email By Nilesh";
            string toEmail = "krutik.shah1310@gmail.com";

            return new EmailMessageVM()
            {
                SendEmailTo = { toEmail },
                Subject = subject,
                Body = body
            };
        }

        public static string GetFileText(string filePathName)
        {
            string dirPath = AppDomain.CurrentDomain.BaseDirectory;
            string FullPath = Path.Combine(dirPath, filePathName);

            return File.ReadAllText(FullPath);
        }

        public static void SendEmail2(EmailMessageVM emailModel)
        {

            string fromEmailID = string.Empty;
            string emailFromName = string.Empty;

            try
            {
                DuolingoSk_Entities _db = new DuolingoSk_Entities();
                tbl_GeneralSetting objGensetting = _db.tbl_GeneralSetting.FirstOrDefault();

                // as per the application run mode, assign proper from email address from configuration file.
                using (SmtpClient emailClient = new SmtpClient())
                {
                    emailClient.EnableSsl = objGensetting.EnableSSL == true ? true : false;
                    //emailClient.UseDefaultCredentials = false; 

                    emailClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                    emailClient.Host = objGensetting.SMTPHost;
                    emailClient.Port = Convert.ToInt32(objGensetting.SMTPPort);
                    emailClient.Credentials = new NetworkCredential(
                        objGensetting.SMTPEmail,
                        objGensetting.SMTPPwd
                    );
                    fromEmailID = objGensetting.SMTPEmail;
                    emailFromName = "Shopping & Saving";


                    //Create SMTP message
                    MailMessage message = new MailMessage
                    {
                        From = new MailAddress(fromEmailID, emailFromName),
                        Subject = emailModel.Subject,
                        Body = emailModel.Body,
                        IsBodyHtml = true,
                        BodyEncoding = Encoding.UTF8
                    };

                    emailModel.SendEmailTo.ForEach(x => message.To.Add(x));
                    emailModel.SendEmailCc.ForEach(x => message.CC.Add(x));
                    emailModel.SendEmailBcc.ForEach(x => message.Bcc.Add(x));

                    //Send message.
                    emailClient.Timeout = 10000;
                    emailClient.Send(message);
                }

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

        }

    } 
}