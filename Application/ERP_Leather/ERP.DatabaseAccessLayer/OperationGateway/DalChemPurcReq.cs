using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using Microsoft.Scripting.Hosting;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalChemPurcReq
    {
        private readonly BLC_DEVEntities _context;
        private readonly ValidationMsg _vmMsg;
        private UnitOfWork _unit;

        public DalChemPurcReq()
        {
            _context = new BLC_DEVEntities();
            _unit = new UnitOfWork();
            _vmMsg = new ValidationMsg();
        }

        public ValidationMsg SaveData(PrqChemPurcReq modelReq, int userId, string pageUrl)
        {
            try
            {
                using (var tx = new TransactionScope())
                {

                    var currentRequisitionId = 0;
                    var currentRequisitionCode = DalCommon.GetPreDefineNextCodeByUrl(pageUrl);
                    //var currentRequisitionCode = DalCommon.GetPreDefineValue("1", "00051");
                    using (_context)
                    {
                        var chemPurcReq = new PRQ_ChemPurcReq
                        {
                            RequisitionNo = currentRequisitionCode,
                            RequisitionCategory = modelReq.RequisitionCategory,
                            RequisitionType = modelReq.RequisitionType,
                            ReqRaisedOn = DalCommon.SetDate(modelReq.ReqRaisedOn),
                            RequisitionFrom = modelReq.RequisitionFrom,
                            RequisitionTo = modelReq.RequisitionTo,
                            RequiredByTime = modelReq.RequiredByTime,
                            ReqRaisedBy = modelReq.ReqRaisedBy,
                            RecordStatus = "NCF",
                            RequisitionState = "RNG",
                            SetOn = DateTime.Now,
                            SetBy = userId,
                            IPAddress = GetIPAddress.LocalIPAddress()
                        };
                        _context.PRQ_ChemPurcReq.Add(chemPurcReq);
                        _context.SaveChanges();

                        currentRequisitionId = chemPurcReq.RequisitionID;

                        if (modelReq.ChemPurcReqItems != null)
                        {
                            foreach (var chemItem in modelReq.ChemPurcReqItems.Select(item => new PRQ_ChemPurcReqItem()
                            {
                                RequisitionID = currentRequisitionId,
                                ItemID = item.ItemID,
                                SupplierID = modelReq.RequisitionTo,
                                PackSize = item.SizeID,
                                SizeUnit = item.PackUnitID,
                                PackQty = item.PackQty,
                                RequsitionQty = item.RequisitionQty,
                                RequisitionUnit = item.RequisitionUnit,
                                ManufacturerID = item.ManufacturerID,
                                SetOn = DateTime.Now,
                                SetBy = userId
                            }))
                            {
                                _context.PRQ_ChemPurcReqItem.Add(chemItem);
                                _context.SaveChanges();
                            }
                        }

                    }
                    tx.Complete();

                    _vmMsg.Type = Enums.MessageType.Success;
                    _vmMsg.Msg = "Saved Successfully.";
                    _vmMsg.ReturnId = currentRequisitionId;
                    _vmMsg.ReturnCode = currentRequisitionCode;
                }
            }
            catch (Exception)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to save.";
            }
            return _vmMsg;
        }

        public ValidationMsg UpdateData(PrqChemPurcReq modelReq, int userId)
        {
            try
            {
                using (var tx = new TransactionScope())
                {

                    using (_context)
                    {

                        var objModel = _context.PRQ_ChemPurcReq.FirstOrDefault(r => r.RequisitionID == modelReq.RequisitionID);
                        if (objModel != null)
                        {
                            objModel.RequisitionCategory = modelReq.RequisitionCategory;
                            objModel.RequisitionType = modelReq.RequisitionType;
                            objModel.RequisitionFrom = modelReq.RequisitionFrom;
                            objModel.RequisitionTo = modelReq.RequisitionTo;
                            objModel.RequiredByTime = modelReq.RequiredByTime;
                            objModel.ReqRaisedOn = DalCommon.SetDate(modelReq.ReqRaisedOn);
                            objModel.ReqRaisedBy = modelReq.ReqRaisedBy;
                            objModel.ModifiedBy = userId;
                            objModel.ModifiedOn = DateTime.Now;
                        }
                        _context.SaveChanges();

                        if (modelReq.ChemPurcReqItems != null)
                        {

                            foreach (var chemItem in modelReq.ChemPurcReqItems)
                            {
                                if (chemItem.RequisitionItemID == 0)
                                {
                                    var objItem = new PRQ_ChemPurcReqItem();
                                    objItem.RequisitionID = modelReq.RequisitionID;
                                    objItem.ItemID = chemItem.ItemID;
                                    objItem.PackSize = chemItem.SizeID;
                                    objItem.SizeUnit = chemItem.PackUnitID;
                                    objItem.PackQty = chemItem.PackQty;
                                    objItem.RequsitionQty = chemItem.RequisitionQty;
                                    objItem.RequisitionUnit = chemItem.RequisitionUnit;
                                    objItem.ManufacturerID = chemItem.ManufacturerID;
                                    objItem.SupplierID = modelReq.RequisitionTo;
                                    objItem.SetOn = DateTime.Now;
                                    objItem.SetBy = userId;
                                    _context.PRQ_ChemPurcReqItem.Add(objItem);
                                }
                                else
                                {
                                    var updateItem =
                                        _context.PRQ_ChemPurcReqItem.First(r => r.RequisitionItemID == chemItem.RequisitionItemID);
                                    updateItem.ItemID = chemItem.ItemID;
                                    updateItem.PackSize = chemItem.SizeID;
                                    updateItem.SizeUnit = chemItem.PackUnitID;
                                    updateItem.PackQty = chemItem.PackQty;
                                    updateItem.RequsitionQty = chemItem.RequisitionQty;
                                    updateItem.RequisitionUnit = chemItem.RequisitionUnit;
                                    updateItem.SupplierID = modelReq.RequisitionTo;
                                    updateItem.ManufacturerID = chemItem.ManufacturerID;
                                    updateItem.ModifiedOn = DateTime.Now;
                                    updateItem.ModifiedBy = userId;
                                }
                            }
                            _context.SaveChanges();
                        }

                    }
                    tx.Complete();

                    _vmMsg.Type = Enums.MessageType.Success;
                    _vmMsg.Msg = "Updated Successfully.";
                    _vmMsg.ReturnId = modelReq.RequisitionID;
                    _vmMsg.ReturnCode = modelReq.RequisitionNo;
                }
            }
            catch (Exception)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to save.";
            }
            return _vmMsg;
        }

        public IEnumerable<PrqChemPurcReq> GetAllNcfChemPurcReqList(string pageMode, string reqCategory)
        {
            var query = new StringBuilder();
            using (_context)
            {
                if (pageMode.Equals("ENT"))
                {
                    query.Append(" SELECT CP.RequisitionID,CP.RequisitionNo,CASE WHEN CP.RequisitionCategory='FP' THEN 'Foreign' ELSE 'Local' END RequisitionCategory, CP.RequisitionCategory AS RequisitionCategoryValue,");
                    query.Append(" CASE WHEN CP.RequisitionType='UR' THEN 'Urgent' ELSE 'Normal' END RequisitionType,CP.RequisitionType AS RequisitionTypeValue,CONVERT(NVARCHAR(12), CP.ReqRaisedOn,106) ReqRaisedOn, ");
                    query.Append(" CASE CP.RecordStatus  WHEN 'NCF' THEN 'Not Confirmed' WHEN 'CHK' THEN 'Checked' WHEN 'CNF' THEN 'Confirmed' WHEN 'APV' THEN 'Approved' END RecordStatus, ");
                    query.Append(" CP.RequiredByTime,CP.RequisitionFrom,CP.RequisitionTo, (ISNULL(U.FirstName,'') + ' ' + ISNULL(U.MiddleName,'')+ ' ' +ISNULL(U.LastName,'')) ReqRaisedByName,U.UserID ReqRaisedBy, S.SupplierCode, ISNULL(S.SupplierName,'') SupplierName,ST.StoreName ");
                    query.Append(" FROM PRQ_ChemPurcReq AS CP INNER JOIN SYS_Store AS ST ON ST.StoreID =CP.RequisitionFrom ");
                    query.Append(" LEFT JOIN Sys_Supplier AS S ON S.SupplierID=CP.RequisitionTo LEFT JOIN Security.Users U ON U.UserID=CP.ReqRaisedBy  WHERE CP.RecordStatus !='APV' ");
                    // where CP.RecordStatus !='APV'
                }
                else if(pageMode.Equals("APV"))
                {
                    query.Append(" SELECT CP.RequisitionID,CP.RequisitionNo,CASE WHEN CP.RequisitionCategory='FP' THEN 'Foreign' ELSE 'Local' END RequisitionCategory,");
                    query.Append(" CASE WHEN CP.RequisitionType='UR' THEN 'Urgent' ELSE 'Normal' END RequisitionType, ");
                    query.Append(" CONVERT(NVARCHAR(12), CP.ReqRaisedOn,106) ReqRaisedOn, ISNULL(CP.ApprovalAdviceNote,'') ApprovalAdviceNote, ");
                    query.Append(" CASE CP.RecordStatus WHEN 'NCF' THEN 'Not Confirmed' WHEN 'CHK' THEN 'Checked'  WHEN 'CNF' THEN 'Confirmed' WHEN 'APV' THEN 'Approved' END RecordStatus, ");
                    query.Append(" CP.RequiredByTime,CP.RequisitionFrom,CP.RequisitionTo, (ISNULL(U.FirstName,'')+' '+ISNULL(U.MiddleName,'')+' ' +ISNULL(U.LastName,'')) ReqRaisedByName,U.UserID ReqRaisedBy, S.SupplierCode, ISNULL(S.SupplierName,'') SupplierName,ST.StoreName ");
                    query.Append(" FROM PRQ_ChemPurcReq AS CP INNER JOIN SYS_Store AS ST ON ST.StoreID =CP.RequisitionFrom  ");
                    query.Append(" LEFT JOIN Sys_Supplier AS S ON S.SupplierID=CP.RequisitionTo LEFT JOIN Security.Users U ON U.UserID=CP.ReqRaisedBy  ");
                    query.Append(" WHERE CP.RecordStatus !='NCF' AND  CP.RecordStatus !='CHK'");
                   
                }
                else //This is for Chemical Order against requisition 
                {
                    query.Append(" SELECT CP.RequisitionID,CP.RequisitionNo,CASE WHEN CP.RequisitionCategory='FP' THEN 'Foreign' ELSE 'Local' END RequisitionCategory,");
                    query.Append(" CASE WHEN CP.RequisitionType='UR' THEN 'Urgent' ELSE 'Normal' END RequisitionType, ");
                    query.Append(" CONVERT(NVARCHAR(12), CP.ReqRaisedOn,106) ReqRaisedOn, ISNULL(CP.ApprovalAdviceNote,'') ApprovalAdviceNote, ");
                    query.Append(" CASE CP.RecordStatus WHEN 'NCF' THEN 'Not Confirmed' WHEN 'CHK' THEN 'Checked'  WHEN 'CNF' THEN 'Confirmed' WHEN 'APV' THEN 'Approved' END RecordStatus, ");
                    query.Append(" CP.RequiredByTime,CP.RequisitionFrom,CP.RequisitionTo, (ISNULL(U.FirstName,'')+' '+ISNULL(U.MiddleName,'')+' ' +ISNULL(U.LastName,'')) ReqRaisedByName,U.UserID ReqRaisedBy, S.SupplierCode, ISNULL(S.SupplierName,'') SupplierName,ST.StoreName ");
                    query.Append(" FROM PRQ_ChemPurcReq AS CP INNER JOIN SYS_Store AS ST ON ST.StoreID =CP.RequisitionFrom  ");
                    query.Append(" LEFT JOIN Sys_Supplier AS S ON S.SupplierID=CP.RequisitionTo LEFT JOIN Security.Users U ON U.UserID=CP.ReqRaisedBy  ");
                    query.Append(" WHERE CP.RecordStatus ='APV'");
                    if (!string.IsNullOrEmpty(reqCategory)) 
                    {
                        query.Append(" AND CP.RequisitionCategory='" + reqCategory + "'");
                    }
                }
                var items = _context.Database.SqlQuery<PrqChemPurcReq>(query.ToString());
                return items.ToList().OrderByDescending(o => o.RequisitionID);
            }
        }

        public IEnumerable<ChemicalPurchaseItemModel> GetAllChemicalPurchaseItems()
        {
            var query = new StringBuilder();
            using (_context)
            {
                query.Append(" SELECT T1.ItemID,T1.ItemName,T1.UnitID,T1.UnitName,T1.HSCode,ISNULL(T4.StockQty,0) StockQty,ISNULL(T3.PipelineQty,0) PipelineQty,");
                query.Append(" (ISNULL(T4.StockQty,0) + ISNULL(T3.PipelineQty,0)) AS TotalQty, ISNULL(T1.ReorderLevel,0) ReorderLevel,ISNULL(T1.SafetyStock,0) SafetyStock, ISNULL(T2.ProdReqQty,0)ProdReqQty ");
                query.Append(" FROM (SELECT CI.ItemID,CI.ItemName,U.UnitID,U.UnitName,CI.HSCode,ISNULL(CI.SafetyStock,0) SafetyStock,ISNULL(CI.ReorderLevel,0) ReorderLevel ");
                query.Append(" FROM Sys_ChemicalItem CI INNER JOIN Sys_Unit U ON CI.UnitID=U.UnitID WHERE CI.IsActive=1 ) T1 ");
                query.Append(" LEFT JOIN (SELECT ItemID, SUM(ISNULL(RequsitionQty,0)) ProdReqQty FROM PRD_ChemProdReqItem  GROUP BY ItemID) T2 ON T1.ItemID=T2.ItemID  ");
                query.Append(" LEFT JOIN (SELECT ItemID, SUM(ISNULL(PLQty,0)) PipelineQty FROM LCM_PackingListItem  GROUP BY ItemID) T3 ON T1.ItemID=T3.ItemID   ");
                query.Append(" LEFT JOIN (SELECT inv.ItemID,ISNULL(SUM(inv.ClosingQty),0) StockQty FROM dbo.INV_ChemStockItem inv INNER JOIN (SELECT MAX(TransectionID)TransectionID,StoreID,ItemID,UnitID,PackSize,SizeUnit FROM dbo.INV_ChemStockItem  ");
                query.Append(" GROUP BY StoreID,ItemID,UnitID,PackSize,SizeUnit) sub ON inv.TransectionID=sub.TransectionID GROUP BY inv.ItemID ) T4 ON T1.ItemID=T4.ItemID");
                var items = _context.Database.SqlQuery<ChemicalPurchaseItemModel>(query.ToString());
                return items.ToList().OrderBy(o => o.ItemName);
            }
        }
        public IEnumerable<PrqChemPurcReqItem> GetRequisitionItemList(int requisitionId)
        {
            var query = new StringBuilder();
            using (_context)
            {
                query.Append(" SELECT PRI.RequisitionItemID,CI.ItemID,CI.ItemName,ISNULL(S.SizeID,null)SizeID,ISNULL(S.SizeName,'')SizeName,ISNULL(SU.UnitID,null) PackUnitID,ISNULL(SU.UnitName,'') PackUnitName,");
                query.Append(" PRI.PackQty,PRI.RequsitionQty RequisitionQty,RU.UnitID,RU.UnitName,ISNULL(SP.SupplierID,null) ManufacturerID, ISNULL(SP.SupplierName,'') ManufacturerName,ISNULL(CI.ReorderLevel,0)ReorderLevel,");
                query.Append(" ISNULL(STK.SafetyStock,0)SafetyStock, ISNULL(PL.PLQty,0) PipelineQty, CASE  WhEN PL.PLQty is null THEN  ISNULL(STK.SafetyStock,0) ELSE ISNULL(PL.PLQty,0) + ISNULL(STK.SafetyStock,0) END TotalQty,");
                query.Append(" ISNULL(PRI.ApproveQty,PRI.RequsitionQty)ApproveQty,ISNULL(PR.ProdReqQty,0)ProdReqQty,");
                query.Append(" CASE PRI.ApprovalState WHEN 'ATL' THEN 'Advice to Take Loan' WHEN 'ATP' THEN 'Advice to Purchase'");
                query.Append(" WHEN 'RTP' THEN 'Rejected Purchase' WHEN 'POR' THEN 'Purchase by Other Requisition' WHEN 'ATL' THEN 'Advice to Take Loan' ELSE '' END ApprovalState");
                query.Append(" FROM  PRQ_ChemPurcReqItem PRI LEFT JOIN Sys_Size S ON S.SizeID=PRI.PackSize LEFT JOIN Sys_Unit SU ON SU.UnitID=PRI.SizeUnit");
                query.Append(" LEFT JOIN Sys_Unit RU ON RU.UnitID=PRI.RequisitionUnit LEFT JOIN Sys_Supplier SP ON SP.SupplierID=PRI.ManufacturerID ");
                query.Append(" LEFT JOIN Sys_ChemicalItem CI ON CI.ItemID=PRI.ItemID LEFT JOIN  (SELECT ItemID, ISNULL(SUM(PLQty),0)PLQty FROM LCM_PackingListItem ");
                query.Append(" GROUP BY ItemID )PL ON PL.ItemID=PRI.ItemID LEFT JOIN ( SELECT INV.ItemID, ISNULL(SUM(INV.ClosingQty),0)SafetyStock FROM INV_ChemStockItem INV ");
                query.Append(" INNER JOIN (SELECT MAX(TransectionID)TransectionID,StoreID,ItemID,UnitID,PackSize,SizeUnit FROM INV_ChemStockItem ");
                query.Append(" GROUP BY StoreID,ItemID,UnitID,PackSize,SizeUnit) SUB  ON INV.TransectionID=SUB.TransectionID GROUP BY  INV.ItemID ");
                query.Append(" ) AS STK ON STK.ItemID=PRI.ItemID LEFT JOIN  (SELECT  ItemID, ISNULL(SUM(RequsitionQty),0) ProdReqQty From  PRD_ChemProdReqItem GROUP BY ItemID ");
                query.Append(" ) AS PR ON PR.ItemID=PRI.ItemID  ");
                query.Append("  WHERE PRI.RequisitionID='" + requisitionId + "'");
                
                var items = _context.Database.SqlQuery<PrqChemPurcReqItem>(query.ToString());
                return items.ToList().OrderByDescending(o=>o.RequisitionItemID);
            }
        }
        private static string ReturnApprovalStateToShort(string stateName)
        {
            switch (stateName)
            {
                case "Advice to Take Loan":
                    return "ATL";
                case "Advice to Purchase":
                    return "ATP";
                case "Rejected Purchase":
                    return "RTP";
                case "Purchase by Other Requisition":
                    return "POR";
                default:
                    return "";
            }
        }
        public ValidationMsg DeletedChemPurcReq(int requisitionId)
        {
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        _context.PRQ_ChemPurcReqItem.RemoveRange(_context.PRQ_ChemPurcReqItem.Where(m => m.RequisitionID == requisitionId));

                        _context.PRQ_ChemPurcReq.RemoveRange(_context.PRQ_ChemPurcReq.Where(m => m.RequisitionID == requisitionId));

                        _context.SaveChanges();
                    }
                    tx.Complete();
                    _vmMsg.Type = Enums.MessageType.Success;
                    _vmMsg.Msg = "Deleted Successfully.";

                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Delete.";
            }
            return _vmMsg;
        }


        public ValidationMsg DeletedChemPurcReqItem(long requisitionItemId, string recordStatus)
        {
            try
            {
                if (recordStatus.Equals("Confirmed") || recordStatus.Equals("CNF"))
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Confirmation record can not be deleted.";
                    return _vmMsg;
                }
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        _context.PRQ_ChemPurcReqItem.RemoveRange(_context.PRQ_ChemPurcReqItem.Where(m => m.RequisitionItemID == requisitionItemId));

                        _context.SaveChanges();
                    }
                    tx.Complete();
                    _vmMsg.Type = Enums.MessageType.Success;
                    _vmMsg.Msg = "Deleted Successfully.";

                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Delete.";
            }
            return _vmMsg;
        }

        public ValidationMsg CheckData(int requisitionId, string chkComment, int userId)
        {
            var prqChemPurcReq = _unit.PrqChemPurcReqRepository.GetByID(requisitionId);
            prqChemPurcReq.CheckedBy = userId;
            prqChemPurcReq.CheckDate = DateTime.Now;
            prqChemPurcReq.RecordStatus = "CHK";
            _unit.PrqChemPurcReqRepository.Update(prqChemPurcReq);
            if (_unit.IsSaved())
            {
                _vmMsg.Type = Enums.MessageType.Success;
                _vmMsg.Msg = "Checked Successfully.";
            }
            else
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to confirm check.";
            }
            return _vmMsg;
        }

        public ValidationMsg ConfirmData(int requisitionId, string cnfComment, int userId)
        {
            var prqChemPurcReq = _unit.PrqChemPurcReqRepository.GetByID(requisitionId);
            prqChemPurcReq.CheckedBy = userId;
            prqChemPurcReq.CheckDate = DateTime.Now;
            prqChemPurcReq.RecordStatus = "CNF";
            prqChemPurcReq.ApprovalAdviceNote = cnfComment;
            _unit.PrqChemPurcReqRepository.Update(prqChemPurcReq);
            if (_unit.IsSaved())
            {
                _vmMsg.Type = Enums.MessageType.Success;
                _vmMsg.Msg = "Confirmed Successfully.";
            }
            else
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Confirmation Failed.";
            }
            return _vmMsg;
        }

        public ValidationMsg ApprovedData(PrqChemPurcReq model, int userId)
        {
            try
            {
                var prqChemPurcReq = _unit.PrqChemPurcReqRepository.GetByID(model.RequisitionID);
                prqChemPurcReq.ApprovedBy = userId;
                prqChemPurcReq.ApproveDate = DateTime.Now;
                prqChemPurcReq.RecordStatus = "APV";
                prqChemPurcReq.ApprovalAdvice = model.ApprovalAdvice;
                prqChemPurcReq.ApprovalAdviceNote = model.ApprovalAdviceNote;
                _unit.PrqChemPurcReqRepository.Update(prqChemPurcReq);

                if (model.ChemPurcReqItems.Count != 0)
                {
                    foreach (var items in model.ChemPurcReqItems)
                    {
                        var objItem = _unit.PrqChemPurcReqItemRepository.GetByID(items.RequisitionItemID);
                        objItem.PackSize = items.SizeID;
                        objItem.PackQty = items.PackQty;
                        objItem.ApproveQty = Convert.ToDecimal(items.ApproveQty);
                        objItem.ApprovalState = ReturnApprovalStateToShort(items.ApprovalState);
                        _unit.PrqChemPurcReqItemRepository.Update(objItem);
                    }
                }
                if (_unit.IsSaved())
                {
                    _vmMsg.Type = Enums.MessageType.Success;
                    _vmMsg.Msg = "Approved Successfully.";
                }
                else
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Approval Failed.";
                }
            }
            catch (Exception)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Approval Failed.";
            }
            return _vmMsg;
        }

        public ValidationMsg CreateChemicalOrderByRequisition(string requisitionIdList, int userId)
        {
            //try
            //{
            //    using (var tx = new TransactionScope())
            //    {
                    
            //        using (_context)
            //        {
            //            var requisitionIdArray = requisitionIdList.Split(',');

            //            foreach (var reqId in requisitionIdArray)
            //            {
            //                var objReq = _context.PRQ_ChemPurcReq.FirstOrDefault(r => r.RequisitionID ==Convert.ToInt32(reqId));
            //                var chemPurcReq = new PRQ_ChemPurcReq
            //                {
            //                    RequisitionNo = objReq.RequisitionNo,
            //                    RequisitionCategory = objReq.RequisitionCategory,
            //                    RequisitionType = objReq.RequisitionType,
            //                    ReqRaisedOn =objReq.ReqRaisedOn,
            //                    RequisitionFrom = objReq.RequisitionFrom,
            //                    RequisitionTo = objReq.RequisitionTo,
            //                    RequiredByTime = objReq.RequiredByTime,
            //                    ReqRaisedBy = objReq.ReqRaisedBy,
            //                    RecordStatus = "NCF",
            //                    RequisitionState = "RNG",
            //                    SetOn = DateTime.Now,
            //                    SetBy = userId,
            //                    IPAddress = GetIPAddress.LocalIPAddress()
            //                };
            //                _context.PRQ_ChemPurcReq.Add(chemPurcReq);
            //                _context.SaveChanges();

            //            }
                        

            //            currentRequisitionId = chemPurcReq.RequisitionID;

            //            if (modelReq.ChemPurcReqItems != null)
            //            {
            //                foreach (var chemItem in modelReq.ChemPurcReqItems.Select(item => new PRQ_ChemPurcReqItem()
            //                {
            //                    RequisitionID = currentRequisitionId,
            //                    ItemID = item.ItemID,
            //                    SupplierID = modelReq.RequisitionTo,
            //                    PackSize = item.SizeID,
            //                    SizeUnit = item.PackUnitID,
            //                    PackQty = item.PackQty,
            //                    RequsitionQty = item.RequisitionQty,
            //                    RequisitionUnit = item.RequisitionUnit,
            //                    ManufacturerID = item.ManufacturerID,
            //                    SetOn = DateTime.Now,
            //                    SetBy = userId
            //                }))
            //                {
            //                    _context.PRQ_ChemPurcReqItem.Add(chemItem);
            //                    _context.SaveChanges();
            //                }
            //            }

            //        }
            //        tx.Complete();

            //        _vmMsg.Type = Enums.MessageType.Success;
            //        _vmMsg.Msg = "Saved Successfully.";
            //        _vmMsg.ReturnId = currentRequisitionId;
            //        _vmMsg.ReturnCode = currentRequisitionCode;
            //    }
            //}
            //catch (Exception)
            //{
            //    _vmMsg.Type = Enums.MessageType.Error;
            //    _vmMsg.Msg = "Failed to save.";
            //}
            return _vmMsg;
        }
    }

}
