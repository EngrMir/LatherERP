using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class TransRequest
    {
        public long RequestID { get; set; }
        public string RequestType { get; set; }
        public string RequestFrom { get; set; }
        public string RequestTo { get; set; }
        public string RequestFromName { get; set; }
        public string RequestToName { get; set; }
        public string RequestDate { get; set; }
        public string RequestNo { get; set; }
        public string FromSource { get; set; }
        public string RequestCode { get; set; }
        public string ReturnMethod { get; set; }
        public string ExpectetReturnTime { get; set; }
        public string RecordStatus { get; set; }
        public string CheckComments { get; set; }
        public string ApproveComments { get; set; }
        public string ToSource { get; set; }
        public virtual ICollection<TransRequestItem> ChemicalSelectedList { get; set; }
         public string ReceiveReqNo { get; set; }
          public string ReturnReqNo { get; set; }
          //public string Remarks { get; set; }
          //public string ReturnDate { get; set; }
          //public string CheckedBy { get; set; }
          //public string RecommendComments { get; set; }
          //public string ApprovedBy { get; set; }
          //public string RecommendBy { get; set; }
          //public int DataStatus { get; set; }

         //public string TransRequestItemID { get; set; }
         //   public string ItemID { get; set; }
         //   public string ItemName{ get; set; }
         //   public string ReceiveQty { get; set; }
         //   public string ReceiveUnitID { get; set; }
         //   public string ReceiveUnitName{ get; set; }
         //   public string AlreadyReturnQty{ get; set; }
         //   public string RemainingQty { get; set; }
         //   public string ReturnMethodID { get; set; }
         //   public string ReturnMethodValue{ get; set; }
         //   public string ReceiveCurrencyID  { get; set; }
         //   public string ReceiveCurrencyName{ get; set; }
         //   public string ReceiveRate{ get; set; }
         //   public string ReceiveValue{ get; set; }

         //public string ReturnItemID { get; set; }
         //                    public string ReturnItemName{ get; set; }
         //                    public string ReturnUnitID { get; set; }
         //                    public string ReturnUnitIDName{ get; set; }
         //                    public string ReturnCurrencyID { get; set; }
         //                    public string ReturnCurrencyName { get; set; }
         //                    public string ReturnExchangeRate { get; set; }
         //                    public string ReturnRate { get; set; }
         //                    public string ReturnQuantity { get; set; }
    }


    public class TransRequestItem
    {
        public long TransRequestItemID { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public string PackSize { get; set; }
        public string SizeID { get; set; }
        public string SizeUnit { get; set; }
        public string SizeUnitID { get; set; }
        public int PackQty { get; set; }
        public int RefItemID { get; set; }
        public int RefSupplierID { get; set; }
        public string RefSupplierName { get; set; }
        public string ReturnMethod { get; set; }
        public decimal ReferenceQty { get; set; }
        public string ReferenceUnit { get; set; }
        public string ReferenceUnitID { get; set; }
        public int SupplierID { get; set; }

    }
}
