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

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "This value must be greater than 0")]
        [Display(Name = "Package Price")]
        public decimal? PackageAmount { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "This value must be greater than 0")]
        [Display(Name = "Total Attempt *")]
        public int? TotalAttempt { get; set; }

        [Required]
        [Display(Name = "Total Webinar *")]
        public int? TotalWebinar { get; set; }

        [Required]
        [Display(Name = "Total Expiry In Days *")]
        [Range(1, int.MaxValue, ErrorMessage = "This value must be greater than 0")]
        public int? ExpiryInDays { get; set; }

        [Required]
        [Display(Name = "Max Level *")]
        [Range(1, int.MaxValue, ErrorMessage = "This value must be greater than 0")]
        public int? MaxLevel { get; set; }

        public bool IsActive { get; set; }

        //
        public string PackageImageName { get; set; }
    }

    public class PackageInquiryVM
    {
        public int PackageBuyDetailId { get; set; }
        public string FullName { get; set; }
        public string EmailId { get; set; }
        public string MobileNo { get; set; }
        public string Message { get; set; }
        public int PackageId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string PackageName { get; set; }

    }

}