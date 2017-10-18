using ERP.EntitiesModel.BaseModel;

namespace ERP.EntitiesModel.AppSetupModel
{
    public class SysSource
    {
        public int SourceID { get; set; }
        public string SourceName { get; set; }
        public string SourceAddress { get; set; }
        public string ContactPerson { get; set; }
        public string ContactNumber { get; set; }
        public string IsActive { get; set; }
        public bool IsDelete { get; set; }
    }
}
