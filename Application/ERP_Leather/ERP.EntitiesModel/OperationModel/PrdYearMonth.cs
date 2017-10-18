using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class PrdYearMonth
    {
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
        public string Remarks { get; set; }
        public string RecordStatus { get; set; }
        public virtual IList<PrdYearMonthSchedule> PrdYearMonthScheduleList { get; set; }
        public virtual IList<PrdYearMonthScheduleDate> PrdYearMonthScheduleDateList { get; set; }
        public virtual IList<PrdYearMonthSchedulePurchase> PrdYearMonthSchedulePurchaseList { get; set; }

        #region YearMonthSchedule

        public long ScheduleID { get; set; }
        public string ScheduleNo { get; set; }
        public string PrepareDate { get; set; }
        public int? ProductionProcessID { get; set; }
        public string ProductionProcessName { get; set; }
        public string ProcessName { get; set; }
        public string ScheduleStatus { get; set; }
        public string ScheduleStatusName { get; set; }

        #endregion
    }
}
