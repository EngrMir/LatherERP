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
    public class DalSysLeatherType
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;
        public long LeatherId = 0;
        public DalSysLeatherType()
        {
            _context = new BLC_DEVEntities();
        }

        public ValidationMsg Save(SysLeatherType objSysLeatherType, int userId)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var tblSysLeatherType = new Sys_LeatherType
                {
                    LeatherTypeID = objSysLeatherType.LeatherTypeID,
                    LeatherTypeName = objSysLeatherType.LeatherTypeName,
                    Description = objSysLeatherType.Description,
                    IsActive = objSysLeatherType.IsActive == "Active",
                    IsDelete = false,
                    SetOn = DateTime.Now,
                    SetBy = Convert.ToInt32(25),
                    ModifiedOn = DateTime.Now,
                    ModifiedBy = userId,
                    IPAddress = string.Empty
                };

                _context.Sys_LeatherType.Add(tblSysLeatherType);
                _context.SaveChanges();

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

        public ValidationMsg Update(SysLeatherType objSysLeatherType, int userId)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var leatherType = _context.Sys_LeatherType.FirstOrDefault(s => s.LeatherTypeID == objSysLeatherType.LeatherTypeID);
                if (leatherType != null)
                {
                    leatherType.LeatherTypeName = objSysLeatherType.LeatherTypeName;
                    leatherType.Description = objSysLeatherType.Description;
                    leatherType.IsActive = objSysLeatherType.IsActive == "Active";
                    leatherType.ModifiedOn = DateTime.Now;
                    leatherType.ModifiedBy = userId;
                }
                _context.SaveChanges();

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

        public IEnumerable<SysLeatherType> GetAll()
        {
            IEnumerable<SysLeatherType> iLstSysLeatherType = from slt in _context.Sys_LeatherType
                                                             where !slt.IsDelete
                                                             select new SysLeatherType
                                                             {
                                                                 LeatherTypeID = slt.LeatherTypeID,
                                                                 LeatherTypeName = slt.LeatherTypeName,
                                                                 Description = slt.Description,
                                                                 IsActive = slt.IsActive ? "Active" : "Inactive",
                                                                 IsDelete = slt.IsDelete,
                                                                 
                                                             };

            return iLstSysLeatherType;
        }

        public IEnumerable<SysLeatherType> GetAllActiveLeatherType()
        {
            IEnumerable<SysLeatherType> iLstSysLeatherType = from slt in _context.Sys_LeatherType
                                                             where slt.IsActive && !slt.IsDelete
                                                             select new SysLeatherType
                                                                {
                                                                    LeatherTypeID = slt.LeatherTypeID,
                                                                    LeatherTypeName = slt.LeatherTypeName
                                                                    //Description = slt.Description,
                                                                    //IsActive = slt.IsActive,
                                                                    //IsDelete = slt.IsDelete,
                                                                };
            return iLstSysLeatherType;
        }

        public long GetLeatherTypeID()
        {
            return LeatherId;
        }

        public ValidationMsg Delete(string leatherTypeID, int userid)
        {
            var typeId = string.IsNullOrEmpty(leatherTypeID) ? 0 : Convert.ToInt32(leatherTypeID);
            var vmMsg = new ValidationMsg();
            try
            {
                var sysType = _context.Sys_LeatherType.FirstOrDefault(s => s.LeatherTypeID == typeId);
                if (sysType != null)
                {
                    _context.Sys_LeatherType.Remove(sysType);
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
