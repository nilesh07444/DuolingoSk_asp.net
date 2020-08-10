using DuolingoSk.Filters;
using DuolingoSk.Helper;
using DuolingoSk.Model;
using DuolingoSk.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DuolingoSk.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class MaterialController : Controller
    {
        private readonly DuolingoSk_Entities _db;
        public string MaterialDirectoryPath = "";

        public MaterialController()
        {
            _db = new DuolingoSk_Entities();
            MaterialDirectoryPath = ErrorMessage.MaterialDirectoryPath;
        }

        public ActionResult Index()
        {
            List<MaterialVM> lstMaterial = new List<MaterialVM>();

            try
            {
                lstMaterial = (from m in _db.tbl_Materials
                               where !m.IsDeleted
                               select new MaterialVM
                               {
                                   MaterialId = m.MaterialId,
                                   MaterialTitle = m.MaterialTitle,
                                   MaterialType = m.MaterialType,
                                   MaterialFileName = m.MaterialFile,
                                   MaterialVideoLink = m.MaterialVideoLink,
                                   MaterialFileType = m.MaterialFileType,
                                   IsActive = m.IsActive
                               }).ToList();
            }
            catch (Exception ex)
            {
            }

            return View(lstMaterial);
        }

        public ActionResult Add()
        {
            MaterialVM objMaterial = new MaterialVM();
            objMaterial.MaterialTypeList = GetMaterialTypeList();
            objMaterial.MaterialFileTypeList = GetMaterialFileTypeList();

            return View(objMaterial);
        }

        [HttpPost]
        public ActionResult Add(MaterialVM materialVM, HttpPostedFileBase MaterialFile)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    #region Validation

                    if (materialVM.MaterialFileType == null)
                    {
                         
                            ModelState.AddModelError("MaterialFileType", ErrorMessage.SelectMaterialFileType);

                            materialVM.MaterialTypeList = GetMaterialTypeList();
                            materialVM.MaterialFileTypeList = GetMaterialFileTypeList();

                            return View(materialVM);
                         
                    }

                    if (materialVM.MaterialFileType == (int)MaterialFileTypes.Document)
                    {
                        if (MaterialFile == null)
                        {
                            ModelState.AddModelError("MaterialFile", ErrorMessage.SelectMaterialFile);

                            materialVM.MaterialTypeList = GetMaterialTypeList();
                            materialVM.MaterialFileTypeList = GetMaterialFileTypeList();

                            return View(materialVM);
                        }
                    }

                    if (materialVM.MaterialFileType == (int)MaterialFileTypes.Video)
                    {
                        if (string.IsNullOrEmpty(materialVM.MaterialVideoLink))
                        {
                            ModelState.AddModelError("MaterialVideoLink", ErrorMessage.EnterMaterialVideoLink);

                            materialVM.MaterialTypeList = GetMaterialTypeList();
                            materialVM.MaterialFileTypeList = GetMaterialFileTypeList();

                            return View(materialVM);
                        }
                    }

                    string fileName = string.Empty;
                    string path = Server.MapPath(MaterialDirectoryPath);

                    bool folderExists = Directory.Exists(path);
                    if (!folderExists)
                        Directory.CreateDirectory(path);

                    if (MaterialFile != null)
                    {
                        string ext = Path.GetExtension(MaterialFile.FileName);
                        string f_name = Path.GetFileNameWithoutExtension(MaterialFile.FileName);

                        fileName = f_name + "-" + Guid.NewGuid() + ext;
                        MaterialFile.SaveAs(path + fileName);
                    }
                    else
                    {
                        fileName = materialVM.MaterialFileName;
                    }

                    #endregion Validation

                    #region CreateMaterial

                    tbl_Materials objMaterial = new tbl_Materials();

                    objMaterial.MaterialTitle = materialVM.MaterialTitle;
                    objMaterial.MaterialType = materialVM.MaterialType;
                    objMaterial.MaterialFile = fileName;
                    objMaterial.MaterialFileType = materialVM.MaterialFileType;
                    objMaterial.MaterialVideoLink = materialVM.MaterialVideoLink;

                    objMaterial.IsActive = true;
                    objMaterial.IsDeleted = false;
                    objMaterial.CreatedDate = DateTime.UtcNow;
                    objMaterial.CreatedBy = LoggedInUserId;
                    _db.tbl_Materials.Add(objMaterial);
                    _db.SaveChanges();

                    return RedirectToAction("Index");

                    #endregion CreateMaterial
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

            materialVM.MaterialTypeList = GetMaterialTypeList();
            materialVM.MaterialFileTypeList = GetMaterialFileTypeList();

            return View(materialVM);
        }

        public ActionResult Edit(long Id)
        {
            MaterialVM objMaterial = new MaterialVM();

            try
            {
                objMaterial = (from m in _db.tbl_Materials 
                               where m.MaterialId == Id
                               select new MaterialVM
                               {
                                   MaterialId = m.MaterialId,
                                   MaterialTitle = m.MaterialTitle,
                                   MaterialType = m.MaterialType,
                                   MaterialFileName = m.MaterialFile,
                                   MaterialVideoLink = m.MaterialVideoLink,
                                   MaterialFileType = m.MaterialFileType,
                                   IsActive = m.IsActive 
                               }).FirstOrDefault();

                objMaterial.MaterialTypeList = GetMaterialTypeList();
                objMaterial.MaterialFileTypeList = GetMaterialFileTypeList();

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

            return View(objMaterial);
        }

        [HttpPost]
        public ActionResult Edit(MaterialVM materialVM, HttpPostedFileBase MaterialFile)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    #region Validation

                    tbl_Materials objMaterial = _db.tbl_Materials.Where(x => x.MaterialId == materialVM.MaterialId).FirstOrDefault();

                    if (materialVM.MaterialFileType == null)
                    {

                        ModelState.AddModelError("MaterialFileType", ErrorMessage.SelectMaterialFileType);

                        materialVM.MaterialTypeList = GetMaterialTypeList();
                        materialVM.MaterialFileTypeList = GetMaterialFileTypeList();

                        return View(materialVM);

                    }

                    if (materialVM.MaterialFileType == (int)MaterialFileTypes.Document)
                    {
                        if (MaterialFile == null && string.IsNullOrEmpty(objMaterial.MaterialFile))
                        {
                            ModelState.AddModelError("MaterialFile", ErrorMessage.SelectMaterialFile);

                            materialVM.MaterialTypeList = GetMaterialTypeList();
                            materialVM.MaterialFileTypeList = GetMaterialFileTypeList();

                            return View(materialVM);
                        }
                    }

                    if (materialVM.MaterialFileType == (int)MaterialFileTypes.Video)
                    {
                        if (string.IsNullOrEmpty(materialVM.MaterialVideoLink))
                        {
                            ModelState.AddModelError("MaterialVideoLink", ErrorMessage.EnterMaterialVideoLink);

                            materialVM.MaterialTypeList = GetMaterialTypeList();
                            materialVM.MaterialFileTypeList = GetMaterialFileTypeList();

                            return View(materialVM);
                        }
                    }

                    

                    string fileName = string.Empty;
                    string path = Server.MapPath(MaterialDirectoryPath);

                    bool folderExists = Directory.Exists(path);
                    if (!folderExists)
                        Directory.CreateDirectory(path);

                    if (MaterialFile != null)
                    {
                        string ext = Path.GetExtension(MaterialFile.FileName);
                        string f_name = Path.GetFileNameWithoutExtension(MaterialFile.FileName);

                        fileName = f_name + "-" + Guid.NewGuid() + ext;
                        MaterialFile.SaveAs(path + fileName);
                    }
                    else
                    {
                        fileName = objMaterial.MaterialFile;
                    }

                    #endregion Validation

                    #region UpdateUser

                    objMaterial.MaterialTitle = materialVM.MaterialTitle;
                    objMaterial.MaterialType = materialVM.MaterialType;
                    objMaterial.MaterialFile = fileName;
                    objMaterial.MaterialFileType = materialVM.MaterialFileType;
                    objMaterial.MaterialVideoLink = materialVM.MaterialVideoLink;

                    objMaterial.UpdatedDate = DateTime.UtcNow;
                    objMaterial.UpdatedBy = LoggedInUserId;

                    _db.SaveChanges();

                    return RedirectToAction("Index");

                    #endregion UpdateUser
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

            materialVM.MaterialTypeList = GetMaterialTypeList();
            materialVM.MaterialFileTypeList = GetMaterialFileTypeList();

            return View(materialVM);
        }

        public ActionResult View(int Id)
        {
            MaterialVM objMaterial = new MaterialVM();

            try
            {

                objMaterial = (from m in _db.tbl_Materials

                               join uC in _db.tbl_AdminUsers on m.CreatedBy equals uC.AdminUserId into outerCreated
                               from uC in outerCreated.DefaultIfEmpty()

                               join uM in _db.tbl_AdminUsers on m.UpdatedBy equals uM.AdminUserId into outerModified
                               from uM in outerModified.DefaultIfEmpty()

                               where m.MaterialId == Id
                               select new MaterialVM
                               {
                                   MaterialId = m.MaterialId,
                                   MaterialTitle = m.MaterialTitle,
                                   MaterialType = m.MaterialType,
                                   MaterialFileName = m.MaterialFile,
                                   MaterialVideoLink = m.MaterialVideoLink,
                                   MaterialFileType = m.MaterialFileType,
                                   IsActive = m.IsActive,

                                   CreatedDate = m.CreatedDate,
                                   UpdatedDate = m.UpdatedDate,
                                   strCreatedBy = (uC != null ? uC.FirstName + " " + uC.LastName : ""),
                                   strModifiedBy = (uM != null ? uM.FirstName + " " + uM.LastName : "")

                               }).FirstOrDefault();
                  
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(objMaterial);
        }

        [HttpPost]
        public string DeleteMaterial(int MaterialId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_Materials objMaterial = _db.tbl_Materials.Where(x => x.MaterialId == MaterialId).FirstOrDefault();

                if (objMaterial == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    _db.tbl_Materials.Remove(objMaterial);
                    _db.SaveChanges();

                    ReturnMessage = "success";
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }

        [HttpPost]
        public string ChangeStatus(long Id, string Status)
        {
            string ReturnMessage = "";
            try
            {
                tbl_Materials objMaterial = _db.tbl_Materials.Where(x => x.MaterialId == Id).FirstOrDefault();

                if (objMaterial != null)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    if (Status == "Active")
                    {
                        objMaterial.IsActive = true;
                    }
                    else
                    {
                        objMaterial.IsActive = false;
                    }

                    objMaterial.UpdatedBy = LoggedInUserId;
                    objMaterial.UpdatedDate = DateTime.UtcNow;

                    _db.SaveChanges();
                    ReturnMessage = "success";
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }

        private List<SelectListItem> GetMaterialTypeList()
        {
            List<SelectListItem> lst = new List<SelectListItem>();

            lst.Add(new SelectListItem { Value = "1", Text = "Material" });
            lst.Add(new SelectListItem { Value = "2", Text = "Tips" });

            return lst;
        }

        private List<SelectListItem> GetMaterialFileTypeList()
        {
            List<SelectListItem> lst = new List<SelectListItem>();

            lst.Add(new SelectListItem { Value = "1", Text = "Document" });
            lst.Add(new SelectListItem { Value = "2", Text = "Video" });

            return lst;
        }

    }
}