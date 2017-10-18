using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.AppSetupModel;
using ERP.DatabaseAccessLayer.AppSetupGateway;

namespace ERP.DatabaseAccessLayer.AppSetupGateway
{
    public class DalMenuItems
    {
        readonly BLC_DEVEntities _dc = new BLC_DEVEntities();

        readonly IList<MenuItems> _newlist = new List<MenuItems>();
        public List<MenuItems> LoadMenu(int? id, int userId)
        {
            Int16 roleId = GetRoleId(userId);
            
            var list = (from n in _dc.vw_MenuInScreen
                        where n.RoleID==roleId
                        select new MenuItems
                        {
                            ID = n.MenuID, 
                            Caption = n.Caption,
                            ParentID = n.ParentID, 
                            ItemOrder=n.ItemOrder,
                            HasChild = true,
                            Link ="" + "/" + n.URL 
                        }).ToList().OrderBy(o=>o.ItemOrder);

            //var menuList =new List<MenuItems>();
            //var menuItem = new MenuItems();
            //foreach (var m in list)
            //{
            //    menuItem.ID = m.ID;
            //    menuItem.Caption = m.Caption;
            //    menuItem.ParentID = m.ParentID;
            //    menuItem.HasChild = m.HasChild;
            //    menuItem.Link = string.Format("~/{0}", m.Link);
            //    menuList.Add(menuItem);
            //}


            foreach (var eachlist in list.Where(eachlist => eachlist != null))
            {
                if (_dc.vw_MenuInScreen != null)
                    eachlist.HasChild = _dc.vw_MenuInScreen.Where(m => m.ParentID == eachlist.ID).ToList().Any() ? true : false;
                _newlist.Add(eachlist);
            }
            var nodes = (from n in _newlist
                         where (id.HasValue ? n.ParentID == id.Value : n.ParentID == null)
                         select n).OrderBy(m => m.ItemOrder).ToList();

            return nodes;
        }

        private Int16 GetRoleId(int userId)
        {
            return _dc.UserInRoles.Where(r => r.UserID.Equals(userId)).Select(s => s.RoleID).FirstOrDefault(); 
        }

        public object UserAccessPermissionList(int userId)
        {
            return _dc.UspGetUserPermission(userId).ToList();
        }
    }
}
