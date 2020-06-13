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
        public ActionResult Index(int Type = -1)
        {
          // List<tbl_QuestionsMaster> lstQuestions = _db.tbl_QuestionsMaster.Where(o => Type == -1 || o.QuestionTypeId == Type).ToList();
           List<QuestionVM> lstQuestions = (from p in _db.tbl_QuestionsMaster
                                            join Q in _db.tbl_QuestionType on p.QuestionTypeId.Value equals Q.QuestionTypeId
                                            where p.IsDeleted == false && (Type == -1 || p.QuestionTypeId == Type)
                                            select new QuestionVM
                                            {
                                               QuestionText = p.QuestionText ,
                                               QuestionTypeText = Q.QuestionTypeName,                                                                                                                                             
                                               IsActive = p.IsActive.Value,
                                               QuestionId = p.QuestionId                                               
                                            }).OrderByDescending(x => x.QuestionId).ToList();
            List<tbl_QuestionType> lstQuestionType = _db.tbl_QuestionType.ToList();
            ViewData["lstQuestionType"] = lstQuestionType;
            ViewBag.Type = Type;
            return View(lstQuestions);
        }
        public ActionResult Add()
        {
            List<tbl_QuestionType> lstQuestionType = _db.tbl_QuestionType.ToList();
            ViewData["lstQuestionType"] = lstQuestionType;
            return View();
        }

        [HttpPost]
        public ActionResult AddQuestion(FormCollection frm)
        {
            int QuestionType = Convert.ToInt32(frm["QuestionTypeId"]);
            int minutes = Convert.ToInt32(frm["minutes"]);
            int seconds = Convert.ToInt32(frm["seconds"]);
            string QuestionText = frm["QuestionText"].ToString();
            tbl_QuestionsMaster objtbl_QuestionsMaster = new tbl_QuestionsMaster();
            objtbl_QuestionsMaster.QuestionTypeId = QuestionType;
            objtbl_QuestionsMaster.QuestionText = QuestionText;
            objtbl_QuestionsMaster.QuestionTime = (minutes * 60) + seconds;
            objtbl_QuestionsMaster.IsActive = true;
            objtbl_QuestionsMaster.IsDeleted = false;
            objtbl_QuestionsMaster.CreatedDate = DateTime.UtcNow;
            objtbl_QuestionsMaster.ModifiedDate = DateTime.UtcNow;
            objtbl_QuestionsMaster.CreatedBy = clsAdminSession.UserID;
            string pathmp3 = Server.MapPath("~/QuestionMp3/");
            string pathImg = Server.MapPath("~/QuestionImage/");
            List<string> filenmsmp3 = new List<string>();
            if (QuestionType == 1)
            {
                string optiontext = frm["QuestionOptionText"].ToString();
                objtbl_QuestionsMaster.QuestionOptionText = optiontext;
            }
            else if (QuestionType == 2)
            {
                if(frm["EngWords"] != null)
                {
                    string wordss = frm["EngWords"].ToString();
                    objtbl_QuestionsMaster.Words = wordss;
                }               
            }
            else if (QuestionType == 3)
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
            for (int i = 0; i < Request.Files.Count; i++)
            {
                if (QuestionType == 3 && Request.Files.GetKey(i) == "mp3file")
                {
                    HttpPostedFileBase fileUpload = Request.Files.Get(i);
                    string mp3nm = Guid.NewGuid() + "-" + Path.GetFileName(fileUpload.FileName);
                    fileUpload.SaveAs(pathmp3 + mp3nm);
                    objtbl_QuestionsMaster.Mp3FileName = mp3nm;
                }
                if (QuestionType == 4 && Request.Files.GetKey(i) == "imagefile")
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
             
            }
            _db.tbl_QuestionsMaster.Add(objtbl_QuestionsMaster);
            _db.SaveChanges();
            if(QuestionType == 5)
            {
                foreach(string str in filenmsmp3)
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
            return View();
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