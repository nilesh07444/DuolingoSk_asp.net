using DuolingoSk.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DuolingoSk.Models;
using DuolingoSk.Helper;
using DuolingoSk.Model;

namespace DuolingoSk.Areas.Admin.Controllers
{
    [CustomAuthorize]
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
            AdminUserVM objAdminUser = new AdminUserVM();
            long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

            try
            {
                objAdminUser = (from a in _db.tbl_AdminUsers  
                                where a.AdminUserId == LoggedInUserId
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
                                    Remarks = a.Remarks,
                                    ProfilePicture = a.ProfilePicture,
                                    IsActive = a.IsActive, 
                                }).FirstOrDefault();
                 
            }
            catch (Exception ex)
            {
            }

            return View(objAdminUser);
        }
    }
}