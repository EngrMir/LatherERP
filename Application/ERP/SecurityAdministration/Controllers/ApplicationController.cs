using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SecurityAdministration.BLL;
using SecurityAdministration.BLL.ViewModels;
using SecurityAdministration.DAL;
using SecurityAdministration.DAL.Repositories;

namespace SecurityAdministration.Controllers
{
    public class ApplicationController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        public ActionResult Index()
        {
            if (!Convert.ToBoolean(Session["IsLogged"])) return RedirectToAction("Index", "Home");
            var applicationViewModel = new ApplicationVM
            {
                Applications = _unitOfWork.ApplicationRepository.GetAll().OrderByDescending(s=>s.ApplicationID)
            };
            ViewBag.ApplicationList = applicationViewModel.Applications.ToList();
            return View(applicationViewModel);
        }

        public JsonResult Save([Bind(Include = "ApplicationID,Title,IsActive,Description,SetOn,SetBy")] Application application, bool isInsert, byte? applicationID)
        {
            try
            {
                //applicationID = applicationID ?? 0;
                //application.ApplicationID = (byte) applicationID;
                application.SetBy = LoginInformation.UserID;
                application.SetOn = DateTime.Now;

                if (isInsert)
                {
                    _unitOfWork.ApplicationRepository.Insert(application);
                }
                else
                {
                    _unitOfWork.ApplicationRepository.Update(application);
                }
                _unitOfWork.Save();
                return new JsonResult { Data = _unitOfWork.ApplicationRepository.GetByValue(application.ApplicationID) };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult GetApplication(byte? id)
        {
            return new JsonResult { Data = _unitOfWork.ApplicationRepository.GetByValue(id) };
        }

        public void Delete(byte? id)
        {
            _unitOfWork.ApplicationRepository.IsDeleteTrue(id);
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
