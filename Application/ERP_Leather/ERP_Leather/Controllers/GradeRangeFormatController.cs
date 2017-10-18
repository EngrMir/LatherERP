using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using ERP_Leather.ActionFilters;
using ERP.EntitiesModel.OperationModel;
using System;

namespace ERP_Leather.Controllers
{
    public class GradeRangeFormatController : Controller
    {
        private DalSysGradeRangeFormat _dalGradeRangeFormat;
        private SysGradeRangeFormat _sysGradeRangeFormat = new SysGradeRangeFormat();
        private ValidationMsg _vmMsg;

        public GradeRangeFormatController()
        {
            _vmMsg = new ValidationMsg();
            _dalGradeRangeFormat = new DalSysGradeRangeFormat();
        }

        [CheckUserAccess("GradeRangeFormat/GradeRangeFormat")]
        public ActionResult GradeRangeFormat()
        {
            ViewBag.formTiltle = "Grade Range Format Form";
            return View(_sysGradeRangeFormat);
        }

        [HttpPost]
        public ActionResult Update(string models)
        {
            var sysGradeRangeFormat = new JavaScriptSerializer().Deserialize<IList<SysGradeRangeFormat>>(models).FirstOrDefault();
            _vmMsg = _dalGradeRangeFormat.Update(sysGradeRangeFormat, Convert.ToInt32(Session["UserID"]));

            var result = new { data = sysGradeRangeFormat, message = _vmMsg };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGradeRangeFormatList()
        {
            return new JsonResult { Data = _dalGradeRangeFormat.GetAll().OrderByDescending(s => s.FormatID) };
        }
        public JsonResult GetGradeRangeFormatList2()
        {
            return new JsonResult { Data = "" };
        }

        public JsonResult btnSaveGradeRange(GradeRangeTitle model)
        {
            var allGradeItem = _dalGradeRangeFormat.SaveGradeRange(model, Convert.ToInt32(Session["UserID"]));
            return Json(allGradeItem, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListOfGradeRange()
        {
            var allData = _dalGradeRangeFormat.GetGradeformation();
            return Json(allData, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult Delete(string models)
        {
            var sysGradeRangeFormat = new JavaScriptSerializer().Deserialize<IList<SysGradeRangeFormat>>(models).FirstOrDefault();
            _vmMsg = _dalGradeRangeFormat.Delete(sysGradeRangeFormat, Convert.ToInt32(Session["UserID"]));

            return Json(_vmMsg);
        }



        public ActionResult GetAllGradeType()
        {
            var dalGradeRangeFormat = new DalSysGradeRangeFormat();
            var allGradeItem = dalGradeRangeFormat.GetGradeList();

            return Json(allGradeItem, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllGradeRangeType()
        {
            var dalGradeRangeFormat = new DalSysGradeRangeFormat();
            var allGradeItem = dalGradeRangeFormat.GetGradeRangeList();

            return Json(allGradeItem, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDetailGradeRangeInformation(string GradeRangeID)
        {
            var allGradeItem = _dalGradeRangeFormat.GetDetailGradeRangeInformation(GradeRangeID);
            return Json(allGradeItem, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteGradeRangeInfo(string GradeRangeID)
        {
            var allGradeItem = _dalGradeRangeFormat.DeleteGradeRangeInfo(GradeRangeID, Convert.ToInt32(Session["UserID"]));
            return Json(allGradeItem, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteGradeRangeFormatById(string FormatID)
        {
            _vmMsg = _dalGradeRangeFormat.DeleteGradeRangeFormatById(FormatID, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }

    }
}