using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using ERP.EntitiesModel.AppSetupModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ERP_Leather.ActionFilters;

namespace ERP_Leather.Controllers
{
    public class ComodityController : Controller
    {

        private UnitOfWork repository;
        private ValidationMsg _vmMsg;
        private SysCommonUtility _utility;
        private DalSysComodity dalSysComodity = new DalSysComodity();

        public ComodityController()
        {
            _vmMsg = new ValidationMsg();
            repository = new UnitOfWork();
            _utility = new SysCommonUtility();
        }

         [CheckUserAccess("Comodity/Comodity")]
        public ActionResult Comodity()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetAllComodityData()
        {
            var sysArticle = dalSysComodity.GetCommodity().OrderByDescending(s => s.ComodityID);
            return Json(sysArticle, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult Comodity( SysComodity model)
        {
            if (model != null && model.ComodityID != 0)
            {
                _vmMsg = dalSysComodity.Update(model, Convert.ToInt32(Session["UserID"]));
            }
            else
            {
                _vmMsg = dalSysComodity.Save(model, Convert.ToInt32(Session["UserID"]));
            }
            return Json(new { ComodityID = dalSysComodity.GetComodityID(), msg = _vmMsg });
        }



        // ##################### Search Start ##############
        //public ActionResult GetComoditySearchPopUp()
        //{
        //    var listComodity = (from temp in repository.SysComodityRepository.Get().AsEnumerable() 
        //                           select new
        //                           {
        //                               ComodityID = temp.ComodityID,
        //                               ComodityCode = temp.ComodityCode,
        //                               ComodityName = temp.ComodityName,                                 
        //                               IsActive = temp.IsActive == true ? "Active" : "Inactive"
        //                           }).OrderByDescending(ob => ob.ComodityID); //obLCOpeningDAL.GetLCOpeningInfo();
        //    return Json(listComodity, JsonRequestBehavior.AllowGet);
        //}
        //public ActionResult SearchLcmCnFBillInfoByLCNo(string search)//InsuranceNo
        //{
        //    var commodityInfo = from temp in repository.SysComodityRepository.Get().Where(ob => ob.ComodityCode.StartsWith(search) || ob.ComodityCode == search)
        //                 select new { temp.ComodityID, temp.ComodityCode, temp.ComodityName, temp.IsActive};
        //    return Json(commodityInfo, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult GetAutoCompleteData()
        //{
        //    var data = repository.SysComodityRepository.Get().Select(ob => ob.ComodityCode);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult GetComodityInfoByComodityID(string ComodityID)
        //{
        //    int id = Convert.ToInt32(ComodityID);
        //    Sys_Comodity dataSet = repository.SysComodityRepository.GetByID(id);

        //    SysComodity ob = new SysComodity();

        //    ob.ComodityID = dataSet.ComodityID;
        //    ob.ComodityCode = dataSet.ComodityCode;

        //    ob.ComodityName = dataSet.ComodityName;
        //  //  ob.IsActive = dataSet.IsActive;

        //    return Json(ob, JsonRequestBehavior.AllowGet);

        //}

        // ##################### Search END ##############

        [HttpPost]
        public ActionResult DeleteComodityFromGrid(long ComodityID, int userid)
        {
            var data = dalSysComodity.DeleteComodityFromGrid(ComodityID, userid);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

	}
}