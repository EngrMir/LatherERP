using System.Collections.Generic;
using System.Text;
using SecurityAdministration.BLL.ViewModels;

namespace SecurityAdministration.DAL.Repositories
{
    public class RoleWiseOperationPermissionRepository:GenericRepository<RoleWiseOperationPermission>
    {
        public RoleWiseOperationPermissionRepository(BLC_DEVEntities context) : base(context)
        {
        }

        public IEnumerable<RolePermissionView> GetAll()
        {
            var query = new StringBuilder();
            query.Append("SELECT RWOP.RoleID, R.RoleName, SO.OperationID, SO.OperationTitle, RWOP.HaveAccess,");
            query.Append(" FORMAT(RWOP.SetOn, 'dd-MMM-yyyy') AS SetOn,");
            query.Append(" RWOP.SetBy");
            query.Append(" FROM Security.RoleWiseOperationPermissions AS RWOP INNER JOIN Security.Roles AS R ON RWOP.RoleID = R.RoleID");
            query.Append(" INNER JOIN Security.ScreenOperations AS SO ON RWOP.OperationID = SO.OperationID");
            query.Append(" ORDER BY RWOP.SetOn DESC");

            return context.Database.SqlQuery<RolePermissionView>(query.ToString());
        }

        public IEnumerable<RolePermissionView> GetByValue(int? roleID, byte? operationID = null)
        {
            var query = new StringBuilder();
            query.Append("SELECT RWOP.RoleID, R.RoleName, SO.OperationID, SO.OperationTitle, RWOP.HaveAccess,");
            query.Append(" FORMAT(RWOP.SetOn, 'dd-MMM-yyyy') AS SetOn,");
            query.Append(" RWOP.SetBy");
            query.Append(" FROM Security.RoleWiseOperationPermissions AS RWOP INNER JOIN Security.Roles AS R ON RWOP.RoleID = R.RoleID");
            query.Append(" INNER JOIN Security.ScreenOperations AS SO ON RWOP.OperationID = SO.OperationID");
            query.Append(" WHERE 1=1");
            if (roleID != null)
            {
                query.Append(" AND RWOP.RoleID = {0}");
            }
            if (operationID != null)
            {
                query.Append(" AND RWOP.OperationID = {1}");
            }

            query.Append(" ORDER BY RWOP.SetOn DESC");

            return context.Database.SqlQuery<RolePermissionView>(query.ToString(), roleID, operationID);
        }

        public int UpdateRoleWiseOperationPermission(RoleWiseOperationPermission rolewiseoperationpermission)
        {
            var query = new StringBuilder();
            query.Append("UPDATE [Security].[RoleWiseOperationPermissions]");
            query.Append(" SET [HaveAccess] = '" + rolewiseoperationpermission.HaveAccess + "'");
            query.Append(" ,[SetOn] = '" + rolewiseoperationpermission.SetOn + "'");
            query.Append(" ,[SetBy] = '" + rolewiseoperationpermission.SetBy + "'");
            query.Append(" WHERE [RoleID] = '" + rolewiseoperationpermission.RoleID + "' AND [OperationID] = '" + rolewiseoperationpermission.OperationID + "'");

            return context.Database.ExecuteSqlCommand(query.ToString());
        }

        public int DeleteRoleOperationPermission(int? roleID = null, byte? operationID = null)
        {
            var query = new StringBuilder();
            query.Append("DELETE [Security].[RoleWiseOperationPermissions]");
            query.Append(" WHERE [RoleID] = {0} AND [OperationID] = {1}");

            return context.Database.ExecuteSqlCommand(query.ToString(), roleID, operationID);
        }


    }
}