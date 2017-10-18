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
    public class DesignationController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        public ActionResult Index()
        {
            if (!Convert.ToBoolean(Session["IsLogged"])) return RedirectToAction("Index", "Home");
            var designationViewModel = new DesignationVM
            {
                Designations = _unitOfWork.DesignationRepository.Get().Where(s => s.IsDelete == false).OrderByDescending(s=>s.DesignationID).ToDesignationView(),
            };
            ViewBag.DesignationList = designationViewModel.Designations.ToList();
            return View(designationViewModel);
        }

        public JsonResult Save([Bind(Include = "Name,DesignationID,Description,IsActive,SetOn,SetBy")] DesignationView designationView, bool isInsert)
        {
            var designation = new Designation
            {
                DesignationID = Convert.ToByte(designationView.DesignationID),
                Description = designationView.Description,
                Name = designationView.Name,
                IsActive = designationView.IsActive,
                IsDelete = false,
                SetOn = DateTime.Now,
                SetBy = LoginInformation.UserID
            };

            if (!ModelState.IsValid)
                return new JsonResult
                {
                    Data =
                        _unitOfWork.DesignationRepository.GetByID(Convert.ToByte(designation.DesignationID))
                            .ToDesignationView()
                };

            if (isInsert)
            {
                _unitOfWork.DesignationRepository.Insert(designation);
            }
            else
            {
                _unitOfWork.DesignationRepository.Update(designation);
            }
            _unitOfWork.Save();

            return new JsonResult { Data = _unitOfWork.DesignationRepository.GetByID(Convert.ToByte(designation.DesignationID)).ToDesignationView() };
        }

        public JsonResult GetDesignation(byte? id)
        {
            return new JsonResult { Data = _unitOfWork.DesignationRepository.GetByID(id).ToDesignationView(), };
        }

        public void Delete(byte? id)
        {
            _unitOfWork.DesignationRepository.IsDeleteTrue(id);
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