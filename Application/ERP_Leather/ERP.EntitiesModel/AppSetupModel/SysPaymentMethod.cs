using ERP.EntitiesModel.BaseModel;

namespace ERP.EntitiesModel.AppSetupModel
{
    public class SysPaymentMethod
    {
        public byte ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IsActive { get; set; }
        public bool IsDelete { get; set; }
    }
}
