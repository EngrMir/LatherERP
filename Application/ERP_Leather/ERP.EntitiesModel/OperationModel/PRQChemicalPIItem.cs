using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;

namespace ERP.EntitiesModel.OperationModel
{
    public class PRQChemicalPIItem : CommonStatusInformation
    {
        public long PIItemID { get; set; }
        public int PIID { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public string HSCode { get; set; }
        public decimal OrderQty { get; set; }
        public byte? OrderUnitID { get; set; }
        public string OrderUnitName { get; set; }

        public byte? PackSizeID { get; set; }
        public string PackSizeName { get; set; }
        public byte? SizeUnitID { get; set; }
        public string SizeUnitName { get; set; }
        public int PackQty { get; set; }
        public decimal PIQty { get; set; }
        public byte PIUnitID { get; set; }
        public string PIUnitName { get; set; }
        public decimal PIUnitPrice { get; set; }


        public decimal PITotalPrice { get; set; }
        public int SupplierID { get; set; }
        public int ManufacturerID { get; set; }
        public string ItemSource { get; set; }
        public string ApprovalState { get; set; }
    }
}
