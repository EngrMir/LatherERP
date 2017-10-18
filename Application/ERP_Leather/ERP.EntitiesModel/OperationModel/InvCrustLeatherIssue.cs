using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class InvCrustLeatherIssue
    {
        public long CrustLeatherIssueID { get; set; }
        public string CrustLeatherIssueNo { get; set; }
        public string CrustLeatherIssueDate { get; set; }
        public string IssueCategory { get; set; }
        public string IssueCategoryName { get; set; }
        public string IssueFor { get; set; }
        public string IssueForName { get; set; }
        public byte? IssueFrom { get; set; }
        public string IssueFromName { get; set; }
        public byte? IssueTo { get; set; }
        public string IssueToName { get; set; }
        public string IssueQCStatus { get; set; }
        public string RecordStatus { get; set; }
        public string IssueNote { get; set; }
        public string CheckNote { get; set; }
        public List<InvCrustLeatherIssueItem> Items { get; set; }

        public List<InvCrustLeatherIssueItem> IssueItemList { get; set; }
        public List<InvCrustLeatherIssueColor> IssueColorList { get; set; }
    }

    public class InvCrustLeatherIssueItem
    {
        public long CrustLeatherIssueItemID { get; set; }
        public long? CrustLeatherIssueID { get; set; }
        public int? BuyerID { get; set; }
        public string BuyerName { get; set; }
        public long? BuyerOrderID { get; set; }
        public string BuyerOrderNo { get; set; }
        public long? ArticleChallanID { get; set; }
        public string ArticleChallanNo { get; set; }
        public int? ArticleID { get; set; }
        public string ArticleNo { get; set; }
        public string ArticleName { get; set; }
        public byte? ItemTypeID { get; set; }
        public string ItemTypeName { get; set; }
        public byte? LeatherTypeID { get; set; }
        public string LeatherTypeName { get; set; }
        public byte? LeatherStatusID { get; set; }
        public string LeatherStatusName { get; set; }
        public long? RequisitionDateID { get; set; }
        public string RequisitionNo { get; set; }
        public string CrustQCLabel { get; set; }
        public long? RequisitionItemID { get; set; }
        public byte? IssueFrom { get; set; }
        public List<InvCrustLeatherIssueColor> Colors { get; set; }
    }

    public class InvCrustLeatherIssueColor
    {
        public int SlNo { get; set; }
        public int SlNo2 { get; set; }
        public long CrustLeatherIssueColorID { get; set; }
        public long? CrustLeatherIssueItemID { get; set; }
        public long? CrustLeatherIssueID { get; set; }
        public int? ColorID { get; set; }
        public string ColorName { get; set; }
        public short? GradeID { get; set; }
        public string GradeName { get; set; }
        public decimal? IssuePcs { get; set; }
        public decimal? IssueSide { get; set; }
        public decimal? IssueArea { get; set; }
        public decimal? ClosingStockPcs { get; set; }
        public decimal? ClosingStockSide { get; set; }
        public decimal? ClosingStockArea { get; set; }
        public byte? AreaUnit { get; set; }
        public string AreaUnitName { get; set; }
        public string CrustQCLabel { get; set; }
        public string FinishQCLabel { get; set; }
        public decimal? SideArea { get; set; }
        public string GradeRange { get; set; }
        public int? ArticleColorNo { get; set; }
    }
}
