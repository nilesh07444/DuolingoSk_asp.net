using DuolingoSk.Filters;
using DuolingoSk.Model;
using DuolingoSk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DuolingoSk.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class CouponCodeController : Controller
    {
        private readonly DuolingoSk_Entities _db;
        public CouponCodeController()
        {
            _db = new DuolingoSk_Entities();
        }

        public ActionResult Index()
        {
            List<CouponCodeVM> lstCouponCode = new List<CouponCodeVM>();

            try
            {
                lstCouponCode = (from m in _db.tbl_CouponCode
                              where !m.IsDeleted
                              select new CouponCodeVM
                              {
                                  CouponCodeId = m.CouponCodeId,
                                  CouponCode = m.CouponCode,
                                  DiscountPercentage = m.DiscountPercentage,
                                  TotalMaxUsage = m.TotalMaxUsage,
                                  dtExpiryDate = m.ExpiryDate,
                                  IsActive = m.IsActive
                              }).ToList();
            }
            catch (Exception ex)
            {
            }

            return View(lstCouponCode);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(CouponCodeVM couponcodeVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                      
                    #region CreateCoupon

                    tbl_CouponCode objCouponCode = new tbl_CouponCode();
                    objCouponCode.CouponCode = couponcodeVM.CouponCode;
                    objCouponCode.DiscountPercentage = couponcodeVM.DiscountPercentage;
                    objCouponCode.TotalMaxUsage = couponcodeVM.TotalMaxUsage;
                      
                    if (!string.IsNullOrEmpty(couponcodeVM.ExpiryDate))
                    {
                        DateTime exp_date = DateTime.ParseExact(couponcodeVM.ExpiryDate, "dd/MM/yyyy", null);
                        objCouponCode.ExpiryDate = exp_date;
                    }

                    objCouponCode.IsActive = true;
                    objCouponCode.IsDeleted = false;
                    objCouponCode.CreatedDate = DateTime.UtcNow;
                    objCouponCode.CreatedBy = LoggedInUserId;
                    _db.tbl_CouponCode.Add(objCouponCode);
                    _db.SaveChanges();

                    return RedirectToAction("Index");

                    #endregion CreateCoupon
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

            return View(couponcodeVM);
        }

        public ActionResult Edit(long Id)
        {
            CouponCodeVM objCouponCode = new CouponCodeVM();

            try
            {
                objCouponCode = (from m in _db.tbl_CouponCode
                              where m.CouponCodeId == Id
                              select new CouponCodeVM
                              {
                                  CouponCodeId = m.CouponCodeId,
                                  CouponCode = m.CouponCode,
                                  DiscountPercentage = m.DiscountPercentage,
                                  TotalMaxUsage = m.TotalMaxUsage,
                                  dtExpiryDate = m.ExpiryDate, 
                                  IsActive = m.IsActive
                              }).FirstOrDefault();

                if (objCouponCode.dtExpiryDate != null)
                {
                    objCouponCode.ExpiryDate = Convert.ToDateTime(objCouponCode.dtExpiryDate).ToString("dd/MM/yyyy");
                }

            }
            catch (Exception ex)
            {
            }

            return View(objCouponCode);
        }

        [HttpPost]
        public ActionResult Edit(CouponCodeVM couponcodeVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    #region UpdateCoupon

                    tbl_CouponCode objCouponCode = _db.tbl_CouponCode.Where(x => x.CouponCodeId == couponcodeVM.CouponCodeId).FirstOrDefault();

                    objCouponCode.CouponCode = couponcodeVM.CouponCode;
                    objCouponCode.DiscountPercentage = couponcodeVM.DiscountPercentage;
                    objCouponCode.TotalMaxUsage = couponcodeVM.TotalMaxUsage;

                    if (!string.IsNullOrEmpty(couponcodeVM.ExpiryDate))
                    {
                        DateTime exp_date = DateTime.ParseExact(couponcodeVM.ExpiryDate, "dd/MM/yyyy", null);
                        objCouponCode.ExpiryDate = exp_date;
                    }

                    objCouponCode.UpdatedDate = DateTime.UtcNow;
                    objCouponCode.UpdatedBy = LoggedInUserId;

                    _db.SaveChanges();

                    return RedirectToAction("Index");

                    #endregion UpdateCoupon
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(couponcodeVM);
        }

        [HttpPost]
        public string DeleteCouponCode(int CouponCodeId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_CouponCode objCouponCode = _db.tbl_CouponCode.Where(x => x.CouponCodeId == CouponCodeId).FirstOrDefault();

                if (objCouponCode == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    objCouponCode.IsDeleted = true;
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
                tbl_CouponCode objCouponCode = _db.tbl_CouponCode.Where(x => x.CouponCodeId == Id).FirstOrDefault();

                if (objCouponCode != null)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    if (Status == "Active")
                    {
                        objCouponCode.IsActive = true;
                    }
                    else
                    {
                        objCouponCode.IsActive = false;
                    }

                    objCouponCode.UpdatedBy = LoggedInUserId;
                    objCouponCode.UpdatedDate = DateTime.UtcNow;

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