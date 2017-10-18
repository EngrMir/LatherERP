using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERP.DatabaseAccessLayer.AppSetupGateway
{
    public class DalSysProductionProces
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg vmMsg;
        public int ProcessID = 0;

        public DalSysProductionProces()
        {
            _context = new BLC_DEVEntities();
        }

        public ValidationMsg Save(SysProductionProces model, int userid)
        {
            var vmMsg = new ValidationMsg();
            try
            {
                Sys_ProductionProces tblSysProductionProces = SetToModelObject(model);
                _context.Sys_ProductionProces.Add(tblSysProductionProces);
                _context.SaveChanges();
                ProcessID = tblSysProductionProces.ProcessID;
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

        public long GetProcessID()
        {
            return ProcessID;
        }

        public ValidationMsg Update(SysProductionProces model, int userid)
        {
            var vmMsg = new ValidationMsg();
            try
            {
                var sysProductionProces = _context.Sys_ProductionProces.FirstOrDefault(s => s.ProcessID == model.ProcessID);
                if (sysProductionProces != null)
                {
                    sysProductionProces.ProcessName = model.ProcessName;
                    sysProductionProces.ProcessCategory = model.ProcessCategory;
                    sysProductionProces.ProcessLevel = model.ProcessLevel;
                    sysProductionProces.ProcessEffect = model.ProcessEffect;
                    sysProductionProces.IsActive = model.IsActive == "Active";
                    sysProductionProces.ModifiedOn = DateTime.Now;
                    sysProductionProces.ModifiedBy = 2;
                    //sysProductionProces.ModifiedBy = userid;
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

        public IEnumerable<SysProductionProces> GetAllActiveWetBlueProcessList()
        {
            IEnumerable<SysProductionProces> iLstSysProductionProces = from ss in _context.Sys_ProductionProces
                                                                       where ss.IsActive == true && ss.ProcessCategory == "WB"
                                                                       select new SysProductionProces
                                                                              {
                                                                                  ProcessID = ss.ProcessID,
                                                                                  ProcessName = ss.ProcessName
                                                                              };
            return iLstSysProductionProces.ToList().OrderBy(o => o.ProcessID);
            //return iLstSysProductionProces.ToList().OrderBy(o => o.ProcessName);
        }

        public IEnumerable<SysProductionProces> GetAllActiveCrustProcessList()
        {
            IEnumerable<SysProductionProces> iLstSysProductionProces = from ss in _context.Sys_ProductionProces
                                                                       where ss.IsActive == true && ss.ProcessCategory == "CR"
                                                                       select new SysProductionProces
                                                                       {
                                                                           ProcessID = ss.ProcessID,
                                                                           ProcessName = ss.ProcessName
                                                                       };
            return iLstSysProductionProces.ToList().OrderBy(o => o.ProcessID);
            //return iLstSysProductionProces.ToList().OrderBy(o => o.ProcessName);
        }

        public IEnumerable<SysProductionProces> GetAllActiveFinishProcessList()
        {
            IEnumerable<SysProductionProces> iLstSysProductionProces = from ss in _context.Sys_ProductionProces
                                                                       where ss.IsActive == true && ss.ProcessCategory == "FN"
                                                                       select new SysProductionProces
                                                                       {
                                                                           ProcessID = ss.ProcessID,
                                                                           ProcessName = ss.ProcessName
                                                                       };
            return iLstSysProductionProces.ToList().OrderBy(o => o.ProcessID);
            //return iLstSysProductionProces.ToList().OrderBy(o => o.ProcessName);
        }

        public List<SysProductionProces> GetAll()
        {
            List<Sys_ProductionProces> searchList = _context.Sys_ProductionProces.OrderByDescending(m => m.ProcessID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<SysProductionProces>();
        }

        public SysProductionProces SetToBussinessObject(Sys_ProductionProces Entity)
        {
            SysProductionProces Model = new SysProductionProces();

            Model.ProcessID = Entity.ProcessID;
            Model.ProcessName = Entity.ProcessName;
            Model.ProcessCategory = Entity.ProcessCategory;
            switch (Entity.ProcessCategory)
            {
                case "WB":
                    Model.ProcessCategoryName = "Wet Blue";
                    break;
                case "CR":
                    Model.ProcessCategoryName = "Crust";
                    break;
                case "FN":
                    Model.ProcessCategoryName = "Finish";
                    break;
                //case "FLP":
                //    Model.ProcessCategoryName = "Finished Leather Production";
                //    break;
                default:
                    Model.ProcessCategoryName = "";
                    break;
            }
            Model.ProcessLevel = Entity.ProcessLevel;
            Model.ProcessEffect = Entity.ProcessEffect;
            switch (Entity.ProcessEffect)
            {
                case "NE":
                    Model.ProcessEffectName = "No Effect";
                    break;
                case "CL":
                    Model.ProcessEffectName = "Consume Leather";
                    break;
                case "CC":
                    Model.ProcessEffectName = "Consume Chemical";
                    break;
                case "CB":
                    Model.ProcessEffectName = "Consume Both";
                    break;
                default:
                    Model.ProcessEffectName = "";
                    break;
            }
            Model.IsActive = Entity.IsActive == true ? "Active" : "Inactive";

            return Model;
        }

        public IEnumerable<SysProductionProces> GetAllActiveProductionProces()
        {
            IEnumerable<SysProductionProces> iLstSysProductionProces = from ss in _context.Sys_ProductionProces
                                                                       where ss.IsActive == true
                                                                       select new SysProductionProces
                                                                       {
                                                                           ProcessID = ss.ProcessID,
                                                                           ProcessName = ss.ProcessName
                                                                       };

            return iLstSysProductionProces;
        }

        public ValidationMsg Delete(string ProcessID, int userid)
        {
            var itemid = string.IsNullOrEmpty(ProcessID) ? 0 : Convert.ToInt32(ProcessID);
            var vmMsg = new ValidationMsg();
            try
            {
                var sysProductionProces = _context.Sys_ProductionProces.FirstOrDefault(s => s.ProcessID == itemid);
                _context.Sys_ProductionProces.Remove(sysProductionProces);
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
        public Sys_ProductionProces SetToModelObject(SysProductionProces model)
        {
            Sys_ProductionProces entity = new Sys_ProductionProces();

            entity.ProcessID = model.ProcessID;
            entity.ProcessName = model.ProcessName;
            entity.ProcessCategory = model.ProcessCategory;
            entity.ProcessLevel = model.ProcessLevel;
            entity.ProcessEffect = model.ProcessEffect;
            entity.IsActive = model.IsActive == "Active";
            entity.SetOn = DateTime.Now;
            entity.SetBy = 2;
            //SetBy = userid,
            entity.IPAddress = string.Empty;
            return entity;
        }
    }
}
