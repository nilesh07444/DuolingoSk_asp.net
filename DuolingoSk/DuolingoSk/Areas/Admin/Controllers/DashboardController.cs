using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DuolingoSk.Filters;
using DuolingoSk.Models;

namespace DuolingoSk.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class DashboardController : Controller
    {
        private readonly DuolingoSk_Entities _db;
        public DashboardController()
        {
            _db = new DuolingoSk_Entities();
        }
        public ActionResult Index()
        {
            DashboardCountVM obj = new DashboardCountVM();

            return View(obj);
        }
    }
}