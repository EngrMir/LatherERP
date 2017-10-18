using System;
using ERP.EntitiesModel.BaseModel;

namespace ERP.EntitiesModel.OperationModel
{
    public class PRQChemFrgnPurcOrdrItem : CommonStatusInformation
    {
        public string OrderItemID { get; set; }
        public string OrederID { get; set; }
        public string RequisitionID { get; set; }
        public string ItemID { get; set; }
        public string ItemName { get; set; }

        public string RequisitionPackSizeID { get; set; }
        public string RequisitionPackSizeName { get; set; }

        public string RequisitionPackSizeUnitID { get; set; }
        public string RequisitionPackSizeUnitName { get; set; }

        public string RequisitionPackQty { get; set; }
        public string RequsitionQty { get; set; }
        public string RequisitionUnitID { get; set; }
        public string RequisitionUnitName { get; set; }

        public string ApproveQty { get; set; }
        public string ApproveUnitID { get; set; }
        public string ApproveUnitName { get; set; }
       










        public string OrderPackSizeID { get; set; }
        public string OrderPackSizeName { get; set; }
        public string OrderPackSizeUnitID { get; set; }
        public string OrderPackSizeUnitName { get; set; }
        public string OrderPackQty { get; set; }
        public string OrderQty { get; set; }
        public string OrderUnitID { get; set; }
        public string OrderUnitName { get; set; }

        public string ManufacturerID { get; set; }
        public string ManufacturerName { get; set; }
       
        public int? SupplierID { get; set; }
        
        public string ItemSource { get; set; }
        public string ApprovalState { get; set; }
    }
}
