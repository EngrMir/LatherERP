using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class InvFNTransfer
    {
        public long FNTransferID { get; set; }
        public string FNTransferNo { get; set; }
        public string FNTransferDate { get; set; }
        public string FNTransferCategory { get; set; }
        public string TranrsferType { get; set; }
        public Nullable<byte> TransactionStore { get; set; }
        public Nullable<byte> IssueStore { get; set; }
        public Nullable<long> ReferenceJobOrdNo { get; set; }
        public string RecordStatus { get; set; }
        public Nullable<int> IssuedBy { get; set; }
        public Nullable<System.DateTime> IssueDate { get; set; }
        public string IssueNote { get; set; }
        public Nullable<int> CheckedBy { get; set; }
        public Nullable<System.DateTime> CheckDate { get; set; }
        public string CheckNote { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public Nullable<System.DateTime> ApproveDate { get; set; }
        public string ApproveNote { get; set; }
        public Nullable<System.DateTime> SetOn { get; set; }
        public Nullable<int> SetBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public string IPAddress { get; set; }



        public virtual IList<InvFNTransferFrom> InvFNTransferFromList { get; set; }
        public virtual IList<InvFNTransferTo> InvFNTransferToList { get; set; }


        #region Display Field

        public Nullable<int> BuyerID { get; set; }
        public Nullable<long> BuyerOrderID { get; set; }
        public Nullable<int> ArticleID { get; set; }
        public Nullable<int> ColorID { get; set; }
        public Nullable<byte> ItemTypeID { get; set; }
        public Nullable<byte> LeatherTypeID { get; set; }
        public Nullable<byte> LeatherStatusID { get; set; }
        public Nullable<short> GradeID { get; set; }
        public Nullable<byte> StoreID { get; set; }


        public string BuyerName { get; set; }
        public string StoreName { get; set; }
        public string BuyerOrderNo { get; set; }
        public string ArticleName { get; set; }
        public string ArticleNo { get; set; }
        public string ArticleChallanNo { get; set; }
        public string GradeName { get; set; }
        public string ColorName { get; set; }
        public string UnitName { get; set; }
        public string ItemTypeName { get; set; }
        public string LeatherTypeName { get; set; }
        public string LeatherStatusName { get; set; }

        #endregion Display Field

        #region Necessary Field


        public string FinishQCLabel { get; set; }
        public long FNTransferFromID { get; set; }
        public long FNTransferToID { get; set; }
        public Nullable<decimal> ToStockPcs { get; set; }
        public Nullable<decimal> ToStockSide { get; set; }
        public Nullable<decimal> ToStockArea { get; set; }
        public Nullable<decimal> QCStockPcs { get; set; }
        public Nullable<decimal> QCStockSide { get; set; }
        public Nullable<decimal> QCStockArea { get; set; }

        public Nullable<decimal> ClosingStockPcs { get; set; }
        public Nullable<decimal> ClosingStockSide { get; set; }
        public Nullable<decimal> ClosingStockArea { get; set; }
        public Nullable<byte> ClosingStockAreaUnit { get; set; }
        public Nullable<byte> AreaUnit { get; set; }

        #endregion




        public class InvFinishBuyerQCStock
        {
            #region INV_FinishBuyerQCStock

            public long TransectionID { get; set; }
            public Nullable<byte> StoreID { get; set; }
            public string FinishQCLabel { get; set; }
            public Nullable<int> BuyerID { get; set; }
            public Nullable<long> BuyerOrderID { get; set; }
            public Nullable<int> ArticleID { get; set; }
            public string ArticleNo { get; set; }
            public string ArticleChallanNo { get; set; }
            public Nullable<int> ColorID { get; set; }
            public Nullable<byte> ItemTypeID { get; set; }
            public Nullable<byte> LeatherTypeID { get; set; }
            public Nullable<byte> LeatherStatusID { get; set; }
            public Nullable<short> GradeID { get; set; }
            public Nullable<decimal> OpeningStockPcs { get; set; }
            public Nullable<decimal> OpeningStockSide { get; set; }
            public Nullable<decimal> OpeningStockArea { get; set; }
            public Nullable<decimal> ReceiveStockPcs { get; set; }
            public Nullable<decimal> ReceiveStockSide { get; set; }
            public Nullable<decimal> ReceiveStockArea { get; set; }
            public Nullable<decimal> IssueStockPcs { get; set; }
            public Nullable<decimal> IssueStockSide { get; set; }
            public Nullable<decimal> IssueStockArea { get; set; }
            public Nullable<decimal> ClosingStockPcs { get; set; }
            public Nullable<decimal> ClosingStockSide { get; set; }
            public Nullable<decimal> ClosingStockArea { get; set; }
            public Nullable<byte> ClosingStockAreaUnit { get; set; }

            #endregion
        }

    }
}
