using System.Collections.Generic;
using System.Linq;
using System.Text;
using SecurityAdministration.BLL.ViewModels;

namespace SecurityAdministration.DAL.Repositories
{
    public class ModuleRepository:GenericRepository<Module>
    {
        public ModuleRepository(BLC_DEVEntities context) : base(context)
        {
        }

        public IEnumerable<ModuleView> GetByValue(string moduleID)
        {
            var query = new StringBuilder();

            query.Append("SELECT m.ModuleID, m.ApplicationID, m.Title, m.IsActive, m.Description,");
            query.Append(" FORMAT(m.SetOn, 'dd-MMM-yyyy') AS SetOn, m.SetBy, CONCAT(u.MiddleName,' ',u.LastName) AS SetByPerson, ap.Title AS ApplicationName");
            query.Append(" FROM [Security].[Modules] AS m INNER JOIN [Security].[Users] u ON u.UserID = m.SetBy");
            query.Append(" LEFT JOIN [Security].[Applications] ap ON ap.ApplicationID = m.ApplicationID");
            query.Append(" WHERE ModuleID = " + moduleID);
            return context.Database.SqlQuery<ModuleView>(query.ToString(), moduleID);
        }

        public IEnumerable<ModuleView> GetAll()
        {
            const string query = "SELECT m.ModuleID, m.Title, m.Description ,m.IsActive, m.SetBy,FORMAT(m.SetOn, 'dd-MMM-yyyy') AS SetOn," +
                                 " CONCAT(u.MiddleName,' ',u.LastName) AS SetByPerson," +
                                 " ap.ApplicationID, ap.Title AS ApplicationName FROM [Security].[Modules] m" +
                                 " left join [Security].[Applications] ap on m.ApplicationID = ap.ApplicationID " +
                                 " INNER JOIN [Security].[Users] u ON u.UserID = m.SetBy" +
                                 " where M.IsDelete=0 ORDER BY m.ModuleID DESC";
            return context.Database.SqlQuery<ModuleView>(query).ToList();
        }

        public int IsDeleteTrue(string id)
        {
            var query = new StringBuilder();
            query.Append("UPDATE [Security].[Modules]");
            query.Append(" SET [IsDelete] = 1");
            query.Append(" WHERE [ModuleID] = '" + id + "'");

            return context.Database.ExecuteSqlCommand(query.ToString()); 
        }

        public string GetLastModuleID()
        {
            return context.Modules.Max(m => m.ModuleID);
        }
    }
}