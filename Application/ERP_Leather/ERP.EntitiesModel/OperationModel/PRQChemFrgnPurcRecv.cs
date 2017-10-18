using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class PRQChemFrgnPurcRecv
    {
        public int ReceiveID { get; set; }
        public string ReceiveNo { get; set; }
        public string ReceiveDate { get; set; }
        public string ReceiveCategory { get; set; }
        public string ReceiveType { get; set; }
        public int? SupplierID { get; set; }
        public int? SupplierAddressID { get; set; }
        public string Address { get; set; }
        public byte ReceiveStore { get; set; }
        public string ReceiveStoreName { get; set; }
        public string Remark { get; set; }
        public string RecordStatus { get; set; }
        public string RecordStatusName { get; set; }
        public int? CheckedBy { get; set; }
        public string CheckDate { get; set; }
        public string CheckComment { get; set; }
        public int? ApprovedBy { get; set; }
        public string ApproveDate { get; set; }
        public string ApproveComment { get; set; }
        public virtual IList<PRQChemFrgnPurcRecvChallan> PrqChemFrgnPurcRecvChallanList { get; set; }
        public virtual IList<PRQChemFrgnPurcRecvItem> PrqChemFrgnPurcRecvItemList { get; set; }
        public virtual IList<PRQChemFrgnPurcRecvPL> PrqChemFrgnPurcRecvPlList { get; set; }
        public string SupplierName { get; set; }
        public string SupplierCode { get; set; }

        public int? LCID { get; set; }
        public string LCNo { get; set; }
        public string LCDate { get; set; }

        public byte? Currency { get; set; }
        public byte? ExchangeCurrency { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? ExchangeValue { get; set; }

        public int OrderID { get; set; }
        public string OrderNo { get; set; }
        public string OrderDate { get; set; }
    }
}
