using System;

namespace ERP.EntitiesModel.BaseModel
{
    public class CommonApprovalInformation
    {
        public int? CheckedBy { get; set; }
        public DateTime? CheckDate { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApproveDate { get; set; }
    }
}
