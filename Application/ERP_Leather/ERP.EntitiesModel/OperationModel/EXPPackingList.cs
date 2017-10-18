using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class EXPPackingList
    {
        public long PLID { get; set; }
        public string PLNo { get; set; }
        public string PLDate { get; set; }
        public long? CIID { get; set; }
        public string CINo { get; set; }
        public string CIDate { get; set; }
        public string BalesNo { get; set; }
        public decimal? BaleQty { get; set; }
        public decimal? TotalPcs { get; set; }
        public decimal? TotalSide { get; set; }
        public decimal? MeterCIQty { get; set; }
        public decimal? FootCIQty { get; set; }
        public decimal? PLNetWeight { get; set; }
        public byte? NetWeightUnit { get; set; }
        public decimal? PLGrossWeight { get; set; }
        public byte? GrossWeightUnit { get; set; }
        public string PLMarks { get; set; }
        public string PLNote { get; set; }
        public string RecordStatus { get; set; }
        public string RecordStatusName { get; set; }
        public virtual IList<EXPPLPI> EXPPLPIList { get; set; }
        public virtual IList<EXPPLPIItemColor> EXPPLPIItemColorList { get; set; }
        public virtual IList<EXPPLPIItemColorBale> EXPPLPIItemColorBaleList { get; set; }
    }
}
