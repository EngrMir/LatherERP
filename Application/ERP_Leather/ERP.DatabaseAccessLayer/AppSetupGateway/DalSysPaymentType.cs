using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;

namespace ERP.DatabaseAccessLayer.AppSetupGateway
{
    public class DalSysPaymentType
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;

        public long Id = 0;
        public DalSysPaymentType()
        {
            _context = new BLC_DEVEntities();
        }

        public long GetPaymentTypeID()
        {
            return Id;
        }
        public ValidationMsg Save(SysPaymentType objSysPaymentType, int userId)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var tblSysPaymentType = new Sys_PaymentType
                {
                    ID = objSysPaymentType.ID,
                    Name = objSysPaymentType.Name,
                    Description = objSysPaymentType.Description,
                    IsActive = objSysPaymentType.IsActive == "Active",
                    IsDelete = false,
                    SetOn = DateTime.Now,
                    SetBy = userId,
                    ModifiedOn = DateTime.Now,
                    ModifiedBy = userId,
                    IPAddress = string.Empty
                };

                _context.Sys_PaymentType.Add(tblSysPaymentType);
                _context.SaveChanges();
                Id = objSysPaymentType.ID;
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

        public ValidationMsg Update(SysPaymentType objSysPaymentType, int userId)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var paymentType = _context.Sys_PaymentType.FirstOrDefault(s => s.ID == objSysPaymentType.ID);
                if (paymentType != null)
                {
                    paymentType.Name = objSysPaymentType.Name;
                    paymentType.Description = objSysPaymentType.Description;
                    paymentType.IsActive = objSysPaymentType.IsActive == "Active";
                    paymentType.ModifiedOn = DateTime.Now;
                    paymentType.ModifiedBy = userId;
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

        public IEnumerable<SysPaymentType> GetAll()
        {
            IEnumerable<SysPaymentType> iLstSysPaymentType = from spt in _context.Sys_PaymentType
                                                             where !spt.IsDelete 
                                                             select new SysPaymentType
                                                             {
                                                                 ID = spt.ID,
                                                                 Name = spt.Name,
                                                                 Description = spt.Description,
                                                                 IsActive = spt.IsActive ? "Active" : "Inactive",
                                                                 IsDelete = spt.IsDelete,
                                                             };

            return iLstSysPaymentType;
        }

        public IEnumerable<SysPaymentType> GetAllPaymentTypeList()
        {
            IEnumerable<SysPaymentType> iLstSysPaymentType = from spt in _context.Sys_PaymentType
                                                             where !spt.IsDelete && spt.IsActive
                                                             select new SysPaymentType
                                                             {
                                                                 ID = spt.ID,
                                                                 Name = spt.Name
                                                             };

            return iLstSysPaymentType;
        }


        public ValidationMsg Delete(string id, int userId)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var i = string.IsNullOrEmpty(id) ? 0 : Convert.ToInt32(id);
                var paymentType = _context.Sys_PaymentType.FirstOrDefault(s => s.ID == i);
                if (paymentType != null)
                {
                    _context.Sys_PaymentType.Remove(paymentType);
                }
                _context.SaveChanges();

                _vmMsg.Type = Enums.MessageType.Update;
                _vmMsg.Msg = "Deleted Successfully.";
            }
            catch (SqlException se)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Cannot delete because it has already used.";
            }
            catch (Exception ex)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Delete.";
            }
            return _vmMsg;
        }
    }
}
