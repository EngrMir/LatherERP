using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class LoanReturnIssue
    {
     public long RequestID { get; set; }
     public string RequestType { get; set; }
     public string RequestFrom { get; set; }
     public string Remarks { get; set; }
     public string RequestNo { get; set; }
     public string RequestDate { get; set; }
     public string ReturnMethod { get; set; }
     public string ExpectetReturnTime { get; set; }
     public string CheckedBy { get; set; }
     public string CheckComments { get; set; }
     public string RecommendBy { get; set; }
     public string RecommendComments { get; set; }
     public string ApprovedBy { get; set; }
     public string ApproveComments { get; set; }
     public long RefRequestID { get; set; }

     public string RequestTo { get; set; }
     public string ToSource { get; set; }
     public virtual ICollection<LoanReturnIssueItems> lstLoanReturnIssueItems { get; set; }
    }
      
    public class LoanReturnIssueItems
    {
        public string ItemID { get; set; }
        public int ReceiveUnitID { get; set; }
        public long TransRequestItemID { get; set; }
        public string ReceiveCurrencyID { get; set; }
         public string ReturnMethodID { get; set; }
         public string ReturnItemID { get; set; }
         public string ReturnUnitID { get; set; }
         public string ReturnCurrencyID { get; set; }
        public string ItemName { get; set; }
        public decimal ReceiveQty { get; set; }
        public string ReceiveUnitName { get; set; }
        public decimal AlreadyReturnQty { get; set; }
        public decimal RemainingQty { get; set; }
        public string ReturnMethodValue { get; set; }
        public string ReceiveCurrencyName { get; set; }
        public decimal ReceiveRate { get; set; }
        public decimal ReceiveValue { get; set; }
        public string ReturnUnitIDName { get; set; }
        public string ReturnCurrencyName { get; set; }
        public decimal ReturnRate { get; set; }
        public decimal ReturnExchangeRate { get; set; }
        public decimal ReturnQuantity { get; set; }
        public decimal ReferenceQty { get; set; }
        public string ReferenceUnit { get; set; }
       
    }

}
