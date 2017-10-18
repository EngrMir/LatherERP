using System;
using System.Collections.Generic;
using System.Linq;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;

namespace ERP.DatabaseAccessLayer.AppSetupGateway
{
    public class DalSysSource
    {
        private readonly BLC_DEVEntities _context;


        public long SourceId = 0;

        public DalSysSource()
        {
            _context = new BLC_DEVEntities();
        }

        public ValidationMsg Create(SysSource objSysSource, int userid)
        {
            var vmMsg = new ValidationMsg();
            try
            {
                var tblSysSource = new Sys_Source
                {
                    SourceID = objSysSource.SourceID,
                    SourceName = objSysSource.SourceName,
                    SourceAddress = objSysSource.SourceAddress,
                    ContactPerson = objSysSource.ContactPerson,
                    ContactNumber = objSysSource.ContactNumber,
                    IsActive = objSysSource.IsActive == "Active",
                    IsDelete = objSysSource.IsDelete,
                    SetOn = DateTime.Now,
                    SetBy = userid,
                    IPAddress = string.Empty
                };

                _context.Sys_Source.Add(tblSysSource);
                _context.SaveChanges();
                SourceId = tblSysSource.SourceID;
                vmMsg.Type = Enums.MessageType.Success;
                vmMsg.Msg = "Saved Successfully.";
            }
            catch (Exception ex)
            {
                vmMsg.Type = Enums.MessageType.Error;
                vmMsg.Msg = "Failed to Save.";
            }
            return vmMsg;
        }

        public long GetSourceID()
        {
            return SourceId;
        }

        public ValidationMsg Update(SysSource objSysSource, int userid)
        {
            var vmMsg = new ValidationMsg();
            try
            {
                var sysSource = _context.Sys_Source.FirstOrDefault(s => s.SourceID == objSysSource.SourceID);
                if (sysSource != null)
                {
                    sysSource.SourceName = objSysSource.SourceName;
                    sysSource.SourceAddress = objSysSource.SourceAddress;
                    sysSource.ContactPerson = objSysSource.ContactPerson;
                    sysSource.ContactNumber = objSysSource.ContactNumber;
                    sysSource.IsActive = objSysSource.IsActive == "Active";// Convert.ToBoolean(objSysSource.IsActive);
                    sysSource.ModifiedOn = DateTime.Now;
                    sysSource.ModifiedBy = userid;
                }
                _context.SaveChanges();

                vmMsg.Type = Enums.MessageType.Update;
                vmMsg.Msg = "Updated Successfully.";
            }
            catch (Exception ex)
            {
                vmMsg.Type = Enums.MessageType.Error;
                vmMsg.Msg = "Failed to Update.";
            }
            return vmMsg;
        }

        public IEnumerable<SysSource> GetAll()
        {
            IEnumerable<SysSource> iLstSysSource = from ss in _context.Sys_Source
                                                   where !ss.IsDelete
                                                   select new SysSource
                                                          {
                                                              SourceID = ss.SourceID,
                                                              SourceName = ss.SourceName,
                                                              SourceAddress = ss.SourceAddress,
                                                              ContactPerson = ss.ContactPerson,
                                                              ContactNumber = ss.ContactNumber,
                                                              IsActive = ss.IsActive ? "Active" : "Inactive",
                                                              IsDelete = ss.IsDelete,
                                                          };

            return iLstSysSource;
        }

        public IEnumerable<SysSource> GetAllActiveSource()
        {
            IEnumerable<SysSource> iLstSysSource = from ss in _context.Sys_Source
                                                   where ss.IsActive && !ss.IsDelete
                                                   select new SysSource
                                                   {
                                                       SourceID = ss.SourceID,
                                                       SourceName = ss.SourceName
                                                   };

            return iLstSysSource;
        }

        public ValidationMsg Delete(string sourceId,int userid)
        {
            var sourceid = string.IsNullOrEmpty(sourceId) ? 0 : Convert.ToInt32(sourceId);
            var vmMsg = new ValidationMsg();
            try
            {
                var sysSource = _context.Sys_Source.FirstOrDefault(s => s.SourceID == sourceid);
                _context.Sys_Source.Remove(sysSource);
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

    }
}
