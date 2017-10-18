using System.Collections.Generic;

namespace SecurityAdministration.BLL.ViewModels
{
    public class DesignationVM
    {
        public IEnumerable<DesignationView> Designations { get; set; }
        public DesignationView Designation { get; set; }
    }

    public class DesignationView
    {
        public byte? DesignationID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public string SetOn { get; set; }
        public int SetBy { get; set; }
    }

}