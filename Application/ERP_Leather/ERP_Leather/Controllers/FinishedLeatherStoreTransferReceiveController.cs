using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using ERP_Leather.ActionFilters;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using System.Data.Entity.Validation;
using System.Text;

namespace ERP_Leather.Controllers
{
    public class FinishedLeatherStoreTransferReceiveController : Controller
    {

        public DalSysStore _objDalStore = new DalSysStore();
        public DalFinishedLeatherStoreTransferReceive _objDalFinishedLeatherStoreTransferReceive = new DalFinishedLeatherStoreTransferReceive();
        private UnitOfWork repository;
        private ValidationMsg _vmMsg;
        private SysCommonUtility _utility;
        private int _userId;

        public FinishedLeatherStoreTransferReceiveController()
        {
            _vmMsg = new ValidationMsg();
            repository = new UnitOfWork();
            _utility = new SysCommonUtility();
        }


        public ActionResult FinishedLeatherStoreTransferReceive()
        {
            ViewBag.formTiltle = "Finished Leather Store Transfer Receive";
            ViewBag.ReceiveAtList = _objDalStore.GetAllActiveStoreTransferReceiveStore();
            ViewBag.IssueFromList = _objDalFinishedLeatherStoreTransferReceive.GetIssueFromStockDetail();

            return View();
        }



        #region Start EXPCnfBill Save & Update
        [HttpPost]
        public ActionResult Save(InvFinishLeatherReceive dataSet)
        {
            _userId = Convert.ToInt32(Session["UserID"]);

            _vmMsg = _objDalFinishedLeatherStoreTransferReceive.Save(dataSet, _userId, "FinishedLeatherStoreTransferReceive/FinishedLeatherStoreTransferReceive");
            return Json(new { FNReceiveID = _objDalFinishedLeatherStoreTransferReceive.GetFNReceiveID(), FNReceiveNo = _objDalFinishedLeatherStoreTransferReceive.GetFNReceiveNo(), msg = _vmMsg });
        }

        [HttpPost]
        public ActionResult Update(InvFinishLeatherReceive dataSet)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            _vmMsg = _objDalFinishedLeatherStoreTransferReceive.Update(dataSet, _userId);
            return Json(new { msg = _vmMsg });
        }



        //################################# DELETE ALL DATA  ########################################################
        public ActionResult DeleteAllReceiveData(int id)
        {
            INV_FinishLeatherReceive receive = repository.FinishLeatherReceiveRepository.GetByID(id);

            if (receive != null)
            {
                var data = (from t in repository.FinishLeatherReceiveItemRepository.Get() where t.FNReceiveID == receive.FNReceiveID select t);
                foreach (var item in data)
                {
                    INV_FinishLeatherReceiveItem receiveItem = (from temp in data where temp.FNReceiveItemID == item.FNReceiveItemID select temp).FirstOrDefault();
                    var color = repository.FinishLeatherReceiveColorRepository.Get(ob => ob.FNReceiveItemID == receiveItem.FNReceiveItemID);
                    foreach (var item3 in color)
                    {
                        INV_FinishLeatherReceiveColor gr = (from t in color where t.FNReceiveColorID == item3.FNReceiveColorID select t).FirstOrDefault();
                        repository.FinishLeatherReceiveColorRepository.Delete(gr);
                    }
                    repository.FinishLeatherReceiveItemRepository.Delete(receiveItem);
                }
                repository.FinishLeatherReceiveRepository.Delete(receive);

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
                _vmMsg.Msg = "Please Delete Item Info First.";

            }

            return Json(_vmMsg, JsonRequestBehavior.AllowGet);
        }
        
        
        #endregion




