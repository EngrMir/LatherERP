using System;
using System.Collections.Generic;
using System.Linq;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using ERP.DatabaseAccessLayer.OperationGateway;

namespace ERP.DatabaseAccessLayer.AppSetupGateway
{
    public class DalSysMachine
    {
        private readonly BLC_DEVEntities _context;


        public long MachineId = 0;

        public DalSysMachine()
        {
            _context = new BLC_DEVEntities();
        }

        public ValidationMsg Create(SysMachine objSysMachine, int userid)
        {
            var vmMsg = new ValidationMsg();
            try
            {
                Sys_Machine tblSysMachine = SetToModelObject(objSysMachine, userid);
                _context.Sys_Machine.Add(tblSysMachine);
                _context.SaveChanges();

                MachineId = tblSysMachine.MachineID;
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

        public Sys_Machine SetToModelObject(SysMachine model, int userid)
        {
            Sys_Machine Entity = new Sys_Machine();

            Entity.MachineID = model.MachineID;
            Entity.MachineNo = model.MachineNo;
            Entity.MachineName = model.MachineName;
            Entity.MachineCategory = model.MachineCategory;
            Entity.MachineType = model.MachineType;
            Entity.MachineCapacity = model.MachineCapacity;
            Entity.CapacityUnit = model.CapacityUnit;
            if (model.InstallationDate == null)
                Entity.InstallationDate = null;
            else
                Entity.InstallationDate = DalCommon.SetDate(model.InstallationDate);
            if (model.ExpiaryDate == null)
                Entity.ExpiaryDate = null;
            else
                Entity.ExpiaryDate = DalCommon.SetDate(model.ExpiaryDate);
            Entity.Remarks = model.Remarks;
            Entity.IsActive = model.IsActive == "Active";
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }

        public long GetMachineID()
        {
            return MachineId;
        }

        public ValidationMsg Update(SysMachine objSysMachine, int userid)
        {
            var vmMsg = new ValidationMsg();
            try
            {
                var sysMachine = _context.Sys_Machine.FirstOrDefault(s => s.MachineID == objSysMachine.MachineID);
                if (sysMachine != null)
                {
                    sysMachine.MachineNo = objSysMachine.MachineNo;
                    sysMachine.MachineName = objSysMachine.MachineName;
                    sysMachine.MachineCategory = objSysMachine.MachineCategory;
                    sysMachine.MachineType = objSysMachine.MachineType;
                    sysMachine.MachineCapacity = objSysMachine.MachineCapacity;
                    sysMachine.CapacityUnit = objSysMachine.CapacityUnit;
                    if (objSysMachine.InstallationDate == null)
                        sysMachine.InstallationDate = null;
                    else
                        sysMachine.InstallationDate = DalCommon.SetDate(objSysMachine.InstallationDate);
                    if (objSysMachine.ExpiaryDate == null)
                        sysMachine.ExpiaryDate = null;
                    else
                        sysMachine.ExpiaryDate = DalCommon.SetDate(objSysMachine.ExpiaryDate);
                    //sysMachine.InstallationDate = DalCommon.SetDate(objSysMachine.InstallationDate);
                    //sysMachine.ExpiaryDate = DalCommon.SetDate(objSysMachine.ExpiaryDate);
                    sysMachine.Remarks = objSysMachine.Remarks;
                    sysMachine.IsActive = objSysMachine.IsActive == "Active";// Convert.ToBoolean(objSysMachine.IsActive);
                    sysMachine.ModifiedOn = DateTime.Now;
                    sysMachine.ModifiedBy = userid;
                    sysMachine.IPAddress = GetIPAddress.LocalIPAddress();
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

        public List<SysMachine> GetAll()
        {
            List<Sys_Machine> searchList = _context.Sys_Machine.OrderByDescending(m => m.MachineID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<SysMachine>();
        }

        public List<SysMachine> GetAllWetBlueProductionMachine()
        {
            //List<Sys_Machine> searchList = _context.Sys_Machine.Where(m => m.MachineCategory == "WBP").OrderByDescending(m => m.MachineID).ToList();
            List<Sys_Machine> searchList = _context.Sys_Machine.Where(m => m.MachineCategory == "WBP").ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<SysMachine>();
        }
        public List<SysMachine> GetAllCrustLeatherProductionMachine()
        {
            //List<Sys_Machine> searchList = _context.Sys_Machine.Where(m => m.MachineCategory == "WBP").OrderByDescending(m => m.MachineID).ToList();
            List<Sys_Machine> searchList = _context.Sys_Machine.Where(m => m.MachineCategory == "CLP").ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<SysMachine>();
        }

        public SysMachine SetToBussinessObject(Sys_Machine Entity)
        {
            SysMachine Model = new SysMachine();

            Model.MachineID = Entity.MachineID;
            Model.MachineNo = Entity.MachineNo;
            Model.MachineName = Entity.MachineName;
            Model.MachineCategory = Entity.MachineCategory;
            switch (Entity.MachineCategory)
            {
                case "RHP":
                    Model.MachineCategoryName = "Raw Hide Production";
                    break;
                case "WBP":
                    Model.MachineCategoryName = "Wet Blue Production";
                    break;
                case "CLP":
                    Model.MachineCategoryName = "Crust Leather Production";
                    break;
                case "FLP":
                    Model.MachineCategoryName = "Finished Leather Production";
                    break;
                default:
                    Model.MachineCategoryName = "";
                    break;
            }
            Model.MachineType = Entity.MachineType;
            switch (Entity.MachineType)
            {
                case "DRUM":
                    Model.MachineTypeName = "Drum";
                    break;
                //case "WBP":
                //    Model.MachineTypeName = "Wet Blue Production";
                //    break;
                //case "CLP":
                //    Model.MachineTypeName = "Crust Leather Production";
                //    break;
                //case "FLP":
                //    Model.MachineTypeName = "Finished Leather Production";
                //    break;
                default:
                    Model.MachineTypeName = "";
                    break;
            }
            Model.MachineCapacity = Entity.MachineCapacity;
            Model.CapacityUnit = Entity.CapacityUnit;
            Model.UnitName = Entity.CapacityUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.CapacityUnit).FirstOrDefault().UnitName;
            Model.InstallationDate = Entity.InstallationDate == null ? "" : Convert.ToDateTime(Entity.InstallationDate).ToString("dd/MM/yyyy");
            Model.ExpiaryDate = Entity.ExpiaryDate == null ? "" : Convert.ToDateTime(Entity.ExpiaryDate).ToString("dd/MM/yyyy");
            Model.Remarks = Entity.Remarks;
            Model.IsActive = Entity.IsActive == true ? "Active" : "Inactive";


            return Model;
        }

        public IEnumerable<SysMachine> GetAllActiveMachine()
        {
            IEnumerable<SysMachine> iLstSysMachine = from ss in _context.Sys_Machine
                                                     where ss.IsActive == true
                                                     select new SysMachine
                                                     {
                                                         MachineID = ss.MachineID,
                                                         MachineNo = ss.MachineNo,
                                                         MachineName = ss.MachineName
                                                     };

            return iLstSysMachine;
        }

        public ValidationMsg Delete(string sourceId, int userid)
        {
            var sourceid = string.IsNullOrEmpty(sourceId) ? 0 : Convert.ToInt32(sourceId);
            var vmMsg = new ValidationMsg();
            try
            {
                var sysMachine = _context.Sys_Machine.FirstOrDefault(s => s.MachineID == sourceid);
                if (sysMachine != null)
                {
                    _context.Sys_Machine.Remove(sysMachine);
                    _context.SaveChanges();
                    vmMsg.Type = Enums.MessageType.Success;
                    vmMsg.Msg = "Deleted Successfully.";
                }
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
