using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SecurityAdministration.BLL.ViewModels
{
    public class MenuVM
    {
        public IEnumerable<MenuView> Menus { get; set; } 
    }

    public class MenuView
    {
        public byte MenuID { get; set; }
        public string Caption { get; set; }
        public byte MenuLevel { get; set; }
        public byte ItemOrder { get; set; }
        public byte? ParentID { get; set; }
        public string ScreenCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public bool HasChild { get; set; }
        public string URL { get; set; }
    }
}