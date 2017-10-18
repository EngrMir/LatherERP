using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class PrdYearMonthCrustScheduleItem
    {
        public long ScheduleItemID { get; set; }
        public string ScheduleProductionNo { get; set; }
        public long? ScheduleDateID { get; set; }
        public long? RequisitionDateID { get; set; }
        public string RequisitionNo { get; set; }
        public long? ScheduleID { get; set; }
        public long? YearMonID { get; set; }
        public int? BuyerID { get; set; }
        public long? BuyerOrderID { get; set; }
        public int? ArticleID { get; set; }
        public string ArticleNo { get; set; }
        public long? ArticleChallanID { get; set; }
        public string ArticleChallanNo { get; set; }
        public string AvgSize { get; set; }
        public byte? AvgSizeUnit { get; set; }
        public string SelectionRange { get; set; }
        public string SideDescription { get; set; }
        public string Thickness { get; set; }
        public byte? ThicknessUnit { get; set; }
        public string ThicknessAt { get; set; }
        public byte? ItemTypeID { get; set; }
        public byte? LeatherTypeID { get; set; }
        public byte? LeatherStatusID { get; set; }
        public decimal? SchedulePcs { get; set; }
        public decimal? ScheduleSide { get; set; }
        public decimal? ScheduleArea { get; set; }
        public byte? ScheduleAreaUnit { get; set; }
        public string ScheduleAreaUnitName { get; set; }
        public decimal? ScheduleWeight { get; set; }
        public byte? ScheduleWeightUnit { get; set; }
        public string ScheduleWeightUnitName { get; set; }
        public string Remarks { get; set; }


        public int? ColorID { get; set; }
        public string ColorName { get; set; }

        public decimal? ColorPCS { get; set; }
        public decimal? ColorSide { get; set; }
        public decimal? ColorArea { get; set; }

        #region Badhon
        public int? ProductionArticleID { get; set; }
        public string ProductionArticleName { get; set; }
        public string ProductionArticleNo { get; set; }

        public long? ProductionArticleChallanID { get; set; }
        public string ProductionArticleChallanNo { get; set; }


        public string ArticleName { get; set; }
        public string LeatherStatusName { get; set; }
        public string ItemTypeName { get; set; }
        public string ThicknessUnitName { get; set; }
        public string AvgSizeUnitName { get; set; }
        public long? RequisitionItemID { get; set; }
        public string BuyerName { get; set; }
        public string BuyerOrderNo { get; set; }

        #endregion

        public virtual IList<PrdYearMonthCrustScheduleColor> PrdYearMonthCrustScheduleColorList { get; set; }
        public virtual IList<PrdYearMonthCrustScheduleDrum> PrdYearMonthCrustScheduleDrumList { get; set; }
    }
}
