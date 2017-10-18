using System;
using System.Collections.Generic;
using System.Linq;
using SecurityAdministration.BLL.ViewModels;
using SecurityAdministration.DAL;

namespace SecurityAdministration.BLL
{
    public static class Extensions
    {
        #region General

        public static string ToString(this DateTime dateTime)
        {
            string date = dateTime.ToString("dd-MMM-yyyy");
            return date;
        }

        #endregion

        #region Role View

        public static RoleView ToRoleView(this Role role)
        {
            return new RoleView
            {
                RoleID = role.RoleID,
                RoleName = role.RoleName,
                Description = role.Description,
                IsActive = role.IsActive,
                SetOn = role.SetOn.ToString("dd-MMM-yyyy"),
                SetBy = role.SetBy
            };
        }

        public static IEnumerable<RoleView> ToRoleView(this IEnumerable<Role> roleList)
        {
            return roleList.Select(ToRoleView).ToList();
        }

        #endregion

        #region Menu-Item View

        public static MenuItem ToMenuItem(this MenuItemView menuItemView)
        {
            return new MenuItem
            {
                MenuID = Convert.ToInt32(menuItemView.MenuID),
                Caption = menuItemView.Caption,
                MenuLevel = menuItemView.MenuLevel,
                ItemOrder = menuItemView.ItemOrder,
                ParentID = menuItemView.ParentID,
                ScreenCode = menuItemView.ScreenCode,
                Description = menuItemView.Description,
                IsActive = menuItemView.IsActive,
                IsDelete = menuItemView.IsDelete,
                HasChild = menuItemView.HasChild,
                SetOn = Convert.ToDateTime(menuItemView.SetOn),
                SetBy = menuItemView.SetBy
            };
        }

        #endregion

        #region Module View

        public static ModuleView ToModuleView(this Module module)
        {
            return new ModuleView
            {
                ModuleID = module.ModuleID,
                ApplicationID = module.ApplicationID,
                Title = module.Title,
                IsActive = module.IsActive,
                IsDelete = module.IsDelete,
                Description = module.Description,
                SetOn = module.SetOn.ToString("dd-MMM-yyyy"),
                SetBy = module.SetBy
            };
        }

        public static IEnumerable<ModuleView> ToModuleView(this IEnumerable<Module> moduleList)
        {
            return moduleList.Select(ToModuleView).ToList();
        }
        #endregion

        #region Designation View

        public static DesignationView ToDesignationView(this Designation designation)
        {
            return new DesignationView()
            {
                DesignationID = designation.DesignationID,
                Name = designation.Name,
                Description = designation.Description,
                IsActive = designation.IsActive,
                SetOn = designation.SetOn.ToString("dd-MMM-yyyy"),
                SetBy = designation.SetBy
            };
        }

        public static IEnumerable<DesignationView> ToDesignationView(this IEnumerable<Designation> designationList)
        {
            return designationList.Select(designation => new DesignationView()
            {
                DesignationID = designation.DesignationID,
                Name = designation.Name,
                Description = designation.Description,
                IsActive = designation.IsActive,
                SetOn = designation.SetOn.ToString("dd-MMM-yyyy"),
                SetBy = designation.SetBy
            }).ToList();
        }

        #endregion

        #region UserInRole

        public static UserInRoleView ToUserInRoleView(this UserInRole userInRole)
        {
            return new UserInRoleView
            {
                UserID = userInRole.UserID,
                RoleID = userInRole.RoleID,
                IsActive = userInRole.IsActive,
                SetOn = userInRole.SetOn.ToString("dd-MMM-yyyy"),
                SetBy = userInRole.SetBy
            };
        }

        public static IEnumerable<UserInRoleView> ToUserInRoleView(this IEnumerable<UserInRole> userInRoleList)
        {
            return userInRoleList.Select(ToUserInRoleView).ToList();
        }
        #endregion

        #region Role Wise Screen Permission

        public static ScreenPermissionView ToScreenPermissionView(this RoleWiseScreenPermission screenPermission)
        {
            return new ScreenPermissionView()
            {
                RoleID = screenPermission.RoleID,
                RoleName = screenPermission.Role.RoleName,
                ScreenCode = screenPermission.ScreenCode,
                ScreenTitle = screenPermission.Screen.Title,
                //AccessRight = new AccessRight
                //{
                //    CanCreate = screenPermission.AccessRight.Substring(0, 1) == "1",
                //    CanRead = screenPermission.AccessRight.Substring(1, 1) == "1",
                //    CanUpdate = screenPermission.AccessRight.Substring(2, 1) == "1",
                //    CanDelete = screenPermission.AccessRight.Substring(3, 1) == "1"
                //},
                SetOn = screenPermission.SetOn.ToString("dd-MMM-yyyy"),
                SetBy = screenPermission.SetBy
            };
        }

        public static IEnumerable<ScreenPermissionView> ToScreenPermissionView(this IEnumerable<RoleWiseScreenPermission> screenPermissionList)
        {
            return screenPermissionList.Select(ToScreenPermissionView).ToList();
        }

        #endregion

        #region Role Wise Operation Permission 


        public static RolePermissionView ToRoleOperationPermissionView(this RoleWiseOperationPermission roleOperationPermission)
        {
            return new RolePermissionView
            {
                RoleID = roleOperationPermission.RoleID,
                OperationID = roleOperationPermission.OperationID,
                HaveAccess = roleOperationPermission.HaveAccess,
                SetOn = roleOperationPermission.SetOn.ToString("dd-MMM-yyyy"),
                SetBy = roleOperationPermission.SetBy
            };
        }

        public static IEnumerable<RolePermissionView> ToRoleOperationPermissionView(this IEnumerable<RoleWiseOperationPermission> roleOperationPermission)
        {
            return roleOperationPermission.Select(ToRoleOperationPermissionView).ToList();
        }

        #endregion

    }
}