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
           List<tbl_ExamResultDetails> lstExamsDetls =_db.tbl_ExamResultDetails.Where(o => o.ExamId == Id).ToList();
            ViewData["lstExamsDetls"] = lstExamsDetls;
            return View();
        }
    }
}