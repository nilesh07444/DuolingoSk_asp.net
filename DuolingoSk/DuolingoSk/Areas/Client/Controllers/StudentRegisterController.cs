using DuolingoSk.Helper;
using DuolingoSk.Model;
using DuolingoSk.Models;
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
            StudentVM objStudent = new StudentVM(); 
            return View(objStudent);
        }

        [HttpPost]
        public ActionResult Index(StudentVM userVM, HttpPostedFileBase ProfilePictureFile)
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

                    #region PendingFeeEntry
                     
                    tbl_GeneralSetting objSetting = _db.tbl_GeneralSetting.FirstOrDefault();

                    tbl_StudentFee objStudentFee = new tbl_StudentFee();
                    objStudentFee.StudentId = objStudent.StudentId;
                    objStudentFee.FeeStatus = "Pending";
                    objStudentFee.FeeAmount = Convert.ToDecimal(objSetting.RegistrationFee);
                    objStudentFee.TotalExamAttempt = Convert.ToInt32(objSetting.TotalExamAttempt);
                    objStudentFee.IsDeleted = false;
                    objStudentFee.RequestedDate = DateTime.UtcNow;
                    _db.tbl_StudentFee.Add(objStudentFee);
                    _db.SaveChanges();

                    #endregion PendingFeeEntry

                    // Set login details 
                    clsClientSession.SessionID = Session.SessionID;
                    clsClientSession.UserID = objStudent.StudentId;
                    clsClientSession.FirstName = objStudent.FirstName;
                    clsClientSession.LastName = objStudent.LastName;
                    clsClientSession.ImagePath = objStudent.ProfilePicture;
                    clsClientSession.Email = objStudent.Email;
                    clsClientSession.MobileNumber = objStudent.MobileNo;

                    return RedirectToAction("MyExams");

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
         
    }
}