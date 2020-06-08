using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DuolingoSk.Areas.Client.Controllers
{
    public class HomePageController : Controller
    {
        // GET: Client/HomePage
        public ActionResult Index()
        {
            return View();
        }
    }
}