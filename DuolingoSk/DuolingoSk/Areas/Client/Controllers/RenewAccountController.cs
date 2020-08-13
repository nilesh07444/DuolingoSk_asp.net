using DuolingoSk.Model;
using DuolingoSk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DuolingoSk.Areas.Client.Controllers
{
    public class RenewAccountController : Controller
    {
        private readonly DuolingoSk_Entities _db;

        public RenewAccountController()
        {
            _db = new DuolingoSk_Entities();
        }
        public ActionResult Index()
        {
            long LoggedInStudentId = Convert.ToInt64(clsClientSession.UserID);

            List<StudentFeeVM> lstFee = new List<StudentFeeVM>();

            try
            {

                tbl_Students objStudent = _db.tbl_Students.Where(x => x.StudentId == LoggedInStudentId).FirstOrDefault();

                tbl_GeneralSetting objSetting = _db.tbl_GeneralSetting.FirstOrDefault();
                if (objStudent.AdminUserId != null && objStudent.AdminUserId > 0)
                {
                    // Agent Student
                    tbl_AdminUsers objAgent = _db.tbl_AdminUsers.Where(x => x.AdminUserId == objStudent.AdminUserId).FirstOrDefault();
                    ViewBag.RenewFee = objAgent.StudentRenewFee; 

                    List<AgentPackageVM> lstPackages = (from s in _db.tbl_AgentPackage
                                                        join c in _db.tbl_Package on s.PackageId equals c.PackageId
                                                        join p in _db.tbl_AdminUsers on s.AgentId equals p.AdminUserId
                                                        where !s.IsDeleted.Value && s.AgentId.Value == objAgent.AdminUserId
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

                }
                else
                {
                    // Admin Student
                     
                    List<AgentPackageVM> lstPackagesnew = (from s in _db.tbl_Package
                                                           where !s.IsDeleted
                                                           select new AgentPackageVM
                                                           {
                                                               PackageId = s.PackageId,
                                                               PackageName = s.PackageName,
                                                               AgentName = "",
                                                               PackageAmountAgent = s.PackageAmount,
                                                               PackageAgentId = 0,
                                                               TotalAttempt = s.TotalAttempt
                                                           }).OrderBy(x => x.PackageName).ToList();

                    ViewData["lstPackages"] = lstPackagesnew.OrderBy(x => x.PackageName).ToList();

                }


                lstFee = (from a in _db.tbl_StudentFee
                          where !a.IsDeleted && a.StudentId == LoggedInStudentId
                          select new StudentFeeVM
                          {
                              StudentFeeId = a.StudentFeeId,
                              StudentId = a.StudentId,
                              FeeStatus = a.FeeStatus,
                              FeeAmount = a.FeeAmount,
                              TotalExamAttempt = a.TotalExamAttempt,
                              RequestedDate = a.RequestedDate,
                              FeeExpiryDate = a.FeeExpiryDate,
                              IsAttemptUsed = a.IsAttemptUsed,
                              TotalWebinarAttempt = a.TotalWebinarAttempt,
                          }).OrderByDescending(x => x.RequestedDate).ToList();

                if (lstFee.Count > 0)
                {
                    lstFee.ForEach(fee =>
                    {
                        if (fee.IsAttemptUsed != true)
                        {
                            //fee.UsedTotalAttempts = getTotalUsedFeeAttempt(fee.StudentFeeId);
                        }
                    });
                }
                ViewData["objStudent"] = objStudent;
            }
            catch (Exception ex)
            {
            }

            return View(lstFee);
        }

        [HttpPost]
        public string RenewMyAccount(FormCollection frm)
        {
            string ReturnMessage = "";
            try
            {
                long LoggedInStudentId = Convert.ToInt64(clsClientSession.UserID);

                // Validation
                tbl_StudentFee objStudentFee = _db.tbl_StudentFee.Where(x => x.StudentId == LoggedInStudentId && x.FeeStatus == "Pending").FirstOrDefault();
                if (objStudentFee != null)
                {
                    ReturnMessage = "ALREADY_PENDING_FOUND";
                    return ReturnMessage;
                }
                else
                {
                    DateTime todayDate = DateTime.UtcNow.Date;

                    // Validation for unused previous remaining attempts
                    tbl_StudentFee objRemainingFeeAttempt = (from s in _db.tbl_StudentFee
                                                             where s.FeeStatus == "Complete" && s.IsAttemptUsed != true && s.FeeExpiryDate > todayDate && s.StudentId == LoggedInStudentId
                                                             select s
                        ).OrderBy(x => x.StudentFeeId).FirstOrDefault();

                    if (objRemainingFeeAttempt != null)
                    {
                        int usedExamCount = _db.tbl_Exam.Where(x => x.StudentFeeId == LoggedInStudentId).ToList().Count;
                        if (usedExamCount < objRemainingFeeAttempt.TotalExamAttempt)
                        {
                            ReturnMessage = "PREVIOUS_EXAM_REMAINING";
                            return ReturnMessage;
                        }
                    }
                     
                    tbl_Students objStudent = _db.tbl_Students.Where(x => x.StudentId == LoggedInStudentId).FirstOrDefault();
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
                            long agntid = -1;
                            if (objStudent.AdminUserId != null && objStudent.AdminUserId > 0)
                            {
                                agntid = objStudent.AdminUserId.Value;
                            }
                            var objAgentPckg = _db.tbl_AgentPackage.Where(o => o.AgentId == agntid && o.PackageId == objPckg.PackageId && o.IsDeleted == false).FirstOrDefault();
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

                            objStudentFee = new tbl_StudentFee();
                            objStudentFee.StudentId = objStudent.StudentId;
                            if (frm["hdnPaymentId"] != null && !string.IsNullOrEmpty(frm["hdnPaymentId"]))
                            {
                                objStudentFee.FeeStatus = "Complete";
                                objStudentFee.MarkCompleteBy = Convert.ToInt32(clsClientSession.UserID);
                                objStudentFee.MarkCompleteDate = DateTime.UtcNow;
                                objStudentFee.Paymentoken = frm["hdnPaymentId"].ToString();
                            }
                            else
                            {
                                objStudentFee.FeeStatus = "Pending";
                            }
                            objStudentFee.FeeAmount = Math.Round(Convert.ToDecimal(PckPriceForPay) - disc, 2);
                            objStudentFee.TotalExamAttempt = Convert.ToInt32(objPckg.TotalAttempt);
                            objStudentFee.TotalWebinarAttempt = objPckg.TotalWebinar != null ? Convert.ToInt32(objPckg.TotalWebinar) : 0;
                            objStudentFee.FeeExpiryDate = exp_date;
                            objStudentFee.OriginalPackagePrice = objPckg.PackageAmount;
                            objStudentFee.Discount = disc;
                            objStudentFee.IsDeleted = false;
                            objStudentFee.RequestedDate = DateTime.UtcNow;

                            objStudentFee.IsAttemptUsed = false;
                            objStudentFee.PackageId = objPckg.PackageId;
                            objStudentFee.PackageName = objPckg.PackageName;
                            objStudentFee.CouponCode = refralcode;
                            objStudentFee.CouponId = copupnid;
                            _db.tbl_StudentFee.Add(objStudentFee);
                            _db.SaveChanges();


                        }
                    }
                    //tbl_GeneralSetting objSetting = _db.tbl_GeneralSetting.FirstOrDefault();
                    //if (objStudent.AdminUserId != null && objStudent.AdminUserId > 0)
                    //{
                    //    // Agent Student
                    //    tbl_AdminUsers objAgent = _db.tbl_AdminUsers.Where(x => x.AdminUserId == objStudent.AdminUserId).FirstOrDefault();
                    //    RenewFee = objAgent.StudentRenewFee;
                    //    TotalExamAttempt = objSetting.TotalExamAttempt;
                    //}
                    //else
                    //{
                    //    // Admin Student 
                    //    RenewFee = objSetting.RenewFee;
                    //    TotalExamAttempt = objSetting.TotalExamAttempt;
                    //}

                    //tbl_StudentFee objStudentFee1 = new tbl_StudentFee();
                    //objStudentFee1.StudentId = objStudent.StudentId;
                    //objStudentFee1.FeeStatus = "Pending";
                    //objStudentFee1.FeeAmount = Convert.ToDecimal(RenewFee);
                    //objStudentFee1.TotalExamAttempt = Convert.ToInt32(TotalExamAttempt);
                    //objStudentFee1.IsDeleted = false;
                    //objStudentFee1.RequestedDate = DateTime.UtcNow;
                    //_db.tbl_StudentFee.Add(objStudentFee1);
                    //_db.SaveChanges();

                    ReturnMessage = "SUCCESS";


                }


            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                ReturnMessage = "ERROR";
                throw ex;
            }

            return ReturnMessage;
        }

        [HttpPost]
        public string CheckRenewMyAccount()
        {
            string ReturnMessage = "";
            try
            {
                long LoggedInStudentId = Convert.ToInt64(clsClientSession.UserID);

                // Validation
                tbl_StudentFee objStudentFee = _db.tbl_StudentFee.Where(x => x.StudentId == LoggedInStudentId && x.FeeStatus == "Pending").FirstOrDefault();
                if (objStudentFee != null)
                {
                    ReturnMessage = "ALREADY_PENDING_FOUND";
                    return ReturnMessage;
                }
                else
                {
                    DateTime todayDate = DateTime.UtcNow.Date;

                    // Validation for unused previous remaining attempts
                    tbl_StudentFee objRemainingFeeAttempt = (from s in _db.tbl_StudentFee
                                                             where s.FeeStatus == "Complete" && s.IsAttemptUsed != true && s.FeeExpiryDate > todayDate && s.StudentId == LoggedInStudentId
                                                             select s
                        ).OrderBy(x => x.StudentFeeId).FirstOrDefault();

                    if (objRemainingFeeAttempt != null)
                    {
                        int usedExamCount = _db.tbl_Exam.Where(x => x.StudentFeeId == LoggedInStudentId).ToList().Count;
                        if (usedExamCount < objRemainingFeeAttempt.TotalExamAttempt)
                        {
                            ReturnMessage = "PREVIOUS_EXAM_REMAINING";
                            return ReturnMessage;
                        }
                    }

                    ReturnMessage = "SUCCESS";
                }


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