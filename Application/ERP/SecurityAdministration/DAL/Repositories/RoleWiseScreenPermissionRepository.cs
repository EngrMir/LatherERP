using System.Collections.Generic;
using System.Text;
using SecurityAdministration.BLL.ViewModels;

namespace SecurityAdministration.DAL.Repositories
{
    public class RoleWiseScreenPermissionRepository : GenericRepository<RoleWiseScreenPermission>
    {
        public RoleWiseScreenPermissionRepository(BLC_DEVEntities context)
            : base(context)
        {
        }

        public IEnumerable<ScreenPermissionView> Get(int? roleID = null, string moduleID = "", string screenCode = "")
        {
            var query = new StringBuilder();
            query.Append("SELECT RWSP.RoleID, R.RoleName, M.ModuleID, M.Title AS ModuleTitle, RWSP.ScreenCode, S.Title AS ScreenTitle,");
            query.Append(" CASE WHEN SUBSTRING (RWSP.AccessRight,2,1)='1' THEN 'Yes' ELSE 'No' END CanRead,");
            query.Append(" CASE WHEN SUBSTRING (RWSP.AccessRight,1,1)='1' THEN 'Yes' ELSE 'No' END CanCreate,");
            query.Append(" CASE WHEN SUBSTRING (RWSP.AccessRight,3,1)='1' THEN 'Yes' ELSE 'No' END CanUpdate,");
            query.Append(" CASE WHEN SUBSTRING (RWSP.AccessRight,4,1)='1' THEN 'Yes' ELSE 'No' END CanDelete,");
            query.Append(" FORMAT(RWSP.SetOn, 'dd-MMM-yyyy') AS SetOn,");
            query.Append(" RWSP.SetBy");
            query.Append(" FROM Security.RoleWiseScreenPermissions AS RWSP INNER JOIN Security.Roles AS R ON RWSP.RoleID = R.RoleID");
            query.Append(" INNER JOIN Security.Screens AS S ON RWSP.ScreenCode = S.ScreenCode INNER JOIN Security.Modules AS M ON S.ModuleID = M.ModuleID");
            query.Append(" WHERE 1=1");
            if (roleID != null)
            {
                query.Append(" AND RWSP.RoleID = {0}");
            }
            if (!string.IsNullOrWhiteSpace(moduleID))
            {
                query.Append(" AND S.ModuleID = {1}");
            }
            if (!string.IsNullOrWhiteSpace(screenCode))
            {
                query.Append(" AND RWSP.ScreenCode = {2}");
            }
            query.Append(" ORDER BY RWSP.SetOn DESC");

            return context.Database.SqlQuery<ScreenPermissionView>(query.ToString(), roleID, moduleID, screenCode);
        }

        public int InsertRoleWiseScreenPermission(RoleWiseScreenPermission screenPermission)
        {
            var query = new StringBuilder();
            query.Append("INSERT INTO [Security].[RoleWiseScreenPermissions] ([RoleID], [ScreenCode], [AccessRight], [SetOn], [SetBy])");
            query.Append(" VALUES ('" + screenPermission.RoleID + "'");
            query.Append(" ,'" + screenPermission.ScreenCode + "'");
            query.Append(" ,'" + screenPermission.AccessRight + "'");
            query.Append(" ,'" + screenPermission.SetOn + "'");
            query.Append(" ,'" + screenPermission.SetBy + "')");

            return context.Database.ExecuteSqlCommand(query.ToString());
        }

        public int UpdateRoleWiseScreenPermission(RoleWiseScreenPermission screenPermission)
        {
            var query = new StringBuilder();
            query.Append("UPDATE [Security].[RoleWiseScreenPermissions]");
            query.Append(" SET [AccessRight] = '" + screenPermission.AccessRight + "'");
            query.Append(" ,[SetOn] = '" + screenPermission.SetOn + "'");
            query.Append(" ,[SetBy] = '" + screenPermission.SetBy + "'");
            query.Append(" WHERE [RoleID] = '" + screenPermission.RoleID + "' AND [ScreenCode] = '" + screenPermission.ScreenCode + "'");

            return context.Database.ExecuteSqlCommand(query.ToString());
        }

        public int DeleteRoleWiseScreenPermission(int roleID, string screenCode)
        {
            var query = new StringBuilder();
            query.Append("DELETE [Security].[RoleWiseScreenPermissions]");
            query.Append(" WHERE [RoleID] = {0} AND [ScreenCode] = {1}");

            return context.Database.ExecuteSqlCommand(query.ToString(),roleID, screenCode);
        }
    }
}