using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class PRQChemLocalPurcRecv
    {
        public long ReceiveID { get; set; }
        public string ReceiveNo { get; set; }
        public string ReceiveDate { get; set; }
        public string ReceiveCategory { get; set; }
        public string ReceiveType { get; set; }
        public int? SupplierID { get; set; }
        public int? SupplierAddressID { get; set; }
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
        public virtual IList<PRQChemLocalPurcRecvChallan> PrqChemLocalPurcRecvChallanList { get; set; }
        public virtual IList<PRQChemLocalPurcRecvItem> PrqChemLocalPurcRecvItemList { get; set; }
        public virtual IList<PRQChemLocalPurcRecvPO> PrqChemLocalPurcRecvPOList { get; set; }
        public string SupplierName { get; set; }
        public string SupplierCode { get; set; }
    }
}
