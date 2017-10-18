using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.AppSetupModel
{
    public class SysPreDefineValueFor
    {
        public int PreDefineValueForID { get; set; }
        public string PreDefineValueFor { get; set; }
        public string ConcernPageID { get; set; }
        public string Remarks { get; set; }
        public string IsActive { get; set; }
        public virtual IList<SysPreDefineValue> PreDefineValueList { get; set; }

        #region ShowField
        public string ConcernPage { get; set; }

        #endregion
    }
}
