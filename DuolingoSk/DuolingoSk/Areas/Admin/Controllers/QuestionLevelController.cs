using DuolingoSk.Filters;
using DuolingoSk.Helper;
using DuolingoSk.Model;
using DuolingoSk.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DuolingoSk.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class QuestionLevelController : Controller
    {
        private readonly DuolingoSk_Entities _db; 

        public QuestionLevelController()
        {
            _db = new DuolingoSk_Entities(); 
        }

        public ActionResult Index()
        {
            List<QuestionLevelVM> lstLevels = new List<QuestionLevelVM>();

            try
            {
                lstLevels = (from a in _db.tbl_QuestionLevel
                                 where a.IsDeleted == false
                                 select new QuestionLevelVM
                                 {
                                     Level_Id = a.Level_Id,
                                     LevelName = a.LevelName
                                 }).OrderBy(x=>x.Level_Id).ToList();
            }
            catch (Exception ex)
            {
            }

            return View(lstLevels);
        }
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(QuestionLevelVM levelVM)
        {

            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    #region Validation

                    // Validate duplicate MobileNo 
                    tbl_QuestionLevel duplicateLevelName = _db.tbl_QuestionLevel.Where(x => x.LevelName.ToLower() == levelVM.LevelName.ToLower() && x.IsDeleted == false).FirstOrDefault();
                    if (duplicateLevelName != null)
                    {
                        ModelState.AddModelError("LevelName", ErrorMessage.LevelNameExists);
                        return View(levelVM);
                    }
                     

                    #endregion Validation

                    #region CreateLevel

                    tbl_QuestionLevel objAdminUser = new tbl_QuestionLevel(); 
                    objAdminUser.LevelName = levelVM.LevelName;  
                    objAdminUser.IsDeleted = false;
                    objAdminUser.CreatedDate = DateTime.UtcNow;
                    objAdminUser.CreatedBy = LoggedInUserId;
                    _db.tbl_QuestionLevel.Add(objAdminUser);
                    _db.SaveChanges();
                     
                    return RedirectToAction("Index");

                    #endregion CreateLevel
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

            return View(levelVM);
        }

        public ActionResult Edit(long Id)
        {
            QuestionLevelVM lstLevels = new QuestionLevelVM();

            try
            {
                lstLevels = (from a in _db.tbl_QuestionLevel
                             where a.Level_Id == Id
                             select new QuestionLevelVM
                             {
                                 Level_Id = a.Level_Id,
                                 LevelName = a.LevelName
                             }).FirstOrDefault();
            }
            catch (Exception ex)
            {
            }

            return View(lstLevels);
        }

        [HttpPost]
        public ActionResult Edit(QuestionLevelVM levelVM)
        {

            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    #region Validation

                    // Validate duplicate Level Name 
                    tbl_QuestionLevel duplicateLevelName = _db.tbl_QuestionLevel.Where(x => x.LevelName.ToLower() == levelVM.LevelName.ToLower() 
                                                                && x.Level_Id != levelVM.Level_Id && x.IsDeleted == false).FirstOrDefault();
                    if (duplicateLevelName != null)
                    {
                        ModelState.AddModelError("LevelName", ErrorMessage.LevelNameExists);
                        return View(levelVM);
                    }
                     
                    #endregion Validation

                    #region CreateLevel

                    tbl_QuestionLevel objAdminUser = _db.tbl_QuestionLevel.Where(x=>x.Level_Id == levelVM.Level_Id).FirstOrDefault();
                    if (objAdminUser != null)
                    {
                        objAdminUser.LevelName = levelVM.LevelName;
                        _db.SaveChanges();
                    }

                    return RedirectToAction("Index");

                    #endregion CreateLevel
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

            return View(levelVM);
        }

    }
}