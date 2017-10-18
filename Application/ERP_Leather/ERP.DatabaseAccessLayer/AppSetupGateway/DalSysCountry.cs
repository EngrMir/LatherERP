using System;
using System.Collections.Generic;
using System.Linq;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;

namespace ERP.DatabaseAccessLayer.AppSetupGateway
{
    public class DalSysCountry
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg vmMsg;
        public int CountryID = 0;

        public DalSysCountry()
        {
            _context = new BLC_DEVEntities();
        }

        public ValidationMsg Save(SysCountry model, int userid)
        {
            var vmMsg = new ValidationMsg();
            try
            {
                Sys_Country tblSysCountry = SetToModelObject(model, userid);
                _context.Sys_Country.Add(tblSysCountry);
                _context.SaveChanges();
                CountryID = tblSysCountry.CountryID;
                vmMsg.Type = Enums.MessageType.Success;
                vmMsg.Msg = "Saved Successfully.";
            }
            catch (Exception ex)
            {
                if (ex.InnerException.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    vmMsg.Type = Enums.MessageType.Error;
                    vmMsg.Msg = "Country Code Already Exit..";
                }
                else
                {
                    vmMsg.Type = Enums.MessageType.Error;
                    vmMsg.Msg = "Failed to Save.";
                }
            }
            return vmMsg;
        }

        public long GetCountryID()
        {
            return CountryID;
        }

        public ValidationMsg Update(SysCountry model, int userid)
        {
            var vmMsg = new ValidationMsg();
            try
            {
                var sysCountry = _context.Sys_Country.FirstOrDefault(s => s.CountryID == model.CountryID);
                if (sysCountry != null)
                {
                    sysCountry.CountryCode = model.CountryCode;
                    sysCountry.CountryName = model.CountryName;
                    sysCountry.IsActive = model.IsActive == "Active";
                    sysCountry.ModifiedOn = DateTime.Now;
                    sysCountry.ModifiedBy = userid;
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
                    vmMsg.Msg = "Country Code Already Exit.";
                }
                else
                {
                    vmMsg.Type = Enums.MessageType.Error;
                    vmMsg.Msg = "Failed to Update.";
                }
            }
            return vmMsg;
        }

        public IEnumerable<SysCountry> GetAll()
        {
            IEnumerable<SysCountry> iLstSysCountry = (from ss in _context.Sys_Country
                                                      orderby ss.CountryName
                                                      select new SysCountry
                                                             {
                                                                 CountryCode = ss.CountryCode,
                                                                 CountryID = ss.CountryID,
                                                                 CountryName = ss.CountryName,
                                                                 IsActive = ss.IsActive == true ? "Active" : "Inactive",
                                                             });

            return iLstSysCountry;
        }

        public IEnumerable<SysCountry> GetAllActiveCountry()
        {
            IEnumerable<SysCountry> iLstSysCountry = (from ss in _context.Sys_Country
                                                      where ss.IsActive == true
                                                      orderby ss.CountryName
                                                      select new SysCountry
                                                      {
                                                          CountryID = ss.CountryID,
                                                          CountryName = ss.CountryName
                                                      });

            return iLstSysCountry;
        }

        public ValidationMsg Delete(string CountryID)
        {
            var id = string.IsNullOrEmpty(CountryID) ? 0 : Convert.ToInt32(CountryID);
            var vmMsg = new ValidationMsg();
            try
            {
                var sysCountry = _context.Sys_Country.FirstOrDefault(s => s.CountryID == id);
                _context.Sys_Country.Remove(sysCountry);
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
        public Sys_Country SetToModelObject(SysCountry model, int userid)
        {
            Sys_Country entity = new Sys_Country();

            entity.CountryID = model.CountryID;
            entity.CountryCode = model.CountryCode;
            entity.CountryName = model.CountryName;
            entity.IsActive = model.IsActive == "Active";
            entity.SetOn = DateTime.Now;
            entity.SetBy = userid;
            entity.IPAddress = string.Empty;
            return entity;
        }
    }
}
