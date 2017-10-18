using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ERP.EntitiesModel.OperationModel
{
    public class LoanReturnRequestInfo
    {
        public Nullable<long> RequestID { get; set; }
        public string RequestType { get; set; }
        public string RequestFrom { get; set; }
        public string RequestFromName { get; set; }
        public string Remarks { get; set; }
        public string RequestNo { get; set; }
        public string RequestDate { get; set; }
        public string ReturnDate { get; set; }
        public string ReturnMethod { get; set; }
        public Nullable<int> ExpectetReturnTime { get; set; }
        public string CheckedBy { get; set; }
        public string CheckComments { get; set; }
        public string RecommendBy { get; set; }
        public string RecommendComments { get; set; }
        public string ApprovedBy { get; set; }
        public string ApproveComments { get; set; }
        public Nullable<long> TransRequestItemID { get; set; }
        public Nullable<int> ItemID { get; set; }
        public string ItemName { get; set; }
        public Nullable<decimal> ReceiveQty { get; set; }
        public Nullable<int> ReceiveUnitID { get; set; }
        public string ReceiveUnitName { get; set; }
        public Nullable<decimal> AlreadyReturnQty { get; set; }
        public Nullable<decimal> RemainingQty { get; set; }
        public string ReturnMethodID { get; set; }
        public string ReturnMethodValue { get; set; }
        public Nullable<int> ReceiveCurrencyID { get; set; }
        public string ReceiveCurrencyName { get; set; }
        public Nullable<decimal> ReceiveRate { get; set; }
        public Nullable<decimal> ReceiveValue { get; set; }
        public Nullable<int> ReturnItemID { get; set; }
        public string ReturnItemName { get; set; }
        public Nullable<int> ReturnUnitID { get; set; }
        public string ReturnUnitIDName { get; set; }
        public Nullable<int> ReturnCurrencyID { get; set; }
        public string ReturnCurrencyName { get; set; }
        public Nullable<decimal> ReturnExchangeRate { get; set; }
        public Nullable<decimal> ReturnRate { get; set; }
        public Nullable<decimal> ReturnQuantity { get; set; }
        public Nullable<int> DataStatus { get; set; }
    }
}
