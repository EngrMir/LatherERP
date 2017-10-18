namespace SecurityAdministration.DAL.Repositories
{
    public class AdditionalScreenPermissionRepository:GenericRepository<AdditionalScreenPermission>
    {
        public AdditionalScreenPermissionRepository(BLC_DEVEntities context) : base(context)
        {
        }
    }
}