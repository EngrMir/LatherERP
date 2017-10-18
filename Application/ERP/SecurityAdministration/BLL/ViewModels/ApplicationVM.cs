using System.Collections.Generic;

namespace SecurityAdministration.BLL.ViewModels
{
    public class ApplicationVM
    {
        public IEnumerable<ApplicationView> Applications { get; set; }
        public ApplicationView Application { get; set; }
    }

    public class ApplicationView
    {
        public byte ApplicationID { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public string Description { get; set; }
        public string SetOn { get; set; }
        public int SetBy { get; set; }
        public string SetByPerson { get; set; }
    }

}