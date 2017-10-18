using System;
using System.Linq;
using System.Web.Mvc;
using SecurityAdministration.BLL;
using SecurityAdministration.BLL.ViewModels;
using SecurityAdministration.DAL;
using SecurityAdministration.DAL.Repositories;

namespace SecurityAdministration.Controllers
{
    public class RoleController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        public ActionResult Index()
        {
            if (!Convert.ToBoolean(Session["IsLogged"])) return RedirectToAction("Index", "Home");
            var userType = System.Web.HttpContext.Current.Session["UserType"];
            var roleVM = new RoleVM
            {
                Roles = (Convert.ToInt16(userType) != 1) ? _unitOfWork.RoleRepository.Get().Where(s => s.IsDelete == false & s.RoleID!=1).OrderByDescending(s => s.RoleID).ToRoleView() : _unitOfWork.RoleRepository.Get().Where(s => s.IsDelete == false).OrderByDescending(s => s.RoleID).ToRoleView()
            };
                ViewBag.RoleList = roleVM.Roles.ToList();
            return View(roleVM);
        }

        public JsonResult Save([Bind(Include = "RoleID,RoleName,Description,IsActive")] RoleView roleView, bool isInsert)
        {
            roleView.IsDelete = false;

            var role = new Role
            {
                RoleID = Convert.ToInt16(roleView.RoleID),
                RoleName = roleView.RoleName,
                Description = roleView.Description,
                IsActive = roleView.IsActive,
                IsDelete = roleView.IsDelete,
                SetOn = Convert.ToDateTime(DateTime.Now),
                SetBy = LoginInformation.UserID
            };

            if (!ModelState.IsValid)
                return new JsonResult { Data = _unitOfWork.RoleRepository.GetByID(roleView.RoleID).ToRoleView() };

            if (isInsert)
            {
                _unitOfWork.RoleRepository.Insert(role);
            }
            else
            {
                _unitOfWork.RoleRepository.Update(role);
            }

            _unitOfWork.Save();
            return new JsonResult { Data = _unitOfWork.RoleRepository.GetByID(role.RoleID).ToRoleView() };
        }

        public JsonResult GetRole(byte? id)
        {
            return new JsonResult { Data = _unitOfWork.RoleRepository.GetByID(id).ToRoleView() };
        }

        public void Delete(byte? id)
        {
            _unitOfWork.RoleRepository.IsDeleteTrue(id);
            _unitOfWork.Save();

        }

        [HttpGet]
        public ActionResult GetGameListing()
        {
            return PartialView("_ConfirmationView");
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
