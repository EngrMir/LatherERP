using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using ERP_Leather.ActionFilters;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ERP_Leather.Controllers
{
    public class LoanRequestController : Controller
    {
        private UnitOfWork repository;
        private ValidationMsg _vmMsg;
        private SysCommonUtility _utility;
        private DalSysChemicalItem obsysChemical;
        public LoanRequestController()
        {
            _vmMsg = new ValidationMsg();
             repository = new UnitOfWork();
             _utility = new SysCommonUtility();
        }
        [CheckUserAccess("LoanRequest/Index")]
        public ActionResult Index()
        {
            return View();
        }
        [CheckUserAccess("LoanRequest/LoanIssueRequest")]
        public ActionResult LoanIssueRequest()
        {
            return View();
        }

        public ActionResult GetStoreInfo(string storeCategory)
        {
            if (storeCategory == "")
            {
                var lst = from temp in repository.StoreRepository.Get(filter: ob => ob.IsActive == true && ob.IsDelete==false ) 
                      select new { temp.StoreID, temp.StoreName, temp.StoreCode };
                return Json(lst, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var lst = from temp in repository.StoreRepository.Get(filter: ob => ob.IsActive == true && ob.IsDelete == false && ob.StoreCategory == storeCategory && ob.StoreType == "Chemical")
                          select new { temp.StoreID, temp.StoreName, temp.StoreCode };
                return Json(lst, JsonRequestBehavior.AllowGet);
            }
           
        }
        //public ActionResult GetChemicalStore(string storeCategory)
        //{
        //    var lst = from temp in repository.StoreRepository.Get(filter: ob => ob.IsActive == true && ob.IsDelete == false && ob.StoreCategory == "Chemical" && ob.StoreType == "Chemical")
        //                  select new { temp.StoreID, temp.StoreName, temp.StoreCode };
        //        return Json(lst, JsonRequestBehavior.AllowGet);
         
        //}
        public ActionResult SearchStoreByStore(string search)
        {
            search = search.ToUpper();
            var result = from temp in repository.StoreRepository.Get()
                         where (temp.StoreName.ToUpper().StartsWith(search) || temp.StoreName.ToUpper() == search)
                         select new
                         {
                             temp.StoreID,
                             temp.StoreName,
                             temp.StoreCode
                         };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchChemicalStoreByStore(string search)
        {
            search = search.ToUpper();
            var result = from temp in repository.StoreRepository.Get(filter: ob => ob.IsActive == true && ob.IsDelete == false && ob.StoreCategory == "Chemical" && ob.StoreType == "Chemical")
                         where (temp.StoreName.ToUpper().StartsWith(search) || temp.StoreName.ToUpper() == search)
                         select new
                         {
                             temp.StoreID,
                             temp.StoreName,
                             temp.StoreCode
                         };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStoreSearch()
        {
            var search = repository.StoreRepository.Get(filter: ob => ob.IsActive == true, orderBy: q => q.OrderByDescending(d => d.StoreID)).Select(ob => ob.StoreName);
            return Json(search, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetChemicalStoreSearch()
        {
            var search = repository.StoreRepository.Get(filter: ob => ob.IsActive == true && ob.IsDelete == false && ob.StoreCategory == "Chemical" && ob.StoreType == "Chemical", orderBy: q => q.OrderByDescending(d => d.StoreID)).Select(ob => ob.StoreName);
            return Json(search, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSupplierList()
        {
            var result = from temp in repository.SysSupplierRepository.Get(filter: ob => ob.IsActive == true) select new { temp.SupplierID,temp.SupplierCode,temp.SupplierName,temp.SupplierCategory};
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public object GetChemicalList()
        {
            var result = from temp in repository.SysChemicalItemRepository.Get(filter:o=>o.IsActive == true)
                         select new
                         {
                             temp.ItemID,
                             temp.ItemName,
                             temp.HSCode,
                             temp.ItemCategory
                         }; 
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public object SearchChemicalByName(string search)
        {
            var lcInfo = from temp in repository.SysChemicalItemRepository.Get().Where(ob => (ob.ItemName.StartsWith(search) || ob.ItemName == search))
                         select new
                         {
                             temp.ItemID,
                             temp.ItemName,
                             temp.HSCode,
                             temp.ItemCategory
                         }; 
            return Json(lcInfo, JsonRequestBehavior.AllowGet);

        }

        public object GetLCNoSearch()
        {
            var data = repository.SysChemicalItemRepository.Get().Select(ob => ob.ItemName);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Save(TransRequest dataSet)
        {
            try
            {
                //######################################
               // Session["UserID"] = 1;
                if (dataSet.RequestID == 0)
                {
                    INV_TransRequest ob = new INV_TransRequest();
                    if (dataSet.RequestType == "LNIR")
                    {
                        ob.RequestNo = DalCommon.GetPreDefineNextCodeByUrl("LoanRequest/LoanIssueRequest");
                        ob.FromSource = dataSet.ToSource;
                        //ob.ToSource = "STR";
                    }
                    else if (dataSet.RequestType == "LNRR")
                    {
                        ob.RequestNo = DalCommon.GetPreDefineNextCodeByUrl("LoanRequest/Index");
                        //ob.FromSource = "STR";
                        ob.ToSource = dataSet.ToSource;
                    }
                    else
                    { 
                    
                    }
                    
                    ob.RequestDate = DalCommon.SetDate(dataSet.RequestDate);
                    ob.RequestType = dataSet.RequestType;
                    ob.RequestFrom = dataSet.RequestFrom.ToString();
                    ob.RequestTo = dataSet.RequestTo.ToString();
                    ob.ReturnMethod = dataSet.ReturnMethod;
                    ob.ExpectetReturnTime = Convert.ToByte(dataSet.ExpectetReturnTime);
                    ob.RecordStatus = "NCF";
                    ob.SetOn = DateTime.Now;
                    ob.SetBy = Convert.ToInt32(Session["UserID"]);
                    ob.ModifiedBy = Convert.ToInt32(Session["UserID"]);
                    ob.ModifiedOn = DateTime.Now;
                  
                    ob.IPAddress = GetIPAddress.LocalIPAddress();
                    repository.InvTransRequestRepository.Insert(ob);

                    INV_TransRequestRef obRef = new INV_TransRequestRef();
                    obRef.TransRequestRefNo = ob.RequestNo;
                    obRef.RequestID = ob.RequestID;
                    //obRef.RefRequestID = ob.RequestID;
                    obRef.ReturnMethod = dataSet.ReturnMethod;
                    obRef.SetOn = DateTime.Now;
                    obRef.SetBy = Convert.ToInt32(Session["UserID"]);
                    obRef.ModifiedBy = Convert.ToInt32(Session["UserID"]);
                    obRef.ModifiedOn = DateTime.Now;
                    obRef.IPAddress = GetIPAddress.LocalIPAddress();
                    repository.InvTransRequestRefRepository.Insert(obRef);

                    if (dataSet.ChemicalSelectedList != null)
                    {
                        foreach (var item in dataSet.ChemicalSelectedList)
                        {
                            INV_TransRequestItem obReqItem = new INV_TransRequestItem();
                            obReqItem.RequestID = ob.RequestID;
                            obReqItem.ReturnMethod = dataSet.ReturnMethod;
                            obReqItem.TransRequestRefID = obRef.TransRequestRefID;
                            if (item.ItemID > 0) { obReqItem.ItemID = Convert.ToInt32(item.ItemID); }
                            if (Convert.ToByte(item.PackSize) > 0) { obReqItem.PackSize = Convert.ToByte(item.PackSize); }
                           
                            obReqItem.PackQty = Convert.ToInt32(item.PackQty);
                            if (Convert.ToByte(item.SizeUnit) > 0) {
                                obReqItem.SizeUnit = Convert.ToByte(item.SizeUnit);
                            }
                            if (obReqItem.PackQty != null && item.PackSize != null) 
                            { 
                                 obReqItem.TransQty = obReqItem.PackQty * ((Convert.ToByte(repository.SysSizeRepository.GetByID(Convert.ToInt32(item.PackSize)).SizeName)));
                            }
                            if (Convert.ToByte(item.ReferenceUnit) > 0) { obReqItem.TransUnit = Convert.ToByte(item.ReferenceUnit); }
                           
                            if (item.RefSupplierID > 0) { obReqItem.RefSupplierID = Convert.ToInt32(item.RefSupplierID); }
                          
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
                        _vmMsg.ReturnId = repository.InvTransRequestRepository.Get().Last().RequestID;
                        _vmMsg.Type = Enums.MessageType.Success;
                        _vmMsg.Msg = "Saved Successfully.";
                    }
                    catch (Exception ex)
                    {
                        _vmMsg.Type = Enums.MessageType.Error;
                        _vmMsg.Msg = "Saved Faild.";
                    }

                }
                else {  // Update
                        INV_TransRequest ob = repository.InvTransRequestRepository.GetByID(dataSet.RequestID);
                        ob.RequestDate = DalCommon.SetDate(dataSet.RequestDate);                   
                        ob.RequestFrom = dataSet.RequestFrom.ToString();
                        ob.RequestTo = dataSet.RequestTo.ToString();
                        ob.ReturnMethod = dataSet.ReturnMethod;
                        ob.ExpectetReturnTime = Convert.ToByte(dataSet.ExpectetReturnTime);                   
                        ob.ModifiedOn = DateTime.Now;
                        ob.ModifiedBy = Convert.ToInt32(Session["UserID"]);
                        ob.FromSource = "STR";
                        if (ob.RequestType == "LNIR")
                        {
                            ob.FromSource = dataSet.ToSource;                     
                        }
                        else if (ob.RequestType == "LNRR")
                        {
                            ob.ToSource = dataSet.ToSource;
                        }
                        else
                        {

                        }
                        //ob.ToSource = dataSet.ToSource;
                        ob.IPAddress = GetIPAddress.LocalIPAddress();
                        repository.InvTransRequestRepository.Update(ob);

                        INV_TransRequestRef obRef = repository.InvTransRequestRefRepository.Get(filter: o=>o.RequestID == dataSet.RequestID).FirstOrDefault();
                        obRef.ReturnMethod = dataSet.ReturnMethod;                      
                        obRef.ModifiedBy = Convert.ToInt32(Session["UserID"]);
                        obRef.ModifiedOn = DateTime.Now;
                        obRef.IPAddress = GetIPAddress.LocalIPAddress();
                        repository.InvTransRequestRefRepository.Update(obRef);
                        if (dataSet.ChemicalSelectedList != null)
                        {
                            foreach (var item in dataSet.ChemicalSelectedList)
                            {
                                if (item.TransRequestItemID == 0)
                                {
                                    INV_TransRequestItem obReqItem = new INV_TransRequestItem();
                                    obReqItem.RequestID = ob.RequestID;
                                    obReqItem.TransRequestRefID = obRef.TransRequestRefID;
                                    obReqItem.ReturnMethod = dataSet.ReturnMethod; 
                                    if (Convert.ToInt32(item.ItemID) > 0) { obReqItem.ItemID = Convert.ToInt32(item.ItemID); }
                                    if (Convert.ToByte(item.PackSize) > 0) { obReqItem.PackSize = Convert.ToByte(item.PackSize); }
                                    else { obReqItem.PackSize = 0; }                                   
                                    obReqItem.PackQty = Convert.ToInt32(item.PackQty);
                                    if (Convert.ToByte(item.SizeUnit) > 0) { obReqItem.SizeUnit = Convert.ToByte(item.SizeUnit); }

                                    if (obReqItem.PackQty >0 && item.PackSize != null)
                                    { 
                                    obReqItem.TransQty = obReqItem.PackQty * ((Convert.ToByte(repository.SysSizeRepository.GetByID(Convert.ToInt32(item.PackSize)).SizeName)));//Convert.ToDecimal(item.ReferenceQty);
                                    }
                                    else
                                    {
                                        obReqItem.TransQty = Convert.ToDecimal(item.ReferenceQty);
                                    }

                                    if (Convert.ToByte(item.ReferenceUnit) > 0) { obReqItem.TransUnit = Convert.ToByte(item.ReferenceUnit); }
                                    if (Convert.ToInt32(item.RefSupplierID) > 0)
                                    {
                                        obReqItem.RefSupplierID = Convert.ToInt32(item.RefSupplierID);
                                    }     
                                    obReqItem.SetBy = Convert.ToInt32(Session["UserID"]);
                                    obReqItem.SetOn = DateTime.Now;
                                    obReqItem.ModifiedBy = Convert.ToInt32(Session["UserID"]);
                                    obReqItem.ModifiedOn = DateTime.Now;
                                    obReqItem.IPAddress = GetIPAddress.LocalIPAddress();
                                    repository.InvTransRequestItemRepository.Insert(obReqItem);
                                }
                                else
                                {
                                    INV_TransRequestItem obReqItem = repository.InvTransRequestItemRepository.GetByID(item.TransRequestItemID);
                                    if ( Convert.ToInt32(item.ItemID) > 0) { obReqItem.ItemID = Convert.ToInt32(item.ItemID); }
                                    if (Convert.ToByte(item.PackSize) > 0) { obReqItem.PackSize = Convert.ToByte(item.PackSize); }
                                    else { obReqItem.PackSize = 0; }
                                    obReqItem.PackQty = Convert.ToInt32(item.PackQty);
                                    obReqItem.ReturnMethod = dataSet.ReturnMethod; 
                                
                                    if (Convert.ToByte(item.SizeUnit) > 0) { obReqItem.SizeUnit = Convert.ToByte(item.SizeUnit); }
                                    if (obReqItem.PackQty >0 && item.PackSize !=null)
                                    {
                                        obReqItem.TransQty = obReqItem.PackQty * (Convert.ToByte(repository.SysSizeRepository.GetByID(Convert.ToInt32(item.PackSize)).SizeName));//Convert.ToDecimal(item.ReferenceQty);
                                    }
                                    else
                                    {
                                        obReqItem.TransQty = Convert.ToDecimal(item.ReferenceQty);
                                    }
                                    if (Convert.ToByte(item.ReferenceUnit) > 0) { obReqItem.TransUnit = Convert.ToByte(item.ReferenceUnit); }
                                   
                                    if (Convert.ToInt32(item.RefSupplierID) > 0) 
                                    { 
                                        obReqItem.RefSupplierID = Convert.ToInt32(item.RefSupplierID); 
                                    }                             
                                    obReqItem.ModifiedBy = Convert.ToInt32(Session["UserID"]);
                                    obReqItem.ModifiedOn = DateTime.Now;
                                    obReqItem.IPAddress = GetIPAddress.LocalIPAddress();
                                    repository.InvTransRequestItemRepository.Update(obReqItem);
                                }
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
                        catch (Exception x)
                        {
                            _vmMsg.Type = Enums.MessageType.Error;
                            _vmMsg.Msg = "Saved Faild.";
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
                throw new DbEntityValidationException(sb.ToString(), e);
            }
            return Json(new { msg = _vmMsg });
        }


        public ActionResult GetLoanReceiveRequestList()
        {
            var result = from temp in repository.InvTransRequestRepository.Get(filter:o=>o.RequestType=="LNRR")                         
                         select new { temp.RequestID,temp.RequestNo,temp.RequestFrom,temp.RequestTo, temp.ReturnMethod,temp.RequestDate,temp.RecordStatus,temp.FromSource,temp.ToSource };
            var sup = repository.SysSupplierRepository.Get(filter: ob => ob.SupplierCategory == "Chemical" && ob.IsActive == true && ob.IsDelete == false).Select(ob => new { ob.SupplierName, ob.SupplierID });
            var str = repository.StoreRepository.Get(filter: o => o.IsActive == true && o.IsDelete == false && o.StoreCategory == "Chemical" ).Select(ob => new { ob.StoreID, ob.StoreName });
            List<TransRequest> lst = new List<TransRequest>();
            int id = 0;int reqToID = 0;
            foreach (var item in result)
            {
                TransRequest ob = new TransRequest();
                ob.RequestID = item.RequestID;
                ob.RequestNo = item.RequestNo;
                //ob.ReturnMethod = item.ReturnMethod;
                if (item.ReturnMethod.Equals("DTD"))
                {
                    ob.ReturnMethod = "Doller to Doller";
                }
                else if (item.ReturnMethod.Equals("EOI"))
                {
                    ob.ReturnMethod = "Exchange Other Item";
                }
                else if (item.ReturnMethod.Equals("ESI"))
                {
                    ob.ReturnMethod = "Exchange Same Item";
                }
                else
                {
                    ob.ReturnMethod = ob.ReturnMethod;
                }
                ob.RequestDate = Convert.ToDateTime(item.RequestDate).ToString("dd/MM/yyyy");
                
                id = item.RequestTo == null ? 0 : Convert.ToInt32(item.RequestTo);
                
                if (!string.IsNullOrEmpty(item.ToSource)) {
                    if (item.ToSource.Equals("SUP") || item.ToSource.Equals("LAG"))
                    {
                        ob.RequestTo =  (from t in sup where t.SupplierID == id  select t.SupplierID).FirstOrDefault().ToString();
                        ob.RequestToName = (from t in sup where t.SupplierID == id select t.SupplierName).FirstOrDefault();   
                    }
                    else if (item.ToSource.Equals("STR"))
                    {

                        ob.RequestTo = (from t in str where t.StoreID == id select t.StoreID).FirstOrDefault().ToString();
                        ob.RequestToName = (from t in str where t.StoreID == id select t.StoreName).FirstOrDefault();   
                    }
                    else{
                        ob.RequestTo = "";
                        ob.RequestToName = "";   
                    }
                }

                reqToID = item.RequestFrom == null?0: Convert.ToInt32(item.RequestFrom);
                ob.RequestFrom = reqToID.ToString();
                ob.RequestFromName = (from t in str where t.StoreID == reqToID select t.StoreName).FirstOrDefault();   

                if (item.RecordStatus.Equals("CNF"))
                {
                    ob.RecordStatus = "Confirmed";
                }
                else if (item.RecordStatus.Equals("APV"))
                {
                    ob.RecordStatus = "Approved";
                }
                else if (item.RecordStatus.Equals("NCF"))
                {
                    ob.RecordStatus = "Not Confirmed";
                }
                else if (item.RecordStatus.Equals("CHK"))
                {
                    ob.RecordStatus = "Checked";
                }
                else if (item.RecordStatus.Equals("RCV"))
                {
                    ob.RecordStatus = "Received";
                }
                else 
                {
                    ob.RecordStatus = item.RecordStatus;
                }
               // ob.RecordStatus = item.RecordStatus;
                lst.Add(ob);
            }
            var data = (from temp in lst select new { temp.RequestID, temp.RequestFrom, temp.RequestTo, temp.RequestNo, temp.RequestFromName, temp.RequestToName, temp.ReturnMethod, temp.RequestDate, temp.RecordStatus }).OrderByDescending(ob => ob.RequestID);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchLoanReceiveRequest(string search)
        {
            search = search.ToUpper();
            var result = from temp in repository.InvTransRequestRepository.Get(filter: o => o.RequestType == "LNRR", orderBy: q => q.OrderByDescending(d => d.RequestDate))
                         where ((temp.RequestNo.ToUpper().StartsWith(search) || temp.RequestNo.ToUpper() == search))
                         select new { temp.RequestID,temp.RequestNo, temp.ReturnMethod, temp.RequestDate, temp.RecordStatus };
            List<TransRequest> lst = new List<TransRequest>();
            foreach (var item in result)
            {
                TransRequest ob = new TransRequest();
                ob.RequestID = item.RequestID;
                ob.RequestNo = item.RequestNo;
                ob.ReturnMethod = item.ReturnMethod;
                ob.RequestDate = Convert.ToDateTime(item.RequestDate).ToString("dd/MM/yyyy");
                ob.RecordStatus = item.RecordStatus;
                lst.Add(ob);
            }
            var data = from temp in lst select new { temp.RequestID,temp.RequestNo, temp.ReturnMethod, temp.RequestDate, temp.RecordStatus };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAutoCompleteData()
        {
            var search = repository.InvTransRequestRepository.Get(filter:o=>o.RequestType=="LNRR",orderBy: q => q.OrderByDescending(d => d.RequestDate)).Select(ob => ob.RequestNo);
            return Json(search, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLoanIssueRequestList()
        {
            var result = from temp in repository.InvTransRequestRepository.Get(filter: o => o.RequestType == "LNIR")
                         select new { temp.RequestID, temp.RequestNo, temp.ReturnMethod, temp.RequestDate, temp.RecordStatus,temp.RequestFrom,temp.RequestTo,temp.ToSource,temp.FromSource };

            var sup = repository.SysSupplierRepository.Get(filter: ob => ob.SupplierCategory == "Chemical" && ob.IsActive == true && ob.IsDelete == false).Select(ob => new { ob.SupplierName, ob.SupplierID });
            var str = repository.StoreRepository.Get(filter: o => o.IsActive == true && o.IsDelete == false && o.StoreCategory == "Chemical").Select(ob => new { ob.StoreID, ob.StoreName });
            int id = 0; int reqToID = 0;
            List<TransRequest> lst = new List<TransRequest>();
            foreach (var item in result)
            {
                TransRequest ob = new TransRequest();
                ob.RequestID = item.RequestID;
                ob.RequestNo = item.RequestNo;
                if (item.ReturnMethod.Equals("DTD"))
                {
                    ob.ReturnMethod = "Doller to Doller";
                }
                else if (item.ReturnMethod.Equals("EOI"))
                {
                    ob.ReturnMethod = "Exchange Other Item";
                }
                else if (item.ReturnMethod.Equals("ESI"))
                {
                    ob.ReturnMethod = "Exchange Same Item";
                }
                else
                {
                    ob.ReturnMethod = ob.ReturnMethod;
                }
                ob.RequestDate = Convert.ToDateTime(item.RequestDate).ToString("dd/MM/yyyy");


                id = item.RequestFrom == null ? 0 : Convert.ToInt32(item.RequestFrom);

                if (!string.IsNullOrEmpty(item.FromSource))
                {
                    if (item.FromSource.Equals("SUP") || item.FromSource.Equals("LAG"))
                    {
                        ob.RequestFrom = (from t in sup where t.SupplierID == id select t.SupplierID).FirstOrDefault().ToString();
                        ob.RequestFromName = (from t in sup where t.SupplierID == id select t.SupplierName).FirstOrDefault();

                        //ob.RequestTo = (from t in sup where t.SupplierID == id select t.SupplierID).FirstOrDefault().ToString();
                        //ob.RequestToName = (from t in sup where t.SupplierID == id select t.SupplierName).FirstOrDefault();
                    }
                    else if (item.FromSource.Equals("STR"))
                    {

                        ob.RequestFrom = (from t in str where t.StoreID == id select t.StoreID).FirstOrDefault().ToString();
                        ob.RequestFromName = (from t in str where t.StoreID == id select t.StoreName).FirstOrDefault();
                    }
                    else
                    {
                        ob.RequestFrom = "";
                        ob.RequestFromName = "";
                    }
                }

                reqToID = item.RequestTo == null ? 0 : Convert.ToInt32(item.RequestTo);
                ob.RequestTo = reqToID.ToString();
                ob.RequestToName = (from t in str where t.StoreID == reqToID select t.StoreName).FirstOrDefault();   



                if (item.RecordStatus.Equals("NCF")) 
                {
                    ob.RecordStatus = "Not Confirmed";
                }
                else if (item.RecordStatus.Equals("CNF"))
                {
                    ob.RecordStatus = "Confirmed";
                }
                else if (item.RecordStatus.Equals("CHK"))
                {
                    ob.RecordStatus = "Checked";
                }
                else 
                {
                    ob.RecordStatus = item.RecordStatus;
                }               
                lst.Add(ob);
            }
            var data = (from temp in lst select new { temp.RequestID, temp.RequestNo, temp.ReturnMethod, temp.RequestFrom, temp.RequestFromName, temp.RequestTo, temp.RequestToName, temp.RequestDate, temp.RecordStatus, temp.ToSource }).OrderByDescending(ob => ob.RequestID);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchLoanIssueRequest(string search)
        {
            search = search.ToUpper();
            var result = from temp in repository.InvTransRequestRepository.Get(filter: o => o.RequestType == "LNIR", orderBy: q => q.OrderByDescending(d => d.RequestDate))
                         where ((temp.RequestNo.ToUpper().StartsWith(search) || temp.RequestNo.ToUpper() == search))
                         select new { temp.RequestID, temp.RequestNo, temp.ReturnMethod, temp.RequestDate, temp.RecordStatus };
            List<TransRequest> lst = new List<TransRequest>();
            foreach (var item in result)
            {
                TransRequest ob = new TransRequest();
                ob.RequestID = item.RequestID;
                ob.RequestNo = item.RequestNo;
                ob.ReturnMethod = item.ReturnMethod;
                ob.RequestDate = Convert.ToDateTime(item.RequestDate).ToString("dd/MM/yyyy");
                ob.RecordStatus = item.RecordStatus;
                lst.Add(ob);
            }
            var data = from temp in lst select new { temp.RequestID, temp.RequestNo, temp.ReturnMethod, temp.RequestDate, temp.RecordStatus };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAutoCompleteIssueData()
        {
            var search = repository.InvTransRequestRepository.Get(filter: o => o.RequestType == "LNIR", orderBy: q => q.OrderByDescending(d => d.RequestDate)).Select(ob => ob.RequestNo);
            return Json(search, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLoanReceiveRequestByID(long RequestID)
        {            
            INV_TransRequest data = repository.InvTransRequestRepository.GetByID(RequestID);
            TransRequest ob = new TransRequest();
            ob.RequestID = Convert.ToInt64(data.RequestID);
            ob.RequestNo = data.RequestNo;
            ob.RequestDate = Convert.ToDateTime(data.RequestDate).ToString("dd/MM/yyyy");
            ob.RequestType = data.RequestType;
            ob.RequestFrom = data.RequestFrom;
            ob.RequestTo = data.RequestTo;
            if (ob.RequestType == "LNIR")
            {
              //  ob.RequestFromName = repository.StoreRepository.GetByID(Convert.ToInt32(data.RequestTo)).StoreName;// data.RequestFrom;
                ob.RequestToName = repository.StoreRepository.GetByID(Convert.ToInt32(data.RequestTo)).StoreName;

                if (data.FromSource.Equals("SUP") || data.FromSource.Equals("LAG"))
                {
                    ob.RequestFromName = repository.SysSupplierRepository.GetByID(Convert.ToInt32(data.RequestFrom)).SupplierName;
                }
                else if (data.FromSource == "STR")
                {
                    ob.RequestFromName = repository.StoreRepository.GetByID(Convert.ToInt32(data.RequestFrom)).StoreName;// data.RequestFrom;
                }
                else
                {
                    ob.RequestFromName = "";
                }
                ob.FromSource = data.FromSource;
            }
            else 
            {
                ob.RequestFromName = repository.StoreRepository.GetByID(Convert.ToInt32(data.RequestFrom)).StoreName;// data.RequestFrom;
                if (data.ToSource.Equals("SUP") || data.ToSource.Equals("LAG"))
                {
                    ob.RequestToName = repository.SysSupplierRepository.GetByID(Convert.ToInt32(data.RequestTo)).SupplierName;
                }
                else if (data.ToSource.Equals("STR"))
                {
                    ob.RequestToName = repository.StoreRepository.GetByID(Convert.ToInt32(data.RequestTo)).StoreName;// data.RequestFrom;
                }
                else
                {
                    ob.RequestToName = "";
                }
            }
            ob.ReturnMethod = data.ReturnMethod;
            ob.ExpectetReturnTime = Convert.ToByte(data.ExpectetReturnTime).ToString();
            ob.CheckComments = data.CheckComments;
            ob.ApproveComments = data.ApproveComments;
            ob.RecordStatus = data.RecordStatus;
            ob.ToSource = data.ToSource;
            var reqItemData = repository.InvTransRequestItemRepository.Get(filter: o => o.RequestID == ob.RequestID);
            ob.ChemicalSelectedList = new List<TransRequestItem>();
            foreach (var item in reqItemData)
            {               
                TransRequestItem obItem = new TransRequestItem();
                obItem.TransRequestItemID = item.TransRequestItemID;
                obItem.ItemID = Convert.ToInt32(item.ItemID);
                if (obItem.ItemID> 0) { obItem.ItemName = repository.SysChemicalItemRepository.GetByID(obItem.ItemID).ItemName; }
                if (Convert.ToInt32(item.PackSize) >0) { obItem.PackSize = repository.SysSizeRepository.GetByID(Convert.ToInt32(item.PackSize)).SizeName; }
               
                obItem.SizeID = item.PackSize.ToString();
                if (Convert.ToInt32(item.SizeUnit)>0) { obItem.SizeUnit = repository.SysUnitRepository.GetByID(Convert.ToInt32(item.SizeUnit)).UnitName; }
               
                obItem.SizeUnitID = item.SizeUnit.ToString();
                obItem.PackQty = Convert.ToInt32(item.PackQty);
                obItem.RefItemID = Convert.ToInt32(item.RefItemID);
                obItem.RefSupplierID = Convert.ToInt32(item.RefSupplierID);
                obItem.SupplierID = Convert.ToInt32(item.RefSupplierID);
                if (item.RefSupplierID != null) { obItem.RefSupplierName = repository.SysSupplierRepository.GetByID(Convert.ToInt32(item.RefSupplierID)).SupplierName; }
              
                obItem.ReturnMethod = item.ReturnMethod;
                obItem.ReferenceQty = item.TransQty == null ? 0 : (decimal)item.TransQty;
                decimal d = item.TransUnit == null ? 0 : Convert.ToInt32(item.TransUnit);
                if (d > 0)
                {
                    obItem.ReferenceUnit = repository.SysUnitRepository.GetByID(d).UnitName;
                }
                else
                { 
                     obItem.ReferenceUnit = "";
                }
                obItem.ReferenceUnitID = item.ReferenceUnit.ToString();              
                ob.ChemicalSelectedList.Add(obItem);
            }
            return Json(ob, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckRecordStatus(string note, long RequestID)
        {
            try
            {
                if (RequestID >0)
                {
                    INV_TransRequest ob = repository.InvTransRequestRepository.GetByID(Convert.ToInt64(RequestID));
                    if (ob.RecordStatus == "NCF")
                    {
                        ob.CheckComments = note;
                        ob.RecordStatus = "CHK";
                        ob.CheckedBy = Convert.ToInt32(Session["UserID"]);
                        ob.CheckDate = DateTime.Now;
                        repository.InvTransRequestRepository.Update(ob);
                        int flag = repository.Save();
                        if (flag == 1)
                        {
                            _vmMsg.Type = Enums.MessageType.Success;
                            _vmMsg.Msg = "Confirmed Successfully.";
                        }
                        else
                        {
                            _vmMsg.Type = Enums.MessageType.Error;
                            _vmMsg.Msg = "Confirmed Faild.";
                        }
                    }
                    else
                    {
                        _vmMsg.Type = Enums.MessageType.Error;
                        _vmMsg.Msg = "Only Saved Record Should be Checked.";
                    }
                }
                else
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Data Save First Before Checked.";
                }
            }
            catch (DbEntityValidationException e)
            {
                // throw new Exception e;
            }
            return Json(new { msg = _vmMsg });
        }

        public ActionResult ConfirmRecordStatus(string note, long RequestID)
        {
            try
            {
                if (RequestID > 0)
                {
                    INV_TransRequest ob = repository.InvTransRequestRepository.GetByID(Convert.ToInt64(RequestID));
                    if (ob.RecordStatus == "CHK")
                    {
                        ob.ApproveComments = note;
                        ob.RecordStatus = "CNF";
                        ob.ApprovedBy = Convert.ToInt32(Session["UserID"]);
                        ob.ApproveDate = DateTime.Now;
                        repository.InvTransRequestRepository.Update(ob);
                        int flag = repository.Save();
                        if (flag == 1)
                        {
                            _vmMsg.Type = Enums.MessageType.Success;
                            _vmMsg.Msg = "Approved Successfully.";
                        }
                        else
                        {
                            _vmMsg.Type = Enums.MessageType.Error;
                            _vmMsg.Msg = "Approved Faild.";
                        }
                    }
                    else
                    {
                        _vmMsg.Type = Enums.MessageType.Error;
                        _vmMsg.Msg = "Only Confirmed Record Should be Approved.";
                    }
                }
                else
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Data Save First Before Checked.";
                }
            }
            catch (DbEntityValidationException e)
            {
                // throw new Exception e;
            }
            return Json(new { msg = _vmMsg });
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetUnit()
        {
            var sysUnit = (from t in repository.SysUnitRepository.Get(filter: ob => ob.IsActive == true && ob.IsDelete == false && ob.UnitCategory == "ChemicalPack")
                         select new { UnitID = t.UnitID, UnitName = t.UnitName }).OrderBy(o=>o.UnitName);
            return Json(sysUnit, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetUnit2()
        {
            var sysUnit = (from t in repository.SysUnitRepository.Get(filter: ob => ob.IsActive == true && ob.IsDelete == false && ob.UnitCategory == "Chemical")
                           select new { UnitID = t.UnitID, UnitName = t.UnitName }).OrderBy(o => o.UnitName);
            return Json(sysUnit, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetSize()
        {
            var sysSize = (from t in repository.SysSizeRepository.Get(filter: ob => ob.IsActive == true && ob.IsDelete == false && ob.SizeCategory == "ChemicalPack")
                          select new {
                              SizeID = t.SizeID,
                              SizeName = t.SizeName
                          }).OrderBy(o => o.SizeName); ;
            return Json(sysSize, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetChemicalSupplier()
        {
            DalSysSupplier ob = new DalSysSupplier();
            var sysSupp = ob.GetChemicalSupplier();

            return Json(sysSupp, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult GetLoanStockInfo()
        //{ }

        public ActionResult GetLoanStockInfo()
        {
            var result = from temp in repository.StoreRepository.Get(filter: o => o.IsActive == true && o.IsDelete == false && o.StoreCategory == "Chemical" && o.StoreType == "Loan")
                         select new { StoreID = temp.StoreID, ChemicalStoreName = temp.StoreName, StoreCode = temp.StoreCode };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchLoanStockByFirstName(string search)
        {
            search = search.ToUpper();
            var result = from temp in repository.StoreRepository.Get(filter: o => o.IsActive == true && o.IsDelete == false && o.StoreCategory == "Chemical" && o.StoreType == "Loan")
                         where ((temp.StoreName.ToUpper().StartsWith(search) || temp.StoreName.ToUpper() == search))
                         select new { StoreID = temp.StoreID, ChemicalStoreName = temp.StoreName, StoreCode = temp.StoreCode };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLoanStockAutocompleteData()
        {
            var search = repository.StoreRepository.Get(filter: o => o.IsActive == true && o.IsDelete == false && o.StoreCategory == "Chemical" && o.StoreType == "Loan", orderBy: q => q.OrderByDescending(d => d.StoreID)).Select(ob => ob.StoreName);
            return Json(search, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteChemicalItem(long ItemID)
        {
            var data = repository.InvTransRequestItemRepository.GetByID(Convert.ToInt64(ItemID));
            if (data != null)
            {
                var chk = repository.InvTransRequestRepository.GetByID(data.RequestID);
                if (chk.RecordStatus != "CNF")
                {
                    repository.InvTransRequestItemRepository.Delete(data);
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

        public ActionResult DeleteLoanRequest(long id)
        {
            INV_TransRequest isGradeExist = repository.InvTransRequestRepository.GetByID(id);
            if (isGradeExist != null)
            {
                try
                {
                    var item = repository.InvTransRequestRefRepository.Get(filter:ob=>ob.RequestID == id);
                    if (item != null)
                    { 
                        foreach (var i in item)
                        {
                            repository.InvTransRequestRefRepository.Delete(i.TransRequestRefID);
                            repository.Save();
                        }
                    }
                    repository.InvTransRequestRepository.Delete(isGradeExist);
                    repository.Save();
                    _vmMsg.Type = Enums.MessageType.Success;
                    _vmMsg.Msg = "Data Deleted Successfully.";

                    }
                catch (Exception ex)
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Confirmation faild due to communication error.";
                }
            }
            else
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Please Delete Chemical Info First.";

            }

            return Json(_vmMsg, JsonRequestBehavior.AllowGet);
        }
	}
}