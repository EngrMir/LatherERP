using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;

namespace ERP.EntitiesModel.OperationModel
{
    public class PRDChemProdReq: CommonStatusInformation
    {
        public int RequisitionID { get; set; }
        public string RequisitionNo { get; set; }
        public string RequisitionCategory { get; set; }
        public string RequisitionType { get; set; }
        public byte RequisitionFrom { get; set; }
        public string RequisitionFromName { get; set; }
        public byte RequisitionTo { get; set; }
        public string RequisitionToName { get; set; }


        public byte IssueFrom { get; set; }
        public byte IssueTo { get; set; }


        public byte? RequiredByTime { get; set; }
        public int? ReqRaisedBy { get; set; }
        public string ReqRaisedByName { get; set; }
        public string ReqRaisedOn { get; set; }

        public DateTime? ReqRaisedOnTemp { get; set; }
        public string RequisitionNote { get; set; }
        public int? RecipeFor { get; set; }
        public int? RecipeID { get; set; }
        public string ArticleNo { get; set; }

        public string ArticleName { get; set; }
        public int? ArticleColor { get; set; }
        public string ArticleColorName { get; set; }
        public string ArticleChallanNo { get; set; }

        public string BaseQuantity { get; set; }
        public byte BaseUnit { get; set; }
        public string BaseUnitName { get; set; }
        public string ProductionQuantityUnit { get; set; }

        public string LeatherSize { get; set; }
        public byte? SizeUnit { get; set; }
        public string Selection { get; set; }
        public string Thickness { get; set; }
        public byte? ThicknessUnit { get; set; }
        public int BuyerID { get; set; }
        public string BuyerName { get; set; }

        public long BuyerOrderID { get; set; }
        public int JobOrderID { get; set; }
        public string JobOrderNo { get; set; }

        public decimal? ProductionQty { get; set; }
        public string RequisitionStatus { get; set; }
        public string RequisitionState { get; set; }
        public int ApprovedBy { get; set; }
        public string ApproveDate { get; set; }
        public string ApprovalAdvice { get; set; }
        public string ApprovalAdviceNote { get; set; }
        public int CheckedBy { get; set; }
        public string CheckDate { get; set; }
        public int PreparedBy { get; set; }
        public string PreparedOn { get; set; }


        public PRDChemProdReq RequisitionInfo { get; set; }
        public virtual ICollection<PRDChemProdReqItem> RequisitionItemList { get; set; }
        
    }
}
