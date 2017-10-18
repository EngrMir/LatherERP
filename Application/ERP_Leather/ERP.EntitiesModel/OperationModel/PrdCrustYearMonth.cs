using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class PrdCrustYearMonth
    {
        #region YearMonth

        public long YearMonID { get; set; }
        public string ScheduleYear { get; set; }
        public string ScheduleMonth { get; set; }
        public string ScheduleMonthName { get; set; }
        public string ScheduleFor { get; set; }
        public string ScheduleForName { get; set; }
        public byte? ProductionFloor { get; set; }
        public string ProductionFloorName { get; set; }
        public string Remarks { get; set; }
        public string RecordStatus { get; set; }
        public virtual IList<PrdYearMonthCrustScheduleItem> PrdYearMonthCrustScheduleItemList { get; set; }
        public virtual IList<PrdYearMonthCrustScheduleColor> PrdYearMonthCrustScheduleColorList { get; set; }
        public virtual IList<PrdYearMonthCrustScheduleDrum> PrdYearMonthCrustScheduleDrumList { get; set; }

        #endregion

        #region YearMonthSchedule

        public long ScheduleID { get; set; }
        public string ScheduleNo { get; set; }
        public string PrepareDate { get; set; }
        public int? ProductionProcessID { get; set; }
        public string ProcessName { get; set; }

        #endregion

        #region YearMonthScheduleDate

        public long ScheduleDateID { get; set; }
        public string ProductionNo { get; set; }
        public string ScheduleStartDate { get; set; }
        public string ScheduleEndDate { get; set; }
        public string ProductionStatus { get; set; }

        #endregion

        #region For Stock Update

        public int? BuyerID { get; set; }
        public long? BuyerOrderID { get; set; }
        public int? ArticleID { get; set; }
        public string ArticleNo { get; set; }
        public byte? ItemTypeID { get; set; }
        public byte? LeatherStatusID { get; set; }
        public long? ArticleChallanID { get; set; }
        public string ArticleChallanNo { get; set; }

        #endregion
    }
}
