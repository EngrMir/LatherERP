using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class PRQChemFrgnPurcRecvChallan
    {
        public int ReceiveChallanID { get; set; }
        public int? PLReceiveID { get; set; }
        public int? ReceiveID { get; set; }
        public string ReceiveChallanNo { get; set; }
        public string ChallanDate { get; set; }
        public decimal? CarringCost { get; set; }
        public decimal? LaborCost { get; set; }
        public decimal? OtherCost { get; set; }
        public byte? Currency { get; set; }
        public byte? ExchangeCurrency { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? ExchangeValue { get; set; }
        public byte? CostIndicator { get; set; }
        public string Remark { get; set; }
    }
}
