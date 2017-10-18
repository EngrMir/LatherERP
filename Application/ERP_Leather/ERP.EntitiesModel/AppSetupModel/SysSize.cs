using ERP.EntitiesModel.BaseModel;

namespace ERP.EntitiesModel.AppSetupModel
{
    public class SysSize
    {
        public byte SizeID { get; set; }
        public string SizeName { get; set; }
        public int Size { get; set; }
        public string SizeCategory { get; set; }
        public bool IsDelete { get; set; }
        public string IsActive { get; set; }
    }

    public class SizeVM
    {
        public byte SizeID { get; set; }
        public string SizeName { get; set; }
    }
}
