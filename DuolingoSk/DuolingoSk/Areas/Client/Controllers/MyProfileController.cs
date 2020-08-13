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
    public class MyProfileController : Controller
    {
        private readonly DuolingoSk_Entities _db;
        public string UserProfileDirectoryPath = "";

        public MyProfileController()
        {
            _db = new DuolingoSk_Entities();
            UserProfileDirectoryPath = ErrorMessage.UserProfileDirectoryPath;
        }

        public ActionResult Index()
        {
            long LoggedInUserId = Int64.Parse(clsClientSession.UserID.ToString());

            StudentVM objStudent = (from s in _db.tbl_Students
                                    where s.StudentId == LoggedInUserId
                                    select new StudentVM
                                    {
                                        StudentId = s.StudentId,
                                        FullName = s.FullName, 
                                        Password = s.Password,
                                        Email = s.Email,
                                        MobileNo = s.MobileNo,
                                        Address = s.Address,
                                        City = s.City, 
                                        ProfilePicture = s.ProfilePicture
                                    }).FirstOrDefault();
             
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
                    long LoggedInUserId = Int64.Parse(clsClientSession.UserID.ToString());

                    #region Validation

                    // Validate duplicate MobileNo 
                    tbl_Students duplicateMobile = _db.tbl_Students.Where(x => x.MobileNo.ToLower() == userVM.MobileNo && x.StudentId != userVM.StudentId && !x.IsDeleted).FirstOrDefault();
                    if (duplicateMobile != null)
                    {
                        ModelState.AddModelError("MobileNo", ErrorMessage.MobileNoExists);

                        return View(userVM);
                    }

                    // Validate duplicate EmailId 
                    if (!string.IsNullOrEmpty(userVM.Email))
                    {
                        tbl_Students duplicateEmailId = _db.tbl_Students.Where(x => x.Email.ToLower() == userVM.Email && x.StudentId != userVM.StudentId && !x.IsDeleted).FirstOrDefault();
                        if (duplicateEmailId != null)
                        {
                            ModelState.AddModelError("Email", ErrorMessage.EmailExists);
                            return View(userVM);
                        }
                    }

                    tbl_Students objStudent = _db.tbl_Students.Where(x => x.StudentId == userVM.StudentId).FirstOrDefault();

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
                        fileName = objStudent.ProfilePicture;
                    }

                    #endregion Validation

                    #region UpdateUser

                    objStudent.FullName = userVM.FullName; 
                    objStudent.Email = userVM.Email;  
                    objStudent.Address = userVM.Address;
                    objStudent.City = userVM.City; 
                    objStudent.ProfilePicture = fileName;
                      
                    objStudent.UpdatedDate = DateTime.UtcNow;
                    objStudent.UpdatedBy = LoggedInUserId;

                    _db.SaveChanges();

                    return RedirectToAction("Index");

                    #endregion UpdateUser
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(userVM);
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public string ChangePasswordSubmit(FormCollection frm)
        {
            string ReturnMessage = "";
            try
            {
                string CurrentPassword = frm["currentpwd"];
                string NewPassword = frm["newpwd"];

                long LoggedInUserId = Int64.Parse(clsClientSession.UserID.ToString());
                tbl_Students objStudent = _db.tbl_Students.Where(x => x.StudentId == LoggedInUserId).FirstOrDefault();

                if (objStudent != null)
                {
                    string EncryptedCurrentPassword = CurrentPassword; // clsCommon.EncryptString(CurrentPassword);
                    if (objStudent.Password == EncryptedCurrentPassword)
                    {
                        objStudent.Password = NewPassword; // clsCommon.EncryptString(NewPassword);
                        _db.SaveChanges();

                        ReturnMessage = "SUCCESS";
                        Session.Clear();
                    }
                    else
                    {
                        ReturnMessage = "CP_NOT_MATCHED";
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                ReturnMessage = "ERROR";
            }

            return ReturnMessage;
        }

        public string SendOTP(string MobileNumber)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    WebClient client = new WebClient();
                    Random random = new Random();
                    int num = random.Next(310450, 789899);
                    string msg = "Your change password OTP code is " + num;

                    string url = "http://sms.unitechcenter.com/sendSMS?username=skacademy&message=" + msg + "&sendername=SKANAD&smstype=TRANS&numbers=" + MobileNumber + "&apikey=0b9c3015-bbcd-4ad8-b9ac-30f28451ebe6";

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


    }
}