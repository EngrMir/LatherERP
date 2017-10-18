using System.Collections.Generic;
using System.Web.Mvc;

namespace SecurityAdministration.BLL.ViewModels
{
    public class RoleWiseOperationPermissionVm
    {
        public RolePermissionView RoleWiseOperationPermission { get; set; }
        public IEnumerable<RolePermissionView> RoleWiseScreenPermissions { get; set; }
        public SelectList RoleList { get; set; }
        public SelectList OperationList { get; set; }
    }

    public class RolePermissionView
    {
        public short RoleID { get; set; }
        public byte OperationID { get; set; }
        public bool HaveAccess { get; set; }
        public string SetOn { get; set; }
        public int SetBy { get; set; }
        public string RoleName { get; set; }
        public int OperationTitle { get; set; }
    }
}