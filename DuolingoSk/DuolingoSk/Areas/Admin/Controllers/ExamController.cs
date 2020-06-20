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
            return View();
        }

        public ActionResult Detail(int Id)
        {
            StudentVM objStudent = (from s in _db.tbl_Students
                            join ex in _db.tbl_Exam on s.StudentId equals ex.StudentId                            
                            where ex.Exam_Id == Id
                            select new StudentVM
                            {
                                FirstName = s.FirstName,
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
                    objEx.ResultStatus = 2;
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