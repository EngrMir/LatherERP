using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class EXPPLPI
    {
        public long PLPIID { get; set; }
        public long? PLID { get; set; }
        public long? BuyerOrderID { get; set; }
        public decimal? PLPIPcs { get; set; }
        public decimal? PLPISide { get; set; }
        public decimal? MeterPLPIQty { get; set; }
        public decimal? FootPLPIQty { get; set; }
        public decimal? PLPINetWeight { get; set; }
        public byte? PLPINetWeightUnit { get; set; }
        public decimal? PLPIGrossWeight { get; set; }
        public byte? PLPIGrossWeightUnit { get; set; }
        public string RecordStatus { get; set; }

        public long? PIID { get; set; }

        public string PINo { get; set; }
        public string BuyerOrderNo { get; set; }

        public long? BuyerID { get; set; }
        public string BuyerName { get; set; }

        public long? LCID { get; set; }
        public string LCNo { get; set; }
    }
}
