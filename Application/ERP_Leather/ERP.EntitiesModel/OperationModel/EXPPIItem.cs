using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class EXPPIItem
    {
        public long PIItemID { get; set; }
        public long PIID { get; set; }
        //public string Commodity { get; set; }
        public string commodity { get; set; }
        public string HSCode { get; set; }
        public int ArticleID { get; set; }
        public string ArticleNo { get; set; }
        public string ArticleName { get; set; }
        public byte ItemTypeID { get; set; }
        public byte LeatherTypeID { get; set; }
        public byte LeatherStatusID { get; set; }
        public string AvgSize { get; set; }
        public byte AvgSizeUnit { get; set; }
        public string AvgSizeUnitName { get; set; }
        public string SideDescription { get; set; }
        public string SelectionRange { get; set; }
        public string Thickness { get; set; }
        public byte ThicknessUnit { get; set; }
        public string ThicknessUnitName { get; set; }
        public string ThicknessAt { get; set; }
        public string ItemDeliveryDate { get; set; }
        public string ApprovalState { get; set; }

        public Nullable<decimal> ArticleFootQty { get; set; }
        public Nullable<decimal> SeaFootUnitPrice { get; set; }
        public Nullable<decimal> SeaFootTotalPrice { get; set; }
        public Nullable<decimal> AirFootUnitPrice { get; set; }
        public Nullable<decimal> AirFootTotalPrice { get; set; }
        public Nullable<decimal> RoadFootUnitPrice { get; set; }
        public Nullable<decimal> RoadFootTotalPrice { get; set; }
        public Nullable<decimal> ArticleMeterQty { get; set; }
        public Nullable<decimal> SeaMeterUnitPrice { get; set; }
        public Nullable<decimal> SeaMeterTotalPrice { get; set; }
        public Nullable<decimal> AirMeterUnitPrice { get; set; }
        public Nullable<decimal> AirMeterTotalPrice { get; set; }
        public Nullable<decimal> RoadMeterUnitPrice { get; set; }
        public Nullable<decimal> RoadMeterTotalPrice { get; set; }
    }
}
