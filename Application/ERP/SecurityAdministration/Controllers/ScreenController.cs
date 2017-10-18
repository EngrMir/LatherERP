using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SecurityAdministration.BLL;
using SecurityAdministration.BLL.ViewModels;
using SecurityAdministration.DAL;
using SecurityAdministration.DAL.Repositories;

namespace SecurityAdministration.Controllers
{
    public class ScreenController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        public ActionResult Index()
        {
            if (Convert.ToBoolean(Session["IsLogged"]))
            {
                var screenViewModel = new ScreenVM
                {
                    Screens = _unitOfWork.ScreenRepository.GetAll().OrderByDescending(s=>s.ScreenCode),
                    ModuleList = new SelectList(_unitOfWork.ModuleRepository.Get(), "ModuleID", "Title")
                };
                ViewBag.ScreenList = screenViewModel.Screens.ToList();
                return View(screenViewModel);
            }
            return RedirectToAction("Index", "Home");
        }

        public JsonResult Save([Bind(Include = "ScreenCode,ModuleID,Title,Type,URL,IsActive,Description")] Screen screen, bool isInsert)
        {
            if (ModelState.IsValid)
            {
                screen.SetBy = LoginInformation.UserID;
                screen.SetOn = DateTime.Now;

                if (isInsert)
                {
                    screen.ScreenCode = ScreenCodeGenerator(_unitOfWork.ScreenRepository.LastScreenCode());
                    _unitOfWork.ScreenRepository.Insert(screen);
                }
                else
                {
                    _unitOfWork.ScreenRepository.Update(screen);
                }

                _unitOfWork.Save();
            }

            return new JsonResult { Data = _unitOfWork.ScreenRepository.GetByValue(screen.ScreenCode) };
        }

        public JsonResult GetScreen(string id)
        {
            return new JsonResult { Data = _unitOfWork.ScreenRepository.GetByValue(id) };
        }

        public JsonResult ScreenCheck(string id)
        {
            bool isExistOrNot = false;
            var objScreen = _unitOfWork.ScreenRepository.GetByValue(id);
            if (objScreen == null)
            {
                isExistOrNot = true;
            }
            return new JsonResult { Data = isExistOrNot };
        }

        public void Delete(string id)
        {
            _unitOfWork.ScreenRepository.IsDeleteTrue(id);
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

        private string ScreenCodeGenerator(string screenCode)
        {
            int i;
            i = Convert.ToInt32(screenCode);
            if (screenCode == null)
                return "00001";
            if (i < 99999)
            {
                if (i < 9)
                {
                    i++;
                    return "0000" + i.ToString(CultureInfo.InvariantCulture);
                }
                if (i < 99)
                {
                    i++;
                    return "000" + i.ToString(CultureInfo.InvariantCulture);
                }
                if (i < 999)
                {
                    i++;
                    return "00" + i.ToString(CultureInfo.InvariantCulture);
                }
                if (i < 9999)
                {
                    i++;
                    return "0" + i.ToString(CultureInfo.InvariantCulture);
                }
                i++;
                return i.ToString(CultureInfo.InvariantCulture);

            }
            else
            {
                return "error";
            }
        }
    }
}
