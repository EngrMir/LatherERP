using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.AppSetupModel
{
    public class SysMachine
    {
        public int MachineID { get; set; }
        public string MachineNo { get; set; }
        public string MachineName { get; set; }
        public string MachineType { get; set; }
        public string MachineTypeName { get; set; }
        public string MachineCategory { get; set; }
        public string MachineCategoryName { get; set; }
        public decimal? MachineCapacity { get; set; }
        public byte? CapacityUnit { get; set; }
        public string UnitName { get; set; }
        public string InstallationDate { get; set; }
        public string ExpiaryDate { get; set; }
        public string Remarks { get; set; }
        public string IsActive { get; set; }
    }
}
