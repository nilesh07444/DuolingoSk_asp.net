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
                    ViewBag.TotalExamAttempt = objSetting.TotalExamAttempt;

                }
                else
                {
                    // Admin Student

                    ViewBag.RenewFee = objSetting.RenewFee;
                    ViewBag.TotalExamAttempt = objSetting.TotalExamAttempt;

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
                              IsAttemptUsed = a.IsAttemptUsed
                          }).OrderBy(x => x.RequestedDate).ToList();

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
                                                             where s.FeeStatus == "Complete" && s.IsAttemptUsed != true && s.FeeExpiryDate > todayDate
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

                    decimal? RenewFee = 0;
                    decimal? TotalExamAttempt = 0;

                    tbl_Students objStudent = _db.tbl_Students.Where(x => x.StudentId == LoggedInStudentId).FirstOrDefault();

                    tbl_GeneralSetting objSetting = _db.tbl_GeneralSetting.FirstOrDefault();
                    if (objStudent.AdminUserId != null && objStudent.AdminUserId > 0)
                    {
                        // Agent Student
                        tbl_AdminUsers objAgent = _db.tbl_AdminUsers.Where(x => x.AdminUserId == objStudent.AdminUserId).FirstOrDefault();
                        RenewFee = objAgent.StudentRenewFee;
                        TotalExamAttempt = objSetting.TotalExamAttempt;
                    }
                    else
                    {
                        // Admin Student 
                        RenewFee = objSetting.RenewFee;
                        TotalExamAttempt = objSetting.TotalExamAttempt;
                    }

                    tbl_StudentFee objStudentFee1 = new tbl_StudentFee();
                    objStudentFee1.StudentId = objStudent.StudentId;
                    objStudentFee1.FeeStatus = "Pending";
                    objStudentFee1.FeeAmount = Convert.ToDecimal(RenewFee);
                    objStudentFee1.TotalExamAttempt = Convert.ToInt32(TotalExamAttempt);
                    objStudentFee1.IsDeleted = false;
                    objStudentFee1.RequestedDate = DateTime.UtcNow;
                    _db.tbl_StudentFee.Add(objStudentFee1);
                    _db.SaveChanges();

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