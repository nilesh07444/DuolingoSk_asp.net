using DuolingoSk.Areas.Client.Controllers;
using DuolingoSk.Filters;
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

        public ActionResult Index(int agentId = -1)
        {
            List<StudentVM> lstStudents = new List<StudentVM>();

            try
            {
                int LoggedInUserId = Int32.Parse(clsAdminSession.UserID.ToString());
                bool IsAgent = (clsAdminSession.RoleID == (int)AdminRoles.Agent);

                if (agentId == 0)
                {
                    lstStudents = (from a in _db.tbl_Students
                                   join u in _db.tbl_AdminUsers on a.AdminUserId equals u.AdminUserId into outerAgent
                                   from agent in outerAgent.DefaultIfEmpty()
                                   where !a.IsDeleted
                                   && (a.AdminUserId == 0 || a.AdminUserId == null)
                                   select new StudentVM
                                   {
                                       StudentId = a.StudentId,
                                       AdminUserId = a.AdminUserId,
                                       FullName = a.FullName,
                                       Email = a.Email,
                                       MobileNo = a.MobileNo,
                                       ProfilePicture = a.ProfilePicture,
                                       AgentName = (agent != null ? agent.FirstName + " " + agent.LastName : ""),
                                       IsActive = a.IsActive
                                   }).ToList();

                }
                else if (IsAgent && agentId == -1)
                {
                    lstStudents = (from a in _db.tbl_Students
                                   join u in _db.tbl_AdminUsers on a.AdminUserId equals u.AdminUserId into outerAgent
                                   from agent in outerAgent.DefaultIfEmpty()
                                   where !a.IsDeleted && a.AdminUserId == LoggedInUserId
                                   select new StudentVM
                                   {
                                       StudentId = a.StudentId,
                                       AdminUserId = a.AdminUserId,
                                       FullName = a.FullName,
                                       Email = a.Email,
                                       MobileNo = a.MobileNo,
                                       ProfilePicture = a.ProfilePicture,
                                       AgentName = (agent != null ? agent.FirstName + " " + agent.LastName : ""),
                                       IsActive = a.IsActive
                                   }).ToList();
                }
                else if (!IsAgent && agentId == -1)
                {
                    lstStudents = (from a in _db.tbl_Students
                                   join u in _db.tbl_AdminUsers on a.AdminUserId equals u.AdminUserId into outerAgent
                                   from agent in outerAgent.DefaultIfEmpty()
                                   where !a.IsDeleted
                                   select new StudentVM
                                   {
                                       StudentId = a.StudentId,
                                       AdminUserId = a.AdminUserId,
                                       FullName = a.FullName,
                                       Email = a.Email,
                                       MobileNo = a.MobileNo,
                                       ProfilePicture = a.ProfilePicture,
                                       AgentName = (agent != null ? agent.FirstName + " " + agent.LastName : ""),
                                       IsActive = a.IsActive
                                   }).ToList();
                }
                else
                {
                    lstStudents = (from a in _db.tbl_Students
                                   join u in _db.tbl_AdminUsers on a.AdminUserId equals u.AdminUserId into outerAgent
                                   from agent in outerAgent.DefaultIfEmpty()
                                   where !a.IsDeleted && a.AdminUserId == agentId
                                   select new StudentVM
                                   {
                                       StudentId = a.StudentId,
                                       AdminUserId = a.AdminUserId,
                                       FullName = a.FullName,
                                       Email = a.Email,
                                       MobileNo = a.MobileNo,
                                       ProfilePicture = a.ProfilePicture,
                                       AgentName = (agent != null ? agent.FirstName + " " + agent.LastName : ""),
                                       IsActive = a.IsActive
                                   }).ToList();
                }

                ViewBag.agentId = agentId;
                if (!IsAgent)
                {
                    List<AdminUserVM> lstAgents = (from a in _db.tbl_AdminUsers
                                                   where a.AdminRoleId == (int)AdminRoles.Agent && a.IsActive && !a.IsDeleted
                                                   select new AdminUserVM
                                                   {
                                                       AdminUserId = a.AdminUserId,
                                                       FullName = a.FirstName + " " + a.LastName
                                                   }).ToList();

                    ViewData["lstAgents"] = lstAgents;
                }

            }
            catch (Exception ex)
            {
            }

            return View(lstStudents);
        }

        public ActionResult Add()
        {
            StudentVM objStudent = new StudentVM();
            long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
            List<AgentPackageVM> lstPackages = (from s in _db.tbl_AgentPackage
                                                join c in _db.tbl_Package on s.PackageId equals c.PackageId
                                                join p in _db.tbl_AdminUsers on s.AgentId equals p.AdminUserId
                                                where !s.IsDeleted.Value && s.AgentId.Value == LoggedInUserId
                                                select new AgentPackageVM
                                                {
                                                    PackageId = s.PackageId.Value,
                                                    PackageName = c.PackageName,
                                                    AgentName = p.FirstName + " " + p.LastName,
                                                    PackageAmountAgent = s.PackageAmount,
                                                    PackageAgentId = s.PackageAgentId,
                                                    TotalAttempt = c.TotalAttempt
                                                }).OrderBy(x => x.PackageName).ToList();
            List<long> pkgids = new List<long>();
            if (lstPackages != null && lstPackages.Count() > 0)
            {
                pkgids = lstPackages.Select(x => x.PackageId).ToList();
            }
            List<AgentPackageVM> lstPackagesnew = (from s in _db.tbl_Package
                                                   where !s.IsDeleted && !pkgids.Contains(s.PackageId)
                                                   select new AgentPackageVM
                                                   {
                                                       PackageId = s.PackageId,
                                                       PackageName = s.PackageName,
                                                       AgentName = "",
                                                       PackageAmountAgent = s.PackageAmount,
                                                       PackageAgentId = 0,
                                                       TotalAttempt = s.TotalAttempt
                                                   }).OrderBy(x => x.PackageName).ToList();
            lstPackages.AddRange(lstPackagesnew);
            ViewData["lstPackages"] = lstPackages.OrderBy(x => x.PackageName).ToList();
            return View(objStudent);
        }

        [HttpPost]
        public ActionResult Add(StudentVM userVM, HttpPostedFileBase ProfilePictureFile, FormCollection frm)
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

                    // Validate duplicate Email 
                    tbl_Students duplicateEmail = _db.tbl_Students.Where(x => x.Email.ToLower() == userVM.Email && !x.IsDeleted).FirstOrDefault();
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

                    objStudent.AdminUserId = LoggedInUserId;
                    objStudent.FullName = userVM.FullName;
                    objStudent.Email = userVM.Email;
                    objStudent.MobileNo = userVM.MobileNo;
                    objStudent.Password = userVM.Password;
                    objStudent.Address = userVM.Address;
                    objStudent.City = userVM.City;
                    objStudent.Remarks = userVM.Remarks;
                    objStudent.ProfilePicture = fileName;

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

                    int PackageId = Convert.ToInt32(frm["Package"]);
                    decimal PckPriceForPay = 0;
                    if (PackageId > 0)
                    {
                        var objPckg = _db.tbl_Package.Where(o => o.PackageId == PackageId).FirstOrDefault();
                        if (objPckg != null)
                        {
                            DateTime exp_date = DateTime.UtcNow.AddDays(365); // default 365 days
                            if (objPckg.ExpiryInDays != null && objPckg.ExpiryInDays > 0)
                            {
                                exp_date = DateTime.UtcNow.AddDays(Convert.ToInt32(objPckg.ExpiryInDays));
                            }
                            var objAgentPckg = _db.tbl_AgentPackage.Where(o => o.AgentId == LoggedInUserId && o.PackageId == objPckg.PackageId && o.IsDeleted == false).FirstOrDefault();
                            string refralcode = Convert.ToString(frm["refrealcode"]);
                            int copupnid = 0;
                            var objcpcode = _db.tbl_CouponCode.Where(o => o.CouponCode == refralcode).FirstOrDefault();
                            decimal disc = 0;
                            if (objcpcode != null)
                            {
                                if (objAgentPckg != null)
                                {
                                    disc = (Convert.ToDecimal(objAgentPckg.PackageAmount) * objcpcode.DiscountPercentage.Value) / 100;
                                    PckPriceForPay = Convert.ToDecimal(objAgentPckg.PackageAmount);
                                }
                                else
                                {
                                    disc = (Convert.ToDecimal(objPckg.PackageAmount) * objcpcode.DiscountPercentage.Value) / 100;
                                    PckPriceForPay = Convert.ToDecimal(objPckg.PackageAmount);
                                }
                                copupnid = Convert.ToInt32(objcpcode.CouponCodeId);
                            }
                            else
                            {
                                if (objAgentPckg != null)
                                {
                                    PckPriceForPay = Convert.ToDecimal(objAgentPckg.PackageAmount);
                                }
                                else
                                {
                                    PckPriceForPay = Convert.ToDecimal(objPckg.PackageAmount);
                                }
                            }
                            tbl_StudentFee objStudentFee = new tbl_StudentFee();
                            objStudentFee.StudentId = objStudent.StudentId;
                            if (frm["hdnPaymentId"] != null && !string.IsNullOrEmpty(frm["hdnPaymentId"]))
                            {
                                objStudentFee.FeeStatus = "Complete";
                                objStudentFee.MarkCompleteBy = Convert.ToInt32(clsAdminSession.UserID);
                                objStudentFee.MarkCompleteDate = DateTime.UtcNow;
                                objStudentFee.Paymentoken = frm["hdnPaymentId"].ToString();
                            }
                            else
                            {
                                objStudentFee.FeeStatus = "Pending";
                            }
                            objStudentFee.FeeAmount = Math.Round(Convert.ToDecimal(PckPriceForPay) - disc, 2);
                            objStudentFee.TotalExamAttempt = Convert.ToInt32(objPckg.TotalAttempt);
                            objStudentFee.FeeExpiryDate = exp_date;
                            objStudentFee.OriginalPackagePrice = objPckg.PackageAmount;
                            objStudentFee.Discount = disc;
                            objStudentFee.IsDeleted = false;
                            objStudentFee.RequestedDate = DateTime.UtcNow;
                            objStudentFee.TotalWebinarAttempt = objPckg.TotalWebinar != null ? Convert.ToInt32(objPckg.TotalWebinar) : 0;

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
                                  FullName = a.FullName,
                                  Email = a.Email,
                                  MobileNo = a.MobileNo,
                                  Password = a.Password,
                                  Address = a.Address,
                                  City = a.City,
                                  Remarks = a.Remarks,
                                  ProfilePicture = a.ProfilePicture,
                                  IsActive = a.IsActive
                              }).FirstOrDefault();

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

                    objStudent.FullName = userVM.FullName;
                    objStudent.Email = userVM.Email;
                    objStudent.MobileNo = userVM.MobileNo;
                    objStudent.Password = userVM.Password;
                    objStudent.Address = userVM.Address;
                    objStudent.City = userVM.City;
                    objStudent.Remarks = userVM.Remarks;
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

        [HttpPost]
        public string ChangeStatus(long Id, string Status)
        {
            string ReturnMessage = "";
            try
            {
                tbl_Students objStudent = _db.tbl_Students.Where(x => x.StudentId == Id).FirstOrDefault();

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
                else
                {
                    ReturnMessage = "notfound";
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
                              join uC in _db.tbl_AdminUsers on a.CreatedBy equals uC.AdminUserId into outerCreated
                              from uC in outerCreated.DefaultIfEmpty()

                              join uM in _db.tbl_AdminUsers on a.UpdatedBy equals uM.AdminUserId into outerModified
                              from uM in outerModified.DefaultIfEmpty()

                              where a.StudentId == Id
                              select new StudentVM
                              {
                                  StudentId = a.StudentId,
                                  AdminUserId = a.AdminUserId,
                                  FullName = a.FullName,
                                  Email = a.Email,
                                  MobileNo = a.MobileNo,
                                  Password = a.Password,
                                  Address = a.Address,
                                  City = a.City,
                                  Remarks = a.Remarks,
                                  ProfilePicture = a.ProfilePicture,
                                  IsActive = a.IsActive,
                                  CreatedDate = a.CreatedDate,
                                  UpdatedDate = a.UpdatedDate,
                                  strCreatedBy = (uC != null ? uC.FirstName + " " + uC.LastName : ""),
                                  strModifiedBy = (uM != null ? uM.FirstName + " " + uM.LastName : "")

                              }).FirstOrDefault();

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

                    string msg = "Hello " + userVM.FullName + "\n\n";
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

        [HttpPost]
        public string CheckCouponCode(string couponcode)
        {
            try
            {
                DateTime dtNow = DateTime.UtcNow;
                var objCop = _db.tbl_CouponCode.Where(o => o.CouponCode == couponcode).FirstOrDefault();
                if (objCop == null)
                {
                    return "Invalid Referal Code";
                }
                else
                {
                    if (objCop.ExpiryDate >= dtNow)
                    {
                        int TotalUsed = _db.tbl_StudentFee.Where(o => o.CouponCode == couponcode).ToList().Count();
                        if (TotalUsed >= objCop.TotalMaxUsage)
                        {
                            return "Referal Code Usage Over";
                        }
                        else
                        {
                            return "Success^" + objCop.DiscountPercentage.ToString();
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

        [HttpPost]
        public string GetPaymentToken(string Amount, string MobileNumber, string Email)
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
            catch (Exception e)
            {
                return "Fail" + e.Message.ToString();
            }

        }

    }
}