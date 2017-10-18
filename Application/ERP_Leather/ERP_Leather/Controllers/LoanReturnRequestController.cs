using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using ERP_Leather.ActionFilters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ERP_Leather.Controllers
{
    public class LoanReturnRequestController : Controller
    {
        private UnitOfWork repository;
        private ValidationMsg _vmMsg;
        private SysCommonUtility _utility;
        private DalLoanReturnRequest dalLoanOb = new DalLoanReturnRequest();
        public LoanReturnRequestController()
        {
            _vmMsg = new ValidationMsg();
             repository = new UnitOfWork();
             _utility = new SysCommonUtility();
        }
        //
        // GET: /LoanReturnRequest/
          [CheckUserAccess("LoanReturnRequest/LoanReturnIssue")]
        public ActionResult LoanReturnIssue()
        {
            return View();
        }
        public ActionResult LoanReturnReceiveIssue()
        {
            return View();
        }

        public ActionResult LoanRequestData(string type) {
            var resultSet = dalLoanOb.LoanRequestDatas(type);
                
            
            return Json(resultSet.OrderByDescending(ob=>ob.RequestID), JsonRequestBehavior.AllowGet);
        }
        public ActionResult LoanRequestDataWithItem(string type, long RequestID)
        {
            var data = dalLoanOb.LoanRequestDataWithItem(type, RequestID); 
            return Json(data, JsonRequestBehavior.AllowGet); 
           //var resultSet = data; return Json(resultSet, JsonRequestBehavior.AllowGet); 
        }

        public ActionResult ChemicalItemDropdown()
        {
            var resultSet = from temp in repository.SysChemicalItemRepository.Get() select new { temp.ItemName, temp.ItemID };
            return Json(resultSet, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Save(LoanReturnIssue dataSet)
        {
            decimal RemainingQty = 0;
            decimal ReturnQuantity = 0;
            decimal TotalReturnQty = 0;
            try
            {          
                if (dataSet.RequestID == 0)
                {
                    INV_TransRequest ob = new INV_TransRequest();
                    if (dataSet.RequestType == "RLRR")
                    {
                        ob.RequestNo = DalCommon.GetPreDefineNextCodeByUrl("LoanReturnRequest/LoanReturnIssue");
                    }
                    ob.RequestDate = DalCommon.SetDate(dataSet.RequestDate);
                    ob.RequestType = dataSet.RequestType;
                    ob.Remarks = dataSet.Remarks;
                    ob.RequestFrom = dataSet.RequestFrom.ToString();
                    ob.RequestTo = dataSet.RequestTo;
                    ob.ToSource = dataSet.ToSource;
                    ob.ExpectetReturnTime =  Convert.ToByte(dataSet.ExpectetReturnTime);
                    ob.RecordStatus = "NCF";                   
                    ob.SetOn = DateTime.Now;
                    if (dataSet.ReturnMethod == "Exchange Other Item") 
                    {
                        ob.ReturnMethod = "EOI";
                    }
                    else if(dataSet.ReturnMethod == "Doller to Doller"){
                         ob.ReturnMethod = "DTD";
                    }
                     else if(dataSet.ReturnMethod == "Exchange Same Item"){
                         ob.ReturnMethod = "ESI";
                    }
                     else{
                       ob.ReturnMethod = dataSet.ReturnMethod;
                    }
                  
                    ob.SetBy = Convert.ToInt32(Session["UserID"]);
                    ob.ModifiedBy = Convert.ToInt32(Session["UserID"]);
                    ob.ModifiedOn = DateTime.Now;
                    ob.IPAddress = GetIPAddress.LocalIPAddress();
                    repository.InvTransRequestRepository.Insert(ob);

                    INV_TransRequestRef obRef = new INV_TransRequestRef();
                    obRef.TransRequestRefNo = ob.RequestNo;
                    obRef.RequestID = ob.RequestID;
                    obRef.RefRequestID = Convert.ToInt64(dataSet.RefRequestID);
                    obRef.Remark = dataSet.Remarks;
                    obRef.SetOn = DateTime.Now;
                    obRef.SetBy = Convert.ToInt32(Session["UserID"]);
                    obRef.ModifiedBy = Convert.ToInt32(Session["UserID"]);
                    obRef.ModifiedOn = DateTime.Now;
                    obRef.IPAddress = GetIPAddress.LocalIPAddress();
                    repository.InvTransRequestRefRepository.Insert(obRef);

                    if (dataSet.lstLoanReturnIssueItems != null)
                    {
                        foreach (var item in dataSet.lstLoanReturnIssueItems)
                        {
                            TotalReturnQty = (item.RemainingQty * item.ReturnRate);

                            RemainingQty = item.RemainingQty ==null ? 0 : Convert.ToDecimal(item.RemainingQty);
                            ReturnQuantity = item.ReturnQuantity == null ? 0 : Convert.ToDecimal(item.ReturnQuantity);
                            INV_TransRequestItem obReqItem = new INV_TransRequestItem();                           
                            obReqItem.RequestID = ob.RequestID;
                            obReqItem.TransRequestRefID = obRef.TransRequestRefID;

                            
                            obReqItem.RefItemID =Convert.ToInt32(item.ItemID);
                            if (item.ReturnMethodID == "ESI")
                            { 
                                 obReqItem.ItemID = Convert.ToInt32(item.ItemID);
                            }
                     
                            obReqItem.ReferenceQty = Convert.ToDecimal(item.ReferenceQty);
                            obReqItem.ReferenceUnit =  Convert.ToByte(item.ReferenceUnit);
                            if (item.ReceiveCurrencyID != null) 
                            { 
                                obReqItem.ReferenceCurrency = Convert.ToByte(item.ReceiveCurrencyID);
                            }
                            obReqItem.ReferenceRate = Convert.ToDecimal(item.ReceiveRate);
                            obReqItem.ReferenceValue= Convert.ToDecimal(item.ReceiveValue);
                            obReqItem.ReturnMethod = item.ReturnMethodID == null ? "" : item.ReturnMethodID;
                            if (item.ReturnUnitID != null)
                            {
                                obReqItem.ReturnUnit = Convert.ToByte(item.ReturnUnitID);
                            }
                            if (item.ReturnItemID != null)
                            {
                                obReqItem.ItemID = Convert.ToInt32(item.ReturnItemID);
                            }
                            if (item.ReturnCurrencyID != null)
                            { 
                                obReqItem.ReturnCurrency = Convert.ToByte(item.ReturnCurrencyID);
                            }
                            if (item.ReturnExchangeRate !=null)
                            {
                                obReqItem.ExchangeRate = Convert.ToDecimal(item.ReturnExchangeRate);
                            }
                            if(item.ReturnRate !=null)
                            { 
                                obReqItem.ReturnRate = Convert.ToDecimal(item.ReturnRate);
                            }
                            obReqItem.RefItemDueQty = 0;//Math.Round(item.RemainingQty - ((item.ReturnQuantity * item.RemainingQty) / (TotalReturnQty == 0 ? 1 : TotalReturnQty)),2);
                            obReqItem.ReturnValue = Convert.ToDecimal(item.ReturnQuantity);//Math.Round(((item.ReturnQuantity * item.RemainingQty) / (TotalReturnQty == 0 ? 1 : TotalReturnQty)),2);
                            obReqItem.SetBy = Convert.ToInt32(Session["UserID"]);
                            obReqItem.SetOn = DateTime.Now;
                            obReqItem.ModifiedBy = Convert.ToInt32(Session["UserID"]);
                            obReqItem.ModifiedOn = DateTime.Now;
                            obReqItem.IPAddress = GetIPAddress.LocalIPAddress();                          
                            repository.InvTransRequestItemRepository.Insert(obReqItem);
                        }
                    }
                    try
                    {
                        repository.Save();
                        _vmMsg.ReturnCode = ob.RequestNo;
                        _vmMsg.ReturnId = ob.RequestID;
                        _vmMsg.Type = Enums.MessageType.Success;
                        _vmMsg.Msg = "Saved Successfully.";
                    }
                    catch (Exception ex)
                    {
                        _vmMsg.Type = Enums.MessageType.Error;
                        _vmMsg.Msg = "Saved Faild.";
                    }
                }
                else
                {  // Update
                    INV_TransRequest ob = repository.InvTransRequestRepository.GetByID(dataSet.RequestID);
                    ob.RequestDate = DalCommon.SetDate(dataSet.RequestDate);
                    if (dataSet.ReturnMethod == "Exchange Other Item")
                    {
                        ob.ReturnMethod = "EOI";
                    }
                    else if (dataSet.ReturnMethod == "Doller to Doller")
                    {
                        ob.ReturnMethod = "DTD";
                    }
                    else if (dataSet.ReturnMethod == "Exchange Same Item")
                    {
                        ob.ReturnMethod = "ESI";
                    }
                    else
                    {
                        ob.ReturnMethod = dataSet.ReturnMethod;
                    }
                    ob.RequestType = dataSet.RequestType;
                    ob.RequestFrom = dataSet.RequestFrom.ToString();
                    ob.RequestTo = dataSet.RequestTo;
                    ob.ToSource = dataSet.ToSource;
                    ob.ExpectetReturnTime = Convert.ToByte(dataSet.ExpectetReturnTime);
                    ob.Remarks = dataSet.Remarks;
                    ob.ModifiedOn = DateTime.Now;
                    ob.ModifiedBy = Convert.ToInt32(Session["UserID"]);
                    ob.IPAddress = GetIPAddress.LocalIPAddress();
                    repository.InvTransRequestRepository.Update(ob);

                    INV_TransRequestRef obRef = repository.InvTransRequestRefRepository.Get(filter: o => o.RequestID == dataSet.RequestID).FirstOrDefault();               
                    obRef.Remark = dataSet.Remarks;
                    obRef.ModifiedBy = Convert.ToInt32(Session["UserID"]);
                    obRef.ModifiedOn = DateTime.Now;
                    obRef.IPAddress = GetIPAddress.LocalIPAddress();
                    repository.InvTransRequestRefRepository.Update(obRef);
                    if (dataSet.lstLoanReturnIssueItems != null)
                    {
                        foreach (var item in dataSet.lstLoanReturnIssueItems)
                        {
                            TotalReturnQty = (item.RemainingQty * item.ReturnRate);
                            RemainingQty = item.RemainingQty == null ? 0 : Convert.ToDecimal(item.RemainingQty);
                            ReturnQuantity = item.ReturnQuantity == null ? 0 : Convert.ToDecimal(item.ReturnQuantity);

                                INV_TransRequestItem obReqItem = repository.InvTransRequestItemRepository.GetByID(item.TransRequestItemID);
                                
                                //obReqItem.TransRequestRefID = obRef.TransRequestRefID;
                                obReqItem.RefItemID = Convert.ToInt32(item.ItemID);
                                obReqItem.ReferenceQty = Convert.ToDecimal(item.ReferenceQty);
                                obReqItem.ReferenceUnit = Convert.ToByte(item.ReferenceUnit);
                                if (item.ReceiveCurrencyID != null) { 
                                obReqItem.ReferenceCurrency = Convert.ToByte(item.ReceiveCurrencyID);
                                }
                                if (item.ReturnMethodID == "ESI")
                                {
                                    obReqItem.ItemID = Convert.ToInt32(item.ItemID);
                                }
                     
                                obReqItem.ReferenceRate = Convert.ToDecimal(item.ReceiveRate);
                                obReqItem.ReferenceValue = Convert.ToDecimal(item.ReceiveValue);
                                obReqItem.ReturnMethod = item.ReturnMethodID == null ? "" : item.ReturnMethodID;
                                if (item.ReturnUnitID != null)
                                {
                                    obReqItem.ReturnUnit = Convert.ToByte(item.ReturnUnitID);
                                }
                                if (item.ReturnItemID != null)
                                {
                                    obReqItem.ItemID = Convert.ToInt32(item.ReturnItemID);
                                }
                                if (item.ReturnCurrencyID != null)
                                {
                                    obReqItem.ReturnCurrency = Convert.ToByte(item.ReturnCurrencyID);
                                }
                                if (item.ReturnExchangeRate != null)
                                {
                                    obReqItem.ExchangeRate = Convert.ToDecimal(item.ReturnExchangeRate);
                                }
                                if (item.ReturnRate != null)
                                {
                                    obReqItem.ReturnRate = Convert.ToDecimal(item.ReturnRate);
                                }
                                obReqItem.RefItemDueQty = 0;//Math.Round((item.RemainingQty + (obReqItem.ReturnValue == null?0 : Convert.ToDecimal(obReqItem.ReturnValue))) - ((item.ReturnQuantity * item.RemainingQty) / (TotalReturnQty == 0 ? 1 : TotalReturnQty)), 2);
                                obReqItem.ReturnValue = Convert.ToDecimal(item.ReturnQuantity);//Math.Round(((item.ReturnQuantity * item.RemainingQty) / (TotalReturnQty == 0 ? 1 : TotalReturnQty)), 2);
                                obReqItem.SetBy = Convert.ToInt32(Session["UserID"]);
                                obReqItem.SetOn = DateTime.Now;
                                obReqItem.ModifiedBy = Convert.ToInt32(Session["UserID"]);
                                obReqItem.ModifiedOn = DateTime.Now;
                                obReqItem.IPAddress = GetIPAddress.LocalIPAddress();        
                                repository.InvTransRequestItemRepository.Update(obReqItem);
                        }
                    }

                    try
                    {
                         repository.Save();
                        _vmMsg.ReturnCode = ob.RequestNo;
                        _vmMsg.ReturnId = dataSet.RequestID;
                        _vmMsg.Type = Enums.MessageType.Success;
                        _vmMsg.Msg = "Updated Successfully.";
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
                   
                        _vmMsg.Type = Enums.MessageType.Error;
                        _vmMsg.Msg = "Saved Faild.";
                        throw new DbEntityValidationException(sb.ToString(), e);
                    }
                }
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

                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Saved Faild.";
                throw new DbEntityValidationException(sb.ToString(), e);
            }
            return Json(new { msg = _vmMsg });
        }

        public ActionResult GetLoanReturnRequestInfo()
        {
            var data = from t in dalLoanOb.GetLoanReturnRequestInfo() select new {
                RequestID = t.RequestID,
                RequestNo = t.RequestNo,
                RequestFrom = t.RequestFrom,
                RequestFromName = t.RequestFromName,
                RequestTo =t.RequestTo,
                RequestToName=t.RequestToName,
                RequestDate = t.RequestDate,
                ToSource = t.ToSource == null ? "" : t.ToSource,
                RecordStatus =t.RecordStatus,
                ReturnMethod =t.ReturnMethod
            };
           return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchLoanReturnRequest(string search)
        {
            var data = dalLoanOb.SearchLoanReturnRequest(search);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAutoCompleteData()
        {
            var search = repository.InvTransRequestRepository.Get(filter: o => o.RequestType == "RLRR", orderBy: q => q.OrderByDescending(d => d.RequestDate)).Select(ob => ob.RequestNo);
            return Json(search, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetLoanReturnRequestInfoByID(long RequestID)
        {
           var data= dalLoanOb.GetLoanReturnRequestInfoByID(RequestID);
           return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetLoanReturnRequestItemInfoByID(long RequestID)
        {
            var data = dalLoanOb.GetLoanReturnRequestItemInfoByID2(RequestID);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LoanReturnRequestStatusUpdate(string type, long requestID, string note)
        { 
            if (type.Equals("Checker"))
            {
                INV_TransRequest item = repository.InvTransRequestRepository.GetByID(requestID);
                item.CheckedBy = Convert.ToInt32(Session["UserID"]);
                item.CheckComments = note;
                item.CheckDate = DateTime.Now;
                item.RecordStatus = "CHK";
                repository.InvTransRequestRepository.Update(item);
                repository.Save();
                _vmMsg.Type = Enums.MessageType.Success;
                _vmMsg.Msg = "Checked Successfully.";
            
            }
            else if (type.Equals("Recommend"))
            {
                INV_TransRequest item = repository.InvTransRequestRepository.GetByID(requestID);
                item.RecommendBy = Convert.ToInt32(Session["UserID"]);
                item.RecommendComments = note;
                item.RecommendDate = DateTime.Now;
                item.RecordStatus = "REC";
                repository.InvTransRequestRepository.Update(item);
                repository.Save();
                _vmMsg.Type = Enums.MessageType.Success;
                _vmMsg.Msg = "Recommended Successfully.";
            }
            else if (type.Equals("Approver"))
            {
                INV_TransRequest item = repository.InvTransRequestRepository.GetByID(requestID);
                item.RecommendBy = Convert.ToInt32(Session["UserID"]);
                item.RecommendComments = note;
                item.RecommendDate = DateTime.Now;
                item.RecordStatus = "CNF";
                repository.InvTransRequestRepository.Update(item);
                repository.Save();
                _vmMsg.Type = Enums.MessageType.Success;
                _vmMsg.Msg = "Recommended Successfully.";
            }
            else
            { }
            return Json(new { msg = _vmMsg });
        }
	}
}