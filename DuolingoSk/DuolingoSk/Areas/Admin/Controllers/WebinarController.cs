using DuolingoSk.Filters;
using DuolingoSk.Model;
using DuolingoSk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DuolingoSk.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class WebinarController : Controller
    {
        private readonly DuolingoSk_Entities _db;
        public WebinarController()
        {
            _db = new DuolingoSk_Entities();
        }

        public ActionResult Index()
        {
            List<WebinarVM> lstWebinars = new List<WebinarVM>();

            try
            {
                lstWebinars = (from w in _db.tbl_Webinar
                               join p in _db.tbl_Package on w.PackageId equals p.PackageId
                               select new WebinarVM
                               {
                                   WebinarId = w.WebinarId,
                                   WebinarMessage = w.WebinarMessage,
                                   PackageId = w.PackageId,
                                   CreatedDate = w.CreatedDate,
                                   PackageName = p.PackageName,
                                   TotalAttendedStudent = w.TotalAttendedStudent
                               }).ToList();
            }
            catch (Exception ex)
            {

            }

            return View(lstWebinars);
        }

        public ActionResult Add()
        {
            WebinarVM objWebinar = new WebinarVM();

            List<tbl_Package> lstPackages = _db.tbl_Package.Where(x => x.IsActive).OrderBy(x => x.PackageName).ToList();
            ViewData["lstPackages"] = lstPackages;

            return View();
        }

        [HttpPost]
        public string SaveWebinar(FormCollection frm)
        {
            string ReturnMessage = "";
            try
            {
                long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                int PackageId = Convert.ToInt32(frm["PackageId"]);
                string WebinarMessage = frm["WebinarMessage"];

                DateTime today = DateTime.UtcNow;

                List<StudentWebinarVM> lstStudents = (from f in _db.tbl_StudentFee
                                                      join s in _db.tbl_Students on f.StudentId equals s.StudentId
                                                      where f.PackageId == PackageId && f.TotalWebinarAttempt > 0
                                                           && f.FeeExpiryDate >= today
                                                      select new StudentWebinarVM
                                                      {
                                                          StudentId = s.StudentId,
                                                          StudentFeeId = f.StudentFeeId,
                                                          StudentName = s.FirstName + " " + s.LastName,
                                                          MobileNo = s.MobileNo,
                                                          TotalWebinarUsed = _db.tbl_StudentWebinar.Where(x => x.StudentId == s.StudentId && x.StudentFeeId == f.StudentFeeId
                                                                               && x.PackageId == f.PackageId).Count()
                                                      }).ToList();

                int SMSSentCount = 0;
                if (lstStudents != null && lstStudents.Count > 0)
                {
                    #region SaveWebinar

                    tbl_Webinar objWebinar = new tbl_Webinar();
                    objWebinar.PackageId = PackageId;
                    objWebinar.WebinarMessage = WebinarMessage;
                    objWebinar.CreatedDate = DateTime.UtcNow;
                    objWebinar.CreatedBy = LoggedInUserId;
                    _db.tbl_Webinar.Add(objWebinar);
                    _db.SaveChanges();

                    #endregion

                    lstStudents.ForEach(student =>
                    {

                        bool IsSentSMS = SendWebinarSMS(student.MobileNo, WebinarMessage);

                        if (IsSentSMS)
                        {
                            tbl_StudentWebinar studentWebinar = new tbl_StudentWebinar();
                            studentWebinar.StudentId = student.StudentId;
                            studentWebinar.WebinarId = objWebinar.WebinarId;
                            studentWebinar.StudentFeeId = student.StudentFeeId;
                            studentWebinar.PackageId = PackageId;
                            studentWebinar.CreatedDate = DateTime.UtcNow;
                            _db.tbl_StudentWebinar.Add(studentWebinar);
                            _db.SaveChanges();

                            SMSSentCount++;

                        }

                    });

                    // update with sent count
                    tbl_Webinar updateWebinar = _db.tbl_Webinar.Where(x => x.WebinarId == objWebinar.WebinarId).FirstOrDefault();
                    if (updateWebinar != null)
                    {
                        updateWebinar.TotalAttendedStudent = SMSSentCount;
                        _db.SaveChanges();
                    }

                    ReturnMessage = "SUCCESS";

                }
                else
                {
                    ReturnMessage = "NOSTUDENTFOUND";
                }



            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                ReturnMessage = "ERROR";
            }

            return ReturnMessage;
        }


        public bool SendWebinarSMS(string MobileNo, string Message)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    WebClient client = new WebClient();

                    string url = "http://sms.unitechcenter.com/sendSMS?username=skacademy&message=" + Message + "&sendername=SKANAD&smstype=TRANS&numbers=" + MobileNo + "&apikey=0b9c3015-bbcd-4ad8-b9ac-30f28451ebe6";

                    var json = webClient.DownloadString(url);
                    if (json.Contains("invalidnumber"))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }

                }
            }
            catch (WebException ex)
            {
                return false;
            }
        }


    }
}