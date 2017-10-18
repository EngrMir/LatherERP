using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class PrdYearMonthCrustScheduleColor
    {
        public long SdulItemColorID { get; set; }
        public long ReqItemColorID { get; set; }
        public long? ScheduleItemID { get; set; }
        public long? RequisitionItemID { get; set; }
        public int? ColorID { get; set; }
        public int? ArticleColorNo { get; set; }
        public string ColorName { get; set; }
        public short? GradeID { get; set; }
        public string GradeName { get; set; }
        public decimal? ColorPCS { get; set; }
        public decimal? ColorSide { get; set; }
        public decimal? ColorArea { get; set; }
        public byte? AreaUnit { get; set; }
        public string AreaUnitName { get; set; }
        public decimal? ColorWeight { get; set; }
        public byte? WeightUnit { get; set; }
        public string WeightUnitName { get; set; }
        public string ProductionStatus { get; set; }
        public string Remarks { get; set; }
    }
}
