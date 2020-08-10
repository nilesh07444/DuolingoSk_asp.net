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
}