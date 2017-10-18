using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.AppSetupModel
{
    public class SysProductionProces
    {
        public int ProcessID { get; set; }
        public string ProcessName { get; set; }
        public string ProcessCategory { get; set; }
        public string ProcessCategoryName { get; set; }
        public int? ProcessLevel { get; set; }
        public string ProcessEffect { get; set; }
        public string ProcessEffectName { get; set; }
        public string IsActive { get; set; }
    }
}
