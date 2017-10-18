using System;
using ERP.EntitiesModel.BaseModel;

namespace ERP.EntitiesModel.OperationModel
{
    public class PRQChemFrgnPurcOrdr : CommonStatusInformation
    {
        public int OrderID { get; set; }
        public string OrderNo { get; set; }
        public string OrderDate { get; set; }
        public string OrderCategory { get; set; }
        public string OrderType { get; set; }
        public int OrderFrom { get; set; }
        public string OrderTo { get; set; }
        public int SupplierID { get; set; }
        public string SupplierName { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierAddress { get; set; }
        public string SupplierContactNumber { get; set; }

        public int? BuyerID { get; set; }
        public string BuyerName { get; set; }
        public string BuyerCode { get; set; }
        public string BuyerAddress { get; set; }
        public string BuyerContactNumber { get; set; }
        public int? LocalAgent { get; set; }
        public string LocalAgentName { get; set; }
        public string LocalAgentCode { get; set; }
        public string LocalAgentAddress { get; set; }
        public string LocalAgentContactNumber { get; set; }
        public int? ForeignAgent { get; set; }
        public string ForeignAgentName { get; set; }
        public string ForeignAgentCode { get; set; }
        public string ForeignAgentAddress { get; set; }
        public string ForeignAgentContactNumber { get; set; }
        public string CostIndicator { get; set; }
        public string OrderNote { get; set; }
        public string OrderStatus { get; set; }
        public string OrderState { get; set; }

        public string ConfirmComment { get; set; }
        
    }
}
