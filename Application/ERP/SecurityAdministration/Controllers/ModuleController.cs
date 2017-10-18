using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using SecurityAdministration.BLL;
using SecurityAdministration.BLL.ViewModels;
using SecurityAdministration.DAL;
using SecurityAdministration.DAL.Repositories;

namespace SecurityAdministration.Controllers
{
    public class ModuleController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        public ActionResult Index()
        {
            if (!Convert.ToBoolean(Session["IsLogged"])) return RedirectToAction("Index", "Home");
            var moduleViewModel = new ModuleVM
            {
                Modules = _unitOfWork.ModuleRepository.GetAll().Where(s => s.IsDelete == false).OrderByDescending(s => s.SetOn),
                ApplicationList = new SelectList(_unitOfWork.ApplicationRepository.Get().Where(a=>a.IsActive.Equals(true) && a.IsDelete.Equals(false)), "ApplicationID", "Title")
            };
            ViewBag.ModuleList = moduleViewModel.Modules.ToList();
            return View(moduleViewModel);
        }

        [HttpPost]
        public JsonResult Save([Bind(Include = "ModuleID,ApplicationID,Title,IsActive,Description,SetOn,SetBy")] Module module, bool isInsert)
        {
            if (ModelState.IsValid)
            {
                module.SetBy = LoginInformation.UserID;
                module.SetOn = DateTime.Now;
                module.IsDelete = false;
               
                if (isInsert)
                {
                    var moduleID = _unitOfWork.ModuleRepository.GetLastModuleID();
                    module.ModuleID = ModuleIDGenerator(moduleID);
                    _unitOfWork.ModuleRepository.Insert(module);
                }
                else
                {
                    _unitOfWork.ModuleRepository.Update(module);
                }
                _unitOfWork.Save();
            }

            ViewBag.ApplicationID = new SelectList(_unitOfWork.ApplicationRepository.Get(), "ApplicationID", "Title", module.ApplicationID);
            return new JsonResult { Data = _unitOfWork.ModuleRepository.GetByValue(module.ModuleID).FirstOrDefault() };
        }

        public void Delete(string id)
        {
            _unitOfWork.ModuleRepository.IsDeleteTrue(id);
            _unitOfWork.Save();
        }

        public JsonResult GetModule(string id)
        {
            return new JsonResult { Data = _unitOfWork.ModuleRepository.GetByValue(id).FirstOrDefault() };
        }

        public JsonResult GetApplication(byte applicationId)
        {
            return new JsonResult {Data = _unitOfWork.ApplicationRepository.GetByValue(applicationId)};
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _unitOfWork.Dispose();
            base.Dispose(disposing);
        }

        private static string ModuleIDGenerator(string moduleID)
        {
            int i;
            if (moduleID == null)
                return "01";
            else
            {
                i = Convert.ToInt32(moduleID);
                i++;
                if (i < 10)
                {
                    return "0" + i.ToString(CultureInfo.InvariantCulture);
                }
            }
            return i.ToString(CultureInfo.InvariantCulture);
        }
    }
}
