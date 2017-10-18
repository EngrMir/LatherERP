namespace ERP.EntitiesModel.BaseModel
{
    public class CommonStatusForSetupData
    {
        public string SetOn { get; set; }
        public int SetBy { get; set; }
        public string ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public string IPAddress { get; set; }
    }
}
