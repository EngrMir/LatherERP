using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public  class CrustReportModel
    {
        public string StoreID { get; set; }
        public string ItemTypeID { get; set; }
        public string ProductionStatus { get; set; }
        public string ScheduleYear { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }

        public string ReportName { get; set; }
        public string ReportType { get; set; }
    }
}

