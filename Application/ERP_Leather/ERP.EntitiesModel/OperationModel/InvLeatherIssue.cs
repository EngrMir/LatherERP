using System;
using System.Collections.Generic;
using ERP.EntitiesModel.BaseModel;

namespace ERP.EntitiesModel.OperationModel
{
    public class InvLeatherIssue : CommonStatusInformation
    {
        public long IssueID { get; set; }
        public string IssueDate { get; set; }
        public string IssueFor { get; set; }
        public string IssueForName { get; set; }
        public string IssueRef { get; set; }
        public byte IssueFrom { get; set; }
        public string IssueFromName { get; set; }
        public byte? IssueTo { get; set; }
        public string IssueToName { get; set; }
        public string JobOrderNo { get; set; }
        public string PurchaseYear { get; set; }
        public string RecordStatusName { get; set; }
        public int? CheckedBy { get; set; }
        public string CheckDate { get; set; }
        public string CheckComment { get; set; }
        public int? ApprovedBy { get; set; }
        public string ApproveDate { get; set; }
        public string ApproveComment { get; set; }
        public virtual IList<InvLeatherIssueItem> LeatherIssueItemList { get; set; }
    }
}
