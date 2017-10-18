using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class InvFNTransferTo
    {
        public long FNTransferToID { get; set; }
        public Nullable<long> FNTransferFromID { get; set; }
        public Nullable<long> FNTransferID { get; set; }
        public string FNTransferNo { get; set; }
        public Nullable<byte> StoreID { get; set; }
        public Nullable<byte> TransactionStore { get; set; }
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
        public Nullable<decimal> ToStockPcs { get; set; }
        public Nullable<decimal> ToStockSide { get; set; }
        public Nullable<decimal> ToStockArea { get; set; }
        public Nullable<byte> AreaUnit { get; set; }
        public Nullable<System.DateTime> SetOn { get; set; }
        public Nullable<int> SetBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public string IPAddress { get; set; }


        #region Display Field
        public string BuyerName { get; set; }
        public string StoreName { get; set; }
        public string BuyerOrderNo { get; set; }
        public string ArticleName { get; set; }
        public string GradeName { get; set; }
        public string ColorName { get; set; }
        public string UnitName { get; set; }
        public string ItemTypeName { get; set; }
        public string LeatherTypeName { get; set; }
        public string LeatherStatusName { get; set; }



        public Nullable<decimal> ClosingStockPcs { get; set; }
        public Nullable<decimal> ClosingStockSide { get; set; }
        public Nullable<decimal> ClosingStockArea { get; set; }
        public Nullable<byte> ClosingStockAreaUnit { get; set; }



        #endregion Display Field



    }
}
