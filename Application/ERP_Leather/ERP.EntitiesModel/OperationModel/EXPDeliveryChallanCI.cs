using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class EXPDeliveryChallanCI
    {
        public long DeliverChallanID { get; set; }
        public long CIID { get; set; }
        public Nullable<long> PLID { get; set; }
        public string RecordStatus { get; set; }
        public Nullable<System.DateTime> SetOn { get; set; }
        public Nullable<int> SetBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public string IPAddress { get; set; }


        #region Display Field


        public string CINo { get; set; }
        public string CIDate { get; set; }
        public string PLNo { get; set; }
        public string PLDate { get; set; }
        public decimal? CIAmount { get; set; }


        #endregion

        public virtual IList<EXPDeliveryChallanCI> expDeliveryChallanCIList { get; set; }

    }
}













