using System;
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
        [MaxLength(20), Display(Name = "Agent Code *")]
        public string AgentCode { get; set; }
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
         
        public string ProfilePicture { get; set; }
         
        [MaxLength(200), Display(Name = "Remarks")]
        public string Remarks { get; set; }
        public bool IsActive { get; set; }

        [Display(Name = "Profile Picture")]
        public HttpPostedFileBase ProfilePictureFile { get; set; }
          
        // Addional fields  
        public string RoleName { get; set; }
         
        public string strCreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string strModifiedBy { get; set; }
        public DateTime? UpdatedDate { get; set; } 
        public string FullName { get; set; }
    }
}