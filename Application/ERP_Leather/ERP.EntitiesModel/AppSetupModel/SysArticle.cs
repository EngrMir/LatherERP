using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.AppSetupModel
{
    public class SysArticle
    {
        public int ArticleID { get; set; }
        public string ArticleNo { get; set; }
        public string ArticleName { get; set; }
        public int? ArticleColor { get; set; }
        public string ArticleChallanNo { get; set; }
        public string ColorName { get; set; }
        public string ArticleCategory { get; set; }
        public string Remarks { get; set; }
        public string IsActive { get; set; }
        public IList<SysArticle> ArticleList = new List<SysArticle>();
    }
}
