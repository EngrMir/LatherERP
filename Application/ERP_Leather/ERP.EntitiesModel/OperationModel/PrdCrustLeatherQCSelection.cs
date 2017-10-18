using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class PrdCrustLeatherQCSelection
    {
        public long CLQCSelectionID { get; set; }
        public long? CLQCItemID { get; set; }
        public string CLSelectItemNo { get; set; }
        public long? CrustLeatherQCRefID { get; set; }
        public long? CrustLeatherQCID { get; set; }
        public long? CLProductionID { get; set; }
        public string ProductionNo { get; set; }
        public long? ScheduleItemID { get; set; }
        public string ScheduleProductionNo { get; set; }
        public short? GradeID { get; set; }
        public decimal? ProductionPcs { get; set; }
        public decimal? ProductionSide { get; set; }
        public decimal? ProductionArea { get; set; }
        public byte? ProductionAreaUnit { get; set; }
        public decimal? QCPassPcs { get; set; }
        public decimal? QCPassSide { get; set; }
        public decimal? QCPassArea { get; set; }
        public byte? QCPassAreaUnit { get; set; }
        public string QCPassAreaUnitName { get; set; }
        public string QCPassRemarks { get; set; }
        public decimal? QCFailPcs { get; set; }
        public decimal? QCFailSide { get; set; }
        public decimal? QCFailArea { get; set; }
        public byte? QCFailAreaUnit { get; set; }
        public string QCFailAreaUnitName { get; set; }
        public string QCFailRemarks { get; set; }
        public string CrustQCLabel { get; set; }

        #region MyRegion

        public string GradeName { get; set; }
        public string GradeRange { get; set; }
        public string ShowGradeRange { get; set; }

        #endregion
    }
}
