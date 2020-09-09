﻿using DuolingoSk.Model;
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

        public ActionResult ExamLevels()
        {
            int Levl = 0;
            List<tbl_QuestionLevel> lstQuestionLevel = new List<tbl_QuestionLevel>();
            List<long> lvlIdsUsed = new List<long>();
            ViewBag.Message = "";
            if (clsClientSession.UserID > 0)
            {
                var objStudent = _db.tbl_Students.Where(o => o.StudentId == clsClientSession.UserID).FirstOrDefault();
                if (objStudent != null)
                {
                    DateTime dtNowUtc = DateTime.UtcNow.Date;
                    var objtblstudfee = _db.tbl_StudentFee.Where(o => o.StudentId == clsClientSession.UserID && o.FeeStatus == "Complete" && (o.IsAttemptUsed == null || o.IsAttemptUsed == false) && o.FeeExpiryDate != null && o.FeeExpiryDate >= dtNowUtc).FirstOrDefault();
                    if (objtblstudfee != null)
                    {
                        long pkgid = Convert.ToInt64(objtblstudfee.PackageId);
                        var objpkkg = _db.tbl_Package.Where(o => o.PackageId == pkgid).FirstOrDefault();
                        lstQuestionLevel = _db.tbl_QuestionLevel.ToList().Take(objpkkg.MaxLevel.Value).ToList();
                        lvlIdsUsed = _db.tbl_Exam.Where(o => o.StudentFeeId == objtblstudfee.StudentFeeId).OrderBy(x => x.ExamDate).ToList().Select(x => x.QuestionLevelId.Value).ToList();
                    }
                    else
                    {
                        ViewBag.Message = "Your package is expired or your exam attempts are finish";
                    }
                }

                ViewData["lstQuestionLevel"] = lstQuestionLevel;

                List<tbl_Exam> lstResults = _db.tbl_Exam.Where(o => o.IsDeleted == false && o.ResultStatus == 2).GroupBy(p => p.StudentId)
                                               .Select(g => g.OrderByDescending(p => p.Overall.HasValue ? p.Overall.Value : 0)
                                                             .FirstOrDefault()
                                                ).ToList();

                List<StudentVM> lststud = new List<StudentVM>();
                if (lstResults != null && lstResults.Count() > 0)
                {
                    var lstexmresu = lstResults.ToList().Take(10).ToList();

                    foreach (var objexm in lstexmresu)
                    {
                        var stud = _db.tbl_Students.Where(o => o.StudentId == objexm.StudentId).FirstOrDefault();
                        if (stud != null)
                        {
                            StudentVM objstud = new StudentVM();
                            objstud.StudentId = stud.StudentId;
                            objstud.MaxScore = objexm.Overall.Value;
                            objstud.FullName = stud.FullName;
                            objstud.ProfilePicture = stud.ProfilePicture;
                            lststud.Add(objstud);
                        }
                    }
                }

                lststud = lststud.OrderByDescending(x => x.MaxScore).ToList();

                ViewData["lststud"] = lststud;
                ViewData["lvlIdsUsed"] = lvlIdsUsed;
                return View("~/Areas/Client/Views/StartExam/ExamLevel.cshtml");
            }
            else
            {
                return RedirectToRoute("Client_Login");
            }

        }

        [HttpPost]
        // GET: Client/StartExam
        public ActionResult Index(int levelid)
        {
            if (clsClientSession.UserID > 0)
            {
                var objStudent = _db.tbl_Students.Where(o => o.StudentId == clsClientSession.UserID).FirstOrDefault();
                ViewBag.FullName = objStudent.FullName;
                //ViewBag.Dob = objStudent.Dob != null ? objStudent.Dob.Value.ToString() : "";

                //List<tbl_QuestionLevel> lstQuestionLevel = _db.tbl_QuestionLevel.ToList().Take(Levl).ToList();                
                ViewBag.LevelId = levelid;
                var objLevl = _db.tbl_QuestionLevel.Where(o => o.Level_Id == levelid).FirstOrDefault();
                ViewBag.LevelName = objLevl.LevelName;
                return View();
            }
            else
            {
                return RedirectToRoute("Client_Login");
            }

        }

        [HttpGet]
        // GET: Client/StartExam
        public ActionResult Index()
        {
            return RedirectToRoute("Client_MyExamsLevel");
        }

        public ActionResult Demo()
        {

            if (clsClientSession.UserID > 0)
            {
                var objStudent = _db.tbl_Students.Where(o => o.StudentId == clsClientSession.UserID).FirstOrDefault();

                ViewBag.FullName = objStudent.FullName;
                //ViewBag.Dob = objStudent.Dob != null ? objStudent.Dob.Value.ToString() : "";
                List<tbl_QuestionLevel> lstQuestionLevel = _db.tbl_QuestionLevel.ToList().Take(1).ToList();
                ViewData["lstQuestionLevel"] = lstQuestionLevel;
                return View();
            }
            else
            {
                return RedirectToRoute("Client_Login");
            }

        }

        public ActionResult StartNow(long LevelId)
        {
            long LoggedInStudentId = Convert.ToInt64(clsClientSession.UserID);
            var objStudnt = _db.tbl_Students.Where(o => o.StudentId == LoggedInStudentId).FirstOrDefault();
            long agntid = 0;
            string returnvl = "";
            if (objStudnt != null && objStudnt.AdminUserId != null)
            {
                agntid = objStudnt.AdminUserId.Value;
            }
            ViewBag.DisplayFeedBack = "no";
            var objFeeback = _db.tbl_Feedback.Where(o => o.StudentId == LoggedInStudentId).FirstOrDefault();
            if (objFeeback == null)
            {
                ViewBag.DisplayFeedBack = "yes";
            }
            DateTime dtNowUtc = DateTime.UtcNow;
            var objtblstudfee = _db.tbl_StudentFee.Where(o => o.StudentId == clsClientSession.UserID && o.FeeStatus == "Complete" && (o.IsAttemptUsed == null || o.IsAttemptUsed == false) && o.FeeExpiryDate != null && o.FeeExpiryDate >= dtNowUtc).FirstOrDefault();
            if (objtblstudfee != null)
            {
                tbl_Exam objExm = new tbl_Exam();
                objExm.StudentId = LoggedInStudentId;
                objExm.ExamDate = DateTime.UtcNow;
                objExm.AgentId = agntid;
                objExm.Score = "";
                objExm.ResultText = "";
                objExm.IsDeleted = false;
                objExm.QuestionLevelId = Convert.ToInt64(LevelId);
                objExm.ResultStatus = 1;
                objExm.StudentFeeId = objtblstudfee.StudentFeeId;
                _db.tbl_Exam.Add(objExm);
                _db.SaveChanges();
                int cnt = _db.tbl_Exam.Where(o => o.StudentFeeId == objtblstudfee.StudentFeeId).ToList().Count();
                if (cnt == objtblstudfee.TotalExamAttempt)
                {
                    objtblstudfee.IsAttemptUsed = true;
                    _db.SaveChanges();
                }
                returnvl = objExm.Exam_Id + "";
            }

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
                                                     NoOfWords = p.NoOfWords.HasValue ? p.NoOfWords.Value : 0,
                                                     PreparationTime = p.PreparationTime.HasValue ? p.PreparationTime.Value : 0,
                                                     MinimumTime = p.MinimumTime.HasValue ? p.MinimumTime.Value : 0,
                                                     Images = p.Images,
                                                     QuestionsHtml = p.QuestionsHtml
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
            ViewBag.ExamId = returnvl;
            return PartialView("~/Areas/Client/Views/StartExam/_StartNow.cshtml");
        }

        public ActionResult StartNowDemo(long LevelId)
        {
            long agntid = 0;
            string returnvl = "";
            ViewBag.DisplayFeedBack = "no";

            Session["lstQuestionsExamDemo"] = null;
            if (Session["lstQuestionsExamDemo"] == null && clsClientSession.IsDemoUsed == false)
            {
                if (clsClientSession.UserID > 0)
                {
                    var objStudent = _db.tbl_Students.Where(o => o.StudentId == clsClientSession.UserID).FirstOrDefault();
                    objStudent.IsDemoUsed = true;
                    clsClientSession.IsDemoUsed = true;
                    _db.SaveChanges();
                }
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
                                                     NoOfWords = p.NoOfWords.HasValue ? p.NoOfWords.Value : 0,
                                                     PreparationTime = p.PreparationTime.HasValue ? p.PreparationTime.Value : 0,
                                                     MinimumTime = p.MinimumTime.HasValue ? p.MinimumTime.Value : 0,
                                                     Images = p.Images,
                                                     QuestionsHtml = p.QuestionsHtml
                                                 }).OrderByDescending(x => x.QuestionId).ToList();

                lstQuestions.Where(x => x.QuestionTypeId == 5).ToList().ForEach(x => x.Mp3Options = GetMp3Options(x.QuestionId));
                //  List<QuestionVM> lstQue = new List<QuestionVM>();
                //  foreach (long TypId in TypIds)
                //  {
                //     lstQue.Add(lstQuestions.Where(x => x.QuestionTypeId == TypId).OrderBy(x => Guid.NewGuid()).ToList().FirstOrDefault());
                // }
                lstQuestions = lstQuestions.OrderBy(x => Guid.NewGuid()).ToList();
                ViewData["lstQue"] = lstQuestions;
                Session["lstQuestionsExamDemo"] = lstQuestions;
            }
            else
            {
                ViewData["lstQue"] = Session["lstQuestionsExamDemo"] as List<QuestionVM>;
            }
            ViewBag.ExamId = returnvl;
            return PartialView("~/Areas/Client/Views/StartExam/_StartNowDemo.cshtml");
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
            if (objQ.QuestionTypeId == 1)
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
            else if (objQ.QuestionTypeId == 6)
            {
                return PartialView("~/Areas/Client/Views/StartExam/_QuestionType6.cshtml", objQ);
            }
            else if (objQ.QuestionTypeId == 7)
            {
                return PartialView("~/Areas/Client/Views/StartExam/_QuestionType7.cshtml", objQ);
            }
            else if (objQ.QuestionTypeId == 8)
            {
                return PartialView("~/Areas/Client/Views/StartExam/_QuestionType8.cshtml", objQ);
            }
            else if (objQ.QuestionTypeId == 9)
            {
                return PartialView("~/Areas/Client/Views/StartExam/_QuestionType9.cshtml", objQ);
            }
            else if (objQ.QuestionTypeId == 10)
            {
                return PartialView("~/Areas/Client/Views/StartExam/_QuestionType10.cshtml", objQ);
            }
            else
            {
                return PartialView("~/Areas/Client/Views/StartExam/_QuestionType11.cshtml", objQ);
            }
        }

        public ActionResult GetQuestionByIdDemo(int QueId)
        {
            QuestionVM objQ = new QuestionVM();
            if (Session["lstQuestionsExamDemo"] != null)
            {
                List<QuestionVM> lstQ = Session["lstQuestionsExamDemo"] as List<QuestionVM>;
                objQ = lstQ.Where(o => o.QuestionId == QueId).FirstOrDefault();
            }
            if (objQ.QuestionTypeId == 1)
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
            else if (objQ.QuestionTypeId == 6)
            {
                return PartialView("~/Areas/Client/Views/StartExam/_QuestionType6.cshtml", objQ);
            }
            else if (objQ.QuestionTypeId == 7)
            {
                return PartialView("~/Areas/Client/Views/StartExam/_QuestionType7.cshtml", objQ);
            }
            else if (objQ.QuestionTypeId == 8)
            {
                return PartialView("~/Areas/Client/Views/StartExam/_QuestionType8.cshtml", objQ);
            }
            else if (objQ.QuestionTypeId == 9)
            {
                return PartialView("~/Areas/Client/Views/StartExam/_QuestionType9.cshtml", objQ);
            }
            else if (objQ.QuestionTypeId == 10)
            {
                return PartialView("~/Areas/Client/Views/StartExam/_QuestionType10.cshtml", objQ);
            }
            else
            {
                return PartialView("~/Areas/Client/Views/StartExam/_QuestionType11.cshtml", objQ);
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

            if (lstExam != null && lstExam.Count() > 0)
            {
                foreach (var objj in lstExam)
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

        [HttpPost]
        public string SaveExamStartResult(string LevelId)
        {
            long LoggedInStudentId = Convert.ToInt64(clsClientSession.UserID);
            var objStudnt = _db.tbl_Students.Where(o => o.StudentId == LoggedInStudentId).FirstOrDefault();
            long agntid = 0;
            string returnvl = "";
            if (objStudnt != null && objStudnt.AdminUserId != null)
            {
                agntid = objStudnt.AdminUserId.Value;
            }

            var objtblstudfee = _db.tbl_StudentFee.Where(o => o.StudentId == clsClientSession.UserID && o.FeeStatus == "Complete" && (o.IsAttemptUsed == null || o.IsAttemptUsed == false)).FirstOrDefault();
            if (objtblstudfee != null)
            {
                tbl_Exam objExm = new tbl_Exam();
                objExm.StudentId = LoggedInStudentId;
                objExm.ExamDate = DateTime.UtcNow;
                objExm.AgentId = agntid;
                objExm.Score = "";
                objExm.ResultText = "";
                objExm.IsDeleted = false;
                objExm.QuestionLevelId = Convert.ToInt64(LevelId);
                objExm.ResultStatus = 1;
                objExm.StudentFeeId = objtblstudfee.StudentFeeId;
                _db.tbl_Exam.Add(objExm);
                _db.SaveChanges();
                int cnt = _db.tbl_StudentFee.Where(o => o.StudentFeeId == objtblstudfee.StudentFeeId).ToList().Count();
                if (cnt == objtblstudfee.TotalExamAttempt)
                {
                    objtblstudfee.IsAttemptUsed = true;
                    _db.SaveChanges();
                }
                returnvl = objExm.Exam_Id + "";
            }

            return returnvl;
        }

        [HttpPost]
        public ActionResult SaveExamResultPartially(IList<ExamResultVM> lstExam)
        {
            long LoggedInStudentId = Convert.ToInt64(clsClientSession.UserID);

            if (lstExam != null && lstExam.Count() > 0)
            {
                foreach (var objj in lstExam)
                {
                    tbl_ExamResultDetails objExre = new tbl_ExamResultDetails();
                    objExre.ExamId = objj.ExamId;
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
            //Session["lstQuestionsExam"] = null;
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

            return Json(Convert.ToString(Request.Form[0]));
        }

        [HttpPost]
        public string CheckLevel(string LevelId)
        {
            string ReturnMessage = "";
            try
            {
                long qlvl = Convert.ToInt64(LevelId);
                if (clsClientSession.UserID > 0)
                {
                    DateTime dtNowUtc = DateTime.UtcNow;
                    ReturnMessage = "Success";
                    var objtblstudfee = _db.tbl_StudentFee.Where(o => o.StudentId == clsClientSession.UserID && o.FeeStatus == "Complete" && (o.IsAttemptUsed == null || o.IsAttemptUsed == false) && o.FeeExpiryDate != null && o.FeeExpiryDate >= dtNowUtc).FirstOrDefault();
                    if (objtblstudfee == null)
                    {
                        ReturnMessage = "You don't have any remaining exam attempts. Please renew your account to give more exams.";
                    }
                    else
                    {
                        var lstq = _db.tbl_QuestionsMaster.Where(o => o.QuestionLevel == qlvl).ToList();
                        if (lstq != null && lstq.Count() > 0)
                        {
                            var tblexmp = _db.tbl_Exam.Where(o => o.StudentId == clsClientSession.UserID && o.StudentFeeId == objtblstudfee.StudentFeeId && o.QuestionLevelId == qlvl).FirstOrDefault();
                            if (tblexmp != null)
                            {
                                ReturnMessage = "You have already used this selected Exam Level. Please use different Level.";
                            }
                        }
                        else
                        {
                            ReturnMessage = "This selected Exam Level has not questions. Please use different Level.";
                        }

                    }

                }

            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }

        [HttpPost]
        public ActionResult SaveFeedBack(IList<ExamResultVM> lstExam)
        {
            long LoggedInStudentId = Convert.ToInt64(clsClientSession.UserID);

            if (lstExam != null && lstExam.Count() > 0)
            {
                foreach (var objj in lstExam)
                {
                    tbl_Feedback objfeed = new tbl_Feedback();
                    objfeed.ExamId = objj.ExamId;
                    objfeed.StudentId = LoggedInStudentId;
                    objfeed.FeedbackQuestion = objj.QuestionTxt;
                    objfeed.FeedbackAnswer = objj.Ans;
                    objfeed.FeedBackDate = DateTime.UtcNow;
                    _db.tbl_Feedback.Add(objfeed);

                }
                _db.SaveChanges();
            }
            //Session["lstQuestionsExam"] = null;
            //objExm.
            // this line convert the json to a list of your type, exactly what you want.
            //IList<ExamResultVM> ctm =
            // new JavaScriptSerializer().Deserialize<IList<ExamResultVM>>(jsonstr);

            return Json("");
        }

        public ActionResult GetFeedbackForm()
        {
            return PartialView("~/Areas/Client/Views/StartExam/_FeedBack1.cshtml");
        }
    }

    public class ClsExamResult
    {
        List<ExamResultVM> lstExamResult { get; set; }
    }
}