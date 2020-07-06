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
                          }).OrderBy(x => x.RequestedDate).ToList();

            }
            catch (Exception ex)
            {
            }

            return View(lstFee);
        }
    }
}