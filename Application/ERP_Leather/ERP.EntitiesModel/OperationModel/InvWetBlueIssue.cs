using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
  public  class InvWetBlueIssue
    {
        public InvWetBlueIssue()
        {
            ItemModels = new HashSet<InvWetBlueIssueItem>();
            RefModels = new HashSet<InvWetBlueIssueRef>();
        }
        public long WetBlueIssueID { get; set; }
        public string WetBlueIssueNo { get; set; }
        public string WetBlueIssueDate { get; set; }
        public string IssueCategory { get; set; }
        public string IssueFor { get; set; }
        public byte? IssueFrom { get; set; }
        public string IssueFromName { get; set; }
        public byte? IssueTo { get; set; }
        public string IssueToName { get; set; }
        public long RequisitionDateID { get; set; }
        public string RequisitionNo { get; set; }
      
        public string RecordStatus { get; set; }
        public int? IssuedBy { get; set; }
        public string IssueDate { get; set; }
        public string IssueNote { get; set; }
        public int? CheckedBy { get; set; }
        public DateTime? CheckDate { get; set; }
        public string CheckNote { get; set; }
        public int? ConfirmedBy { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public DateTime? SetOn { get; set; }
        public int? SetBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public string IPAddress { get; set; }

        public ICollection<InvWetBlueIssueItem> ItemModels { get; set; }
        public ICollection<InvWetBlueIssueRef> RefModels { get; set; }
    }

    public class WetBlueIssue
    {
        public WetBlueIssue WetBlueIssueInfo { get; set; }
        public virtual ICollection<InvWetBlueIssueItem> ItemModels { get; set; }
        public virtual ICollection<InvWetBlueIssueRef> RefModels { get; set; }

    }

    public class BuyerOrderItem
    {
        public long BuyerOrderID { get; set; }
        public string BuyerOrderNo { get; set; }
        public string OrderNo { get; set; }
        public int? ArticleID { get; set; }
        public string ArticleNo { get; set; }
        public string ArticleName { get; set; }

    }

}
