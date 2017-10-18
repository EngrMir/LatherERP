using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.AppSetupModel
{
    public class MenuItems
    {
        public int? ID { get; set; }
        public string Caption { get; set; }
        public int MenuLevel { get; set; }
        public int ItemOrder { get; set; }
        public int? ParentID { get; set; }
        public string ScreenCode { get; set; }
        public string ScreenTitle { get; set; }
        public string ModuleTitle { get; set; }
        //public bool IsActive { get; set; }
        //public bool IsDelete { get; set; }
        public string ScreenModuleID { get; set; }
        public string Description { get; set; }
        public bool HasChild { get; set; }
        public string Link { get; set; } 
        //public int SetBy { get; set; }
    }
}
