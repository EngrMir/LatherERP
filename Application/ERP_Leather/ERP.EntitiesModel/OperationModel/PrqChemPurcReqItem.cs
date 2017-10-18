using System;
using ERP.EntitiesModel.BaseModel;

namespace ERP.EntitiesModel.OperationModel
{
    public class PrqChemPurcReqItem 
    {
        public long RequisitionItemID { get; set; }
        public int? RequisitionID { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public decimal? SafetyStock { get; set; }
        public decimal? PipelineQty { get; set; }
        public decimal? TotalQty { get; set; }
        public int? ReorderLevel { get; set; }
        public decimal? ProdReqQty { get; set; }
        public int? SupplierID { get; set; }
        public int? ManufacturerID { get; set; }
        public string ManufacturerName { get; set; }
        public decimal? RequiredQty { get; set; }
        public byte? RequiredUnit { get; set; }
        public decimal RequisitionQty { get; set; }
        public byte RequisitionUnit { get; set; }
        public byte? SizeID { get; set; }
        public string SizeName { get; set; }
        public byte? PackUnitID { get; set; }
        public string PackUnitName { get; set; }
        public int? PackQty { get; set; }
        public byte UnitID { get; set; }
        public string UnitName { get; set; }
        public decimal? ApproveQty { get; set; }
        public byte? ApproveUnit { get; set; }
        public string ItemSource { get; set; }
        public string ItemStatus { get; set; }
        public string ApprovalState { get; set; }
        //public string ApprovalStateID { get; set; }
        public DateTime? SetOn { get; set; }
        public int? SetBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public string IPAddress { get; set; }
    
    }
}
