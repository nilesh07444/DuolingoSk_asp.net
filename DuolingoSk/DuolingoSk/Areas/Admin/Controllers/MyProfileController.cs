﻿using DuolingoSk.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DuolingoSk.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class MyProfileController : Controller
    {
        // GET: Admin/MyProfile
        public ActionResult Index()
        {
            return View();
        }
    }
}