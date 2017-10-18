
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.OperationModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalLoanReturnRequest
    {
        private UnitOfWork repository = new UnitOfWork();
        private BLC_DEVEntities db = new BLC_DEVEntities();
        private readonly string _connString = string.Empty;
         public DalLoanReturnRequest(){
             _connString = StrConnection.GetConnectionString();
         }

         public object GetLoanReturnRequestInfoByID(long RequestID)
         {
             //var d = Convert.ToInt64(RequestID);
             //var RefID=(from t in db.INV_TransRequestRef where t.RequestID == d select t.RefRequestID).FirstOrDefault();
           
             var res = (from temp in db.INV_TransRequest.AsEnumerable()  where temp.RequestType == "RLRR" && temp.RequestID==RequestID
                       select new
                       {
                             RequestID = temp.RequestID,
                             //RefRequestID = RefID,                           
                             RequestType = temp.RequestType,
                             RequestFrom = temp.RequestFrom,
                             RequestFromName =db.SYS_Store.Find(Convert.ToInt32(temp.RequestFrom == null ? "0" : temp.RequestFrom)).StoreName,  
                             RequestTo =temp.RequestTo==null?"":temp.RequestTo,
                             RequestToName=temp.ToSource==null?"": temp.ToSource=="SUP"?db.Sys_Supplier.Find(Convert.ToInt32(temp.RequestTo)).SupplierName:db.Sys_Buyer.Find(Convert.ToInt32(temp.RequestTo)).BuyerName,
                             Remarks = temp.Remarks,
                             RequestNo = temp.RequestNo,
                             RequestDate = temp.RequestDate != null? Convert.ToDateTime(temp.RequestDate).ToString("dd/MM/yyyy"):"",
                             ReturnDate = temp.ExpectetReturnTime,
                             ExpectetReturnTime = temp.ExpectetReturnTime,
                             CheckedBy = temp.CheckedBy,
                             CheckComments = temp.CheckComments,
                             ReturnMethod = temp.ReturnMethod=="DTD"?"Doller to Doller":temp.ReturnMethod=="ESI"?"Exchange Same Item":temp.ReturnMethod=="EOI"?"Exchange Other Item":temp.ReturnMethod,
                             RecommendBy = temp.RecommendBy,
                             RecommendComments = temp.RecommendComments,
                             ApprovedBy = temp.ApprovedBy,
                             ApproveComments = temp.ApproveComments
                        }).ToList();
             
             return res;
         }
         public object GetLoanReturnRequestItemInfoByID(long RequestID,long recRequestID)
         {
             var rev = repository.InvTransRequestItemRepository.Get(filter: ob => ob.RequestID == recRequestID).FirstOrDefault();
             var res = from t in db.INV_TransRequestItem.AsEnumerable() where t.RequestID == RequestID select new {
                 RequestID = t.RequestID,
                 TransRequestItemID = t.TransRequestItemID,
                 ItemID=t.RefItemID,
                 ItemName = t.RefItemID == null ? "" : db.Sys_ChemicalItem.Find(t.RefItemID).ItemName,//t.Sys_ChemicalItem.ItemName,
                 ReceiveQty = rev.TransQty == null ? 0 : Convert.ToDecimal(rev.TransQty),
                 ReceiveUnitID=t.ReferenceUnit,
                 ReceiveUnitName =t.ReferenceUnit==null?"": db.Sys_Unit.Find(t.ReferenceUnit).UnitName,//t.Sys_Unit.UnitName,
                 AlreadyReturnQty=t.ReferenceQty,
                 RemainingQty = (rev.TransQty == null ? 0 : Convert.ToDecimal(rev.TransQty)) - (t.ReferenceQty == null ? 0 : Convert.ToDecimal(t.ReferenceQty)),
                 ReturnMethodID=t.ReturnMethod,
                 ReturnMethodValue=t.ReturnMethod =="DTD"?"Doller to Doller":
                 t.ReturnMethod == "ESI" ? "Exchange Same Item" : t.ReturnMethod == "EOI" ? "Exchange Other Item" : t.ReturnMethod,
                 ReceiveCurrencyID=t.ReferenceCurrency,
                 ReceiveCurrencyName =t.ReferenceCurrency == null ?"":db.Sys_Currency.Find(t.ReferenceCurrency).CurrencyName,//t.Sys_Currency.CurrencyName,
                 ReceiveRate=t.ReferenceRate,
                 ReceiveValue = t.ReferenceQty * t.ReferenceRate,
                 ReturnItemID=t.ItemID,
                 ReturnItemName = t.ItemID == null ? "Press F9" : db.Sys_ChemicalItem.Find(t.ItemID).ItemName,
                 ReturnUnitID=t.ReturnUnit,
                 ReturnUnitIDName = t.ReturnUnit == null ? "" : db.Sys_Unit.Find(t.ReturnUnit).UnitName,
                 ReturnCurrencyID = t.ReturnCurrency,
                 ReturnCurrencyName = t.ReturnCurrency ==null?"":db.Sys_Currency.Find(t.ReturnCurrency).CurrencyName,//t.Sys_Currency1.CurrencyName,
                 ReturnExchangeRate=t.ExchangeRate,
                 ReturnRate=t.ReturnRate,
                 ReturnQuantity= t.ReturnValue
             };
             return res.OrderByDescending(ob=>ob.RequestID);
         }
         public object GetLoanReturnRequestItemInfoByID2(long RequestID)
         {
             var rev = repository.InvTransRequestRefRepository.Get(filter: ob => ob.RequestID == RequestID).FirstOrDefault();
          
             if(rev !=null){
                 var refData = repository.InvTransRequestItemRepository.Get(filter: ob => ob.RequestID == rev.RefRequestID).FirstOrDefault();
            
             var res = from t in db.INV_TransRequestItem.AsEnumerable()
                       where t.RequestID == RequestID
                       select new
                       {
                           RequestID = t.RequestID,
                           TransRequestItemID = t.TransRequestItemID,
                           ItemID = t.RefItemID,
                           ItemName = t.RefItemID == null ? "" : db.Sys_ChemicalItem.Find(t.RefItemID).ItemName,//t.Sys_ChemicalItem.ItemName,
                           ReceiveQty = refData.TransQty == null ? 0 : Convert.ToDecimal(refData.TransQty),
                           ReceiveUnitID = t.ReferenceUnit,
                           ReceiveUnitName = t.ReferenceUnit == null ? "" : db.Sys_Unit.Find(t.ReferenceUnit).UnitName,//t.Sys_Unit.UnitName,
                           AlreadyReturnQty = t.ReferenceQty,
                           RemainingQty = (refData.TransQty == null ? 0 : Convert.ToDecimal(refData.TransQty)) - (t.ReferenceQty == null ? 0 : Convert.ToDecimal(t.ReferenceQty)),
                           ReturnMethodID = t.ReturnMethod,
                           ReturnMethodValue = t.ReturnMethod == "DTD" ? "Doller to Doller" :
                           t.ReturnMethod == "ESI" ? "Exchange Same Item" : t.ReturnMethod == "EOI" ? "Exchange Other Item" : t.ReturnMethod,
                           ReceiveCurrencyID = t.ReferenceCurrency,
                           ReceiveCurrencyName = t.ReferenceCurrency == null ? "" : db.Sys_Currency.Find(t.ReferenceCurrency).CurrencyName,//t.Sys_Currency.CurrencyName,
                           ReceiveRate = t.ReferenceRate,
                           ReceiveValue = t.ReferenceQty * t.ReferenceRate,
                           ReturnItemID = t.ItemID,
                           ReturnItemName = t.ItemID == null ? "Press F9" : db.Sys_ChemicalItem.Find(t.ItemID).ItemName,
                           ReturnUnitID = t.ReturnUnit,
                           ReturnUnitIDName = t.ReturnUnit == null ? "" : db.Sys_Unit.Find(t.ReturnUnit).UnitName,
                           ReturnCurrencyID = t.ReturnCurrency,
                           ReturnCurrencyName = t.ReturnCurrency == null ? "" : db.Sys_Currency.Find(t.ReturnCurrency).CurrencyName,//t.Sys_Currency1.CurrencyName,
                           ReturnExchangeRate = t.ExchangeRate,
                           ReturnRate = t.ReturnRate,
                           ReturnQuantity = t.ReturnValue
                       };
                     return res.OrderByDescending(ob => ob.RequestID);
             }
             return "";
         }

        public object LoanRequestDataWithItem(string type, long RequestID)
        {
            var exist = db.INV_TransRequestRef.Where(ob=>ob.RefRequestID == RequestID).Select(ob=>ob.RequestID).FirstOrDefault();
            if (exist != null) {
                var data = GetLoanReturnRequestItemInfoByID(Convert.ToInt64(exist), RequestID);
               return data;
            }

            var resultSet = db.UspGetLoanReceivedRequestInfo(RequestID).ToList();             
            return resultSet;
        }

        public List<TransRequest> GetLoanReturnRequestInfo()
        {
            string sql = @"DECLARE @Table Table(
	                        RequestID bigint,
	                        RequestNo nvarchar(50),
	                        RequestDate nvarchar(50),
	                        RequestFrom nvarchar(50),
	                        RequestFromName nvarchar(500),
	                        RequestTo nvarchar(50),
	                        ToSource nvarchar(50),	
	                        RecordStatus nvarchar(50),
	                        ReturnMethod nvarchar(50),
	                        RequestToName nvarchar(500),
	                        RequestType nvarchar(50)
                            );

                        INSERT INTO @Table
                        SELECT DISTINCT r.RequestID, r.RequestNo,
		                                CONVERT(varchar(11),r.RequestDate,106)  RequestDate, r.RequestFrom,s.StoreName RequestFromName, 
				                        r.RequestTo,r.ToSource,
                                                 (CASE 
	                                                WHEN r.RecordStatus='NCF' 
		                                                THEN 'Not Confirmend'
	                                                WHEN r.RecordStatus='CNF' 
		                                                THEN 'Confirmend'
	                                                WHEN r.RecordStatus='CHK' 
		                                                THEN 'Checked'
	                                                WHEN r.RecordStatus='APV' 
		                                                THEN 'Approved'
	                                                WHEN r.RecordStatus='RJT' 
		                                                THEN 'Reject'
                                                    WHEN r.RecordStatus='REC' 
		                                                THEN 'Recommanded'
							                        WHEN r.RecordStatus='RCV' 
		                                                THEN 'Received'
	                                                END) RecordStatus,
                                                (CASE 
	                                                WHEN r.ReturnMethod='DTD' 
		                                                THEN 'Doller to Doller'
	                                                WHEN r.ReturnMethod='ESI' 
		                                                THEN 'Exchange Same Item'
	                                                WHEN r.ReturnMethod='EOI' 
		                                                THEN 'Exchange Other Item'
	                       
	                                                END) ReturnMethod, su.SupplierName RequestToName,r.RequestType
                                                   FROM INV_TransRequest r 
                                            LEFT JOIN INV_TransRequestRef rf ON r.RequestID = rf.RefRequestID
                                            LEFT JOIN INV_TransRequestItem ti ON rf.TransRequestRefID = ti.TransRequestRefID
					                        LEFT JOIN SYS_Store s ON r.RequestFrom = s.StoreID
					                        LEFT JOIN Sys_Supplier su ON r.RequestTo = su.SupplierID AND ToSource ='SUP'
					
			                        Update @Table SET RequestToName =T2.StoreName FROM  @Table t1
			                        INNER JOIN (
						                         SELECT b.StoreName,RequestID FROM @Table t2
						                        INNER JOIN SYS_Store b ON t2.RequestTo = b.StoreID 
						                        WHERE t2.ToSource='STR'
					                        ) T2 ON t1.RequestID = T2.RequestID
					
			                        SELECT * FROM @Table WHERE RequestType ='RLRR'";

            var d = db.Database.SqlQuery<TransRequest>(sql).OrderByDescending(ob => ob.RequestID).ToList();
             return d;
        }

        public object SearchLoanReturnRequest(string searchKey)
        {
            string sql = @"SELECT r.RequestID, r.RequestNo ReceiveReqNo,ISNULL(rf.TransRequestRefNo, 0) ReturnReqNo, CONVERT(varchar(11),r.RequestDate,101)  RequestDate,  
                         (CASE 
	                        WHEN r.RecordStatus='NCF' 
		                        THEN 'Not Confirmend'
	                        WHEN r.RecordStatus='CNF' 
		                        THEN 'Confirmend'
	                        WHEN r.RecordStatus='CHK' 
		                        THEN 'Checked'
	                        WHEN r.RecordStatus='APV' 
		                        THEN 'Approved'
	                        WHEN r.RecordStatus='RJT' 
		                        THEN 'Reject'
                            WHEN r.RecordStatus='REC' 
		                        THEN 'Recommanded'
	                        END) RecordStatus
                           FROM INV_TransRequest r 
                    LEFT JOIN INV_TransRequestRef rf ON r.RequestID = rf.RefRequestID
                    LEFT JOIN INV_TransRequestItem ti ON rf.TransRequestRefID = ti.TransRequestRefID
                    WHERE r.RequestType ='LNRR' AND r.RecordStatus='CNF' AND rf.TransRequestRefNo LIKE '" + searchKey + "%'";

            return db.Database.SqlQuery<InvLoanReturnRequestData>(sql);
        }

  

        public List<TransRequest> LoanRequestDatas(string type)
        {
            string sql = @"DECLARE @Table Table(
	                        RequestID bigint,
	                        RequestNo nvarchar(50),
	                        RequestDate nvarchar(50),
	                        RequestFrom nvarchar(50),
	                        RequestFromName nvarchar(500),
	                        RequestTo nvarchar(50),
	                        ToSource nvarchar(50),
	
	                        RecordStatus nvarchar(50),
	                        ReturnMethod nvarchar(50),
	                        RequestToName nvarchar(500),
	                        RequestType nvarchar(50)
                        );

                        INSERT INTO @Table
                        SELECT DISTINCT r.RequestID, r.RequestNo,
		                                CONVERT(varchar(11),r.RequestDate,101)  RequestDate, r.RequestFrom,s.StoreName RequestFromName, 
				                        r.RequestTo,r.ToSource,
                                                 (CASE 
	                                                WHEN r.RecordStatus='NCF' 
		                                                THEN 'Not Confirmend'
	                                                WHEN r.RecordStatus='CNF' 
		                                                THEN 'Confirmend'
	                                                WHEN r.RecordStatus='CHK' 
		                                                THEN 'Checked'
	                                                WHEN r.RecordStatus='APV' 
		                                                THEN 'Approved'
	                                                WHEN r.RecordStatus='RJT' 
		                                                THEN 'Reject'
                                                    WHEN r.RecordStatus='REC' 
		                                                THEN 'Recommanded'
							                        WHEN r.RecordStatus='RCV' 
		                                                THEN 'Received'
	                                                END) RecordStatus,
                                                (CASE 
	                                                WHEN r.ReturnMethod='DTD' 
		                                                THEN 'Doller to Doller'
	                                                WHEN r.ReturnMethod='ESI' 
		                                                THEN 'Exchange Same Item'
	                                                WHEN r.ReturnMethod='EOI' 
		                                                THEN 'Exchange Other Item'
	                       
	                                                END) ReturnMethod, su.SupplierName RequestToName,r.RequestType
                                                   FROM INV_TransRequest r 
                                            LEFT JOIN INV_TransRequestRef rf ON r.RequestID = rf.RefRequestID
                                            LEFT JOIN INV_TransRequestItem ti ON rf.TransRequestRefID = ti.TransRequestRefID
					                        LEFT JOIN SYS_Store s ON r.RequestFrom = s.StoreID
					                        LEFT JOIN Sys_Supplier su ON r.RequestTo = su.SupplierID AND ToSource ='SUP'
					
			                        Update @Table SET RequestToName =T2.StoreName FROM  @Table t1
			                        INNER JOIN (
						                         SELECT b.StoreName,RequestID FROM @Table t2
						                        INNER JOIN SYS_Store b ON t2.RequestTo = b.StoreID 
						                        WHERE t2.ToSource='STR'
					                        ) T2 ON t1.RequestID = T2.RequestID
					
			                        SELECT * FROM @Table
					                        WHERE RecordStatus='Received' AND RequestType='" + type + "'";
            
            return db.Database.SqlQuery<TransRequest>(sql).ToList();
        }
    }
}
