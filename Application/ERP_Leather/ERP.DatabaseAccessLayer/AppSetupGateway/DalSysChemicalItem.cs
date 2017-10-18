using System;
using System.Collections.Generic;
using System.Linq;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using System.Text;
namespace ERP.DatabaseAccessLayer.AppSetupGateway
{
    public class DalSysChemicalItem
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg vmMsg;
        public int ItemID = 0;

        public DalSysChemicalItem()
        {
            _context = new BLC_DEVEntities();
        }

        public ValidationMsg Save(SysChemicalItem model, int userid)
        {
            var vmMsg = new ValidationMsg();
            try
            {
                Sys_ChemicalItem tblSysChemicalItem = SetToModelObject(model, userid);
                _context.Sys_ChemicalItem.Add(tblSysChemicalItem);
                _context.SaveChanges();
                ItemID = tblSysChemicalItem.ItemID;
                vmMsg.Type = Enums.MessageType.Success;
                vmMsg.Msg = "Saved Successfully.";
            }
            catch (Exception ex)
            {
                if (ex.InnerException.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    vmMsg.Type = Enums.MessageType.Error;
                    vmMsg.Msg = "HS Code Already Exit..";
                }
                else
                {
                    vmMsg.Type = Enums.MessageType.Error;
                    vmMsg.Msg = "Failed to Save.";
                }
            }
            return vmMsg;
        }

        public long GetItemID()
        {
            return ItemID;
        }

        public ValidationMsg Update(SysChemicalItem model, int userid)
        {
            var vmMsg = new ValidationMsg();
            try
            {
                var sysChemicalItem = _context.Sys_ChemicalItem.FirstOrDefault(s => s.ItemID == model.ItemID);
                if (sysChemicalItem != null)
                {
                    sysChemicalItem.HSCode = model.HSCode;
                    sysChemicalItem.ItemName = model.ItemName;
                    sysChemicalItem.ItemCategory = model.ItemCategory;
                    sysChemicalItem.ItemTypeID = model.ItemTypeID;
                    sysChemicalItem.UnitID = model.UnitID;
                    sysChemicalItem.SafetyStock = model.SafetyStock;
                    sysChemicalItem.ReorderLevel = model.ReorderLevel;
                    sysChemicalItem.ExpiryLimit = model.ExpiryLimit;
                    sysChemicalItem.IsActive = model.IsActive == "Active";
                    sysChemicalItem.ModifiedOn = DateTime.Now;
                    sysChemicalItem.ModifiedBy = userid;
                }
                _context.SaveChanges();

                vmMsg.Type = Enums.MessageType.Update;
                vmMsg.Msg = "Updated Successfully.";
            }
            catch (Exception ex)
            {
                if (ex.InnerException.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    vmMsg.Type = Enums.MessageType.Error;
                    vmMsg.Msg = "HS Code Already Exit.";
                }
                else
                {
                    vmMsg.Type = Enums.MessageType.Error;
                    vmMsg.Msg = "Failed to Update.";
                }
            }
            return vmMsg;
        }

        public List<SysChemicalItem> GetAll()
        {
            List<Sys_ChemicalItem> searchList = _context.Sys_ChemicalItem.ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<SysChemicalItem>();
        }

        public SysChemicalItem SetToBussinessObject(Sys_ChemicalItem Entity)
        {
            SysChemicalItem Model = new SysChemicalItem();

            Model.ItemID = Entity.ItemID;
            Model.HSCode = Entity.HSCode;
            Model.ItemName = Entity.ItemName;
            Model.ItemCategory = Entity.ItemCategory;
            if (Entity.ItemCategory == "RH")
                Model.ItemCategoryName = "Raw Hide";
            else if (Entity.ItemCategory == "WB")
                Model.ItemCategoryName = "Wet Bule";
            else if (Entity.ItemCategory == "CR")
                Model.ItemCategoryName = "Crust";
            else if (Entity.ItemCategory == "FN")
                Model.ItemCategoryName = "Finished";
            else if (Entity.ItemCategory == "CM")
                Model.ItemCategoryName = "Common";
            else
                Model.ItemCategoryName = "--Select--";
            Model.ItemTypeID = Entity.ItemTypeID;
            Model.UnitID = Entity.UnitID;
            Model.ItemTypeName = _context.Sys_ItemType.Where(m => m.ItemTypeID == Entity.ItemTypeID).FirstOrDefault() != null ? _context.Sys_ItemType.Where(m => m.ItemTypeID == Entity.ItemTypeID).FirstOrDefault().ItemTypeName : null;
            Model.UnitName = _context.Sys_Unit.Where(m => m.UnitID == Entity.UnitID).FirstOrDefault() != null ? _context.Sys_Unit.Where(m => m.UnitID == Entity.UnitID).FirstOrDefault().UnitName : null;
            Model.SafetyStock = Entity.SafetyStock;
            Model.ReorderLevel = Entity.ReorderLevel;
            Model.ExpiryLimit = Entity.ExpiryLimit;
            Model.IsActive = Entity.IsActive == true ? "Active" : "Inactive";

            return Model;
        }

