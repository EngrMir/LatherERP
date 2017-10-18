using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using SecurityAdministration.BLL.ViewModels;

namespace SecurityAdministration.DAL.Repositories
{
    public class UserInRoleRepository : GenericRepository<UserInRole>
    {
        public UserInRoleRepository(BLC_DEVEntities context)
            : base(context)
        {
        }

        public IEnumerable<UserInRoleView> GetAll()
        {
            var userType = HttpContext.Current.Session["UserType"];
            var query = new StringBuilder();
            query.Append("SELECT CONCAT(u.FirstName,' ',u.MiddleName,' ',u.LastName) AS FullName,r.RoleName, ur.UserID, ur.RoleID,");
            query.Append(" ur.IsActive, CONVERT(VARCHAR(25), ur.SetOn, 106)  AS SetOn, ur.SetBy, CONCAT(uu.MiddleName,' ',uu.LastName) AS SetByPerson FROM [Security].[UserInRoles] AS ur");
            query.Append(" INNER JOIN [Security].[Users] AS u ON ur.UserID = u.UserID");
            query.Append(" INNER JOIN [Security].[Roles] AS r ON ur.RoleID = r.RoleID");
            query.Append(" INNER JOIN [Security].[Users] AS uu ON ur.SetBy = uu.UserID  Where u.IsActive='true'");
            if (Convert.ToInt16(userType) != 1)
            {
                query.Append(" AND u.UserType !=1");
            }
            return context.Database.SqlQuery<UserInRoleView>(query.ToString());
        }

        public IEnumerable<UserInRoleView> Get(int userID, int? roleID)
        {
            var query = new StringBuilder();
            query.Append("SELECT CONCAT(u.FirstName,' ',u.MiddleName,' ',u.LastName) AS FullName,r.RoleName, ur.UserID, ur.RoleID,");
            query.Append(" ur.IsActive, CONVERT(VARCHAR(25), ur.SetOn, 106) AS SetOn, ur.SetBy, CONCAT(uu.MiddleName,' ',uu.LastName) AS SetByPerson FROM [Security].[UserInRoles] AS ur");
            query.Append(" INNER JOIN [Security].[Users] AS u ON ur.UserID = u.UserID");
            query.Append(" INNER JOIN [Security].[Roles] AS r ON ur.RoleID = r.RoleID");
            query.Append(" INNER JOIN [Security].[Users] AS uu ON ur.SetBy = uu.UserID");
            query.Append(" WHERE 1=1");

            if (userID > 0)
            {
                query.Append(" AND ur.UserID = " + userID + "");
            }
            if (roleID != null)
            {
                query.Append(" AND ur.RoleID = " + roleID + "");
            }
            return context.Database.SqlQuery<UserInRoleView>(query.ToString());
        }

        public int InsertUserInRole(UserInRole userInRole)
        {
            var query = new StringBuilder();
            query.Append("INSERT INTO [Security].[UserInRoles] ([UserID], [RoleID], [IsActive], [SetOn], [SetBy]) VALUES (");
            query.Append("'" + userInRole.UserID + "'");
            query.Append(", '" + userInRole.RoleID + "'");
            query.Append(", '" + userInRole.IsActive + "'");
            query.Append(", '" + userInRole.SetOn.ToString("yyyy-MM-dd") + "'");
            query.Append(", '" + userInRole.SetBy + "')");

            return context.Database.ExecuteSqlCommand(query.ToString());
        }

        public int UpdateUserInRole(UserInRole userInRole)
        {
            var query = new StringBuilder();
            query.Append("UPDATE [Security].[UserInRoles]");
            query.Append(" SET [IsActive] = '" + userInRole.IsActive + "'");
            query.Append(" ,[SetOn] = '" + userInRole.SetOn.ToString("yyyy-MM-dd") + "'");
            query.Append(" ,[SetBy] = '" + userInRole.SetBy + "'");
            query.Append(" WHERE [UserID] = '" + userInRole.UserID + "' AND [RoleID] = '" + userInRole.RoleID + "'");

            return context.Database.ExecuteSqlCommand(query.ToString());
        }

        public int DeleteUserInRole(int userId, int roleId)
        {
            var query = new StringBuilder();
            query.Append("DELETE FROM [Security].[UserInRoles] WHERE [UserID]=");
            query.Append("'" + userId + "' AND [RoleID]=");
            query.Append("'" + roleId + "'");
            return context.Database.ExecuteSqlCommand(query.ToString());
        }

        public IEnumerable<UserInRoleView> CheckUser(int userID)
        {
            var query = new StringBuilder();

            query.Append("SELECT [UserID], [RoleID], [IsActive], [SetOn], [SetBy] FROM [Security].[UserInRoles]");
            query.Append(" WHERE 1=1");

            if (userID > 0)
            {
                query.Append(" AND [UserID] = {0}");
            }
            return context.Database.SqlQuery<UserInRoleView>(query.ToString(), userID);
        }

        public int GetRoleId(int userId)
        {
           return  context.UserInRoles.Where(r => r.UserID == userId).Select(s => s.RoleID).FirstOrDefault();

        }

    }
}