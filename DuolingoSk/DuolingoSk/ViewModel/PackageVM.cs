using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DuolingoSk
{
    public class PackageVM
    {
        public int PackageId { get; set; }
        [Required]
        [MaxLength(100), Display(Name = "Package Name *")]
        public string PackageName { get; set; }
        [Display(Name = "Package Image")]
        public HttpPostedFileBase PackageImage { get; set; }
        [Display(Name = "Package Amount")]
        public decimal? PackageAmount { get; set; }
        public bool IsActive { get; set; }

        //
        public string PackageImageName { get; set; }
    }
}