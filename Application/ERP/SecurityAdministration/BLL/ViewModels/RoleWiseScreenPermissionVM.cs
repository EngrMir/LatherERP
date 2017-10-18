using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SecurityAdministration.BLL.ViewModels
{
    public class RoleWiseScreenPermissionVM
    {
        public ScreenPermissionView RoleWiseScreenPermission { get; set; }
        public IEnumerable<ScreenPermissionView> RoleWiseScreenPermissions { get; set; }

        public SelectList RoleList { get; set; }
        public SelectList ModuleList { get; set; }
    }

    public class ScreenPermissionView
    {
        public Int16 RoleID { get; set; }
        public string RoleName { get; set; }
        public string ModuleID { get; set; }
        public string ModuleTitle { get; set; }
        public string ScreenCode { get; set; }
        public string ScreenTitle { get; set; }
        public string CanRead { get; set; }
        public string CanCreate { get; set; }
        public string CanUpdate { get; set; }
        public string CanDelete { get; set; }
        public string SetOn { get; set; }
        public int SetBy { get; set; }
    }

    //public class AccessRight
    //{
    //    public bool CanCreate { get; set; }
    //    public bool CanRead { get; set; }
    //    public bool CanUpdate { get; set; }
    //    public bool CanDelete { get; set; }
    //}
}