using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public  class LcReportModel
    {
        public int LcId { get; set; }
        public string LcNo { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }

        public string ReportName { get; set; }
        public string ReportType { get; set; }
    }
}
