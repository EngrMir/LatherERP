using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.AppSetupModel
{
    public class SysPreDefineValue
    {
        public int PreDefineValueID { get; set; }
        public int? PreDefineValueForID { get; set; }
        public string PreDefineValueTitle { get; set; }
        public string PreDefineValueContent { get; set; }
        public string PreDefineValueGroup { get; set; }
        public string PreDefineValueIncreaseBy { get; set; }
        public string MaxValue { get; set; }
        public string InternalMailAddress { get; set; }
        public string InternalMailAutoSend { get; set; }
        public string ExternalMailAddress { get; set; }
        public string ExternalMailAutoSend { get; set; }
        public string Remarks { get; set; }
        public string IsActive { get; set; }

        #region ShowField
        public string PreDefineValueFor { get; set; }

        #endregion
    }
}
