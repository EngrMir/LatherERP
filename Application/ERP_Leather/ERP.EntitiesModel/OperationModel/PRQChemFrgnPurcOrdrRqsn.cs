using System;
using ERP.EntitiesModel.BaseModel;

namespace ERP.EntitiesModel.OperationModel
{
    public class PRQChemFrgnPurcOrdrRqsn : CommonStatusInformation
    {
        public int OrederID { get; set; }
        public int RequisitionID { get; set; }
        public string RequisitionNo { get; set; }
        public string RequisitionState { get; set; }

        public string RequisitionType { get; set; }
        public string ReqRaisedOn { get; set; }
        public string ReqRaisedByID { get; set; }
        public string ReqRaisedByName { get; set; }
        public string RequisitionStatus { get; set; }
    }
}
