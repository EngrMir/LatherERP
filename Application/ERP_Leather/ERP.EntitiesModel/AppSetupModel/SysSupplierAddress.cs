using ERP.EntitiesModel.BaseModel;
using System;

namespace ERP.EntitiesModel.AppSetupModel
{
    public class SysSupplierAddress
    {
        public int SupplierAddressID { get; set; }
        public int? SupplierID { get; set; }
        public string Address { get; set; }
        public string ContactPerson { get; set; }
        public string ContactNumber { get; set; }
        public string EmailAddress { get; set; }
        public string FaxNo { get; set; }
        public string PhoneNo { get; set; }
        public string SkypeID { get; set; }
        public string Website { get; set; }
        public string IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime SetOn { get; set; }
        public int SetBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
    }
}
