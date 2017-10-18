using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.AppSetupModel
{
    public class SysColor
    {
        public int ColorID { get; set; }
        public string ColorCode { get; set; }
        public string ColorName { get; set; }
        public string IsActive { get; set; }
        public IList<SysColor> ColorList = new List<SysColor>();
    }
}
