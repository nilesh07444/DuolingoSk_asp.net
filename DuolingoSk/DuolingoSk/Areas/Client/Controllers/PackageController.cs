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

    }
}