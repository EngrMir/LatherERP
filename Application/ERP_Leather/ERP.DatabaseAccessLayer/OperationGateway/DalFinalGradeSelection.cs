using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalFinalGradeSelection
    {
        private UnitOfWork repository = new UnitOfWork();
        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;
        public DalFinalGradeSelection()
        {
            _vmMsg = new ValidationMsg();
            _context = new BLC_DEVEntities();
        }
        public object GetPurchaseInfo()
        {
            var data = _context.Database.SqlQuery<UspWBSelectionData>("EXEC UspGetWetBlueProductionData");//_context.UspGetWetBlueProductionData().ToList();

            int productionQty = Convert.ToInt32((from temp in data select temp.TotalProductionQty).FirstOrDefault());
            var result = from temp in data
                         select new
                         {
                             TransectionID = temp.PurchaseID,
                             SupplierID = temp.SupplierID,
                             PurchaseID = temp.PurchaseID,
                             ItemTypeID = temp.ItemTypeID,
                             LeatherTypeID = "",
                             LeatherStatusID = temp.LeatherStatusID,
                             PurchaseNo = temp.PurchaseNo,
                             PurchaseDate = Convert.ToDateTime(temp.PurchaseDate).ToString("dd/MM/yyyy"),
                             PurchaseQty = temp.PurchaseQty,
                             ProductionQty = productionQty,
                             ProductionDue = (temp.PurchaseQty - productionQty),
                             SelectionComplete = temp.SelectionComplete,
                             SelectionDue = temp.SelectionDue,
                             StoreID = temp.StoreID,
                             StoreName = temp.StoreName,
                             ItemTypeName = temp.ItemTypeName,
                             LeatherStatusName = temp.LeatherStatusName,
                             UnitName = temp.UnitName,
                             UnitID = temp.UnitID,
                             ClosingProductionkPcs = temp.ProductionQty,
                             ClosingProductionSide = temp.ClosingProductionSide,
                             ClosingProductionArea = temp.ClosingProductionArea
                         };
            return result;
        }

        public object SearchPurchaseByFirstName(string search, int SupplierID, int ConcernStore)
        {
            var query = @"SELECT T1.PurchaseID,T3.PurchaseNo,ISNULL(T3.ReceiveQty,0) PurchaseQty,ISNULL(T1.ProductionQty,0) ProductionQty,(ISNULL(T3.ReceiveQty,0) -ISNULL(T1.ProductionQty,0)) ProductionDue ,ISNULL(T2.SelectionQty,0) SelectionQty, (ISNULL(T1.ProductionQty,0) -ISNULL(T2.SelectionQty,0)) SelectionDueQty,T1.ItemTypeID,T1.LeatherStatusID  FROM (
                            SELECT  ps.StoreID,ps.SupplierID,ps.PurchaseID,ps.ItemTypeID,ps.LeatherStatusID, SUM(ReceivePcs) ProductionQty FROM PRD_WetBlueProductionStock ps 
                            WHERE ps.SupplierID=" + SupplierID + " AND StoreID=" + ConcernStore + " GROUP BY ps.StoreID,ps.SupplierID,ps.PurchaseID,ps.ItemTypeID,ps.LeatherStatusID) T1 LEFT JOIN (SELECT  ps.StoreID,ps.SupplierID,ps.PurchaseID,ps.ItemTypeID,ps.LeatherStatusID, SUM(ps.IssuePcs) SelectionQty FROM PRD_WetBlueProductionStock ps WHERE ps.SupplierID=" + SupplierID + " AND StoreID=" + ConcernStore + " GROUP BY ps.StoreID,ps.SupplierID,ps.PurchaseID,ps.ItemTypeID,ps.LeatherStatusID) T2 ON T1.StoreID = T2.StoreID AND T1.SupplierID = T2.SupplierID AND T1.PurchaseID = T2.PurchaseID AND T1.LeatherStatusID = T2.LeatherStatusID AND T1.ItemTypeID = T2.ItemTypeID INNER JOIN ( SELECT p.purchaseID,p.PurchaseNo,pci.ItemTypeID,SUM(pci.ReceiveQty) ReceiveQty FROM Prq_Purchase p INNER JOIN Prq_PurchaseChallan pc ON p.PurchaseID = pc.PurchaseID INNER JOIN Prq_PurchaseChallanItem  pci ON pc.ChallanID =pci.ChallanID WHERE p.SupplierID=" + SupplierID + " GROUP BY p.PurchaseID,p.PurchaseNo,pci.ItemTypeID) T3 ON T1.PurchaseID = T3.PurchaseID AND T1.ItemTypeID = T3.ItemTypeID";

            var result = _context.Database.SqlQuery<wbshowField>(query).ToList();
            if (search == null)
            {
                return result.Select(c => SetToBussinessObject(c)).ToList<wbshowField>();
            }
            else
            {
                search = search.ToUpper();
                var data2 = from temp in result
                            where ((temp.PurchaseNo.ToString().ToUpper().StartsWith(search) || temp.PurchaseNo.ToString().ToUpper() == search))
                            select temp;
                return data2.Select(c => SetToBussinessObject(c)).ToList<wbshowField>();
            }

        }

        public object GetPurchaseAutocompleteData()
        {
            var query = @"SELECT T1.PurchaseID,T3.PurchaseNo,ISNULL(T3.ReceiveQty,0) PurchaseQty,ISNULL(T1.ProductionQty,0) ProductionQty,(ISNULL(T3.ReceiveQty,0) -ISNULL(T1.ProductionQty,0)) ProductionDue ,ISNULL(T2.SelectionQty,0) SelectionQty, (ISNULL(T1.ProductionQty,0) -ISNULL(T2.SelectionQty,0)) SelectionDueQty,T1.ItemTypeID,T1.LeatherStatusID  FROM (
                          SELECT  ps.StoreID,ps.SupplierID,ps.PurchaseID,ps.ItemTypeID,ps.LeatherStatusID, SUM(ReceivePcs) ProductionQty FROM PRD_WetBlueProductionStock ps 
                          WHERE ps.SupplierID >0 AND StoreID > 0 
                          GROUP BY ps.StoreID,ps.SupplierID,ps.PurchaseID,ps.ItemTypeID,ps.LeatherStatusID) T1 
                          LEFT JOIN (SELECT  ps.StoreID,ps.SupplierID,ps.PurchaseID,ps.ItemTypeID,ps.LeatherStatusID, SUM(ps.IssuePcs) SelectionQty FROM PRD_WetBlueProductionStock ps WHERE ps.SupplierID > 0 AND StoreID >0 GROUP BY ps.StoreID,ps.SupplierID,ps.PurchaseID,ps.ItemTypeID,ps.LeatherStatusID) T2 ON T1.StoreID = T2.StoreID AND T1.SupplierID = T2.SupplierID AND T1.PurchaseID = T2.PurchaseID AND T1.LeatherStatusID = T2.LeatherStatusID AND T1.ItemTypeID = T2.ItemTypeID INNER JOIN ( SELECT p.purchaseID,p.PurchaseNo,pci.ItemTypeID,SUM(pci.ReceiveQty) ReceiveQty FROM Prq_Purchase p INNER JOIN Prq_PurchaseChallan pc ON p.PurchaseID = pc.PurchaseID INNER JOIN Prq_PurchaseChallanItem  pci ON pc.ChallanID =pci.ChallanID WHERE p.SupplierID >0 GROUP BY p.PurchaseID,p.PurchaseNo,pci.ItemTypeID) T3 
                          ON T1.PurchaseID = T3.PurchaseID AND T1.ItemTypeID = T3.ItemTypeID";
            var result = from t in _context.Database.SqlQuery<wbshowField>(query).ToList() select new { PurchaseNo = t.PurchaseNo };
            return result;

        }

        public object GetItemTypeInfo(long purchaseID)
        {
            var result = _context.UspGetFinalGradeSelectionItemType(purchaseID).ToList();
            var data = from temp in result
                       select new
                       {
                           ItemTypeID = temp.ItemTypeID,
                           ItemTypeName = temp.ItemTypeName,
                           LeatherStatusID = temp.LeatherStatusID,
                           LeatherStatusName = temp.LeatherStatusName,
                           ClosingProductionkPcs = temp.ClosingProductionkPcs,
                           ClosingProductionSide = temp.ClosingProductionSide,
                           ClosingProductionArea = temp.ClosingProductionArea,
                           UnitID = temp.UnitID,
                           UnitName = temp.UnitName
                       };

            return data;
        }

        public object GetSelectionInfo(int supplierID, long purchaseID, int itemTypeID, int leatherTypeID, int leatherStatusId)
        {
            var data = from temp in _context.UspGetFinalGradeSelectedGrde(supplierID, purchaseID, itemTypeID, leatherTypeID, leatherStatusId)
                       select new
                       {
                           ItemID = temp.WBSelectItemID,
                           GradeID = temp.GradeID,
                           GradeName = temp.GradeName,
                           // GradeQuantity = temp.GradeQuantity,
                           GradeSide = temp.GradeSide,
                           GradeArea = temp.GradeArea,
                           UnitID = temp.UnitID,
                           UnitName = temp.UnitName
                       };
            return data;
        }

        public object SearchItemTypeByFirstName(string search, long purchaseID)
        {
            search = search.ToUpper();
            var result = _context.UspGetFinalGradeSelectionItemType(purchaseID).ToList();
            var data = from temp in result
                       where ((temp.ItemTypeName.ToUpper().StartsWith(search) || temp.ItemTypeName.ToUpper() == search))
                       select new
                       {
                           ItemTypeID = temp.ItemTypeID,
                           ItemTypeName = temp.ItemTypeName,
                           LeatherStatusID = temp.LeatherStatusID,
                           LeatherStatusName = temp.LeatherStatusName,
                           ClosingProductionkPcs = temp.ClosingProductionkPcs,
                           ClosingProductionSide = temp.ClosingProductionSide,
                           ClosingProductionArea = temp.ClosingProductionArea,
                           ProductionUnitID = temp.UnitID,
                           ProductionAreaUnit = temp.UnitName
                       };

            return data;
        }
        public ValidationMsg Save(wbSelection data, int userID)
        {
            using (var tx = new TransactionScope())
            {
                using (_context)
                {
                    //decimal receiveStockPcs = 0;
                    //decimal receiveStockSide = 0;
                    //decimal receiveStockArea = 0;
                    int supplierID = 0;
                    PRD_WBSelection obSelection = new PRD_WBSelection();
                    obSelection.WBSelectionNo = DalCommon.GetPreDefineNextCodeByUrl("FinalGradeSelection/FinalGrdSelection");
                    obSelection.SelectionDate = DalCommon.SetDate(data.SelectionDate);
                    obSelection.SelectedBy = data.SelectedBy;
                    obSelection.SelectionComments = data.SelectionComments;
                    obSelection.Remarks = data.Remarks;
                    obSelection.RecordStatus = "NCF";
                    obSelection.RecordState = "ATS";
                    obSelection.SetOn = DateTime.Now;
                    obSelection.SetBy = userID;
                    obSelection.ModifiedOn = DateTime.Now;
                    obSelection.ModifiedBy = userID;
                    obSelection.IPAddress = GetIPAddress.LocalIPAddress();
                    obSelection.StoreID = data.StoreID;
                    obSelection.AverageThickness = data.AverageThickness;
                    obSelection.LessCut = data.LessCut;
                    obSelection.GrainOff = data.GrainOff;
                    if (data.ProductionDate.ToString() != null) { obSelection.ProductionDate = Convert.ToDateTime(data.ProductionDate); }
                    _context.PRD_WBSelection.Add(obSelection);
                    _context.SaveChanges();

                    long wbSelectionID = obSelection.WBSelectionID;
                    foreach (var item in data.wbSelectionItem)
                    {
                        supplierID = item.SupplierID;//repository.SysSupplierRepository.Get(filter: ob => ob.SupplierCode == item.SupplierCode).FirstOrDefault().SupplierID;
                        PRD_WBSelectionItem obSelectionItem = new PRD_WBSelectionItem();
                        obSelectionItem.WBSelectItemNo = item.WBSelectItemNo;
                        obSelectionItem.WBSelectionID = wbSelectionID;
                        obSelectionItem.SupplierID = supplierID;
                        obSelectionItem.PurchaseID = Convert.ToInt32(item.PurchaseID);
                        obSelectionItem.ItemTypeID = Convert.ToByte(item.ItemTypeID);
                        //obSelectionItem.LeatherTypeID = 22;
                        obSelectionItem.LeatherStatusID = Convert.ToByte(item.LeatherStatusID);
                        obSelectionItem.ProductionPcs = Convert.ToDecimal(item.ClosingProductionkPcs);
                        obSelectionItem.ProductionSide = Convert.ToDecimal(item.ClosingProductionSide);
                        obSelectionItem.ProductionArea = Convert.ToDecimal(item.ClosingProductionArea);
                        obSelectionItem.ProductionAreaUnit = Convert.ToByte(item.UnitID);
                        obSelectionItem.Remarks = item.Remarks;
                        obSelectionItem.SetOn = DateTime.Now;
                        obSelectionItem.SetBy = userID;
                        obSelectionItem.ModifiedOn = DateTime.Now;
                        obSelectionItem.ModifiedBy = userID;
                        obSelectionItem.IPAddress = GetIPAddress.LocalIPAddress();
                        repository.PrdWBSelectionItemRepository.Insert(obSelectionItem);

                        foreach (var item2 in item.wbSelectionGrade)
                        {
                            if (item2.WBSelectionGradeID == 0 && (item2.SizeQty1 > 0 || item2.SizeQty2 > 0
                                   || item2.SizeQty3 > 0 || item2.SizeQty4 > 0 || item2.SizeQty5 > 0
                                   || item2.SizeQty6 > 0 || item2.SizeQty7 > 0 || item2.SizeQty8 > 0 || item2.SizeQty9 > 0
                                   || item2.SizeQty10 > 0 || item2.GradeSide > 0 || item2.GradeArea > 0))
                            {
                                PRD_WBSelectionGrade obSelectionGrade = new PRD_WBSelectionGrade();
                                obSelectionGrade.WBSelectItemID = obSelectionItem.WBSelectItemID;
                                obSelectionGrade.WBSelectItemNo = item.WBSelectItemNo;
                                obSelectionGrade.WBSelectionID = wbSelectionID;
                                obSelectionGrade.GradeID = Convert.ToInt16(item2.GradeID);
                                obSelectionGrade.SizeQty1 = Convert.ToDecimal(item2.SizeQty1 == null ? 0 : item2.SizeQty1);
                                obSelectionGrade.SizeQty2 = Convert.ToDecimal(item2.SizeQty2 == null ? 0 : item2.SizeQty2);
                                obSelectionGrade.SizeQty3 = Convert.ToDecimal(item2.SizeQty3 == null ? 0 : item2.SizeQty3);
                                obSelectionGrade.SizeQty4 = Convert.ToDecimal(item2.SizeQty4 == null ? 0 : item2.SizeQty4);
                                obSelectionGrade.SizeQty5 = Convert.ToDecimal(item2.SizeQty5 == null ? 0 : item2.SizeQty5);
                                obSelectionGrade.SizeQty6 = Convert.ToDecimal(item2.SizeQty6 == null ? 0 : item2.SizeQty6);
                                obSelectionGrade.SizeQty7 = Convert.ToDecimal(item2.SizeQty7 == null ? 0 : item2.SizeQty7);
                                obSelectionGrade.SizeQty8 = Convert.ToDecimal(item2.SizeQty8 == null ? 0 : item2.SizeQty8);
                                obSelectionGrade.SizeQty9 = Convert.ToDecimal(item2.SizeQty9 == null ? 0 : item2.SizeQty9);
                                obSelectionGrade.SizeQty10 = Convert.ToDecimal(item2.SizeQty10 == null ? 0 : item2.SizeQty10);
                                obSelectionGrade.GradeSide = Convert.ToDecimal(item2.GradeSide == null ? 0 : item2.GradeSide);                                
                                obSelectionGrade.GradeArea = Convert.ToDecimal(item2.GradeArea == null ? 0 : item2.GradeArea);                                
                                obSelectionGrade.AreaUnitID = Convert.ToByte(item2.AreaUnitID);// repository.SysUnitRepository.Get(ob => ob.UnitID == (item2.AreaUnitID == null ? 0 : item2.AreaUnitID)).FirstOrDefault().UnitID; // Convert.ToByte(item2.AreaUnitID == null ? "" : item2.AreaUnitID);
                                obSelectionGrade.Remarks = "";
                                obSelectionGrade.SetOn = DateTime.Now;
                                obSelectionGrade.SetBy = userID;
                                obSelectionGrade.ModifiedOn = DateTime.Now;
                                obSelectionGrade.ModifiedBy = userID;
                                obSelectionGrade.IPAddress = GetIPAddress.LocalIPAddress();
                                repository.PrdWBSelectionGradeRepository.Insert(obSelectionGrade);
                            }
                        }

                        try
                        {

                            _vmMsg.ReturnCode = obSelection.WBSelectionNo.ToString();
                            _vmMsg.ReturnId = _context.PRD_WBSelection.Max(d => d.WBSelectionID);//_context.PRD_WBSelection.Max().WBSelectionID.ToString();                          
                            repository.Save();
                            tx.Complete();
                            _vmMsg.Type = Enums.MessageType.Success;
                            _vmMsg.Msg = "Saved Successfully.";

                        }
                        catch (Exception ex)
                        {
                            _vmMsg.Type = Enums.MessageType.Error;
                            _vmMsg.Msg = "Saved Faild.";

                        }
                    }
                }
            }
            return _vmMsg;
        }


        public ValidationMsg Update(wbSelection data, int userID, long selectionId)
        {
            using (var tx = new TransactionScope())
            {
                using (_context)
                {
                    decimal receiveStockPcs = 0;
                    decimal receiveStockSide = 0;
                    decimal receiveStockArea = 0;
                    int supplierID = 0;
                    PRD_WBSelection obSelection = (from temp in _context.PRD_WBSelection where temp.WBSelectionID == selectionId select temp).FirstOrDefault();
                    if (obSelection.RecordStatus != "CNF")
                    {
                        obSelection.SelectionDate = DalCommon.SetDate(data.SelectionDate);
                        obSelection.SelectedBy = data.SelectedBy;
                        obSelection.SelectionComments = data.SelectionComments;
                        obSelection.ModifiedOn = DateTime.Now;
                        obSelection.ModifiedBy = userID;
                        obSelection.IPAddress = GetIPAddress.LocalIPAddress();
                        obSelection.StoreID = data.StoreID;
                        obSelection.AverageThickness = data.AverageThickness;
                        obSelection.LessCut = data.LessCut;
                        obSelection.GrainOff = data.GrainOff;
                        if (data.ProductionDate.ToString() != null) { obSelection.ProductionDate = Convert.ToDateTime(data.ProductionDate); }
                        _context.SaveChanges();

                        long wbSelectionID = obSelection.WBSelectionID;
                        foreach (var item in data.wbSelectionItem)
                        {
                            supplierID = item.SupplierID;//repository.SysSupplierRepository.Get(filter: ob => ob.SupplierCode == item.SupplierCode).FirstOrDefault().SupplierID;
                            PRD_WBSelectionItem obSelectionItem = (from t in _context.PRD_WBSelectionItem where t.WBSelectionID == wbSelectionID select t).FirstOrDefault();
                            if (obSelectionItem != null)
                            {
                                obSelectionItem.SupplierID = supplierID;
                                obSelectionItem.PurchaseID = Convert.ToInt32(item.PurchaseID);
                                obSelectionItem.ItemTypeID = Convert.ToByte(item.ItemTypeID);
                                obSelectionItem.LeatherStatusID = Convert.ToByte(item.LeatherStatusID);
                                obSelectionItem.ProductionPcs = item.ClosingProductionkPcs == null ? 0 : Convert.ToDecimal(item.ClosingProductionkPcs);
                                obSelectionItem.ProductionSide = item.ClosingProductionSide == null ? 0 : Convert.ToDecimal(item.ClosingProductionSide);
                                obSelectionItem.ProductionArea = item.ClosingProductionArea == null ? 0 : Convert.ToDecimal(item.ClosingProductionArea);
                                if (item.UnitID == null)
                                {
                                }
                                else
                                {
                                    obSelectionItem.ProductionAreaUnit = Convert.ToByte(item.UnitID);
                                }

                                obSelectionItem.ModifiedOn = DateTime.Now;
                                obSelectionItem.ModifiedBy = userID;
                                obSelectionItem.IPAddress = GetIPAddress.LocalIPAddress();
                                _context.SaveChanges();
                            }
                            foreach (var item2 in item.wbSelectionGrade)
                            {
                                if (item2.WBSelectionGradeID == 0 && (item2.SizeQty1 > 0 || item2.SizeQty2 > 0
                                    || item2.SizeQty3 > 0 || item2.SizeQty4 > 0 || item2.SizeQty5 > 0
                                    || item2.SizeQty6 > 0 || item2.SizeQty7 > 0 || item2.SizeQty8 > 0 || item2.SizeQty9 > 0
                                    || item2.SizeQty10 > 0 || item2.GradeSide > 0 || item2.GradeArea > 0))
                                {
                                    PRD_WBSelectionGrade ob = new PRD_WBSelectionGrade();
                                    ob.WBSelectItemID = obSelectionItem.WBSelectItemID;
                                    ob.WBSelectItemNo = item.WBSelectItemNo;
                                    ob.WBSelectionID = wbSelectionID;
                                    ob.GradeID = Convert.ToInt16(item2.GradeID);
                                    ob.GradeQty = Convert.ToDecimal(item2.GradeQty == null ? 0 : item2.GradeQty);

                                    ob.SizeQty1 = Convert.ToDecimal(item2.SizeQty1 == null ? 0 : item2.SizeQty1);
                                    ob.SizeQty2 = Convert.ToDecimal(item2.SizeQty2 == null ? 0 : item2.SizeQty2);
                                    ob.SizeQty3 = Convert.ToDecimal(item2.SizeQty3 == null ? 0 : item2.SizeQty3);
                                    ob.SizeQty4 = Convert.ToDecimal(item2.SizeQty4 == null ? 0 : item2.SizeQty4);
                                    ob.SizeQty5 = Convert.ToDecimal(item2.SizeQty5 == null ? 0 : item2.SizeQty5);
                                    ob.SizeQty6 = Convert.ToDecimal(item2.SizeQty6 == null ? 0 : item2.SizeQty6);
                                    ob.SizeQty7 = Convert.ToDecimal(item2.SizeQty7 == null ? 0 : item2.SizeQty7);
                                    ob.SizeQty8 = Convert.ToDecimal(item2.SizeQty8 == null ? 0 : item2.SizeQty8);
                                    ob.SizeQty9 = Convert.ToDecimal(item2.SizeQty9 == null ? 0 : item2.SizeQty9);
                                    ob.SizeQty10 = Convert.ToDecimal(item2.SizeQty10 == null ? 0 : item2.SizeQty10);

                                    receiveStockPcs += Convert.ToDecimal(ob.GradeQty);
                                    ob.GradeSide = Convert.ToDecimal(item2.GradeSide == null ? 0 : item2.GradeSide);
                                    receiveStockSide += Convert.ToDecimal(ob.GradeSide);
                                    ob.GradeArea = Convert.ToDecimal(item2.GradeArea == null ? 0 : item2.GradeArea);
                                    receiveStockArea += Convert.ToDecimal(ob.GradeArea);
                                    if (item.UnitID == null)
                                    {
                                    }
                                    else
                                    {
                                        ob.AreaUnitID = Convert.ToByte(item2.AreaUnitID);
                                    }

                                    ob.Remarks = "";
                                    ob.SetOn = DateTime.Now;
                                    ob.SetBy = userID;
                                    ob.ModifiedOn = DateTime.Now;
                                    ob.ModifiedBy = userID;
                                    ob.IPAddress = GetIPAddress.LocalIPAddress();
                                    if (item2.SizeID != null) { ob.SizeID = item2.SizeID; }
                                    repository.PrdWBSelectionGradeRepository.Insert(ob);
                                    repository.Save();
                                }
                                else if (item2.WBSelectionGradeID > 0 && (item2.SizeQty1 > 0 || item2.SizeQty2 > 0
                                    || item2.SizeQty3 > 0 || item2.SizeQty4 > 0 || item2.SizeQty5 > 0
                                    || item2.SizeQty6 > 0 || item2.SizeQty7 > 0 || item2.SizeQty8 > 0 || item2.SizeQty9 > 0
                                    || item2.SizeQty10 > 0 || item2.GradeSide > 0 || item2.GradeArea > 0))
                                {
                                    PRD_WBSelectionGrade obSelectionGrade = (from t in _context.PRD_WBSelectionGrade where t.WBSelectionGradeID == item2.WBSelectionGradeID select t).FirstOrDefault();
                                    obSelectionGrade.GradeID = Convert.ToInt16(item2.GradeID);
                                    obSelectionGrade.GradeQty = Convert.ToDecimal(item2.GradeQty == null ? 0 : item2.GradeQty);
                                    receiveStockPcs += Convert.ToDecimal(obSelectionGrade.GradeQty);

                                    obSelectionGrade.SizeQty1 = Convert.ToDecimal(item2.SizeQty1 == null ? 0 : item2.SizeQty1);
                                    obSelectionGrade.SizeQty2 = Convert.ToDecimal(item2.SizeQty2 == null ? 0 : item2.SizeQty2);
                                    obSelectionGrade.SizeQty3 = Convert.ToDecimal(item2.SizeQty3 == null ? 0 : item2.SizeQty3);
                                    obSelectionGrade.SizeQty4 = Convert.ToDecimal(item2.SizeQty4 == null ? 0 : item2.SizeQty4);
                                    obSelectionGrade.SizeQty5 = Convert.ToDecimal(item2.SizeQty5 == null ? 0 : item2.SizeQty5);
                                    obSelectionGrade.SizeQty6 = Convert.ToDecimal(item2.SizeQty6 == null ? 0 : item2.SizeQty6);
                                    obSelectionGrade.SizeQty7 = Convert.ToDecimal(item2.SizeQty7 == null ? 0 : item2.SizeQty7);
                                    obSelectionGrade.SizeQty8 = Convert.ToDecimal(item2.SizeQty8 == null ? 0 : item2.SizeQty8);
                                    obSelectionGrade.SizeQty9 = Convert.ToDecimal(item2.SizeQty9 == null ? 0 : item2.SizeQty9);
                                    obSelectionGrade.SizeQty10 = Convert.ToDecimal(item2.SizeQty10 == null ? 0 : item2.SizeQty10);


                                    obSelectionGrade.GradeSide = Convert.ToDecimal(item2.GradeSide == null ? 0 : item2.GradeSide);
                                    receiveStockSide += Convert.ToDecimal(obSelectionGrade.GradeSide);
                                    obSelectionGrade.GradeArea = Convert.ToDecimal(item2.GradeArea == null ? 0 : item2.GradeArea);
                                    receiveStockArea += Convert.ToDecimal(obSelectionGrade.GradeArea);
                                    obSelectionGrade.AreaUnitID = Convert.ToByte(item2.AreaUnitID);
                                    obSelectionGrade.ModifiedOn = DateTime.Now;
                                    obSelectionGrade.ModifiedBy = userID;
                                    obSelectionGrade.IPAddress = GetIPAddress.LocalIPAddress();
                                    if (item2.SizeID != null) { obSelectionGrade.SizeID = item2.SizeID; }

                                    _context.SaveChanges();
                                }
                                else
                                {

                                }
                            }

                            try
                            {
                                _vmMsg.ReturnCode = obSelection.WBSelectionNo.ToString();
                                _vmMsg.ReturnId = selectionId;
                                tx.Complete();
                                _vmMsg.Type = Enums.MessageType.Success;
                                _vmMsg.Msg = "Update Successfully.";

                            }
                            catch (Exception ex)
                            {
                                _vmMsg.Type = Enums.MessageType.Error;
                                _vmMsg.Msg = "Update Faild.";
                            }
                        }
                    }
                    else
                    {
                        _vmMsg.Type = Enums.MessageType.Error;
                        _vmMsg.Msg = "Data Already Confirmed.";
                    }
                }
            }
            return _vmMsg;
        }

        public int UpdateProductionStock(wbSelection dataSet)
        {

            if (dataSet == null) { return 0; }
            var wbSelectionItem = from t in dataSet.wbSelectionItem select t;
            // Selection Stock

            foreach (var item in wbSelectionItem)
            {
            
                byte UnitID = Convert.ToByte(dataSet.wbSelectionItem.Select(ob => ob.UnitID).FirstOrDefault());
             

                // ############## Insert Data into Production Stock Start ##########################
                PRD_WetBlueProductionStock wbProdStck = new PRD_WetBlueProductionStock();
                wbProdStck.SupplierID = item.SupplierID;
                wbProdStck.PurchaseID = Convert.ToInt64(item.PurchaseID);
                wbProdStck.ItemTypeID = Convert.ToByte(item.ItemTypeID);
                wbProdStck.StoreID = Convert.ToByte(item.StoreID);
                wbProdStck.LeatherTypeID = (from t in _context.PRD_WetBlueProductionStock where t.SupplierID == wbProdStck.SupplierID && t.PurchaseID == wbProdStck.PurchaseID && t.ItemTypeID == wbProdStck.ItemTypeID && t.StoreID == wbProdStck.StoreID && (t.ReceivePcs > 0 || t.ReceiveSide > 0) select t.LeatherTypeID).FirstOrDefault();
                wbProdStck.LeatherStatusID = (from t in _context.PRD_WetBlueProductionStock where t.SupplierID == wbProdStck.SupplierID && t.PurchaseID == wbProdStck.PurchaseID && t.ItemTypeID == wbProdStck.ItemTypeID && t.StoreID == wbProdStck.StoreID && (t.ReceivePcs > 0 || t.ReceiveSide > 0) select t.LeatherStatusID).FirstOrDefault();//Convert.ToByte(item.LeatherStatusID);                  

                PRD_WetBlueProductionStock checkProdExist = (from t in _context.PRD_WetBlueProductionStock
                                                             where (t.StoreID == wbProdStck.StoreID && t.SupplierID == wbProdStck.SupplierID &&
                                                                t.PurchaseID == wbProdStck.PurchaseID && t.ItemTypeID == wbProdStck.ItemTypeID && t.LeatherStatusID == wbProdStck.LeatherStatusID && t.LeatherTypeID == wbProdStck.LeatherTypeID)
                                                             select t).OrderByDescending(m => m.TransectionID).FirstOrDefault();
             

                if (checkProdExist == null)
                {
                    return 0;
                }
                else
                {
                    wbProdStck.OpeningPcs = Convert.ToDecimal(checkProdExist.ClosingProductionkPcs == null ? 0 : checkProdExist.ClosingProductionkPcs);
                    wbProdStck.OpeningSide = Convert.ToDecimal(checkProdExist.ClosingProductionSide == null ? 0 : checkProdExist.ClosingProductionSide);
                    wbProdStck.OpeningArea = Convert.ToDecimal(checkProdExist.ClosingProductionArea == null ? 0 : checkProdExist.ClosingProductionArea);
                    wbProdStck.ReceivePcs = 0;
                    wbProdStck.ReceiveSide = 0;
                    wbProdStck.ReceiveArea = 0;
                    wbProdStck.IssuePcs = Convert.ToDecimal(dataSet.wbSelectionItem.Select(ob => ob.ClosingProductionkPcs).FirstOrDefault());
                    wbProdStck.IssueSide = Convert.ToDecimal(dataSet.wbSelectionItem.Select(ob => ob.ClosingProductionSide).FirstOrDefault());
                    wbProdStck.IssueArea = 0;
                    wbProdStck.ClosingProductionkPcs = Convert.ToDecimal(wbProdStck.OpeningPcs - wbProdStck.IssuePcs);
                    wbProdStck.ClosingProductionSide = Convert.ToDecimal(wbProdStck.OpeningSide - wbProdStck.IssueSide);
                    wbProdStck.ClosingProductionArea = 0;
                    if (wbProdStck.ClosingProductionkPcs < 0 || wbProdStck.ClosingProductionSide < 0)
                    {
                        return 0;
                    }
                    wbProdStck.AreaUnit = UnitID;
                    repository.PrdWetBlueProductionStockRepo.Insert(wbProdStck);
                }


                //   // ############## Insert Data into Selection Stock Start ##########################
                var wbSelectionGrade = from t in item.wbSelectionGrade select t;
                foreach (var itemGrade in wbSelectionGrade)
                {
                    int i = 1;
                    string sizeQtyRef = "";
                    for (i = 1; i <= 10; i++)
                    {
                        sizeQtyRef = "SizeQty" + i;
                        if (CheckDataValidity(sizeQtyRef, itemGrade))
                        {                           
                            INV_WetBlueSelectionStock obSelStock = new INV_WetBlueSelectionStock();
                            obSelStock.StoreID = item.StoreID;
                            obSelStock.SupplierID = item.SupplierID;
                            obSelStock.PurchaseID = item.PurchaseID;
                            obSelStock.ItemTypeID = Convert.ToByte(item.ItemTypeID);
                            obSelStock.LeatherStatusID =  Convert.ToByte(item.LeatherStatusID);
                            obSelStock.SelectionStatus = "CNF";
                            obSelStock.SizeQtyRef = sizeQtyRef;
                            INV_WetBlueSelectionStock checkExist = (from t in _context.INV_WetBlueSelectionStock
                                                                    where (t.StoreID == obSelStock.StoreID && t.SupplierID == obSelStock.SupplierID &&
                                                                           t.PurchaseID == obSelStock.PurchaseID && t.LeatherStatusID == obSelStock.LeatherStatusID && t.GradeID == itemGrade.GradeID && t.ItemTypeID == obSelStock.ItemTypeID && t.SizeQtyRef == sizeQtyRef)
                                                                    select t).OrderByDescending(m => m.TransectionID).FirstOrDefault();
                            if (checkExist == null)
                            {
                                obSelStock.OpeningStockPcs = GetSizePcsValue(sizeQtyRef, itemGrade);
                                obSelStock.ReceiveStockPcs = GetSizePcsValue(sizeQtyRef, itemGrade);
                                obSelStock.IssueStockPcs = 0;
                                obSelStock.ClosingStockPcs = GetSizePcsValue(sizeQtyRef, itemGrade); //Convert.ToDecimal(item2.GradeQty);
                            }
                            else
                            {
                                obSelStock.OpeningStockPcs = checkExist.ClosingStockPcs;
                                obSelStock.ReceiveStockPcs = GetSizePcsValue(sizeQtyRef, itemGrade);
                                obSelStock.IssueStockPcs = 0;
                                obSelStock.ClosingStockPcs = Convert.ToDecimal(checkExist.ClosingStockPcs) + Convert.ToDecimal(GetSizePcsValue(sizeQtyRef, itemGrade));
                            }
                            obSelStock.LeatherTypeID = _context.Sys_LeatherType.FirstOrDefault(m => m.LeatherTypeName == "Wet Blue").LeatherTypeID;
                            obSelStock.ClosingStockAreaUnit = Convert.ToByte(itemGrade.AreaUnitID);
                            obSelStock.GradeID = Convert.ToInt16(itemGrade.GradeID);
                            repository.InvWetBlueSelectionStockRepo.Insert(obSelStock);
                        }
                    }
                }

                var grdSide = (from t in item.wbSelectionGrade
                               where (t.GradeSide > 0 || t.GradeArea > 0)
                               select new
                               {
                                   GradeSide = t.GradeSide,
                                   GradeArea = t.GradeArea,
                                   GradeID = t.GradeID
                               }).AsEnumerable();

                                foreach (var sideArea in grdSide)
                                {
                                    INV_WetBlueSelectionStock obSelSideStock = new INV_WetBlueSelectionStock();
                                    obSelSideStock.StoreID = Convert.ToByte(item.StoreID);
                                    obSelSideStock.SupplierID = item.SupplierID;
                                    obSelSideStock.PurchaseID = Convert.ToInt32(item.PurchaseID);
                                    obSelSideStock.ItemTypeID = Convert.ToByte(item.ItemTypeID);
                                    obSelSideStock.LeatherStatusID = Convert.ToByte(item.LeatherStatusID);
                                    obSelSideStock.SelectionStatus = "CNF";
                                    obSelSideStock.GradeID = sideArea.GradeID;

                                    INV_WetBlueSelectionStock checkExist = (from t in _context.INV_WetBlueSelectionStock
                                                                            where (t.StoreID == item.StoreID && t.SupplierID == item.SupplierID &&
                                                                                   t.PurchaseID == item.PurchaseID && t.LeatherStatusID == item.LeatherStatusID && 
                                                                                   t.GradeID == obSelSideStock.GradeID && t.ItemTypeID == obSelSideStock.ItemTypeID )
                                                                            select t).OrderByDescending(m => m.TransectionID).FirstOrDefault();
                                    if (checkExist == null)
                                    {
                                        obSelSideStock.OpeningStockSide = sideArea.GradeSide;
                                        obSelSideStock.ReceiveStockSide = 0;
                                        obSelSideStock.IssueStockSide = 0;
                                        obSelSideStock.ClosingStockSide = sideArea.GradeSide;                               
                                    }
                                    else {

                                        obSelSideStock.OpeningStockSide = Convert.ToDecimal(checkExist.ClosingStockSide == null ? 0 : checkExist.ClosingStockSide);
                                        obSelSideStock.ReceiveStockSide = sideArea.GradeSide;
                                        obSelSideStock.IssueStockSide = 0;
                                        obSelSideStock.ClosingStockSide = Convert.ToDecimal(obSelSideStock.OpeningStockSide + obSelSideStock.ReceiveStockSide);                                
                                    }
                                    obSelSideStock.SizeQtyRef = "SideQty";
                                    repository.InvWetBlueSelectionStockRepo.Insert(obSelSideStock);

                                    INV_WetBlueSelectionStock obSelAreaStock = new INV_WetBlueSelectionStock();
                                    obSelAreaStock.StoreID = Convert.ToByte(item.StoreID);
                                    obSelAreaStock.SupplierID = item.SupplierID;
                                    obSelAreaStock.PurchaseID = Convert.ToInt32(item.PurchaseID);
                                    obSelAreaStock.ItemTypeID = Convert.ToByte(item.ItemTypeID);
                                    obSelAreaStock.LeatherStatusID = Convert.ToByte(item.LeatherStatusID);
                                    obSelAreaStock.SelectionStatus = "CNF";
                                    obSelAreaStock.GradeID = sideArea.GradeID;
                                    if (checkExist == null)
                                    {
                                        obSelAreaStock.OpeningStockArea = sideArea.GradeArea;
                                        obSelAreaStock.ReceiveStockArea = 0;
                                        obSelAreaStock.IssueStockArea = 0;
                                        obSelAreaStock.ClosingStockArea = sideArea.GradeArea;
                                    }
                                    else 
                                    {
                                        obSelAreaStock.OpeningStockArea = Convert.ToDecimal(checkExist.ClosingStockArea == null ? 0 : checkExist.ClosingStockArea);
                                        obSelAreaStock.ReceiveStockArea = sideArea.GradeArea;
                                        obSelAreaStock.IssueStockArea = 0;
                                        obSelAreaStock.ClosingStockArea = Convert.ToDecimal(obSelAreaStock.OpeningStockArea + obSelAreaStock.ReceiveStockArea);
                                    }
                                    obSelAreaStock.SizeQtyRef = "AreaQty";
                                    repository.InvWetBlueSelectionStockRepo.Insert(obSelAreaStock);
                                }
                                try
                                {
                                    repository.Save();
                                }
                                catch (Exception ex)
                                {
                                    return 0;
                                }                              
                            
                break;
            }
            return 1;





            //PRD_WetBlueProductionStock wbProdStck = new PRD_WetBlueProductionStock();
            //if (dataSet.wbSelectionItem != null)
            //{
            //    foreach (var item in dataSet.wbSelectionItem)
            //    {
            //        int SupplierID = item.SupplierID;//Convert.ToInt32(repository.SysSupplierRepository.Get(filter: ob => ob.SupplierCode == item.SupplierCode).Select(o => o.SupplierID).FirstOrDefault());
            //        try
            //        {
            //            foreach (var item2 in item.wbSelectionGrade)
            //            {
            //                wbProdStck.SupplierID = SupplierID;
            //                wbProdStck.PurchaseID = Convert.ToInt64(item.PurchaseID);
            //                wbProdStck.ItemTypeID = Convert.ToByte(item.ItemTypeID);
            //                wbProdStck.StoreID = Convert.ToByte(item.StoreID);
            //                wbProdStck.LeatherTypeID = (from t in _context.PRD_WetBlueProductionStock where t.SupplierID == wbProdStck.SupplierID && t.PurchaseID == wbProdStck.PurchaseID && t.ItemTypeID == wbProdStck.ItemTypeID && t.StoreID == wbProdStck.StoreID && (t.ReceivePcs > 0 || t.ReceiveSide > 0) select t.LeatherTypeID).FirstOrDefault();
            //                wbProdStck.LeatherStatusID = (from t in _context.PRD_WetBlueProductionStock where t.SupplierID == wbProdStck.SupplierID && t.PurchaseID == wbProdStck.PurchaseID && t.ItemTypeID == wbProdStck.ItemTypeID && t.StoreID == wbProdStck.StoreID && (t.ReceivePcs > 0 || t.ReceiveSide > 0) select t.LeatherStatusID).FirstOrDefault();//Convert.ToByte(item.LeatherStatusID);                  

            //                PRD_WetBlueProductionStock checkProdExist = (from t in _context.PRD_WetBlueProductionStock
            //                                                             where (t.StoreID == wbProdStck.StoreID && t.SupplierID == wbProdStck.SupplierID &&
            //                                                                t.PurchaseID == wbProdStck.PurchaseID && t.ItemTypeID == wbProdStck.ItemTypeID && t.LeatherStatusID == wbProdStck.LeatherStatusID && t.LeatherTypeID == wbProdStck.LeatherTypeID)
            //                                                             select t).OrderByDescending(m => m.TransectionID).FirstOrDefault();
            //                if (checkProdExist == null)
            //                {
            //                    return 0;
            //                }
            //                else
            //                {
            //                    wbProdStck.OpeningPcs = Convert.ToDecimal(checkProdExist.ClosingProductionkPcs == null ? 0 : checkProdExist.ClosingProductionkPcs);
            //                    wbProdStck.OpeningSide = Convert.ToDecimal(checkProdExist.ClosingProductionSide == null ? 0 : checkProdExist.ClosingProductionSide);
            //                    wbProdStck.OpeningArea = Convert.ToDecimal(checkProdExist.ClosingProductionArea == null ? 0 : checkProdExist.ClosingProductionArea);
            //                    wbProdStck.ReceivePcs = 0;
            //                    wbProdStck.ReceiveSide = 0;
            //                    wbProdStck.ReceiveArea = 0;
            //                    wbProdStck.IssuePcs = Convert.ToDecimal(item.ClosingProductionkPcs);
            //                    wbProdStck.IssueSide = Convert.ToDecimal(item.ClosingProductionSide);
            //                    wbProdStck.IssueArea = 0;
            //                    wbProdStck.ClosingProductionkPcs = Convert.ToDecimal(wbProdStck.OpeningPcs - wbProdStck.IssuePcs);
            //                    wbProdStck.ClosingProductionSide = Convert.ToDecimal(wbProdStck.OpeningSide - wbProdStck.IssueSide);
            //                    wbProdStck.ClosingProductionArea = 0;// Convert.ToDecimal(wbProdStck.OpeningArea);
            //                    if (wbProdStck.ClosingProductionkPcs < 0 || wbProdStck.ClosingProductionSide < 0)
            //                    {
            //                        return 0;
            //                    }
            //                }


            //                int i = 1;
            //                string sizeQtyRef = "";
            //                for (i = 1; i <= 10; i++)
            //                {
            //                    sizeQtyRef = "SizeQty" + i;
            //                    if (CheckDataValidity(sizeQtyRef, item.wbSelectionGrade))
            //                    {
            //                        INV_WetBlueSelectionStock obSelStock = new INV_WetBlueSelectionStock();
            //                        obSelStock.StoreID = Convert.ToByte(item.StoreID);
            //                        obSelStock.SupplierID = SupplierID;
            //                        obSelStock.PurchaseID = Convert.ToInt32(item.PurchaseID);
            //                        obSelStock.ItemTypeID = Convert.ToByte(item.ItemTypeID);
            //                        //obSelStock.LeatherTypeID = 22;
            //                        obSelStock.LeatherStatusID = Convert.ToByte(item.LeatherStatusID);
            //                        obSelStock.SelectionStatus = "CNF";
            //                        obSelStock.SizeQtyRef = sizeQtyRef;
            //                        byte ItemTypeID = Convert.ToByte(item.ItemTypeID);
            //                        INV_WetBlueSelectionStock checkExist = (from t in _context.INV_WetBlueSelectionStock
            //                                                                where (t.StoreID == item.StoreID && t.SupplierID == SupplierID &&
            //                                                                       t.PurchaseID == item.PurchaseID && t.LeatherStatusID == item.LeatherStatusID && t.GradeID == item2.GradeID && t.ItemTypeID == ItemTypeID && t.SizeQtyRef == sizeQtyRef)
            //                                                                select t).OrderByDescending(m => m.TransectionID).FirstOrDefault();
            //                        if (checkExist == null)
            //                        {
            //                            obSelStock.OpeningStockPcs = GetSizePcsValue(sizeQtyRef, item.wbSelectionGrade);
            //                            obSelStock.ReceiveStockPcs = GetSizePcsValue(sizeQtyRef, item.wbSelectionGrade);
            //                            obSelStock.IssueStockPcs = 0;
            //                            obSelStock.ClosingStockPcs = GetSizePcsValue(sizeQtyRef, item.wbSelectionGrade); //Convert.ToDecimal(item2.GradeQty);
            //                        }
            //                        else
            //                        {
            //                            obSelStock.OpeningStockPcs = checkExist.ClosingStockPcs;
            //                            obSelStock.ReceiveStockPcs = GetSizePcsValue(sizeQtyRef, item.wbSelectionGrade);
            //                            obSelStock.IssueStockPcs = 0;
            //                            obSelStock.ClosingStockPcs = Convert.ToDecimal(checkExist.ClosingStockPcs) + Convert.ToDecimal(GetSizePcsValue(sizeQtyRef, item.wbSelectionGrade));
            //                        }
            //                        obSelStock.LeatherTypeID = _context.Sys_LeatherType.FirstOrDefault(m => m.LeatherTypeName == "Wet Blue").LeatherTypeID;
            //                        obSelStock.ClosingStockAreaUnit = Convert.ToByte(item2.AreaUnitID);
            //                        obSelStock.GradeID = item2.GradeID;
            //                        repository.InvWetBlueSelectionStockRepo.Insert(obSelStock);
            //                    }                             
            //                }
                                
            //                wbProdStck.AreaUnit = Convert.ToByte(item.UnitID);
            //                repository.PrdWetBlueProductionStockRepo.Insert(wbProdStck);
            //                break;
            //            }

            //            var grdSide = (from t in item.wbSelectionGrade where (t.GradeSide >0 || t.GradeArea >0) select new{
            //            GradeSide = t.GradeSide,
            //            GradeArea = t.GradeArea,
            //            GradeID = t.GradeID
            //            }).AsEnumerable();
                       
            //            foreach (var sideArea in grdSide)
            //            {
            //                INV_WetBlueSelectionStock obSelStock = new INV_WetBlueSelectionStock();
            //                obSelStock.StoreID = Convert.ToByte(item.StoreID);
            //                obSelStock.SupplierID = SupplierID;
            //                obSelStock.PurchaseID = Convert.ToInt32(item.PurchaseID);
            //                obSelStock.ItemTypeID = Convert.ToByte(item.ItemTypeID);
            //                obSelStock.LeatherStatusID = Convert.ToByte(item.LeatherStatusID);
            //                obSelStock.SelectionStatus = "CNF";
            //                obSelStock.GradeID = sideArea.GradeID;

            //                INV_WetBlueSelectionStock checkExist = (from t in _context.INV_WetBlueSelectionStock
            //                                                        where (t.StoreID == item.StoreID && t.SupplierID == SupplierID &&
            //                                                               t.PurchaseID == item.PurchaseID && t.LeatherStatusID == item.LeatherStatusID && 
            //                                                               t.GradeID == obSelStock.GradeID && t.ItemTypeID == obSelStock.ItemTypeID )
            //                                                        select t).OrderByDescending(m => m.TransectionID).FirstOrDefault();
            //                if (checkExist == null)
            //                {
            //                    obSelStock.OpeningStockSide = sideArea.GradeSide;
            //                    obSelStock.ReceiveStockSide = 0;
            //                    obSelStock.IssueStockSide = 0;
            //                    obSelStock.ClosingStockSide = sideArea.GradeSide;                               
            //                }
            //                else {

            //                    obSelStock.OpeningStockSide = Convert.ToDecimal(checkExist.ClosingStockSide == null ? 0 : checkExist.ClosingStockSide);
            //                    obSelStock.ReceiveStockSide = sideArea.GradeSide;
            //                    obSelStock.IssueStockSide = 0;
            //                    obSelStock.ClosingStockSide = Convert.ToDecimal(obSelStock.OpeningStockSide + obSelStock.ReceiveStockSide);                                
            //                }

            //                repository.InvWetBlueSelectionStockRepo.Insert(obSelStock);
                            
            //                obSelStock = new INV_WetBlueSelectionStock();
            //                obSelStock.StoreID = Convert.ToByte(item.StoreID);
            //                obSelStock.SupplierID = SupplierID;
            //                obSelStock.PurchaseID = Convert.ToInt32(item.PurchaseID);
            //                obSelStock.ItemTypeID = Convert.ToByte(item.ItemTypeID);
            //                obSelStock.LeatherStatusID = Convert.ToByte(item.LeatherStatusID);
            //                obSelStock.SelectionStatus = "CNF";
            //                obSelStock.GradeID = sideArea.GradeID;
            //                if (checkExist == null)
            //                {
            //                    obSelStock.OpeningStockArea = sideArea.GradeArea;
            //                    obSelStock.ReceiveStockArea = 0;
            //                    obSelStock.IssueStockArea = 0;
            //                    obSelStock.ClosingStockArea = sideArea.GradeArea;
            //                }
            //                else 
            //                {
            //                    obSelStock.OpeningStockArea = Convert.ToDecimal(checkExist.ClosingStockArea == null ? 0 : checkExist.ClosingStockArea);
            //                    obSelStock.ReceiveStockArea = sideArea.GradeArea;
            //                    obSelStock.IssueStockArea = 0;
            //                    obSelStock.ClosingStockArea = Convert.ToDecimal(obSelStock.OpeningStockArea + obSelStock.ReceiveStockArea);
            //                }
            //                repository.InvWetBlueSelectionStockRepo.Insert(obSelStock);
            //            }
            //            try
            //            {
            //                repository.Save();
            //            }
            //            catch (Exception ex)
            //            {
            //                return 0;
            //            }
            //            return 1;
            //        }
            //        catch (Exception ex)
            //        {
            //            return 0;
            //        }
            //        break;
            //    }
            //}
        }

        private decimal? GetSizePcsValue(string sizeQtyRef, wbSelectionGrade itemGrade)
        {
            decimal qty = 0;
            switch (sizeQtyRef)
            {
                case "SizeQty1":
                    qty = (decimal)(itemGrade.SizeQty1 == null ? 0 : itemGrade.SizeQty1);
                    break;
                case "SizeQty2":
                    qty = (decimal)(itemGrade.SizeQty2 == null ? 0 : itemGrade.SizeQty2);
                    break;
                case "SizeQty3":
                    qty = (decimal)(itemGrade.SizeQty3 == null ? 0 : itemGrade.SizeQty3);
                    break;
                case "SizeQty4":
                    qty = (decimal)(itemGrade.SizeQty4 == null ? 0 : itemGrade.SizeQty4);
                    break;
                case "SizeQty5":
                    qty = (decimal)(itemGrade.SizeQty5 == null ? 0 : itemGrade.SizeQty5);
                    break;
                case "SizeQty6":
                    qty = (decimal)(itemGrade.SizeQty6 == null ? 0 : itemGrade.SizeQty6);
                    break;
                case "SizeQty7":
                    qty = (decimal)(itemGrade.SizeQty7 == null ? 0 : itemGrade.SizeQty7);
                    break;
                case "SizeQty8":
                    qty = (decimal)(itemGrade.SizeQty8 == null ? 0 : itemGrade.SizeQty8);
                    break;
                case "SizeQty9":
                    qty = (decimal)(itemGrade.SizeQty9 == null ? 0 : itemGrade.SizeQty9);
                    break;
                case "SizeQty10":
                    qty = (decimal)(itemGrade.SizeQty10 == null ? 0 : itemGrade.SizeQty10);
                    break;
            }
            return qty;
        }

        private bool CheckDataValidity(string sizeQtyRef, wbSelectionGrade itemGrade)
        {
            decimal qty = 0;
            switch (sizeQtyRef)
            {
                case "SizeQty1":
                    qty = (decimal)(itemGrade.SizeQty1 == null ? 0 : itemGrade.SizeQty1);
                    break;
                case "SizeQty2":
                    qty = (decimal)(itemGrade.SizeQty2 == null ? 0 : itemGrade.SizeQty2);
                    break;
                case "SizeQty3":
                    qty = (decimal)(itemGrade.SizeQty3 == null ? 0 : itemGrade.SizeQty3);
                    break;
                case "SizeQty4":
                    qty = (decimal)(itemGrade.SizeQty4 == null ? 0 : itemGrade.SizeQty4);
                    break;
                case "SizeQty5":
                    qty = (decimal)(itemGrade.SizeQty5 == null ? 0 : itemGrade.SizeQty5);
                    break;
                case "SizeQty6":
                    qty = (decimal)(itemGrade.SizeQty6 == null ? 0 : itemGrade.SizeQty6);
                    break;
                case "SizeQty7":
                    qty = (decimal)(itemGrade.SizeQty7 == null ? 0 : itemGrade.SizeQty7);
                    break;
                case "SizeQty8":
                    qty = (decimal)(itemGrade.SizeQty8 == null ? 0 : itemGrade.SizeQty8);
                    break;
                case "SizeQty9":
                    qty = (decimal)(itemGrade.SizeQty9 == null ? 0 : itemGrade.SizeQty9);
                    break;
                case "SizeQty10":
                    qty = (decimal)(itemGrade.SizeQty10==null?0:itemGrade.SizeQty10);
                    break;
                default: qty = 0; break;
            }
            if (qty > 0) { return true; }
            return false;
        }

        




        public object GetSelectionInfoByID(int id)
        {
            var gradeInfo = (from temp in _context.PRD_WBSelectionItem
                             join temp2 in _context.PRD_WBSelectionGrade on temp.WBSelectItemID equals temp2.WBSelectItemID
                             where temp.WBSelectionID == id
                             select temp2).ToList();

            var data = _context.UspGetWBSelectionDetailsBySelectionID(id).ToList();

            var lst = (from item in data
                       select new
                       {
                           WBSelectionID = item.WBSelectionID,
                           WBSelectItemNo = item.WBSelectionNo,
                           SelectionDate = Convert.ToDateTime(item.SelectionDate).ToString("dd/MM/yyyy"),
                           SupplierID = item.SupplierID,
                           SupplierCode = item.SupplierCode,
                           SupplierName = item.SupplierName,
                           Address = (from t in _context.Sys_SupplierAddress where t.SupplierID == (item.SupplierID == null ? 0 : item.SupplierID) select t.Address).DefaultIfEmpty("").FirstOrDefault(),
                           UserID = item.UserID,
                           SelectedBy = item.SelectedBy,
                           SelectionComments = item.SelectionComments,
                           PurchaseID = item.PurchaseID,
                           PurchaseDate = Convert.ToDateTime(item.PurchaseDate).ToString("dd/MM/yyyy"),
                           PurchaseQty = item.PurchaseQty,
                           ProductionQty = item.ProductionQty,
                           ProductionDue = item.ProductionDue,
                           StoreID = item.StoreID,
                           StoreName = item.StoreName,
                           SelectionComplete = item.SelectionComplete,
                           SelectionDue = item.SelectionDue,
                           ItemTypeID = item.ItemTypeID,
                           ItemTypeName = item.ItemTypeName,
                           LeatherStatusID = item.LeatherStatusID,
                           LeatherStatusName = item.LeatherStatusName,
                           UnitID = item.UnitID,
                           UnitName = item.UnitName,
                           ClosingProductionkPcs = item.ClosingProductionkPcs,
                           ClosingProductionSide = item.ClosingProductionSide,
                           ClosingProductionArea = item.ClosingProductionArea,

                           WBSelectItemID = (from t in gradeInfo select t.WBSelectItemID).FirstOrDefault(),
                           gradeData = from t in gradeInfo
                                       select new
                                       {
                                           WBSelectionGradeID = t.WBSelectionGradeID == null ? 0 : t.WBSelectionGradeID,
                                           GradeID = t.GradeID == null ? 0 : t.GradeID,
                                           GradeName = repository.SysGrade.GetByID(Convert.ToInt32(t.GradeID == null ? 0 : t.GradeID)).GradeName,
                                           // GradeQuantity = t.GradeQty == null ? 0 : t.GradeQty,
                                           SizeQty1 = t.SizeQty1,
                                           SizeQty2 = t.SizeQty2,
                                           SizeQty3 = t.SizeQty3,
                                           SizeQty4 = t.SizeQty4,
                                           SizeQty5 = t.SizeQty5,
                                           SizeQty6 = t.SizeQty6,
                                           SizeQty7 = t.SizeQty7,
                                           SizeQty8 = t.SizeQty8,
                                           SizeQty9 = t.SizeQty9,
                                           SizeQty10 = t.SizeQty10,
                                           GradeSide = t.GradeSide == null ? 0 : t.GradeSide,
                                           GradeArea = t.GradeArea == null ? 0 : t.GradeArea,
                                           UnitID = t.AreaUnitID == null ? 0 : t.AreaUnitID,
                                           UnitName = repository.SysUnitRepository.GetByID(Convert.ToInt32(t.AreaUnitID == null ? 0 : t.AreaUnitID)).UnitName
                                       }
                       }).AsEnumerable();
            return lst;
        }
        public ValidationMsg CheckedRecordStatus(long wbSelectionID, string status, int userID)
        {
            int flag = 0;
            long id = Convert.ToInt64(wbSelectionID == null ? 0 : wbSelectionID);
            var ob = (from temp in _context.PRD_WBSelection where temp.WBSelectionID == wbSelectionID select temp).FirstOrDefault();//repository.PrdWBSelectionRepository.GetByID(id);
            if (ob != null)
            {
                _vmMsg.ReturnCode = ob.RecordStatus;
                if (ob.RecordStatus == "CNF")
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Record already Confirmed.";
                }
                else if (ob.RecordStatus == "CHK")
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Data already Checked.";
                }

                else if (ob.RecordStatus == "NCF" && status == "CHK")
                {
                    // Chked              
                    ob.RecordStatus = status;
                    ob.CheckedBy = userID;
                    ob.CheckDate = DateTime.Now;
                    flag = _context.SaveChanges();
                    if (flag == 1)
                    {
                        _vmMsg.Type = Enums.MessageType.Success;
                        _vmMsg.Msg = "Checked Successfully.";
                    }
                    else
                    {
                        _vmMsg.Type = Enums.MessageType.Error;
                        _vmMsg.Msg = "Checked Faild.";
                    }
                }
                else
                {
                    if (ob.RecordStatus == "CHK")
                    {
                        _vmMsg.Type = Enums.MessageType.Error;
                        _vmMsg.Msg = "Data Already Checked ";
                    }
                    else if (ob.RecordStatus == "CNF")
                    {
                        _vmMsg.Type = Enums.MessageType.Error;
                        _vmMsg.Msg = "Data Already Confirmed";
                    }
                    else
                    {
                    }
                }
            }
            else
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Checked Faild.";
            }
            return _vmMsg;
        }
        public ValidationMsg ConfirmRecordStatus(wbSelection dataSet, int userID)
        {
            using (var tx = new TransactionScope())
            {
                using (_context)
                {
                    try
                    {
                        long purchaseID = 0;
                        if (!string.IsNullOrEmpty(dataSet.Remarks))
                        {
                            purchaseID = Convert.ToInt64(dataSet.Remarks);
                        }

                        var item = dataSet.wbSelectionItem.FirstOrDefault();
                        decimal selectionDue = 0;

                        if (!string.IsNullOrEmpty(dataSet.CheckNote))
                        {
                            selectionDue = Convert.ToDecimal(dataSet.CheckNote);
                        }

                        if (selectionDue > 0)
                        {
                            if (Convert.ToDecimal(item.ClosingProductionkPcs) > selectionDue)
                            {
                                _vmMsg.Type = Enums.MessageType.Error;
                                _vmMsg.Msg = "Your selection quantity should not grater than production quantity.";
                                return _vmMsg;
                            }
                        }

                        var obSelection = repository.PrdWBSelectionRepository.GetByID(dataSet.WBSelectionID);

                        if (obSelection.RecordStatus == "CNF")
                        {
                            _vmMsg.Type = Enums.MessageType.Error;
                            _vmMsg.Msg = "Record Already Confirmed.";
                        }
                        else if (obSelection.RecordStatus == "CHK" && obSelection.RecordStatus != "NCF")
                        {
                            // Checked              
                            obSelection.RecordStatus = "CNF";
                            obSelection.ConfirmedBy = userID;
                            obSelection.ConfirmedDate = DateTime.Now;
                            repository.PrdWBSelectionRepository.Update(obSelection);
                            int stockID = UpdateProductionStock(dataSet);
                            try
                            {
                                if (stockID > 0)
                                {
                                    repository.Save();
                                    tx.Complete();
                                    _vmMsg.Type = Enums.MessageType.Success;
                                    _vmMsg.Msg = "Confirmed Successfully.";
                                }
                                else
                                {
                                    _vmMsg.Type = Enums.MessageType.Error;
                                    _vmMsg.Msg = "Confirmation faild due to communication error.";
                                }

                            }
                            catch (Exception ex)
                            {
                                _vmMsg.Type = Enums.MessageType.Error;
                                _vmMsg.Msg = "Confirmed Faild.";
                            }
                        }
                        else
                        {
                            _vmMsg.Type = Enums.MessageType.Error;
                            _vmMsg.Msg = "Please Check the Record First.";
                        }
                    }
                    catch (Exception ex)
                    {
                        _vmMsg.Type = Enums.MessageType.Error;
                        _vmMsg.Msg = "Please Reset Page and Try Again.";
                    }
                }
            }
            return _vmMsg;
        }

        public List<wbSelectionItem> GetWetBlueProductionStoreList()
        {
            var query = "select distinct dbo.PRD_WetBlueProductionStock.StoreID,dbo.SYS_Store.StoreName from dbo.PRD_WetBlueProductionStock" +
                        " inner join dbo.SYS_Store on SYS_Store.StoreID = PRD_WetBlueProductionStock.StoreID WHERE SYS_Store.IsDelete =0";
            var allData = _context.Database.SqlQuery<wbSelectionItem>(query).ToList();
            return allData;
        }

        public List<SupplierPopupInfo> GetSupplierFromStoreList(string ConcernStore)
        {
            if (ConcernStore == "")
            {
                ConcernStore = "0";
            }
            var query = @"select DISTINCT inv.SupplierID,(select SupplierName from dbo.Sys_Supplier where SupplierID = inv.SupplierID)SupplierName,
                        (select TOP(1) [Address] from dbo.Sys_SupplierAddress where SupplierID = inv.SupplierID AND IsDelete=0) [Address],
                        (select TOP(1) ContactPerson from dbo.Sys_SupplierAddress where SupplierID = inv.SupplierID AND IsDelete=0) ContactPerson,
                        (select TOP(1) ContactNumber from dbo.Sys_SupplierAddress where SupplierID = inv.SupplierID AND IsDelete=0) ContactNumber,
                        (select SupplierCode from dbo.Sys_Supplier where SupplierID = inv.SupplierID AND IsDelete=0) SupplierCode from dbo.PRD_WetBlueProductionStock inv
                         INNER JOIN (select MAX(TransectionID)TransectionID,SupplierID,StoreID,ItemTypeID,LeatherTypeID,LeatherStatusID from dbo.PRD_WetBlueProductionStock
                         group by SupplierID,StoreID,ItemTypeID,LeatherTypeID,LeatherStatusID) sup
                         ON inv.TransectionID=sup.TransectionID
                         where inv.StoreID = " + ConcernStore + " and inv.ClosingProductionkPcs>0";
            var allData = _context.Database.SqlQuery<SupplierPopupInfo>(query).ToList();
            return allData;
        }

        public List<SupplierPopupInfo> GetSupplierListSearchById(string supplier)
        {
            var query = @"SELECT * FROM (
                        select DISTINCT inv.SupplierID,(select SupplierName from dbo.Sys_Supplier where SupplierID = inv.SupplierID)SupplierName,
                        (select TOP(1) [Address] from dbo.Sys_SupplierAddress where SupplierID = inv.SupplierID) [Address],
                        (select TOP(1) ContactPerson from dbo.Sys_SupplierAddress where SupplierID = inv.SupplierID) ContactPerson,
                        (select TOP(1) ContactNumber from dbo.Sys_SupplierAddress where SupplierID = inv.SupplierID) ContactNumber,
                        (select SupplierCode from dbo.Sys_Supplier where SupplierID = inv.SupplierID)SupplierCode from dbo.PRD_WetBlueProductionStock inv
                         INNER JOIN (select MAX(TransectionID)TransectionID,SupplierID,StoreID,ItemTypeID,LeatherTypeID,LeatherStatusID from dbo.PRD_WetBlueProductionStock
                         group by SupplierID,StoreID,ItemTypeID,LeatherTypeID,LeatherStatusID) sup
                         ON inv.TransectionID=sup.TransectionID) T1
						 WHERE T1.SupplierName LIKE '" + supplier + "%'";
            var allData = _context.Database.SqlQuery<SupplierPopupInfo>(query).ToList();
            return allData.ToList();
        }

        public List<wbshowField> GetLeatherInfoList(string ConcernStore, string SupplierID)
        {
            if ((!string.IsNullOrEmpty(ConcernStore) && (!string.IsNullOrEmpty(SupplierID))))
            {
                var query = @"SELECT T1.PurchaseID,T1.StoreID,T1.ItemTypeID,T1.SupplierID,(ISNULL(T1.ProductionQty,0) + (ISNULL(T1.SelectionSideQty,0)/2)) ProductionQty,ISNULL(T1.SelectionSideQty,0) SelectionSideQty,ISNULL(SelectionCompletedSide,0) SelectionCompSide,
ISNULL((ISNULL(T2.PurchaseQty,0) - (ISNULL(T1.ProductionQty,0) + (ISNULL(T1.SelectionSideQty,0)/2))),0) ProductionDue,
ISNULL(T1.SelectionSideDueQty,0) SelectionSideDueQty,
ISNULL(T1.SelectionQty,0) SelectionQty,
((ISNULL(T1.ProductionQty,0)) - ISNULL(T1.SelectionQty,0))  SelectionDueQty,
      T1.PurchaseID,T1.ItemTypeID,ISNULL(T2.PurchaseQty,0) PurchaseQty

                              FROM (
		                            -- ProductionInfo
				                            SELECT wbps.PurchaseID,wbps.StoreID,wbps.ItemTypeID,wbps.SupplierID, SUM(ISNULL(ReceivePcs,0)) ProductionQty,
				                            (SUM(ISNULL(IssuePcs,0))) SelectionQty, SUM(ISNULL(ReceiveSide,0)) SelectionSideQty,SUM(ISNULL(IssueSide,0)) SelectionCompletedSide,SUM(ISNULL(wbps.ReceiveSide,0))-SUM(ISNULL(IssueSide,0)) SelectionSideDueQty,
				                             (SUM(ISNULL(ReceivePcs,0)) - SUM(ISNULL(IssuePcs,0))) SelectionDueQty
				                            FROM PRD_WetBlueProductionStock wbps 
				                            GROUP BY wbps.PurchaseID,wbps.StoreID,wbps.ItemTypeID,wbps.SupplierID
	                            ) T1
	                            INNER JOIN (
		                            -- Purchse Info
				                            SELECT p.PurchaseID,pci.ItemTypeID,SUM(ISNULL(pci.ReceiveQty,0)) PurchaseQty 
				                            FROM Prq_Purchase p 
				                            INNER JOIN Prq_PurchaseChallan pc ON p.PurchaseID=pc.PurchaseID
				                            INNER JOIN Prq_PurchaseChallanItem pci ON pc.ChallanID = pci.ChallanID
				                            GROUP BY p.PurchaseID,pci.ItemTypeID
	                            ) T2 
	                            ON T1.PurchaseID = T2.PurchaseID AND T1.ItemTypeID = T2.ItemTypeID
	                        WHERE T1.StoreID = " + ConcernStore + " AND T1.SupplierID=" + SupplierID + "";

                var allData = _context.Database.SqlQuery<wbshowField>(query).ToList();
                return allData.Select(c => SetToBussinessObject(c)).ToList<wbshowField>();
            }
            else
                return null;
        }

        public wbshowField SetToBussinessObject(wbshowField Entity)
        {
            wbshowField Model = new wbshowField();
            //byte leatherTypeID = Entity.LeatherTypeID == null ? Convert.ToByte(0) : Convert.ToByte(Entity.LeatherTypeID);
            Model.PurchaseID = Entity.PurchaseID;
            Model.PurchaseNo = Entity.PurchaseID == null
                ? ""
                : _context.Prq_Purchase.Where(m => m.PurchaseID == Entity.PurchaseID).FirstOrDefault().PurchaseNo;
            Model.PurchaseDate = Entity.PurchaseID == null
        ? ""
        : Convert.ToDateTime(_context.Prq_Purchase.Where(m => m.PurchaseID == Entity.PurchaseID).FirstOrDefault().PurchaseDate).ToString("dd/MM/yyyy");

            Model.ItemTypeID = Entity.ItemTypeID;
            Model.ItemTypeName = Entity.ItemTypeID == null ? "" : _context.Sys_ItemType.Where(m => m.ItemTypeID == Entity.ItemTypeID).FirstOrDefault().ItemTypeName;
            Model.LeatherTypeID = Entity.LeatherTypeID;
            Model.LeatherTypeName = "Raw Hide";//Entity.LeatherTypeID == null ? "" : _context.Sys_LeatherType.Where(m => m.LeatherTypeID == leatherTypeID).FirstOrDefault().LeatherTypeName.DefaultIfEmpty("NA").ToString();
            Model.LeatherStatusID = Entity.LeatherStatusID;
            if (Entity.LeatherStatusID != 0)
            {
                Model.LeatherStatusName = Entity.LeatherStatusID == null ? "" : _context.Sys_LeatherStatus.Where(m => m.LeatherStatusID == Entity.LeatherStatusID).FirstOrDefault().LeatherStatusName;
            }
            Model.ProductionQty = Entity.ProductionQty;
            Model.SelectionQty = Entity.SelectionQty;
            Model.SelectionDueQty = Entity.SelectionDueQty;
            Model.PurchaseQty = Entity.PurchaseQty;
            Model.ProductionDue = Entity.ProductionDue;
            Model.AreaUnit = Entity.AreaUnit;
            Model.SelectionSideQty = Entity.SelectionSideQty;
            Model.SelectionSideDueQty = Entity.SelectionSideDueQty;
            Model.SelectionCompSide = Entity.SelectionCompSide;
            return Model;
        }

        public List<GradeSelectionSearchPopup> GetFinalGradeSelectionInfo()
        {
            DalReport ob = new DalReport();
            List<GradeSelectionSearchPopup> lstSearch = new List<GradeSelectionSearchPopup>();
            DataTable data = ob.GetFinalGradeSelecByWBSelectID(0);
            foreach (DataRow dr in data.Rows)
            {
                GradeSelectionSearchPopup obData = new GradeSelectionSearchPopup();
                obData.WBSelectionID = Convert.ToInt64(dr["WBSelectionID"]);
                obData.WBSelectionNo = Convert.ToString(dr["WBSelectionNo"]);
                obData.SelectionDate = Convert.ToDateTime(dr["SelectionDate"]);
                obData.StoreName = Convert.ToString(dr["StoreName"]);
                obData.SupplierName = Convert.ToString(dr["SupplierName"]);
                obData.PurchaseNo = Convert.ToString(dr["PurchaseNo"]);
                obData.PurchaseDate = Convert.ToDateTime(dr["PurchaseDate"]);
                obData.ItemTypeName = Convert.ToString(dr["ItemTypeName"]);
                obData.SelectionComplete = Convert.ToDecimal(dr["SelectionComplete"]);
                obData.RecordStatus = dr["RecordStatus"].ToString();
                lstSearch.Add(obData);
            }
            return lstSearch;
        }

        public object GetFinalGradeSelectionInfo(long id)
        {
            List<GradeSelectionSearchPopup> lstSearchData = new List<GradeSelectionSearchPopup>();
            DalReport obReport = new DalReport();
            //      DataTable data = obReport.GetFinalGradeSelecByWBSelectID(id);
            DataTable data = obReport.UspFinalGradeSelectionByWBSelectionID(id);

            if (data.Rows.Count > 0 && data != null)
            {
                GradeSelectionSearchPopup obSearchData = new GradeSelectionSearchPopup();
                obSearchData.StoreID = Convert.ToByte(data.Rows[0]["StoreID"]);
                obSearchData.WBSelectionNo = data.Rows[0]["WBSelectionNo"].ToString();
                obSearchData.SelectionDate = Convert.ToDateTime(data.Rows[0]["SelectionDate"]);
                obSearchData.SupplierID = Convert.ToInt32(data.Rows[0]["SupplierID"]);
                obSearchData.UserID = Convert.ToInt32(data.Rows[0]["UserID"]);
                obSearchData.UnitID = (byte)data.Rows[0]["UnitID"];
                obSearchData.SelectionComments = data.Rows[0]["SelectionComments"].ToString();
                obSearchData.RecordStatus = data.Rows[0]["RecordStatus"].ToString();
                obSearchData.PurchaseID = Convert.ToInt64(data.Rows[0]["PurchaseID"]);
                obSearchData.PurchaseNo = data.Rows[0]["PurchaseNo"].ToString();
                obSearchData.ItemTypeID = (byte)data.Rows[0]["ItemTypeID"];
                obSearchData.LeatherStatusID = (byte)data.Rows[0]["LeatherStatusID"];
                obSearchData.StoreName = data.Rows[0]["StoreName"].ToString();
                obSearchData.SupplierName = data.Rows[0]["SupplierName"].ToString();
                obSearchData.SupplierCode = data.Rows[0]["SupplierCode"].ToString();
                obSearchData.SelectedBy = data.Rows[0]["SelectedBy"].ToString();
                obSearchData.ItemTypeName = data.Rows[0]["ItemTypeName"].ToString();
                obSearchData.LeatherStatusName = data.Rows[0]["LeatherStatusName"].ToString();
                obSearchData.ClosingProductionkPcs = Convert.ToDecimal(data.Rows[0]["ClosingProductionkPcs"]);
                obSearchData.ClosingProductionSide = Convert.ToDecimal(data.Rows[0]["ClosingProductionSide"]);
                //obSearchData.ClosingProductionArea = Convert.ToDecimal(data.Rows[0]["ClosingProductionArea"]);                
                obSearchData.SelectionSideDue = Convert.ToDecimal(data.Rows[0]["SelectionSideDue"]);
                obSearchData.PurchaseDate = Convert.ToDateTime(data.Rows[0]["PurchaseDate"]);
                obSearchData.PurchaseQty = Convert.ToDecimal(data.Rows[0]["PurchaseQty"]);
                obSearchData.ProductionQty = Convert.ToDecimal(data.Rows[0]["ProductionQty"]);
                obSearchData.ProductionDue = Convert.ToDecimal(data.Rows[0]["ProductionDue"]);
                obSearchData.SelectionComplete = Convert.ToDecimal(data.Rows[0]["SelectionComplete"]);
                obSearchData.SelectionDue = Convert.ToDecimal(data.Rows[0]["SelectionDue"]);
                obSearchData.AverageThickness = data.Rows[0]["AverageThickness"].ToString();
                obSearchData.LessCut = data.Rows[0]["LessCut"].ToString();
                obSearchData.GrainOff = data.Rows[0]["GrainOff"].ToString();
                if (!string.IsNullOrEmpty(data.Rows[0]["ProductionDate"].ToString()))
                { obSearchData.ProductionDate = Convert.ToDateTime(data.Rows[0]["ProductionDate"]); }
                lstSearchData.Add(obSearchData);

                List<GradeList> lstGrade = new List<GradeList>();

                var gradeList = repository.SysGrade.Get().ToList();
                foreach (var item in gradeList)
                {
                    GradeList ob = new GradeList();
                    ob.GradeID =item.GradeID;
                    ob.GradeName = item.GradeName;
                    lstGrade.Add(ob);
                }
                foreach (DataRow item in data.Rows)
                {
                    int GradeID = Convert.ToInt16(item["GradeID"]);
                    var exist = lstGrade.Find(temp => temp.GradeID == GradeID);                    
                    GradeList ob = new GradeList();
                    ob.GradeID = exist.GradeID;                    
                    ob.WBSelectionGradeID = Convert.ToInt64(item["WBSelectionGradeID"]);
                    ob.GradeName = exist.GradeName;
                    ob.GradeSide = Convert.ToDecimal(item["GradeSide"]);
                    ob.GradeArea = Convert.ToDecimal(item["GradeArea"]);
                    ob.UnitID = (byte)item["UnitID"];
                    ob.SizeQty1 = Convert.ToDecimal(item["SizeQty1"]);
                    ob.SizeQty2 = Convert.ToDecimal(item["SizeQty2"]);
                    ob.SizeQty3 = Convert.ToDecimal(item["SizeQty3"]);
                    ob.SizeQty4 = Convert.ToDecimal(item["SizeQty4"]);
                    ob.SizeQty5 = Convert.ToDecimal(item["SizeQty5"]);
                    ob.SizeQty6 = Convert.ToDecimal(item["SizeQty6"]);
                    ob.SizeQty7 = Convert.ToDecimal(item["SizeQty7"]);
                    ob.SizeQty8 = Convert.ToDecimal(item["SizeQty8"]);
                    ob.SizeQty9 = Convert.ToDecimal(item["SizeQty9"]);
                    ob.SizeQty10 = Convert.ToDecimal(item["SizeQty10"]);                   
                   
                    lstGrade.Remove(exist);
                    lstGrade.Add(ob);
                }

                var record = (from temp in lstSearchData
                              select new
                              {
                                  StoreID = temp.StoreID,
                                  WBSelectionNo = temp.WBSelectionNo.ToString(),
                                  SelectionDate = Convert.ToDateTime(temp.SelectionDate).ToString("dd/MM/yyyy"),
                                  SupplierID = Convert.ToInt32(temp.SupplierID),
                                  UserID = Convert.ToInt32(temp.UserID),
                                  UnitID = (byte)temp.UnitID,
                                  SelectionComments = temp.SelectionComments.ToString(),
                                  RecordStatus = temp.RecordStatus.ToString(),
                                  PurchaseID = Convert.ToInt64(temp.PurchaseID),
                                  PurchaseNo = temp.PurchaseNo.ToString(),
                                  ItemTypeID = (byte)temp.ItemTypeID,
                                  LeatherStatusID = (byte)temp.LeatherStatusID,
                                  StoreName = temp.StoreName.ToString(),
                                  SupplierName = temp.SupplierName.ToString(),
                                  SupplierCode = temp.SupplierCode.ToString(),
                                  SelectedBy = temp.SelectedBy.ToString(),
                                  ItemTypeName = temp.ItemTypeName.ToString(),
                                  LeatherStatusName = temp.LeatherStatusName.ToString(),
                                  ClosingProductionkPcs = Convert.ToDecimal(temp.ClosingProductionkPcs),
                                  SelectionSide = Convert.ToDecimal(temp.ClosingProductionSide),
                                  ClosingProductionArea = Convert.ToDecimal(temp.ClosingProductionArea),
                                  PurchaseDate = Convert.ToDateTime(temp.PurchaseDate).ToString("dd/MM/yyyy"),
                                  PurchaseQty = Convert.ToDecimal(temp.PurchaseQty),
                                  ProductionQty = Convert.ToDecimal(temp.ProductionQty),
                                  ProductionDue = Convert.ToDecimal(temp.ProductionDue),
                                  SelectionComplete = Convert.ToDecimal(temp.SelectionComplete),
                                  SelectionDue = Convert.ToDecimal(temp.SelectionDue),
                                  SelectionSideDue = Convert.ToDecimal(temp.SelectionSideDue),
                                  AverageThickness = temp.AverageThickness.ToString(),
                                  LessCut = temp.LessCut.ToString(),
                                  GrainOff = temp.GrainOff.ToString(),
                                  ProductionDate = temp.ProductionDate.ToShortDateString(),
                                  Grade = lstGrade.OrderBy(ob=>ob.GradeName)
                              }).AsEnumerable();
                return record;
            }
            return data;
        }

        public object GetGrade(long id)
        {
            var data = repository.PrdWBSelectionGradeRepository.Get(filter: ob => ob.WBSelectionID == id).OrderBy(o => o.GradeID).ToList();
            var gradeGrid = from r in data
                   select new
                   {
                       WBSelectionGradeID = r.WBSelectionGradeID,
                       WBSelectItemID = r.WBSelectItemID,
                       WBSelectionID = r.WBSelectionID,
                       GradeID = r.GradeID,
                       GradeName = r.Sys_Grade.GradeName,
                       SizeQty1 = r.SizeQty1,
                       SizeQty2 = r.SizeQty2,
                       SizeQty3 = r.SizeQty3,
                       SizeQty4 = r.SizeQty4,
                       SizeQty5 = r.SizeQty5,
                       SizeQty6 = r.SizeQty6,
                       SizeQty7 = r.SizeQty7,
                       SizeQty8 = r.SizeQty8,
                       SizeQty9 = r.SizeQty9,
                       SizeQty10 = r.SizeQty10,
                       GradeSide = r.GradeSide,
                       GradeArea = r.GradeArea
                   };
            return gradeGrid;
        }
    }
}
