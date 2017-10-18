

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class EXPDeliveryChallan
    {
        public long DeliverChallanID { get; set; }
        public string DeliverChallanNo { get; set; }
        public string DeliverChallanRef { get; set; }
        public DateTime DeliverChallanDate { get; set; }
        public string TruckNo { get; set; }
        public string DeliveryChallanNote { get; set; }
        public string RecordStatus { get; set; }
        public Nullable<int> ConfirmedBy { get; set; }
        public Nullable<System.DateTime> ConfirmeDate { get; set; }
        public string ConfirmNote { get; set; }
        public Nullable<System.DateTime> SetOn { get; set; }
        public Nullable<int> SetBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public string IPAddress { get; set; }

        #region Display Field

        public long CIID { get; set; }
        public Nullable<long> PLID { get; set; }
        public string CINo { get; set; }
        public string CIDate { get; set; }
        public string PLNo { get; set; }
        public string PLDate { get; set; }
        public decimal? CIAmount { get; set; }


        #endregion



        public virtual IList<EXPDeliveryChallanCI> expDeliveryChallanCIList { get; set; }


    }
}



