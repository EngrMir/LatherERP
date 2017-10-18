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
    public class PurchaseController : Controller
    {
        DalSysSupplier objSupplier = new DalSysSupplier();
        DalPrqPurchase objPurchase = new DalPrqPurchase();
        BllPrqPurchase objBllPrqPurchase = new BllPrqPurchase();
        DalPrqPurchaseYearTarget objPurchaseTarget = new DalPrqPurchaseYearTarget();

        [HttpGet]
        [CheckUserAccess("Purchase/Purchase")]
        public ActionResult Purchase()
        {

            ViewBag.PurchaseYear = objPurchaseTarget.GetPurchaseYear();
            ViewBag.formTiltle = "Purchase Receive";
            return View();
        }

        [HttpPost]
        [CheckUserAccess("Purchase/Purchase")]
        public ActionResult Purchase(PurchaseReceive model)
        {
            if ( model.PurchaseID == 0)
            {
                
                var msg = objBllPrqPurchase.SavePurchaseReceiveWithChallanItem(model, Convert.ToInt32(Session["UserID"]));
                long purchaseID = objBllPrqPurchase.GetpurchaseID();
                string PurchaseNo = objPurchase.GetPurchaseNumber(purchaseID);
                var ChallanList = objPurchase.GetChallanList(purchaseID);

                return Json(new { Msg = msg, purchaseID = purchaseID, PurchaseNo = PurchaseNo, challanList = ChallanList }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var msg = objBllPrqPurchase.UpdatePurchaseReceiveWithChallanItem(model, Convert.ToInt32(Session["UserID"]));
                var ChallanList = objPurchase.GetChallanList(Convert.ToInt64(model.PurchaseID));


                return Json(new { Msg = msg, challanList = ChallanList }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetSupplierList()
        {
            var supplier = objSupplier.GetCategoryWiseSupplier("Leather");
            return Json(supplier, JsonRequestBehavior.AllowGet);
        }


        //For Search Window Grid
        public ActionResult GetPurchaseInformation()
        {

            var allData = objPurchase.GetPurchaseInformation();
            return Json(allData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDetailPurchaseInformation(string PurchaseNumber)
        {
            var AllData = objPurchase.GetDetailPurchaseInformation(PurchaseNumber);
            return Json(AllData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetItemListForChallan(string ChallanID)
        {
            var ItemList = objPurchase.GetItemListForChallan(ChallanID);
            return Json(ItemList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ConfirmPurchase(string purchaseNumber, string confirmComment)
        {
            bool checkConfirm = objPurchase.ConfirmPurchase(purchaseNumber, confirmComment);

            if (checkConfirm)
            {
                return Json(new { Msg = "Success" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Msg = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeleteChallanItem(string ChallanItemID, string _PageStatus)
        {
            bool CheckStatus = false; ;
            if (_PageStatus == "NCF" || _PageStatus == "Not Confirmed")
            {
                 CheckStatus = objPurchase.DeleteChallanItem(ChallanItemID);
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

        public ActionResult DeleteChallan(string ChallanID, string _PageStatus)
        {
            bool CheckStatus = false; ;
            if (_PageStatus == "NCF" || _PageStatus == "Not Confirmed")
            {
                CheckStatus = objPurchase.DeleteChallan(ChallanID);
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

        public ActionResult DeletePurchase(string PurchaseID, string _PageStatus)
        {
            bool CheckStatus = false; ;
            if (_PageStatus == "NCF" || _PageStatus == "Not Confirmed")
            {
                CheckStatus = objPurchase.DeletePurchase(PurchaseID);
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