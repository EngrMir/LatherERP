using System;
using System.Collections.Generic;
using ERP.EntitiesModel.BaseModel;

namespace ERP.EntitiesModel.OperationModel
{
    public class PrqPreGradeSelection : CommonStatusInformation
    {
        public long SelectionID { get; set; }
        public int SupplierID { get; set; }
        public long PurchaseID { get; set; }
        public byte ItemTypeID { get; set; } 
        public string SelectionType { get; set; }
        public int SupplierAddressID { get; set; }
        public byte SelectionStore { get; set; }
        public DateTime? SelectionDate { get; set; }
        public string Remarks { get; set; }
        public int? SelectedBy { get; set; }
        public string SelectionComments { get; set; }
        public int? CheckedBy { get; set; }
        public string CheckComments { get; set; }
        public int? ApprovedBy { get; set; }
        public string ApproveComments { get; set; }





        public virtual ICollection<PrqPreGradeSelectedData> GradeItemListItemList { get; set; }
        //public virtual ICollection<PrqPreGradeChallan> GradeChallanList { get; set; }
    }
}
