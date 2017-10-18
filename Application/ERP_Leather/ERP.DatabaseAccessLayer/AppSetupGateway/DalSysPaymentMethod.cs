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
    public class DalSysPaymentMethod
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;

        public long ID;

        public DalSysPaymentMethod()
        {
            _context = new BLC_DEVEntities();
            _vmMsg = new ValidationMsg();
        }

        public ValidationMsg Save(SysPaymentMethod objSysPaymentMethod, int userId)
        {
            try
            {
                var tblSysPaymentMethod = new Sys_PaymentMethod
                {
                    ID = objSysPaymentMethod.ID,
                    Name = objSysPaymentMethod.Name,
                    Description = objSysPaymentMethod.Description,
                    IsActive = objSysPaymentMethod.IsActive == "Active",
                    IsDelete = objSysPaymentMethod.IsDelete,
                    SetOn = DateTime.Now,
                    SetBy = userId,
                    ModifiedOn = DateTime.Now,
                    ModifiedBy = userId,
                    IPAddress = string.Empty
                };
                _context.Sys_PaymentMethod.Add(tblSysPaymentMethod);
                _context.SaveChanges();
                ID = objSysPaymentMethod.ID;
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

        public ValidationMsg Update(SysPaymentMethod objSysPaymentMethod, int userId)
        {
            try
            {
                var paymentMethod = _context.Sys_PaymentMethod.FirstOrDefault(s => s.ID == objSysPaymentMethod.ID);
                if (paymentMethod != null)
                {
                    paymentMethod.ID = objSysPaymentMethod.ID;
                    paymentMethod.Name = objSysPaymentMethod.Name;
                    paymentMethod.Description = objSysPaymentMethod.Description;
                    paymentMethod.IsActive = objSysPaymentMethod.IsActive == "Active";
                    paymentMethod.IsDelete = objSysPaymentMethod.IsDelete;
                    paymentMethod.ModifiedOn = DateTime.Now;
                    paymentMethod.ModifiedBy = userId;

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

        public IEnumerable<SysPaymentMethod> GetAll()
        {
            IEnumerable<SysPaymentMethod> iLstSysPaymentMethod = from smp in _context.Sys_PaymentMethod
                                                                 where !smp.IsDelete 
                                                                 select new SysPaymentMethod
                                                                 {
                                                                     ID = smp.ID,
                                                                     Name = smp.Name,
                                                                     Description = smp.Description,
                                                                     IsActive = smp.IsActive ? "Active" : "Inactive",
                                                                     IsDelete = smp.IsDelete,
                                                                 };

            return iLstSysPaymentMethod;
        }

        public IEnumerable<SysPaymentMethod> GetAll(bool isActive)
        {
            IEnumerable<SysPaymentMethod> iLstSysPaymentMethod = from smp in _context.Sys_PaymentMethod
                                                                 where smp.IsActive && !smp.IsDelete
                                                                 select new SysPaymentMethod
                                                                 {
                                                                     ID = smp.ID,
                                                                     Name = smp.Name,
                                                                     Description = smp.Description,
                                                                     IsActive = smp.IsActive ? "Active":"Inactive",
                                                                     IsDelete = smp.IsDelete,

                                                                 };

            return iLstSysPaymentMethod;
        }

        public IEnumerable<SysPaymentMethod> GetAllPaymentMethodList()
        {
            IEnumerable<SysPaymentMethod> iLstSysPaymentMethod = from smp in _context.Sys_PaymentMethod
                                                                 where !smp.IsDelete && smp.IsActive
                                                                 select new SysPaymentMethod
                                                                 {
                                                                     ID = smp.ID,
                                                                     Name = smp.Name
                                                                 };

            return iLstSysPaymentMethod;
        }
        public ValidationMsg Delete(string Id, int userId)
        {
            try
            {
                var id = string.IsNullOrEmpty(Id) ? 0 : Convert.ToInt32(Id);
                var paymentMethod = _context.Sys_PaymentMethod.FirstOrDefault(s => s.ID == id);
                if (paymentMethod != null)
                {
                    paymentMethod.IsDelete = true;
                    paymentMethod.ModifiedOn = DateTime.Now;
                    paymentMethod.ModifiedBy = userId;
                }
                _context.SaveChanges();
                _vmMsg.Type = Enums.MessageType.Success;
                _vmMsg.Msg = "Deleted Successfully.";
            }
            catch (Exception ex)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to delete.";
            }
            return _vmMsg;
        }

        public long GetPaymentMethodID()
        {
            return ID;
        }
    }
}
