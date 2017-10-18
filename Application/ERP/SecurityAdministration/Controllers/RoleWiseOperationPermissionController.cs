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
    public class RoleWiseOperationPermissionController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        public ActionResult Index()
        {
            if (Convert.ToBoolean(Session["IsLogged"]))
            {
                var roleWiseOperationPermissionVm = new RoleWiseOperationPermissionVm
                {
                    RoleWiseScreenPermissions = _unitOfWork.RolewiseOperationPermissionRepository.GetAll(),
                    RoleList = new SelectList(_unitOfWork.RoleRepository.Get(), "RoleID", "RoleName"),
                    OperationList = new SelectList(_unitOfWork.ScreenOperationRepository.Get(), "OperationID", "OperationTitle"),
                };
                ViewBag.RoleWiseOperationPermissionList = roleWiseOperationPermissionVm.RoleWiseScreenPermissions.ToList();
                return View(roleWiseOperationPermissionVm);
            }
            return RedirectToAction("Index", "Home");
        }

        public JsonResult Save([Bind(Include = "RoleID,OperationID,HaveAccess,SetOn,SetBy")] RoleWiseOperationPermission rolewiseoperationpermission, bool isInsert)
        {
            if (ModelState.IsValid)
            {
                rolewiseoperationpermission.SetBy = LoginInformation.UserID;
                rolewiseoperationpermission.SetOn = DateTime.Now;

                if (isInsert)
                {
                    _unitOfWork.RolewiseOperationPermissionRepository.Insert(rolewiseoperationpermission);
                }
                else
                {
                    _unitOfWork.RolewiseOperationPermissionRepository.UpdateRoleWiseOperationPermission(rolewiseoperationpermission);
                }
                _unitOfWork.Save();
            }

            ViewBag.RoleID = new SelectList(_unitOfWork.RoleRepository.Get(), "RoleID", "RoleName", rolewiseoperationpermission.RoleID);
            ViewBag.OperationID = new SelectList(_unitOfWork.ScreenOperationRepository.Get(), "OperationID", "OperationTitle", rolewiseoperationpermission.OperationID);

            return new JsonResult { Data = _unitOfWork.RolewiseOperationPermissionRepository.GetByValue(rolewiseoperationpermission.RoleID, rolewiseoperationpermission.OperationID).FirstOrDefault() };
        }

        public JsonResult GetRoleWiseScreenOperationData(byte? roleID, byte? operationID)
        {
            return new JsonResult { Data = _unitOfWork.RolewiseOperationPermissionRepository.GetByValue(roleID, operationID).FirstOrDefault() };
        }


        public JsonResult CheckEntry(byte? roleID, byte? operationID)
        {
            bool isExistOrNot = false;
            var objUserView = _unitOfWork.RolewiseOperationPermissionRepository.GetByValue(roleID, operationID).FirstOrDefault();
            if (objUserView == null)
            {
                isExistOrNot = true;
            }
            return new JsonResult { Data = isExistOrNot };
        }

        public void Delete(byte? roleID, byte? operationID)
        {
            _unitOfWork.RolewiseOperationPermissionRepository.DeleteRoleOperationPermission(roleID, operationID);
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
