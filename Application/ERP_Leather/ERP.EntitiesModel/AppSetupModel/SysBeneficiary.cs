using System.Collections.Generic;
using ERP.EntitiesModel.BaseModel;
using System;
using ERP.EntitiesModel.OperationModel;

namespace ERP.EntitiesModel.AppSetupModel
{
    public class SysBeneficiary
    {
        public int BeneficiaryID { get; set; }
        public string BeneficiaryCode { get; set; }
        public string BeneficiaryName { get; set; }
        public int BeneficiaryAddressID { get; set; }
        public string BeneficiaryAddress { get; set; }

        public string BeneficiaryContactNumber { get; set; }

    }
}
