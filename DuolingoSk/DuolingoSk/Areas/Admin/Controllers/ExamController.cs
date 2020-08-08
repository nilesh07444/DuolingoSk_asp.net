using DuolingoSk.Helper;
using DuolingoSk.Model;
using DuolingoSk.Models;
using HiQPdf;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.IO;
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
        public ActionResult Index(int level = -1, string status = "-1")
        {
            List<ExamVM> lstExams = new List<ExamVM>();
            int resulstatusid = 2;
            if(status == "Pending")
            {
                resulstatusid = 1;
            }

            lstExams = (from e in _db.tbl_Exam
                        join l in _db.tbl_QuestionLevel on e.QuestionLevelId equals l.Level_Id
                        join s in _db.tbl_Students on e.StudentId equals s.StudentId
                        join a in _db.tbl_AdminUsers on s.AdminUserId equals a.AdminUserId into outerAgent
                        from agent in outerAgent.DefaultIfEmpty() 
                        where (level == -1 || e.QuestionLevelId == level) && (status == "-1" || e.ResultStatus == resulstatusid)
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

            ViewData["LevelList"] = _db.tbl_QuestionLevel.Where(x => x.IsDeleted == false)
                     .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.Level_Id).Trim(), Text = o.LevelName})
                     .OrderBy(x => x.Text).ToList();
            ViewBag.levelid = level;
            ViewBag.status = status;
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
        public string SaveExamResult(long ExamId,string Literacy, string Comprehension, string Conversation, string Production, string Overall)
        {
            string ReturnMessage = "";

            try
            {
                tbl_Exam objEx = _db.tbl_Exam.Where(o => o.Exam_Id == ExamId).FirstOrDefault();
                if(objEx != null)
                {
                    objEx.Literacy = Convert.ToDecimal(Literacy);
                    objEx.Comprehension = Convert.ToDecimal(Comprehension);
                    objEx.Conversation = Convert.ToDecimal(Conversation);
                    objEx.Production = Convert.ToDecimal(Production);
                    objEx.Overall = Convert.ToDecimal(Overall);

                    objEx.ResultStatus = (int)ExamResultStatus.Complete;
                    objEx.ModifiedBy = clsAdminSession.UserID;
                    objEx.ModifiedDate = DateTime.UtcNow;
                    _db.SaveChanges();
                    string meterimg = "https://www.duolingo-ielts-pte.in/Images/150to160.png";
                    if(objEx.Overall >= 40 && objEx.Overall <= 60)
                    {
                        meterimg = "https://www.duolingo-ielts-pte.in/Images/40to60.png";
                    }
                    else if (objEx.Overall > 60 && objEx.Overall <= 80)
                    {
                        meterimg = "https://www.duolingo-ielts-pte.in/Images/60to80.png";
                    }
                    else if (objEx.Overall > 80 && objEx.Overall <= 100)
                    {
                        meterimg = "https://www.duolingo-ielts-pte.in/Images/90to110.png";
                    }
                    else if (objEx.Overall > 100 && objEx.Overall <= 120)
                    {
                        meterimg = "https://www.duolingo-ielts-pte.in/Images/110to130.png";
                    }
                    else if (objEx.Overall > 120 && objEx.Overall <= 140)
                    {
                        meterimg = "https://www.duolingo-ielts-pte.in/Images/130to150.png";
                    }
                    else if (objEx.Overall > 140 && objEx.Overall <= 160)
                    {
                        meterimg = "https://www.duolingo-ielts-pte.in/Images/150to160.png";
                    }
                    string flName = "Result_"+ objEx.Exam_Id+"_"+ objEx.ExamDate.Value.ToString("ddMMyyyy") + ".pdf";
                    StreamReader sr;
                    string file = Server.MapPath("~/Template/certificate.html");
                    string htmldata = "";
                    FileInfo fi = new FileInfo(file);
                    sr = System.IO.File.OpenText(file);
                    htmldata += sr.ReadToEnd();

                    // create the HTML to PDF converter
                    HtmlToPdf htmlToPdfConverter = new HtmlToPdf();

                    // set browser width
                    htmlToPdfConverter.BrowserWidth = 1200;

                    // set PDF page size and orientation
                    htmlToPdfConverter.Document.PageSize = PdfPageSize.A4; 
                    htmlToPdfConverter.Document.PageOrientation = PdfPageOrientation.Portrait;
                    
                    // set PDF page margins
                    htmlToPdfConverter.Document.Margins = new PdfMargins(5);

                    // convert HTML code
                    htmldata = htmldata.Replace("--OVERALL--", Overall).Replace("--Literacy--", Literacy).Replace("--Comprehension--", Comprehension).Replace("--Conversation--", Conversation).Replace("--Production--", Production).Replace("--METERIMG--", meterimg);

                    // convert HTML code to a PDF memory buffer
                    htmlToPdfConverter.ConvertHtmlToFile(htmldata,"", Server.MapPath("~/Certificates/") + flName);
                    
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