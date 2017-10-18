using System;
using System.Linq;
using System.Web.Mvc;
using SecurityAdministration.BLL;
using SecurityAdministration.BLL.ViewModels;
using SecurityAdministration.DAL;
using SecurityAdministration.DAL.Repositories;

namespace SecurityAdministration.Controllers
{
    public class MenuItemController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        public ActionResult Index()
        {
            if (!Convert.ToBoolean(Session["IsLogged"])) return RedirectToAction("Index", "Home");
            var menuItemViewModel = new MenuItemVM
            {
                MenuItems = _unitOfWork.MenuItemRepository.GetAll().Where(s => s.IsDelete == false).OrderByDescending(s => s.MenuID),
                ModuleList = new SelectList(_unitOfWork.ModuleRepository.Get().Where(m=>m.IsDelete.Equals(false) && m.IsActive.Equals(true)), "ModuleID", "Title"),
            };
            ViewBag.MenuItemList = menuItemViewModel.MenuItems.ToList();
            return View(menuItemViewModel);
        }

        public JsonResult Save([Bind(Include = "MenuID,Caption,MenuLevel,ItemOrder,Module,ScreenCode,ParentID,HasChild,Description,IsActive,SetOn,SetBy, IsDelete")] MenuItemView menuItemView, bool isInsert)
        {
            MenuItem menuItem = menuItemView.ToMenuItem();

            menuItem.MenuID = Convert.ToInt32(menuItem.MenuID);
            menuItem.SetBy = LoginInformation.UserID;
            menuItem.SetOn = DateTime.Now;

            if (!ModelState.IsValid) return GetMenuItem(menuItem.MenuID);
            if (isInsert)
            {
                 var menuMaxId = _unitOfWork.MenuItemRepository.GetAll().Max(m => m.MenuID);
                menuItem.MenuID = menuMaxId!=null? Convert.ToInt32(menuMaxId) + 1:1;
                _unitOfWork.MenuItemRepository.Insert(menuItem);
            }
            else
            {
                _unitOfWork.MenuItemRepository.Update(menuItem);
            }
            _unitOfWork.Save();
            return GetMenuItem(menuItem.MenuID);
        }

        public JsonResult GetMenuItem(int? id)
        {
            var data = _unitOfWork.MenuItemRepository.GetByValue(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetScreenList(string moduleid)
        {
            var screenList = new SelectList(_unitOfWork.ScreenRepository.Get().Where(w => w.ModuleID == moduleid).ToList(), "ScreenCode", "Title");
            return new JsonResult { Data = screenList };
        }

        public void Delete(int? id)
        {
            _unitOfWork.MenuItemRepository.IsDeleteTrue(id);
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
