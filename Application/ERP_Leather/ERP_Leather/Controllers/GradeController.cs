using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using ERP_Leather.ActionFilters;

namespace ERP_Leather.Controllers
{
    public class GradeController : Controller
    {

        private DalSysGrade _dalGrade;
        private SysGrade _sysGrade = new SysGrade();
        private ValidationMsg _vmMsg;

        public GradeController()
        {
            _vmMsg = new ValidationMsg();
            _dalGrade = new DalSysGrade();
        }

        [CheckUserAccess("Grade/Grade")]
        public ActionResult Grade()
        {
            ViewBag.formTiltle = "Grade Form";
            return View(_sysGrade);
        }

        [HttpPost]
        public ActionResult Grade(SysGrade model)
        {
            if (model != null && model.GradeID != 0)
            {
                _vmMsg = _dalGrade.Update(model, Convert.ToInt32(Session["UserID"]));
            }
            else
            {
                _vmMsg = _dalGrade.Save(model, Convert.ToInt32(Session["UserID"]));
            }
            return Json(new { GradeID = _dalGrade.GetGradeID(), msg = _vmMsg });
        }


        public JsonResult GetGradeList()
        {
            var AllData = _dalGrade.GetAll().OrderByDescending(s => s.GradeID);
            return Json(AllData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGradeInfoList()
        {
            var AllData = _dalGrade.GetAll().OrderBy(s => s.GradeID);
            return Json(AllData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(string GradeID)
        {
            _vmMsg = _dalGrade.Delete(GradeID, Convert.ToInt32(Session["UserID"]));

            return Json(new { msg = _vmMsg });
        }


    }
}