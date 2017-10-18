using System;
using System.Globalization;
using System.Web;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DatabaseUtility;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.OperationModel;
using System.Transactions;
using System.Linq;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalPrqPurchase : ExecuteSqlStatement
    {
        private readonly BLC_DEVEntities _context;
        private readonly DatabseManager _databseManager;
        //private string _procedureName;
        private List<InputParameter> _lstInputParameter;
        private List<OutpuParameters> _lstOutputParameter;
        private string _query;
        //private int _affectedRow;
        
        public DalPrqPurchase(DatabseManager databseManager)
        {
            _databseManager = databseManager;
            _lstInputParameter = new List<InputParameter>();
            _lstOutputParameter = new List<OutpuParameters>();
            _query = string.Empty;

        }
        public DalPrqPurchase()
        {
            _context = new BLC_DEVEntities();
        }


        public long SavePurchaseInformation(PurchaseReceive model, int userId)
        {

            long CurrentPurchaseID = 0;
            try
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    
                        #region New_Purchase_Insert
                    using (_context)
                    {
                        Prq_Purchase objPurchase = new Prq_Purchase();
                        //Random r = new Random();

                        //objPurchase.PurchaseNo = r.Next().ToString();
                        objPurchase.PurchaseNo = DalCommon.GetPreDefineNextCodeByUrl("Purchase/Purchase");
                        objPurchase.SupplierID = Convert.ToInt16(model.SupplierID);
                        objPurchase.SupplierAddressID = Convert.ToInt16(model.SupplierAddressID);
                        objPurchase.PurchaseCategory = model.PurchaseCategory;
                        objPurchase.PurchaseType = model.PurchaseType;
                        objPurchase.PurchaseYear = model.PurchaseYear;
                        objPurchase.PurchaseDate = DalCommon.SetDate(model.PurchaseDate);
                        objPurchase.PurchaseNote = model.PurchaseNote;
                        objPurchase.RecordStatus = "NCF";
                        objPurchase.SetOn = DateTime.Now;
                        objPurchase.SetBy = userId;

                        _context.Prq_Purchase.Add(objPurchase);
                        _context.SaveChanges();

                        CurrentPurchaseID = objPurchase.PurchaseID;

                        if (model.ChallanList != null)
                        {
                            Prq_PurchaseChallan objChallan = new Prq_PurchaseChallan();

                            objChallan.ChallanNo = model.ChallanList.FirstOrDefault().ChallanNo;
                            objChallan.PurchaseID = CurrentPurchaseID;
                            objChallan.SourceID = model.ChallanList.FirstOrDefault().SourceID;
                            objChallan.LocationID = model.ChallanList.FirstOrDefault().LocationID;
                            objChallan.ReceiveStore = DalCommon.GetStoreCode(model.ChallanList.FirstOrDefault().ReceiveStore);
                            objChallan.ChallanCategory = model.ChallanList.FirstOrDefault().ChallanCategory;
                            objChallan.ChallanNote = model.ChallanList.FirstOrDefault().ChallanNote;



                            objChallan.ChallanDate = Convert.ToDateTime(model.ChallanList.FirstOrDefault().ChallanDate);


                            objChallan.RecordStatus = "NCF";
                            objChallan.SetOn = DateTime.Now;
                            objChallan.SetBy = userId;

                            _context.Prq_PurchaseChallan.Add(objChallan);
                            _context.SaveChanges();

                            var CurrentChallanNo = objChallan.ChallanID;

                            if (model.ChallanItemList != null)
                            {
                                foreach (var ChallanItem in model.ChallanItemList)
                                {

                                    Prq_PurchaseChallanItem objPurchaseChallanItem = new Prq_PurchaseChallanItem();

                                    objPurchaseChallanItem.ChallanID = CurrentChallanNo;
                                    objPurchaseChallanItem.ItemCategory = "Leather";
                                    objPurchaseChallanItem.ItemTypeID = DalCommon.GetItemTypeCode(ChallanItem.ItemTypeID);
                                    objPurchaseChallanItem.ItemSizeID = DalCommon.GetSizeCode(ChallanItem.ItemSizeID);
                                    objPurchaseChallanItem.Description = ChallanItem.Description;
                                    objPurchaseChallanItem.UnitID = DalCommon.GetUnitCode(ChallanItem.UnitID);
                                    objPurchaseChallanItem.ChallanQty = ChallanItem.ChallanQty;
                                    objPurchaseChallanItem.ReceiveQty = ChallanItem.ReceiveQty;
                                    objPurchaseChallanItem.Remark = ChallanItem.Remark;
                                    objPurchaseChallanItem.RecordStatus = "NCF";
                                    objPurchaseChallanItem.SetBy = userId;
                                    objPurchaseChallanItem.SetOn = DateTime.Now;

                                    _context.Prq_PurchaseChallanItem.Add(objPurchaseChallanItem);
                                    _context.SaveChanges();
                                }
                            }
                        }

                    }
                        #endregion

                        transaction.Complete();

                }
                return CurrentPurchaseID;
            }
            catch(Exception e)
            {
                return CurrentPurchaseID;
            }
            
        }

        public string GetPurchaseNumber(long _PurchaseID)
        {
            var PurchaseNumber = (from p in _context.Prq_Purchase.AsEnumerable()
                                  where p.PurchaseID == _PurchaseID
                                  select p.PurchaseNo).FirstOrDefault();

            return PurchaseNumber;
        }


        public bool ConfirmPurchase(string purchaseNumber, string confirmComment)
        {
            try
            {
                using (TransactionScope Transaction = new TransactionScope())
                {
                    using (_context)
                    {


                        var purchaseInfo = (from p in _context.Prq_Purchase.AsEnumerable()
                                            where (p.PurchaseID).ToString() == purchaseNumber
                                            select p).FirstOrDefault();

                        var challanList = (from c in _context.Prq_PurchaseChallan.AsEnumerable()
                                           where (c.PurchaseID).ToString() == purchaseNumber
                                           select c).ToList();

                        foreach (var item in challanList)
                        {
                            var challanItem = (from i in _context.Prq_PurchaseChallanItem.AsEnumerable()
                                               where i.ChallanID == item.ChallanID
                                               select i).ToList();

                            foreach (var leatherItem in challanItem)
                            {
                                var leatherTypeID = DalCommon.GetLeatherTypeCode("Raw Hide");
                                leatherTypeID = leatherTypeID == 0 ? Convert.ToByte(1) : leatherTypeID;
                                var leatherStatusID = DalCommon.GetLeatherStatusCode("Raw Hide");
                                leatherStatusID = leatherStatusID == 0 ? Convert.ToByte(1) : leatherStatusID;
                               
                                var currentDate = DateTime.Now.Date;

                                if (leatherItem.RecordStatus != "CNF" && leatherItem.RecordStatus != "Confirmed")
                                {
                                    #region Daily_Stock_Update

                                    var CheckDate = (from ds in _context.Inv_StockDaily.AsEnumerable()
                                                     where ds.ItemTypeID == leatherItem.ItemTypeID && ds.StoreID == item.ReceiveStore
                                                      && ds.LeatherStatus == leatherStatusID && ds.LeatherType == leatherTypeID && ds.StockDate == currentDate
                                                     select ds).Any();

                                    if (CheckDate)
                                    {
                                        var CurrentItem = (from ds in _context.Inv_StockDaily.AsEnumerable()
                                                           where ds.ItemTypeID == leatherItem.ItemTypeID && ds.StoreID == item.ReceiveStore
                                                            && ds.LeatherStatus == leatherStatusID && ds.LeatherType == leatherTypeID && ds.StockDate == currentDate
                                                           select ds).FirstOrDefault();

                                        CurrentItem.DailyReceiveQty = CurrentItem.DailyReceiveQty + leatherItem.ReceiveQty;
                                        CurrentItem.ClosingQty = CurrentItem.ClosingQty + leatherItem.ReceiveQty;
                                        _context.SaveChanges();
                                    }
                                    else
                                    {
                                        var PreviousRecord = (from ds in _context.Inv_StockDaily.AsEnumerable()
                                                              where ds.ItemTypeID == leatherItem.ItemTypeID && ds.StoreID == item.ReceiveStore
                                                              && ds.LeatherStatus == leatherStatusID && ds.LeatherType == leatherTypeID
                                                              orderby ds.TransectionID descending
                                                              select ds).FirstOrDefault();

                                        Inv_StockDaily objStockDaily = new Inv_StockDaily();

                                        objStockDaily.ItemTypeID = leatherItem.ItemTypeID;
                                        objStockDaily.StoreID = item.ReceiveStore;
                                        objStockDaily.UnitID = leatherItem.UnitID;
                                        objStockDaily.LeatherStatus = leatherStatusID;
                                        objStockDaily.LeatherType = leatherTypeID;

                                        objStockDaily.OpeningQty = (PreviousRecord == null ? 0 : PreviousRecord.ClosingQty);
                                        objStockDaily.DailyReceiveQty = leatherItem.ReceiveQty;
                                        objStockDaily.DailyIssueQty = 0;
                                        objStockDaily.ClosingQty = objStockDaily.OpeningQty + leatherItem.ReceiveQty;

                                        objStockDaily.StockDate = currentDate;

                                        _context.Inv_StockDaily.Add(objStockDaily);
                                        _context.SaveChanges();

                                    }

                                    #endregion

                                    #region Supplier_Stock_Update

                                    var CheckSupplierStock = (from i in _context.Inv_StockSupplier.AsEnumerable()
                                                              where i.SupplierID == purchaseInfo.SupplierID && i.ItemTypeID == leatherItem.ItemTypeID && 
                                                              i.LeatherType == leatherTypeID && i.LeatherStatusID == leatherStatusID && i.StoreID == item.ReceiveStore && 
                                                              i.PurchaseID.ToString()== purchaseInfo.PurchaseID.ToString()
                                                              select i).Any();

                                    if (!CheckSupplierStock)
                                    {
                                        Inv_StockSupplier objStockSupplier = new Inv_StockSupplier();
                                        objStockSupplier.SupplierID = purchaseInfo.SupplierID;
                                        objStockSupplier.StoreID = item.ReceiveStore;
                                        objStockSupplier.PurchaseID = purchaseInfo.PurchaseID;
                                        objStockSupplier.RefChallanID = item.ChallanID;
                                        objStockSupplier.ItemTypeID = leatherItem.ItemTypeID;
                                        objStockSupplier.LeatherType = leatherTypeID;
                                        objStockSupplier.LeatherStatusID = leatherStatusID;
                                        objStockSupplier.UnitID = leatherItem.UnitID;
                                        objStockSupplier.OpeningQty = 0;
                                        objStockSupplier.ReceiveQty = leatherItem.ReceiveQty;
                                        objStockSupplier.IssueQty = 0;
                                        objStockSupplier.ClosingQty = leatherItem.ReceiveQty;
                                        objStockSupplier.UpdateReason = "Receive";


                                        _context.Inv_StockSupplier.Add(objStockSupplier);
                                        _context.SaveChanges();
                                    }
                                    else
                                    {
                                        var LastSupplierStock = (from i in _context.Inv_StockSupplier.AsEnumerable()
                                                                 where i.SupplierID == purchaseInfo.SupplierID && i.ItemTypeID == leatherItem.ItemTypeID && i.LeatherType == leatherTypeID && i.LeatherStatusID == leatherStatusID && i.StoreID == item.ReceiveStore && i.PurchaseID.ToString() == purchaseInfo.PurchaseID.ToString()
                                                                 orderby i.TransectionID descending
                                                                 select i).FirstOrDefault();

                                        Inv_StockSupplier objStockSupplier = new Inv_StockSupplier();
                                        objStockSupplier.SupplierID = purchaseInfo.SupplierID;
                                        objStockSupplier.StoreID = item.ReceiveStore;
                                        objStockSupplier.PurchaseID = purchaseInfo.PurchaseID;
                                        objStockSupplier.RefChallanID = item.ChallanID;
                                        objStockSupplier.ItemTypeID = leatherItem.ItemTypeID;
                                        objStockSupplier.LeatherType = leatherTypeID;
                                        objStockSupplier.LeatherStatusID = leatherStatusID;
                                        objStockSupplier.UnitID = leatherItem.UnitID;

                                        objStockSupplier.OpeningQty = LastSupplierStock.ClosingQty;
                                        objStockSupplier.ReceiveQty = leatherItem.ReceiveQty;
                                        objStockSupplier.IssueQty = 0;
                                        objStockSupplier.ClosingQty = LastSupplierStock.ClosingQty + leatherItem.ReceiveQty;
                                        objStockSupplier.UpdateReason = "Receive";

                                        _context.Inv_StockSupplier.Add(objStockSupplier);
                                        _context.SaveChanges();
                                    }
                                    #endregion
                                    
                                    #region Item_Stock_Update

                                    var CheckItemStock = (from i in _context.Inv_StockItem.AsEnumerable()
                                                          where i.ItemTypeID == leatherItem.ItemTypeID && i.LeatherType == leatherTypeID && i.LeatherStatus == leatherStatusID && i.StoreID == item.ReceiveStore
                                                          select i).Any();


                                    if (!CheckItemStock)
                                    {
                                        Inv_StockItem objStockItem = new Inv_StockItem();

                                        objStockItem.ItemTypeID = leatherItem.ItemTypeID;
                                        objStockItem.LeatherType = leatherTypeID;
                                        objStockItem.LeatherStatus = leatherStatusID;
                                        objStockItem.StoreID = item.ReceiveStore;
                                        objStockItem.UnitID = leatherItem.UnitID;
                                        objStockItem.OpeningQty = 0;
                                        objStockItem.ReceiveQty = leatherItem.ReceiveQty;
                                        objStockItem.IssueQty = 0;
                                        objStockItem.ClosingQty = leatherItem.ReceiveQty;

                                        _context.Inv_StockItem.Add(objStockItem);
                                        _context.SaveChanges();
                                    }
                                    else
                                    {
                                        var LastItemInfo = (from i in _context.Inv_StockItem
                                                            where i.ItemTypeID == leatherItem.ItemTypeID && i.LeatherType == leatherTypeID && i.LeatherStatus == leatherStatusID && i.StoreID == item.ReceiveStore
                                                            orderby i.TransectionID descending
                                                            select i).FirstOrDefault();

                                        //var LastItemInfo = (from i in context.Inv_StockItem
                                        //                    where i.TransectionID == LastItemTransactionID
                                        //                    select i).FirstOrDefault();

                                        Inv_StockItem objStockItem = new Inv_StockItem();

                                        objStockItem.OpeningQty = LastItemInfo.ClosingQty;
                                        objStockItem.IssueQty = 0;
                                        objStockItem.ReceiveQty = leatherItem.ReceiveQty;
                                        objStockItem.ClosingQty = LastItemInfo.ClosingQty + leatherItem.ReceiveQty;
                                        objStockItem.ItemTypeID = LastItemInfo.ItemTypeID;
                                        objStockItem.LeatherType = LastItemInfo.LeatherType;
                                        objStockItem.LeatherStatus = LastItemInfo.LeatherStatus;
                                        objStockItem.StoreID = LastItemInfo.StoreID;
                                        objStockItem.UnitID = leatherItem.UnitID;

                                        _context.Inv_StockItem.Add(objStockItem);
                                        _context.SaveChanges();
                                    }

                                    #endregion

                                    leatherItem.RecordStatus = "CNF";
                                }
                               
                            }

                            item.RecordStatus = "CNF";
                        }

                        purchaseInfo.RecordStatus = "CNF";

                        _context.SaveChanges();

                    }
                    Transaction.Complete();
                }

                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public int UpdatePurchaseInformation(PurchaseReceive model, int userId)
        {
            long CurrentChallanNo = 0;
            try
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    using(_context)
                    {
                    
                        #region Purchase_Informaiton_Update
                        var currentPurchase = (from p in _context.Prq_Purchase.AsEnumerable()
                                               where p.PurchaseID == Convert.ToInt64(model.PurchaseID)
                                               select p).FirstOrDefault();

                        if(currentPurchase.RecordStatus!="CNF")
                        {
                            currentPurchase.SupplierID = Convert.ToInt32(model.SupplierID);
                            currentPurchase.SupplierAddressID = Convert.ToInt32(model.SupplierAddressID);
                            currentPurchase.PurchaseCategory = model.PurchaseCategory;
                            currentPurchase.PurchaseType = model.PurchaseType;
                            currentPurchase.PurchaseYear = model.PurchaseYear;
                            currentPurchase.PurchaseNote = model.PurchaseNote;
                            currentPurchase.RecordStatus = "NCF";

                            currentPurchase.PurchaseDate = DalCommon.SetDate(model.PurchaseDate);
                            currentPurchase.SetBy = userId;
                            currentPurchase.SetOn = DateTime.Now;
                            _context.SaveChanges();
                        }
                        
                        #endregion

                        #region Update_Challan_Information
                        if (model.ChallanList != null)
                        {
                            foreach (var challan in model.ChallanList)
                            {
                                #region New_Challan_Insertion
                                if (challan.ChallanID == 0)
                                {
                                    Prq_PurchaseChallan objChallan = new Prq_PurchaseChallan();

                                    objChallan.ChallanNo = challan.ChallanNo;
                                    objChallan.PurchaseID = Convert.ToInt32(model.PurchaseID);
                                    objChallan.SourceID = challan.SourceID;
                                    objChallan.LocationID = challan.LocationID;
                                    objChallan.ReceiveStore = DalCommon.GetStoreCode(challan.ReceiveStore);
                                    objChallan.ChallanCategory = challan.ChallanCategory;
                                    objChallan.ChallanNote = challan.ChallanNote;
                                    objChallan.ChallanDate = Convert.ToDateTime(challan.ChallanDate);
                                    objChallan.RecordStatus = "NCF";
                                    objChallan.SetOn = DateTime.Now;
                                    objChallan.SetBy = userId; ;

                                    _context.Prq_PurchaseChallan.Add(objChallan);
                                    _context.SaveChanges();

                                    CurrentChallanNo = objChallan.ChallanID;
                                }
                                #endregion

                                #region Existing_Challan_Update
                                else if (challan.ChallanID != 0 && challan.RecordStatus != "CNF" && challan.RecordStatus != "Confirmed")
                                {
                                    var currentChallan = (from c in _context.Prq_PurchaseChallan.AsEnumerable()
                                                          where c.ChallanID == challan.ChallanID
                                                          select c).FirstOrDefault();

                                    currentChallan.ChallanNo = challan.ChallanNo;
                                    //currentChallan.PurchaseID = Convert.ToInt32(model.PurchaseID);
                                    currentChallan.SourceID = challan.SourceID;
                                    currentChallan.LocationID = challan.LocationID;
                                    currentChallan.ReceiveStore = DalCommon.GetStoreCode(challan.ReceiveStore);
                                    currentChallan.ChallanCategory = challan.ChallanCategory;
                                    currentChallan.ChallanNote = challan.ChallanNote;

                                    try
                                    {
                                        //var GridChallanDate = Convert.ToDateTime(challan.ChallanDate).Date.ToString("dd/MM/yyyy");
                                        //currentChallan.ChallanDate = DalCommon.SetDate(GridChallanDate);

                                        var GridChallanDate = challan.ChallanDate.Contains("/") ? challan.ChallanDate : Convert.ToDateTime(challan.ChallanDate).ToString("dd/MM/yyyy");
                                        //var GridChallanDate = Convert.ToDateTime(challan.ChallanDate).ToString().Contains("/") ? challan.ChallanDate : Convert.ToDateTime(challan.ChallanDate).ToString("dd/MM/yyyy");
                                        currentChallan.ChallanDate = DalCommon.SetDate(GridChallanDate);
                                    }
                                    catch
                                    {
                                        var GridChallanDate = Convert.ToDateTime(challan.ChallanDate).Date.ToString("dd/MM/yyyy");
                                        currentChallan.ChallanDate = DalCommon.SetDate(GridChallanDate);
                                        //currentChallan.ChallanDate = DalCommon.SetDate(challan.ChallanDate);
                                    }
                                    


                                    currentChallan.RecordStatus = "NCF";
                                    currentChallan.SetOn = DateTime.Now;
                                    currentChallan.SetBy = userId;

                                    _context.SaveChanges();

                                }
                                #endregion
                            }
                        }

                        #endregion

                        #region To_Find_ChallanID_For_Items_If_Any

                        if (model.ChallanItemList != null)
                        {
                            foreach (var challanItem in model.ChallanItemList)
                            {
                                if (challanItem.ChallanID != 0)
                                {
                                    CurrentChallanNo = challanItem.ChallanID;
                                    break;
                                }
                            }
                        }
                        #endregion

                        #region Update_Challan_Item_Information
                        if (model.ChallanItemList != null)
                        {
                            foreach (var challanItem in model.ChallanItemList)
                            {
                                #region New_Challan_Item_Insertion
                                if (challanItem.ChallanItemID == 0)
                                {
                                    Prq_PurchaseChallanItem objPurchaseChallanItem = new Prq_PurchaseChallanItem();

                                    objPurchaseChallanItem.ChallanID = CurrentChallanNo;
                                    objPurchaseChallanItem.ItemCategory = "Leather";
                                    objPurchaseChallanItem.ItemTypeID = DalCommon.GetItemTypeCode(challanItem.ItemTypeID);
                                    objPurchaseChallanItem.ItemSizeID = DalCommon.GetSizeCode(challanItem.ItemSizeID);
                                    objPurchaseChallanItem.Description = challanItem.Description;
                                    objPurchaseChallanItem.UnitID = DalCommon.GetUnitCode(challanItem.UnitID);
                                    objPurchaseChallanItem.ChallanQty = challanItem.ChallanQty;
                                    objPurchaseChallanItem.ReceiveQty = challanItem.ReceiveQty;
                                    objPurchaseChallanItem.Remark = challanItem.Remark;
                                    objPurchaseChallanItem.RecordStatus = "NCF";
                                    objPurchaseChallanItem.SetBy = userId; ;
                                    objPurchaseChallanItem.SetOn = DateTime.Now;

                                    _context.Prq_PurchaseChallanItem.Add(objPurchaseChallanItem);
                                    _context.SaveChanges();
                                }
                                #endregion

                                #region Update_Existing_Challan_Item
                                else if (challanItem.ChallanItemID != 0 && challanItem.RecordStatus != "CNF" && challanItem.RecordStatus != "Confirmed")
                                {
                                    var currentChallanItem = (from c in _context.Prq_PurchaseChallanItem.AsEnumerable()
                                                              where c.ChallanItemID == challanItem.ChallanItemID
                                                              select c).FirstOrDefault();

                                    currentChallanItem.ItemSizeID = DalCommon.GetSizeCode(challanItem.ItemSizeID);
                                    currentChallanItem.ItemTypeID = DalCommon.GetItemTypeCode(challanItem.ItemTypeID);
                                    currentChallanItem.UnitID = DalCommon.GetUnitCode(challanItem.UnitID);
                                    currentChallanItem.Description = challanItem.Description;
                                    currentChallanItem.ChallanQty = challanItem.ChallanQty;
                                    currentChallanItem.ReceiveQty = challanItem.ReceiveQty;
                                    currentChallanItem.Remark = challanItem.Remark;
                                    _context.SaveChanges();


                                }
                                #endregion

                            }
                        }
                        #endregion

                    }

                        transaction.Complete();
                  

                }
                return 1;
            }
            catch(Exception e)
            {
                return 0;
            }

        }

        public List<PrqPurchaseChallan> GetAllChallan()
        {
            var alldata = (from c in _context.Prq_PurchaseChallan.AsEnumerable()
                               select new PrqPurchaseChallan
                               {
                                   ChallanID = c.ChallanID,
                                   ChallanNo = c.ChallanNo,
                                   ChallanNote = c.ChallanNote
                               }).ToList();
                return alldata;
        }

        public List<PrqPurchaseChallanItem> GetAllChallanItem()
        {
            var alldata = (from c in _context.Prq_PurchaseChallanItem.AsEnumerable()
                               select new PrqPurchaseChallanItem
                               {
                                   ChallanID = c.ChallanID,
                                   Description = c.Description,
                                   ChallanQty = c.ChallanQty,
                                   ReceiveQty = c.ReceiveQty

                               }).ToList();
                return alldata;
        }

      
        public List<PurchaseReceive> GetPurchaseInformation()
        {
            var allData = (from p in _context.Prq_Purchase
                           //where p.RecordStatus == "NCF"
                           from s in _context.Sys_Supplier
                           where s.SupplierID == p.SupplierID
                           from sa in _context.Sys_SupplierAddress
                           where sa.SupplierAddressID == p.SupplierAddressID
                           orderby p.PurchaseID descending
                           select new PurchaseReceive
                           {
                               PurchaseID = p.PurchaseID,
                               PurchaseNo = p.PurchaseNo,
                               SupplierID = p.SupplierID,
                               SupplierName = s.SupplierName,
                               Address = sa.Address,
                               SupplierAddressID = p.SupplierAddressID,
                               PurchaseCategory = p.PurchaseCategory,
                               PurchaseType = p.PurchaseType,
                               PurchaseYear = p.PurchaseYear,
                               TempPurchaseDate = (p.PurchaseDate),
                               RecordStatus = p.RecordStatus
                           }).ToList();

            foreach (var Purchase in allData)
            {
                //decimal TotalQty = 0;

                var Challan = (from c in _context.Prq_PurchaseChallan
                               where c.PurchaseID == Purchase.PurchaseID
                               join i in _context.Prq_PurchaseChallanItem on c.ChallanID equals i.ChallanID into Items
                               from i in Items.DefaultIfEmpty().AsEnumerable()
                               select i).ToList();

                Purchase.TotalItem = Challan.Sum(p => p.ReceiveQty);
                Purchase.PurchaseDate = (Purchase.TempPurchaseDate).ToString("dd'/'MM'/'yyyy");
                Purchase.RecordStatus = DalCommon.ReturnRecordStatus(Purchase.RecordStatus);
            }
           

                return allData;
        }

        public PurchaseReceiveTest GetDetailPurchaseInformation(string PurchaseNumber)
        {
                var model = new PurchaseReceiveTest();

                var PurchaseInfo = (from p in _context.Prq_Purchase.AsEnumerable()
                                    where (p.PurchaseID).ToString() == PurchaseNumber
                                    from s in _context.Sys_Supplier
                                    where s.SupplierID == p.SupplierID
                                    from sa in _context.Sys_SupplierAddress
                                    where sa.SupplierID == p.SupplierID
                                    select new PrqPurchase
                                    {
                                        SupplierID = p.SupplierID,
                                        SupplierCode= s.SupplierCode,
                                        SupplierName = s.SupplierName,
                                        SupplierAddressID = p.SupplierAddressID,
                                        Address = sa.Address,
                                        ContactNumber = sa.ContactNumber,
                                        ContactPerson = sa.ContactPerson,
                                        PurchaseID = p.PurchaseID,
                                        PurchaseNo= p.PurchaseNo,
                                        PurchaseCategory = p.PurchaseCategory,
                                        PurchaseType = p.PurchaseType,
                                        PurchaseDate = (p.PurchaseDate).ToString("dd'/'MM'/'yyyy"),
                                        PurchaseYear = p.PurchaseYear,
                                        PurchaseNote = p.PurchaseNote,
                                        RecordStatus= p.RecordStatus
                                    }).FirstOrDefault();

                model.PurchaseInformation = PurchaseInfo;

                var ChallanInfo = (from c in _context.Prq_PurchaseChallan.AsEnumerable()
                                   where (c.PurchaseID).ToString() == PurchaseNumber
                                   from s in _context.Sys_Source
                                   where s.SourceID == c.SourceID
                                   from l in _context.Sys_Location
                                   where l.LocationID == c.LocationID
                                   orderby c.ChallanID
                                   select new PrqPurchaseChallan
                                   {
                                       ChallanID = c.ChallanID,
                                       ChallanNo = c.ChallanNo,
                                       ChallanDate = (c.ChallanDate).ToString("dd'/'MM'/'yyyy"),
                                       SourceID = c.SourceID,
                                       SourceName = s.SourceName,
                                       LocationID = c.LocationID,
                                       LocationName = l.LocationName,
                                       ChallanCategory = c.ChallanCategory,
                                       ReceiveStore = DalCommon.GetStoreName(c.ReceiveStore),
                                       ChallanNote = c.ChallanNote,
                                       RecordStatus= DalCommon.ReturnRecordStatus(c.RecordStatus)
                                   }).ToList();

                model.ChallanList = ChallanInfo;


                if(ChallanInfo.Count > 0)
                {
                    if ((ChallanInfo.FirstOrDefault().ChallanID).ToString() != null)
                    {
                        var ChallanItemInfoTest = (from c in _context.Prq_PurchaseChallanItem.AsEnumerable()
                                                   where c.ChallanID == ChallanInfo.FirstOrDefault().ChallanID
                                                   select new PrqPurchaseChallanItem
                                                   {
                                                       ChallanItemID = c.ChallanItemID,
                                                       ChallanID = c.ChallanID,
                                                       ItemTypeName = DalCommon.GetItemTypeName(c.ItemTypeID),
                                                       SizeName = DalCommon.GetSizeName(c.ItemSizeID),
                                                       UnitName = DalCommon.GetUnitName(c.UnitID),
                                                       Description = c.Description,
                                                       ChallanQty = c.ChallanQty,
                                                       ReceiveQty = c.ReceiveQty,
                                                       Remark = c.Remark,
                                                       RecordStatus = DalCommon.ReturnRecordStatus(c.RecordStatus)
                                                   });
                        model.ChallanItemList = ChallanItemInfoTest.ToList();
                    }
                }
                
                return model;
        }

        public List<PrqPurchaseChallanItem> GetItemListForChallan(string ChallanID)
        {
            var ItemList = (from c in _context.Prq_PurchaseChallanItem.AsEnumerable()
                                where (c.ChallanID).ToString() == ChallanID
                                select new PrqPurchaseChallanItem
                                {
                                    ChallanItemID = c.ChallanItemID,
                                    ChallanID = c.ChallanID,
                                    ItemTypeName = DalCommon.GetItemTypeName(c.ItemTypeID),
                                    SizeName = DalCommon.GetSizeName(c.ItemSizeID),
                                    UnitName = DalCommon.GetUnitName(c.UnitID),
                                    Description = c.Description,
                                    ChallanQty = c.ChallanQty,
                                    ReceiveQty = c.ReceiveQty,
                                    Remark = c.Remark,
                                    RecordStatus = DalCommon.ReturnRecordStatus(c.RecordStatus)
                                });

                return ItemList.ToList();
        }

        public List<PrqPurchaseChallanItem> GetItemListForChallan2(string ChallanID)
        {
            using (var Context = new BLC_DEVEntities())
            {
                var ItemList = (from c in Context.Prq_PurchaseChallanItem.AsEnumerable()
                                where (c.ChallanID).ToString() == ChallanID
                                select new PrqPurchaseChallanItem
                                {
                                    ChallanItemID = c.ChallanItemID,
                                    ChallanID = c.ChallanID,
                                    ItemTypeID = c.ItemTypeID.ToString(CultureInfo.InvariantCulture),
                                    ItemSizeID = c.ItemSizeID.ToString(CultureInfo.InvariantCulture),
                                    UnitID = c.UnitID.ToString(CultureInfo.InvariantCulture),
                                    Description = c.Description,
                                    ChallanQty = c.ChallanQty,
                                    ReceiveQty = c.ReceiveQty,
                                    Remark = c.Remark,
                                    RecordStatus = DalCommon.ReturnRecordStatus(c.RecordStatus)
                                });

                return ItemList.ToList();
            }
        }

        public List<PrqPurchaseChallan> GetChallanList(long purchaseID)
        {
            var challanList = (from c in _context.Prq_PurchaseChallan.AsEnumerable()
                                   where c.PurchaseID == purchaseID
                               from s in _context.Sys_Source
                                   where s.SourceID == c.SourceID
                               from l in _context.Sys_Location
                                   where l.LocationID == c.LocationID
                                   orderby c.ChallanID
                                   select new PrqPurchaseChallan
                                   {
                                       ChallanID = c.ChallanID,
                                       ChallanNo = c.ChallanNo,
                                       ChallanDate = (c.ChallanDate).ToString("dd'/'MM'/'yyyy"),
                                       SourceID = c.SourceID,
                                       SourceName = s.SourceName,
                                       LocationID = c.LocationID,
                                       LocationName = l.LocationName,
                                       ChallanCategory = c.ChallanCategory,
                                       ReceiveStore = DalCommon.GetStoreName(c.ReceiveStore),
                                       ChallanNote = c.ChallanNote,
                                       RecordStatus = DalCommon.ReturnRecordStatus(c.RecordStatus)
                                   }).ToList();

                return challanList;
        }

        public bool DeleteChallanItem(string ChallanItemID)
        {
            try
            {
                var ChallanItem = (from c in _context.Prq_PurchaseChallanItem.AsEnumerable()
                                   where c.ChallanItemID == Convert.ToInt64(ChallanItemID)
                                   select c).FirstOrDefault();

                _context.Prq_PurchaseChallanItem.Remove(ChallanItem);
                _context.SaveChanges();

                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public bool DeleteChallan(string ChallanID)
        {
            try
            {
                var Challan = (from c in _context.Prq_PurchaseChallan.AsEnumerable()
                                   where c.ChallanID == Convert.ToInt64(ChallanID)
                                   select c).FirstOrDefault();

                _context.Prq_PurchaseChallan.Remove(Challan);
                _context.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool DeletePurchase(string PurchaseID)
        {
            try
            {
                var Purchase = (from c in _context.Prq_Purchase.AsEnumerable()
                               where c.PurchaseID == Convert.ToInt64(PurchaseID)
                               select c).FirstOrDefault();

                _context.Prq_Purchase.Remove(Purchase);
                _context.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

    }
}