        #region Receive Stock POP UP List
        public ActionResult GetPopUpDetail(string IssueFrom)
        {
            var StockDetail = _objDalFinishedLeatherStoreTransferReceive.GetPopUpDetail(IssueFrom);
            return Json(StockDetail, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetItemGridStockDetail(long FinishLeatherIssueID)
        {
            var StockDetail = _objDalFinishedLeatherStoreTransferReceive.GetItemGridStockDetail(FinishLeatherIssueID);
            return Json(StockDetail, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetColorGridStockDetail(long FinishLeatherIssueItemID)
        {
            var StockDetail = _objDalFinishedLeatherStoreTransferReceive.GetColorGridStockDetail(FinishLeatherIssueItemID);
            return Json(StockDetail, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchIssueNo(string search)
        {
            if (search == null)
            {
                var result = from temp in repository.FinishLeatherIssueItem.Get().AsEnumerable()
                             join temp1 in repository.FinishLeatherIssueColor.Get() on temp.FinishLeatherIssueID equals temp1.FinishLeatherIssueID
                             join temp4 in repository.FinishLeatherIssue.Get() on temp.FinishLeatherIssueID equals temp4.FinishLeatherIssueID
                             join temp2 in repository.SysBuyerRepository.Get() on temp.BuyerID equals temp2.BuyerID
                             join temp5 in repository.SlsBuyerOrederRepository.Get() on temp.BuyerOrderID equals temp5.BuyerOrderID
                             join temp6 in repository.SysItemTypeRepository.Get() on temp.ItemTypeID equals temp6.ItemTypeID
                             join temp3 in repository.StoreRepository.Get() on temp4.IssueFrom equals temp3.StoreID
                             select new
                             {
                                 FinishLeatherIssueNo = temp4.FinishLeatherIssueNo,
                                 ArticleChallanNo = temp.ArticleChallanNo,
                                 BuyerID = temp.BuyerID,
                                 BuyerName = temp2.BuyerName,
                                 BuyerOrderID = temp.BuyerOrderID,
                                 BuyerOrderNo = temp5.BuyerOrderNo,
                                 ItemTypeID = temp.ItemTypeID,
                                 ItemTypeName = temp6.ItemTypeName
                                 //ArticleNo = temp.ArticleNo,
                                 //StoreID = temp3.StoreID,
                                 //StoreName = temp3.StoreName
                             };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                search = search.ToUpper();
                var result = from temp in repository.FinishLeatherIssueItem.Get().AsEnumerable()
                             join temp1 in repository.FinishLeatherIssueColor.Get() on temp.FinishLeatherIssueID equals temp1.FinishLeatherIssueID
                             join temp4 in repository.FinishLeatherIssue.Get() on temp.FinishLeatherIssueID equals temp4.FinishLeatherIssueID
                             join temp2 in repository.SysBuyerRepository.Get() on temp.BuyerID equals temp2.BuyerID
                             join temp5 in repository.SlsBuyerOrederRepository.Get() on temp.BuyerOrderID equals temp5.BuyerOrderID
                             join temp6 in repository.SysItemTypeRepository.Get() on temp.ItemTypeID equals temp6.ItemTypeID
                             join temp3 in repository.StoreRepository.Get() on temp4.IssueFrom equals temp3.StoreID
                             where ((temp4.FinishLeatherIssueNo.ToUpper().StartsWith(search) || temp4.FinishLeatherIssueNo.ToUpper() == search))
                             select new
                             {
                                 FinishLeatherIssueNo = temp4.FinishLeatherIssueNo,
                                 ArticleChallanNo = temp.ArticleChallanNo,
                                 BuyerID = temp.BuyerID,
                                 BuyerName = temp2.BuyerName,
                                 BuyerOrderID = temp.BuyerOrderID,
                                 BuyerOrderNo = temp5.BuyerOrderNo,
                                 ItemTypeID = temp.ItemTypeID,
                                 ItemTypeName = temp6.ItemTypeName
                             };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult GetIssueNoAutocompleteData()
        {
            var search = repository.FinishLeatherIssue.Get().Select(ob => ob.FinishLeatherIssueNo);
            return Json(search, JsonRequestBehavior.AllowGet);
        }
        #endregion




        #region  SEARCH WET BLUE SAVE & CONFIRMED DATA



        public ActionResult GetFinishRecieveMasterDetail()
        {
            var data = _objDalFinishedLeatherStoreTransferReceive.SearchMasterInfo();
            var searchIssueDetails = (from t in data
                                     select new
                                     {
                                         FNReceiveID = t.FNReceiveID,
                                         FNReceiveNo = t.FNReceiveNo,//FN
                                         FinishLeatherIssueID = t.FinishLeatherIssueID,
                                         FinishLeatherIssueNo = t.FinishLeatherIssueNo,//FinishLeather
                                         FNReceiveDate = t.FNReceiveDate,
                                         ReceiveCategory = t.ReceiveCategory,
                                         IssueFrom = t.IssueFrom,
                                         IssueFromName = t.IssueFromName,
                                         ReceiveAt = t.ReceiveAt,
                                         ReceiveAtName = t.ReceiveAtName,
                                         RecordStatus = t.RecordStatus,//DalCommon.ReturnRecordStatus(t.RecordStatus),
                                     }).OrderByDescending(m=>m.FNReceiveID);
            return Json(searchIssueDetails, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetFinishRecieveItemDetail(long FNReceiveID)
        {
            var data = _objDalFinishedLeatherStoreTransferReceive.SearchItemInfo(FNReceiveID);
            var searchIssueDetails = (from t in data
                                      select new
                                      {
                                          FNReceiveItemID = t.FNReceiveItemID,
                                          BuyerID = t.BuyerID,
                                          BuyerName = t.BuyerName,
                                          BuyerOrderID = t.BuyerOrderID,
                                          BuyerOrderNo = t.BuyerOrderNo,
                                          ArticleID = t.ArticleID,
                                          ArticleNo = t.ArticleNo,
                                          ItemTypeID = t.ItemTypeID,
                                          ItemTypeName = t.ItemTypeName,
                                          LeatherStatusID = t.LeatherStatusID,
                                          LeatherStatusName = t.LeatherStatusName,
                                          ArticleChallanNo = t.ArticleChallanNo
                                      }).OrderByDescending(m => m.FNReceiveItemID);
            return Json(searchIssueDetails, JsonRequestBehavior.AllowGet);
        }



        public ActionResult SearchFinishLeatherReceiveNo(string search)
        {
            search = search.ToUpper();
            var data = _objDalFinishedLeatherStoreTransferReceive.SearchMasterInfo();
            var result = (from temp in data
                          where ((temp.FNReceiveNo.ToUpper().StartsWith(search) || temp.FNReceiveNo.ToUpper() == search))
                          select new
                          {
                              FNReceiveID = temp.FNReceiveID,
                              ReceiveNo = temp.FNReceiveNo,//FN
                              FinishLeatherIssueID = temp.FinishLeatherIssueID,
                              FinishLeatherIssueNo = temp.FinishLeatherIssueNo,
                              IssueNo = temp.FinishLeatherIssueNo,//FinishLeather
                              FNReceiveDate = temp.FNReceiveDate,
                              ReceiveCategory = temp.ReceiveCategory,
                              ReceiveCategoryName = temp.ReceiveCategoryName,
                              ReceiveAt = temp.ReceiveAt,
                              ReceiveAtName = temp.ReceiveAtName,
                              IssueFrom = temp.IssueFrom,
                              IssueFromName = temp.IssueFromName,
                              RecordStatus = temp.RecordStatus,//DalCommon.ReturnRecordStatus(t.RecordStatus),

                          }).ToList();


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAutoCompleteData()
        {
            var search = repository.FinishLeatherReceiveRepository.Get().Select(ob => ob.FNReceiveNo);
            return Json(search, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetReceiveItemColorDetailInfoForSearch(long FNReceiveItemID)//long FNReceiveItemID
        {
            var searchIssueDetails = _objDalFinishedLeatherStoreTransferReceive.SearchReceiveItemColorDetailInfo(FNReceiveItemID);
            return Json(searchIssueDetails, JsonRequestBehavior.AllowGet);
        }


        #endregion


	}
}