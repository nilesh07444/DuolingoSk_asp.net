using DuolingoSk.Model;
using DuolingoSk.Models;
using DuolingoSk.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DuolingoSk.Areas.Admin.Controllers
{
    public class QuestionController : Controller
    {
        private readonly DuolingoSk_Entities _db;
        public QuestionController()
        {
            _db = new DuolingoSk_Entities();
        }

        // GET: Admin/Question
        public ActionResult Index(int Type = -1, int Level = -1)
        {
            List<QuestionVM> lstQuestions = (from p in _db.tbl_QuestionsMaster
                                             join Q in _db.tbl_QuestionType on p.QuestionTypeId.Value equals Q.QuestionTypeId
                                             join l in _db.tbl_QuestionLevel on (long)p.QuestionLevel equals l.Level_Id
                                             where p.IsDeleted == false
                                             && (Type == -1 || p.QuestionTypeId == Type)
                                             && (Level == -1 || p.QuestionLevel == Level)
                                             select new QuestionVM
                                             {
                                                 QuestionText = p.QuestionText,
                                                 QuestionTypeText = Q.QuestionTypeName,
                                                 IsActive = p.IsActive.Value,
                                                 QuestionId = p.QuestionId,
                                                 LevelName = l.LevelName
                                             }).OrderByDescending(x => x.QuestionId).ToList();

            List<tbl_QuestionType> lstQuestionType = _db.tbl_QuestionType.OrderBy(x => x.QuestionTypeName).ToList();
            ViewData["lstQuestionType"] = lstQuestionType;
            ViewBag.Type = Type;

            List<tbl_QuestionLevel> lstQuestionLevel = _db.tbl_QuestionLevel.ToList();
            ViewData["lstQuestionLevel"] = lstQuestionLevel;
            ViewBag.Level = Level;
             
            return View(lstQuestions);
        }
        public ActionResult Add()
        {
            List<tbl_QuestionType> lstQuestionType = _db.tbl_QuestionType.ToList();
            ViewData["lstQuestionType"] = lstQuestionType;
            List<tbl_QuestionLevel> lstQuestionLevel = _db.tbl_QuestionLevel.ToList();
            ViewData["lstQuestionLevel"] = lstQuestionLevel;
            return View();
        }

        [HttpPost,ValidateInput(false)]
        public ActionResult AddQuestion(FormCollection frm)
        {
            int QuestionType = Convert.ToInt32(frm["QuestionTypeId"]);
            int minutes = Convert.ToInt32(frm["minutes"]);
            int seconds = Convert.ToInt32(frm["seconds"]);
            string QuestionText = frm["QuestionText"].ToString();
            string questionss = frm["qcardtextques"].ToString();
            tbl_QuestionsMaster objtbl_QuestionsMaster = new tbl_QuestionsMaster();
            objtbl_QuestionsMaster.QuestionTypeId = QuestionType;
            objtbl_QuestionsMaster.QuestionText = QuestionText;
            objtbl_QuestionsMaster.QuestionTime = (minutes * 60) + seconds;
            objtbl_QuestionsMaster.IsActive = true;
            objtbl_QuestionsMaster.IsDeleted = false;
            objtbl_QuestionsMaster.CreatedDate = DateTime.UtcNow;
            objtbl_QuestionsMaster.ModifiedDate = DateTime.UtcNow;
            objtbl_QuestionsMaster.QuestionLevel = Convert.ToInt32(frm["QuestionLevelId"]);
            objtbl_QuestionsMaster.CreatedBy = clsAdminSession.UserID;
            string pathmp3 = Server.MapPath("~/QuestionMp3/");
            string pathImg = Server.MapPath("~/QuestionImage/");
            List<string> filenmsmp3 = new List<string>();
            List<string> filenmsimgs = new List<string>();
            if (QuestionType == 1)
            {
                string optiontext = frm["QuestionOptionText"].ToString();
                objtbl_QuestionsMaster.QuestionOptionText = optiontext;
            }
            else if (QuestionType == 2)
            {
                if (frm["EngWords"] != null)
                {
                    string wordss = frm["EngWords"].ToString();
                    objtbl_QuestionsMaster.Words = wordss;
                }
            }
            else if (QuestionType == 3 || QuestionType == 9)
            {
                if (frm["MaxReplay"] != null)
                {
                    string MaxReplay = frm["MaxReplay"].ToString();
                    objtbl_QuestionsMaster.MaxReplay = Convert.ToInt32(MaxReplay);
                }
            }
            else if (QuestionType == 6)
            {
                if (frm["QuestionOptionText"] != null)
                {
                    string QuestionOptionText = frm["QuestionOptionText"].ToString();
                    objtbl_QuestionsMaster.QuestionOptionText = QuestionOptionText;
                }
                if (frm["noofwords"] != null)
                {
                    string noofwords = frm["noofwords"].ToString();
                    objtbl_QuestionsMaster.NoOfWords = Convert.ToInt32(noofwords);
                }

            }
            else if(QuestionType == 11)
            {                
                objtbl_QuestionsMaster.QuestionsHtml = questionss;
            }
            
            if(QuestionType == 7 || QuestionType == 8 || QuestionType == 9 || QuestionType == 10 || QuestionType == 11)
            {
                if (frm["QuestionOptionText"] != null)
                {
                    string QuestionOptionText = frm["QuestionOptionText"].ToString();
                    objtbl_QuestionsMaster.QuestionOptionText = QuestionOptionText;
                }
                if (QuestionType == 8 || QuestionType == 9 || QuestionType == 10 || QuestionType == 11)
                {
                    int minutespre = Convert.ToInt32(frm["minutespre"]);
                    int secondspre = Convert.ToInt32(frm["secondspre"]);
                    objtbl_QuestionsMaster.PreparationTime = (minutespre * 60) + secondspre;
                }

                if(QuestionType == 9 || QuestionType == 10 || QuestionType == 11) 
                {
                    int minutesmin = Convert.ToInt32(frm["minutesmin"]);
                    int secondsmin = Convert.ToInt32(frm["secondsmin"]);
                    objtbl_QuestionsMaster.MinimumTime = (minutesmin * 60) + secondsmin;
                }
            }

            

            for (int i = 0; i < Request.Files.Count; i++)
            {
                if ((QuestionType == 3 || QuestionType == 9) && Request.Files.GetKey(i) == "mp3file")
                {
                    HttpPostedFileBase fileUpload = Request.Files.Get(i);
                    string mp3nm = Guid.NewGuid() + "-" + Path.GetFileName(fileUpload.FileName);
                    fileUpload.SaveAs(pathmp3 + mp3nm);
                    objtbl_QuestionsMaster.Mp3FileName = mp3nm;
                }
                if ((QuestionType == 4 || QuestionType == 10) && Request.Files.GetKey(i) == "imagefile")
                {
                    HttpPostedFileBase fileUpload = Request.Files.Get(i);
                    string mp3nm = Guid.NewGuid() + "-" + Path.GetFileName(fileUpload.FileName);
                    fileUpload.SaveAs(pathImg + mp3nm);
                    objtbl_QuestionsMaster.ImageName = mp3nm;
                }
                if (QuestionType == 5 && Request.Files.GetKey(i) == "mp3fileeword")
                {
                    HttpPostedFileBase fileUpload = Request.Files.Get(i);
                    string mp3nm = Guid.NewGuid() + "-" + Path.GetFileName(fileUpload.FileName);
                    fileUpload.SaveAs(pathmp3 + mp3nm);
                    filenmsmp3.Add(mp3nm);
                }

                if (QuestionType == 8 && Request.Files.GetKey(i) == "imgfl")
                {
                    HttpPostedFileBase fileUpload = Request.Files.Get(i);
                    string mp3nm = Guid.NewGuid() + "-" + Path.GetFileName(fileUpload.FileName);
                    fileUpload.SaveAs(pathImg + mp3nm);
                    filenmsimgs.Add(mp3nm);
                }
            }

            if(QuestionType == 8)
            {
                objtbl_QuestionsMaster.Images = String.Join("^", filenmsimgs);
            }
            _db.tbl_QuestionsMaster.Add(objtbl_QuestionsMaster);
            _db.SaveChanges();
            if (QuestionType == 5)
            {
                foreach (string str in filenmsmp3)
                {
                    tbl_Mp3Options objMp3 = new tbl_Mp3Options();
                    objMp3.QuestionId = objtbl_QuestionsMaster.QuestionId;
                    objMp3.Mp3FileName = str;
                    _db.tbl_Mp3Options.Add(objMp3);
                    _db.SaveChanges();
                }
            }
            return RedirectToAction("Add");
        }
        public ActionResult Edit(int Id)
        {
            tbl_QuestionsMaster objQue = _db.tbl_QuestionsMaster.Where(o => o.QuestionId == Id).FirstOrDefault();
            List<string> mp3files = _db.tbl_Mp3Options.Where(o => o.QuestionId == Id).Select(x => x.Mp3FileName).ToList();
            ViewData["mp3files"] = mp3files;
            List<tbl_QuestionType> lstQuestionType = _db.tbl_QuestionType.ToList();
            ViewData["lstQuestionType"] = lstQuestionType;
            List<tbl_QuestionLevel> lstQuestionLevel = _db.tbl_QuestionLevel.ToList();
            ViewData["lstQuestionLevel"] = lstQuestionLevel;
            TimeSpan ts = TimeSpan.FromSeconds(objQue.QuestionTime.Value);
            int Minues = ts.Minutes;
            int seconds = ts.Seconds;
            ViewBag.Minutes = Minues;
            ViewBag.Seconds = seconds;
            ViewBag.PreMinutes = 0;
            ViewBag.PreSeconds = 0;
            ViewBag.minminute = 0;
            ViewBag.miSeconds = 0;
            if (objQue.QuestionTypeId == 8 || objQue.QuestionTypeId == 9 || objQue.QuestionTypeId == 10 || objQue.QuestionTypeId == 11)
            {
                TimeSpan ts1 = TimeSpan.FromSeconds(objQue.PreparationTime.Value);
                int prpminutes = ts1.Minutes;
                int preseconds = ts1.Seconds;
                ViewBag.PreMinutes = prpminutes;
                ViewBag.PreSeconds = preseconds;
                if(objQue.QuestionTypeId == 9 || objQue.QuestionTypeId == 10 || objQue.QuestionTypeId == 11) 
                {
                    TimeSpan ts2 = TimeSpan.FromSeconds(objQue.MinimumTime.Value);
                    int minminutes = ts2.Minutes;
                    int minseconds = ts2.Seconds;
                    ViewBag.minminute = minminutes;
                    ViewBag.miSeconds = minseconds;
                }
            }
            return View(objQue);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EditQuestion(FormCollection frm)
        {
            int QuestionType = Convert.ToInt32(frm["QuestionTypeId"]);
            int QuestionId = Convert.ToInt32(frm["QuestionId"]);
            int minutes = Convert.ToInt32(frm["minutes"]);
            int seconds = Convert.ToInt32(frm["seconds"]);            
            string QuestionText = frm["QuestionText"].ToString();
            string questionss = frm["qcardtextques"].ToString();
            tbl_QuestionsMaster objtbl_QuestionsMaster = _db.tbl_QuestionsMaster.Where(o => o.QuestionId == QuestionId).FirstOrDefault();
            objtbl_QuestionsMaster.QuestionTypeId = QuestionType;
            objtbl_QuestionsMaster.QuestionText = QuestionText;
            objtbl_QuestionsMaster.QuestionLevel = Convert.ToInt32(frm["QuestionLevelId"]);
            objtbl_QuestionsMaster.QuestionTime = (minutes * 60) + seconds;
            objtbl_QuestionsMaster.ModifiedDate = DateTime.UtcNow;
            objtbl_QuestionsMaster.ModifiedBy = clsAdminSession.UserID;
            string pathmp3 = Server.MapPath("~/QuestionMp3/");
            string pathImg = Server.MapPath("~/QuestionImage/");
            List<string> filenmsmp3 = new List<string>();
            List<string> filenmsimgs = new List<string>();
            if (QuestionType == 1)
            {
                string optiontext = frm["QuestionOptionText"].ToString();
                objtbl_QuestionsMaster.QuestionOptionText = optiontext;
            }
            else if (QuestionType == 2)
            {
                if (frm["EngWords"] != null)
                {
                    string wordss = frm["EngWords"].ToString();
                    objtbl_QuestionsMaster.Words = wordss;
                }
            }
            else if (QuestionType == 3 || QuestionType == 9)
            {
                if (frm["MaxReplay"] != null)
                {
                    string MaxReplay = frm["MaxReplay"].ToString();
                    objtbl_QuestionsMaster.MaxReplay = Convert.ToInt32(MaxReplay);
                }
            }
            else if (QuestionType == 6)
            {
                if (frm["QuestionOptionText"] != null)
                {
                    string QuestionOptionText = frm["QuestionOptionText"].ToString();
                    objtbl_QuestionsMaster.QuestionOptionText = QuestionOptionText;
                }
                if (frm["noofwords"] != null)
                {
                    string noofwords = frm["noofwords"].ToString();
                    objtbl_QuestionsMaster.NoOfWords = Convert.ToInt32(noofwords);
                }

            }
            else if (QuestionType == 11)
            {
                objtbl_QuestionsMaster.QuestionsHtml = questionss;
            }

            if (QuestionType == 7 || QuestionType == 8 || QuestionType == 9 || QuestionType == 10 || QuestionType == 11)
            {
                if (frm["QuestionOptionText"] != null)
                {
                    string QuestionOptionText = frm["QuestionOptionText"].ToString();
                    objtbl_QuestionsMaster.QuestionOptionText = QuestionOptionText;
                }
                if (QuestionType == 8 || QuestionType == 9 || QuestionType == 10 || QuestionType == 11)
                {
                    int minutespre = Convert.ToInt32(frm["minutespre"]);
                    int secondspre = Convert.ToInt32(frm["secondspre"]);
                    objtbl_QuestionsMaster.PreparationTime = (minutespre * 60) + secondspre;
                }

                if (QuestionType == 9 || QuestionType == 10 || QuestionType == 11)
                {
                    int minutesmin = Convert.ToInt32(frm["minutesmin"]);
                    int secondsmin = Convert.ToInt32(frm["secondsmin"]);
                    objtbl_QuestionsMaster.MinimumTime = (minutesmin * 60) + secondsmin;
                }
            }

            for (int i = 0; i < Request.Files.Count; i++)
            {
                if ((QuestionType == 3 || QuestionType == 9) && Request.Files.GetKey(i) == "mp3file")
                {
                    HttpPostedFileBase fileUpload = Request.Files.Get(i);
                    if (fileUpload != null && fileUpload.ContentLength > 0)
                    {
                        string mp3nm = Guid.NewGuid() + "-" + Path.GetFileName(fileUpload.FileName);
                        fileUpload.SaveAs(pathmp3 + mp3nm);
                        objtbl_QuestionsMaster.Mp3FileName = mp3nm;
                    }

                }
                if ((QuestionType == 4 || QuestionType == 10) && Request.Files.GetKey(i) == "imagefile")
                {
                    HttpPostedFileBase fileUpload = Request.Files.Get(i);
                    if (fileUpload != null && fileUpload.ContentLength > 0)
                    {
                        string mp3nm = Guid.NewGuid() + "-" + Path.GetFileName(fileUpload.FileName);
                        fileUpload.SaveAs(pathImg + mp3nm);
                        objtbl_QuestionsMaster.ImageName = mp3nm;
                    }
                }
                if (QuestionType == 5 && Request.Files.GetKey(i) == "mp3fileeword")
                {
                    HttpPostedFileBase fileUpload = Request.Files.Get(i);
                    if (fileUpload != null && fileUpload.ContentLength > 0)
                    {
                        string mp3nm = Guid.NewGuid() + "-" + Path.GetFileName(fileUpload.FileName);
                        fileUpload.SaveAs(pathmp3 + mp3nm);
                        filenmsmp3.Add(mp3nm);
                    }
                }

                if (QuestionType == 8 && Request.Files.GetKey(i) == "imgfl")
                {
                    HttpPostedFileBase fileUpload = Request.Files.Get(i);
                    if (fileUpload != null && fileUpload.ContentLength > 0)
                    {
                        string mp3nm = Guid.NewGuid() + "-" + Path.GetFileName(fileUpload.FileName);
                        fileUpload.SaveAs(pathImg + mp3nm);
                        filenmsimgs.Add(mp3nm);
                    }
                   
                }

            }
            //_db.tbl_QuestionsMaster.Add(objtbl_QuestionsMaster);
            
            if (QuestionType == 8)
            {
                if (frm["hdnImgss"] != null)
                {
                    string[] values = Request.Form.GetValues("hdnImgss");
                    filenmsimgs.AddRange(values.ToList());
                }

                objtbl_QuestionsMaster.Images = String.Join("^", filenmsimgs);
            }
            _db.SaveChanges();
            if (QuestionType == 5)
            {
                List<tbl_Mp3Options> lstmp3sc = _db.tbl_Mp3Options.Where(o => o.QuestionId == QuestionId).ToList();
                if (lstmp3sc != null && lstmp3sc.Count() > 0)
                {
                    foreach (var mp3opt in lstmp3sc)
                    {
                        _db.tbl_Mp3Options.Remove(mp3opt);
                    }
                    _db.SaveChanges();
                }

                if (frm["hdnMp3s"] != null)
                {
                    string[] values = Request.Form.GetValues("hdnMp3s");
                    filenmsmp3.AddRange(values.ToList());
                }
                foreach (string str in filenmsmp3)
                {
                    tbl_Mp3Options objMp3 = new tbl_Mp3Options();
                    objMp3.QuestionId = objtbl_QuestionsMaster.QuestionId;
                    objMp3.Mp3FileName = str;
                    _db.tbl_Mp3Options.Add(objMp3);
                    _db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public string ChangeStatus(long Id, string Status)
        {
            string ReturnMessage = "";
            try
            {
                tbl_QuestionsMaster objtbl_Que = _db.tbl_QuestionsMaster.Where(x => x.QuestionId == Id).FirstOrDefault();

                if (objtbl_Que != null)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    if (Status == "Active")
                    {
                        objtbl_Que.IsActive = true;
                    }
                    else
                    {
                        objtbl_Que.IsActive = false;
                    }

                    objtbl_Que.ModifiedBy = LoggedInUserId;
                    objtbl_Que.ModifiedDate = DateTime.UtcNow;

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

        [HttpPost]
        public string DeleteQuestion(long QuestionId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_QuestionsMaster objQue = _db.tbl_QuestionsMaster.Where(x => x.QuestionId == QuestionId && x.IsDeleted == false).FirstOrDefault();

                if (objQue == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    objQue.IsDeleted = true;
                    objQue.ModifiedBy = LoggedInUserId;
                    objQue.ModifiedDate = DateTime.UtcNow;

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