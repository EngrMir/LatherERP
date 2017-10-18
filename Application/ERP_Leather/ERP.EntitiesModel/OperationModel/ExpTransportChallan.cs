using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class ExpTransportChallan
    {
        public long TransportChallanID { get; set; }
        public string TransportChallanNo { get; set; }
        public string TransportChallanRef { get; set; }
        public string TransportChallanDate { get; set; }
        public string TransportChallanNote { get; set; }
        public long? DeliverChallanID { get; set; }
        public string DeliverChallanNo { get; set; }
        public string Sender { get; set; }
        public string SenderAddress { get; set; }
        public string Receiver { get; set; }
        public string ReceverAddress { get; set; }
        public string DeliveryFrom { get; set; }
        public string DEliveryTo { get; set; }
        public string ConfirmNote { get; set; }
        public string RecordStatus { get; set; }
        public List<ExpTransportChallanCI> ChallanCis { get; set; }
    }

    public class ExpTransportChallanCI
    {
        public long TransportChallanID { get; set; }
        public long CIID { get; set; }
        public string CINo { get; set; }
        public string CIDate { get; set; }
        public long? PLID { get; set; }
        public string PLNo { get; set; }
        public string PLDate { get; set; }
        public long? DeliverChallanID { get; set; }
        public string RecordStatus { get; set; }
    }

}
