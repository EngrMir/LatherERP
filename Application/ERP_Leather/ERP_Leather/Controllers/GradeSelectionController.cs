using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.OperationModel;
using System.Web.Script.Serialization;
using ERP_Leather.ActionFilters;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Data;
using ERP.DatabaseAccessLayer.DB;

namespace ERP_Leather.Controllers
{
    public class GradeSelectionController : Controller
    {
        private DalSysGrade _dalSysGrade = new DalSysGrade();
        private DalSysStore _dalSysStore = new DalSysStore();
        private DalSysGradeRange _dalSysGradeRange = new DalSysGradeRange();
        private DalPrqPurchaseChallan _dalPrqPurchaseChallan = new DalPrqPurchaseChallan();
        private DalPrqPreGradeSelection _dalPrqPreGradeSelection = new DalPrqPreGradeSelection();
        private DalPrqPreGradeSelectedData _dalGradeSelection = new DalPrqPreGradeSelectedData();
        private DalSysSupplier _dalSysSupplier = new DalSysSupplier();
        private DalUser _dalUser = new DalUser();

        //[CheckUserAccess("GradeSelection/GradeSelection")]
        public ActionResult GradeSelection()
        {
            var usrList = _dalUser.GetAllUser();
            List<User> lst =new List<User>();
            foreach (var item in usrList)
            {
                User ob =new User();
                ob.UserID = item.UserID;
                ob.UserCode = item.Title + " " + item.FirstName + " " + item.MiddleName + " " + item.LastName;
                lst.Add(ob);
            }

            ViewBag.GradeList = _dalSysGrade.GetAll();
            ViewBag.UnitList = new DalSysUnit().GetAllActiveUnit().Select(cl => new SelectListItem { Text = cl.UnitName, Value = Convert.ToString(cl.UnitID) }).ToList();
            ViewBag.CheckedBy = new SelectList(lst, "UserID", "UserCode");
            ViewBag.ApprovedBy = new SelectList(lst, "UserID", "UserCode");
            ViewBag.formTiltle = "Grade Selection Form";
            return View();
        }

        public JsonResult GetChallanList(string supplierId)
        {
            var challanList = _dalPrqPurchaseChallan.GetChallanListInfo(supplierId);
            var items = challanList.Select(cl => new SelectListItem { Text = cl.ChallanNo, Value = Convert.ToString(cl.ChallanID) }).ToList();
            return new JsonResult { Data = items };
        }

        public JsonResult GetPurchaseList(string supplierId, string storeId)
        {
            var purchaseList = _dalPrqPurchaseChallan.GetPurchaseListInfo(supplierId, storeId);
          //  var items = purchaseList.Select(cl => new SelectListItem { Text = cl.PurchaseID.ToString(), Value = Convert.ToString(cl.PurchaseID) }).ToList();
            return new JsonResult { Data = purchaseList };
        }

        //public ActionResult GetChallanItemSumBySourceID(string id)
        //{
        //    //string id = "1002";
        //    var challanList = _dalPrqPurchaseChallan.GetChallanItemSumBySourceID(id);
        //    JavaScriptSerializer ob = new JavaScriptSerializer();
        //    return Json(challanList, JsonRequestBehavior.AllowGet);
        //    //var items = challanList.Select(cl => new SelectListItem {Text = cl.ChallanNo, Value = Convert.ToString(cl.ChallanID)}).ToList();
        //    //return new JsonResult { Data = items };
        //}


        public JsonResult GetStoreLoactionList(string supplierId)
        {
            var storeLocationList = _dalPrqPurchaseChallan.GetStoreLoactionList(supplierId);
            var items = storeLocationList.Select(cl => new SelectListItem { Text = cl.StoreName, Value = Convert.ToString(cl.StoreID) }).ToList();
            return new JsonResult { Data = items };
        }

        public JsonResult GetChallanInfoForChallanTable(string purchaseID)
        {
            var purchaseInfo = _dalPrqPreGradeSelection.GetChallanInfoForChallanTable(purchaseID);
            return new JsonResult { Data = purchaseInfo };
        }

        public JsonResult SaveGradeSelectionData(PreGradeSelectionData model)
        {
            var confirmMessage =  _dalPrqPreGradeSelection.SaveGradeSelectionData(model);
            return new JsonResult { Data = confirmMessage };
        }

        public JsonResult GetSelectionQtyData(string purchaseId, string itemTypeID)
        {
            var gradeSelectionInfo = _dalPrqPreGradeSelection.GetGradeSelectionData(Convert.ToInt64(purchaseId),Convert.ToByte( itemTypeID));
            return new JsonResult { Data = gradeSelectionInfo };
        }

        public JsonResult GradeSelectionConfirm(string selectionID)
        {
            if (selectionID == "")
            {
                selectionID = "0";
            }
            var confirmMessage = _dalPrqPreGradeSelection.GradeSelectionConfirm(Convert.ToInt64(selectionID));
            return new JsonResult { Data = confirmMessage };
        }

        public JsonResult setSNR(string purchaseId) 
        {        
            if (purchaseId != "")
            {
              var  confirmMessage = _dalPrqPreGradeSelection.setSNR(purchaseId);
              return new JsonResult { Data = confirmMessage };
            }

            return new JsonResult { Data = "" };

        }
        
        [HttpPost]
        public string DD_GetSupplierByStockID(string id)
        {
            int ids = Convert.ToInt32(id);
            string dd = "";
            var supplierList = _dalPrqPurchaseChallan.GetSupplierByStockID(id);
            dd = "<option>-- Select One --</option>";
            foreach (var item in supplierList)
            {
                dd += "<option value='" + item.SupplierID + "'>" + item.SupplierName + "</option>";
            }
            return dd;
        }
        [HttpPost]
        public string DD_GetStockBySupplierID(string id)
        {
            int ids = Convert.ToInt32(id);
            string dd = "";
            var storeLocationList = _dalPrqPurchaseChallan.GetStoreLoactionList(id);
            dd = "<option>-- Select One --</option>";
            foreach (var item in storeLocationList)
            {
                dd += "<option value='" + item.StoreID + "'>" + item.StoreName + "</option>";
            }
            return dd;
        }

        [HttpPost]
        public string DDLGetSelectionID(PrqSelectionReport data)
        {
            string dd = "";
            var storeLocationList = _dalPrqPurchaseChallan.GetSelectionListID(data.StoreId, data.SupllierId, data.DateFrom, data.DateTo);
            dd = "<option>-- Select One --</option>";
            foreach (var item in storeLocationList)
            {
                dd += "<option value='" + item.SelectionID + "'> " + item.SelectionID + " ("+ Convert.ToDateTime(item.SelectionDate).ToString("dd/MM/yyyy")+") </option>";
            }
            return dd;
        }

        [HttpPost]
        public string PurchaseDDL(PrqSelectionReport data)
        {
            string dd = "";
            List<Prq_Purchase> storeLocationList = _dalPrqPurchaseChallan.GetPurchaseListID(data.SupllierId, data.DateFrom, data.DateTo);
           // var lst= storeLocationList.GroupBy(test => test.PurchaseID).Select(group => group.First());
            dd = "<option>-- Select All --</option>";
            foreach (var item in storeLocationList)
            {
                dd += "<option value='" + item.PurchaseID + "'> " + item.PurchaseNo + " ("+ item.PurchaseDate.ToString("dd/MM/yyyy")+") </option>";
            }
            return dd;
        }
        
	}
}