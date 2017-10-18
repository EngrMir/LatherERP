using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class ReportModel
    {
        public string SupplierID { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string GradeRangeID { get; set; }
        public string StoreID { get; set; }
        public string ItemTypeID { get; set; }
        public string LeatherTypeID { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string ItemID { get; set; } //For chemical item
        public string ItemName { get; set; }
        public string ItemCategory { get; set; } 
        public string ReportName { get; set; }
        public string ReportType { get; set; }
        public string GradeID { get; set; }
        public string PurchaseID { get; set; }
        public string FilterExpression { get; set; }
        public string BuyerID { get; set; }
        public string BuyerName { get; set; }
        public string ArticalID { get; set; }
        public string ArticleID { get; set; }
        public string ArticleName { get; set; }
        public string ArticleChallanID { get; set; }
        public string ArticleChallanNo { get; set; }

        public string CnfBillID { get; set; }
        public string CnfBillNo { get; set; }

        public string DeliverChallanID { get; set; }
        public string DeliverChallanNo { get; set; }

        public string LCRetirementID { get; set; }
        public string LCRetirementNo { get; set; }

        public string CIID { get; set; }
        public string CINo { get; set; }
    }
}
