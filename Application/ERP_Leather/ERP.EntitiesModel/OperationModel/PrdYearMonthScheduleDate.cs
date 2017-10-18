using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class PrdYearMonthScheduleDate
    {
        public long ScheduleDateID { get; set; }
        public long? ScheduleID { get; set; }
        public string ProductionNo { get; set; }
        public string ScheduleStartDate { get; set; }
        public string ScheduleEndDate { get; set; }
        public decimal? SchedulePcs { get; set; }
        public decimal? ScheduleSide { get; set; }
        public decimal? ScheduleWeight { get; set; }
        public byte? ScheduleWeightUnit { get; set; }
        public string ScheduleWeightUnitName { get; set; }
        public string ProductionStatus { get; set; }
        public string Remark { get; set; }
        public string RecordStatus { get; set; }
        public string UnitName { get; set; }
        public long ProductionProcessID { get; set; }

        #region Query Show Filed

        public string ScheduleNo { get; set; }
        public string ProcessName { get; set; }

        #endregion
    }
}
