using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DuolingoSk.Filters;
using DuolingoSk.Helper;
using DuolingoSk.Model;
using DuolingoSk.Models;

namespace DuolingoSk.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class DashboardController : Controller
    {
        private readonly DuolingoSk_Entities _db;
        public DashboardController()
        {
            _db = new DuolingoSk_Entities();
        }
        public ActionResult Index()
        {
            int LoggedInUserId = Int32.Parse(clsAdminSession.UserID.ToString());

            bool IsAgent = (clsAdminSession.RoleID == (int)AdminRoles.Agent);

            DashboardCountVM obj = new DashboardCountVM();

            obj.TotalAgents = _db.tbl_AdminUsers.Where(x => !x.IsDeleted && x.AdminRoleId == (int)AdminRoles.Agent).ToList().Count;

            obj.TotalStudents = (from s in _db.tbl_Students
                                 where !s.IsDeleted
                                 && (
                                        !IsAgent
                                        || (s.AdminUserId == LoggedInUserId)
                                    )
                                 select s
                                 ).ToList().Count;

            obj.TotalPendingFees = (from f in _db.tbl_StudentFee
                                    join s in _db.tbl_Students on f.StudentId equals s.StudentId
                                    where !f.IsDeleted && f.FeeStatus == "Pending"
                                    && (
                                        !IsAgent
                                        || (s.AdminUserId == LoggedInUserId)
                                    )
                                    select f
                                    ).ToList().Sum(x => x.FeeAmount);

            obj.TotalPendingExams = _db.tbl_Exam.Where(x => x.ResultStatus == (int)ExamResultStatus.Pending).ToList().Count;

            return View(obj);

        }
    }
}