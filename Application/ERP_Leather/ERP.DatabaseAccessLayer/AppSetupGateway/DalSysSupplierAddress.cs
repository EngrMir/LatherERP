using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;

namespace ERP.DatabaseAccessLayer.AppSetupGateway
{
    public class DalSysSupplierAddress
    {
        private readonly BLC_DEVEntities _context;

        public DalSysSupplierAddress()
        {
            _context = new BLC_DEVEntities();
        }

        //public ValidationMsg Create(SysSupplierAddress objSysSupplierAddress)
        //{
        //    var vmMsg = new ValidationMsg();
        //    try
        //    {
        //        var tblSysSupplierAddress = new Sys_SupplierAddress
        //        {
        //            SupplierAddressID = objSysSupplierAddress.SupplierAddressID,
        //            SupplierID = objSysSupplierAddress.SupplierID,
        //            Address = objSysSupplierAddress.Address,
        //            ContactPerson = objSysSupplierAddress.ContactPerson,
        //            ContactNumber = objSysSupplierAddress.ContactNumber,
        //            EmailAddress = objSysSupplierAddress.EmailAddress,
        //            FaxNo = objSysSupplierAddress.FaxNo,
        //            PhoneNo = objSysSupplierAddress.PhoneNo,
        //            SkypeID = objSysSupplierAddress.SkypeID,
        //            IsActive = objSysSupplierAddress.IsActive,
        //            IsDelete = false,
        //            SetOn = DateTime.Now,
        //            SetBy = 25,
        //            ModifiedOn = DateTime.Now,
        //            ModifiedBy = 25,
        //            IPAddress = string.Empty
        //        };

        //        _context.Sys_SupplierAddress.Add(tblSysSupplierAddress);
        //        _context.SaveChanges();

        //        vmMsg.Type = Enums.MessageType.Success;
        //        vmMsg.Msg = "Record Save Successfully.";
        //    }
        //    catch (Exception ex)
        //    {
        //        vmMsg.Type = Enums.MessageType.Error;
        //        vmMsg.Msg = "Failed to save.";
        //    }
        //    return vmMsg;

        //}

        //public ValidationMsg Update(SysSupplierAddress objSysSupplierAddress)
        //{
        //    var vmMsg = new ValidationMsg();
        //    try
        //    {
        //        var sysSupplierAddress = _context.Sys_SupplierAddress.FirstOrDefault(s => s.SupplierID == objSysSupplierAddress.SupplierID);
        //        if (sysSupplierAddress != null)
        //        {
        //            sysSupplierAddress.SupplierID = objSysSupplierAddress.SupplierID;
        //            sysSupplierAddress.Address = objSysSupplierAddress.Address;
        //            sysSupplierAddress.ContactPerson = objSysSupplierAddress.ContactPerson;
        //            sysSupplierAddress.ContactNumber = objSysSupplierAddress.ContactNumber;
        //            sysSupplierAddress.EmailAddress = objSysSupplierAddress.EmailAddress;
        //            sysSupplierAddress.FaxNo = objSysSupplierAddress.FaxNo;
        //            sysSupplierAddress.PhoneNo = objSysSupplierAddress.PhoneNo;
        //            sysSupplierAddress.SkypeID = objSysSupplierAddress.SkypeID;
        //            sysSupplierAddress.IsActive = Convert.ToBoolean(objSysSupplierAddress.IsActive);
        //            sysSupplierAddress.IsDelete = Convert.ToBoolean(objSysSupplierAddress.IsDelete);

        //            sysSupplierAddress.ModifiedOn = DateTime.Now;
        //            sysSupplierAddress.ModifiedBy = 27;
        //        }
        //        _context.SaveChanges();

        //        vmMsg.Type = Enums.MessageType.Update;
        //        vmMsg.Msg = "Record Update Successfully.";
        //    }
        //    catch (Exception ex)
        //    {
        //        vmMsg.Type = Enums.MessageType.Error;
        //        vmMsg.Msg = "Failed to record updated.";
        //    }
        //    return vmMsg;

        //}

        //public IEnumerable<SysSupplierAddress> GetAll()
        //{
        //    IEnumerable<SysSupplierAddress> iLstSysSupplierAddress = from ssa in _context.Sys_SupplierAddress
        //                                                             where !ssa.IsDelete && ssa.IsActive
        //                                                             select new SysSupplierAddress
        //                                                               {
        //                                                                   SupplierAddressID = ssa.SupplierAddressID,
        //                                                                   SupplierID = ssa.SupplierID,
        //                                                                   Address = ssa.Address,
        //                                                                   ContactPerson = ssa.ContactPerson,
        //                                                                   ContactNumber = ssa.ContactNumber,
        //                                                                   EmailAddress = ssa.EmailAddress,
        //                                                                   FaxNo = ssa.FaxNo,
        //                                                                   PhoneNo = ssa.PhoneNo,
        //                                                                   SkypeID = ssa.SkypeID,
        //                                                                   IsActive = ssa.IsActive,
        //                                                                   IsDelete = ssa.IsDelete,
        //                                                               };

        //    return iLstSysSupplierAddress;
        //}

        //public List<SysSupplier> GetSupplierList()
        //{
        //    List<SysSupplier> iListSysSupplier = (from s in _context.Sys_Supplier
        //                                          where s.IsActive && !s.IsDelete
        //                                          select new SysSupplier
        //                                          {
        //                                              SupplierID = s.SupplierID,
        //                                              SupplierName = s.SupplierName,
        //                                          }).ToList();

        //    return iListSysSupplier;
        //}

        //public ValidationMsg Delete(SysSupplierAddress objSysSupplierAddress)
        //{
        //    var vmMsg = new ValidationMsg();
        //    try
        //    {
        //        var sysSupplierAddress = _context.Sys_SupplierAddress.FirstOrDefault(s => s.SupplierID == objSysSupplierAddress.SupplierID);
        //        if (sysSupplierAddress != null)
        //        {
        //            sysSupplierAddress.SupplierID = objSysSupplierAddress.SupplierID;
        //            sysSupplierAddress.Address = objSysSupplierAddress.Address;
        //            sysSupplierAddress.ContactPerson = objSysSupplierAddress.ContactPerson;
        //            sysSupplierAddress.ContactNumber = objSysSupplierAddress.ContactNumber;
        //            sysSupplierAddress.EmailAddress = objSysSupplierAddress.EmailAddress;
        //            sysSupplierAddress.FaxNo = objSysSupplierAddress.FaxNo;
        //            sysSupplierAddress.PhoneNo = objSysSupplierAddress.PhoneNo;
        //            sysSupplierAddress.SkypeID = objSysSupplierAddress.SkypeID;
        //            sysSupplierAddress.IsActive = Convert.ToBoolean(objSysSupplierAddress.IsActive);
        //            sysSupplierAddress.IsDelete = true;

        //            sysSupplierAddress.ModifiedOn = DateTime.Now;
        //            sysSupplierAddress.ModifiedBy = 27;
        //        }
        //        _context.SaveChanges();

        //        vmMsg.Type = Enums.MessageType.Update;
        //        vmMsg.Msg = "Deleted Successfully.";
        //    }
        //    catch (Exception ex)
        //    {
        //        vmMsg.Type = Enums.MessageType.Error;
        //        vmMsg.Msg = "Failed to record updated.";
        //    }
        //    return vmMsg;
        //}
    }
}
