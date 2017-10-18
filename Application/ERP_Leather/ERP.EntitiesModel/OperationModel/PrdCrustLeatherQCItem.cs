using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class PrdCrustLeatherQCItem
    {
        public long CLQCItemID { get; set; }
        public long CrustLeatherQCRefID { get; set; }
        public long CrustLeatherQCID { get; set; }
        public long? ScheduleItemID { get; set; }
        public string ScheduleProductionNo { get; set; }
        public int? BuyerID { get; set; }
        public string BuyerName { get; set; }
        public long? BuyerOrderID { get; set; }
        public string BuyerOrderNo { get; set; }
        public int? ArticleID { get; set; }
        public string ArticleName { get; set; }
        public string ArticleNo { get; set; }
        public string ChallaNo { get; set; }
        public int? ColorID { get; set; }
        public string ColorName { get; set; }
        public byte? ItemTypeID { get; set; }
        public string ItemTypeName { get; set; }
        public byte? LeatherTypeID { get; set; }
        public byte? LeatherStatusID { get; set; }
        public string LeatherStatusName { get; set; }
        public decimal? ProductionPcs { get; set; }
        public decimal? ProductionSide { get; set; }
        public decimal? ProductionArea { get; set; }
        public byte? ProductionAreaUnit { get; set; }
        public string ProductionAreaUnitName { get; set; }
        public decimal? ProductionWeight { get; set; }
        public byte? ProductionWeightUnit { get; set; }
        public string ProductionWeightUnitName { get; set; }
        public int CLQCPcs { get; set; }
        public int CLQCSide { get; set; }
        public decimal? CLQCArea { get; set; }
        public byte? CLQCAreaUnit { get; set; }
        public string Remarks { get; set; }

        #region MyRegion
        public long? SdulItemColorID { get; set; }

        public long? TransectionID { get; set; }

        public string ArticleChallanNo { get; set; }

        public long? ArticleChallanID { get; set; }

        public int? ArticleColorNo { get; set; }

        #endregion
    }
}
