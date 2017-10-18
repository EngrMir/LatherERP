using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;

namespace ERP.DatabaseAccessLayer.AppSetupGateway
{
    public class DalSysCurrency
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg vmMsg;

        public long CurencyId = 0;

        public DalSysCurrency()
        {
            _context = new BLC_DEVEntities();
            vmMsg = new ValidationMsg();
        }

        public ValidationMsg Save(SysCurrency objSysCurrency, int userId)
        {
            try
            {
                var tblSysCurrency = new Sys_Currency
                {
                    CurrencyID = objSysCurrency.CurrencyID,
                    CurrencyName = objSysCurrency.CurrencyName,
                    Description = objSysCurrency.Description,
                    IsActive = objSysCurrency.IsActive == "Active",
                    IsDelete = false,
                    SetOn = DateTime.Now,
                    SetBy = userId,
                    ModifiedOn = DateTime.Now,
                    ModifiedBy = userId,
                    IPAddress = string.Empty,
                };

                _context.Sys_Currency.Add(tblSysCurrency);
                _context.SaveChanges();
                CurencyId = tblSysCurrency.CurrencyID;
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

        public long GetCurrencyID()
        {
            return CurencyId;
        }
        public ValidationMsg Update(SysCurrency objSysCurrency, int userid)
        {
            try
            {
                var currency = _context.Sys_Currency.FirstOrDefault(s => s.CurrencyID == objSysCurrency.CurrencyID);
                if (currency != null)
                {
                    currency.CurrencyName = objSysCurrency.CurrencyName;
                    currency.Description = objSysCurrency.Description;
                    currency.IsActive = objSysCurrency.IsActive== "Active";
                    currency.ModifiedOn = DateTime.Now;
                    currency.ModifiedBy = userid;
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

        public IEnumerable<SysCurrency> GetAll()
        {
            IEnumerable<SysCurrency> iLstSysCurrency = (from sc in _context.Sys_Currency
                                                       where !sc.IsDelete
                                                       orderby sc.CurrencyName
                                                       select new SysCurrency
                                                       {
                                                           CurrencyID = sc.CurrencyID,
                                                           CurrencyName = sc.CurrencyName,
                                                           Description = sc.Description,
                                                           IsActive = sc.IsActive ? "Active" : "Inactive",
                                                       });

            return iLstSysCurrency;
        }

        public IEnumerable<SysCurrency> GetAllActiveCurrency()
        {
            IEnumerable<SysCurrency> iLstSysCurrency = (from sc in _context.Sys_Currency
                                                       where sc.IsDelete == false & sc.IsActive==true
                                                       orderby sc.CurrencyName
                                                       select new SysCurrency
                                                       {
                                                           CurrencyID = sc.CurrencyID,
                                                           CurrencyName = sc.CurrencyName
                                                       }).ToList();

            return iLstSysCurrency;
        }

        public string GetCurrencyName(byte id)
        {
            return _context.Sys_Currency.Where(m => m.IsDelete == false && m.CurrencyID == id).SingleOrDefault().CurrencyName.ToString(CultureInfo.InvariantCulture);
        }

        public ValidationMsg Delete(string currencyId, int userid)
        {
            var currencyid = string.IsNullOrEmpty(currencyId) ? 0 : Convert.ToInt32(currencyId);
            var vmMsg = new ValidationMsg();
            try
            {
                var sysSource = _context.Sys_Currency.FirstOrDefault(s => s.CurrencyID == currencyid);
                if (sysSource != null)
                {
                    sysSource.IsDelete = true;
                    sysSource.ModifiedOn = DateTime.Now;
                    sysSource.ModifiedBy = userid;
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
    }
}
