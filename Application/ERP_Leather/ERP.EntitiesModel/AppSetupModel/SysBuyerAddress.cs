using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.AppSetupModel
{
    public class SysBuyerAddress
    {
        public int BuyerAddressID { get; set; }
        public int? BuyerID { get; set; }
        public string Address { get; set; }
        public string ContactPerson { get; set; }
        public string ContactNumber { get; set; }
        public string EmailAddress { get; set; }
        public string FaxNo { get; set; }
        public string PhoneNo { get; set; }
        public string SkypeID { get; set; }
        public string IsActive { get; set; }
        public DateTime SetOn { get; set; }
        public int SetBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
    }
}
