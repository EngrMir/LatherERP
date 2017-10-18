using System.Collections.Generic;
using System.Linq;
using System.Text;
using SecurityAdministration.BLL.ViewModels;

namespace SecurityAdministration.DAL.Repositories
{
    public class MenuItemRepository : GenericRepository<MenuItem>
    {
        public MenuItemRepository(BLC_DEVEntities context)
            : base(context)
        {
        }

        public IEnumerable<MenuItemView> GetAll()
        {
            const string itemQuery = "SELECT M.MenuID,M.Caption,M.MenuLevel,M.ItemOrder,M.ParentID," +
                                     " M.ScreenCode,M.Description,M.IsActive,M.IsDelete,M.HasChild,FORMAT(M.SetOn,'dd-MMM-yyyy') " +
                                     " AS SetOn,M.SetBy,S.ScreenCode, S.Title AS ScreenTitle, S.ModuleID AS ScreenModuleID, Mo.Title AS ModuleTitle" +
                                     " FROM Security.MenuItems AS M" +
                                     " LEFT JOIN Security.Screens AS S" +
                                     " ON M.ScreenCode = S.ScreenCode" +
                                     " LEFT JOIN Security.Modules AS Mo" +
                                     " ON S.ModuleID = Mo.ModuleID" +
                                     " WHERE M.IsDelete = 0";


            var menuList = context.Database.SqlQuery<MenuItemView>(itemQuery);
            return menuList.ToList();
        }

        public MenuItemView GetByValue(int? id)
        {
            var itemQuery = "SELECT M.MenuID,M.Caption,M.MenuLevel,M.ItemOrder,M.ParentID," +
                                                " M.Description,M.IsActive,M.IsDelete,M.HasChild,FORMAT(M.SetOn,'dd-MMM-yyyy') " +
                                                " AS SetOn,M.SetBy,S.ScreenCode, S.Title AS ScreenTitle, MD.ModuleID AS ScreenModuleID, MD.Title AS ModuleTitle" +
                                                " FROM BLC_DEV.Security.MenuItems AS M" +
                                                " LEFT JOIN BLC_DEV.Security.Screens AS S" +
                                                " ON M.ScreenCode = S.ScreenCode" +
                                                " LEFT JOIN BLC_DEV.Security.Modules AS MD" +
                                                " ON S.ModuleID = MD.ModuleID" +
                                                " WHERE M.IsDelete = 0 AND M.MenuID = '" + id + "'" +
                                                " ORDER BY M.SetOn DESC";
            return context.Database.SqlQuery<MenuItemView>(itemQuery).FirstOrDefault();
        }

        public int IsDeleteTrue(int? id)
        {
            var query = new StringBuilder();
            query.Append("UPDATE [Security].[MenuItems]");
            query.Append(" SET [IsDelete] = 1");
            query.Append(" WHERE [MenuID] = '" + id + "'");

            return context.Database.ExecuteSqlCommand(query.ToString());
        }
    }
}