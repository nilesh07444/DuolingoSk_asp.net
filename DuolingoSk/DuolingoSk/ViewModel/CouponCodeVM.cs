using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DuolingoSk
{
    public class CouponCodeVM
    {
        public long CouponCodeId { get; set; }

        [Required] 
        [Display(Name = "Coupon Code *")]
        public string CouponCode { get; set; }

        [Required] 
        [Display(Name = "Discount Percentage *")]
        public decimal? DiscountPercentage { get; set; }

        [Required]
        [Display(Name = "Expiry Date *")]
        public string ExpiryDate { get; set; }

        [Required]
        [Display(Name = "Total Max Usage *")]
        public int? TotalMaxUsage { get; set; } 

        public bool IsActive { get; set; }

        //
        public DateTime? dtExpiryDate { get; set; }

        public int TotalUsedByStudent { get; set; }

    }
}
