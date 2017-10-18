using System;
using System.Collections.Generic;
using ERP.EntitiesModel.BaseModel;

namespace ERP.EntitiesModel.OperationModel
{
    public class PrqChemPurcReq 
    {
        //public string RequisitionID { get; set; }
        //public string RequisitionNo { get; set; }

        //public string RequisitionType { get; set; }
        //public string ReqRaisedOn { get; set; }
        //public string ReqRaisedByID { get; set; }
        //public string ReqRaisedByName { get; set; }
        //public string RequisitionStatus { get; set; }



        //public string RequisitionCategory { get; set; }
        //public string RequisitionFrom { get; set; }
        //public string RequisitionTo { get; set; }
        
        // Add other properties if you need


        public int RequisitionID { get; set; }
        public string RequisitionNo { get; set; }
        public string RequisitionCategory { get; set; }
        public string RequisitionCategoryValue { get; set; }
        public string RequisitionType { get; set; }
        public string RequisitionTypeValue { get; set; }
        public byte? RequisitionFrom { get; set; } // RequisitionFrom from Store ID
        public string StoreName { get; set; } //  get name using RequisitionFrom field  ID
        public int? RequisitionTo { get; set; } // RequisitionTo from SupplierId
        public string SupplierName { get; set; } //  get name using RequisitionTo field  ID
        public string SupplierCode { get; set; }
        public byte? RequiredByTime { get; set; }
        public int? ReqRaisedBy { get; set; }
        public string ReqRaisedByName { get; set; }
        public string ReqRaisedOn { get; set; }
        
        public string RequisitionNote { get; set; }
        public int? RecipeFor { get; set; }
        public int? RecipeID { get; set; }
        public string ArticleNo { get; set; }
        public decimal? ProductionQty { get; set; }
        public string RequisitionStatus { get; set; }
        public string RequisitionState { get; set; }
        public string RecordStatus { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string ApprovalAdvice { get; set; }
        public string ApprovalAdviceNote { get; set; }
        public int? CheckedBy { get; set; }
        public DateTime? CheckDate { get; set; }
        public DateTime? PreparedOn { get; set; }
        public int? PreparedBy { get; set; }
        public DateTime? SetOn { get; set; }
        public int? SetBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public string IPAddress { get; set; }

        public  ICollection<PrqChemPurcReqItem> ChemPurcReqItems { get; set; }
    
    }
}
