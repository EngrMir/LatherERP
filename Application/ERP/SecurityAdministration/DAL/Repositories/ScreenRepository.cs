using System.Collections.Generic;
using System.Linq;
using System.Text;
using SecurityAdministration.BLL.ViewModels;

namespace SecurityAdministration.DAL.Repositories
{
    public class ScreenRepository : GenericRepository<Screen>
    {
        public ScreenRepository(BLC_DEVEntities context)
            : base(context)
        {
        }

        public IEnumerable<ScreenView> GetAll()
        {
           const string itemQuery = "SELECT s.ScreenCode ,s.ModuleID ,s.Title, s.Type ,s.URL," +
                                    " s.IsActive,s.Description,FORMAT(s.SetOn,'dd-MMM-yyyy') as SetOn,s.SetBy, m.Title as ModuleTitle, s.IsDelete" +
                                    " FROM Security.Screens as s" +
                                    " left join Security.Modules as m" +
                                    " on s.ModuleID = m.ModuleID where s.IsDelete = 0";

            var screenList = context.Database.SqlQuery<ScreenView>(itemQuery);
            return screenList.ToList();
        }

        public ScreenView GetByValue(string id)
        {
            string itemQuery = "SELECT s.ScreenCode ,s.ModuleID ,s.Title, s.Type ,s.URL," +
                         " s.IsActive,s.Description,FORMAT(s.SetOn,'dd-MMM-yyyy') as SetOn,s.SetBy, m.Title as ModuleTitle" +
                         " FROM Security.Screens as s" +
                         " left join Security.Modules as m" +
                         " on s.ModuleID = m.ModuleID" +
                         " WHERE s.ScreenCode = '" + id + "' AND s.IsDelete = 0";

            return context.Database.SqlQuery<ScreenView>(itemQuery).FirstOrDefault();
        }

        public int IsDeleteTrue(string id)
        {
            var query = new StringBuilder();
            query.Append("UPDATE [Security].[Screens]");
            query.Append(" SET [IsDelete] = 1");
            query.Append(" WHERE [ScreenCode] = '" + id + "'");

            return context.Database.ExecuteSqlCommand(query.ToString());
        }

        public string LastScreenCode()
        {
            return context.Screens.Max(s => s.ScreenCode);
        }
    }
}