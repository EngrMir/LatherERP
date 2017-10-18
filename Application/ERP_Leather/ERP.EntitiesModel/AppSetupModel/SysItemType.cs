using ERP.EntitiesModel.BaseModel;

namespace ERP.EntitiesModel.AppSetupModel 
{
    public class SysItemType 
    {
        public byte ItemTypeID { get; set; }
        public string ItemTypeName { get; set; }
        public string ItemTypeCategory { get; set; }
        public string IsActive { get; set; }
        public bool IsDelete { get; set; }
    }
}
