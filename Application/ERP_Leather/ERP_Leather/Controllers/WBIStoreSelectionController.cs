using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.BaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.OperationModel;
using ERP_Leather.ActionFilters;
using System.Data.Entity.Validation;
using System.Text;


namespace ERP_Leather.Controllers
{
    public class WBIStoreSelectionController : Controller
    {

        private DalSysStore _objDalStore;

        private UnitOfWork repository;
        private ValidationMsg _vmMsg;
        private SysCommonUtility _utility;
        private int _userId;
        DalWBIStoreSelection _dalWBIStoreSelectionObj = new DalWBIStoreSelection();

        public WBIStoreSelectionController()
        {
            _vmMsg = new ValidationMsg();
            repository = new UnitOfWork();
            _utility = new SysCommonUtility();
            _objDalStore = new DalSysStore();
        }

        [CheckUserAccess("WBIStoreSelection/WBIStoreSelection")]
        public ActionResult WBIStoreSelection()
        {

            ViewBag.IssueToStoreList = _objDalStore.GetAllActiveWetBlueLeatherStore();
            ViewBag.IssueFromStoreList = _objDalStore.GetAllActiveProductionStore();
            ViewBag.UnitList = _dalWBIStoreSelectionObj.GetAllActiveLeatherUnit();


            return View();
        }


        //####################################### SELECTION POPUP ACCORDING STORE ##################################

        #region SELECTION POPUP ACCORDING STORE

