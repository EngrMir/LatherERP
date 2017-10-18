using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using ERP_Leather.ActionFilters;


namespace ERP_Leather.Controllers
{
    public class FinishedLeatherTransferTreatmentController : Controller
    {
        public DalSysStore _objDalStore = new DalSysStore();
        public DalFinishedLeatherTransferTreatment _objDalFinishedLeatherTransferTreatment = new DalFinishedLeatherTransferTreatment();
        private UnitOfWork repository;
        private ValidationMsg _vmMsg;
        private SysCommonUtility _utility;
        private int _userId;

        public FinishedLeatherTransferTreatmentController()
        {
            _vmMsg = new ValidationMsg();
            repository = new UnitOfWork();
            _utility = new SysCommonUtility();
        }
        public ActionResult FinishedLeatherTransferTreatment()
        {
            ViewBag.formTiltle = "Finish Leather Transfer Treatment";
            ViewBag.TransactionStoreList = _objDalStore.GetAllActiveFinishedStore();
            return View();
        }

        #region Start FinishLeather Save & Update, Delete
        [HttpPost]
        public ActionResult FinishedLeatherTransferTreatment(InvFNTransfer model)
        {
            _vmMsg = _objDalFinishedLeatherTransferTreatment.Save(model, Convert.ToInt32(Session["UserID"]), "FinishedLeatherTransferTreatment/FinishedLeatherTransferTreatment");
            return Json(new { FNTransferID = _objDalFinishedLeatherTransferTreatment.GetFNTransferID(), FNTransferNo = _objDalFinishedLeatherTransferTreatment.GetFNTransferNo(), msg = _vmMsg });
        }

        //public ActionResult DeletedFinishLeatherTransferData(string FNTransferNo, string RecordStatus)
        //{
        //    _vmMsg = _objDalFinishedLeatherTransferTreatment.DeletedFinishLeatherTransferItem(FNTransferNo, RecordStatus);
        //    return Json(new { msg = _vmMsg });
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult DeletedFinishLeatherTransferFromData(string _FNTransferFromID)
        //{
        //    long fnTransferFromID = Convert.ToInt64(_FNTransferFromID);
        //    _vmMsg = _objDalFinishedLeatherTransferTreatment.DeletedFinishLeatherTransferFromItem(fnTransferFromID);
        //    return Json(new { msg = _vmMsg });
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult DeletedFinishLeatherTransferToItem(string _FNTransferToID)
        //{
        //    long fnTransferToID = Convert.ToInt64(_FNTransferToID);
        //    _vmMsg = _objDalFinishedLeatherTransferTreatment.DeletedFinishLeatherTransferToItem(fnTransferToID);
        //    return Json(new { msg = _vmMsg });
        //}

        #endregion

        //public ActionResult FinishLeatherQC(PrdCrustLeatherQC model)
        //{
        //    _vmMsg = model.FNTransferID == 0 ? _objDalFinishedLeatherTransferTreatment.Save(model, Convert.ToInt32(Session["UserID"]), "FinishedLeatherTransferTreatment/FinishedLeatherTransferTreatment") : _objDalFinishedLeatherTransferTreatment.Update(model, Convert.ToInt32(Session["UserID"]));
        //    return Json(new { FNTransferID = _objDalFinishedLeatherTransferTreatment.GetFNTransferID(), FNTransferNo = _objDalFinishedLeatherTransferTreatment.GetFNTransferNo(), msg = _vmMsg });
        //}
        //public ActionResult GetSearchInformation()
        //{
        //    var searchList = _objDalFinishedLeatherTransferTreatment.GetSearchInformation();
        //    return Json(searchList, JsonRequestBehavior.AllowGet);
        //}




        //public ActionResult GetBuyerOrderList(string _BuyerID)
        //{
        //    return Json(objDal.GetBuyerOrderList(_BuyerID), JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult GetChildGridItemListForBuyer(string _FNTransferFromID)
        //{
        //    var childGridItemListForBuyer = _objDalFinishedLeatherTransferTreatment.GetChildGridItemListForBuyer(_FNTransferFromID);
        //    return Json(childGridItemListForBuyer, JsonRequestBehavior.AllowGet);
        //}


