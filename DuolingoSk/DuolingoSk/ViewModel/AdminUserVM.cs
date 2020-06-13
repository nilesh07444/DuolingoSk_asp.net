﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DuolingoSk
{
    public class AdminUserVM
    {
        public long AdminUserId { get; set; } 
        public int AdminRoleId { get; set; }
        [Required]
        [MaxLength(100), Display(Name = "First Name *")]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(100), Display(Name = "Last Name *")]
        public string LastName { get; set; }
        [MaxLength(100), Display(Name = "Email Id")]
        public string Email { get; set; }
        [Required]
        [MinLength(5), MaxLength(20), Display(Name = "Password *")]
        public string Password { get; set; }
        [Required]
        [MaxLength(10), MinLength(10), Display(Name = "Mobile No *")]
        public string MobileNo { get; set; }
        
        [MaxLength(200), Display(Name = "Address")]
        public string Address { get; set; }
        [MaxLength(100), Display(Name = "City")]
        public string City { get; set; }

        //[Display(Name = "Date Of Birth")]
        //public string Dob { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Registration Fee must be greater than 0")]
        [Display(Name = "Registration Fee *")]
        public decimal? StudentRegistrationFee { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Renew Fee must be greater than 0")]
        [Display(Name = "Renew Fee *")]
        public decimal? StudentRenewFee { get; set; }

        public string ProfilePicture { get; set; }
         
        [MaxLength(200), Display(Name = "Remarks")]
        public string Remarks { get; set; }
        public bool IsActive { get; set; }

        [Display(Name = "Profile Picture")]
        public HttpPostedFileBase ProfilePictureFile { get; set; }
         
        // Addional fields 
        public DateTime? dtDob { get; set; }
        public string RoleName { get; set; }
         
        public string strCreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string strModifiedBy { get; set; }
        public DateTime? UpdatedDate { get; set; } 
        public string FullName { get; set; }
    }
}