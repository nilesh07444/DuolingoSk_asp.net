using System;
using System.Collections.Generic;
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
    public class PackageController : Controller
    {
        private readonly DuolingoSk_Entities _db;
        public string PackageDirectoryPath = "";

        public PackageController()
        {
            _db = new DuolingoSk_Entities();
            PackageDirectoryPath = ErrorMessage.PackageDirectoryPath;
        }

        public ActionResult Index()
        {
            List<PackageVM> lstPackage = new List<PackageVM>();

            try
            {
                lstPackage = (from m in _db.tbl_Package
                              where !m.IsDeleted
                              select new PackageVM
                              {
                                  PackageId = m.PackageId,
                                  PackageName = m.PackageName,
                                  PackageImageName = m.PackageImage,
                                  PackageAmount = m.PackageAmount,
                                  IsActive = m.IsActive
                              }).ToList();
            }
            catch (Exception ex)
            {
            }

            return View(lstPackage);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(PackageVM packageVM, HttpPostedFileBase PackageImage)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    #region Validation

                    string fileName = string.Empty;
                    string path = Server.MapPath(PackageDirectoryPath);

                    bool folderExists = Directory.Exists(path);
                    if (!folderExists)
                        Directory.CreateDirectory(path);

                    if (PackageImage != null)
                    {
                        string ext = Path.GetExtension(PackageImage.FileName);
                        string f_name = Path.GetFileNameWithoutExtension(PackageImage.FileName);

                        fileName = f_name + "-" + Guid.NewGuid() + ext;
                        PackageImage.SaveAs(path + fileName);
                    }
                    else
                    {
                        fileName = packageVM.PackageImageName;
                    }

                    #endregion Validation

                    #region CreateMaterial

                    tbl_Package objPackage = new tbl_Package();
                    objPackage.PackageName = packageVM.PackageName;
                    objPackage.PackageAmount = packageVM.PackageAmount;
                    objPackage.PackageImage = fileName;
                    objPackage.IsActive = true;
                    objPackage.IsDeleted = false;
                    objPackage.CreatedDate = DateTime.UtcNow;
                    objPackage.CreatedBy = LoggedInUserId;
                    _db.tbl_Package.Add(objPackage);
                    _db.SaveChanges();

                    return RedirectToAction("Index");

                    #endregion CreateMaterial
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

            return View(packageVM);
        }

        public ActionResult Edit(long Id)
        {
            PackageVM objPackage = new PackageVM();

            try
            {
                objPackage = (from m in _db.tbl_Package
                              where m.PackageId == Id
                              select new PackageVM
                              {
                                  PackageId = m.PackageId,
                                  PackageName = m.PackageName,
                                  PackageAmount = m.PackageAmount,
                                  PackageImageName = m.PackageImage,
                                  IsActive = m.IsActive
                              }).FirstOrDefault(); 
            }
            catch (Exception ex)
            { 
            }

            return View(objPackage);
        }

        [HttpPost]
        public ActionResult Edit(PackageVM packageVM, HttpPostedFileBase PackageImage)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    #region Validation

                    tbl_Package objPackage = _db.tbl_Package.Where(x => x.PackageId == packageVM.PackageId).FirstOrDefault();

                    string fileName = string.Empty;
                    string path = Server.MapPath(PackageDirectoryPath);

                    bool folderExists = Directory.Exists(path);
                    if (!folderExists)
                        Directory.CreateDirectory(path);

                    if (PackageImage != null)
                    {
                        string ext = Path.GetExtension(PackageImage.FileName);
                        string f_name = Path.GetFileNameWithoutExtension(PackageImage.FileName);

                        fileName = f_name + "-" + Guid.NewGuid() + ext;
                        PackageImage.SaveAs(path + fileName);
                    }
                    else
                    {
                        fileName = objPackage.PackageImage;
                    }

                    #endregion Validation

                    #region UpdateUser

                    objPackage.PackageName = packageVM.PackageName;
                    objPackage.PackageAmount = packageVM.PackageAmount;
                    objPackage.PackageImage = fileName;

                    objPackage.UpdatedDate = DateTime.UtcNow;
                    objPackage.UpdatedBy = LoggedInUserId;

                    _db.SaveChanges();

                    return RedirectToAction("Index");

                    #endregion UpdateUser
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(packageVM);
        }

        [HttpPost]
        public string DeletePackage(int PackageId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_Package objPackage = _db.tbl_Package.Where(x => x.PackageId == PackageId).FirstOrDefault();

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

        [HttpPost]
        public string ChangeStatus(long Id, string Status)
        {
            string ReturnMessage = "";
            try
            {
                tbl_Package objPackage = _db.tbl_Package.Where(x => x.PackageId == Id).FirstOrDefault();

                if (objPackage != null)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    if (Status == "Active")
                    {
                        objPackage.IsActive = true;
                    }
                    else
                    {
                        objPackage.IsActive = false;
                    }

                    objPackage.UpdatedBy = LoggedInUserId;
                    objPackage.UpdatedDate = DateTime.UtcNow;

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

        public ActionResult Inquiry()
        {
            List<PackageInquiryVM> lstPackage = new List<PackageInquiryVM>();

            try
            {
                lstPackage = (from m in _db.tbl_PackageBuyDetails 
                              join p in _db.tbl_Package on m.PackageId equals p.PackageId
                              select new PackageInquiryVM
                              {
                                  PackageBuyDetailId = m.PackageBuyDetailId,
                                  FullName = m.FullName,
                                  EmailId = m.EmailId,
                                  MobileNo = m.MobileNo,
                                  Message = m.Message,
                                  CreatedDate = m.CreatedDate,
                                  PackageId = m.PackageId,
                                  PackageName = p.PackageName, 
                              }).ToList();
            }
            catch (Exception ex)
            {
            }

            return View(lstPackage);
        }


        public ActionResult ViewInquiry(long Id)
        {
            PackageInquiryVM objPackage = new PackageInquiryVM();

            try
            {
                objPackage = (from m in _db.tbl_PackageBuyDetails
                              join p in _db.tbl_Package on m.PackageId equals p.PackageId
                              where m.PackageBuyDetailId == Id
                              select new PackageInquiryVM
                              {
                                  PackageBuyDetailId = m.PackageBuyDetailId,
                                  FullName = m.FullName,
                                  EmailId = m.EmailId,
                                  MobileNo = m.MobileNo,
                                  Message = m.Message,
                                  CreatedDate = m.CreatedDate,
                                  PackageId = m.PackageId,
                                  PackageName = p.PackageName,
                              }).FirstOrDefault();
            }
            catch (Exception ex)
            {
            }

            return View(objPackage);
        }


    }
}