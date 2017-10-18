using System;
using ERP.EntitiesModel.BaseModel;
using System.Collections.Generic;

namespace ERP.EntitiesModel.OperationModel
{
    public class PrqPurchaseYearPeriod : CommonStatusInformation
    {
        public int PeriodID { get; set; }
        public string PeriodName { get; set; }
        public int YearID { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public virtual IList<PrqPurchaseYearPeriodItem> PurchaseYearPeriodItemList { get; set; }
    }
}
