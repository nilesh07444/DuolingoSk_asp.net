using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DuolingoSk
{
    public class AgentPackageVM
    {
        public string PackageName { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "This value must be greater than 0")]
        [Display(Name = "Package Price for Agent")]
        public decimal? PackageAmountAgent { get; set; }
        public int? TotalAttempt { get; set; }
        public long PackageAgentId { get; set; }
        public string AgentName { get; set; }

        [Required]
        [Display(Name = "Package Name *")]
        public long PackageId { get; set; }
        [Required]
        [Display(Name = "Agent Name *")]
        public long AgentId { get; set; }

        public decimal PackagePrice { get; set; }               
        public List<SelectListItem> AgentList { get; set; }
        public List<SelectListItem> PackageList { get; set; }
    }
}