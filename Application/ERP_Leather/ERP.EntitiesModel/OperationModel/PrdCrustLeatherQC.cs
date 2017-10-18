using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class PrdCrustLeatherQC
    {
        public long CrustLeatherQCID { get; set; }
        public string CrustLeatherQCNo { get; set; }
        public string CrustLeatherQCDate { get; set; }
        public int? CrustLeatherQCBy { get; set; }
        public string QCComments { get; set; }
        public byte? ProductionFloor { get; set; }
        public byte? QCStore { get; set; }
        public string QCStoreName { get; set; }
        public string ProductionFloorName { get; set; }
        public byte? AfterQCIssueFloor { get; set; }
        public string AfterQCIssueFloorName { get; set; }
        public string IssueFloorQCLabel { get; set; }
        public byte? AfterQCIssueStore { get; set; }
        public string AfterQCIssueStoreName { get; set; }
        public string IssueStoreQCLabel { get; set; }
        public string IssueAfterQC { get; set; }
        public string QCTransactionOf { get; set; }
        public string QCTransactionOfName { get; set; }
        public int ConfirmedBy { get; set; }
        public string ConfirmedDate { get; set; }
        public string ConfirmedNote { get; set; }
        public int? CheckedBy { get; set; }
        public string CheckDate { get; set; }
        public string CheckNote { get; set; }
        public string Remarks { get; set; }
        public string RecordStatus { get; set; }
        public string RecordState { get; set; }
        public virtual IList<PrdCrustLeatherQCItem> PrdCrustLeatherQCItemList { get; set; }
        public virtual IList<PrdCrustLeatherQCSelection> PrdCrustLeatherQCSelectionList { get; set; }
    }
}
