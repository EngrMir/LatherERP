using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;

namespace ERP.EntitiesModel.OperationModel
{
    public class PRDYearMonthCrustReqItem: CommonStatusInformation
    {
        public long RequisitionItemID { get; set; }
        public long SelectedRequisitionItemID { get; set; }
        public long? RequisitionDateID { get; set; }
        public string RequisitionNo { get; set; }
        public long? ScheduleItemID { get; set; }
        public string ScheduleProductionNo { get; set; }

        public int? BuyerID { get; set; }
        public long? BuyerOrderID { get; set; }
        public int? ArticleID { get; set; }
        public string ArticleNo { get; set; }
       
        public string ArticleChallanNo { get; set; }

        public long ArticleChallanID { get; set; }
        public string AvgSize { get; set; }
        public byte? AvgSizeUnit { get; set; }
        public string AvgSizeUnitName { get; set; }
        public string SelectionRange { get; set; }
        public string SideDescription { get; set; }
        public string Thickness { get; set; }
        public byte? ThicknessUnit { get; set; }
        public string ThicknessUnitName { get; set; }
        public string ThicknessAt { get; set; }


        public byte? ItemTypeID { get; set; }
        public string ItemTypeName { get; set; }
        public byte? LeatherTypeID { get; set; }
        public byte? LeatherStatusID { get; set; }
        public string LeatherStatusName { get; set; }


        public int? RequisitionPcs { get; set; }
        public int RequisitionSide { get; set; }
        public decimal? RequisitionArea { get; set; }
        public byte? AreaUnit { get; set; }
        public string Remarks { get; set; }

        public string ConfirmNote { get; set; }

        public IList<PRDYearMonthCrustReqItem> ItemList { get; set; }
        public IList<PRDYearMonthCrustReqItemColor> ColorList { get; set; }
    }
}
