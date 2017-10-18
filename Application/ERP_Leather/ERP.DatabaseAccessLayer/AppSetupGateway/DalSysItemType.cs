using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;

namespace ERP.DatabaseAccessLayer.AppSetupGateway
{
    public class DalSysItemType
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg = null;
        public long ItemTypeID = 0;
        public DalSysItemType()
        {
            _context = new BLC_DEVEntities();
            _vmMsg = new ValidationMsg();
        }

        public ValidationMsg  Save(SysItemType objSysItemType, int userID)
        {
            try
            {
                var tblSysItemType = new Sys_ItemType
                {
                    ItemTypeID = objSysItemType.ItemTypeID,
                    ItemTypeName = objSysItemType.ItemTypeName,
                    ItemTypeCategory = objSysItemType.ItemTypeCategory,
                    IsActive = objSysItemType.IsActive == "Active",
                    IsDelete = false,
                    SetOn = DateTime.Now,
                    SetBy = userID,
                    ModifiedOn = DateTime.Now,
                    ModifiedBy = userID,
                    IPAddress = string.Empty
                };

                _context.Sys_ItemType.Add(tblSysItemType);
                _context.SaveChanges();
                ItemTypeID = tblSysItemType.ItemTypeID;

                _vmMsg.Type = Enums.MessageType.Success;
                _vmMsg.Msg = "Saved Successfully.";
            }
            catch (Exception ex)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to save.";
            }

            return _vmMsg;
        }

        public ValidationMsg Update(SysItemType objSysItemType, int userID)
        {
            try
            {
                var itemType = _context.Sys_ItemType.FirstOrDefault(s => s.ItemTypeID == objSysItemType.ItemTypeID);
                if (itemType != null)
                {
                    //itemType.ItemTypeID = objSysItemType.ItemTypeID;
                    itemType.ItemTypeName = objSysItemType.ItemTypeName;
                    itemType.ItemTypeCategory = objSysItemType.ItemTypeCategory;
                    itemType.IsActive = objSysItemType.IsActive == "Active";
                    itemType.IsDelete = objSysItemType.IsDelete;
                    itemType.ModifiedBy = 25;
                    itemType.ModifiedOn = DateTime.Now;
                    _context.SaveChanges();
                }

                _vmMsg.Type = Enums.MessageType.Update;
                _vmMsg.Msg = "Updated Successfully.";
            }
            catch (Exception ex)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to update.";
            }
            return _vmMsg;
        }

        public IEnumerable<SysItemType> GetAll()
        {
            IEnumerable<SysItemType> iLstSysItemType = from sit in _context.Sys_ItemType
                                                       where !sit.IsDelete
                                                       select new SysItemType
                                                       {
                                                           ItemTypeID = sit.ItemTypeID,
                                                           ItemTypeName = sit.ItemTypeName,
                                                           ItemTypeCategory = sit.ItemTypeCategory,
                                                           IsActive = sit.IsActive ? "Active" : "Inactive",
                                                           IsDelete = sit.IsDelete,
                                                       };
            return iLstSysItemType;
        } 

        public IEnumerable<SysItemType> GetAllActiveItemTypeLeather()
        {
            IEnumerable<SysItemType> iLstSysItemType = from sit in _context.Sys_ItemType 
                                                        where sit.IsActive && !sit.IsDelete && sit.ItemTypeCategory=="Leather"
                                                       select new SysItemType
                                                       {
                                                           ItemTypeID = sit.ItemTypeID,
                                                           ItemTypeName = sit.ItemTypeName,
                                                       };

            return iLstSysItemType;
        }

        public IEnumerable<SysItemType> GetAllActiveItemTypeChemical()
        {
            IEnumerable<SysItemType> iLstSysItemType = from sit in _context.Sys_ItemType
                                                       where sit.IsActive && !sit.IsDelete && sit.ItemTypeCategory == "Chemical"
                                                       select new SysItemType
                                                       {
                                                           ItemTypeID = sit.ItemTypeID,
                                                           ItemTypeName = sit.ItemTypeName,
                                                       };

            return iLstSysItemType;
        }

        public ValidationMsg Delete(string ItemTypeID, int userID)
        {
            try
            {
                var itemType = (from i in _context.Sys_ItemType.AsEnumerable()
                    where i.ItemTypeID == Convert.ToByte(ItemTypeID)
                    select i).FirstOrDefault();
                if (itemType != null)
                {
                    _context.Sys_ItemType.Remove(itemType);
                }
                _context.SaveChanges();

                _vmMsg.Type = Enums.MessageType.Success;
                _vmMsg.Msg = "Deleted Successfully.";
            }
            catch (SqlException se)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Cannot delete because it has already used.";
            }
            catch (Exception e )
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Delete.";
            }
           
            return _vmMsg;
        }

        public long GetItemTypeID()
        {
            return ItemTypeID;
        }
        
        
    }
}
