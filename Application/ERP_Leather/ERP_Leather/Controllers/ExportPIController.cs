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
    public class ExportPIController : Controller
    {
        DalProformaInvoice objDALPI = new DalProformaInvoice();
        DalExportPI objDALExportPI = new DalExportPI();
        BllExportPI objBLLPI = new BllExportPI();

        [CheckUserAccess("ExportPI/ExportPI")]
        public ActionResult ExportPI()
        {
            DalSysCurrency objCurrency = new DalSysCurrency();
            DalSysCountry objCountry = new DalSysCountry();
            DalSysBranch objBranch = new DalSysBranch();
            DalSysPort objPort = new DalSysPort();

            ViewBag.formTiltle = "Proforma Invoice/Contract";
            ViewBag.CurrencyList = objCurrency.GetAllActiveCurrency();
            ViewBag.CountryList = objCountry.GetAll();
            ViewBag.BuyerBank = objBranch.GetCategoryTypeWiseBranchNameWithBank("BNK", "BYR");
            ViewBag.BeneficiaryBank = objBranch.GetCategoryTypeWiseBranchNameWithBank("BNK", "SLR");
            ViewBag.PortList = objPort.GetAll();
            return View();
        }


        [HttpPost]
        [CheckUserAccess("ExportPI/ExportPI")]
        public ActionResult ExportPI(EXPLeatherPI model)
        {
            if (model.PIID == 0)
            {

                var msg = objBLLPI.Save(model, Convert.ToInt32(Session["UserID"]), "ProformaInvoice/ProformaInvoice");
                var PIID = objBLLPI.GetPIID();
                var GrandTotalSFT = objDALExportPI.GetGrandTotalSFT(PIID);
                var GrandTotalSMT = objDALExportPI.GetGrandTotalSMT(PIID);
                var PIItemList = objDALExportPI.GetPIItemList(PIID);
                var OrderDeliveryList = objDALExportPI.BuyerOrderDeliveryListAfterSave(model.BuyerOrderID);
                return Json(new
                {
                    Msg = msg,
                    PIID = PIID,
                    GrandTotalSFT = GrandTotalSFT,
                    GrandTotalSMT = GrandTotalSMT,
                    PIItemList = PIItemList,
                    OrderDeliveryList = OrderDeliveryList,
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var msg = objBLLPI.Update(model, Convert.ToInt32(Session["UserID"]));
                var PIItemList = objDALExportPI.GetPIItemList(model.PIID);
                var GrandTotalSFT = objDALExportPI.GetGrandTotalSFT(model.PIID);
                var GrandTotalSMT = objDALExportPI.GetGrandTotalSMT(model.PIID);
                var OrderDeliveryList = objDALExportPI.BuyerOrderDeliveryListAfterSave(model.BuyerOrderID);
                return Json(new
                {
                    Msg = msg,
                    GrandTotalSFT = GrandTotalSFT,
                    GrandTotalSMT = GrandTotalSMT,
                    PIItemList = PIItemList,
                    OrderDeliveryList = OrderDeliveryList,
                }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult GetBuyerListForPI()
        {
            var Data = objDALExportPI.GetBuyerListForPI();
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLocalBuyerAgent()
        {
            var Data = objDALExportPI.GetLocalBuyerAgent();
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetForeignBuyerAgent()
        {
            var Data = objDALExportPI.GetForeignBuyerAgent();
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBeneficiaryListForPI()
        {
            var Data = objDALExportPI.GetBeneficiaryListForPI();
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOrderListForParticularBuyer(string _BuyerID)
        {
            return Json(objDALExportPI.GetOrderListForParticularBuyer(_BuyerID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBuyerOrderDetails(string _BuyerOrderID)
        {
            var BuyerOrderDeliveryList = objDALExportPI.BuyerOrderDeliveryList(Convert.ToInt64(_BuyerOrderID));

            return Json(new { BuyerOrderDeliveryList = BuyerOrderDeliveryList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOrderItemList(string _BuyerOrderID)
        {
            return Json(objDALExportPI.GetOrderItemList(_BuyerOrderID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOrderItemColorList(string _BuyerOrderID, string _BuyerOrderItemId)
        {
            return Json(objDALExportPI.GetOrderItemColorList(_BuyerOrderID, _BuyerOrderItemId), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPIInformationForSearch()
        {
            var Data = objDALExportPI.GetPIInformationForSearch();
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPIDetailsAfterSearch(int PIID)
        {
            var Data = objDALExportPI.GetPIDetailsAfterSearch(PIID);

            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetColorListForPIItem(long _PIItemID)
        {
            return Json(objDALExportPI.GetColorListForPIItem(_PIItemID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeletePIItem(string _PIItemID, string _PageStatus)
        {
            bool CheckStatus = false; ;
            if (_PageStatus == "NCF" || _PageStatus == "Not Confirmed")
            {
                CheckStatus = objDALExportPI.DeletePIItem(_PIItemID);
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

        public ActionResult DeletePIItemColor(string _PIItemColorID, string _PageStatus)
        {
            bool CheckStatus = false; ;
            if (_PageStatus == "NCF" || _PageStatus == "Not Confirmed")
            {
                CheckStatus = objDALExportPI.DeletePIItemColor(_PIItemColorID);
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


        public ActionResult DeleteDelivery(string _BuyerOrderDeliveryID, string _PageStatus)
        {
            bool CheckStatus = false; ;
            if (_PageStatus == "NCF" || _PageStatus == "Not Confirmed")
            {
                CheckStatus = objDALExportPI.DeleteDelivery(_BuyerOrderDeliveryID);
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


        public ActionResult DeletePrice(string _BuyerOrderPriceID, string _PageStatus)
        {
            bool CheckStatus = false; ;
            if (_PageStatus == "NCF" || _PageStatus == "Not Confirmed")
            {
                CheckStatus = objDALExportPI.DeletePrice(_BuyerOrderPriceID);
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
                CheckStatus = objDALExportPI.DeletePI(_PIID);
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


        public ActionResult ConfirmPI(string _PIID, string confirmComment)
        {
            bool checkConfirm = objDALExportPI.ConfirmPI(_PIID, confirmComment);

            if (checkConfirm)
            {
                return Json(new { Msg = "Success" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Msg = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetColorList()
        {
            var objColor = new DalSysColor();

            return Json(objColor.GetAllActiveColor(), JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetAllExportPi()
        {
            var data = objDALExportPI.GetAllExportPi();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}