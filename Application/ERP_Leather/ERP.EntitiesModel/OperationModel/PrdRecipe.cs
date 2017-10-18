using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
   public class PrdRecipe
    {
        public int RecipeID { get; set; }
        public string RecipeName { get; set; }
        public string RecipeNo { get; set; }
        public string ArticleNo { get; set; }
        public int? ArticleID { get; set; }
        public string ArticleName { get; set; }
        public int? ArticleColor { get; set; }
        public string  ArticleColorName { get; set; }
        public string ArticleChallanNo { get; set; }
        public int? RecipeFor { get; set; }
        public string RecipeProcessName { get; set; }
       
        public string CalculationBase { get; set; }
        public decimal BaseQuantity { get; set; }
        public byte BaseUnit { get; set; }
        public string BaseUnitName { get; set; }
        public string UnitName { get; set; }
        public string RivisionNo { get; set; }
        public DateTime? RivisionDate { get; set; }
        public bool isNewRevision { get; set; }
        public string Remarks { get; set; }
        public string RecordStatus { get; set; }
        public DateTime? SetOn { get; set; }
        public int? SetBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public string IPAddress { get; set; }

        public  ICollection<PrdRecipeItem> RecipeItems { get; set; }
    }
}
