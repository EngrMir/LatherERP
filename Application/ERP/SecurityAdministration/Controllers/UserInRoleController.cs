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
    public class UserInRoleController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        public ActionResult Index()
        {
            if (!Convert.ToBoolean(Session["IsLogged"])) return RedirectToAction("Index", "Home");
            var userInRoleVM = new UserInRoleVM
            {
                UserInRoles = _unitOfWork.UserInRoleRepository.GetAll(),
                RoleList = new SelectList(_unitOfWork.RoleRepository.GetRoleList(), "RoleID", "RoleName"),
            };
            ViewBag.UserInRoleList = userInRoleVM.UserInRoles.ToList();
            ViewBag.UserListFor= _unitOfWork.UserRepository.GetUserList();

            return View(userInRoleVM);
        }

        public JsonResult Save([Bind(Include = "UserID,RoleID,IsActive,SetOn,SetBy")] UserInRole userInRole, bool isInsert)
        {
            if (ModelState.IsValid)
            {
                userInRole.SetBy = LoginInformation.UserID;
                userInRole.SetOn = DateTime.Now;
                IEnumerable<UserInRoleView> userInRoles = _unitOfWork.UserInRoleRepository.Get(userInRole.UserID, userInRole.RoleID);
                var userInRoleViews = userInRoles as UserInRoleView[] ?? userInRoles.ToArray();
                if (!userInRoleViews.Any())
                {
                    if (isInsert)
                    {
                        _unitOfWork.UserInRoleRepository.InsertUserInRole(userInRole);
                    }

                    return new JsonResult
                    {
                        Data =
                            _unitOfWork.UserInRoleRepository.Get(userInRole.UserID, userInRole.RoleID)
                                .FirstOrDefault()
                    };
                }
                if (userInRoleViews.Any() && !isInsert)
                {
                    _unitOfWork.UserInRoleRepository.UpdateUserInRole(userInRole);
                }
                _unitOfWork.Save();
            }
            return new JsonResult
            {
                Data = _unitOfWork.UserInRoleRepository.Get(userInRole.UserID, userInRole.RoleID)
                    .FirstOrDefault()
            };
        }

        public JsonResult GetRole(int userID, int roleID)
        {
            return new JsonResult { Data = _unitOfWork.UserInRoleRepository.Get(userID, Convert.ToByte(roleID)).FirstOrDefault() };
        }

        public JsonResult GetUserInRoleList(int userID, int? roleID)
        {
            IEnumerable<UserInRoleView> userInRoles = _unitOfWork.UserInRoleRepository.Get(userID, roleID);
            return new JsonResult { Data = userInRoles };
        }


        public JsonResult CheckUserInRoleEntry(int roleID)
        {
            bool isExistOrNot = false;
            var objUserView = _unitOfWork.UserInRoleRepository.CheckUser(roleID);
            if (objUserView == null)
            {
                isExistOrNot = true;
            }
            return new JsonResult { Data = isExistOrNot };
        }

        public void Delete(int userId, int roleId)
        {
            _unitOfWork.UserInRoleRepository.DeleteUserInRole(userId, roleId);
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
