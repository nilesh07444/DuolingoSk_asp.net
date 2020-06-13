using DuolingoSk.Filters;
using DuolingoSk.Helper;
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
    public class FeesController : Controller
    {
        private readonly DuolingoSk_Entities _db;
        public string UserProfileDirectoryPath = "";

        public FeesController()
        {
            _db = new DuolingoSk_Entities();
            UserProfileDirectoryPath = ErrorMessage.UserProfileDirectoryPath;
        }

        public ActionResult Index()
        {
            List<StudentFeeVM> lstFee = new List<StudentFeeVM>();

            try
            {
                lstFee = (from a in _db.tbl_StudentFee
                          join s in _db.tbl_Students on a.StudentId equals s.StudentId
                          join u in _db.tbl_AdminUsers on s.AdminUserId equals u.AdminUserId into outerAgent
                          from agent in outerAgent.DefaultIfEmpty()

                          where !a.IsDeleted
                          select new StudentFeeVM
                          {
                              StudentFeeId = a.StudentFeeId,
                              StudentId = a.StudentId,
                              FeeStatus = a.FeeStatus,
                              FeeAmount = a.FeeAmount,
                              TotalExamAttempt = a.TotalExamAttempt,
                              RequestedDate = a.RequestedDate,
                              StudentName = s.FirstName + " " + s.LastName,
                              AgentName = (agent != null ? agent.FirstName + " " + agent.LastName : "")
                          }).ToList();

            }
            catch (Exception ex)
            {
            }

            return View(lstFee);
        }

        [HttpPost]
        public string MarkCompleteFeeStatus(long StudentFeeId)
        {
            string ReturnMessage = "";
            try
            {
                tbl_StudentFee objFee = _db.tbl_StudentFee.Where(x => x.StudentFeeId == StudentFeeId).FirstOrDefault();

                if (objFee != null)
                {
                    int LoggedInUserId = Int32.Parse(clsAdminSession.UserID.ToString());

                    objFee.FeeStatus = "Complete";
                    objFee.MarkCompleteBy = LoggedInUserId;
                    objFee.MarkCompleteDate = DateTime.UtcNow;

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