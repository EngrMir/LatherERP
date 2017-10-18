using System;
using System.Web;
using System.Web.Mvc;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.BusinessLogicLayer.OperationManager;
using ERP.EntitiesModel.OperationModel;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP_Leather.ActionFilters;

namespace ERP_Leather.Controllers
{
    public class ChemicalForeignPurchaseOrderController : Controller
    {

        DalChemicalForeignPurchaseOrder objDal = new DalChemicalForeignPurchaseOrder();
        BllPrqChemicalForeignPurchaseOrder objBllChemicalForeignPurchaseOrder = new BllPrqChemicalForeignPurchaseOrder();

        [HttpGet]
        [CheckUserAccess("ChemicalForeignPurchaseOrder/ChemicalForeignPurchaseOrder")]
        public ActionResult ChemicalForeignPurchaseOrder()
        {
            ViewBag.formTiltle = "Chemical Foreign Purchase Order";
            return View();
        }

        [HttpPost]
        [CheckUserAccess("ChemicalForeignPurchaseOrder/ChemicalForeignPurchaseOrder")]
        public ActionResult ChemicalForeignPurchaseOrder(ChemicalForeignPurchaseOrder model)
        {
            if (model.OrderNo == null)
            {
                var msg = objBllChemicalForeignPurchaseOrder.Save(model, Convert.ToInt32(Session["UserID"]), "ChemicalForeignPurchaseOrder/ChemicalForeignPurchaseOrder");
                long OrderID = objBllChemicalForeignPurchaseOrder.GetpurchaseID();
                var OrderNo = objDal.GetOrderNo(OrderID);
                var RequisitionList = objDal.GetRequisitionInformationForSingleOrder((OrderID).ToString());

                return Json(new { Msg = msg, RequisitionList = RequisitionList, OrderID = OrderID, OrderNo = OrderNo }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var msg = objBllChemicalForeignPurchaseOrder.Update(model, Convert.ToInt32(Session["UserID"]));
                var RequisitionList = objDal.GetRequisitionInformationForSingleOrder(model.OrderID);


                return Json(new { Msg = msg, RequisitionList = RequisitionList }, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpGet]
        [CheckUserAccess("ChemicalForeignPurchaseOrder/ChemicalLocalPurchaseOrder")]
        public ActionResult ChemicalLocalPurchaseOrder()
        {
            ViewBag.formTiltle = "Chemical Local Purchase Order";
            return View();
        }

        [HttpPost]
        [CheckUserAccess("ChemicalForeignPurchaseOrder/ChemicalLocalPurchaseOrder")]
        public ActionResult ChemicalLocalPurchaseOrder(ChemicalForeignPurchaseOrder model)
        {
            if (model.OrderNo == null)
            {
                var msg = objBllChemicalForeignPurchaseOrder.Save(model, Convert.ToInt32(Session["UserID"]), "ChemicalForeignPurchaseOrder/ChemicalForeignPurchaseOrder");
                long OrderID = objBllChemicalForeignPurchaseOrder.GetpurchaseID();
                var OrderNo = objDal.GetOrderNo(OrderID);
                var RequisitionList = objDal.GetRequisitionInformationForSingleOrder((OrderID).ToString());

                return Json(new { Msg = msg, RequisitionList = RequisitionList, OrderID = OrderID, OrderNo = OrderNo }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var msg = objBllChemicalForeignPurchaseOrder.Update(model, Convert.ToInt32(Session["UserID"]));
                var RequisitionList = objDal.GetRequisitionInformationForSingleOrder(model.OrderID);


                return Json(new { Msg = msg, RequisitionList = RequisitionList }, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult GetSupplierListForChemicalForeignPurchaseOrder(string _SupplierType)
        {
            var Supplier = objDal.GetSupplierListForChemicalForeignPurchaseOrder(_SupplierType);
            return Json(Supplier, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLocalAgentListForChemicalForeignPurchaseOrder(string _SupplierType)
        {
            var LocalAgent = objDal.GetLocalAgentListForChemicalForeignPurchaseOrder(_SupplierType);
            return Json(LocalAgent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetForeignAgentListForChemicalForeignPurchaseOrder(string _SupplierType)
        {
            var ForiegnAgent = objDal.GetForeignAgentListForChemicalForeignPurchaseOrder(_SupplierType);
            return Json(ForiegnAgent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOrderInformationForSearch(string _OrderCategory)
        {
            var Orders = objDal.GetOrderInformation(_OrderCategory);
            return Json(Orders, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDetailOrderInformation(string OrderID)
        {
            var AllData = objDal.GetDetailOrderInformation(OrderID);
            return Json(AllData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAgentInformation(string SupplierID, string LocalAgentID, string ForeignAgentID)
        {
            if(SupplierID != null)
            {
                var Data = objDal.GetSupplierAgentInformation(Convert.ToInt16(SupplierID), 0, 0);
                return Json(Data, JsonRequestBehavior.AllowGet);
            }
            else if (LocalAgentID != null)
            {
                var Data = objDal.GetSupplierAgentInformation(0, Convert.ToInt16(LocalAgentID), 0);
                return Json(Data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var Data = objDal.GetSupplierAgentInformation(0, 0, Convert.ToInt16(ForeignAgentID));
                return Json(Data, JsonRequestBehavior.AllowGet);
            }
            
        }

        public ActionResult GetChemicalRequisition( string _RequisitionCategory, string _SupplierID)
        {
            var Data = objDal.GetChemicalRequisition(_RequisitionCategory, _SupplierID);
            return Json(Data, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetChemicalRequisitionItemList(string _RequisitionID, string OrderID)
        {
            if(OrderID !=null && OrderID != "")
            {
                var Data = objDal.GetChemicalRequisitionItemList(_RequisitionID, OrderID);
                return Json(Data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var Data = objDal.GetChemicalRequisitionItemList(_RequisitionID);
                return Json(Data, JsonRequestBehavior.AllowGet);
            }
            
        }

        public ActionResult GetAllChemicalItem()
        {
            var ItemList = objDal.GetAllChemicalItem();
            return Json(ItemList, JsonRequestBehavior.AllowGet);
        }

         public ActionResult GetAllPackSizeForChemical()
         {
             var Data = objDal.GetAllPackSizeForChemical();
            return Json(Data, JsonRequestBehavior.AllowGet);
         }

         public ActionResult GetAllUnitForChemicalPackSize()
         {
             var Data = objDal.GetAllUnitForChemicalPackSize();
             return Json(Data, JsonRequestBehavior.AllowGet);
         }

         public ActionResult GetAllUnitForChemical()
         {
             var Data = objDal.GetAllUnitForChemical();
             return Json(Data, JsonRequestBehavior.AllowGet);
         }

         public ActionResult ConfirmOrder(string OrderID, string confirmComment)
         {
             bool checkConfirm = objDal.ConfirmOrder(OrderID, confirmComment);

             if (checkConfirm)
             {
                 return Json(new { Msg = "Success" }, JsonRequestBehavior.AllowGet);
             }
             else
             {
                 return Json(new { Msg = "Fail" }, JsonRequestBehavior.AllowGet);
             }
         }


         public ActionResult DeleteRequisitionItem(string OrderItemID, string _PageStatus)
         {
             bool CheckStatus = false; ;
             if (_PageStatus == "NCF" || _PageStatus== "Not Confirmed")
             {
                 CheckStatus = objDal.DeleteRequisitionItem(OrderItemID);
                 if (CheckStatus)
                 {
                     return Json(new { Msg = "Success" }, JsonRequestBehavior.AllowGet);
                 }
                 else
                 {
                     return Json(new { Msg = "Fail" }, JsonRequestBehavior.AllowGet);
                 }
             }
             else if (_PageStatus == "CNF" || _PageStatus== "Confirmed")
             {
                 return Json(new { Msg = "CNF" }, JsonRequestBehavior.AllowGet);
             }
             else
             {
                 return Json(new { Msg = "Fail" }, JsonRequestBehavior.AllowGet);
             }
         }

         public ActionResult DeleteRequisition(string OrderID, string RequisitionID, string _PageStatus)
         {
             bool CheckStatus = false; ;
             if (_PageStatus == "NCF" || _PageStatus == "Not Confirmed")
             {
                 CheckStatus = objDal.DeleteRequisition(OrderID, RequisitionID);
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

         public ActionResult DeleteOrder(string OrderID, string _PageStatus)
         {
             bool CheckStatus = false; ;
             if (_PageStatus == "NCF" || _PageStatus == "Not Confirmed")
             {
                 CheckStatus = objDal.DeleteOrder(OrderID);
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

        public ActionResult ConsealRequisition(string RequisitionID)
         {
             var CheckStatus = objDal.ConsealRequisition(RequisitionID);

             if (CheckStatus)
             {
                 return Json(new { Msg = "Success" }, JsonRequestBehavior.AllowGet);
             }
             else
             {
                 return Json(new { Msg = "Fail" }, JsonRequestBehavior.AllowGet);
             }
         }

       
	}
}