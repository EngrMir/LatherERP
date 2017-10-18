using System.Collections.Generic;
using System.Web.Mvc;
using SecurityAdministration.DAL;

namespace SecurityAdministration.BLL.ViewModels
{
    public class ScreenVM
    {
        public IEnumerable<ScreenView> Screens { get; set; }
        public ScreenView Screen { get; set; }
        public SelectList ModuleList { get; set; }
    }

    public class ScreenView
    {
        public string ScreenCode { get; set; }
        public string ModuleID { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string URL { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public string Description { get; set; }
        public string SetOn { get; set; }
        public int SetBy { get; set; }
        public string ModuleTitle{ get; set; }
    }

}