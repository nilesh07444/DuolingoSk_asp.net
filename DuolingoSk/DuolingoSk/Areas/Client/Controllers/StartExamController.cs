using DuolingoSk.Models;
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
            return View();
        }
    }
}