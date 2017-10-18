using System;
using System.Collections.Generic;
using ERP.EntitiesModel.BaseModel;

namespace ERP.EntitiesModel.OperationModel
{
    public class PrdWBProduction : CommonStatusInformation
    {
        public Int64 WBProductionID { get; set; }
        public string WBProductionNo { get; set; }
        public string ProductionFor { get; set; }
        public string ProductionNo { get; set; }
        public byte? ProductionFloor { get; set; }
        public string ProductionFloorName { get; set; }
        public string WBProductionInitiator { get; set; }
        public Int64 ScheduleDateID { get; set; }
        public string WBProductionStartDate { get; set; }
        public string WBProductionEndDate { get; set; }
        public int WBProductionPcs { get; set; }
        public int WBProductionSide { get; set; }
        public decimal WBProductionWeight { get; set; }
        public byte WBWeightUnit { get; set; }
        public int? RecipeID { get; set; }
        public string RecipeName { get; set; }
        public string RecipeNo { get; set; }
        public int? ArticleID { get; set; }
        public string ArticleName { get; set; }
        public string ArticleColorName { get; set; }
        public string ArticleNo { get; set; }
        public string RecipeChallanNo { get; set; }
        public int? ProductionProcessID { get; set; }
        public string ProductionProcessName { get; set; }
        public int ConfirmedBy { get; set; }
        public string ConfirmeDate { get; set; }
        public string ConfirmeNote { get; set; }
        public int CheckedBy { get; set; }
        public string CheckDate { get; set; }
        public int PreparedBy { get; set; }
        public string PreparedOn { get; set; }


        public string ScheduleYear { get; set; }
        public string ScheduleMonth { get; set; }
        public string ScheduleNo { get; set; }
        public string PrepareDate { get; set; }
        public decimal? SchedulePcs { get; set; }
        public decimal? ScheduleSide { get; set; }
        public decimal? ScheduleWeight { get; set; }
        public byte? ScheduleWeightUnit { get; set; }
        public string ScheduleWeightUnitName { get; set; }
        public string ProductionStatus { get; set; }
        public string Remark { get; set; }
        public string ScheduleStartDate { get; set; }
        public string ScheduleEndDate { get; set; }
        public string ScheduleFor { get; set; }
        public decimal BaseQuantity { get; set; }
        public byte? BaseUnit { get; set; }
        public string BaseUnitName { get; set; }
        public string RequirementOn { get; set; }

        public long? ParentID { get; set; }


        public PrdWBProduction FormInfo { get; set; }
        public List<PrdWBProductionPurchase> LeatherList { get; set; }
        public List<PrdWBProductionChemical> ChemicalList { get; set; }
        public List<PRDChemProdReqItem> ChemicalListForShow { get; set; }
    }
    public class PrdWBProductionPurchase : CommonStatusInformation
    {
        public Int64 WBProductionPurchaseID { get; set; }
        public Int64 WBProductionID { get; set; }
        public Int64? SchedulePurchaseID { get; set; }
        public Int64 ScheduleDateID { get; set; }
        public string ProductionNo { get; set; }
        public int? MachineID { get; set; }
        public string MachineNo { get; set; }
        public int? SupplierID { get; set; }
        public string SupplierName { get; set; }
        public Int64? PurchaseID { get; set; }
        public string PurchaseNo { get; set; }
        public byte? ItemTypeID { get; set; }
        public string ItemTypeName { get; set; }
        public byte? LeatherTypeID { get; set; }
        public string LeatherTypeName { get; set; }
        public byte? LeatherStatusID { get; set; }
        public string LeatherStatusName { get; set; }
        public int? WBProductionPcs { get; set; }
        public int? WBProductionSide { get; set; }
        public decimal? WBProductionWeight { get; set; }
        public byte? WeightUnit { get; set; }
        public string UnitName { get; set; }
        public int RecipeID { get; set; }
        public int ArticleID { get; set; }
        public string ArticleNo { get; set; }
        public string RecipeChallanNo { get; set; }
        public int ProductionProcessID { get; set; }
        public string Remarks { get; set; }

        public decimal? ProductionPcs { get; set; }
        public decimal? ProductionSide { get; set; }
        public decimal? ProductionWeight { get; set; }
    }
    public class PrdWBProductionChemical : CommonStatusInformation
    {
        public Int64 WBProdChemicalID { get; set; }
        public Int64 WBProductionPurchaseID { get; set; }
        public Int64 WBProductionID { get; set; }
        public Int64 SchedulePurchaseID { get; set; }
        public int RequisitionID { get; set; }
        public int RecipeID { get; set; }
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public decimal? RequiredQty { get; set; }
        public byte? RequiredUnitID { get; set; }
        public string RequiredUnitName { get; set; }
        public byte? UsePackSize { get; set; }
        public string UsePackSizeName { get; set; }
        public byte? UseSizeUnit { get; set; }
        public string UseSizeUnitName { get; set; }
        public int? UsePackQty { get; set; }
        public decimal? UseQty { get; set; }
        public byte? UseUnitID { get; set; }
        public string UseUnitName { get; set; }
        public int? SupplierID { get; set; }
        public string SupplierName { get; set; }
        public int ManufacturerID { get; set; }
        public string ItemSource { get; set; }
        public string CalculationBase { get; set; }


        public long CLProdChemicalID { get; set; }
        public long CLProductionDrumID { get; set; }
    }
    public class PrdWetBlueProductionStock : CommonStatusInformation
    {
        public Int64 TransectionID { get; set; }
        public byte StoreID { get; set; }
        public int SupplierID { get; set; }
        public Int64 PurchaseID { get; set; }
        public Int64 ChallanID { get; set; }
        public int ChallanNo { get; set; }
        public byte ItemTypeID { get; set; }
        public byte LeatherTypeID { get; set; }
        public byte LeatherStatusID { get; set; }
        public string SelectionStatus { get; set; }
        public int OpeningPcs { get; set; }
        public int OpeningSide { get; set; }
        public decimal OpeningArea { get; set; }
        public int ReceivePcs { get; set; }
        public int ReceiveSide { get; set; }
        public decimal ReceiveArea { get; set; }
        public int IssuePcs { get; set; }
        public int IssueSide { get; set; }
        public decimal IssueArea { get; set; }
        public int ClosingProductionkPcs { get; set; }
        public int ClosingProductionSide { get; set; }
        public decimal ClosingProductionArea { get; set; }
        public byte AreaUnit { get; set; }
    }
   
}
