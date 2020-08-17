using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DuolingoSk
{
    public class StudentVM
    {
        public int StudentId { get; set; } 
        [Required]
        [MaxLength(100), Display(Name = "Full Name *")]
        public string FullName { get; set; }
        //[Required]
        //[MaxLength(100), Display(Name = "Last Name *")]
        //public string LastName { get; set; }
        [Required]
        [MaxLength(100), Display(Name = "Email Id")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [MaxLength(10), MinLength(10), Display(Name = "Mobile No *")]        
        public string MobileNo { get; set; }
        [Required]
        [MinLength(5), MaxLength(20), Display(Name = "Password *")]
        [DataType(DataType.Password)]
        public string Password { get; set; } 

        [MaxLength(200), Display(Name = "Address")]
        public string Address { get; set; }
        [MaxLength(100), Display(Name = "City")]
        public string City { get; set; }
          
        public string ProfilePicture { get; set; }

        [MaxLength(200), Display(Name = "Remarks")]
        public string Remarks { get; set; }
        public bool IsActive { get; set; }

        [Display(Name = "Student Photo")]
        public HttpPostedFileBase ProfilePictureFile { get; set; }

        // Addional fields 
        public long? AdminUserId { get; set; } 
        public string strCreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string strModifiedBy { get; set; }
        public DateTime? UpdatedDate { get; set; } 
        public string AgentName { get; set; }
        public decimal MaxScore { get; set; } 

    }
}