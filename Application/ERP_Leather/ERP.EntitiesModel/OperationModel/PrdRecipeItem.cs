using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
   public class PrdRecipeItem
    {
        public long RecipeItemID { get; set; }
        public int? RecipeID { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public int? SafetyStock { get; set; }
        public decimal RequiredQty { get; set; }
        public byte UnitID { get; set; }
        public string UnitName { get; set; }
        public DateTime? SetOn { get; set; }
        public int? SetBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public string IPAddress { get; set; }
    }
}
