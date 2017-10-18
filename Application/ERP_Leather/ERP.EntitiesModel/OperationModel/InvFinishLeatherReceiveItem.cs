using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class InvFinishLeatherReceiveItem
    {

        public long FNReceiveItemID { get; set; }
        public Nullable<long> FNReceiveID { get; set; }
        public Nullable<long> FinishLeatherIssueID { get; set; }
        public Nullable<long> FinishLeatherIssueItemID { get; set; }
        public Nullable<int> BuyerID { get; set; }
        public Nullable<long> BuyerOrderID { get; set; }
        public Nullable<int> ArticleID { get; set; }
        public string ArticleNo { get; set; }
        public long ArticleChallanID { get; set; }
        public string ArticleChallanNo { get; set; }
        public Nullable<byte> ItemTypeID { get; set; }
        public Nullable<byte> LeatherTypeID { get; set; }
        public Nullable<byte> LeatherStatusID { get; set; }
        public string FinishQCLabel { get; set; }
        public Nullable<System.DateTime> SetOn { get; set; }
        public Nullable<int> SetBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public string IPAddress { get; set; }


        #region Grid Display Field

        //public long FinishLeatherIssueID { get; set; }
        public string FinishLeatherIssueNo { get; set; }
        public string IssueFromName { get; set; }
        public string ReceiveAtName { get; set; }
        public string IssueCategory { get; set; }
        public string IssueCategoryName { get; set; }
        public string RecordStatus { get; set; }

        #endregion



        #region Display Field
        public string BuyerName { get; set; }
        public string StoreName { get; set; }
        public string BuyerOrderNo { get; set; }
        public string ArticleName { get; set; }
        //public string ArticleNo { get; set; }
        //public string ArticleChallanNo { get; set; }
        public string GradeName { get; set; }
        public string ColorName { get; set; }
        public string UnitName { get; set; }
        public string ItemTypeName { get; set; }
        public string LeatherTypeName { get; set; }
        public string LeatherStatusName { get; set; }

        #endregion Display Field


    }
}