        public IEnumerable<SysChemicalItem> GetAllActiveChemicalItem()
        {
            IEnumerable<SysChemicalItem> iLstSysChemicalItem = from ss in _context.Sys_ChemicalItem
                                                               where ss.IsActive == true
                                                               select new SysChemicalItem
                                                               {
                                                                   ItemID = ss.ItemID,
                                                                   HSCode = ss.HSCode,
                                                                   ItemName = ss.ItemName,
                                                                   SafetyStock = ss.SafetyStock,
                                                                   UnitID = ss.UnitID,
                                                                   UnitName = _context.Sys_Unit.FirstOrDefault(m => m.UnitID == ss.UnitID) != null ? _context.Sys_Unit.FirstOrDefault(m => m.UnitID == ss.UnitID).UnitName : ""
                                                               };

            return iLstSysChemicalItem;
        }
        public IEnumerable<SysChemicalItem> GetAllChemicalItemCategory()
        {
            var query = new StringBuilder();
            using (_context)
            {
                query.Append(" SELECT ItemCategory,CASE ItemCategory WHEN 'CM' THEN 'Common' WHEN 'FN' THEN 'Finished' WHEN 'WB' THEN 'Wet Blue' WHEN 'CR' THEN 'Crust' ELSE 'N/A' END AS ItemCategoryName ");
                query.Append(" FROM Sys_ChemicalItem GROUP BY ItemCategory");
                var items = _context.Database.SqlQuery<SysChemicalItem>(query.ToString());
                return items.ToList().OrderBy(o => o.ItemCategoryName);
            }
        }

        public ValidationMsg Delete(string ItemID)
        {
            var itemid = string.IsNullOrEmpty(ItemID) ? 0 : Convert.ToInt32(ItemID);
            var vmMsg = new ValidationMsg();
            try
            {
                var sysChemicalItem = _context.Sys_ChemicalItem.FirstOrDefault(s => s.ItemID == itemid);
                _context.Sys_ChemicalItem.Remove(sysChemicalItem);
                _context.SaveChanges();

                vmMsg.Type = Enums.MessageType.Success;
                vmMsg.Msg = "Deleted Successfully.";
            }
            catch (Exception ex)
            {
                vmMsg.Type = Enums.MessageType.Error;
                vmMsg.Msg = "Failed to Delete.";
            }
            return vmMsg;
        }
        public Sys_ChemicalItem SetToModelObject(SysChemicalItem model, int userid)
        {
            Sys_ChemicalItem entity = new Sys_ChemicalItem();

            entity.ItemID = model.ItemID;
            entity.HSCode = model.HSCode;
            entity.ItemName = model.ItemName;
            entity.ItemCategory = model.ItemCategory;
            entity.ItemTypeID = model.ItemTypeID;
            entity.UnitID = model.UnitID;
            entity.SafetyStock = model.SafetyStock;
            entity.ReorderLevel = model.ReorderLevel;
            entity.ExpiryLimit = model.ExpiryLimit;
            entity.IsActive = model.IsActive == "Active";
            entity.SetOn = DateTime.Now;
            entity.SetBy = userid;
            entity.IPAddress = string.Empty;
            return entity;
        }
        public List<string> GetChemicalListForSearch()
        {
            return _context.Sys_ChemicalItem.Where(s=>s.IsActive==true).Select(m => m.ItemName).ToList();
        }
        
    }
}
