namespace SecurityAdministration.DAL.Repositories
{
    public class AccessLogRepository: GenericRepository<AccessLog>
    {
        public AccessLogRepository(BLC_DEVEntities context) : base(context)
        {
        }
    }
}