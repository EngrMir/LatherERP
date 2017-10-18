using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
   public class ExpPiOrderReportModel
   {
       public string PIID { get; set; }
       public string PINo { get; set; }
       public string PiDate { get; set; }
       public string BuyerID { get; set; }
       public string BuyerName { get; set; }
       public string ArticleID { get; set; }
       public string ArticleName { get; set; }
       public string ArticleChallanId { get; set; }
       public string ArticleChallanNo { get; set; }
       
       public string BuyerOrderID { get; set; }
       public string BuyerOrderNo { get; set; }
       public string ItemTypeID { get; set; }
       public string ItemTypeName { get; set; }
       public string LeatherTypeID { get; set; }
       public string LeatherTypeName { get; set; }
       public string LeatherStatusID { get; set; }
       public string LeatherStatusName { get; set; }
       public string ColorID { get; set; }
       public string ColorName { get; set; }
       public string PriceLevel { get; set; }
       public string ReportName { get; set; }
       public string ReportType { get; set; }
       public string FromDate { get; set; }
       public string ToDate { get; set; }

    }
}
