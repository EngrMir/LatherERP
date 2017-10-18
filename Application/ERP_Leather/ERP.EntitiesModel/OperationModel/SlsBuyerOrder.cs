using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class SlsBuyerOrder
    {
        //public SlsBuyerOrder() 
        //{
        //    this.OrderItems = new List<SlsBuyerOrderItem>();
        //    this.OrderDelivery = new List<SlsBuyerOrderDelivery>();
        //    this.OrderPrices = new List<SlsBuyerOrderPrice>();
        //}
        public long BuyerOrderID { get; set; }
        public string BuyerOrderNo { get; set; }
        public string BuyerOrderDate { get; set; }
        public string BuyerOrderCategory { get; set; }
        public string BuyerOrderStatus { get; set; }
        public string AcknowledgementStatus { get; set; }
        public int? BuyerID { get; set; }
        public string BuyerName { get; set; }
        public int? BuyerAddressID { get; set; }
        public string BuyerAddress { get; set; } 
        public int? BuyerLocalAgentID { get; set; }
        public string LocalAgentName { get; set; }
        public int? BuyerForeignAgentID { get; set; }
        public string ForeignAgentName { get; set; }
        public string BuyerRef { get; set; }
        public string ExpectedShipmentDate { get; set; }
        public string ProposedShipmentDate { get; set; }
        public string DeliveryLastDate { get; set; }
        public string ExpectedProdStartDate { get; set; }
        public string RevisionNo { get; set; }
        public string RevisionDate { get; set; }
        public string RecordStatus { get; set; }
        public int? CheckedBy { get; set; }
        public string CheckDate { get; set; }
        public string CheckNote { get; set; }
        public int? ApprovedBy { get; set; }
        public string ApproveDate { get; set; }
        public string ApprovalNote { get; set; }
        public string SetOn { get; set; }
        public int? SetBy { get; set; }
        public string ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public string IPAddress { get; set; }
        public string OrderFrom { get; set; }
        public string OrderNo { get; set; }
        public string AcknowledgeNote { get; set; }
        public int? AcknowledgedBy { get; set; }
        public string AckDate { get; set; }
        public byte? OrderCurrency { get; set; }
        public decimal? TotalFootQty { get; set; }
        public decimal? TotalMeterQty { get; set; }
        public string PriceLevel { get; set; }
        public List<SlsBuyerOrderItem> OrderItems { get; set; }
        public List<SlsBuyerOrderDelivery> OrderDelivery { get; set; }
        public List<SlsBuyerOrderPrice> OrderPrices { get; set; } 
    }

    public class SlsBuyerOrderItem
    {
        public long BuyerOrderItemID { get; set; }
        public long? BuyerOrderID { get; set; }
        public int? ArticleID { get; set; }
        public string ArticleName { get; set; }
        public string ArticleNo { get; set; }
        public string ArticleChallanNo { get; set; }
        public string AvgSize { get; set; }
        public byte? AvgSizeUnit { get; set; }
        public string AvgSizeUnitName { get; set; }
        public string SideDescription { get; set; }
        public string SelectionRange { get; set; }
        public string Thickness { get; set; }
        public byte? ThicknessUnit { get; set; }
        public string ThicknessUnitName { get; set; }
        public string ThicknessAt { get; set; }
        public string commodity { get; set; }
        public string HSCode { get; set; }
        public byte? ItemTypeID { get; set; }
        public string ItemTypeName { get; set; }
        public byte? LeatherTypeID { get; set; }
        public string LeatherTypeName { get; set; }
        public byte? LeatherStatusID { get; set; }
        public string LeatherStatusName { get; set; }
        public decimal? ArticleFootQty { get; set; }
        public decimal? SeaFootUnitPrice { get; set; }
        public decimal? SeaFootTotalPrice { get; set; }
        public decimal? AirFootUnitPrice { get; set; }
        public decimal? AirFootTotalPrice { get; set; }
        public decimal? RoadFootUnitPrice { get; set; }
        public decimal? RoadFootTotalPrice { get; set; }
        public decimal? ArticleMeterQty { get; set; }
        public decimal? SeaMeterUnitPrice { get; set; }
        public decimal? SeaMeterTotalPrice { get; set; }
        public decimal? AirMeterUnitPrice { get; set; }
        public decimal? AirMeterTotalPrice { get; set; }
        public decimal? RoadMeterUnitPrice { get; set; }
        public decimal? RoadMeterTotalPrice { get; set; }
        public List<SlsBuyerOrderItemColor> ItemColors { get; set; } 
    }

    public class SlsBuyerOrderItemColor
    {
        public long BuyerOrdItemColorID { get; set; }
        public long? BuyerOrderID { get; set; }
        public long? BuyerOrderItemID { get; set; }
        public int? ColorID { get; set; }
        public string ColorName { get; set; }
        public int? ColorQty { get; set; }
        public byte? ColorUnit { get; set; }
        public string ColorUnitName { get; set; }
        public decimal? ColorFootQty { get; set; }
        public decimal? SeaFootUnitPrice { get; set; }
        public decimal? SeaFootTotalPrice { get; set; }
        public decimal? AirFootUnitPrice { get; set; }
        public decimal? AirFootTotalPrice { get; set; }
        public decimal? RoadFootUnitPrice { get; set; }
        public decimal? RoadFootTotalPrice { get; set; }
        public decimal? ColorMeterQty { get; set; }
        public decimal? SeaMeterUnitPrice { get; set; }
        public decimal? SeaMeterTotalPrice { get; set; }
        public decimal? AirMeterUnitPrice { get; set; }
        public decimal? AirMeterTotalPrice { get; set; }
        public decimal? RoadMeterUnitPrice { get; set; }
        public decimal? RoadMeterTotalPrice { get; set; }
    }

    public class SlsBuyerOrderDelivery
    {
        public long BuyerOrderDeliveryID { get; set; }
        public long? BuyerOrderID { get; set; }
        public int? OrdDeliverySL { get; set; }
        public string OrdDeliveryDate { get; set; }
        public decimal? OrdDateFootQty { get; set; }
        public decimal? OrdDateMeterQty { get; set; }
        public int? PIDeliverySL { get; set; }
        public string PIDeliveryDate { get; set; }
        public decimal? PIDateFootQty { get; set; }
        public decimal? PIDateMeterQty { get; set; }
        public int? ArticleID { get; set; }
        public string ArticleName { get; set; }
        public decimal? ArticleFootQty { get; set; }
        public decimal? ArticleMeterQty { get; set; }
        public int? ColorID { get; set; }
        public string ColorName { get; set; }
        public decimal? ColorFootQty { get; set; }
        public decimal? ColorMeterQty { get; set; }
        public string Remarks { get; set; }
    }

    public class SlsBuyerOrderPrice
    {
        public long BuyerOrderPriceID { get; set; }
        public long? BuyerOrderID { get; set; }
        public string OrdDeliveryMode { get; set; }
        public string OrdDeliveryModeName { get; set; }
        public string OrdRefDeliverySL { get; set; }
        public decimal? OrdAverageUnitPrice { get; set; }
        public string OrdDeliveryModeNote { get; set; }
        public string PIDeliveryMode { get; set; }
        public string PIRefDeliverySL { get; set; }
        public decimal? PIAverageUnitPrice { get; set; }
        public string PIDeliveryModeNote { get; set; }
        public int? ArticleID { get; set; }
        public string ArticleName { get; set; }
        public decimal? ArticleFootQty { get; set; }
        public decimal? ArticleFootUnitPrice { get; set; }
        public decimal? ArticleFootTotalPrice { get; set; }
        public decimal? ArticleMeterQty { get; set; }
        public decimal? ArticleMeterUnitPrice { get; set; }
        public decimal? ArticleMeterTotalPrice { get; set; }
        public int? ColorID { get; set; }
        public string ColorName { get; set; }
        public decimal? ColorFootQty { get; set; }
        public decimal? ColorFootUnitPrice { get; set; }
        public decimal? ColorFootTotalPrice { get; set; }
        public decimal? ColorMeterQty { get; set; }
        public decimal? ColorMeterUnitPrice { get; set; }
        public decimal? ColorMeterTotalPrice { get; set; }
        public string Remarks { get; set; }
    }
}
