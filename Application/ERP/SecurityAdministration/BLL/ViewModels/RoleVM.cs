using System;
using System.Collections.Generic;

namespace SecurityAdministration.BLL.ViewModels
{
    public class RoleVM
    {
        public IEnumerable<RoleView> Roles { get; set; }
        public RoleView Role { get; set; }               
    }

    public class RoleView
    {
        public short? RoleID { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public string SetOn { get; set; }
        public int SetBy { get; set; }
    }
}