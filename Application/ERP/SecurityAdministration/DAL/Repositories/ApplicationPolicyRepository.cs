namespace SecurityAdministration.DAL.Repositories
{
    public class ApplicationPolicyRepository:GenericRepository<ApplicationPolicy>
    {
        public ApplicationPolicyRepository(BLC_DEVEntities context) : base(context)
        {
        }
    }
}