namespace SecurityAdministration.DAL.Repositories
{
    public class ScreenOperationRepository:GenericRepository<ScreenOperation>
    {
        public ScreenOperationRepository(BLC_DEVEntities context) : base(context)
        {
        }
    }
}