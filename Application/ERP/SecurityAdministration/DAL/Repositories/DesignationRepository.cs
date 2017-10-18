using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Text;
using SecurityAdministration.BLL.ViewModels;

namespace SecurityAdministration.DAL.Repositories
{
    public class DesignationRepository : GenericRepository<Designation>
    {
        public DesignationRepository(BLC_DEVEntities context) : base(context)
        {

        }

        public int InactivateDesignation()
        {
            return context.Database.ExecuteSqlCommand("UPDATE Designation SET IsActive = 0");
        }

        public int IsDeleteTrue(byte? id)
        {
            var query = new StringBuilder();
            query.Append("UPDATE [Security].[Designations]");
            query.Append(" SET [IsDelete] = 1");
            query.Append(" WHERE [DesignationID] = '" + id + "'");

            return context.Database.ExecuteSqlCommand(query.ToString());
        }

        public IEnumerable<Designation> GetByValue(int id)
        {
            var query = "SELECT * FROM [Security].[Designations] WHERE DesignationID ='" + id + "'";
            return context.Database.SqlQuery<Designation>(query, id);
        }
    }
}