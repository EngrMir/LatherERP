namespace ERP.EntitiesModel.AppSetupModel
{
    public class SysLocation
    {
        public int LocationID { get; set; }
        public string LocationName { get; set; }
        public string LocationAddress { get; set; }
        public string ContactPerson { get; set; }
        public string ContactNumber { get; set; }
        public string IsActive { get; set; }
        public bool IsDelete { get; set; }
    }
}
