using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DuolingoSk.Models;

namespace DuolingoSk.Areas.Client.Controllers
{
    public class TipsController : Controller
    {
        private readonly DuolingoSk_Entities _db;
        public TipsController()
        {
            _db = new DuolingoSk_Entities();
        }

        public ActionResult Index()
        {
            List<MaterialVM> lstMaterials = new List<MaterialVM>();

            try
            {
                lstMaterials = (from m in _db.tbl_Materials
                                where m.IsActive && !m.IsDeleted && m.MaterialType == 2
                                select new MaterialVM
                                {
                                    MaterialId = m.MaterialId,
                                    MaterialTitle = m.MaterialTitle,
                                    MaterialFileName = m.MaterialFile
                                }).ToList();
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(lstMaterials);

        }

    }
}