        public string RetriveDropdownDataAgainstTransferCategory(string tempFlag)
        {

            int flag = Convert.ToInt32(tempFlag);
            string ddTransferCategory = "";
            if (flag == 1)
            {
                var qcToProductionddList = from temp in repository.StoreRepository.Get(filter: ob => ob.StoreCategory == "Leather" && ob.StoreType == "Own QC") select new { temp.StoreID, temp.StoreName };


                ddTransferCategory = "<option>....SELECT....</option>";
                foreach (var item in qcToProductionddList)
                {
                    ddTransferCategory += "<option value='" + item.StoreID + "'>" + item.StoreName + "</option>";
                }
                return ddTransferCategory;
            }
            else if (flag == 2)
            {
                var qcToStoreddList = from temp in repository.StoreRepository.Get(filter: ob => ob.StoreCategory == "Leather" && ob.StoreType == "Buyer QC") select new { temp.StoreID, temp.StoreName };
                //string ddAQS = "";
                ddTransferCategory = "<option>-- Select One --</option>";
                foreach (var item in qcToStoreddList)
                {
                    ddTransferCategory += "<option value='" + item.StoreID + "'>" + item.StoreName + "</option>";
                }
                return ddTransferCategory;
            }

            return ddTransferCategory;
        }

        public ActionResult GetMasterGridItemList(string _FNTransferFromID)
        {
            var masterGridItemLsitSearch = _objDalFinishedLeatherTransferTreatment.GetMasterGridItemList(_FNTransferFromID);
            return Json(masterGridItemLsitSearch, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetChildGridItemListForMasterGridRow(string _FNTransferFromID)
        {
            var childGridItemList = _objDalFinishedLeatherTransferTreatment.GetChildGridItemListForMasterGridRow(_FNTransferFromID);
            return Json(childGridItemList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllGridList(string _FNTransferID)//CrustLeatherQCID
        {
            InvFNTransfer model = new InvFNTransfer();
            if (string.IsNullOrEmpty(_FNTransferID)) return Json(model, JsonRequestBehavior.AllowGet);
            model.InvFNTransferFromList = _objDalFinishedLeatherTransferTreatment.GetFNTransferFromAfterSave(_FNTransferID);
            if (model.InvFNTransferFromList.Count > 0)
                model.InvFNTransferToList = _objDalFinishedLeatherTransferTreatment.GetFNTransferToAfterSave(model.InvFNTransferFromList[0].FNTransferFromID.ToString());
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        #region Search Start
        //public ActionResult GetFinishedLeatherTransfer()
        //{
        //    var listCrustLeather = (from temp in repository.InvFNTransfer.Get()
        //                            select new
        //                            {
        //                                FNTransferNo = temp.FNTransferNo,
        //                                ssueTo = repository.StoreRepository.Get(filter: ob => ob.StoreID == temp3.StoreID && ob.StoreType == "WetBlue" && ob.StoreCategory == "Leather").FirstOrDefault().StoreName,
        //                                TransactionStore = repository.StoreRepository.Get(filter: ob => ob.StoreID == temp.TransactionStore).FirstOrDefault().StoreName,
        //                                IssueStore = repository.StoreRepository.Get(filter: ob => ob.StoreID == temp.IssueStore).FirstOrDefault().StoreName,
        //                                FNTransferDate = string.Format("{0:dd/MM/yyyy}", temp.FNTransferDate),

        //                            }).AsEnumerable();
        //    return Json(listCrustLeather, JsonRequestBehavior.AllowGet);
        //}
        //public ActionResult SearchFinishedLeatherByFNTransferNo(string search)//InsuranceNo
        //{
        //    var finishLeatherInfo =
        //        from temp in repository.InvFNTransfer.Get()
        //        select new
        //        {
        //            FNTransferNo = temp.FNTransferNo,
        //            TransactionStore = repository.StoreRepository.Get(filter: ob => ob.StoreID == temp.TransactionStore).FirstOrDefault().StoreName,
        //            IssueStore = repository.StoreRepository.Get(filter: ob => ob.StoreID == temp.IssueStore).FirstOrDefault().StoreName,
        //            CLTransferDate = string.Format("{0:dd/MM/yyyy}", temp.FNTransferDate),
        //        };

        //    return Json(finishLeatherInfo, JsonRequestBehavior.AllowGet);
        //}
        //public ActionResult GetAutoCompleteData()
        //{
        //    var data = from temp in repository.InvFNTransfer.Get() select temp.FNTransferNo;
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}
        //public ActionResult GetIssueInfoForSearch(long fnTransferID)
        //{
        //    var searchIssueDetails = _objDalFinishedLeatherTransferTreatment.SearchCrustLeatherTransferDetail(fnTransferID);
        //    return Json(searchIssueDetails, JsonRequestBehavior.AllowGet);
        //}
        #endregion Search END

        #region Buyer POP UP List
        public ActionResult GetFinishQCStockDetail(string TransactionStore)//,InvFNTransfer model
        {
            var finishQcDetail = _objDalFinishedLeatherTransferTreatment.GetQCStockDetail(TransactionStore);//, model
            return Json(finishQcDetail, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFinishTransferFrom(string TransactionStore, string BuyerID, string ArticleID, string BuyerOrderID, string ItemTypeID, string LeatherStatusID)
        {
            var packItemList = _objDalFinishedLeatherTransferTreatment.GetFinishTransferFromGridDetail(TransactionStore, BuyerID, ArticleID, BuyerOrderID, ItemTypeID, LeatherStatusID);
            return Json(packItemList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchArticleNo(string search)
        {
            if (search == null)
            {
                var result = from temp in repository.InvFinishQCStock.Get().AsEnumerable()
                             join temp2 in repository.SysBuyerRepository.Get() on temp.BuyerID equals temp2.BuyerID
                             join temp3 in repository.StoreRepository.Get() on temp.StoreID equals temp3.StoreID
                             select new
                             {
                                 BuyerID = temp.BuyerID,
                                 BuyerName = temp2.BuyerName,
                                 ArticleNo = temp.ArticleNo,
                                 StoreID = temp.StoreID,
                                 StoreName = temp3.StoreName
                             };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                search = search.ToUpper();
                var result = from temp in repository.InvFinishQCStock.Get().AsEnumerable()
                             join temp2 in repository.SysBuyerRepository.Get() on temp.BuyerID equals temp2.BuyerID
                             join temp3 in repository.StoreRepository.Get() on temp.StoreID equals temp3.StoreID
                             where ((temp2.BuyerName.ToUpper().StartsWith(search) || temp2.BuyerName.ToUpper() == search))
                             select new
                             {
                                 BuyerID = temp.BuyerID,
                                 BuyerName = temp2.BuyerName,
                                 ArticleNo = temp.ArticleNo,
                                 StoreID = temp.StoreID,
                                 StoreName = temp3.StoreName
                             };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult GetArticleNoAutocompleteData()
        {
            var search = repository.InvFinishQCStock.Get().Select(ob => ob.ArticleNo);
            return Json(search, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public ActionResult GetAllItemType()
        {
            var allData = from temp in repository.SysItemTypeRepository.Get().AsEnumerable()
                          select new
                          {
                              temp.ItemTypeID,
                              temp.ItemTypeName
                          };


            return Json(allData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllLeatherStatus()
        {
            var allData = from temp in repository.SysLeatherStatusRepo.Get().AsEnumerable()
                          select new
                          {
                              temp.LeatherStatusID,
                              temp.LeatherStatusName
                          };


            return Json(allData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllBuyer()
        {
            var allData = from temp in repository.SysBuyerRepository.Get().AsEnumerable()
                          select new
                          {
                              temp.BuyerID,
                              temp.BuyerName
                          };


            return Json(allData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllBuyerOrder()
        {
            var allData = from temp in repository.SlsBuyerOrederRepository.Get().AsEnumerable()
                          select new
                          {
                              temp.BuyerOrderID,
                              temp.BuyerOrderNo
                          };


            return Json(allData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllColor()
        {
            var allData = from temp in repository.SysColorRepository.Get().AsEnumerable()
                          select new
                          {
                              temp.ColorID,
                              temp.ColorName
                          };


            return Json(allData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllGrade()
        {
            var allData = from temp in repository.SysGrade.Get().AsEnumerable()
                          select new
                          {
                              temp.GradeID,
                              temp.GradeName
                          };


            return Json(allData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllArticle()
        {
            var allData = from temp in repository.ArticleRepository.Get().AsEnumerable()
                          select new
                          {
                              temp.ArticleID,
                              temp.ArticleNo
                          };


            return Json(allData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetArticleChallanNoDD()
        {
            var allData = from temp in repository.ArticleRepository.Get().AsEnumerable()
                          select new
                          {
                              temp.ArticleID,
                              temp.ArticleChallanNo
                          };


            return Json(allData, JsonRequestBehavior.AllowGet);
        }

    }
}