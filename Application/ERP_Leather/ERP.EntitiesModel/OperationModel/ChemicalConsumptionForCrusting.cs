using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class ChemicalConsumptionForCrusting
    {
        public long CLProductionID { get; set; }
        public long CLProductionDrumID { get; set; }
        public long CLProductionItemID { get; set; }

        public long CLProductionColorID { get; set; }
        public string CLProductionNo { get; set; }
        public byte? ProductionFloor { get; set; }

        public string ProductionFloorName { get; set; }
        public long YearMonID { get; set; }
        public string ScheduleYear { get; set; }
        public string ScheduleMonth { get; set; }
        public string ScheduleMonthName { get; set; }
        public long ParentID { get; set; }
        public long ScheduleID { get; set; }
        public string ScheduleNo { get; set; }
        public int? ProductionProcessID { get; set; }
        public string ProductionProcessName { get; set; }
        public long ScheduleDateID { get; set; }
        public string ProductionNo { get; set; }
        public string ProductionStatus { get; set; }
        public string ScheduleStartDate { get; set; }
        public string ScheduleEndDate { get; set; }

        public DateTime? TempScheduleStartDate { get; set; }
        public DateTime? TempScheduleEndDate { get; set; }


        public long ScheduleItemID { get; set; }
        public string ScheduleProductionNo { get; set; }
        public long? ProductionArticleChallanID { get; set; }
        public string ProductionArticleChallanNo { get; set; }

        public decimal? SchedulePcs { get; set; }
        public decimal ScheduleSide { get; set; }
        public decimal? ScheduleArea { get; set; }
        public byte? ScheduleAreaUnit { get; set; }
        public string ScheduleAreaUnitName { get; set; }
        public decimal? ScheduleWeight { get; set; }
        public byte? ScheduleWeightUnit { get; set; }
        public string ScheduleWeightUnitName { get; set; }

        public int? BuyerID { get; set; }
        public string BuyerName { get; set; }
        public long? BuyerOrderID { get; set; }
        public string BuyerOrderNo { get; set; }
        public byte? ItemTypeID { get; set; }
        public string ItemTypeName { get; set; }
        public byte? LeatherTypeID { get; set; }
        public byte? LeatherStatusID { get; set; }
        public string LeatherStatusName { get; set; }
        

        public int? ProductionArticleID { get; set; }
        public string ProductionArticleNo { get; set; }
        public string ProductionArticleName { get; set; }

        public long SdulItemColorID { get; set; }
        public int? ColorID { get; set; }
        public int? ArticleColorNo { get; set; }
        public string ColorName { get; set; }
        public decimal? ColorPCS { get; set; }
        public decimal? ColorSide { get; set; }
        public decimal? ColorArea { get; set; }
        public byte? AreaUnit { get; set; }
        public string AreaUnitName { get; set; }
        public decimal? ColorWeight { get; set; }
        public byte? WeightUnit { get; set; }
        public string WeightUnitName { get; set; }
        public string Remarks { get; set; }
        public int? ArticleID { get; set; }
        public string ArticleName { get; set; }
        public string ArticleNo { get; set; }
        public int RecipeID { get; set; }
        public string RecipeName { get; set; }
        public string RecipeNo { get; set; }
        public string CalculationBase { get; set; }
        public string RequirementOn { get; set; }
        public decimal BaseQuantity { get; set; }
        public byte BaseUnit { get; set; }
        public string BaseUnitName { get; set; }
        public string RecipeChallanNo { get; set; }
        public string ProductionProcess { get; set; }

        public string ArticleColorName { get; set; }

        public string RecordStatus { get; set; }

        public ChemicalConsumptionForCrusting FormInfo { get; set; }

        public IList<PrdYearMonthCrustScheduleDrum> LeatherList { get; set; }
        public List<PrdWBProductionChemical> ChemicalList { get; set; }
        public List<PRDChemProdReqItem> ChemicalListForShow { get; set; }

    }


    public class ChemicalStockInfo
    {
        public long TransectionID { get; set; }
        public byte StoreID { get; set; }
        public int? ItemID { get; set; }
        public int? SupplierID { get; set; }
        public decimal? ClosingQty { get; set; }

        public byte? StockUnit { get; set; }
    }
}
