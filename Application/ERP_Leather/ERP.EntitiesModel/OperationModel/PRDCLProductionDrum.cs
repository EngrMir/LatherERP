using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;

namespace ERP.EntitiesModel.OperationModel
{
    public class PRDCLProductionDrum : CommonStatusInformation
    {
        public long CLProductionDrumID { get; set; }
        public long CLProductionColorID { get; set; }

        public long CrustSdulDrumID { get; set; }
        public int? MachineID { get; set; }
        public string MachineNo { get; set; }
        public int? ColorID { get; set; }
        public short? GradeID { get; set; }
        public decimal DrumArea { get; set; }
        public byte AreaUnit { get; set; }
        public decimal DrumPcs { get; set; }
        public decimal DrumSide { get; set; }
        public decimal DrumWeight { get; set; }
        public byte WeightUnit { get; set; }
        public string Remarks { get; set; }
    }
}
