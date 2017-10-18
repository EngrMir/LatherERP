using ERP.EntitiesModel.BaseModel;

namespace ERP.EntitiesModel.AppSetupModel
{
    public class SysLeatherStatus 
    {
        public byte LeatherStatusID { get; set; }
        public string LeatherStatusName { get; set; }
        public string Description { get; set; }
        public string IsActive { get; set; }
        public bool IsDelete { get; set; }
    }
}
