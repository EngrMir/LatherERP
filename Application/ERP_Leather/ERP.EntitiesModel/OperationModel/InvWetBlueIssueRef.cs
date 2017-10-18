using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.EntitiesModel.OperationModel
{
    public  class InvWetBlueIssueRef
    {
        public long WetBlueIssueRefID { get; set; }
        public long? WetBlueIssueID { get; set; }
        public long? WetBlueReqID { get; set; }
        public int? BuyerID { get; set; }
        public string BuyerName { get; set; }
        public long? BuyerOrderID { get; set; }
        public string BuyerOrderNo { get; set; }
        
        public int? ArticleID { get; set; }
        public string ArticleNo { get; set; }
        public string ArticleName { get; set; }
        public long? ArticleChallanID { get; set; }
        public string ArticleChallanNo { get; set; }
        
        public string AvgSize { get; set; }
        public byte? AvgSizeUnit { get; set; }
        public string AvgSizeUnitName { get; set; }
        public string SideDescription { get; set; }
        public string SelectionRange { get; set; }
        public string Thickness { get; set; }
        public byte? UnitID { get; set; }
        public string UnitName { get; set; }
        public string ThicknessAtID { get; set; }
        public string ThicknessAt { get; set; }
        public DateTime? SetOn { get; set; }
        public int? SetBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public string IPAddress { get; set; }
        public string Remarks { get; set; }
    
    }
}
