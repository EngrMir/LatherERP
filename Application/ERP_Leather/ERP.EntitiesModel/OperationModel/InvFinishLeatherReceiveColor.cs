using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class InvFinishLeatherReceiveColor
    {
        public long FNReceiveColorID { get; set; }
        public Nullable<long> FNReceiveItemID { get; set; }
        public Nullable<long> FNReceiveID { get; set; }
        public Nullable<long> FinishLeatherIssueItemID { get; set; }
        public Nullable<long> FinishLeatherIssueID { get; set; }
        public Nullable<long> FinishLeatherIssueColorID { get; set; }
        public Nullable<int> ColorID { get; set; }
        public Nullable<short> GradeID { get; set; }
        public string FinishQCLabel { get; set; }
        public Nullable<decimal> IssuePcs { get; set; }
        public Nullable<decimal> IssueSide { get; set; }
        public Nullable<decimal> IssueArea { get; set; }
        public Nullable<decimal> SideArea { get; set; }
        public Nullable<byte> AreaUnit { get; set; }
        public Nullable<System.DateTime> SetOn { get; set; }
        public Nullable<int> SetBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public string IPAddress { get; set; }
        public Nullable<decimal> ReceivePcs { get; set; }
        public Nullable<decimal> ReceiveSide { get; set; }
        public Nullable<decimal> ReceiveArea { get; set; }
        public Nullable<decimal> ReceiveSideArea { get; set; }
        public Nullable<int> ArticleColorNo { get; set; }
        public string GradeRange { get; set; }



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
