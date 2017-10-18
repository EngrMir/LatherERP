using System;
using ERP.EntitiesModel.BaseModel;
using System.Collections.Generic;

namespace ERP.EntitiesModel.OperationModel
{
    public class InvLeatherTransferRecieve : CommonStatusInformation
    {
        public long ReceiveID { get; set; }
        public long IssueID { get; set; }
        //public string IssueRef { get; set; }
        public string ReceiveDate { get; set; }
        public byte ReceiveLocation { get; set; }
        public string ReceiveLocationName { get; set; }
        public string PurchaseYear { get; set; }
        //public string RecordStatus { get; set; }
        public int? CheckedBy { get; set; }
        public DateTime? CheckDate { get; set; }
        public string CheckComment { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string ApproveComment { get; set; }
        public virtual IList<InvLeatherTransferReceiveItem> LeatherTransferReceiveItemList { get; set; }
    }
}
