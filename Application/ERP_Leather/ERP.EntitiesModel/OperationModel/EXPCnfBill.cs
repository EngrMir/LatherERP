using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class EXPCnfBill
    {
        public long CnfBillID { get; set; }
        public string CnfBillNo { get; set; }
        public string RefCnfBillNo { get; set; }
        public string CnfBillDate { get; set; }
        public Nullable<long> CIID { get; set; }
        public Nullable<long> PLID { get; set; }
        public string BillOfEntryNo { get; set; }
        public string BillOfEntryDate { get; set; }
        public Nullable<decimal> AssesmentValue { get; set; }
        public string CnfBillNote { get; set; }
        public Nullable<int> CnfAgentID { get; set; }
        public decimal? DutyAccountCharge { get; set; }
        public Nullable<decimal> PortCharge { get; set; }
        public Nullable<decimal> ShippingCharge { get; set; }
        public Nullable<decimal> BerthOperatorCharge { get; set; }
        public Nullable<decimal> NOCCharge { get; set; }
        public Nullable<decimal> ExamineCharge { get; set; }
        public Nullable<decimal> SpecialDeliveryCharge { get; set; }
        public Nullable<decimal> AmendmentCharge { get; set; }
        public Nullable<decimal> ChemicalTestCharge { get; set; }
        public Nullable<decimal> AgencyCommission { get; set; }
        public Nullable<decimal> StampCharge { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<byte> CnfBillCurrency { get; set; }
        public Nullable<byte> ExchangeCurrency { get; set; }
        public Nullable<decimal> ExchangeRate { get; set; }
        public Nullable<decimal> ExchangeValue { get; set; }
        public string RecordStatus { get; set; }
        public Nullable<System.DateTime> SetOn { get; set; }
        public Nullable<int> SetBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public string IPAddress { get; set; }


        #region Display Field

        public int BuyerID { get; set; }
        public string AgentCode { get; set; }
        public string AgentName { get; set; }
        public string BuyerCode { get; set; }
        public string BuyerName { get; set; }
        public string ContactPerson { get; set; }
        public string ContactNumber { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNo { get; set; }
        public string BuyerCategory { get; set; }
        public string BuyerType { get; set; }
        public int BuyerAddressID { get; set; }
        public string Address { get; set; }
        public Nullable<long> PIID { get; set; }
        public long LCID { get; set; }
        public string LCNo { get; set; }
        public string PINo { get; set; }
        public string CINo { get; set; }
        public string CIRefNo { get; set; }
        public string PLNo { get; set; }
        public string BalesNo { get; set; }
        public Nullable<decimal> BaleQty { get; set; }
        public Nullable<decimal> TotalBales { get; set; }

        #endregion



    }
}
