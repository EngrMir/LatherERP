using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class InvFinishLeatherReceive
    {
        public long FNReceiveID { get; set; }
        public string FNReceiveNo { get; set; }
        public string FNReceiveDate { get; set; }
        public string ReceiveCategory { get; set; }
        public string ReceiveFor { get; set; }
        public Nullable<byte> IssueFrom { get; set; }
        public Nullable<byte> ReceiveAt { get; set; }
        public string RecordStatus { get; set; }
        public Nullable<int> ReceiveBy { get; set; }
        public Nullable<System.DateTime> ReceiveDate { get; set; }
        public string ReceiveNote { get; set; }
        public Nullable<int> CheckedBy { get; set; }
        public Nullable<System.DateTime> CheckDate { get; set; }
        public string CheckNote { get; set; }
        public Nullable<System.DateTime> SetOn { get; set; }
        public Nullable<int> SetBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public string IPAddress { get; set; }


        public virtual IList<InvFinishLeatherReceiveItem> InvFinishLeatherReceiveItemList { get; set; }
        public virtual IList<InvFinishLeatherReceiveColor> InvFinishLeatherReceiveColorList { get; set; }


        #region Necessary ID Field 
        public long FNReceiveColorID { get; set; }
        public long FNReceiveItemID { get; set; }
        public Nullable<long> FinishLeatherIssueItemID { get; set; }
        public Nullable<long> FinishLeatherIssueID { get; set; }
        public Nullable<long> FinishLeatherIssueColorID { get; set; }


        public Nullable<byte> StoreID { get; set; }
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

        #endregion END OF Necessary ID Field

        #region Display Field with Name

        public string FinishLeatherIssueNo { get; set; }
        public string IssueFromName { get; set; }
        public string ReceiveAtName { get; set; }
        public string IssueCategory { get; set; }
        public string ReceiveCategoryName { get; set; }



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
        public Nullable<decimal> IssuePcs { get; set; }
        public Nullable<decimal> IssueSide { get; set; }
        public Nullable<decimal> IssueArea { get; set; }
        public Nullable<decimal> SideArea { get; set; }
        public Nullable<byte> AreaUnit { get; set; }

        public Nullable<decimal> ReceivePcs { get; set; }
        public Nullable<decimal> ReceiveSide { get; set; }
        public Nullable<decimal> ReceiveArea { get; set; }
        public Nullable<decimal> ReceiveSideArea { get; set; }
        public string FinishQCLabel { get; set; }

        #endregion END OF Display Field with Name

    }
}
