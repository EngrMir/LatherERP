using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class InvFinishLeatherIssue
    {
        public long FinishLeatherIssueID { get; set; }
        public string FinishLeatherIssueNo { get; set; }
        public string FinishLeatherIssueDate { get; set; }
        public string IssueCategory { get; set; }
        public string IssueFor { get; set; }
        public byte? IssueFrom { get; set; }
        public byte? IssueTo { get; set; }
        public long? FinishLeatherQCID { get; set; }
        public string RecordStatus { get; set; }
        public string IssueNote { get; set; }
        public string CheckNote { get; set; }
        public List<InvFinishedLeatherIssueItem> Items { get; set; } 
    }

    public class InvFinishedLeatherIssueItem
    {
        public long FinishLeatherIssueItemID { get; set; }
        public long? FinishLeatherIssueID { get; set; }
        public long? RequisitionDateID { get; set; }
        public string RequisitionNo { get; set; }
        public int? BuyerID { get; set; }
        public string BuyerName { get; set; }
        public long? BuyerOrderID { get; set; }
        public string BuyerOrderNo { get; set; }
        public int? ArticleID { get; set; }
        public string ArticleNo { get; set; }
        public string ArticleChallanNo { get; set; }
        public string ArticleName { get; set; }
        public byte? ItemTypeID { get; set; }
        public string ItemTypeName { get; set; }
        public byte? LeatherTypeID { get; set; }
        public string LeatherTypeName { get; set; }
        public byte? LeatherStatusID { get; set; }
        public string LeatherStatusName { get; set; }
        public List<InvFinishedLeatherIssueColor> Colors { get; set; } 
    }

    public class InvFinishedLeatherIssueColor
    {
        public long FinishLeatherIssueColorID { get; set; }
        public long? FinishLeatherIssueItemID { get; set; }
        public long? FinishLeatherIssueID { get; set; }
        public int? ColorID { get; set; }
        public string ColorName { get; set; }
        public short? GradeID { get; set; }
        public string GradeName { get; set; }
        public string FinishQCLabel { get; set; }
        public decimal? ClosingStockPcs { get; set; }
        public decimal? ClosingStockSide { get; set; }
        public decimal? ClosingStockArea { get; set; }
        public decimal? IssuePcs { get; set; }
        public decimal? IssueSide { get; set; }
        public decimal? IssueArea { get; set; }
        public decimal? SideArea { get; set; }
        public byte? AreaUnit { get; set; }
        public string UnitName { get; set; }
    }
}
