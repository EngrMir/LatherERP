using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SecurityAdministration.BLL;
using SecurityAdministration.BLL.ViewModels;
using SecurityAdministration.DAL;
using SecurityAdministration.DAL.Repositories;

namespace SecurityAdministration.Controllers
{
    public class RoleWiseScreenPermissionController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        public ActionResult Index()
        {
            //if (Convert.ToBoolean(Session["IsLogged"]))
            //{
                var roleWiseScreenPermissionVM = new RoleWiseScreenPermissionVM
                {
                    RoleWiseScreenPermissions = _unitOfWork.RolewiseScreenPermissionRepository.Get(),
                    //RoleList = new SelectList(_unitOfWork.RoleRepository.Get(filter:"RoleID !=1"), "RoleID", "RoleName"),

                    RoleList = new SelectList(_unitOfWork.RoleRepository.GetRoleList(), "RoleID", "RoleName"),
                    ModuleList = new SelectList(_unitOfWork.ModuleRepository.Get(), "ModuleID", "Title"),
                };

                ViewBag.RoleWiseScreenPermissionList = roleWiseScreenPermissionVM.RoleWiseScreenPermissions.ToList();

                return View(roleWiseScreenPermissionVM);
            //}
            //return RedirectToAction("Index", "Home");
        }

        public JsonResult GetScreenList(string moduleID)
        {
            var screenList = new SelectList(_unitOfWork.ScreenRepository.Get().Where(w => w.ModuleID == moduleID).ToList(), "ScreenCode", "Title");
            return new JsonResult { Data = screenList };
        }

        public JsonResult GetScreenPermissionList(int? roleID, string moduleID, string screenCode)
        {
            IEnumerable<ScreenPermissionView> screenPermissionList = _unitOfWork.RolewiseScreenPermissionRepository.Get(roleID, moduleID, screenCode);
            return new JsonResult { Data = screenPermissionList };
        }

        public JsonResult GetRoleWiseScreenPermission(int roleID, string screenCode)
        {
            return new JsonResult
            {
                Data = new JData
                {
                    JsonData = _unitOfWork.RolewiseScreenPermissionRepository.Get(roleID, screenCode: screenCode).FirstOrDefault(),
                    Message = null
                }
            };
        }

        public JsonResult Save([Bind(Include = "RoleID,ScreenCode,AccessRight")] RoleWiseScreenPermission rolewisescreenpermission, bool isInsert)
        {
            if (ModelState.IsValid)
            {
                rolewisescreenpermission.SetBy = LoginInformation.UserID;
                rolewisescreenpermission.SetOn = DateTime.Now;

                var screenPermission = _unitOfWork.RolewiseScreenPermissionRepository.Get(rolewisescreenpermission.RoleID, screenCode: rolewisescreenpermission.ScreenCode).FirstOrDefault();

                if (isInsert)
                {
                    if (screenPermission != null)
                    {
                        return new JsonResult
                        {
                            Data = new JData
                                {
                                    JsonData = null,
                                    Message = "Information already exists. Please press edit to modify."
                                }
                        };
                    }
                    _unitOfWork.RolewiseScreenPermissionRepository.Insert(rolewisescreenpermission);
                }
                else
                {
                    _unitOfWork.RolewiseScreenPermissionRepository.Update(rolewisescreenpermission);
                }
                _unitOfWork.Save();
            }

            return GetRoleWiseScreenPermission(rolewisescreenpermission.RoleID, rolewisescreenpermission.ScreenCode);
        }

        public void Delete(int roleID, string screenCode)
        {
            _unitOfWork.RolewiseScreenPermissionRepository.DeleteRoleWiseScreenPermission(roleID, screenCode);
            _unitOfWork.Save();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }

    }    
}
