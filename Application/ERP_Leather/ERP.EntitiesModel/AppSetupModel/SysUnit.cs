using ERP.EntitiesModel.BaseModel;

namespace ERP.EntitiesModel.AppSetupModel
{
    public class SysUnit
    {
        public byte UnitID { get; set; }
        public string UnitName { get; set; }
        public string UnitCategory { get; set; }
        public string IsActive { get; set; }
        public bool IsDelete { get; set; }
    }
}
