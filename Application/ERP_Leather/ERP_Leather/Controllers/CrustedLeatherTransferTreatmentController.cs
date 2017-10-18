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
    public class CrustedLeatherTransferTreatmentController : Controller
    {
        public DalSysStore _objDalStore = new DalSysStore();
        public DalCrustedLeatherTransferTreatment _objDalCrustedLeatherTransferTreatment = new DalCrustedLeatherTransferTreatment();
        private UnitOfWork repository;
        private ValidationMsg _vmMsg;
        private SysCommonUtility _utility;
        private int _userId;

        public CrustedLeatherTransferTreatmentController()
        {
            _vmMsg = new ValidationMsg();
            repository = new UnitOfWork();
            _utility = new SysCommonUtility();
        }

        [CheckUserAccess("CrustedLeatherTransferTreatment/CrustedLeatherTransferTreatment")]
        public ActionResult CrustedLeatherTransferTreatment()
        {
            ViewBag.IssueFromStoreList = _objDalStore.GetAllActiveCrustQCStore();
            return View();
        }

        #region Cascading DropDown for Transfer Category
        public string RetriveDropdownDataAgainstTransferCategory(string tempFlag)
        {

            int flag = Convert.ToInt32(tempFlag);
            string ddTransferCategory = "";
            if (flag == 1)
            {
                var qcToProductionddList = from temp in repository.StoreRepository.Get(filter: ob => ob.StoreCategory == "Leather" && ob.StoreType == "Crust") select new { temp.StoreID, temp.StoreName };


                ddTransferCategory = "<option>....SELECT....</option>";
                foreach (var item in qcToProductionddList)
                {
                    ddTransferCategory += "<option value='" + item.StoreID + "'>" + item.StoreName + "</option>";
                }
                return ddTransferCategory;
            }
            else if (flag == 2)
            {
                var qcToStoreddList = from temp in repository.StoreRepository.Get(filter: ob => ob.StoreCategory == "Leather" && ob.StoreType == "Crust QC" || ob.StoreType == "Crust Fail") select new { temp.StoreID, temp.StoreName };
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

        #endregion

        #region  Retrive Challan Data for Dropdown in Grid
        public ActionResult GetArticleChallanNoDD()
        {
            var allData = from temp in repository.SysArticleChallanRepository.Get().AsEnumerable()
                          join temp2 in repository.Sys_ArticleChallanColorRepo.Get() on temp.ArticleChallanID equals temp2.ArticleChallanID
                          select new
                          {
                              ArticleChallanID = temp.ArticleChallanID,
                              ArticleChallanNo = temp.ArticleChallanNo,
                              ArticleColorNo = temp2.ArticleColorNo
                          };


            return Json(allData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Party POP UP List for 1st/Master Grid
        public ActionResult GetCrustTransferItemGrid(string TransactionStore)
        {
            var packItemList = _objDalCrustedLeatherTransferTreatment.GetCrustTransferPopUpGridDetail(TransactionStore);
            return Json(packItemList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCrustDataItemAndGradeGrid(string TransactionStore, string BuyerID, string BuyerOrderID, string ArticleID, string ItemTypeID, string LeatherStatusID, string ColorID, string ArticleChallanID)
        {
            clTransfer model = new clTransfer();
            model.TransferFromList = _objDalCrustedLeatherTransferTreatment.GetCrustTransferMasterGridDetail(TransactionStore, BuyerID, BuyerOrderID, ArticleID, ItemTypeID, LeatherStatusID, ColorID, ArticleChallanID);
            if (model.TransferFromList.Count > 0)
                model.TransferToList = _objDalCrustedLeatherTransferTreatment.GetCrustTransferChildGridDetail(TransactionStore, BuyerID, BuyerOrderID, ArticleID, ItemTypeID, LeatherStatusID, ColorID, ArticleChallanID);
            return Json(model, JsonRequestBehavior.AllowGet);

        }
  
        #endregion

        #region 2nd/Child Grid Popup
        public ActionResult GetAllItemType()
        {
            var allData = _objDalCrustedLeatherTransferTreatment.GetAllItemTypeGridData();
            return Json(allData, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetAllLeatherStatus()
        {
            var allData = _objDalCrustedLeatherTransferTreatment.GetAllLeatherStatusGridData();
            return Json(allData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllBuyer()
        {
            var allData = _objDalCrustedLeatherTransferTreatment.GetAllBuyerGridData();
            return Json(allData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllBuyerOrder()
        {

            var allData = _objDalCrustedLeatherTransferTreatment.GetAllBuyerOrderGridData();
            return Json(allData, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetAllColor()
        {

            var allData = _objDalCrustedLeatherTransferTreatment.GetAllColorGridData();
            return Json(allData, JsonRequestBehavior.AllowGet);

        }

        //public ActionResult GetAllGrade()
        //{

        //    var allData = _objDalCrustedLeatherTransferTreatment.GetAllGradeGridData();
        //    return Json(allData, JsonRequestBehavior.AllowGet);

        //}

        public ActionResult GetAllArticle()
        {

            var allData = _objDalCrustedLeatherTransferTreatment.GetAllArticleGridData();
            return Json(allData, JsonRequestBehavior.AllowGet);

        }

        #endregion

        #region Crust Leather Transfer treatment  SAVE & UPDATE Code 
        [HttpPost]
        public ActionResult CrustedLeatherTransferTreatment(clTransfer model)
        {
            _vmMsg = model.CLTransferID == 0 ? _objDalCrustedLeatherTransferTreatment.Save(model, Convert.ToInt32(Session["UserID"]), "CrustedLeatherTransferTreatment/CrustedLeatherTransferTreatment") : _objDalCrustedLeatherTransferTreatment.Update(model, Convert.ToInt32(Session["UserID"]));
            return Json(new { CLTransferID = _objDalCrustedLeatherTransferTreatment.GetCLTransferID(), CLTransferFromID = _objDalCrustedLeatherTransferTreatment.GetCLTransferFromID(), CLTransferToID = _objDalCrustedLeatherTransferTreatment.GetCLTransferToID(), CLTransferNo = _objDalCrustedLeatherTransferTreatment.GetCLTransferNo(), msg = _vmMsg });
        }

        #endregion

        #region  SEARCH Code of Crust Leather Transfer Treatment
        public ActionResult GetCrustLeatherQCInfo()
        {
            var data = _objDalCrustedLeatherTransferTreatment.GetclTransferInfo();

            var searchIssueDetails = from t in data
                                     select new
                                     {
                                         CLTransferID = t.CLTransferID,
                                         CLTransferNo = t.CLTransferNo,
                                         CLTransferCategory = t.CLTransferCategory,
                                         CLTransferCategoryName = t.CLTransferCategory == "AQP" ? "After QC To Production" : "After QC to Store",
                                         TranrsferType = t.TranrsferType,
                                         TranrsferTypeName = t.TranrsferType == "TAO" ? "Transfer Against Order" : "Transfer Without Order",
                                         TransactionStore = t.TransactionStore,
                                         CLTransferDate = t.CLTransferDate,
                                         IssueStore = t.IssueStore,
                                         TransactionStoreName = t.TransactionStoreName,
                                         IssueStoreName = t.IssueStoreName,
                                         RecordStatus = DalCommon.ReturnRecordStatus(t.RecordStatus),
                                     };
            return Json(searchIssueDetails, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetAllGridDetailsList(string CLTransferID)
        {
            clTransfer model = new clTransfer();
            if (string.IsNullOrEmpty(CLTransferID)) return Json(model, JsonRequestBehavior.AllowGet);
            model.TransferFromList = _objDalCrustedLeatherTransferTreatment.GetCLTransferFromList(CLTransferID);
            if (model.TransferFromList.Count > 0)
            {
                model.TransferToList = _objDalCrustedLeatherTransferTreatment.GetCLTransferToList(model.TransferFromList[0].CLTransferFromID.ToString());
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Delete All data from Crust Leather Transfer Treatment 

        public ActionResult DeleteSelectionData(int id)
        {
            INV_CLTransfer issue = repository.InvCLTransfer.GetByID(id);

            if (issue != null)
            {
                var data = (from t in repository.InvCLTransferFrom.Get() where t.CLTransferID == issue.CLTransferID select t);
                foreach (var item in data)
                {
                    INV_CLTransferFrom issueItem = (from temp in data where temp.CLTransferFromID == item.CLTransferFromID select temp).FirstOrDefault();
                    var grade = repository.InvCLTransferTo.Get(ob => ob.CLTransferFromID == issueItem.CLTransferFromID);
                    foreach (var item3 in grade)
                    {
                        INV_CLTransferTo gr = (from t in grade where t.CLTransferToID == item3.CLTransferToID select t).FirstOrDefault();
                        repository.InvCLTransferTo.Delete(gr);
                    }
                    repository.InvCLTransferFrom.Delete(issueItem);
                }
                repository.InvCLTransfer.Delete(issue);

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
        #endregion

        public ActionResult DeleteMasterItemGridInfo(long CLTransferFromID)
        {
            var data = repository.InvCLTransferFrom.GetByID(Convert.ToInt64(CLTransferFromID));
            if (data != null)
            {
                var chk = repository.InvCLTransferFrom.GetByID(data.CLTransferFromID);
                var delStatus = repository.InvCLTransfer.GetByID(chk.CLTransferID);
                if (delStatus.RecordStatus != "CNF")
                {
                    repository.InvCLTransferFrom.Delete(data);
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

        public ActionResult DeleteChildItemGridInfo(long CLTransferToID)
        {
            var data = repository.InvCLTransferTo.GetByID(Convert.ToInt64(CLTransferToID));
            if (data != null)
            {
                var chk = repository.InvCLTransferFrom.GetByID(data.CLTransferFromID);
                var delStatus = repository.InvCLTransfer.GetByID(chk.CLTransferID);
                if (delStatus.RecordStatus != "CNF")
                {
                    repository.InvCLTransferTo.Delete(data);
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


        #region Confirm Crust leather Transfer Treatment Stock
        [HttpPost]
        public ActionResult ConfirmCrustLeatherTransferTreatment(long CLTransferID, int userid)
        {
            _vmMsg = _objDalCrustedLeatherTransferTreatment.ConfirmCrustleatherTransferTreatmentStock(CLTransferID, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }
        #endregion



    }
}


