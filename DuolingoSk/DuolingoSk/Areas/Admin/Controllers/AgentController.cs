using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DuolingoSk.Filters;
using DuolingoSk.Helper;
using DuolingoSk.Model;
using DuolingoSk.Models;

namespace DuolingoSk.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class AgentController : Controller
    {
        private readonly DuolingoSk_Entities _db;
        public string UserProfileDirectoryPath = "";

        public AgentController()
        {
            _db = new DuolingoSk_Entities();
            UserProfileDirectoryPath = ErrorMessage.UserProfileDirectoryPath;
        }

        public ActionResult Index()
        {
            List<AdminUserVM> lstAdminUsers = new List<AdminUserVM>();

            try
            {
                lstAdminUsers = (from a in _db.tbl_AdminUsers 
                                 where !a.IsDeleted && a.AdminRoleId == (int)AdminRoles.Agent
                                 select new AdminUserVM
                                 {
                                     AdminUserId = a.AdminUserId,
                                     AdminRoleId = a.AdminRoleId, 
                                     FirstName = a.FirstName,
                                     LastName = a.LastName,
                                     Email = a.Email,
                                     MobileNo = a.MobileNo,
                                     ProfilePicture = a.ProfilePicture,
                                     StudentRegistrationFee = a.StudentRegistrationFee,
                                     StudentRenewFee = a.StudentRenewFee,
                                     IsActive = a.IsActive
                                 }).ToList();
            }
            catch (Exception ex)
            {
            }

            return View(lstAdminUsers);
        }
         
        public ActionResult Add()
        {
            AdminUserVM objAdminUser = new AdminUserVM();
             
            return View(objAdminUser);
        }

        [HttpPost]
        public ActionResult Add(AdminUserVM userVM, HttpPostedFileBase ProfilePictureFile)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    #region Validation

                    // Validate duplicate MobileNo 
                    tbl_AdminUsers duplicateMobile = _db.tbl_AdminUsers.Where(x => x.MobileNo.ToLower() == userVM.MobileNo && !x.IsDeleted).FirstOrDefault();
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

                    tbl_AdminUsers objAdminUser = new tbl_AdminUsers();

                    objAdminUser.AdminRoleId = (int)AdminRoles.Agent; //userVM.AdminRoleId;
                    objAdminUser.FirstName = userVM.FirstName;
                    objAdminUser.LastName = userVM.LastName;
                    objAdminUser.Email = userVM.Email;
                    objAdminUser.MobileNo = userVM.MobileNo;
                    objAdminUser.Password = userVM.Password; 
                    objAdminUser.Address = userVM.Address;
                    objAdminUser.City = userVM.City; 
                    objAdminUser.Remarks = userVM.Remarks;
                    objAdminUser.ProfilePicture = fileName;
                    objAdminUser.MaxLevel = userVM.MaxLevel;

                    //if (!string.IsNullOrEmpty(userVM.Dob))
                    //{
                    //    DateTime exp_Dob = DateTime.ParseExact(userVM.Dob, "dd/MM/yyyy", null);
                    //    objAdminUser.Dob = exp_Dob;
                    //}

                    objAdminUser.StudentRegistrationFee = userVM.StudentRegistrationFee;
                    objAdminUser.StudentRenewFee = userVM.StudentRenewFee;

                    objAdminUser.IsActive = true;
                    objAdminUser.IsDeleted = false;
                    objAdminUser.CreatedDate = DateTime.UtcNow;
                    objAdminUser.CreatedBy = LoggedInUserId;
                    _db.tbl_AdminUsers.Add(objAdminUser);
                    _db.SaveChanges();

                    // Send Notification
                    string SmsResponse = SendSMSOfCreateUser(userVM);

                    return RedirectToAction("Index");

                    #endregion CreateUser
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
            AdminUserVM objAdminUser = new AdminUserVM();

            try
            {
                objAdminUser = (from a in _db.tbl_AdminUsers
                                where a.AdminUserId == Id
                                select new AdminUserVM
                                {
                                    AdminUserId = a.AdminUserId,
                                    AdminRoleId = a.AdminRoleId,
                                    FirstName = a.FirstName,
                                    LastName = a.LastName,
                                    Email = a.Email,
                                    MobileNo = a.MobileNo,
                                    Password = a.Password, 
                                    Address = a.Address,
                                    City = a.City, 
                                    dtDob = a.Dob, 
                                    Remarks = a.Remarks,
                                    StudentRegistrationFee = a.StudentRegistrationFee,
                                    StudentRenewFee = a.StudentRenewFee,
                                    ProfilePicture = a.ProfilePicture,
                                    IsActive = a.IsActive,
                                    MaxLevel = a.MaxLevel
                                }).FirstOrDefault();

                //if (objAdminUser.dtDob != null)
                //{
                //    objAdminUser.Dob = Convert.ToDateTime(objAdminUser.dtDob).ToString("dd/MM/yyyy");
                //}
                  
            }
            catch (Exception ex)
            {

            }

            return View(objAdminUser);
        }

        [HttpPost]
        public ActionResult Edit(AdminUserVM userVM, HttpPostedFileBase ProfilePictureFile)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    #region Validation

                    // Validate duplicate MobileNo 
                    tbl_AdminUsers duplicateMobile = _db.tbl_AdminUsers.Where(x => x.MobileNo.ToLower() == userVM.MobileNo && x.AdminUserId != userVM.AdminUserId && !x.IsDeleted).FirstOrDefault();
                    if (duplicateMobile != null)
                    {
                        ModelState.AddModelError("MobileNo", ErrorMessage.MobileNoExists); 
                        return View(userVM);
                    }

                    tbl_AdminUsers objAdminUser = _db.tbl_AdminUsers.Where(x => x.AdminUserId == userVM.AdminUserId).FirstOrDefault();

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
                        fileName = objAdminUser.ProfilePicture;
                    }

                    #endregion Validation

                    #region UpdateUser
                     
                    objAdminUser.FirstName = userVM.FirstName;
                    objAdminUser.LastName = userVM.LastName;
                    objAdminUser.Email = userVM.Email;
                    objAdminUser.MobileNo = userVM.MobileNo;
                    objAdminUser.Password = userVM.Password; 
                    objAdminUser.Address = userVM.Address;
                    objAdminUser.City = userVM.City; 
                    objAdminUser.Remarks = userVM.Remarks;
                    objAdminUser.ProfilePicture = fileName;
                    objAdminUser.MaxLevel = userVM.MaxLevel;

                    //if (!string.IsNullOrEmpty(userVM.Dob))
                    //{
                    //    DateTime exp_Dob = DateTime.ParseExact(userVM.Dob, "dd/MM/yyyy", null);
                    //    objAdminUser.Dob = exp_Dob;
                    //}
                    //else
                    //{
                    //    objAdminUser.Dob = null;
                    //}

                    objAdminUser.StudentRegistrationFee = userVM.StudentRegistrationFee;
                    objAdminUser.StudentRenewFee = userVM.StudentRenewFee;

                    objAdminUser.UpdatedDate = DateTime.UtcNow;
                    objAdminUser.UpdatedBy = LoggedInUserId;

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
        public string ChangePassword(FormCollection frm)
        {
            string ReturnMessage = "";
            try
            {
                string CurrentPassword = frm["currentpwd"];
                string NewPassword = frm["newpwd"];

                long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                tbl_AdminUsers objUser = _db.tbl_AdminUsers.Where(x => x.AdminUserId == LoggedInUserId).FirstOrDefault();

                if (objUser != null)
                {
                    string EncryptedCurrentPassword = CurrentPassword; // CoreHelper.Encrypt(CurrentPassword);
                    if (objUser.Password == EncryptedCurrentPassword)
                    {
                        objUser.Password = NewPassword; //CoreHelper.Encrypt(NewPassword);
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

        [HttpPost]
        public string ChangeStatus(long Id, string Status)
        {
            string ReturnMessage = "";
            try
            {
                tbl_AdminUsers objAdminUser = _db.tbl_AdminUsers.Where(x => x.AdminUserId == Id).FirstOrDefault();

                if (objAdminUser != null)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    if (Status == "Active")
                    {
                        objAdminUser.IsActive = true;
                    }
                    else
                    {
                        objAdminUser.IsActive = false;
                    }

                    objAdminUser.UpdatedBy = LoggedInUserId;
                    objAdminUser.UpdatedDate = DateTime.UtcNow;

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
        public string DeleteAdminUser(int AdminUserId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_AdminUsers objAdminUser = _db.tbl_AdminUsers.Where(x => x.AdminUserId == AdminUserId).FirstOrDefault();

                if (objAdminUser == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    objAdminUser.IsDeleted = true;
                    objAdminUser.UpdatedBy = LoggedInUserId;
                    objAdminUser.UpdatedDate = DateTime.UtcNow;

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
            AdminUserVM objAdminUser = new AdminUserVM();

            try
            {
                objAdminUser = (from a in _db.tbl_AdminUsers
                                join uC in _db.tbl_AdminUsers on a.CreatedBy equals uC.AdminUserId into outerCreated
                                from uC in outerCreated.DefaultIfEmpty()

                                join uM in _db.tbl_AdminUsers on a.UpdatedBy equals uM.AdminUserId into outerModified
                                from uM in outerModified.DefaultIfEmpty()

                                where a.AdminUserId == Id
                                select new AdminUserVM
                                {
                                    AdminUserId = a.AdminUserId,
                                    AdminRoleId = a.AdminRoleId,
                                    FirstName = a.FirstName,
                                    LastName = a.LastName,
                                    Email = a.Email,
                                    MobileNo = a.MobileNo,
                                    Password = a.Password, 
                                    Address = a.Address,
                                    City = a.City, 
                                    MaxLevel = a.MaxLevel,
                                    dtDob = a.Dob, 
                                    Remarks = a.Remarks,
                                    StudentRegistrationFee = a.StudentRegistrationFee,
                                    StudentRenewFee = a.StudentRenewFee,
                                    ProfilePicture = a.ProfilePicture,
                                    IsActive = a.IsActive, 
                                    CreatedDate = a.CreatedDate,
                                    UpdatedDate = a.UpdatedDate,
                                    strCreatedBy = (uC != null ? uC.FirstName + " " + uC.LastName : ""),
                                    strModifiedBy = (uM != null ? uM.FirstName + " " + uM.LastName : "")

                                }).FirstOrDefault();

                //if (objAdminUser.dtDob != null)
                //{
                //    objAdminUser.Dob = Convert.ToDateTime(objAdminUser.dtDob).ToString("dd/MM/yyyy");
                //}
                 

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(objAdminUser);
        }
         
        public string SendSMSOfCreateUser(AdminUserVM userVM)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    WebClient client = new WebClient();

                    string msg = "Hello " + userVM.FirstName + "\n\n";
                    msg += "You are agent of Duolingo sk." + "\n\n";

                    msg += "Below are login details:" + "\n";
                    msg += "Mobile No:" + userVM.MobileNo + "\n";
                    msg += "Password:" + userVM.Password + "\n\n";

                    msg += "Fee details:" + "\n";
                    msg += "Registration Fee:" + userVM.StudentRegistrationFee + "\n";
                    msg += "Renew Fee:" + userVM.StudentRenewFee + "\n";

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

        public ActionResult Packages(int AgentId = 0)
        {
          List<AgentPackageVM> lstPackages = (from s in _db.tbl_AgentPackage
                              join c in _db.tbl_Package on s.PackageId equals c.PackageId
                              join p in _db.tbl_AdminUsers on s.AgentId equals p.AdminUserId
                              where !s.IsDeleted.Value && (AgentId == 0 || s.AgentId.Value == AgentId)
                              select new AgentPackageVM
                              {
                                  PackageId = s.PackageId.Value,
                                  PackageName = c.PackageName,
                                  AgentName = p.FirstName+" "+p.LastName,
                                  PackageAmountAgent = s.PackageAmount,
                                  PackageAgentId = s.PackageAgentId
                              }).OrderBy(x => x.PackageName).ToList();
           ViewData["Agents"] = _db.tbl_AdminUsers.Where(x => x.IsActive && !x.IsDeleted && x.AdminRoleId == 2)
                       .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.AdminUserId).Trim(), Text = o.FirstName + " " + o.LastName })
                       .OrderBy(x => x.Text).ToList();
            ViewBag.AgentId = AgentId;
            return View(lstPackages);
        }
        public ActionResult AddPackage()
        {
            AgentPackageVM objAgnt = new AgentPackageVM();

            List<tbl_Package> lstPackges = _db.tbl_Package.Where(o => o.IsDeleted == false && o.IsActive == true).ToList();
          
            objAgnt.AgentList = _db.tbl_AdminUsers.Where(x => x.IsActive && !x.IsDeleted && x.AdminRoleId == 2)
                        .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.AdminUserId).Trim(), Text = o.FirstName + " "+ o.LastName })
                        .OrderBy(x => x.Text).ToList();
            objAgnt.PackageList = _db.tbl_Package.Where(o => o.IsDeleted == false && o.IsActive == true)
                      .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.PackageId).Trim(), Text = o.PackageName})
                      .OrderBy(x => x.Text).ToList();
            ViewData["lstPackges"] = lstPackges;
          
            return View(objAgnt);
        }

        [HttpPost]
        public ActionResult AddPackage(AgentPackageVM pkg)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    #region Validation
                 
                    tbl_AgentPackage duplicate = _db.tbl_AgentPackage.Where(x => x.AgentId == pkg.AgentId && x.PackageId == pkg.PackageId && x.IsDeleted == false).FirstOrDefault();
                    if (duplicate != null)
                    {
                        ModelState.AddModelError("PackageId", ErrorMessage.PackageExistsAgent);
                        List<tbl_Package> lstPackges = _db.tbl_Package.Where(o => o.IsDeleted == false && o.IsActive == true).ToList();

                        pkg.AgentList = _db.tbl_AdminUsers.Where(x => x.IsActive && !x.IsDeleted && x.AdminRoleId == 2)
                                    .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.AdminUserId).Trim(), Text = o.FirstName + " " + o.LastName })
                                    .OrderBy(x => x.Text).ToList();
                        pkg.PackageList = _db.tbl_Package.Where(o => o.IsDeleted == false && o.IsActive == true)
                                  .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.PackageId).Trim(), Text = o.PackageName })
                                  .OrderBy(x => x.Text).ToList();
                        ViewData["lstPackges"] = lstPackges;

                        return View(pkg);
                    }
                   
                    #endregion Validation

                   
                    tbl_AgentPackage objtbl_AgentPackage = new tbl_AgentPackage();

                    objtbl_AgentPackage.PackageId = Convert.ToInt32(pkg.PackageId);
                    objtbl_AgentPackage.AgentId = pkg.AgentId;
                    objtbl_AgentPackage.PackageAmount = pkg.PackageAmountAgent;
                    objtbl_AgentPackage.IsDeleted = false;
                    objtbl_AgentPackage.CreatedDate = DateTime.UtcNow;
                    objtbl_AgentPackage.CreatedBy = LoggedInUserId;
                    _db.tbl_AgentPackage.Add(objtbl_AgentPackage);
                    _db.SaveChanges();

                    return RedirectToAction("Packages");

                   
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

            return View(pkg);
        }

        public ActionResult EditPackage(long Id)
        {
            AgentPackageVM objAgnt = new AgentPackageVM();
            var objPackgg = _db.tbl_AgentPackage.Where(o => o.PackageAgentId == Id).FirstOrDefault();

            objAgnt.PackageAgentId = Id;
            objAgnt.PackageId = Convert.ToInt64(objPackgg.PackageId);
            objAgnt.AgentId = objPackgg.AgentId.Value;
            objAgnt.PackageAmountAgent = objPackgg.PackageAmount;
            List<tbl_Package> lstPackges = _db.tbl_Package.Where(o => o.IsDeleted == false && o.IsActive == true).ToList();

            objAgnt.AgentList = _db.tbl_AdminUsers.Where(x => x.IsActive && !x.IsDeleted && x.AdminRoleId == 2)
                        .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.AdminUserId).Trim(), Text = o.FirstName + " " + o.LastName })
                        .OrderBy(x => x.Text).ToList();
            objAgnt.PackageList = _db.tbl_Package.Where(o => o.IsDeleted == false && o.IsActive == true)
                      .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.PackageId).Trim(), Text = o.PackageName })
                      .OrderBy(x => x.Text).ToList();
            ViewData["lstPackges"] = lstPackges;

            return View(objAgnt);
        }

        [HttpPost]
        public ActionResult EditPackage(AgentPackageVM pkg)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    #region Validation

                    tbl_AgentPackage duplicate = _db.tbl_AgentPackage.Where(x => x.AgentId == pkg.AgentId && x.PackageAgentId != pkg.PackageAgentId && x.PackageId == pkg.PackageId && x.IsDeleted == false).FirstOrDefault();
                    if (duplicate != null)
                    {
                        ModelState.AddModelError("PackageId", ErrorMessage.PackageExistsAgent);
                        List<tbl_Package> lstPackges = _db.tbl_Package.Where(o => o.IsDeleted == false && o.IsActive == true).ToList();

                        pkg.AgentList = _db.tbl_AdminUsers.Where(x => x.IsActive && !x.IsDeleted && x.AdminRoleId == 2)
                                    .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.AdminUserId).Trim(), Text = o.FirstName + " " + o.LastName })
                                    .OrderBy(x => x.Text).ToList();
                        pkg.PackageList = _db.tbl_Package.Where(o => o.IsDeleted == false && o.IsActive == true)
                                  .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.PackageId).Trim(), Text = o.PackageName })
                                  .OrderBy(x => x.Text).ToList();
                        ViewData["lstPackges"] = lstPackges;

                        return View(pkg);
                    }

                    #endregion Validation


                    tbl_AgentPackage objtbl_AgentPackage = _db.tbl_AgentPackage.Where(o => o.PackageAgentId == pkg.PackageAgentId).FirstOrDefault();

                    objtbl_AgentPackage.PackageId = Convert.ToInt32(pkg.PackageId);
                    objtbl_AgentPackage.AgentId = pkg.AgentId;
                    objtbl_AgentPackage.PackageAmount = pkg.PackageAmountAgent;                 
                    objtbl_AgentPackage.ModifiedDate = DateTime.UtcNow;
                    objtbl_AgentPackage.ModifiedBy = LoggedInUserId;                    
                    _db.SaveChanges();

                    return RedirectToAction("Packages");


                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

            return View(pkg);
        }

        [HttpPost]
        public string DeleteAgentPackage(int AgentPackageId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_AgentPackage objPackage = _db.tbl_AgentPackage.Where(x => x.PackageAgentId == AgentPackageId).FirstOrDefault();

                if (objPackage == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    objPackage.IsDeleted = true;
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

    }
}