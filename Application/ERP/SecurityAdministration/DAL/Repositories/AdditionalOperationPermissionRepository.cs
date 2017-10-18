namespace SecurityAdministration.DAL.Repositories
{
    public class AdditionalOperationPermissionRepository:GenericRepository<AdditionalOperationPermission>
    {
        public AdditionalOperationPermissionRepository(BLC_DEVEntities context) : base(context)
        {
        }
    }
}