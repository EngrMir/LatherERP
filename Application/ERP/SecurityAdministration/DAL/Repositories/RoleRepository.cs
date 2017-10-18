using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using SecurityAdministration.BLL.ViewModels;

namespace SecurityAdministration.DAL.Repositories
{
    public class RoleRepository:GenericRepository<Role>
    {
        public RoleRepository(BLC_DEVEntities context) : base(context)
        {
        }

        public int IsDeleteTrue(int? id)
        {
            var query = new StringBuilder();
            query.Append("UPDATE [Security].[Roles]");
            query.Append(" SET [IsDelete] = 1");
            query.Append(" WHERE [RoleID] = '" + id + "'");

            return context.Database.ExecuteSqlCommand(query.ToString());
        }
        public IEnumerable<RoleView> GetByValue(int id)
        {
            var query = "SELECT * FROM [Security].[Designations] WHERE DesinationID ='" + id + "'";
            return context.Database.SqlQuery<RoleView>(query, id);
        }

        public IEnumerable GetRoleList()
        {

            var query = new StringBuilder();
            var userType = HttpContext.Current.Session["UserType"];
            query.Append(Convert.ToInt16(userType) != 1
                ? "SELECT RoleID, RoleName FROM  Security.Roles where RoleId !=1 "
                : "SELECT RoleID, RoleName FROM  Security.Roles");

            return context.Database.SqlQuery<RoleView>(query.ToString());
        }
    }
}