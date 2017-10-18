using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class EXPCIPIItem
    {
        public long CIPIItemID { get; set; }
        public long? CIPIID { get; set; }
        public string Commodity { get; set; }
        public int? ArticleID { get; set; }
        public string ArticleNo { get; set; }
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
        public decimal? MeterCIQty { get; set; }
        public decimal? MeterUnitPrice { get; set; }
        public decimal? MeterTotalPrice { get; set; }
        public decimal? FootCIQty { get; set; }
        public decimal? FootUnitPrice { get; set; }
        public decimal? FootTotalPrice { get; set; }
        public decimal? PackQty { get; set; }
        public byte? PackUnit { get; set; }
        public decimal? RecordStatus { get; set; }
        public string Remarks { get; set; }

        #region Display Filed
        public long? PIID { get; set; }
        public long? PIItemID { get; set; }
        public string ArticleName { get; set; }
        public string AvgSizeUnitName { get; set; }
        public string ThicknessUnitName { get; set; }
        public string ThicknessAtName { get; set; }
        public string PackUnitName { get; set; }
        #endregion
    }
}
