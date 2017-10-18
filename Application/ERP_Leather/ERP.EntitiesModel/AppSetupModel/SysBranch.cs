using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.AppSetupModel
{
    public class SysBranch
    {
        public int BranchID { get; set; }
        public int? BankID { get; set; }
        public string BanchCode { get; set; }
        public string BranchName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public decimal? LCLimit { get; set; }
        public decimal? LCOpened { get; set; }
        public decimal? LCBalance { get; set; }
        public decimal? LCMargin { get; set; }
        public decimal? LCMarginUsed { get; set; }
        public decimal? LCMarginBalance { get; set; }
        public string BranchSwiftCode { get; set; }
        public decimal? LIMLimit { get; set; }
        public decimal? LIMTaken { get; set; }
        public decimal? LIMBalance { get; set; }
        public string IsActive { get; set; }
        public string BankName { get; set; }
        public virtual IList<SysBranch> branchList { get; set; }



    }
}
