using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.AppSetupModel;
using SecurityAdministration.BLL.ViewModels;
using ERP.EntitiesModel.OperationModel;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalPrqPurchaseChallan
    {
        private readonly BLC_DEVEntities _context;

        public DalPrqPurchaseChallan()
        {
            _context = new BLC_DEVEntities();
        }


        public List<PrqPurchaseChallan> GetChallanListInfo()
        {
            var queryString = "SELECT ChallanID,ChallanNo FROM [dbo].[Prq_PurchaseChallan]";
            var iChallanList = _context.Database.SqlQuery<PrqPurchaseChallan>(queryString);
            return iChallanList.ToList();
        }

        public List<PrqPurchaseChallan> GetChallanListInfo(string supplierId)
        {
            var queryString = "SELECT ChallanID,ChallanNo,ChallanCategory FROM [dbo].[Prq_PurchaseChallan]" +
                              " WHERE PurchaseID IN (SELECT PurchaseID from [dbo].[Prq_Purchase]" +
                              " WHERE SupplierID = '" + supplierId + "')";
            var iChallanList = _context.Database.SqlQuery<PrqPurchaseChallan>(queryString);
            return iChallanList.ToList();
        }

        public List<UspGetPurchaseIdBySupplierAndStoreId_Result> GetPurchaseListInfo(string suplierId, string storeId)
        {
            int SupplierID = Convert.ToInt32(suplierId);
            int ReceiveStore = Convert.ToInt32(storeId);           
            var lstPurchase = _context.UspGetPurchaseIdBySupplierAndStoreId(SupplierID,ReceiveStore).ToList();

            //var iChallanInof = _context.UspGetPurchaseIdBySupplierAndStoreId(SupplierID, ReceiveStore);
            return lstPurchase;
        }

        public string GetChallanCategoryByChallanId(long challanId)
        {
            string value =
                _context.Prq_PurchaseChallan.Where(w => w.ChallanID.Equals(challanId))
                    .SingleOrDefault()
                    .ChallanCategory;
            return value; 
        }

        public List<SysSupplier> GetSupplierByStockID(string storeId)
        {
            var query = @"Select distinct p.SupplierID,s.SupplierName from Prq_Purchase p 
                          INNER JOIN Sys_Supplier s ON p.SupplierID = s.SupplierID
                          WHERE p.PurchaseID IN(
                          Select PurchaseID from Prq_PurchaseChallan pc WHERE pc.ReceiveStore = '"+storeId+"' ) AND s.IsActive = 'true' AND s.IsDelete='false'";
            var iSupplierList = _context.Database.SqlQuery<SysSupplier>(query);
            return iSupplierList.ToList();
        }
        public List<SysStore> GetStoreLoactionList(string supplierId)
        {
            var query = "Select distinct s.StoreID, s.StoreName from Prq_Purchase as p" +
                         " inner join Prq_PurchaseChallan as pc" +
                         " on p.PurchaseID = pc.PurchaseID" +
                         " inner join SYS_Store as s" +
                         " on pc.ReceiveStore = s.StoreID AND s.StoreCategory='Leather' AND s.StoreType='Raw Hide'" +
                         " where SupplierID = '" + supplierId + "'";
            var iStoreList = _context.Database.SqlQuery<SysStore>(query);
            return iStoreList.ToList();
        }

       public List<Prq_PreGradeSelection> GetSelectionListID(int store, int supplier, string dateFrom, string  dateTo) 
        {
            DateTime fromdate = DalCommon.SetDate(dateFrom);
            DateTime toDate = DalCommon.SetDate(dateTo);
            List<Prq_PreGradeSelection> result = (from temp in _context.Prq_PreGradeSelection
                                                  where temp.SelectionStore == store && temp.SupplierID == supplier && temp.RecordStatus == "CNF" && temp.SelectionDate >= fromdate && temp.SelectionDate <= toDate
                                                  select temp).OrderByDescending(o =>o.SelectionID).ToList();
            return result.ToList();

        }

       public List<Prq_Purchase> GetPurchaseListID(int supplier, string dateFrom, string dateTo)
       {
           DateTime fromdate = DalCommon.SetDate(dateFrom);
           DateTime toDate = DalCommon.SetDate(dateTo);
           List<Prq_PreGradeSelection> result = (from temp in _context.Prq_PreGradeSelection
                                                 where temp.SupplierID == supplier && temp.RecordStatus == "CNF" && temp.SelectionDate >= fromdate && temp.SelectionDate <= toDate
                                                 select temp).ToList();
           var lstPurchase = (from temp in result
                                            join temp2 in _context.Prq_Purchase on temp.PurchaseID equals temp2.PurchaseID
                                            select new {
                                                PurchaseID = temp2.PurchaseID,
                                                PurchaseNo = temp2.PurchaseNo,
                                                PurchaseDate = temp2.PurchaseDate
                                            }).AsEnumerable();

           List<Prq_Purchase> lstPo = new List<Prq_Purchase>();
           foreach (var item in lstPurchase)
           {
               Prq_Purchase ob = new Prq_Purchase();
               ob.PurchaseID = item.PurchaseID;
               ob.PurchaseNo = item.PurchaseNo;
               ob.PurchaseDate = item.PurchaseDate;
               lstPo.Add(ob);
           }
           return lstPo;

       }
    }
}
