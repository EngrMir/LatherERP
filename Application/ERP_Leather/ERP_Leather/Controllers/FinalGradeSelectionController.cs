using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using ERP_Leather.ActionFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_Leather.Controllers
{
    public class FinalGradeSelectionController : Controller
    {
        private UnitOfWork repository;
        private ValidationMsg _vmMsg;
        private SysCommonUtility _utility;
        DalFinalGradeSelection _dalFinalGradeObj = new DalFinalGradeSelection();
        DalSysUnit obSysUnit = new DalSysUnit();
        public FinalGradeSelectionController()
        {
            _vmMsg = new ValidationMsg();
             repository = new UnitOfWork();
             _utility = new SysCommonUtility();
        }
        //
        // GET: /FinalGradeSelection/
        //public ActionResult FinalGradeSelection()
        //{
        //    ViewBag.ddlWetBlueStoreList = _dalFinalGradeObj.GetWetBlueProductionStoreList();
        //    ViewBag.UnitID = new SelectList(from t in obSysUnit.GetAll() where t.UnitCategory=="Leather" select t, "UnitID", "UnitName");
        //    return View();
        //}

         [CheckUserAccess("FinalGradeSelection/FinalGrdSelection")]
        public ActionResult FinalGrdSelection()
        {
            ViewBag.LeatherStatusID = new SelectList((from t in repository.SysLeatherStatusRepo.Get() where t.IsDelete == false && t.IsActive == true select new { t.LeatherStatusID, t.LeatherStatusName }), "LeatherStatusID", "LeatherStatusName");
            ViewBag.ddlWetBlueStoreList = _dalFinalGradeObj.GetWetBlueProductionStoreList();
            ViewBag.UnitID = new SelectList(from t in obSysUnit.GetAll() where t.UnitCategory == "Leather" select t, "UnitID", "UnitName");
            return View();
        }

        #region User Popup Grid
        public ActionResult GetUserInfo()
        {
            var result = from temp in repository.SysUserRepository.Get(filter: o => o.IsActive == true).AsEnumerable()
                                  select new {
                                      UserID = temp.UserID,
                                      SelectedBy = temp.FirstName +" "+ temp.MiddleName+" "+temp.LastName,
                                      Email = temp.Email,
                                      Phone = temp.Phone,
                                      Mobile = temp.Mobile
                                  };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchUserByFirstName(string search)
        {
            if(search == null)
            {
                var result = from temp in repository.SysUserRepository.Get(filter: o => o.IsActive == true).AsEnumerable()
                             select new
                             {
                                 UserID = temp.UserID,
                                 SelectedBy = temp.FirstName + " " + temp.MiddleName + " " + temp.LastName,
                                 Email = temp.Email,
                                 Phone = temp.Phone,
                                 Mobile = temp.Mobile
                             };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            { 
                search = search.ToUpper();
                var result = from temp in repository.SysUserRepository.Get(filter: o => o.IsActive == true).AsEnumerable()
                             where ((temp.FirstName.ToUpper().StartsWith(search) || temp.FirstName.ToUpper() == search))
                             select new
                             {
                                 UserID = temp.UserID,
                                 SelectedBy = temp.FirstName + " " + temp.MiddleName + " " + temp.LastName,
                                 Email = temp.Email,
                                 Phone = temp.Phone,
                                 Mobile = temp.Mobile
                             };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
          
        }

        public ActionResult GetUserAutocompleteData()
        {
            var search = repository.SysUserRepository.Get(filter: o => o.IsActive == true).Select(ob => ob.FirstName);
            return Json(search, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Source Popup Grid
        public ActionResult GetSourceInfo()
        {
            var result = from temp in repository.SysSourceRepository.Get(filter: o => o.IsActive == true).AsEnumerable()
                         select new
                         {
                             SourceID = temp.SourceID,
                             SourceName = temp.SourceName,
                             SourceAddress = temp.SourceAddress,
                             ContactNumber = temp.ContactNumber
                         };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchSourceByFirstName(string search)
        {
            if (search == null)
            {
                var result = from temp in repository.SysSourceRepository.Get(filter: o => o.IsActive == true).AsEnumerable()
                             select new
                             {
                                 SourceID = temp.SourceID,
                                 SourceName = temp.SourceName,
                                 SourceAddress = temp.SourceAddress,
                                 ContactNumber = temp.ContactNumber
                             };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                search = search.ToUpper();
                var result = from temp in repository.SysSourceRepository.Get(filter: o => o.IsActive == true).AsEnumerable()
                             where ((temp.SourceName.ToUpper().StartsWith(search) || temp.SourceName.ToUpper() == search))
                             select new
                             {
                                 SourceID = temp.SourceID,
                                 SourceName = temp.SourceName,
                                 SourceAddress = temp.SourceAddress,
                                 ContactNumber = temp.ContactNumber
                             };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetSourceAutocompleteData()
        {
            var search = repository.SysSourceRepository.Get(filter: o => o.IsActive == true).Select(ob => ob.SourceName);
            return Json(search, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Purchase Popup Grid
        public ActionResult GetPurchaseInfo()
        {
            var result = _dalFinalGradeObj.GetPurchaseInfo();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchPurchaseByFirstName(string search, int SupplierID, int ConcernStore)
        {
            var result = _dalFinalGradeObj.SearchPurchaseByFirstName(search, SupplierID, ConcernStore);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPurchaseAutocompleteData()
        {
            var search = _dalFinalGradeObj.GetPurchaseAutocompleteData();
            return Json(search, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Grade Entry Grid Bind
        public ActionResult GetGrade() {
            var result = from temp in repository.SysGrade.Get(filter: ob => ob.IsActive == true && ob.IsDelete==false) select new {
                GradeID = temp.GradeID,
                GradeName = temp.GradeName
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion
        public ActionResult GetUnit()
        {
            DalSysUnit _dalSysUnit = new DalSysUnit();
            var sysUnit = from temp in _dalSysUnit.GetAll().OrderByDescending(s => s.UnitID)
                          select new {
                              UnitID = temp.UnitID,
                              UnitName = temp.UnitName
                          };
            return Json(sysUnit, JsonRequestBehavior.AllowGet);
        }

        #region Item Type Popup Grid

        public ActionResult GetItemTypeInfo(long purchaseID)
        {
            var result = _dalFinalGradeObj.GetItemTypeInfo(purchaseID);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchItemTypeByFirstName(string search,long purchaseID)
        {
            if(search == null)
            {
                var result = _dalFinalGradeObj.GetItemTypeInfo(purchaseID);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = _dalFinalGradeObj.SearchItemTypeByFirstName(search, purchaseID);  
                return Json(result, JsonRequestBehavior.AllowGet);
            }
          
        }

        public ActionResult GetItemTypeAutocompleteData()
        {
            var search = from temp in repository.SysItemTypeRepository.Get(filter: o => o.IsActive == true && o.ItemTypeCategory == "Leather").AsEnumerable()
                         join temp2 in repository.PrdWetBlueProductionStockRepo.Get() on temp.ItemTypeID equals temp2.ItemTypeID
                         select temp.ItemTypeName;
            return Json(search, JsonRequestBehavior.AllowGet);
        }

        #endregion

       
        public ActionResult GetSelectionInfo(string supplierID,long purchaseID, int itemTypeID, int leatherTypeID, int leatherStatusId ) 
        {
            int suppID = repository.SysSupplierRepository.Get(filter: o => o.SupplierCode == supplierID).FirstOrDefault().SupplierID;
            var data = _dalFinalGradeObj.GetSelectionInfo(suppID, purchaseID, itemTypeID, 22, leatherStatusId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        
        #region Save Data
        public ActionResult Save(wbSelection data)
        {
            ValidationMsg _vmMsg = _dalFinalGradeObj.Save(data, Convert.ToInt32(Session["UserID"]));            
            return Json(_vmMsg, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Update Data
        public ActionResult Update(wbSelection data)
        {
            ValidationMsg _vmMsg = _dalFinalGradeObj.Update(data, Convert.ToInt32(Session["UserID"]), data.WBSelectionID);
            return Json(_vmMsg, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Check
        public ActionResult CheckedRecordStatus(long wbSelectionID, string status)
        {
            ValidationMsg _vmMsg = _dalFinalGradeObj.CheckedRecordStatus(wbSelectionID, status, Convert.ToInt32(Session["UserID"]));
            return Json(_vmMsg, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Confirm
        public ActionResult ConfirmRecordStatus(wbSelection data)
        {
           ValidationMsg _vmMsg= _dalFinalGradeObj.ConfirmRecordStatus(data,Convert.ToInt32(Session["UserID"]));         
            return Json(_vmMsg, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region Search
        public ActionResult GetFinalGradeSelectionInfo()
        {
            var result= _dalFinalGradeObj.GetFinalGradeSelectionInfo();
            var data= (from temp in result  select new {
                WBSelectionID = temp.WBSelectionID,
                WBSelectionNo = temp.WBSelectionNo,
                SelectionDate = Convert.ToDateTime(temp.SelectionDate).ToString("dd/MM/yyyy"),
                StoreName = temp.StoreName,
                SupplierName = temp.SupplierName,
                PurchaseNo = temp.PurchaseNo,
                PurchaseDate = Convert.ToDateTime(temp.PurchaseDate).ToString("dd/MM/yyyy"),
                ItemTypeName = Convert.ToString(temp.ItemTypeName),
                SelectionQty = Convert.ToDecimal(temp.SelectionComplete),
                RecordStatus = DalCommon.ReturnRecordStatus(temp.RecordStatus)
            }).OrderByDescending(ob=>ob.WBSelectionID);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult GetSelectionInfoByID(long id)
        {
            var result =_dalFinalGradeObj.GetFinalGradeSelectionInfo(id);         
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult SearchFinalGradeSelectionNo(string search)
        {
            search = search.ToUpper();
            var result = (from temp in _dalFinalGradeObj.GetFinalGradeSelectionInfo()
                         where ((temp.WBSelectionNo.ToUpper().StartsWith(search) || temp.WBSelectionNo.ToUpper() == search))
                         select new
                         {
                             WBSelectionID = temp.WBSelectionID,
                             WBSelectionNo = temp.WBSelectionNo,
                             SelectionDate = Convert.ToDateTime(temp.SelectionDate).ToString("dd/MM/yyyy"),
                             StoreName = temp.StoreName,
                             SupplierName = temp.SupplierName,
                             PurchaseNo = temp.PurchaseNo,
                             PurchaseDate = Convert.ToDateTime(temp.PurchaseDate).ToString("dd/MM/yyyy"),
                             ItemTypeName = Convert.ToString(temp.ItemTypeName),
                             SelectionQty = Convert.ToDecimal(temp.SelectionComplete),
                             RecordStatus = DalCommon.ReturnRecordStatus(temp.RecordStatus)
                         }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAutoCompleteData()
        {
            var search = repository.PrdWBSelectionRepository.Get().Select(ob => ob.WBSelectionNo);
            return Json(search, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public JsonResult GetSupplierFromStoreList(string ConcernStore)
        {
            var supplierAgentList = _dalFinalGradeObj.GetSupplierFromStoreList(ConcernStore);
            return Json(supplierAgentList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSupplierListSearchById(string supplier)
        {
            var supplierAgentList = _dalFinalGradeObj.GetSupplierListSearchById(supplier);
            return Json(supplierAgentList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLeatherInfoList(string ConcernStore, string SupplierID)
        {
            var packItemList = _dalFinalGradeObj.GetLeatherInfoList(ConcernStore, SupplierID);
            return Json(packItemList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteSelectionGradeInfo(long WBSelectionGradeID)
        {
            var data = repository.PrdWBSelectionGradeRepository.GetByID(Convert.ToInt64(WBSelectionGradeID));
            if(data != null)
            { 
                 var chk = repository.PrdWBSelectionRepository.GetByID(data.WBSelectionID);
                 if (chk.RecordStatus != "CNF")
                 {
                     repository.PrdWBSelectionGradeRepository.Delete(data);
                     int flag = repository.Save();
                     if (flag > 0)
                     {
                         _vmMsg.Type = Enums.MessageType.Success;
                         _vmMsg.Msg = "Data Deleted Successfully.";
                     }
                     else
                     {
                         _vmMsg.Type = Enums.MessageType.Error;
                         _vmMsg.Msg = "Deleted Faild.";
                     }
                 }
                 else
                 {
                     _vmMsg.Type = Enums.MessageType.Error;
                     _vmMsg.Msg = "Data Already Confirmed.";
                 }
               
            }
            return Json(_vmMsg, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public string PurchaseIDBySupplierID(PrqSelectionReport data)
        {
             string dd = "";
             DateTime DateFrom = DalCommon.SetDate(data.DateFrom);
             DateTime DateTo = DalCommon.SetDate(data.DateTo);
             
            var storeLocationList = repository.PrqPurchaseRepository.Get(filter: ob=>ob.SupplierID == data.SupllierId && (ob.PurchaseDate >= DateFrom && ob.PurchaseDate <= DateTo) ).OrderByDescending( o =>o.PurchaseID);
            dd = "<option value='all'>All Purchase Summary</option>";
            if (storeLocationList.Count() > 0)
            { 
                dd += "<option value='alldetails'>All Purchase Details</option>";
                foreach (var item in storeLocationList)
                {
                    dd += "<option value='" + item.PurchaseID + "'> " + item.PurchaseNo + " ("+ item.PurchaseDate.ToString("dd/MM/yyyy") +") </option>";
                }
            }           
            return dd;
        }

        public ActionResult DeleteSelectionData(int id)
        {
            var isGradeExist = repository.PrdWBSelectionGradeRepository.Get(filter: ob=>ob.WBSelectionID == id).ToList();
            if (isGradeExist.Count == 0)
            {
                PRD_WBSelectionItem data = repository.PrdWBSelectionItemRepository.Get(filter: ob => ob.WBSelectionID == id).FirstOrDefault();
                repository.PrdWBSelectionItemRepository.Delete(data);
                PRD_WBSelection dataDelete = repository.PrdWBSelectionRepository.GetByID(id);
                repository.PrdWBSelectionRepository.Delete(dataDelete);
                try
                {
                    repository.Save();
                    _vmMsg.Type = Enums.MessageType.Success;
                    _vmMsg.Msg = "Data Deleted Successfully.";
                }
                catch (Exception ex)
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Deleted Faild.";
                }
            }
            else
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Please Delete Grade Info First.";
                
            }
            return Json(_vmMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetGradeInfo(long id)
        {
            var _vmMsg = _dalFinalGradeObj.GetGrade(id);
            return Json(_vmMsg, JsonRequestBehavior.AllowGet);
        }

      
    }
}