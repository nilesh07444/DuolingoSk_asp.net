using DuolingoSk.Filters;
using DuolingoSk.Helper;
using DuolingoSk.Model;
using DuolingoSk.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
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

        public ActionResult Index(int agentid = -1,string status ="-1",string startdate="",string enddate="")
        {
            List<StudentFeeVM> lstFee = new List<StudentFeeVM>();

            try
            {
                int LoggedInUserId = Int32.Parse(clsAdminSession.UserID.ToString());

                bool IsAgent = (clsAdminSession.RoleID == (int)AdminRoles.Agent);
                DateTime dtStart = DateTime.MinValue;
                DateTime dtEnd = DateTime.MaxValue;
                if (!string.IsNullOrEmpty(startdate))
                {
                    dtStart = DateTime.ParseExact(startdate, "dd/MM/yyyy", null);
                }

                if (!string.IsNullOrEmpty(enddate))
                {
                    dtEnd = DateTime.ParseExact(enddate, "dd/MM/yyyy", null);
                }

                lstFee = (from a in _db.tbl_StudentFee
                          join s in _db.tbl_Students on a.StudentId equals s.StudentId
                          join u in _db.tbl_AdminUsers on s.AdminUserId equals u.AdminUserId into outerAgent
                          from agent in outerAgent.DefaultIfEmpty()
                          where !a.IsDeleted && (agentid == -1 || s.AdminUserId == agentid) && (status == "-1" ||a.FeeStatus == status) && a.RequestedDate >= dtStart && a.RequestedDate <= dtEnd
                          && (
                                !IsAgent
                                    || (s.AdminUserId == LoggedInUserId)
                            )
                          select new StudentFeeVM
                          {
                              StudentFeeId = a.StudentFeeId,
                              StudentId = a.StudentId,
                              FeeStatus = a.FeeStatus,
                              FeeAmount = a.FeeAmount,
                              TotalExamAttempt = a.TotalExamAttempt,
                              RequestedDate = a.RequestedDate,
                              StudentName = s.FullName,
                              AgentName = (agent != null ? agent.FirstName + " " + agent.LastName : ""),
                              PackageName = a.PackageName
                          }).ToList();

                ViewData["AgentList"] = _db.tbl_AdminUsers.Where(x => x.AdminRoleId == (int)AdminRoles.Agent && !x.IsDeleted)
                        .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.AdminUserId).Trim(), Text = o.FirstName+" "+o.LastName})
                        .OrderBy(x => x.Text).ToList();

                ViewBag.agentid = agentid;
                ViewBag.status = status;
                ViewBag.startdate = startdate;
                ViewBag.enddate = enddate;

                ViewBag.IsAgent = IsAgent;

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