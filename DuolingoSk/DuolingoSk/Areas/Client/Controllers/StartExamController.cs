using DuolingoSk.Models;
using DuolingoSk.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            if(Session["lstQuestionsExam"] == null)
            {
                List<long> TypIds = _db.tbl_QuestionType.ToList().Select(x => x.QuestionTypeId).ToList();
                List<QuestionVM> lstQuestions = (from p in _db.tbl_QuestionsMaster
                                                 where p.IsDeleted == false && p.IsActive == true
                                                 select new QuestionVM
                                                 {
                                                     QuestionText = p.QuestionText,
                                                     QuestionTime = p.QuestionTime.Value,
                                                     QuestionId = p.QuestionId,
                                                     QuestionTypeId = p.QuestionTypeId.Value,
                                                     QuestionOptionText = p.QuestionOptionText,
                                                     Words = p.Words,
                                                     Mp3FileName = p.Mp3FileName,
                                                     MaxReplay = p.MaxReplay.HasValue ? p.MaxReplay.Value : 0,
                                                     ImageName = p.ImageName,
                                                     NoOfWords = p.NoOfWords.HasValue ? p.NoOfWords.Value : 0
                                                 }).OrderByDescending(x => x.QuestionId).ToList();

                lstQuestions.Where(x => x.QuestionTypeId == 5).ToList().ForEach(x => x.Mp3Options = GetMp3Options(x.QuestionId));
                List<QuestionVM> lstQue = new List<QuestionVM>();
                foreach (long TypId in TypIds)
                {
                    lstQue.Add(lstQuestions.Where(x => x.QuestionTypeId == TypId).OrderBy(x => Guid.NewGuid()).ToList().FirstOrDefault());
                }
                lstQue = lstQue.OrderBy(x => Guid.NewGuid()).ToList();
                ViewData["lstQue"] = lstQue;
                Session["lstQuestionsExam"] = lstQue;
            }
            else
            {
                ViewData["lstQue"] = Session["lstQuestionsExam"] as List<QuestionVM>;
            }           
            return View();
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
    }
}