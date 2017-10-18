using System;
using ERP.EntitiesModel.BaseModel;

namespace ERP.EntitiesModel.OperationModel
{
    public class PrqPreGradeSelectedData : CommonStatusInformation
    {
        public long SelectionID { get; set; }
        //public long ChallanItemID { get; set; }
        //public long ChallanID { get; set; }
        public short GradeID { get; set; }
        public string GradeName { get; set; }
        public decimal? GradeQty { get; set; }
        public decimal? GradeSide { get; set; }
        public decimal? GradeArea { get; set; }
        public byte UnitID { get; set; }
        public string UnitName { get; set; }
        public string SelectionDate { get; set; }
        public string SelectedBy { get; set; }
        public string SelectionComments { get; set; }
        public Nullable<int> CheckedBy { get; set; }
        public string CheckComments { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public string ApproveComments { get; set; }

    }
}