        public ActionResult GetWBSelectionInfo(string StoreID)
        {
            var packItemList = _dalWBIStoreSelectionObj.GetWBSelectionInfo(StoreID);
            return Json(packItemList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetWBSelectionGrade(string WBSelectionID, string IssueFrom, string SupplierID, string PurchaseID, string ItemTypeID, string LeatherStatusID)
        {
            //var packItemList = _dalWBIStoreSelectionObj.GetWBSelectionGrade(WBSelectItemID);
            var packItemList = _dalWBIStoreSelectionObj.GetWBSelectionGrade(WBSelectionID, IssueFrom, SupplierID, PurchaseID, ItemTypeID, LeatherStatusID);
            return Json(packItemList, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetStockGradeInfo(string StoreID)
        {
            var packItemList = _dalWBIStoreSelectionObj.GetStockGradeInfo(StoreID);
            return Json(packItemList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetWBGradeforGrid(string IssueFrom, string SupplierID, string PurchaseID, string ItemTypeID, string LeatherStatusID)
        {
            //var packItemList = _dalWBIStoreSelectionObj.GetWBSelectionGrade(WBSelectItemID);
            var packItemList = _dalWBIStoreSelectionObj.GetWBGradeforGrid(IssueFrom, SupplierID, PurchaseID, ItemTypeID, LeatherStatusID);
            return Json(packItemList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchWBSelectionNo(string search, string WBSelectionNo)
        {
            if (search == null)
            {
                var result = from temp in repository.PrdWBSelectionRepository.Get().AsEnumerable()
                             join temp2 in repository.PrdWBSelectionItemRepository.Get() on temp.WBSelectionID equals temp2.WBSelectionID
                             select new
                             {
                                 WBSelectionID = temp.WBSelectionID,
                                 WBSelectionNo = temp.WBSelectionNo,
                                 SelectionDate = temp.SelectionDate,
                                 SelectedBy = temp.SelectedBy,
                                 Remarks = temp.Remarks,

                             };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                search = search.ToUpper();
                var result = from temp in repository.PrdWBSelectionRepository.Get().AsEnumerable()
                             join temp2 in repository.PrdWBSelectionItemRepository.Get() on temp.WBSelectionID equals temp2.WBSelectionID
                             where ((temp.WBSelectionNo.ToUpper().StartsWith(search) || temp.WBSelectionNo.ToUpper() == search))
                             select new
                             {
                                 WBSelectionID = temp.WBSelectionID,
                                 WBSelectionNo = temp.WBSelectionNo,
                                 SelectionDate = temp.SelectionDate,
                                 SelectedBy = temp.SelectedBy,
                                 Remarks = temp.Remarks,
                             };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult GetWBSelectionNoAutocompleteData()
        {
            var search = repository.PrdWBSelectionRepository.Get().Select(ob => ob.WBSelectionNo);
            return Json(search, JsonRequestBehavior.AllowGet);
        }
        #endregion


        //####################################### SAVE & UPDATE LOGIC ##############################################

        #region WET BLUE SELECTION DATA SAVE & UPDATE

        [HttpPost]
        public ActionResult WBIStoreSelection(wbSelectionIssue model)
        {
            _vmMsg = model.IssueID == 0 ? _dalWBIStoreSelectionObj.Save(model, Convert.ToInt32(Session["UserID"]), "WBIStoreSelection/WBIStoreSelection") : _dalWBIStoreSelectionObj.Update(model, Convert.ToInt32(Session["UserID"]));
            return Json(new { IssueID = _dalWBIStoreSelectionObj.GetIssueID(), WBSIssueItemID = _dalWBIStoreSelectionObj.GetWBSIssueItemID(), IssueNo = _dalWBIStoreSelectionObj.GetIssueNo(), msg = _vmMsg });
        }

        #endregion

        //################################# SEARCH WET BLUE SAVE & CONFIRMED DATA ##################################

        #region  SEARCH WET BLUE SAVE & CONFIRMED DATA



        public ActionResult GetSelectionIssue()
        {
            var data = _dalWBIStoreSelectionObj.SearchMasterInfo();
            var searchIssueDetails = from t in data
                                     select new
                                     {
                                         IssueID = t.IssueID,
                                         IssueNo = t.IssueNo,
                                         WBSIssueItemID = t.WBSIssueItemID,
                                         WBSelectionNo = t.WBSelectionNo,
                                         WBSelectionID = t.WBSelectionID,
                                         SelectionDate = t.SelectionDate,
                                         IssueDate = t.IssueDate,
                                         IssueFrom = t.IssueFrom,
                                         IssueTo = t.IssueTo,
                                         IssueFromName = t.IssueFromName,
                                         IssueToName = t.IssueToName,
                                         RecordStatus = t.RecordStatus,//DalCommon.ReturnRecordStatus(t.RecordStatus),
                                         RecordState = t.RecordState,
                                         CheckNote = t.CheckNote,
                                         SupplierID = t.SupplierID,
                                         SupplierName = t.SupplierName,
                                         ItemTypeID = t.ItemTypeID,
                                         ItemTypeName = t.ItemTypeName,
                                         LeatherStatusID = t.LeatherStatusID,
                                         LeatherStatusName = t.LeatherStatusName,
                                         PurchaseID = t.PurchaseID,
                                         PurchaseNo = t.PurchaseNo,
                                         ProductionPcs = t.ProductionPcs,
                                         ProductionSide = t.ProductionSide,
                                         ProductionArea = t.ProductionArea,
                                         ProductionAreaUnit = t.ProductionAreaUnit,
                                         SelectedBy = t.SelectedBy,//repository.SysUserRepository.Get(filter: ob => ob.UserID == t.SelectedBy).FirstOrDefault().FirstName,
                                         SelectedByName = t.SelectedByName,
                                         IssueCategory = t.IssueCategory//"After Production"


                                     };
            return Json(searchIssueDetails, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchWetBlueIssueNo(string search)
        {
            search = search.ToUpper();
            var data = _dalWBIStoreSelectionObj.SearchMasterInfo();
            var result = (from temp in data
                          where ((temp.WBSelectionNo.ToUpper().StartsWith(search) || temp.WBSelectionNo.ToUpper() == search))
                          select new
                          {
                              IssueID = temp.IssueID,
                              WBSIssueItemID = temp.WBSIssueItemID,
                              SelectionNo = temp.WBSelectionNo,
                              SelectionDate = temp.SelectionDate,
                              SelectedBy = repository.SysUserRepository.Get(filter: ob => ob.UserID == temp.SelectedBy).FirstOrDefault().FirstName,
                              IssueNo = temp.IssueNo,
                              IssueDate = temp.IssueDate,
                              IssueCategory = "After Production",
                              //StoreID = temp.StoreID,
                              //StoreName = temp.StoreName,
                              //RecordStatus = temp.RecordStatus,
                              IssueFrom = temp.IssueFrom,
                              IssueTo = temp.IssueTo,
                              IssueFromName = repository.StoreRepository.Get(filter: ob => ob.StoreID == temp.IssueFrom).FirstOrDefault().StoreName,
                              IssueToName = repository.StoreRepository.Get(filter: ob => ob.StoreID == temp.IssueTo).FirstOrDefault().StoreName,
                              RecordStatus = DalCommon.ReturnRecordStatus(temp.RecordStatus),

                          }).ToList();


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAutoCompleteData()
        {
            var search = repository.PrdWBSellectionIssueRepository.Get().Select(ob => ob.IssueNo);
            return Json(search, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetIssueInfoForSearch(string WBSIssueItemID, string IssueFrom, string SupplierID, string PurchaseID, string ItemTypeID, string LeatherStatusID)
        {
            var searchIssueDetails = _dalWBIStoreSelectionObj.SearchIssueInfo(WBSIssueItemID, IssueFrom, SupplierID, PurchaseID, ItemTypeID, LeatherStatusID);
            return Json(searchIssueDetails, JsonRequestBehavior.AllowGet);
        }


        #endregion

        //################################# CHECK &  COFIRM DATA WITH STOCK UPDATE ##################################

        #region Confirm Save Data & Update Stock
        [HttpPost]
        public ActionResult ConfirmWetBlueSelectionStock(long issueID, long wbSIssueItemID, bool isSelectionLock, long wbSelectionID)
        {
            _vmMsg = _dalWBIStoreSelectionObj.ConfirmWetBlueSelectionStock(issueID, wbSIssueItemID, Convert.ToInt32(Session["UserID"]), isSelectionLock, wbSelectionID);
            return Json(new { msg = _vmMsg });
        }
        #endregion

        #region Check Saved Data

        [HttpPost]
        public ActionResult CheckedRecordStatus(long IssueID, string status)
        {
            ValidationMsg _vmMsg = _dalWBIStoreSelectionObj.CheckedRecordStatus(IssueID, status, Convert.ToInt32(Session["UserID"]));
            return Json(_vmMsg, JsonRequestBehavior.AllowGet);
        }

        #endregion

        //################################# DELETE ALL DATA  ########################################################
        public ActionResult DeleteSelectionData(int id)
        {
            PRD_WBSellectionIssue issue = repository.PrdWBSellectionIssueRepository.GetByID(id);

            if (issue != null)
            {
                var data = (from t in repository.PrdWBSellectionIssueItemRepository.Get() where t.IssueID == issue.IssueID select t);
                foreach (var item in data)
                {
                    PRD_WBSellectionIssueItem issueItem = (from temp in data where temp.WBSIssueItemID == item.WBSIssueItemID select temp).FirstOrDefault();
                    var grade = repository.PrdWBSellectionIssueGradeRepository.Get(ob => ob.WBSIssueItemID == issueItem.WBSIssueItemID);
                    foreach (var item3 in grade)
                    {
                        PRD_WBSellectionIssueGrade gr = (from t in grade where t.WBSIssueGradeID == item3.WBSIssueGradeID select t).FirstOrDefault();
                        repository.PrdWBSellectionIssueGradeRepository.Delete(gr);
                    }
                    repository.PrdWBSellectionIssueItemRepository.Delete(issueItem);
                }
                repository.PrdWBSellectionIssueRepository.Delete(issue);

                try
                {
                    repository.Save();
                    _vmMsg.Type = Enums.MessageType.Success;
                    _vmMsg.Msg = "Data Deleted Successfully.";

                }
                catch (DbEntityValidationException e)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        sb.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                                        eve.Entry.Entity.GetType().Name,
                                                        eve.Entry.State));
                        foreach (var ve in eve.ValidationErrors)
                        {
                            sb.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                                        ve.PropertyName,
                                                        ve.ErrorMessage));
                        }
                    }
                    throw new DbEntityValidationException(sb.ToString(), e);
                }
            }
            else
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Please Delete Grade Info First.";

            }

            return Json(_vmMsg, JsonRequestBehavior.AllowGet);
        }




        #region Start Grade Grid Name Bind
        public ActionResult GetGrade()
        {
            var result = from temp in repository.SysGrade.Get(filter: ob => ob.IsActive == true)
                         select new
                         {
                             GradeID = temp.GradeID,
                             GradeName = temp.GradeName
                         };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion


        //public ActionResult GetAllGrade(string IssueFrom, string SupplierID, string PurchaseID, string ItemTypeID, string LeatherStatusID)
        //{
        //    var allGrade = _dalWBIStoreSelectionObj.GetWBSelectionGrade(IssueFrom, SupplierID, PurchaseID, ItemTypeID,LeatherStatusID);

        //    return Json(allGrade, JsonRequestBehavior.AllowGet);
        //}




        //public ActionResult DeleteItem(string _WBSIssueGradeID, string _PageStatus)
        //{
        //    bool CheckStatus = false; ;
        //    if (_PageStatus == "NCF" || _PageStatus == "Not Confirmed")
        //    {
        //        CheckStatus = _dalWBIStoreSelectionObj.DeleteItem(_WBSIssueGradeID);
        //        if (CheckStatus)
        //        {
        //            return Json(new { Msg = "Success" }, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            return Json(new { Msg = "Fail" }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    else if (_PageStatus == "CNF" || _PageStatus == "Confirmed")
        //    {
        //        return Json(new { Msg = "CNF" }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(new { Msg = "Fail" }, JsonRequestBehavior.AllowGet);
        //    }
        //}


        public ActionResult DeleteSelectionGradeInfo(long WBSIssueGradeID)
        {
            var data = repository.PrdWBSellectionIssueGradeRepository.GetByID(Convert.ToInt64(WBSIssueGradeID));
            if (data != null)
            {
                var chk = repository.PrdWBSellectionIssueItemRepository.GetByID(data.WBSIssueItemID);
                var delStatus = repository.PrdWBSellectionIssueRepository.GetByID(chk.IssueID);
                if (delStatus.RecordStatus != "CNF")
                {
                    repository.PrdWBSellectionIssueGradeRepository.Delete(data);
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


        public ActionResult GetLockedSelection(long WBSelectionID, long IssueID)
        {

            var data = repository.PrdWBSelectionRepository.GetByID(Convert.ToInt64(WBSelectionID));
            var issuedata = repository.PrdWBSellectionIssueRepository.GetByID(Convert.ToInt64(IssueID));

            if (data != null)
            {
                if (issuedata.RecordStatus == "CNF")
                {
                    data.RecordState = "ITS";
                    repository.PrdWBSelectionRepository.Update(data);
                    repository.Save();
                    _vmMsg.Type = Enums.MessageType.Success;
                    _vmMsg.Msg = "Selection Locked Successfully.";
                }
                else
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Issued Data doesn't Confirm";

                }
            }
            else
            {

                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Selection is empty";

            }

            return Json(_vmMsg, JsonRequestBehavior.AllowGet);
        }


    }
}




