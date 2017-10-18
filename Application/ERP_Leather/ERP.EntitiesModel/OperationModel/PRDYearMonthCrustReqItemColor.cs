using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;

namespace ERP.EntitiesModel.OperationModel
{
    public class PRDYearMonthCrustReqItemColor: CommonStatusInformation
    {
        public long ReqItemColorID { get; set; }
        public long? RequisitionItemID { get; set; }
        public int? ColorID { get; set; }
        public int? ArticleColorNo { get; set; }
        public int? ColorPCS { get; set; }
        public int? ColorSide { get; set; }
        public decimal? ColorArea { get; set; }
        public byte? AreaUnit { get; set; }
        public string AreaUnitName { get; set; }
        //public decimal? ColorWeight { get; set; }
        //public byte? WeightUnit { get; set; }
        public string Remarks { get; set; }
    }
}
