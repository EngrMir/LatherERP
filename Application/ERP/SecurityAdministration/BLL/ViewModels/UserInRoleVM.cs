using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SecurityAdministration.BLL.ViewModels
{
    public class UserInRoleVM
    {
        public IEnumerable<UserInRoleView> UserInRoles { get; set; }
        public UserInRoleView UserInRole { get; set; }
        public SelectList UserList { get; set; }
        public SelectList RoleList { get; set; }
    }

    public class UserInRoleView
    {
        public string FullName { get; set; }
        public string RoleName { get; set; }
        public int UserID { get; set; }
        public short RoleID { get; set; }
        public bool IsActive { get; set; }
        public string SetOn { get; set; }
        public int SetBy { get; set; }
        public string SetByPerson { get; set; }
    }
}