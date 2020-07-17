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
    public class ExamController : Controller
    {
        private readonly DuolingoSk_Entities _db;

        public ExamController()
        {
            _db = new DuolingoSk_Entities();           
        }
        // GET: Admin/Exam
        public ActionResult Index()
        {
            List<ExamVM> lstExams = new List<ExamVM>();

            lstExams = (from e in _db.tbl_Exam
                        join l in _db.tbl_QuestionLevel on e.QuestionLevelId equals l.Level_Id
                        join s in _db.tbl_Students on e.StudentId equals s.StudentId
                        join a in _db.tbl_AdminUsers on s.AdminUserId equals a.AdminUserId into outerAgent
                        from agent in outerAgent.DefaultIfEmpty() 
                        select new ExamVM
                        {
                            Exam_Id = e.Exam_Id,
                            ExamDate = e.ExamDate,
                            StudentId = e.StudentId,
                            StudentName = s.FirstName + " "+ s.LastName,
                            AgentName = (agent != null ? agent.FirstName + " " + agent.LastName : ""),
                            LevelName = l.LevelName,
                            ResultStatus = e.ResultStatus,
                            Score = e.Score
                        }).OrderByDescending(x => x.ExamDate).ToList();

            return View(lstExams);
        }

        public ActionResult Detail(int Id)
        {
            StudentVM objStudent = (from s in _db.tbl_Students
                            join ex in _db.tbl_Exam on s.StudentId equals ex.StudentId                            
                            where ex.Exam_Id == Id
                            select new StudentVM
                            {
                                FirstName = s.FirstName,
                                StudentId = s.StudentId,
                                LastName = s.LastName,
                                Email = s.Email,
                                MobileNo = s.MobileNo,
                                ProfilePicture = s.ProfilePicture,
                                IsActive = s.IsActive,
                                CreatedDate = ex.ExamDate.Value       
                            }).FirstOrDefault();
            tbl_Exam objexx = _db.tbl_Exam.Where(o => o.Exam_Id == Id).FirstOrDefault();
            List<tbl_ExamResultDetails> lstExamsDetls =_db.tbl_ExamResultDetails.Where(o => o.ExamId == Id).ToList();
            ViewData["lstExamsDetls"] = lstExamsDetls;
            ViewData["objStudent"] = objStudent;
            ViewData["objexx"] = objexx;
           List<tbl_Feedback> lstFeedbk = new List<tbl_Feedback>();
            lstFeedbk = _db.tbl_Feedback.Where(o => o.ExamId == Id && o.StudentId == objStudent.StudentId).ToList();
            ViewData["feedback"] = lstFeedbk;
            return View();
        }

        [HttpPost]
        public string SaveExamResult(long ExamId,string ResultScore,string ResultText)
        {
            string ReturnMessage = "";

            try
            {
                tbl_Exam objEx = _db.tbl_Exam.Where(o => o.Exam_Id == ExamId).FirstOrDefault();
                if(objEx != null)
                {
                    objEx.Score = ResultScore;
                    objEx.ResultText = ResultText;
                    objEx.ResultStatus = (int)ExamResultStatus.Complete;
                    objEx.ModifiedBy = clsAdminSession.UserID;
                    objEx.ModifiedDate = DateTime.UtcNow;
                    _db.SaveChanges();
                }
                ReturnMessage = "Success";
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