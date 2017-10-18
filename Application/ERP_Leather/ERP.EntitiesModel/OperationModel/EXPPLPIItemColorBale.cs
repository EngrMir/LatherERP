using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ERP.EntitiesModel.OperationModel
{
    public class EXPPLPIItemColorBale
    {
        public long PLPIItemColorBaleID { get; set; }
        public string PLPIItemColorBaleNo { get; set; }
        public long? PLPIItemColorID { get; set; }
        public short? GradeID { get; set; }
        public string GradeName { get; set; }
        public decimal? PcsInBale { get; set; }
        public decimal? SideInBale { get; set; }
        public decimal? MeterPLPIBaleQty { get; set; }
        public decimal? FootPLPIBaleQty { get; set; }
        public decimal? PLPIBaleNetWeight { get; set; }
        public byte? NetWeightUnit { get; set; }
        public decimal? PLPIBGrossaleWeight { get; set; }
        public byte? GrossBaleWeightUnit { get; set; }
        public string Remarks { get; set; }

        #region Display
        public int? ArticleID { get; set; }
        public int? ColorID { get; set; }

        public string GrossBaleWeightUnitName { get; set; }

        #endregion
    }
    public class MyModel
    {
        public HttpPostedFileBase MyFile { get; set; }
    }
}
