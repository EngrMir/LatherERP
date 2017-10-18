using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;

namespace ERP.EntitiesModel.OperationModel
{
    public class INVStoreTrans : CommonStatusInformation
    {
        public long TransactionID { get; set; }
        public string TransactionNo { get; set; }
        public string TransactionDate { get; set; }
        public string AdjustmentType { get; set; }
        public DateTime? TransactionDateTemp { get; set; }
        public string TransactionCategory { get; set; }
        public string TransactionType { get; set; }
        public string TransactionFrom { get; set; }
        public string TransactionFromName { get; set; }
        public string TransactionTo { get; set; }
        public string TransactionToName { get; set; }
        public long RefTransactionID { get; set; }
        public string RefTransactionNo { get; set; }
        public string TransactionStatus { get; set; }
        public string TransactionState { get; set; }
        public int CheckedBy { get; set; }
        public string CheckDate { get; set; }
        public string CheckComments { get; set; }
        public string Remark { get; set; }
        public string IssueFrom { get; set; }
        public string IssueTo { get; set; }
        public int RequisitionID { get; set; }
        public string RequisitionNo { get; set; }
        public string RequisitionCategory { get; set; }
        public string RequisitionType { get; set; }
        public string ReqRaisedOn { get; set; }
        public string RequiredByTime { get; set; }
        public string ReqRaisedByName { get; set; }
        public string JobOrderNo { get; set; }
        public string RecordStatus { get; set; }
        public string RecordStatusName { get; set; }
        public string FromSource { get; set; }
        public string FromSourceName { get; set; }
        public string ToSource { get; set; }
        public string ToSourceName { get; set; }
        public string ReqFromDate { get; set; }

        public DateTime? ReqFromDateTemp { get; set; }
        public string ReqToDate { get; set; }
        public DateTime? ReqToDateTemp { get; set; }

        public string PageName { get; set; }
        public virtual INVStoreTrans TransactionInfo { get; set; }
        public virtual IList<PRDChemProdReqItem> TransactionItemList { get; set; }

        #region Change Hamza

        public virtual IList<INVStoreTransChallan> InvStoreTransChallanList { get; set; }
        public virtual IList<INVStoreTransItem> InvStoreTransItemList { get; set; }
        public virtual IList<INVStoreTransRequest> InvStoreTransRequestList { get; set; }
        public int? SupplierID { get; set; }
        public string SupplierName { get; set; }
        public string SupplierCode { get; set; }
        
        #endregion
    }
}
