using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class LcmBillOfLadingVM
    {
        public int Blid { get; set; }
        public string BlNo { get; set; }
        public string BlDate { get; set; }
        public int? Lcid { get; set; }
        public string LcNo { get; set; }
        public string CINo { get; set; }
        public int? CIID { get; set; }
        public string PiNo { get; set; }
        public string ShippedOnBoardDate { get; set; }
        public string ExpectedArrivalTime { get; set; }
        public int? Shipper { get; set; }
        public string ShipperName { get; set; }
        public string ShipmentMode { get; set; }
        public string VesselName { get; set; }
        public string VoyageNo { get; set; }
        public int? TransShipmentPort { get; set; }
        public string TransShipmentPortName { get; set; }
        public int? ShipmentPort { get; set; }
        public string ShipmentPortName { get; set; }
        public string BlNote { get; set; }
        public string RecordStatus { get; set; }
        public List<LcmBillOfLadingContainerVM> Containers { get; set; } 
    }

    public class LcmBillOfLadingContainerVM
    {
        public int BlcCntId { get; set; }
        public int? Blid { get; set; }
        public string ContainerNo { get; set; }
        public string ContainerType { get; set; }
        public string SealNo { get; set; }
        public string PackageQty { get; set; }
        public decimal? GrossWeight { get; set; }
        public byte? WeightUnit { get; set; }
        public string WeightUnitName { get; set; }
    }
}
