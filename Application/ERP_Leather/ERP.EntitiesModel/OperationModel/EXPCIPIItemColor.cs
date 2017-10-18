using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class EXPCIPIItemColor
    {
        public long CIPIItemColorID { get; set; }
        public long? CIPIItemID { get; set; }
        public string MaterialNo { get; set; }
        public int? ColorID { get; set; }
        public string AvgSize { get; set; }
        public byte? AvgSizeUnit { get; set; }
        public string SideDescription { get; set; }
        public string SelectionRange { get; set; }
        public string Thickness { get; set; }
        public byte? ThicknessUnit { get; set; }
        public string ThicknessAt { get; set; }
        public decimal? MeterColorQty { get; set; }
        public decimal? MeterUnitPrice { get; set; }
        public decimal? MeterTotalPrice { get; set; }
        public decimal? FootColorQty { get; set; }
        public decimal? FootUnitPrice { get; set; }
        public decimal? FootTotalPrice { get; set; }
        public decimal? PackQty { get; set; }
        public byte? PackUnit { get; set; }
        public string Remarks { get; set; }

        #region Display Field

        public int? ArticleID { get; set; }
        public string ColorName { get; set; }
        public string AvgSizeUnitName { get; set; }
        public string ThicknessUnitName { get; set; }
        public string PackUnitName { get; set; } 
        #endregion
    }
}
