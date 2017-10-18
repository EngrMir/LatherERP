using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using DatabaseUtility;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.OperationModel;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.BaseModel;
using System.Transactions;
using System.Text;
using System.Threading.Tasks;


namespace ERP.DatabaseAccessLayer.AppSetupGateway
{
    public class DalSysComodity
    {

        private readonly BLC_DEVEntities _context = new BLC_DEVEntities();
        UnitOfWork repository = new UnitOfWork();
        private ValidationMsg _vmMsg = new ValidationMsg();
        private int save;
        public int ComodityID = 0;


        public DalSysComodity()
        {
            _context = new BLC_DEVEntities();
            _vmMsg = new ValidationMsg();

        }

        public IEnumerable<SysComodity>GetCommodity()
        {
            IEnumerable<SysComodity> query = from ss in _context.Sys_Comodity
                                             select new SysComodity
                        {
                            ComodityID = ss.ComodityID,
                            ComodityCode = ss.ComodityCode,
                            ComodityName = ss.ComodityName,
                            IsActive = ss.IsActive == true ? "Active" : "Inactive"
                        };
          
          return query;
        }

        public long GetComodityID()
        {
            return ComodityID;
        }

        public ValidationMsg Save(SysComodity model,int userid)
        {
            try
            {              
                using (_context)
                {
                    Sys_Comodity tblsysComodity = SetToModelObject(model, userid);
                    _context.Sys_Comodity.Add(tblsysComodity);
                    _context.SaveChanges();

                    //ComodityID = tblsysComodity.ComodityID;

                    _vmMsg.ReturnId = tblsysComodity.ComodityID;//dataSet.CnfBillID;
                    _vmMsg.ReturnCode = tblsysComodity.ComodityCode;
                   
                    _vmMsg.Type = Enums.MessageType.Success;
                    _vmMsg.Msg = "Saved Successfully.";
                }   
            }
            catch(Exception ex) 
            {
                if (ex.InnerException.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Comodity No Already Exit.";
                }
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to save.";
            }
            return _vmMsg;   
        }


        public ValidationMsg Update(SysComodity model, int userid)
        {
            try
            {
                using (_context)
                {

                    if (model.ComodityID!= 0)
                    {
                    Sys_Comodity IssueEntity = SetToModelObject(model, userid);
                    var OriginalEntity = _context.Sys_Comodity.First(m => m.ComodityID == model.ComodityID);

                    OriginalEntity.ComodityID = IssueEntity.ComodityID;
                    OriginalEntity.ComodityCode = IssueEntity.ComodityCode;
                    OriginalEntity.ComodityName = IssueEntity.ComodityName;
                    OriginalEntity.IsActive = IssueEntity.IsActive;
                    OriginalEntity.ModifiedBy = userid;
                    OriginalEntity.ModifiedOn = DateTime.Now;
                    _context.SaveChanges();

                    _vmMsg.Type = Enums.MessageType.Success;
                    _vmMsg.Msg = "Update Successfully.";
                    }
                    else{

                        _vmMsg.Type = Enums.MessageType.Error;
                        _vmMsg.Msg = "Data Save first.";
                    
                    }
                }
            }
            catch (Exception ex)
            {

                if (ex.InnerException.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Comodity No Already Exit.";
                }
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Update.";
            
               
            }
            return _vmMsg;
        }






        public Sys_Comodity SetToModelObject(SysComodity model, int userid)
        {
            Sys_Comodity Entity = new Sys_Comodity();

            Entity.ComodityID = model.ComodityID;
            Entity.ComodityCode = model.ComodityCode;
            Entity.ComodityName = model.ComodityName;
            Entity.IsActive = model.IsActive == "Active";
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }






        public ValidationMsg DeleteComodityFromGrid(long ComodityID, int userid)
        {
            var sourceid = Convert.ToInt32(ComodityID);
            var vmMsg = new ValidationMsg();
            try
            {
                var sysComodity = _context.Sys_Comodity.FirstOrDefault(s => s.ComodityID == sourceid);
                if (sysComodity != null)
                {
                    _context.Sys_Comodity.Remove(sysComodity);
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
