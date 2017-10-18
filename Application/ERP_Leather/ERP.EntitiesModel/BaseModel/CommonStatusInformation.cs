using System;


namespace ERP.EntitiesModel.BaseModel
{
    public class CommonStatusInformation
    {
        public string RecordStatus { get; set; }
        public DateTime SetOn { get; set; }
        public int SetBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public string IPAddress { get; set; }
        public int CrudStatus { get; set; }
    }
}
