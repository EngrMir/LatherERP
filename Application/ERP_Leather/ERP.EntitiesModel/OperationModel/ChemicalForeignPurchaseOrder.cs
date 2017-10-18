using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;

namespace ERP.EntitiesModel.OperationModel
{
    public class ChemicalForeignPurchaseOrder : CommonStatusInformation
    {
        public string OrderID { get; set; }
        public string OrderTo { get; set; }
        public string OrderNo { get; set; }

        public string OrderDate { get; set; }
        public string OrderType { get; set; }
        public string OrderCategory { get; set; }
        public int? SupplierID { get; set; }
        public int? LocalAgentID { get; set; }
        public int? ForeignAgentID { get; set; }
        public string OrderNote { get; set; }


        public PRQChemFrgnPurcOrdr OrderInformation { get; set; }
        public virtual ICollection<PRQChemFrgnPurcOrdrRqsn> RequisitionList { get; set; }
        public virtual ICollection<PRQChemFrgnPurcOrdrItem> RequisitionItemList { get; set; }

    }
}
