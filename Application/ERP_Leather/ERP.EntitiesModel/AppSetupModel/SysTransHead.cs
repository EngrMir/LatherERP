using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.AppSetupModel
{
    public class SysTransHead
    {
        public int HeadID { get; set; }
        public string HeadCode { get; set; }
        public string HeadName { get; set; }
        public string HeadCategory { get; set; }
        public string HeadType { get; set; }
        public string IsActive { get; set; }
        public Nullable<System.DateTime> SetOn { get; set; }
        public Nullable<int> SetBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public string IPAddress { get; set; }
    }
}
