using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;

namespace ERP.DatabaseAccessLayer.AppSetupGateway
{
    public class DalSysLeatherStatus
    {
        private readonly BLC_DEVEntities _context;
        private int LeatherStatusId = 0;
        private ValidationMsg _vmMsg;
        public DalSysLeatherStatus()
        {
            _context = new BLC_DEVEntities();
            _vmMsg = new ValidationMsg();
        }

        public ValidationMsg Save(SysLeatherStatus objSysLeatherStatus, int userId)
        {

            try
            {
                var tblSysLeatherStatus = new Sys_LeatherStatus
                {
                    LeatherStatusID = objSysLeatherStatus.LeatherStatusID,
                    LeatherStatusName = objSysLeatherStatus.LeatherStatusName,
                    Description = objSysLeatherStatus.Description,
                    IsActive = objSysLeatherStatus.IsActive == "Active",
                    IsDelete = objSysLeatherStatus.IsDelete,
                    SetOn = DateTime.Now,
                    SetBy = userId,
                    ModifiedOn = DateTime.Now,
                    ModifiedBy = userId,
                    IPAddress = string.Empty
                };

                _context.Sys_LeatherStatus.Add(tblSysLeatherStatus);
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

        public ValidationMsg Update(SysLeatherStatus objSysLeatherStatus, int userId)
        {
            try
            {
                var leatherStatus = _context.Sys_LeatherStatus.FirstOrDefault(s => s.LeatherStatusID == objSysLeatherStatus.LeatherStatusID);
                if (leatherStatus != null)
                {
                    leatherStatus.LeatherStatusName = objSysLeatherStatus.LeatherStatusName;
                    leatherStatus.Description = objSysLeatherStatus.Description;
                    leatherStatus.IsActive = objSysLeatherStatus.IsActive == "Active"; 
                    leatherStatus.IsDelete = objSysLeatherStatus.IsDelete;
                    leatherStatus.ModifiedBy = userId;
                    leatherStatus.ModifiedOn = DateTime.Now;
                }

                _context.SaveChanges();
                _vmMsg.Type = Enums.MessageType.Success;
                _vmMsg.Msg = "Updated Successfully.";
            }
            catch (Exception ex)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to update.";
            }
            return _vmMsg;
        }

        public IEnumerable<SysLeatherStatus> GetAll()
        {
            IEnumerable<SysLeatherStatus> iLstSysLeatherStatus = from sls in _context.Sys_LeatherStatus
                                                                 where !sls.IsDelete
                                                                 select new SysLeatherStatus
                                                                 {
                                                                     LeatherStatusID = sls.LeatherStatusID,
                                                                     LeatherStatusName = sls.LeatherStatusName,
                                                                     Description = sls.Description,
                                                                     IsActive = sls.IsActive ? "Active" : "Inactive",
                                                                     IsDelete = sls.IsDelete,
                                                                 };

            return iLstSysLeatherStatus;
        }

         public List<SysLeatherStatus> GetAllLeatherStatus()
        {
            List<Sys_LeatherStatus> searchList = _context.Sys_LeatherStatus.Where(x=>x.IsActive==true).ToList(); //using table
            return searchList.Select(c => SetToBussinessObject(c)).ToList<SysLeatherStatus>();
        }

        public SysLeatherStatus SetToBussinessObject(Sys_LeatherStatus Entity)
        {
            SysLeatherStatus Model = new SysLeatherStatus();

            Model.LeatherStatusID = Entity.LeatherStatusID;
            Model.LeatherStatusName = Entity.LeatherStatusName;

            return Model;
        }
        public long GetLeatherStatusID()
        {
            return LeatherStatusId;
        }

        public ValidationMsg Delete(string leatherStatusId, int userid)
        {
            var leatherStatusIds = string.IsNullOrEmpty(leatherStatusId) ? 0 : Convert.ToInt32(leatherStatusId);
            var vmMsg = new ValidationMsg();
            try
            {
                var leatherStatus = _context.Sys_LeatherStatus.FirstOrDefault(s => s.LeatherStatusID == leatherStatusIds);
                if (leatherStatus != null)
                {
                    leatherStatus.IsDelete = true;
                    leatherStatus.ModifiedOn = DateTime.Now;
                    leatherStatus.ModifiedBy = userid;
                }
                _context.SaveChanges();

                vmMsg.Type = Enums.MessageType.Success;
                vmMsg.Msg = "Deleted Successfully.";
            }
            catch (Exception ex)
            {
                vmMsg.Type = Enums.MessageType.Error;
                vmMsg.Msg = "Failed to record updated.";
            }
            return vmMsg;
        }
        ////public ValidationMsg Delete(string leatherStatusId, int userId)
        ////{
        ////    try
        ////    {
        ////        var leatherStatus = _context.Sys_LeatherStatus.FirstOrDefault(s => s.LeatherStatusID == leatherStatusId);
        ////        if (leatherStatus != null)
        ////        {
        ////            leatherStatus.LeatherStatusName = leatherStatusId.LeatherStatusName;
        ////            leatherStatus.Description = leatherStatusId.Description;
        ////            //leatherStatus.IsActive = objLeatherStatus.IsActive ? "Active" : "Inactive";
        ////            leatherStatus.IsDelete = true;
        ////            leatherStatus.ModifiedBy = userId;
        ////            leatherStatus.ModifiedOn = DateTime.Now;

        ////        }
        ////        _context.SaveChanges();

        ////        _vmMsg.Type = Enums.MessageType.Success;
        ////        _vmMsg.Msg = "Delete Successfully.";
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        _vmMsg.Type = Enums.MessageType.Success;
        ////        _vmMsg.Msg = "Failed to record deleted.";
        ////    }
        ////    return _vmMsg;
        ////}
    }
}


