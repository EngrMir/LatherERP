using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.BusinessLogicLayer.OperationManager;
using ERP.EntitiesModel.OperationModel;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP_Leather.ActionFilters;


namespace ERP_Leather.Controllers
{
    public class ProformaInvoiceController : Controller
    {

        
        DalProformaInvoice objDALPI = new DalProformaInvoice();
        BLLProformaInvoice objBLLPI = new BLLProformaInvoice();

        [HttpGet]
        [CheckUserAccess("ProformaInvoice/ProformaInvoice")]
        public ActionResult ProformaInvoice()
        {
            DalSysCurrency objCurrency = new DalSysCurrency();
            
            
            

            ViewBag.formTiltle = "PI/Indent";
            ViewBag.CurrencyList = objCurrency.GetAllActiveCurrency();
            //ViewBag.CountryList = objCountry.GetAll();
            //ViewBag.BeneficiaryBankList = objBranch.GetBeneficiaryAdvisingBankList();
            //ViewBag.AdvisingBankList = objBranch.GetBeneficiaryAdvisingBankList();
            //ViewBag.PortList = objPort.GetAll();
            return View();
        }

        [HttpPost]
        [CheckUserAccess("ProformaInvoice/ProformaInvoice")]
        public ActionResult ProformaInvoice(PRQChemicalPI model)
        {
            if (model.PIID == 0)
            {

                var msg = objBLLPI.Save(model, Convert.ToInt32(Session["UserID"]), "ProformaInvoice/ProformaInvoice");
                int PIID = objBLLPI.GetPIID();
                var PIItemList = objDALPI.GetPIItemList(PIID);
                return Json(new { Msg = msg, PIID = PIID, PIItemList = PIItemList }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var msg = objBLLPI.Update(model, Convert.ToInt32(Session["UserID"]));
                var PIItemList = objDALPI.GetPIItemList(model.PIID);
                return Json(new { Msg = msg, PIItemList = PIItemList }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetBuyerListForPI()
        {
            var Data = objDALPI.GetBuyerListForPI();
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBeneficiaryAdvisingBankList()
        {
            DalSysBranch objBranch = new DalSysBranch();

            var Data = objBranch.GetBeneficiaryAdvisingBankList();
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllPortList()
        {
            DalSysPort objPort = new DalSysPort();

            var Data = objPort.GetAll();
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllCountryList()
        {
            DalSysCountry objCountry = new DalSysCountry();

            var Data = objCountry.GetAll();
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOrderInformationForLOV()
        {
            var Orders = objDALPI.GetOrderInformationForLOV();
            return Json(Orders, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOrderItemListForLOV(string OrderID)
        {
            var Data = objDALPI.GetOrderItemListForLOV(OrderID);
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPIInformationForSearch()
        {
            var Data = objDALPI.GetPIInformationForSearch();
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPIDetailsAfterSearch(int PIID)
        {
            var Data = objDALPI.GetPIDetailsAfterSearch(PIID);

            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ConfirPI(string PIID, string confirmComment)
        {
            bool checkConfirm = objDALPI.ConfirmPI(PIID, confirmComment);

            if (checkConfirm)
            {
                return Json(new { Msg = "Success" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Msg = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeletePIItem(string _PIItemID, string _PageStatus)
        {
            bool CheckStatus = false; ;
            if (_PageStatus == "NCF" || _PageStatus == "Not Confirmed")
            {
                CheckStatus = objDALPI.DeletePIItem(_PIItemID);
                if (CheckStatus)
                {
                    return Json(new { Msg = "Success" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Msg = "Fail" }, JsonRequestBehavior.AllowGet);
                }
            }
            else if (_PageStatus == "CNF" || _PageStatus == "Confirmed")
            {
                return Json(new { Msg = "CNF" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Msg = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeletePI(string _PIID, string _PageStatus)
        {
            bool CheckStatus = false; ;
            if (_PageStatus == "NCF" || _PageStatus == "Not Confirmed")
            {
                CheckStatus = objDALPI.DeletePI(_PIID);
                if (CheckStatus)
                {
                    return Json(new { Msg = "Success" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Msg = "Fail" }, JsonRequestBehavior.AllowGet);
                }
            }
            else if (_PageStatus == "CNF" || _PageStatus == "Confirmed")
            {
                return Json(new { Msg = "CNF" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Msg = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }
	}
}