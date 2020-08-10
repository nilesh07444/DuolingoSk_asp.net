using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DuolingoSk
{
    public class MaterialVM
    {
        public int MaterialId { get; set; }
        [Required]
        [MaxLength(100), Display(Name = "Material Title *")]
        public string MaterialTitle { get; set; }
        [Required]
        [Display(Name = "Material Type *")]
        public int MaterialType { get; set; }

        [Display(Name = "Material File Type *")]
        public int? MaterialFileType { get; set; }

        [Display(Name = "Material File *")]
        public HttpPostedFileBase MaterialFile { get; set; }

        [MaxLength(100), Display(Name = "Material Video Link *")]
        public string MaterialVideoLink { get; set; }
        
        public bool IsActive { get; set; } 
        public string MaterialFileName { get; set; }
        public List<SelectListItem> MaterialTypeList { get; set; }
        public List<SelectListItem> MaterialFileTypeList { get; set; }

        public string strCreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string strModifiedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
}