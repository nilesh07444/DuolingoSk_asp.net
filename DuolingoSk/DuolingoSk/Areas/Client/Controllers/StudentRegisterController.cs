using DuolingoSk.Helper;
using DuolingoSk.Model;
using DuolingoSk.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DuolingoSk.Areas.Client.Controllers
{
    public class StudentRegisterController : Controller
    {
        private readonly DuolingoSk_Entities _db;
        public string UserProfileDirectoryPath = "";

        public StudentRegisterController()
        {
            _db = new DuolingoSk_Entities();
            UserProfileDirectoryPath = ErrorMessage.UserProfileDirectoryPath;
        }

        public ActionResult Index()
        {
            ViewData["tbl_Packages"] = _db.tbl_Package.Where(o => o.IsActive == true && o.IsDeleted == false).ToList();
            StudentVM objStudent = new StudentVM(); 
            return View(objStudent);
        }

        [HttpPost]
        public ActionResult Index(StudentVM userVM, HttpPostedFileBase ProfilePictureFile,FormCollection frm)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                     
                    #region Validation

                    // Validate duplicate MobileNo 
                    tbl_Students duplicateMobile = _db.tbl_Students.Where(x => x.MobileNo.ToLower() == userVM.MobileNo && !x.IsDeleted).FirstOrDefault();
                    if (duplicateMobile != null)
                    {
                        ModelState.AddModelError("MobileNo", ErrorMessage.MobileNoExists);
                        return View(userVM);
                    }

                    tbl_Students duplicateEmail = _db.tbl_Students.Where(x => x.Email.ToLower() == userVM.Email.ToLower() && !x.IsDeleted).FirstOrDefault();
                    if (duplicateMobile != null)
                    {
                        ModelState.AddModelError("Email", ErrorMessage.EmailExists);
                        return View(userVM);
                    }

                    

                    string fileName = string.Empty;
                    string path = Server.MapPath(UserProfileDirectoryPath);

                    bool folderExists = Directory.Exists(path);
                    if (!folderExists)
                        Directory.CreateDirectory(path);

                    if (ProfilePictureFile != null)
                    {
                        string ext = Path.GetExtension(ProfilePictureFile.FileName);
                        string f_name = Path.GetFileNameWithoutExtension(ProfilePictureFile.FileName);

                        fileName = f_name + "-" + Guid.NewGuid() + ext;
                        ProfilePictureFile.SaveAs(path + fileName);
                    }
                    else
                    {
                        fileName = userVM.ProfilePicture;
                    }

                    #endregion Validation

                    #region CreateUser

                    tbl_Students objStudent = new tbl_Students();
                     
                    objStudent.FirstName = userVM.FirstName;
                    objStudent.LastName = userVM.LastName;
                    objStudent.Email = userVM.Email;
                    objStudent.MobileNo = userVM.MobileNo;
                    objStudent.Password = userVM.Password;
                    objStudent.Address = userVM.Address;
                    objStudent.City = userVM.City;
                    objStudent.Remarks = userVM.Remarks;
                    objStudent.ProfilePicture = fileName;

                    if (!string.IsNullOrEmpty(userVM.Dob))
                    {
                        DateTime exp_Dob = DateTime.ParseExact(userVM.Dob, "dd/MM/yyyy", null);
                        objStudent.Dob = exp_Dob;
                    }

                    objStudent.IsActive = true;
                    objStudent.IsDeleted = false;
                    objStudent.CreatedDate = DateTime.UtcNow; 
                    _db.tbl_Students.Add(objStudent);
                    _db.SaveChanges();

                    #endregion CreateUser

                    #region SendNotification

                    // Send Notification to User
                    string SmsResponse = SendSMSOfCreateUser(userVM);

                    #endregion SendNotification

                    #region PendingFeeEntry
                    int PackageId = Convert.ToInt32(frm["Package"]);
                    tbl_GeneralSetting objSetting = _db.tbl_GeneralSetting.FirstOrDefault();

                    DateTime exp_date = DateTime.UtcNow.AddDays(365); // default 365 days
                    if (objSetting.FeeExpiryInDays != null && objSetting.FeeExpiryInDays > 0)
                    {
                        exp_date = DateTime.UtcNow.AddDays(Convert.ToInt32(objSetting.FeeExpiryInDays));
                    }
                    if (PackageId > 0)
                    {
                        var objPckg = _db.tbl_Package.Where(o => o.PackageId == PackageId).FirstOrDefault();
                        if (objPckg != null)
                        {
                            string refralcode = Convert.ToString(frm["refrealcode"]);
                            int copupnid = 0;
                            var objcpcode = _db.tbl_CouponCode.Where(o => o.CouponCode == refralcode).FirstOrDefault();
                            decimal disc = 0;
                            if (objcpcode != null)
                            {
                                disc = (Convert.ToDecimal(objPckg.PackageAmount) * objcpcode.DiscountPercentage.Value) / 100;
                                copupnid = Convert.ToInt32(objcpcode.CouponCodeId);
                            }
                            tbl_StudentFee objStudentFee = new tbl_StudentFee();
                            objStudentFee.StudentId = objStudent.StudentId;
                            objStudentFee.FeeStatus = "Complete";
                            objStudentFee.FeeAmount = Math.Round(Convert.ToDecimal(objPckg.PackageAmount) - disc, 2);
                            objStudentFee.TotalExamAttempt = Convert.ToInt32(objPckg.TotalAttempt);
                            objStudentFee.FeeExpiryDate = exp_date;
                            objStudentFee.IsDeleted = false;
                            objStudentFee.RequestedDate = DateTime.UtcNow;
                            objStudentFee.MarkCompleteBy = objStudent.StudentId;
                            objStudentFee.MarkCompleteDate = DateTime.UtcNow;
                            objStudentFee.IsAttemptUsed = false;
                            objStudentFee.PackageId = objPckg.PackageId;
                            objStudentFee.PackageName = objPckg.PackageName;
                            objStudentFee.CouponCode = refralcode;
                            objStudentFee.CouponId = copupnid;
                            _db.tbl_StudentFee.Add(objStudentFee);
                            _db.SaveChanges();


                        }
                    }                 

                    #endregion PendingFeeEntry

                    // Set login details 
                    clsClientSession.SessionID = Session.SessionID;
                    clsClientSession.UserID = objStudent.StudentId;
                    clsClientSession.FirstName = objStudent.FirstName;
                    clsClientSession.LastName = objStudent.LastName;
                    clsClientSession.ImagePath = objStudent.ProfilePicture;
                    clsClientSession.Email = objStudent.Email;
                    clsClientSession.MobileNumber = objStudent.MobileNo;

                    return RedirectToAction("Index", "MyExams");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

            return View(userVM);
        }

        public string SendSMSOfCreateUser(StudentVM userVM)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    WebClient client = new WebClient();

                    string msg = "Hello " + userVM.FirstName + "\n\n";
                    msg += "You are student of Duolingo sk." + "\n\n";

                    msg += "Below are login details:" + "\n";
                    msg += "Mobile No:" + userVM.MobileNo + "\n";
                    msg += "Password:" + userVM.Password + "\n\n";

                    msg += "Regards," + "\n";
                    msg += "Duolingo Sk";

                    string url = "http://sms.unitechcenter.com/sendSMS?username=skacademy&message=" + msg + "&sendername=SKANAD&smstype=TRANS&numbers=" + userVM.MobileNo + "&apikey=0b9c3015-bbcd-4ad8-b9ac-30f28451ebe6";

                    var json = webClient.DownloadString(url);
                    if (json.Contains("invalidnumber"))
                    {
                        return "InvalidNumber";
                    }
                    else
                    {
                        return "sucess";
                    }

                }
            }
            catch (WebException ex)
            {
                return ex.Message.ToString();
            }
        }

        public string SendOTP(string MobileNumber)
        {
            try
            {
                tbl_Students duplicateMobile = _db.tbl_Students.Where(x => x.MobileNo.ToLower() == MobileNumber && !x.IsDeleted).FirstOrDefault();
                if (duplicateMobile != null)
                {
                    return "Mobile Number Already Exist";
                }

                using (WebClient webClient = new WebClient())
                {
                    Random random = new Random();
                    int num = random.Next(555555, 999999);
                    string msg = "Your Otp code for Registration is " + num;
                    string url = "http://sms.unitechcenter.com/sendSMS?username=skacademy&message=" + msg + "&sendername=SKANAD&smstype=TRANS&numbers=" + MobileNumber + "&apikey=0b9c3015-bbcd-4ad8-b9ac-30f28451ebe6";
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

        [HttpPost]
        public string GetPaymentToken(string Amount,string MobileNumber,string Email)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                string sURL = "https://sandbox-icp-api.bankopen.co/api/payment_token";
                //string sURL = "https://icp-api.bankopen.co/api/payment_token";
            
                WebRequest wrGETURL;
                wrGETURL = WebRequest.Create(sURL);

                wrGETURL.Method = "POST";
                wrGETURL.ContentType = @"application/json; charset=utf-8";
                //Sandbox
                wrGETURL.Headers.Add("Authorization", "Bearer 415101c0-d188-11ea-9f4a-d96d3de71820:6373ce269435bd7a56131741ba27201b426df201");

                //wrGETURL.Headers.Add("Authorization", "Bearer 8dd56630-d1a3-11ea-b4a4-cd7b8d79485d:b6cea9a3cab16ed39ecc67dc4e87639c31560658");
                using (var stream = new StreamWriter(wrGETURL.GetRequestStream()))
                {
                    var bodyContent = new
                    {
                        amount = Amount,
                        mtx = Guid.NewGuid().ToString(),
                        currency = "INR",
                        contact_number = MobileNumber,
                        email_id = Email
                    }; // This will need to be changed to an actual class after finding what the specification sheet requires.

                    var json = JsonConvert.SerializeObject(bodyContent);

                    stream.Write(json);
                }
                var response = (HttpWebResponse)wrGETURL.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                PaymentTokenVM myDeserializedClass = JsonConvert.DeserializeObject<PaymentTokenVM>(responseString);
                return myDeserializedClass.id;
            }
            catch(Exception e)
            {
                return "Fail"+ e.Message.ToString();
            }            
           
        }

        public ActionResult RegisterUsingSocial(string email,string firstname,string last_name)
        {
            ViewData["tbl_Packages"] = _db.tbl_Package.Where(o => o.IsActive == true && o.IsDeleted == false).ToList();
            StudentVM objStudent = new StudentVM();
            objStudent.Email = email;
            objStudent.FirstName = firstname;
            objStudent.LastName = last_name;

            tbl_Students ojstu = _db.tbl_Students.Where(x => x.Email.ToLower() == email.ToLower() && !x.IsDeleted).FirstOrDefault();
            if(ojstu != null)
            {
                clsClientSession.SessionID = Session.SessionID;
                clsClientSession.UserID = ojstu.StudentId;
                clsClientSession.FirstName = ojstu.FirstName;
                clsClientSession.LastName = ojstu.LastName;
                clsClientSession.ImagePath = ojstu.ProfilePicture;
                clsClientSession.Email = ojstu.Email;
                clsClientSession.MobileNumber = ojstu.MobileNo;
                return RedirectToAction("Index", "MyExams");
            }
            return View(objStudent);
        }

        [HttpPost]
        public ActionResult RegisterUsingSocial(StudentVM userVM, HttpPostedFileBase ProfilePictureFile,FormCollection frm)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {

                    #region Validation

                    // Validate duplicate MobileNo 
                    tbl_Students duplicateMobile = _db.tbl_Students.Where(x => x.MobileNo.ToLower() == userVM.MobileNo && !x.IsDeleted).FirstOrDefault();
                    if (duplicateMobile != null)
                    {
                        ModelState.AddModelError("MobileNo", ErrorMessage.MobileNoExists);
                        return View(userVM);
                    }

                    string fileName = string.Empty;
                    string path = Server.MapPath(UserProfileDirectoryPath);

                    bool folderExists = Directory.Exists(path);
                    if (!folderExists)
                        Directory.CreateDirectory(path);

                    if (ProfilePictureFile != null)
                    {
                        string ext = Path.GetExtension(ProfilePictureFile.FileName);
                        string f_name = Path.GetFileNameWithoutExtension(ProfilePictureFile.FileName);

                        fileName = f_name + "-" + Guid.NewGuid() + ext;
                        ProfilePictureFile.SaveAs(path + fileName);
                    }
                    else
                    {
                        fileName = userVM.ProfilePicture;
                    }

                    #endregion Validation

                    #region CreateUser

                    tbl_Students objStudent = new tbl_Students();

                    objStudent.FirstName = userVM.FirstName;
                    objStudent.LastName = userVM.LastName;
                    objStudent.Email = userVM.Email;
                    objStudent.MobileNo = userVM.MobileNo;
                    objStudent.Password = userVM.Password;
                    objStudent.Address = userVM.Address;
                    objStudent.City = userVM.City;
                    objStudent.Remarks = userVM.Remarks;
                    objStudent.ProfilePicture = fileName;

                    if (!string.IsNullOrEmpty(userVM.Dob))
                    {
                        DateTime exp_Dob = DateTime.ParseExact(userVM.Dob, "dd/MM/yyyy", null);
                        objStudent.Dob = exp_Dob;
                    }

                    objStudent.IsActive = true;
                    objStudent.IsDeleted = false;
                    objStudent.CreatedDate = DateTime.UtcNow;
                    _db.tbl_Students.Add(objStudent);
                    _db.SaveChanges();

                    #endregion CreateUser

                    #region SendNotification

                    // Send Notification to User
                    string SmsResponse = SendSMSOfCreateUser(userVM);

                    #endregion SendNotification

                    int PackageId = Convert.ToInt32(frm["Package"]);

                    #region PendingFeeEntry

                    tbl_GeneralSetting objSetting = _db.tbl_GeneralSetting.FirstOrDefault();

                    DateTime exp_date = DateTime.UtcNow.AddDays(365); // default 365 days
                    if (objSetting.FeeExpiryInDays != null && objSetting.FeeExpiryInDays > 0)
                    {
                        exp_date = DateTime.UtcNow.AddDays(Convert.ToInt32(objSetting.FeeExpiryInDays));
                    }
                    if(PackageId > 0)
                    {

                       var objPckg = _db.tbl_Package.Where(o => o.PackageId == PackageId).FirstOrDefault();
                       if(objPckg != null)
                        {
                            string refralcode = Convert.ToString(frm["refrealcode"]);
                            int copupnid = 0;
                            var objcpcode = _db.tbl_CouponCode.Where(o => o.CouponCode == refralcode).FirstOrDefault();
                            decimal disc = 0;
                            if (objcpcode != null)
                            {
                                disc = (Convert.ToDecimal(objPckg.PackageAmount) * objcpcode.DiscountPercentage.Value) / 100;
                                copupnid = Convert.ToInt32(objcpcode.CouponCodeId);
                            }
                            tbl_StudentFee objStudentFee = new tbl_StudentFee();
                            objStudentFee.StudentId = objStudent.StudentId;
                            objStudentFee.FeeStatus = "Complete";
                            objStudentFee.FeeAmount = Math.Round(Convert.ToDecimal(objPckg.PackageAmount) - disc, 2);                          
                            objStudentFee.TotalExamAttempt = Convert.ToInt32(objPckg.TotalAttempt);
                            objStudentFee.FeeExpiryDate = exp_date;
                            objStudentFee.IsDeleted = false;
                            objStudentFee.RequestedDate = DateTime.UtcNow;
                            objStudentFee.MarkCompleteBy = objStudent.StudentId;
                            objStudentFee.MarkCompleteDate = DateTime.UtcNow;
                            objStudentFee.IsAttemptUsed = false;
                            objStudentFee.PackageId = objPckg.PackageId;
                            objStudentFee.PackageName = objPckg.PackageName;
                            objStudentFee.CouponCode = refralcode;
                            objStudentFee.CouponId = copupnid;
                            _db.tbl_StudentFee.Add(objStudentFee);
                            _db.SaveChanges();

                        }
                    }
                  
                    #endregion PendingFeeEntry

                    // Set login details 
                    clsClientSession.SessionID = Session.SessionID;
                    clsClientSession.UserID = objStudent.StudentId;
                    clsClientSession.FirstName = objStudent.FirstName;
                    clsClientSession.LastName = objStudent.LastName;
                    clsClientSession.ImagePath = objStudent.ProfilePicture;
                    clsClientSession.Email = objStudent.Email;
                    clsClientSession.MobileNumber = objStudent.MobileNo;

                    return RedirectToAction("Index", "MyExams");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

            return View(userVM);
        }

        [HttpPost]
        public string CheckCouponCode(string couponcode)
        {
            try
            {
                DateTime dtNow = DateTime.UtcNow;
                var objCop =  _db.tbl_CouponCode.Where(o => o.CouponCode == couponcode).FirstOrDefault();
                if(objCop == null)
                {
                    return "Invalid Referal Code";
                }
                else
                {
                    if(objCop.ExpiryDate >= dtNow)
                    {
                       int TotalUsed = _db.tbl_StudentFee.Where(o => o.CouponCode == couponcode).ToList().Count();
                       if(TotalUsed >= objCop.TotalMaxUsage)
                        {
                            return "Referal Code Usage Over";
                        }
                       else
                        {
                            return "Success^"+objCop.DiscountPercentage.ToString();
                        }
                      
                    }
                    else
                    {
                        return "Referal Code Expired";
                    }
                }
            }
            catch (Exception e)
            {
                return "Fail" + e.Message.ToString();
            }

        }
        
    }

    public class PaymentTokenVM
    {
        public string amount { get; set; }
        public string mtx { get; set; }
        public string id { get; set; }
        
    }
}