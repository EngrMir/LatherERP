using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
        public class SlsBuyerOrderBadhon
        {
            public long OrderId { get; set; }
            public long? BuyerOrderID { get; set; }
            public string BuyerOrderNo { get; set; }
            public string BuyerOrderDate { get; set; }
            public string OrderFrom { get; set; }
            public string OrderNo { get; set; }
            public string BuyerOrderCategory { get; set; }
            public string BuyerOrderStatus { get; set; }
            public string AcknowledgementStatus { get; set; }
            public int? BuyerId { get; set; }
            public string BuyerName { get; set; }
            public int? BuyerAddressId { get; set; }
            public string BuyerAddress { get; set; }
            public int? BuyerLocalAgentId { get; set; }
            public string LocalAgentName { get; set; }
            public int? BuyerForeignAgentId { get; set; }
            public string ForeignAgentName { get; set; }
            public string BuyerRef { get; set; }
            public string ExpectedShipmentDate { get; set; }
            public string ProposedShipmentDate { get; set; }
            public string DeliveryLastDate { get; set; }
            public string ExpectedProdStartDate { get; set; }
            public string RevisionNo { get; set; }
            public string RevisionDate { get; set; }
            public string RecordStatus { get; set; }
            public int CheckedBy { get; set; }
            public string CheckDate { get; set; }
            public string CheckNote { get; set; }
            public int? ApprovedBy { get; set; }
            public string ApproveDate { get; set; }
            public string ApprovalNote { get; set; }
            public string AcknowledgeNote { get; set; }
            public int? AcknowledgedBy { get; set; }
            public string AckDate { get; set; }
            public string PriceLevel { get; set; }
            public List<SlsBuyerOrderItem> OrderItems { get; set; }
            public List<SlsBuyerOrderDeliveryBadhon> DeliveryDates { get; set; }
            
        }

        public class SlsBuyerOrderItemBadhon
        {
            public int SlNo { get; set; }
            public long OrderItemId { get; set; }
            public long? BuyerOrderItemID { get; set; }
            public long? OrderId { get; set; }
            public long? BuyerOrderID { get; set; }
            public string BuyerOrderDate { get; set; }
            public string BuyerOrderNo { get; set; }
            public string commodity { get; set; }
            public string HSCode { get; set; }
            public int? ArticleId { get; set; }
            public int? ArticleID { get; set; }
            public string ArticleNo { get; set; }
            public string ArticleName { get; set; }
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
            public byte? ItemTypeID { get; set; }
            public string ItemTypeName { get; set; }
            public byte? LeatherTypeID { get; set; }
            public string LeatherTypeName { get; set; }
            public byte? LeatherStatusID { get; set; }
            public string LeatherStatusName { get; set; }

            

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

            public List<SlsBuyerOrderItemColor> ItemColors { get; set; }
        }

        public class SlsBuyerOrderItemColorBadhon
        {
            public int SlNo { get; set; }
            public long OrderItemColorId { get; set; }
            public long? OrderId { get; set; }
            public long? OrderItemId { get; set; }


            public int? ColorId { get; set; }
            public int? ColorID { get; set; }
            public string ColorName { get; set; }
            //public int? ColorQty { get; set; }

            //public decimal? ColorMeterQty { get; set; }
            //public decimal? ColorMeterUnitPrice { get; set; }
            //public decimal? ColorFootQty { get; set; }
            //public decimal? ColorFootUnitPrice { get; set; }

            //public decimal? ColorFootTotalPrice { get; set; }
            //public decimal? ColorMeterTotalPrice { get; set; }
            //public byte? ColorUnit { get; set; }
            //public string ColorUnitName { get; set; }
            //public decimal? UnitPrice { get; set; }
            //public decimal? TotalPrice { get; set; }

            public Nullable<decimal> ColorFootQty { get; set; }
            public Nullable<decimal> SeaFootUnitPrice { get; set; }
            public Nullable<decimal> SeaFootTotalPrice { get; set; }
            public Nullable<decimal> AirFootUnitPrice { get; set; }
            public Nullable<decimal> AirFootTotalPrice { get; set; }
            public Nullable<decimal> RoadFootUnitPrice { get; set; }
            public Nullable<decimal> RoadFootTotalPrice { get; set; }
            public Nullable<decimal> ColorMeterQty { get; set; }
            public Nullable<decimal> SeaMeterUnitPrice { get; set; }
            public Nullable<decimal> SeaMeterTotalPrice { get; set; }
            public Nullable<decimal> AirMeterUnitPrice { get; set; }
            public Nullable<decimal> AirMeterTotalPrice { get; set; }
            public Nullable<decimal> RoadMeterUnitPrice { get; set; }
            public Nullable<decimal> RoadMeterTotalPrice { get; set; }
        }


        public class SlsBuyerOrderDeliveryBadhon
        {
            public long BuyerOrderDeliveryID { get; set; }
            public long? BuyerOrderID { get; set; }
            public int? OrdDeliverySL { get; set; }

            public DateTime? TempOrdDeliveryDate { get; set; }
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

       
}
