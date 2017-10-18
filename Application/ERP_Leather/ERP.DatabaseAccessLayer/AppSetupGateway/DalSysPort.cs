using System;
using System.Collections.Generic;
using System.Linq;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;

namespace ERP.DatabaseAccessLayer.AppSetupGateway
{
    public class DalSysPort
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg vmMsg;
        public int PortID = 0;

        public DalSysPort()
        {
            _context = new BLC_DEVEntities();
        }

        public ValidationMsg Save(SysPort model, int userid)
        {
            var vmMsg = new ValidationMsg();
            try
            {
                Sys_Port tblSysPort = SetToModelObject(model,userid);
                _context.Sys_Port.Add(tblSysPort);
                _context.SaveChanges();
                PortID = tblSysPort.PortID;
                vmMsg.Type = Enums.MessageType.Success;
                vmMsg.Msg = "Saved Successfully.";
            }
            catch (Exception ex)
            {
                if (ex.InnerException.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    vmMsg.Type = Enums.MessageType.Error;
                    vmMsg.Msg = "Port Code Already Exit..";
                }
                else
                {
                    vmMsg.Type = Enums.MessageType.Error;
                    vmMsg.Msg = "Failed to Save.";
                }
            }
            return vmMsg;
        }

        public long GetPortID()
        {
            return PortID;
        }

        public ValidationMsg Update(SysPort model, int userid)
        {
            var vmMsg = new ValidationMsg();
            try
            {
                var sysPort = _context.Sys_Port.FirstOrDefault(s => s.PortID == model.PortID);
                if (sysPort != null)
                {
                    sysPort.PortCode = model.PortCode;
                    sysPort.PortName = model.PortName;
                    sysPort.IsActive = model.IsActive == "Active";
                    sysPort.ModifiedOn = DateTime.Now;
                    sysPort.ModifiedBy = userid;
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
                    vmMsg.Msg = "Port Code Already Exit.";
                }
                else
                {
                    vmMsg.Type = Enums.MessageType.Error;
                    vmMsg.Msg = "Failed to Update.";
                }
            }
            return vmMsg;
        }

        public IEnumerable<SysPort> GetAll()
        {
            IEnumerable<SysPort> iLstSysPort = (from ss in _context.Sys_Port
                                                orderby ss.PortName
                                                select new SysPort
                                                       {
                                                           PortCode = ss.PortCode,
                                                           PortID = ss.PortID,
                                                           PortName = ss.PortName,
                                                           IsActive = ss.IsActive == true ? "Active" : "Inactive",
                                                       });

            return iLstSysPort;
        }

        //public IEnumerable<SysPort> GetAllActivePort()
        //{
        //    IEnumerable<SysPort> iLstSysPort = from ss in _context.Sys_Port
        //                                           where ss.IsActive
        //                                           select new SysPort
        //                                           {
        //                                               PortID = ss.PortID,
        //                                               PortName = ss.PortName
        //                                           };

        //    return iLstSysPort;
        //}

        public ValidationMsg Delete(string PortID)
        {
            var itemid = string.IsNullOrEmpty(PortID) ? 0 : Convert.ToInt32(PortID);
            var vmMsg = new ValidationMsg();
            try
            {
                var sysPort = _context.Sys_Port.FirstOrDefault(s => s.PortID == itemid);
                _context.Sys_Port.Remove(sysPort);
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
        public Sys_Port SetToModelObject(SysPort model,int userid)
        {
            Sys_Port entity = new Sys_Port();

            entity.PortID = model.PortID;
            entity.PortCode = model.PortCode;
            entity.PortName = model.PortName;
            entity.IsActive = model.IsActive == "Active";
            entity.SetOn = DateTime.Now;
            entity.SetBy = userid;
            entity.IPAddress = string.Empty;
            return entity;
        }
    }
}
