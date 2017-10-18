using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class EXPPIItemColor
    {
        public long PIItemColorID { get; set; }
        public long PIItemID { get; set; }
        public int ColorID { get; set; }
        public string ColorName { get; set; }
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
}
