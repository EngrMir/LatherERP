using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.AppSetupModel
{
    public class SysBank
    {
        public int BankID { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string BankCategory { get; set; }
        public string BankType { get; set; }
        public string BankBINNo { get; set; }
        public string BankSwiftCode { get; set; }
        public string IsActive { get; set; }

        #region ShowField
        public string BankCategoryName { get; set; }
        public string BankTypeName { get; set; }
        public int BranchID { get; set; }
        public string BranchCode { get; set; }
        public string BanchCode { get; set; }
        public string BranchName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }



        public string InsuranceCategoryName { get; set; }
        public string InsuranceTypeName { get; set; }
        public List<SysBranch> Branches { get; set; }
        public string InsuranceCode { get; set; }
        public string InsuranceName { get; set; }



        #endregion
    }
}
