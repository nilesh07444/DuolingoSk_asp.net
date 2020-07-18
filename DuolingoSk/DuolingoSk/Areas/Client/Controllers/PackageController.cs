using DuolingoSk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DuolingoSk.Areas.Client.Controllers
{
    public class PackageController : Controller
    {
        private readonly DuolingoSk_Entities _db;

        public PackageController()
        {
            _db = new DuolingoSk_Entities(); 
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Detail(int Id)
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
        public string SubmitPackageForm(FormCollection frm)
        {
            string ReturnMessage = "";
            try
            {
                string Name = frm["name"];
                string EmailId = frm["emailid"];
                string MobileNo = frm["mobileno"];
                string Message = frm["message"];
                int PackageId = Convert.ToInt32(frm["packageid"]);

                tbl_PackageBuyDetails objDetail = new tbl_PackageBuyDetails();
                objDetail.FullName = Name;
                objDetail.EmailId = EmailId;
                objDetail.MobileNo = MobileNo;
                objDetail.Message = Message;
                objDetail.PackageId = PackageId;
                objDetail.CreatedDate = DateTime.UtcNow;
                _db.tbl_PackageBuyDetails.Add(objDetail);
                _db.SaveChanges();

                ReturnMessage = "SUCCESS";

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