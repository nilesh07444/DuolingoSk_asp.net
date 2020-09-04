using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DuolingoSk
{
    public class WebinarVM
    {
        public int WebinarId { get; set; }
        public int PackageId { get; set; }
        public string WebinarMessage { get; set; }
        public int? TotalAttendedStudent { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }

        // extra fields
        public string PackageName { get; set; }
        public List<SelectListItem> PackageNameList { get; set; }

    }

    public class FeeWiseWebinarVM
    {
        public int StudentFeeId { get; set; }
        public int? TotalWebinar { get; set; }
        public int UsedTotalWebinar { get; set; }
        public DateTime FeeDate { get; set; }
        public int? PackageId { get; set; }
        public string PackageName { get; set; }
        public List<UsedWebinarVM> lstUsedWebinar { get; set; }
    }

    public class UsedWebinarVM
    {
        public int WebinarId { get; set; }
        public long StudentWebinarId { get; set; }
        public string WebinarMessage { get; set; }
        public string PackageName { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class FeeWisePackageVM
    {
        public int StudentFeeId { get; set; }
        public int? TotalExamAttempt { get; set; }
        public int UsedTotalExams { get; set; }
        public DateTime FeeDate { get; set; }
        public int? PackageId { get; set; }
        public string PackageName { get; set; }
        public List<ExamVM> lstUsedPackageExam { get; set; }
    }

}