using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.AppSetupModel
{
    public class SysBuyer
    {
        public int BuyerID { get; set; }
        public string BuyerCode { get; set; }
        public string BuyerName { get; set; }
        public string BuyerCategory { get; set; }
        public string BuyerType { get; set; }
        public string IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime SetOn { get; set; }
        public int SetBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public virtual IList<SysBuyerAddress> BuyerAddressList { get; set; }
        public virtual IList<SysBuyerAgent> BuyerAgentList { get; set; }

        public int? Count { get; set; }
        public IList<SysBuyer> BuyerList = new List<SysBuyer>();
        public string Address { get; set; }
        public string BuyerAddressID { get; set; }
        public string BuyerContactNumber { get; set; }
    }
}
