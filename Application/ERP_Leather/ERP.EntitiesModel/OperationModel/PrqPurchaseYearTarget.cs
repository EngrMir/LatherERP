using System;
using ERP.EntitiesModel.BaseModel;
using System.Collections.Generic;

namespace ERP.EntitiesModel.OperationModel
{
    public class PrqPurchaseYearTarget : CommonStatusInformation
    {
        public int YearID { get; set; }
        public int PeriodID { get; set; }
        public string PurchaseYear { get; set; }
        public string YearStartDate { get; set; }
        public string YearEndDate { get; set; }
        public string YearStatus { get; set; }
        //public string RecordStatus { get; set; }
        public virtual IList<PrqPurchaseYearPeriod> PurchaseYearPeriodList { get; set; }
        public virtual IList<PrqPurchaseYearPeriod> PeriodList { get; set; }
        public virtual IList<PrqPurchaseYearPeriodItem> PurchaseYearPeriodItemList { get; set; }
    }
}
