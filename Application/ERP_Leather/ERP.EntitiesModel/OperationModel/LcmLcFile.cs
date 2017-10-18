using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class LcmLcFile
    {
        public long LCFileID { get; set; }
        public string LCFileNo { get; set; }
        public Nullable<System.DateTime> LCFileOpenDate { get; set; }
        public string LCFileStatus { get; set; }
        public Nullable<System.DateTime> LCFileClosingDate { get; set; }
        public Nullable<int> LCID { get; set; }
        public string LCNo { get; set; }
        public string Remarks { get; set; }
        public string RecordStatus { get; set; }
        public Nullable<int> CheckedBy { get; set; }
        public Nullable<System.DateTime> CheckDate { get; set; }
        public string CheckComments { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public Nullable<System.DateTime> ApproveDate { get; set; }
        public string ApprovalComments { get; set; }
        public Nullable<System.DateTime> SetOn { get; set; }
        public Nullable<int> SetBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public string IPAddress { get; set; }
    }
}
