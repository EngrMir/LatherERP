using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;

namespace ERP.EntitiesModel.OperationModel
{
    public class PRDYearMonthCrustReqDate : CommonStatusInformation
    {
        public long RequisitionDateID { get; set; }
        public long? ScheduleID { get; set; }
        public string RequisitionNo { get; set; }
        public string RequiredDate { get; set; }
        public string RequisitionStatus { get; set; }
        public string Remark { get; set; }

        public decimal ReqPcs { get; set; }
        public decimal ReqSide { get; set; }
        public decimal ReqArea { get; set; }
        public decimal IssuePcs { get; set; }
        public decimal IssueSide { get; set; }
        public decimal IssueArea { get; set; }
        public decimal RemainPcs { get; set; }
        public decimal RemainSide { get; set; }
        public decimal RemainArea { get; set; }




        public long YearMonID { get; set; }
        public string ScheduleYear { get; set; }
        public string ScheduleMonth { get; set; }
        public string ScheduleMonthName { get; set; }
        public string ScheduleFor { get; set; }
        public string ScheduleForName { get; set; }
        public byte? ProductionFloor { get; set; }
        public byte? ConcernStore { get; set; }
        public string ConcernStoreName { get; set; }
        public string ProductionFloorName { get; set; }

        public string ScheduleNo { get; set; }
        public string PrepareDate { get; set; }
    }
}
