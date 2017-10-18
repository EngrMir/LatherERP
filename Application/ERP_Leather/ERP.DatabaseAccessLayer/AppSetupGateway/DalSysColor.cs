using System;
using System.Collections.Generic;
using System.Linq;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;

namespace ERP.DatabaseAccessLayer.AppSetupGateway
{
    public class DalSysColor
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg vmMsg;
        public int ColorID = 0;
        private string ColorCode = string.Empty;

        public DalSysColor()
        {
            _context = new BLC_DEVEntities();
        }

        public ValidationMsg Save(SysColor model, int userid)
        {
            var vmMsg = new ValidationMsg();
            try
            {
                Sys_Color tblSysColor = SetToModelObject(model, userid);
                _context.Sys_Color.Add(tblSysColor);

                _context.SaveChanges();
                ColorID = tblSysColor.ColorID;
                ColorCode = model.ColorCode;
                vmMsg.Type = Enums.MessageType.Success;
                vmMsg.Msg = "Saved Successfully.";
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.InnerException.Message.Contains("UNIQUE KEY"))
                    {
                        vmMsg.Type = Enums.MessageType.Error;
                        vmMsg.Msg = "Color Code Already Exit.";
                    }
                    else
                    {
                        vmMsg.Type = Enums.MessageType.Error;
                        vmMsg.Msg = "Failed to Save.";
                    }
                }
                else
                {
                    vmMsg.Type = Enums.MessageType.Error;
                    vmMsg.Msg = "Failed to Save.";
                }
            }
            return vmMsg;
        }

        public long GetColorID()
        {
            return ColorID;
        }

        public ValidationMsg Update(SysColor model, int userid)
        {
            var vmMsg = new ValidationMsg();
            try
            {
                var sysColor = _context.Sys_Color.FirstOrDefault(s => s.ColorID == model.ColorID);
                if (sysColor != null)
                {
                    sysColor.ColorCode = model.ColorCode;
                    sysColor.ColorName = model.ColorName;
                    sysColor.IsActive = model.IsActive == "Active";
                    sysColor.ModifiedOn = DateTime.Now;
                    sysColor.ModifiedBy = userid;
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
                    vmMsg.Msg = "Color Code Already Exit.";
                }
                else
                {
                    vmMsg.Type = Enums.MessageType.Error;
                    vmMsg.Msg = "Failed to Update.";
                }
            }
            return vmMsg;
        }

        public IEnumerable<SysColor> GetAll()
        {
            IEnumerable<SysColor> iLstSysColor = from ss in _context.Sys_Color
                                                 select new SysColor
                                                        {
                                                            ColorCode = ss.ColorCode,
                                                            ColorID = ss.ColorID,
                                                            ColorName = ss.ColorName,
                                                            IsActive = ss.IsActive == true ? "Active" : "Inactive",
                                                        };

            return iLstSysColor;
        }

        public IEnumerable<SysColor> GetAllActiveColor()
        {
            IEnumerable<SysColor> iLstSysColor = from ss in _context.Sys_Color
                                                 where ss.IsActive == true
                                                 select new SysColor
                                                 {
                                                     ColorID = ss.ColorID,
                                                     ColorCode = ss.ColorCode,
                                                     ColorName = ss.ColorName
                                                 };

            return iLstSysColor;
        }

        public ValidationMsg Delete(string ColorID)
        {
            var id = string.IsNullOrEmpty(ColorID) ? 0 : Convert.ToInt32(ColorID);
            var vmMsg = new ValidationMsg();
            try
            {
                var sysColor = _context.Sys_Color.FirstOrDefault(s => s.ColorID == id);
                _context.Sys_Color.Remove(sysColor);
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
        public Sys_Color SetToModelObject(SysColor model, int userid)
        {
            Sys_Color entity = new Sys_Color();

            entity.ColorID = model.ColorID;
            entity.ColorCode = model.ColorCode;
            entity.ColorName = model.ColorName;
            entity.IsActive = model.IsActive == "Active";
            entity.SetOn = DateTime.Now;
            entity.SetBy = userid;
            entity.IPAddress = string.Empty;
            return entity;
        }


        public string GetPreDefineValue(string groupid, string pageid)
        {
            var preDefineValueForId =
                _context.Sys_PreDefineValueFor.Where(m => m.ConcernPageID == pageid)
                    .FirstOrDefault()
                    .PreDefineValueForID;
            return _context.Sys_PreDefineValue.Where(m => m.PreDefineValueForID == preDefineValueForId && m.PreDefineValueGroup == groupid && m.IsActive == true).FirstOrDefault() == null ? null : _context.Sys_PreDefineValue.Where(m => m.PreDefineValueForID == preDefineValueForId && m.PreDefineValueGroup == groupid && m.IsActive == true).FirstOrDefault().MaxValue;
        }
    }
}
