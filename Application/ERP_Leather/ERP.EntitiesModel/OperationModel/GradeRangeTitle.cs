using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class GradeRangeTitle
    {
        public int GradeRangeId { get; set; }
        public string GradeRangeName { get; set; }
        public string GradeDescription { get; set; }
        public bool IsActiveGradeRange { get; set; }
        public virtual ICollection<GradeRangeFromTo> gradeData { get; set; }
    }
}
