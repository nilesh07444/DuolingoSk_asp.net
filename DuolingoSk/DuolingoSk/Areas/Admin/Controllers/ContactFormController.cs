using DuolingoSk.Filters;
using DuolingoSk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DuolingoSk.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class ContactFormController : Controller
    {
        private readonly DuolingoSk_Entities _db;

        public ContactFormController()
        {
            _db = new DuolingoSk_Entities();
        }

        public ActionResult Index()
        {
            List<tbl_ContactForm> lstContactForm = _db.tbl_ContactForm.OrderByDescending(x => x.CreatedDate).ToList();
            return View(lstContactForm);
        }
        public ActionResult Detail(int Id)
        {
            tbl_ContactForm objContactForm = _db.tbl_ContactForm.Where(x => x.ContactFormId == Id).FirstOrDefault();
            return View(objContactForm);
        }
    }
}