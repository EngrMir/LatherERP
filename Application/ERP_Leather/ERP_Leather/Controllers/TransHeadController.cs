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
    public class TransHeadController : Controller
    {
        private DalTransHead _dalTransHead;
        private SysTransHead _SysTransHead = new SysTransHead();
        private ValidationMsg _vmMsg;

        public TransHeadController()
        {
            _vmMsg = new ValidationMsg();
            _dalTransHead = new DalTransHead();
        }

        [CheckUserAccess("TransHead/TransHead")]
        public ActionResult TransHead()
        {
            ViewBag.formTiltle = "Transaction Head";
            return View(_SysTransHead);
        }




        [HttpPost]
        [CheckUserAccess("TransHead/TransHead")]
        public ActionResult TransHead(SysTransHead model)
        {
            if (model != null && model.HeadID != 0)
            {
                _vmMsg = _dalTransHead.Update(model, Convert.ToInt32(Session["UserID"]));
            }
            else
            {
                _vmMsg = _dalTransHead.Save(model, Convert.ToInt32(Session["UserID"]));
            }
            return Json(new { HeadID = _dalTransHead.GetHeadID(), msg = _vmMsg });
        }


        public JsonResult GetHeadList()
        {
            var AllData = _dalTransHead.GetAll().OrderByDescending(s => s.HeadID);
            return Json(AllData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllActiveHead()
        {
            var AllData = _dalTransHead.GetAllActiveHead().OrderByDescending(s => s.HeadID);
            return Json(AllData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetHeadInfoList()
        {
            var AllData = _dalTransHead.GetAll().OrderBy(s => s.HeadID);
            return Json(AllData, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult GetCategoryWiseTransHead(string _HeadCategory)
        //{
        //    var Data = _dalTransHead.GetCategoryWiseTransHead(_HeadCategory);

        //    return Json(Data, JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        public ActionResult Delete(string HeadID)
        {
            _vmMsg = _dalTransHead.Delete(HeadID, Convert.ToInt32(Session["UserID"]));

            return Json(new { msg = _vmMsg });
        }
	}
}