using DuolingoSk.Model;
using DuolingoSk.Models;
using DuolingoSk.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace DuolingoSk.Areas.Client.Controllers
{
    public class StartExamController : Controller
    {
        private readonly DuolingoSk_Entities _db;
        public StartExamController()
        {
            _db = new DuolingoSk_Entities();
        }
        // GET: Client/StartExam
        public ActionResult Index()
        {
            List<tbl_QuestionLevel> lstQuestionLevel = _db.tbl_QuestionLevel.ToList().Take(10).ToList();
            ViewData["lstQuestionLevel"] = lstQuestionLevel;
            //if(Session["lstQuestionsExam"] == null)
            //{
            //    List<long> TypIds = _db.tbl_QuestionType.ToList().Select(x => x.QuestionTypeId).ToList();
            //    List<QuestionVM> lstQuestions = (from p in _db.tbl_QuestionsMaster
            //                                     where p.IsDeleted == false && p.IsActive == true
            //                                     select new QuestionVM
            //                                     {
            //                                         QuestionText = p.QuestionText,
            //                                         QuestionTime = p.QuestionTime.Value,
            //                                         QuestionId = p.QuestionId,
            //                                         QuestionTypeId = p.QuestionTypeId.Value,
            //                                         QuestionOptionText = p.QuestionOptionText,
            //                                         Words = p.Words,
            //                                         Mp3FileName = p.Mp3FileName,
            //                                         MaxReplay = p.MaxReplay.HasValue ? p.MaxReplay.Value : 0,
            //                                         ImageName = p.ImageName,
            //                                         NoOfWords = p.NoOfWords.HasValue ? p.NoOfWords.Value : 0
            //                                     }).OrderByDescending(x => x.QuestionId).ToList();

            //    lstQuestions.Where(x => x.QuestionTypeId == 5).ToList().ForEach(x => x.Mp3Options = GetMp3Options(x.QuestionId));
            //    List<QuestionVM> lstQue = new List<QuestionVM>();
            //    foreach (long TypId in TypIds)
            //    {
            //        lstQue.Add(lstQuestions.Where(x => x.QuestionTypeId == TypId).OrderBy(x => Guid.NewGuid()).ToList().FirstOrDefault());
            //    }
            //    lstQue = lstQue.OrderBy(x => Guid.NewGuid()).ToList();
            //    ViewData["lstQue"] = lstQue;
            //    Session["lstQuestionsExam"] = lstQue;
            //}
            //else
            //{
            //    ViewData["lstQue"] = Session["lstQuestionsExam"] as List<QuestionVM>;
            //}           
            return View();
        }

        public ActionResult StartNow(long LevelId)
        {
            Session["lstQuestionsExam"] = null;
            if (Session["lstQuestionsExam"] == null)
            {
                List<long> TypIds = _db.tbl_QuestionType.ToList().Select(x => x.QuestionTypeId).ToList();
                List<QuestionVM> lstQuestions = (from p in _db.tbl_QuestionsMaster
                                                 where p.IsDeleted == false && p.IsActive == true && p.QuestionLevel == LevelId
                                                 select new QuestionVM
                                                 {
                                                     QuestionText = p.QuestionText,
                                                     QuestionTime = p.QuestionTime.Value,
                                                     QuestionId = p.QuestionId,
                                                     QuestionTypeId = p.QuestionTypeId.Value,
                                                     QuestionOptionText = p.QuestionOptionText,
                                                     Words = p.Words,
                                                     LevelId = p.QuestionLevel.Value,
                                                     Mp3FileName = p.Mp3FileName,
                                                     MaxReplay = p.MaxReplay.HasValue ? p.MaxReplay.Value : 0,
                                                     ImageName = p.ImageName,
                                                     NoOfWords = p.NoOfWords.HasValue ? p.NoOfWords.Value : 0
                                                 }).OrderByDescending(x => x.QuestionId).ToList();

                  lstQuestions.Where(x => x.QuestionTypeId == 5).ToList().ForEach(x => x.Mp3Options = GetMp3Options(x.QuestionId));
                //  List<QuestionVM> lstQue = new List<QuestionVM>();
                //  foreach (long TypId in TypIds)
                //  {
                //     lstQue.Add(lstQuestions.Where(x => x.QuestionTypeId == TypId).OrderBy(x => Guid.NewGuid()).ToList().FirstOrDefault());
                // }
                lstQuestions = lstQuestions.OrderBy(x => Guid.NewGuid()).ToList();
                ViewData["lstQue"] = lstQuestions;
                Session["lstQuestionsExam"] = lstQuestions;
            }
            else
            {
                ViewData["lstQue"] = Session["lstQuestionsExam"] as List<QuestionVM>;
            }
            
            return PartialView("~/Areas/Client/Views/StartExam/_StartNow.cshtml");
        }

        public List<Mp3OptionsVM> GetMp3Options(long QuestionId)
        {
            List<Mp3OptionsVM> lstQuestionsMp3 = (from p in _db.tbl_Mp3Options
                                             where p.QuestionId == QuestionId
                                             select new Mp3OptionsVM
                                             {
                                                 Mp3OptionId = p.Mp3OptionId,
                                                 Mp3FileName = p.Mp3FileName                                                 
                                             }).ToList();

            return lstQuestionsMp3;
        }

        public ActionResult GetQuestionById(int QueId)
        {
            QuestionVM objQ = new QuestionVM();
            if (Session["lstQuestionsExam"] != null)
            {
                List<QuestionVM> lstQ = Session["lstQuestionsExam"] as List<QuestionVM>;
                objQ = lstQ.Where(o => o.QuestionId == QueId).FirstOrDefault();
            }
            if(objQ.QuestionTypeId == 1)
            {
                return PartialView("~/Areas/Client/Views/StartExam/_QuestionType1.cshtml", objQ);
            }
            else if (objQ.QuestionTypeId == 2)
            {
                return PartialView("~/Areas/Client/Views/StartExam/_QuestionType2.cshtml", objQ);
            }
            else if (objQ.QuestionTypeId == 3)
            {
                return PartialView("~/Areas/Client/Views/StartExam/_QuestionType3.cshtml", objQ);
            }
            else if (objQ.QuestionTypeId == 4)
            {
                return PartialView("~/Areas/Client/Views/StartExam/_QuestionType4.cshtml", objQ);
            }
            else if (objQ.QuestionTypeId == 5)
            {
                return PartialView("~/Areas/Client/Views/StartExam/_QuestionType5.cshtml", objQ);
            }
            else
            {
                return PartialView("~/Areas/Client/Views/StartExam/_QuestionType6.cshtml", objQ);
            }
        }
        [HttpPost]
        public ActionResult SaveExamResult(IList<ExamResultVM> lstExam)
        {
            long LoggedInStudentId = Convert.ToInt64(clsClientSession.UserID);

            tbl_Exam objExm = new tbl_Exam();
            objExm.StudentId = LoggedInStudentId;
            objExm.ExamDate = DateTime.UtcNow;
            objExm.AgentId = 1;
            objExm.Score = "";
            objExm.ResultText = "";
            objExm.IsDeleted = false;
            objExm.QuestionLevelId = 1;
            objExm.ResultStatus = 1;
            _db.tbl_Exam.Add(objExm);
            _db.SaveChanges();

            if(lstExam != null && lstExam.Count() > 0)
            {
                foreach(var objj in lstExam)
                {
                    tbl_ExamResultDetails objExre = new tbl_ExamResultDetails();
                    objExre.ExamId = objExm.Exam_Id;
                    objExre.StudentId = LoggedInStudentId;
                    objExre.QuestionText = objj.QuestionTxt;
                    objExre.QuestionOptionValue = objj.Que;
                    objExre.QuestionTypeId = objj.QuestionType;
                    objExre.QuestionOptiont_Ans = objj.Ans;
                    objExre.CreatedDate = DateTime.UtcNow;
                    _db.tbl_ExamResultDetails.Add(objExre);
                }
                _db.SaveChanges();
            }
            Session["lstQuestionsExam"] = null;
            //objExm.
            // this line convert the json to a list of your type, exactly what you want.
            //IList<ExamResultVM> ctm =
            // new JavaScriptSerializer().Deserialize<IList<ExamResultVM>>(jsonstr);

            return Json("");
        }

        public ActionResult PostRecordedAudioVideo()
        {            
            var path = Server.MapPath("~/TempFile/");
           
            foreach (string upload in Request.Files)
            {
                //AppDomain.CurrentDomain.BaseDirectory + "uploads/";
                
                var file = Request.Files[upload];
                if (file == null) continue;

                file.SaveAs(System.IO.Path.Combine(path, Request.Form[0]));
                var bytes = System.IO.File.ReadAllBytes(System.IO.Path.Combine(path, Request.Form[0]));
                // lstFiles.Add(Request.Form[0]);               

            }

            return Json(path + Request.Form[0]);
        }
    }

    public class ClsExamResult
    {
        List<ExamResultVM> lstExamResult { get; set; }
    }
}