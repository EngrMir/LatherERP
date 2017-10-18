using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

using SecurityAdministration.BLL.ViewModels;

namespace SecurityAdministration.DAL.Repositories
{
    public class UserRepository : GenericRepository<User>
    {
        public UserRepository(BLC_DEVEntities context)
            : base(context)
        {
        }

        public IEnumerable<UserView> GetUserInfo(int userID, string loginID = null)
        {
            var query = new StringBuilder();
            var userType = HttpContext.Current.Session["UserType"];

            query.Append("SELECT U.UserID, U.UserCode, U.UserType,U.Title, U.FirstName, U.MiddleName, U.LastName,");
            query.Append(" U.DesignationID, U.Email, U.Phone, U.Mobile, U.IsActive, U.SupervisorUserID, U.Description,");
            query.Append(" FORMAT(U.SetOn, 'dd-MMM-yyyy') AS SetOn, U.SetBy,");
            query.Append(" D.Name AS DesignationTitle,");
            query.Append(" UCI.LoginID, UCI.Password, FORMAT(UCI.LastPasswordChangedDate, 'dd-MMM-yyyy') AS LastPasswordChangedDate, UCI.IsPasswordAccepted, UCI.IsLocked,");
            query.Append(" FORMAT(UCI.SetOn, 'dd-MMM-yyyy') AS UCI_SetOn, UCI.SetBy AS UCI_SetBy");
            query.Append(" FROM Security.Users AS U LEFT JOIN");
            query.Append(" Security.Designations AS D ON U.DesignationID = D.DesignationID LEFT JOIN");
            query.Append(" Security.UserCredentialInformation AS UCI ON U.UserID = UCI.UserID");
            query.Append(" WHERE U.IsDelete = 0");

            if (Convert.ToInt16(userType) != 1)
            {
                query.Append(" AND U.UserType !=1");
            }

            if (userID != 0)
            {
                query.Append(" AND U.UserID = {0}");
            }

            if (!string.IsNullOrWhiteSpace(loginID))
            {
                query.Append(" AND UCI.LoginID = {1}");
            }

            query.Append(" ORDER BY U.SetOn DESC");

            return context.Database.SqlQuery<UserView>(query.ToString(), userID, loginID);
        }

        public int InsertUserGeneralInfo(User user)
        {
            var query = new StringBuilder();
            query.Append("INSERT INTO [Security].[Users] ([UserID], [UserCode], [UserType], [Title], [FirstName], [MiddleName], [LastName],");
            query.Append(" [DesignationID], [Email], [Phone], [Mobile], [IsActive], [SupervisorUserID], [Description], [SetOn], [SetBy])");
            query.Append(" VALUES ('" + user.UserID + "'");
            query.Append(" ,'" + user.UserCode + "'");
            query.Append(" ,'" + 2 + "'");
            query.Append(" ,'" + user.Title + "'");
            query.Append(" ,'" + user.FirstName + "'");
            query.Append(" ,'" + user.MiddleName + "'");
            query.Append(" ,'" + user.LastName + "'");
            query.Append(" ,'" + user.DesignationID + "'");
            query.Append(" ,'" + user.Email + "'");
            query.Append(" ,'" + user.Phone + "'");
            query.Append(" ,'" + user.Mobile + "'");
            query.Append(" ,'" + user.IsActive + "'");
            query.Append(" ,'" + user.SupervisorUserID + "'");
            query.Append(" ,'" + user.Description + "'");
            query.Append(" ,'" + user.SetOn + "'");
            query.Append(" ,'" + user.SetBy + "')");

            return context.Database.ExecuteSqlCommand(query.ToString());
        }

        public int UpdateUserGeneralInfo(User user)
        {
            var query = new StringBuilder();
            query.Append("UPDATE [Security].[Users]");
            query.Append(" SET [UserID] = '" + user.UserID + "'");
            query.Append(" ,[UserCode] = '" + user.UserCode + "'");
            query.Append(" ,[UserType] = '" + user.UserType + "'");
            query.Append(" ,[Title] = '" + user.Title + "'");
            query.Append(" ,[FirstName] = '" + user.FirstName + "'");
            query.Append(" ,[MiddleName] = '" + user.MiddleName + "'");
            query.Append(" ,[LastName] = '" + user.LastName + "'");
            query.Append(" ,[DesignationID] = '" + user.DesignationID + "'");
            query.Append(" ,[Email] = '" + user.Email + "'");
            query.Append(" ,[Phone] = '" + user.Phone + "'");
            query.Append(" ,[Mobile] = '" + user.Mobile + "'");
            query.Append(" ,[IsActive] = '" + user.IsActive + "'");
            query.Append(" ,[SupervisorUserID] = '" + user.SupervisorUserID + "'");
            query.Append(" ,[Description] = '" + user.Description + "'");
            query.Append(" ,[SetOn] = '" + user.SetOn + "'");
            query.Append(" ,[SetBy] = '" + user.SetBy + "'");
            query.Append(" [UserCode] = '" + user.UserID + "'");

            return context.Database.ExecuteSqlCommand(query.ToString());
        }

        public int InActiveUserInformation(int id)
        {
            var query = new StringBuilder();
            query.Append("UPDATE [Security].[Users]");
            query.Append(" SET [IsDelete] = '" + 1 + "'");
            query.Append(" ,[IsActive] = '" + 0 + "'");
            query.Append(" WHERE [UserID] = '" + id + "'");

            return context.Database.ExecuteSqlCommand(query.ToString());
        }

        public int GetLastInsertID()
        {
            return context.Users.Max(s => s.UserID);

        }

        public string GetLastUserCodeID()
        {
            return context.Users.Max(s => s.UserCode);
        }

        public IEnumerable<UserView> GetAllSupervisor()
        {
            var userId = HttpContext.Current.Session["UserId"];

            var query = new StringBuilder();
            query.Append("SELECT U.UserID, U.Title, U.FirstName, U.MiddleName, U.LastName, D.Name AS DesignationTitle");
            query.Append(" FROM Security.Users AS U LEFT JOIN");
            query.Append(" Security.Designations AS D ON U.DesignationID = D.DesignationID");
            query.Append(" WHERE U.IsDelete = 'False' AND U.UserType !=1");
            //query.Append(" WHERE U.IsDelete = 'False' AND U.UserType !=1 AND U.UserID != '" + userId + "'");
            query.Append(" ORDER BY U.SetOn DESC");

            return context.Database.SqlQuery<UserView>(query.ToString());
        }

        public int PasswordChange(int userId, string password)
        {
            string query = "UPDATE [Security].[UserCredentialInformation]" +
                           " SET [Password] = '" + password + "', [IsPasswordAccepted] = 'True' WHERE [UserID] = '" + userId + "'";

            return context.Database.ExecuteSqlCommand(query);
        }

        public IEnumerable<SelectListItem> GetUserList()
        {
            var query = new StringBuilder();
            var userType = HttpContext.Current.Session["UserType"];

            query.Append("SELECT U.UserID, U.FirstName, U.MiddleName, U.LastName");
            query.Append(" FROM Security.Users AS U");
            query.Append(" WHERE U.IsDelete = 'False'");
            if (Convert.ToInt16(userType) != 1)
            {
                query.Append(" AND U.UserType !=1");
            }
            query.Append(" ORDER BY U.SetOn DESC");
          
            var lst = context.Database.SqlQuery<UserView>(query.ToString()).ToList();
            var userList = lst.Select(m => new SelectListItem()
            {
                Text = m.FirstName+' '+m.MiddleName+' '+m.LastName, 
                Value = m.UserID.ToString()
            });

            return userList;
        }
    }
}