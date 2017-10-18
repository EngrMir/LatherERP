using System.Collections.Generic;
using ERP.EntitiesModel.BaseModel;
using System;
using ERP.EntitiesModel.OperationModel;

namespace ERP.EntitiesModel.AppSetupModel
{
    public class SysSupplier
    {
        public int SupplierID { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string SupplierCategory { get; set; }
        public string SupplierType { get; set; }
        public string IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime SetOn { get; set; }
        public int SetBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public virtual IList<SysSupplierAddress> SupplierAddressList { get; set; }
        public virtual IList<SysSupplierAgent> SupplierAgentList { get; set; }

        public int? Count { get; set; }
        public IList<SysSupplier> SupplierList = new List<SysSupplier>();
        public IList<SupplierDetails> ChemicalSupplierList = new List<SupplierDetails>();

        public int SupplierAddressID { get; set; }
        public string Address { get; set; }

        public int? CountryID { get; set; }
        public string CountryName { get; set; }
    }

    public class SupplierAgentInformation
    {
        public string SupplierID { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }

        public string LocalAgentID { get; set; }
        public string LocalAgentCode { get; set; }
        public string LocalAgentName { get; set; }

        public string ForeignAgentID { get; set; }
        public string ForeignAgentCode { get; set; }
        public string ForeignAgentName { get; set; }
    }

    public class SupplierPopupInfo
    {
        public int SupplierID { get; set; }
       
        public string ContactPerson { get; set; }
        public string ContactNumber { get; set; }
        public string Address { get; set; }
        public string SupplierName { get; set; }
        public string SupplierCode { get; set; }
    }
}
