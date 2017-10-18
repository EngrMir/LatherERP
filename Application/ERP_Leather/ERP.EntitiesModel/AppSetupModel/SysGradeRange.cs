using ERP.EntitiesModel.BaseModel;

namespace ERP.EntitiesModel.AppSetupModel
{
    public class SysGradeRange
    {
        public short GradeRangeID { get; set; }
        public string GradeRangeName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
    }
}
