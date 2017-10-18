using System.Collections.Generic;
using System.Linq;
using System.Text;
using SecurityAdministration.BLL.ViewModels;

namespace SecurityAdministration.DAL.Repositories
{
    public class ApplicationRepository : GenericRepository<Application>
    {
        public ApplicationRepository(BLC_DEVEntities context)
            : base(context)
        {
        }

        public IEnumerable<ApplicationView> GetAll()
        {
            const string itemQuery = "SELECT s.ApplicationID,s.Title,s.IsActive, s.Description, FORMAT(s.SetOn,'dd-MMM-yyyy') as SetOn,s.SetBy,  CONCAT(u.MiddleName,' ',u.LastName)" +
                " AS SetByPerson FROM [Security].[Applications] AS s INNER JOIN [Security].[Users] AS u ON s.SetBy = u.UserID  where s.IsDelete = 0";

            var applicationList = context.Database.SqlQuery<ApplicationView>(itemQuery);
            return applicationList.ToList();
        }

        public ApplicationView GetByValue(byte? id)
        {
            var itemQuery = "SELECT s.ApplicationID, s.Title, s.IsActive, s.Description, FORMAT(s.SetOn,'dd-MMM-yyyy') as SetOn, s.SetBy,  CONCAT(u.MiddleName,' ',u.LastName) " +
                               " AS SetByPerson FROM [Security].[Applications] AS s INNER JOIN [Security].[Users] AS u ON s.SetBy = u.UserID WHERE s.ApplicationID = '" + id + "' AND s.IsDelete = 0";

            return context.Database.SqlQuery<ApplicationView>(itemQuery).FirstOrDefault();
        }

        public int IsDeleteTrue(byte? id)
        {
            var query = new StringBuilder();
            query.Append("UPDATE [Security].[Applications]");
            query.Append(" SET [IsDelete] = 1");
            query.Append(" WHERE [ApplicationID] = '" + id + "'");

            return context.Database.ExecuteSqlCommand(query.ToString());
        }

    }
}