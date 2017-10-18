using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class PrdYearMonthSchedule
    {
        public long ScheduleID { get; set; }
        public string ScheduleNo { get; set; }
        public long? YearMonID { get; set; }
        public string PrepareDate { get; set; }
        public int? ProductionProcessID { get; set; }
        public string ProductionProcessName { get; set; }
        public string ScheduleStatus { get; set; }
        public string Remarks { get; set; }
        public string RecordStatus { get; set; }
        public decimal? OtherCost { get; set; }
        public byte? CheckedBy { get; set; }
        public string CheckComments { get; set; }
        public byte? ApprovedBy { get; set; }
        public string ApprovalAdvice { get; set; }
        public string ScheduleFor { get; set; }
        public string ScheduleYear { get; set; }
        public string ScheduleMonth { get; set; }
        public byte? ProductionFloor { get; set; }
        public string ProductionFloorName { get; set; }
    }
}
