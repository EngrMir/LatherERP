using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class PrdYearMonthCrustScheduleDrum
    {
        public long CrustSdulDrumID { get; set; }
        public long? TransectionID { get; set; }
        public int? MachineID { get; set; }
        public string MachineNo { get; set; }
        public string BatchNo { get; set; }
        public long? SdulItemColorID { get; set; }
        public long? ScheduleDateID { get; set; }
        public long? ScheduleItemID { get; set; }
        public int? ColorID { get; set; }
        public string ColorName { get; set; }
        public short? GradeID { get; set; }
        public string GradeName { get; set; }
        public decimal? ProductionPcs { get; set; }
        public decimal? ProductionSide { get; set; }
        public decimal? ProductionArea { get; set; }
        public decimal? DrumPcs { get; set; }
        public decimal? DrumSide { get; set; }
        public decimal? DrumArea { get; set; }
        public byte? AreaUnit { get; set; }
        public string AreaUnitName { get; set; }
        public decimal? DrumWeight { get; set; }
        public byte? WeightUnit { get; set; }
        public string WeightUnitName { get; set; }
        public string Remarks { get; set; }

        #region Badhon
        public long? CLProdChemicalID { get; set; }
        public long CLProductionDrumID { get; set; }
        public string GradeRange { get; set; }
        #endregion
    }
}
