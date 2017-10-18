using ERP.EntitiesModel.BaseModel;
using System;

namespace ERP.EntitiesModel.AppSetupModel
{
    public class SysGradeRangeFormat 
    {
        public Int64 FormatID { get; set; }
        public short? GradeRangeID { get; set; }
        public short? GradeIDFrom { get; set; }
        public short? GradeIDTo { get; set; }

        public string GradeRangeName { get; set; }
        public string GradeDescription { get; set; }
        public bool IsActiveGradeRange { get; set; }
        public string GradeNameFrom { get; set; }
        public string GradeNameTo { get; set; }

        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
    }
}
