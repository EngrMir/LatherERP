using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class ExpBillOfLading
    {
        public long BLID { get; set; }
        public string BLNo { get; set; }
        public string RefBLNo { get; set; }
        public string BLDate { get; set; }
        public long? CIID { get; set; }
        public long? PLID { get; set; }
        public string PLNo { get; set; }
        public string ShippedOnBoardDate { get; set; }
        public string ExpectedArrivalTime { get; set; }
        public int? Shipper { get; set; }
        public string ShipmentMode { get; set; }
        public string VesselName { get; set; }
        public string VoyageNo { get; set; }
        public int? TransShipmentPort { get; set; }
        public string TransShipmentPortName { get; set; }
        public int? ShipmentPort { get; set; }
        public string ShipmentPortName { get; set; }
        public string BLNote { get; set; }
        public string RecordStatus { get; set; }
        public List<ExpBillOfLadingContainer> Container { get; set; }
    }

    public class ExpBillOfLadingContainer
    {
        public long BLCcntID { get; set; }
        public long? BLID { get; set; }
        public string ContainerNo { get; set; }
        public string ContainerType { get; set; }
        public string SealNo { get; set; }
        public int? PackageQty { get; set; }
        public decimal? GrossWeight { get; set; }
        public byte? WeightUnit { get; set; }
        public string WeightUnitName { get; set; }
        public byte? MeasurementUnit { get; set; }
        public decimal? Measurement { get; set; }
    }
}
