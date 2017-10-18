using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;

namespace ERP.DatabaseAccessLayer.AppSetupGateway
{
    public class DalSysUnit
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;
        public long unitId = 0;
        public DalSysUnit()
        {
            _context = new BLC_DEVEntities();
        }

        public ValidationMsg Save(SysUnit objSysUnit, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var tblSysUnit = new Sys_Unit
                {
                    UnitID = objSysUnit.UnitID,
                    UnitName = objSysUnit.UnitName,
                    UnitCategory = objSysUnit.UnitCategory,
                    IsActive = objSysUnit.IsActive == "Active",
                    IsDelete = false,
                    SetOn = DateTime.Now,
                    SetBy = Convert.ToInt32(25),
                    ModifiedOn = DateTime.Now,
                    ModifiedBy = Convert.ToInt32(25),
                    IPAddress = string.Empty
                };

                _context.Sys_Unit.Add(tblSysUnit);
                _context.SaveChanges();
                unitId = objSysUnit.UnitID;
                _vmMsg.Type = Enums.MessageType.Success;
                _vmMsg.Msg = "Saved Successfully.";
            }
            catch (Exception)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Save.";
            }

            return _vmMsg;
        }

        public ValidationMsg Update(SysUnit objSysUnit, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var unit = _context.Sys_Unit.FirstOrDefault(s => s.UnitID == objSysUnit.UnitID);
                if (unit != null)
                {
                    unit.UnitName = objSysUnit.UnitName;
                    unit.UnitCategory = objSysUnit.UnitCategory;
                    unit.IsActive = objSysUnit.IsActive == "Active";
                    unit.IsDelete = Convert.ToBoolean(objSysUnit.IsDelete);
                }
                _context.SaveChanges();

                _vmMsg.Type = Enums.MessageType.Update;
                _vmMsg.Msg = "Updated Successfully.";
            }
            catch (Exception)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to record updated.";
            }
            return _vmMsg;
        }

        public long GetUnitID()
        {
            return unitId;
        }

        public IEnumerable<SysUnit> GetAll()
        {
            IEnumerable<SysUnit> iLstSysUnit = from su in _context.Sys_Unit
                                               where !su.IsDelete
                                               select new SysUnit
                                               {
                                                   UnitID = su.UnitID,
                                                   UnitName = su.UnitName,
                                                   UnitCategory = su.UnitCategory,
                                                   IsActive = su.IsActive ? "Active" : "Inactive",
                                                   IsDelete = su.IsDelete,
                                               };

            return iLstSysUnit;
        }

        public IEnumerable<SysUnit> GetAll(bool isActive)
        {
            IEnumerable<SysUnit> iLstSysUnit = from su in _context.Sys_Unit
                                               where su.IsActive && su.IsDelete
                                               select new SysUnit
                                               {
                                                   UnitID = su.UnitID,
                                                   UnitName = su.UnitName,
                                                   UnitCategory = su.UnitCategory,
                                                   IsActive = su.IsActive ? "Active" : "Inactive",
                                                   IsDelete = su.IsDelete,
                                               };

            return iLstSysUnit;
        }

        public IEnumerable<SysUnit> GetAllUnit()
        {
            IEnumerable<SysUnit> iLstSysItemType = from sit in _context.Sys_Unit
                                                   where sit.IsActive && !sit.IsDelete
                                                   select new SysUnit
                                                   {
                                                       UnitID = sit.UnitID,
                                                       UnitName = sit.UnitName
                                                   };

            return iLstSysItemType;
        }

        // Changed from common to leather
        public IEnumerable<SysUnit> GetAllActiveUnit()
        {
            IEnumerable<SysUnit> iLstSysItemType = from sit in _context.Sys_Unit
                                                   where sit.IsActive && !sit.IsDelete && sit.UnitCategory == "Leather"
                                                   orderby sit.UnitName
                                                   select new SysUnit
                                                       {
                                                           UnitID = sit.UnitID,
                                                           UnitName = sit.UnitName
                                                       };

            return iLstSysItemType;
        }

        public IEnumerable<SysUnit> GetAllActiveThicknessUnit()
        {
            IEnumerable<SysUnit> iLstSysItemType = from sit in _context.Sys_Unit
                                                   where sit.IsActive && !sit.IsDelete && sit.UnitCategory == "Thickness"
                                                   orderby sit.UnitName
                                                   select new SysUnit
                                                   {
                                                       UnitID = sit.UnitID,
                                                       UnitName = sit.UnitName
                                                   };

            return iLstSysItemType;
        }

        public IEnumerable<SysUnit> GetAllCommonAndLeatherUnit()
        {
            IEnumerable<SysUnit> iLstSysItemType = from sit in _context.Sys_Unit
                                                   where sit.IsActive && !sit.IsDelete && sit.UnitCategory == "Common" || sit.UnitCategory == "Leather"
                                                   select new SysUnit
                                                   {
                                                       UnitID = sit.UnitID,
                                                       UnitName = sit.UnitName
                                                   };

            return iLstSysItemType;
        }

        public IEnumerable<SysUnit> GetCommonUnit()
        {
            IEnumerable<SysUnit> iLstSysItemType = from sit in _context.Sys_Unit
                                                   where sit.IsActive && !sit.IsDelete && sit.UnitCategory == "Common"
                                                   select new SysUnit
                                                   {
                                                       UnitID = sit.UnitID,
                                                       UnitName = sit.UnitName
                                                   };

            return iLstSysItemType;
        }

        public IEnumerable<SysUnit> GetAllActiveLeatherUnit()
        {
            IEnumerable<SysUnit> iLstSysItemType = from sit in _context.Sys_Unit
                                                   where sit.IsActive && !sit.IsDelete && sit.UnitCategory == "Leather"
                                                   select new SysUnit
                                                   {
                                                       UnitID = sit.UnitID,
                                                       UnitName = sit.UnitName
                                                   };

            return iLstSysItemType;
        }


        public IEnumerable<SysUnit> GetAllActiveLeatherChemical()
        {
            IEnumerable<SysUnit> iLstSysItemType = from sit in _context.Sys_Unit
                                                   where sit.IsActive && !sit.IsDelete && sit.UnitCategory == "Chemical"
                                                   select new SysUnit
                                                   {
                                                       UnitID = sit.UnitID,
                                                       UnitName = sit.UnitName
                                                   };

            return iLstSysItemType;
        }

        public IEnumerable<SysUnit> GetAllActiveChemicalPack()
        {
            IEnumerable<SysUnit> iLstSysItemType = from sit in _context.Sys_Unit
                                                   where sit.IsActive && !sit.IsDelete && sit.UnitCategory == "ChemicalPack"
                                                   select new SysUnit
                                                   {
                                                       UnitID = sit.UnitID,
                                                       UnitName = sit.UnitName
                                                   };

            return iLstSysItemType;
        }

        public string GetAreaUnitName(byte id)
        {
            return _context.Sys_Unit.Where(m => m.IsDelete == false && m.UnitID == id).SingleOrDefault().UnitName.ToString(CultureInfo.InvariantCulture);
        }

        public ValidationMsg Delete(string unitId, int userid)
        {
            var unitid = string.IsNullOrEmpty(unitId) ? 0 : Convert.ToInt32(unitId);
            var vmMsg = new ValidationMsg();
            try
            {
                var sysUnit = _context.Sys_Unit.FirstOrDefault(s => s.UnitID == unitid);
                if (sysUnit != null)
                {
                    _context.Sys_Unit.Remove(sysUnit);
                }
                _context.SaveChanges();

                vmMsg.Type = Enums.MessageType.Success;
                vmMsg.Msg = "Deleted Successfully.";
            }
            catch (SqlException se)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Cannot delete because it has already used.";
            }
            catch (Exception ex)
            {
                vmMsg.Type = Enums.MessageType.Error;
                vmMsg.Msg = "Failed to record updated.";
            }
            return vmMsg;

        }
    }
}
