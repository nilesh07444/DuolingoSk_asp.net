using DuolingoSk.Model;
using DuolingoSk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DuolingoSk.Areas.Client.Controllers
{
    public class MyExamsController : Controller
    {
        private readonly DuolingoSk_Entities _db;

        public MyExamsController()
        {
            _db = new DuolingoSk_Entities();
        }
        // GET: Client/MyExams
        public ActionResult Index()
        {
            List<ExamVM> lstExams = new List<ExamVM>();

            long LoggedInStudentId = Convert.ToInt64(clsClientSession.UserID);

            lstExams = (from e in _db.tbl_Exam
                        join l in _db.tbl_QuestionLevel on e.QuestionLevelId equals l.Level_Id
                        join s in _db.tbl_Students on e.StudentId equals s.StudentId
                        join a in _db.tbl_AdminUsers on s.AdminUserId equals a.AdminUserId into outerAgent
                        from agent in outerAgent.DefaultIfEmpty()
                        where e.StudentId == LoggedInStudentId
                        select new ExamVM
                        {
                            Exam_Id = e.Exam_Id,
                            ExamDate = e.ExamDate,
                            StudentId = e.StudentId,
                            StudentName = s.FirstName + " " + s.LastName,
                            AgentName = (agent != null ? agent.FirstName + " " + agent.LastName : ""),
                            LevelName = l.LevelName,
                            ResultStatus = e.ResultStatus,
                            Score = e.Score
                        }).ToList();

            return View(lstExams);
        }
    }
}