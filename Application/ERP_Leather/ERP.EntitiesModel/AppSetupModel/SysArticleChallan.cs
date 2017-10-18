using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.EntitiesModel.BaseModel;

namespace ERP.EntitiesModel.AppSetupModel
{
    public class SysArticleChallan : CommonStatusInformation
    {
        public long ArticleChallanID { get; set; }
        public string ArticleChallanNo { get; set; }
        public string ChallanNote { get; set; }
        public string PreparationDate { get; set; }
        public int? BuyerID { get; set; }
        public string BuyerName { get; set; }
        public string BuyerCode { get; set; }
        public int? ArticleID { get; set; }
        public string ArticleName { get; set; }
        public decimal? ArticleArea { get; set; }
        public byte? AreaUnit { get; set; }
        public string AreaUnitName { get; set; }
        public decimal? ArticlePcs { get; set; }
        public decimal? ArticleSide { get; set; }
        public string ArticleNote { get; set; }
        public string RivisionNo { get; set; }
        public string Remarks { get; set; }
        public List<SysArticleChallanDetail> ArticleDetailList { get; set; }
        public List<SysArticleChallanColor> ColorList { get; set; }




        public string SizeRange { get; set; }
        public byte? SizeRangeUnit { get; set; }
        public string SizeRangeUnitName { get; set; }
        public string PcsSideDescription { get; set; }
        public string GradeRange { get; set; }
        public string ThicknessRange { get; set; }
        public byte? ThicknessUnit { get; set; }
        public string ThicknessUnitName { get; set; }
        public string ThicknessAt { get; set; }
        public string RequiredPercentage { get; set; }


    }



    public class SysArticleChallanDetail : CommonStatusInformation
    {
        public long ArticleChallanDtlID { get; set; }
        public long ArticleChallanID { get; set; }
        public string SizeRange { get; set; }
        public byte? SizeRangeUnit { get; set; }
        public string SizeRangeUnitName { get; set; }
        public string PcsSideDescription { get; set; }
        public string GradeRange { get; set; }
        public string ThicknessRange { get; set; }
        public byte? ThicknessUnit { get; set; }
        public string ThicknessUnitName { get; set; }
        public string ThicknessAt { get; set; }
        public string RequiredPercentage { get; set; }
        public string Remarks { get; set; }
    }

    public class SysArticleChallanColor : CommonStatusInformation
    {
        public long ArticleChallanIDColor { get; set; }
        public long? ArticleChallanID { get; set; }
        public int? ArticleColorNo { get; set; }
        public int? ArticleColor { get; set; }
        public string ArticleColorName { get; set; }
        public decimal? ArticleColorArea { get; set; }
        public byte? ColorAreaUnit { get; set; }
        public string ColorAreaUnitName { get; set; }
        public string QuantityDescription { get; set; }
        public string RemarkDate { get; set; }
        public string Remarks { get; set; }
    }
}
