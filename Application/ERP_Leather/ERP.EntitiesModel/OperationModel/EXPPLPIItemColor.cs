using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class EXPPLPIItemColor
    {
        public long PLPIItemColorID { get; set; }
        public long? PLPIID { get; set; }
        public string Commodity { get; set; }
        public int? ArticleID { get; set; }
        public string ArticleNo { get; set; }
        public string HSCode { get; set; }
        public byte? ItemTypeID { get; set; }
        public byte? LeatherTypeID { get; set; }
        public byte? LeatherStatusID { get; set; }
        public string MaterialNo { get; set; }
        public string AvgSize { get; set; }
        public byte? AvgSizeUnit { get; set; }
        public string SideDescription { get; set; }
        public string SelectionRange { get; set; }
        public string Thickness { get; set; }
        public byte? ThicknessUnit { get; set; }
        public string ThicknessAt { get; set; }
        public int? ColorID { get; set; }
        public decimal? MeterPLPIItemQty { get; set; }
        public decimal? FootPLPIItemQty { get; set; }
        public decimal? PLPIItemWeight { get; set; }
        public byte? ItemWeightUnit { get; set; }
        public decimal? PackQty { get; set; }
        public byte? PackUnit { get; set; }
        public string RecordStatus { get; set; }
        public string Remarks { get; set; }

        #region MyRegion
        public string ArticleName { get; set; }
        public string AvgSizeUnitName { get; set; }
        public string ThicknessUnitName { get; set; }
        public string ColorName { get; set; }

        #endregion
    }
}
