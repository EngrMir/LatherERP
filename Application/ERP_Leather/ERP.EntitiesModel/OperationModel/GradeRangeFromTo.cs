using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class GradeRangeFromTo
    {
        public int FormatID { get; set; }

        public int GradeIDFrom { get; set; }
        public int GradeIDTo { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

    }
}
