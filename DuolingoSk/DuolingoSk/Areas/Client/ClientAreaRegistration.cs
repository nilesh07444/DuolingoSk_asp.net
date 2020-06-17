﻿using System.Web.Mvc;

namespace DuolingoSk.Areas.Client
{
    public class ClientAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Client";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Client_default",
                "Client/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Client_HomePage",
                "home",
                new { controller = "HomePage", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Client_AboutUs",
                "aboutus",
                new { controller = "AboutUs", action = "Index", id = UrlParameter.Optional }
            );


            context.MapRoute(
                "Client_StartExam",
                "startexam",
                new { controller = "StartExam", action = "Index", id = UrlParameter.Optional }
            );


            context.MapRoute(
                "Client_ContactUs",
                "contactus",
                new { controller = "ContactUs", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Client_Login",
                "login",
                new { controller = "StudentLogin", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Client_Register",
                "register",
                new { controller = "StudentRegister", action = "Index", id = UrlParameter.Optional }
            );
            context.MapRoute(
               "Client_GetQuestion",
               "getQuestion",
               new { controller = "StartExam", action = "GetQuestionById", id = UrlParameter.Optional }
           );
            context.MapRoute(
              "Client_GetStarnow",
              "startnow",
              new { controller = "StartExam", action = "StartNow", id = UrlParameter.Optional }
          );
            context.MapRoute(
             "Client_Saveexamresult",
             "SaveExamResult",
             new { controller = "StartExam", action = "SaveExamResult", id = UrlParameter.Optional }
         );
            
        }
    }
}