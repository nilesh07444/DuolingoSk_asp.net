using DuolingoSk.Filters;
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

namespace DuolingoSk.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class StudentController : Controller
    {
        private readonly DuolingoSk_Entities _db;
        public string UserProfileDirectoryPath = "";

        public StudentController()
        {
            _db = new DuolingoSk_Entities();
            UserProfileDirectoryPath = ErrorMessage.UserProfileDirectoryPath;
        }

        public ActionResult Index()
        {
            List<StudentVM> lstStudents = new List<StudentVM>();

            try
            {
                lstStudents = (from a in _db.tbl_Students
                               where !a.IsDeleted
                               select new StudentVM
                               {
                                   StudentId = a.StudentId,
                                   AdminUserId = a.AdminUserId,
                                   FirstName = a.FirstName,
                                   LastName = a.LastName,
                                   Email = a.Email,
                                   MobileNo = a.MobileNo,
                                   ProfilePicture = a.ProfilePicture,
                                   IsActive = a.IsActive
                               }).ToList();
            }
            catch (Exception ex)
            {
            }

            return View(lstStudents);
        }

        public ActionResult Add()
        {
            StudentVM objStudent = new StudentVM();

            return View(objStudent);
        }

        [HttpPost]
        public ActionResult Add(StudentVM userVM, HttpPostedFileBase ProfilePictureFile)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

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

                    objStudent.AdminUserId = LoggedInUserId;
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
                    objStudent.CreatedBy = LoggedInUserId;
                    _db.tbl_Students.Add(objStudent);
                    _db.SaveChanges();

                    #endregion CreateUser

                    #region SendNotification

                    // Send Notification to User
                    string SmsResponse = SendSMSOfCreateUser(userVM);
                    
                    #endregion SendNotification

                    #region PendingFeeEntry

                    tbl_AdminUsers agentProfile = _db.tbl_AdminUsers.Where(x => x.AdminUserId == LoggedInUserId).FirstOrDefault();
                    tbl_GeneralSetting objSetting = _db.tbl_GeneralSetting.FirstOrDefault();

                    tbl_StudentFee objStudentFee = new tbl_StudentFee();
                    objStudentFee.StudentId = objStudent.StudentId;
                    objStudentFee.FeeStatus = "Pending";
                    objStudentFee.FeeAmount = Convert.ToDecimal(agentProfile.StudentRegistrationFee);
                    objStudentFee.TotalExamAttempt = Convert.ToInt32(objSetting.TotalExamAttempt);
                    objStudentFee.IsDeleted = false;
                    objStudentFee.RequestedDate = DateTime.UtcNow;
                    _db.tbl_StudentFee.Add(objStudentFee);
                    _db.SaveChanges();

                    #endregion PendingFeeEntry

                    return RedirectToAction("Index");
                     
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

            return View(userVM);
        }

        public ActionResult Edit(long Id)
        {
            StudentVM objStudent = new StudentVM();

            try
            {
                objStudent = (from a in _db.tbl_Students
                              where a.StudentId == Id
                              select new StudentVM
                              {
                                  StudentId = a.StudentId,
                                  AdminUserId = a.AdminUserId,
                                  FirstName = a.FirstName,
                                  LastName = a.LastName,
                                  Email = a.Email,
                                  MobileNo = a.MobileNo,
                                  Password = a.Password,
                                  Address = a.Address,
                                  City = a.City,
                                  dtDob = a.Dob,
                                  Remarks = a.Remarks,
                                  ProfilePicture = a.ProfilePicture,
                                  IsActive = a.IsActive
                              }).FirstOrDefault();

                if (objStudent.dtDob != null)
                {
                    objStudent.Dob = Convert.ToDateTime(objStudent.dtDob).ToString("dd/MM/yyyy");
                }

            }
            catch (Exception ex)
            {

            }

            return View(objStudent);
        }

        [HttpPost]
        public ActionResult Edit(StudentVM userVM, HttpPostedFileBase ProfilePictureFile)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    #region Validation

                    // Validate duplicate MobileNo 
                    tbl_Students duplicateMobile = _db.tbl_Students.Where(x => x.MobileNo.ToLower() == userVM.MobileNo && x.StudentId != userVM.StudentId && !x.IsDeleted).FirstOrDefault();
                    if (duplicateMobile != null)
                    {
                        ModelState.AddModelError("MobileNo", ErrorMessage.MobileNoExists);

                        return View(userVM);
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
                    else
                    {
                        objStudent.Dob = null;
                    }

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

        [HttpPost]
        public string ChangeStatus(long Id, string Status)
        {
            string ReturnMessage = "";
            try
            {
                tbl_Students objStudent = _db.tbl_Students.Where(x => x.AdminUserId == Id).FirstOrDefault();

                if (objStudent != null)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    if (Status == "Active")
                    {
                        objStudent.IsActive = true;
                    }
                    else
                    {
                        objStudent.IsActive = false;
                    }

                    objStudent.UpdatedBy = LoggedInUserId;
                    objStudent.UpdatedDate = DateTime.UtcNow;

                    _db.SaveChanges();
                    ReturnMessage = "success";
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }

        [HttpPost]
        public string DeleteStudent(int StudentId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_Students objStudent = _db.tbl_Students.Where(x => x.AdminUserId == StudentId).FirstOrDefault();

                if (objStudent == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    objStudent.IsDeleted = true;
                    objStudent.UpdatedBy = LoggedInUserId;
                    objStudent.UpdatedDate = DateTime.UtcNow;

                    _db.SaveChanges();

                    ReturnMessage = "success";
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }

        public ActionResult View(int Id)
        {
            StudentVM objStudent = new StudentVM();

            try
            {
                objStudent = (from a in _db.tbl_Students
                              join uC in _db.tbl_Students on a.CreatedBy equals uC.AdminUserId into outerCreated
                              from uC in outerCreated.DefaultIfEmpty()

                              join uM in _db.tbl_Students on a.UpdatedBy equals uM.AdminUserId into outerModified
                              from uM in outerModified.DefaultIfEmpty()

                              where a.StudentId == Id
                              select new StudentVM
                              {
                                  StudentId = a.StudentId,
                                  AdminUserId = a.AdminUserId,
                                  FirstName = a.FirstName,
                                  LastName = a.LastName,
                                  Email = a.Email,
                                  MobileNo = a.MobileNo,
                                  Password = a.Password,
                                  Address = a.Address,
                                  City = a.City,
                                  dtDob = a.Dob,
                                  Remarks = a.Remarks,
                                  ProfilePicture = a.ProfilePicture,
                                  IsActive = a.IsActive,
                                  CreatedDate = a.CreatedDate,
                                  UpdatedDate = a.UpdatedDate,
                                  strCreatedBy = (uC != null ? uC.FirstName + " " + uC.LastName : ""),
                                  strModifiedBy = (uM != null ? uM.FirstName + " " + uM.LastName : "")

                              }).FirstOrDefault();

                if (objStudent.dtDob != null)
                {
                    objStudent.Dob = Convert.ToDateTime(objStudent.dtDob).ToString("dd/MM/yyyy");
                }
                 
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(objStudent);
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

                    string url = "http://sms.unitechcenter.com/sendSMS?username=krupab&message=" + msg + "&sendername=KRUPAB&smstype=TRANS&numbers=" + userVM.MobileNo + "&apikey=e8528131-b45b-4f49-94ef-d94adb1010c4";

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