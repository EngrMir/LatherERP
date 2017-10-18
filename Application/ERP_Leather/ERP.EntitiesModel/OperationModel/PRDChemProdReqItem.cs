using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;

namespace ERP.EntitiesModel.OperationModel
{
    public class PRDChemProdReqItem : CommonStatusInformation
    {
        public long RequisitionItemID { get; set; }
        public int RequisitionID { get; set; }
        public int? ItemID { get; set; }
        public string ItemName { get; set; }
        public int? SupplierID { get; set; }
        public string SupplierName { get; set; }
        public int ManufacturerID { get; set; }
        public decimal? RequiredQty { get; set; }
        public decimal? StockQty { get; set; }
        public string ProductionStock { get; set; }

        public byte? StockUnit { get; set; }
        public string StockUnitName { get; set; }
        public byte? RequiredUnit { get; set; }
        public string RequiredUnitName { get; set; }
        public decimal? RequsitionQty { get; set; }
        public byte? RequisitionUnit { get; set; }
        public string RequisitionUnitName { get; set; }
        public byte? PackSize { get; set; }
        public string PackSizeName { get; set; }
        public byte? SizeUnit { get; set; }
        public string SizeUnitName { get; set; }
        public Int32? PackQty { get; set; }
        public decimal ApproveQty { get; set; }
        public byte ApproveUnit { get; set; }
        public string ApproveUnitName { get; set; }
        public string ItemSource { get; set; }
        public string ItemStatus { get; set; }
        public string ApprovalState { get; set; }




        // For Issue To Production
        public long TransItemID { get; set; }
        public byte? IssuePackSize { get; set; }
        public string IssuePackSizeName { get; set; }
        public byte? IssueSizeUnit { get; set; }
        public string IssueSizeUnitName { get; set; }
        public int? IssuePackQty { get; set; }
        public decimal? IssueQty { get; set; }
        public byte? IssueUnit { get; set; }
        public string IssueUnitName { get; set; }



        public decimal? ReceiveQty { get; set; }
        public byte? ReceiveUnit { get; set; }
        public string ReceiveUnitName { get; set; }


        //For Chemical Consumption
        public Int64 WBProdChemicalID { get; set; }


        public long CLProdChemicalID { get; set; }
        public long? CLProductionDrumID { get; set; }
    }
}
