using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using System.Transactions;
using ERP.DatabaseAccessLayer.OperationGateway;

namespace ERP.DatabaseAccessLayer.AppSetupGateway
{
    public class DalSysSupplier
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;

        public int SupplierID = 0;
        private int supplieraddress = 0;
        private int localAgent = 0;
        private int foreignAgent = 0;
        public string SupplierCode = string.Empty;

        public DalSysSupplier()
        {
            _context = new BLC_DEVEntities();
        }

        public ValidationMsg Save(SysSupplier model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        //var exitSupplierCode = _context.Sys_Supplier.Where(m => m.SupplierCode == model.SupplierCode).ToList();
                        //if (exitSupplierCode.Count > 0)
                        //{
                        //    _vmMsg.Type = Enums.MessageType.Error;
                        //    _vmMsg.Msg = "Supplier Code Already Exit.";
                        //}
                        //else
                        //{
                        #region Supplier
                        model.SupplierCode = DalCommon.GetPreDefineNextCodeByUrl("Supplier/Supplier");//DalCommon.GetPreDefineValue("1", "00045");

                        model.SetBy = userid;
                        Sys_Supplier tblSysSupplier = SetToModelObject(model);
                        _context.Sys_Supplier.Add(tblSysSupplier);
                        _context.SaveChanges();

                        SupplierID = tblSysSupplier.SupplierID;

                        #endregion

                        #region Save Supplier Address

                        if (model.SupplierAddressList != null)
                        {
                            if (model.SupplierAddressList.Count > 1)
                            {
                                supplieraddress = 1;
                            }
                            else
                            {
                                foreach (SysSupplierAddress objSysSupplierAddress in model.SupplierAddressList)
                                {
                                    objSysSupplierAddress.SupplierID = SupplierID;
                                    objSysSupplierAddress.SetBy = userid;
                                    objSysSupplierAddress.IsActive = string.IsNullOrEmpty(objSysSupplierAddress.IsActive) ? "Active" : objSysSupplierAddress.IsActive;
                                    Sys_SupplierAddress tblSupplierAddress = SetToModelObject(objSysSupplierAddress);
                                    _context.Sys_SupplierAddress.Add(tblSupplierAddress);
                                }

                                #region Save Supplier Agent

                                if (model.SupplierAgentList != null)
                                {
                                    foreach (SysSupplierAgent objSupplierAgent in model.SupplierAgentList)
                                    {
                                        if (objSupplierAgent.SupplierID != 0)
                                        {
                                            if (objSupplierAgent.AgentType == "Local Agent")
                                            {
                                                ++localAgent;
                                            }
                                            else if (objSupplierAgent.AgentType == "Foreign Agent")
                                            {
                                                ++foreignAgent;
                                            }
                                            if (localAgent == 1 || foreignAgent == 1)
                                            {
                                                objSupplierAgent.SupplierAgentID = SupplierID;
                                                objSupplierAgent.SupplierID = SupplierID;
                                                objSupplierAgent.SetBy = userid;
                                                objSupplierAgent.IsActive = string.IsNullOrEmpty(objSupplierAgent.IsActive) ? "Active" : objSupplierAgent.IsActive;
                                                Sys_SupplierAgent tblSysSupplierAgent = SetToModelObject(objSupplierAgent);
                                                _context.Sys_SupplierAgent.Add(tblSysSupplierAgent);
                                            }
                                        }
                                    }
                                }

                                #endregion

                                #region Transaction

                                if (supplieraddress == 1)
                                {
                                    _vmMsg.Type = Enums.MessageType.Error;
                                    _vmMsg.Msg = "Please Enter only one Active Supplier Address.";
                                }

                                if (localAgent > 1 || foreignAgent > 1)
                                {
                                    _vmMsg.Type = Enums.MessageType.Error;
                                    _vmMsg.Msg = "Please Enter Same Agent Type Only Once.";
                                }

                                if (supplieraddress == 0 && localAgent == 1 && foreignAgent == 1)
                                {
                                    _context.SaveChanges();
                                    tx.Complete();
                                    SupplierCode = model.SupplierCode;
                                    _vmMsg.Type = Enums.MessageType.Success;
                                    _vmMsg.Msg = "Saved Successfully.";
                                }
                                if (supplieraddress == 0 && localAgent == 0 && foreignAgent == 1)
                                {
                                    _context.SaveChanges();
                                    tx.Complete();
                                    SupplierCode = model.SupplierCode;
                                    _vmMsg.Type = Enums.MessageType.Success;
                                    _vmMsg.Msg = "Saved Successfully.";
                                }
                                if (supplieraddress == 0 && localAgent == 1 && foreignAgent == 0)
                                {
                                    _context.SaveChanges();
                                    tx.Complete();
                                    SupplierCode = model.SupplierCode;
                                    _vmMsg.Type = Enums.MessageType.Success;
                                    _vmMsg.Msg = "Saved Successfully.";
                                }
                                if (supplieraddress == 0 && localAgent == 0 && foreignAgent == 0)
                                {
                                    _context.SaveChanges();
                                    tx.Complete();
                                    SupplierCode = model.SupplierCode;
                                    _vmMsg.Type = Enums.MessageType.Success;
                                    _vmMsg.Msg = "Saved Successfully.";
                                }

                                #endregion
                            }
                        }
                        else
                        {
                            _vmMsg.Type = Enums.MessageType.Error;
                            _vmMsg.Msg = "Please Enter Supplier Address.";
                        }

                        #endregion
                        //}
                    }
                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to save.";
            }
            return _vmMsg;
        }

        public int GetSupplierId()
        {
            return SupplierID;
        }

        public string GetSupplierCode()
        {
            return SupplierCode;
        }

        public ValidationMsg Update(SysSupplier model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        Sys_Supplier CurrentEntity = SetToModelObject(model);
                        var OriginalEntity = _context.Sys_Supplier.First(m => m.SupplierID == model.SupplierID);

                        OriginalEntity.SupplierID = CurrentEntity.SupplierID;
                        OriginalEntity.SupplierCode = CurrentEntity.SupplierCode;
                        OriginalEntity.SupplierName = CurrentEntity.SupplierName;
                        OriginalEntity.SupplierCategory = CurrentEntity.SupplierCategory;
                        OriginalEntity.SupplierType = CurrentEntity.SupplierType;
                        OriginalEntity.IsActive = CurrentEntity.IsActive;
                        OriginalEntity.ModifiedBy = userid;
                        OriginalEntity.ModifiedOn = DateTime.Now;

                        #region Save Supplier Address

                        if (model.SupplierAddressList != null)
                        {
                            foreach (SysSupplierAddress objsupplierAddress in model.SupplierAddressList)
                            {
                                if (objsupplierAddress.SupplierAddressID == 0)
                                {
                                    var exitData = _context.Sys_SupplierAddress.Where(m => m.SupplierID == model.SupplierID && !m.IsDelete && m.IsActive).ToList();
                                    if (exitData.Count > 0)
                                    {
                                        supplieraddress = 1;
                                    }
                                    else
                                    {
                                        objsupplierAddress.SupplierID = model.SupplierID;
                                        objsupplierAddress.SetBy = userid;
                                        objsupplierAddress.IsActive = string.IsNullOrEmpty(objsupplierAddress.IsActive) ? "Active" : objsupplierAddress.IsActive;
                                        Sys_SupplierAddress tblPurchaseYearPeriod =
                                            SetToModelObject(objsupplierAddress);
                                        _context.Sys_SupplierAddress.Add(tblPurchaseYearPeriod);
                                        _context.SaveChanges();
                                    }
                                }
                                else
                                {
                                    if (objsupplierAddress.IsActive == "Inactive")
                                    {
                                        Sys_SupplierAddress CurEntity = SetToModelObject(objsupplierAddress);
                                        var OrgEntity = _context.Sys_SupplierAddress.First(m => m.SupplierAddressID == objsupplierAddress.SupplierAddressID);

                                        OrgEntity.SupplierAddressID = CurEntity.SupplierAddressID;
                                        OrgEntity.SupplierID = CurEntity.SupplierID;
                                        OrgEntity.Address = CurEntity.Address;
                                        OrgEntity.ContactPerson = CurEntity.ContactPerson;
                                        OrgEntity.ContactNumber = CurEntity.ContactNumber;
                                        OrgEntity.EmailAddress = CurEntity.EmailAddress;
                                        OrgEntity.FaxNo = CurEntity.FaxNo;
                                        OrgEntity.PhoneNo = CurEntity.PhoneNo;
                                        OrgEntity.SkypeID = CurEntity.SkypeID;
                                        OrgEntity.Website = CurEntity.Website;
                                        OrgEntity.IsActive = CurEntity.IsActive;
                                        OrgEntity.ModifiedBy = userid;
                                        OrgEntity.ModifiedOn = DateTime.Now;
                                    }
                                    else
                                    {
                                        var exitData = _context.Sys_SupplierAddress.Where(m => m.SupplierID == model.SupplierID && !m.IsDelete && m.IsActive).ToList();
                                        if ((exitData.Count > 0) && (model.SupplierAddressList.Count > 1))
                                        {
                                            supplieraddress = 1;
                                        }
                                        else
                                        {
                                            Sys_SupplierAddress CurEntity = SetToModelObject(objsupplierAddress);
                                            var OrgEntity =
                                                _context.Sys_SupplierAddress.First(
                                                    m => m.SupplierAddressID == objsupplierAddress.SupplierAddressID);

                                            OrgEntity.SupplierAddressID = CurEntity.SupplierAddressID;
                                            OrgEntity.SupplierID = CurEntity.SupplierID;
                                            OrgEntity.Address = CurEntity.Address;
                                            OrgEntity.ContactPerson = CurEntity.ContactPerson;
                                            OrgEntity.ContactNumber = CurEntity.ContactNumber;
                                            OrgEntity.EmailAddress = CurEntity.EmailAddress;
                                            OrgEntity.FaxNo = CurEntity.FaxNo;
                                            OrgEntity.PhoneNo = CurEntity.PhoneNo;
                                            OrgEntity.SkypeID = CurEntity.SkypeID;
                                            OrgEntity.Website = CurEntity.Website;
                                            OrgEntity.IsActive = CurEntity.IsActive;
                                            OrgEntity.ModifiedBy = userid;
                                            OrgEntity.ModifiedOn = DateTime.Now;
                                        }
                                    }

                                }
                            }
                        }

                        #endregion

                        #region Save Supplier Agent

                        if (model.SupplierAgentList != null)
                        {
                            foreach (SysSupplierAgent objSupplierAgent in model.SupplierAgentList)
                            {
                                if (objSupplierAgent.SupplierAgentID == 0)
                                {
                                    if (objSupplierAgent.SupplierID != 0)
                                    {
                                        if (objSupplierAgent.AgentType == "Local Agent")
                                        {
                                            ++localAgent;
                                            var exitData = _context.Sys_SupplierAgent.Where(m => m.SupplierID == model.SupplierID && !m.IsDelete && m.IsActive && m.AgentType == "Local Agent").ToList();
                                            if (exitData.Count > 0)
                                            {
                                                ++localAgent;
                                            }
                                        }
                                        else if (objSupplierAgent.AgentType == "Foreign Agent")
                                        {
                                            ++foreignAgent;
                                            var exitData = _context.Sys_SupplierAgent.Where(m => m.SupplierID == model.SupplierID && !m.IsDelete && m.IsActive && m.AgentType == "Foreign Agent").ToList();
                                            if (exitData.Count > 0)
                                            {
                                                ++foreignAgent;
                                            }
                                        }
                                        if (localAgent == 1 || foreignAgent == 1)
                                        {
                                            objSupplierAgent.SupplierID = model.SupplierID;
                                            objSupplierAgent.SetBy = userid;
                                            objSupplierAgent.IsActive = string.IsNullOrEmpty(objSupplierAgent.IsActive) ? "Active" : objSupplierAgent.IsActive;
                                            Sys_SupplierAgent tblSysSupplierAgent = SetToModelObject(objSupplierAgent);
                                            _context.Sys_SupplierAgent.Add(tblSysSupplierAgent);
                                        }
                                    }
                                }
                                else
                                {
                                    if (objSupplierAgent.AgentType == "Local Agent")
                                    {
                                        ++localAgent;
                                    }
                                    else if (objSupplierAgent.AgentType == "Foreign Agent")
                                    {
                                        ++foreignAgent;
                                    }
                                    if (localAgent == 1 || foreignAgent == 1)
                                    {
                                        Sys_SupplierAgent CurEntity = SetToModelObject(objSupplierAgent);
                                        var OrgEntity =
                                            _context.Sys_SupplierAgent.First(
                                                m => m.SupplierAgentID == objSupplierAgent.SupplierAgentID);

                                        OrgEntity.SupplierAgentID = CurEntity.SupplierAgentID;
                                        OrgEntity.AgentID = CurEntity.AgentID;
                                        OrgEntity.SupplierID = CurEntity.SupplierID;
                                        OrgEntity.AgentType = CurEntity.AgentType;
                                        OrgEntity.IsActive = CurEntity.IsActive;
                                        OrgEntity.ModifiedBy = userid;
                                        OrgEntity.ModifiedOn = DateTime.Now;
                                    }
                                }
                            }
                        }

                        #endregion

                        if (supplieraddress == 1)
                        {
                            _vmMsg.Type = Enums.MessageType.Error;
                            _vmMsg.Msg = "Please Enter only one Active Supplier Address.";
                        }

                        if (localAgent > 1 || foreignAgent > 1)
                        {
                            _vmMsg.Type = Enums.MessageType.Error;
                            _vmMsg.Msg = "Please Enter Same Agent Type Only Once.";
                        }

                        if (supplieraddress == 0 && localAgent == 1 && foreignAgent == 1)
                        {
                            _context.SaveChanges();
                            tx.Complete();
                            _vmMsg.Type = Enums.MessageType.Update;
                            _vmMsg.Msg = "Updated Successfully.";
                        }

                        if (supplieraddress == 0 && localAgent == 0 && foreignAgent == 1)
                        {
                            _context.SaveChanges();
                            tx.Complete();
                            _vmMsg.Type = Enums.MessageType.Update;
                            _vmMsg.Msg = "Updated Successfully.";
                        }

                        if (supplieraddress == 0 && localAgent == 1 && foreignAgent == 0)
                        {
                            _context.SaveChanges();
                            tx.Complete();
                            _vmMsg.Type = Enums.MessageType.Update;
                            _vmMsg.Msg = "Updated Successfully.";
                        }
                        if (supplieraddress == 0 && localAgent == 0 && foreignAgent == 0)
                        {
                            _context.SaveChanges();
                            tx.Complete();
                            _vmMsg.Type = Enums.MessageType.Update;
                            _vmMsg.Msg = "Updated Successfully.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "HSCode Already Exit..";
                }
                else
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Failed to Update.";
                }
            }
            return _vmMsg;
        }

        public Sys_Supplier SetToModelObject(SysSupplier model)
        {
            Sys_Supplier Entity = new Sys_Supplier();

            Entity.SupplierID = model.SupplierID;
            Entity.SupplierCode = model.SupplierCode;
            Entity.SupplierName = model.SupplierName;
            Entity.SupplierCategory = model.SupplierCategory;
            Entity.SupplierType = model.SupplierType;
            Entity.CountryID = model.CountryID;
            Entity.IsActive = model.IsActive == "Active";
            Entity.IsDelete = false;
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = model.SetBy;
            Entity.IPAddress = string.Empty;
            return Entity;
        }

        public Sys_SupplierAddress SetToModelObject(SysSupplierAddress model)
        {
            Sys_SupplierAddress Entity = new Sys_SupplierAddress();

            Entity.SupplierAddressID = model.SupplierAddressID;
            Entity.SupplierID = model.SupplierID;
            Entity.Address = model.Address;
            Entity.ContactPerson = model.ContactPerson;
            Entity.ContactNumber = model.ContactNumber;
            Entity.EmailAddress = model.EmailAddress;
            Entity.FaxNo = model.FaxNo;
            Entity.PhoneNo = model.PhoneNo;
            Entity.SkypeID = model.SkypeID;
            Entity.Website = model.Website;
            Entity.IsActive = model.IsActive == "Active";
            Entity.IsDelete = false;
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = model.SetBy;
            Entity.IPAddress = string.Empty;
            return Entity;
        }

        public Sys_SupplierAgent SetToModelObject(SysSupplierAgent model)
        {
            Sys_SupplierAgent Entity = new Sys_SupplierAgent();

            Entity.SupplierAgentID = model.SupplierAgentID;
            Entity.AgentID = model.AgentID;
            Entity.SupplierID = model.SupplierID;
            Entity.AgentType = model.AgentType;
            Entity.IsActive = model.IsActive == "Active";
            Entity.IsDelete = false;
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = model.SetBy;
            Entity.IPAddress = string.Empty;
            return Entity;
        }

        public ValidationMsg Delete(string supplierId, int userid)
        {
            var supplierid = string.IsNullOrEmpty(supplierId) ? 0 : Convert.ToInt32(supplierId);
            var vmMsg = new ValidationMsg();
            try
            {
                var supplierAddressList = _context.Sys_SupplierAddress.Where(s => s.SupplierID == supplierid && s.IsActive && !s.IsDelete).ToList();
                var supplierAgentList = _context.Sys_SupplierAgent.Where(s => s.SupplierID == supplierid && s.IsActive && !s.IsDelete).ToList();

                if (supplierAddressList.Count > 0 || supplierAgentList.Count > 0)
                {
                    vmMsg.Type = Enums.MessageType.Error;
                    vmMsg.Msg = "Child Record Found.";
                }
                else
                {
                    var sysLocation = _context.Sys_Supplier.FirstOrDefault(s => s.SupplierID == supplierid);
                    if (sysLocation != null)
                    {
                        sysLocation.IsDelete = true;
                        sysLocation.ModifiedOn = DateTime.Now;
                        sysLocation.ModifiedBy = userid;
                    }
                    _context.SaveChanges();

                    vmMsg.Type = Enums.MessageType.Success;
                    vmMsg.Msg = "Deleted Successfully.";
                }
            }
            catch (Exception ex)
            {
                vmMsg.Type = Enums.MessageType.Error;
                vmMsg.Msg = "Failed to Deleted.";
            }
            return vmMsg;
        }

        public ValidationMsg DeletedAddress(string supplierAddressId, int userid)
        {
            var supplieraddressid = string.IsNullOrEmpty(supplierAddressId) ? 0 : Convert.ToInt32(supplierAddressId);
            var vmMsg = new ValidationMsg();
            try
            {
                var sysLocation = _context.Sys_SupplierAddress.FirstOrDefault(s => s.SupplierAddressID == supplieraddressid);
                if (sysLocation != null)
                {
                    sysLocation.IsDelete = true;
                    sysLocation.ModifiedOn = DateTime.Now;
                    sysLocation.ModifiedBy = userid;
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

        public ValidationMsg DeletedAgent(string supplierAgentId, int userid)
        {
            var supplieragentid = string.IsNullOrEmpty(supplierAgentId) ? 0 : Convert.ToInt32(supplierAgentId);
            var vmMsg = new ValidationMsg();
            try
            {
                var sysLocation = _context.Sys_SupplierAgent.FirstOrDefault(s => s.SupplierAgentID == supplieragentid);
                if (sysLocation != null)
                {
                    sysLocation.IsDelete = true;
                    sysLocation.ModifiedOn = DateTime.Now;
                    sysLocation.ModifiedBy = userid;
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

        public IEnumerable<SysSupplier> GetAll()
        {
            IEnumerable<SysSupplier> iLstSysSupplier = from slt in _context.Sys_Supplier
                                                       where !slt.IsDelete && slt.IsActive
                                                       select new SysSupplier
                                                       {
                                                           SupplierID = slt.SupplierID,
                                                           SupplierCode = slt.SupplierCode,
                                                           SupplierName = slt.SupplierName,
                                                           SupplierCategory = slt.SupplierCategory,
                                                           SupplierType = slt.SupplierType,
                                                           CountryID = slt.CountryID,
                                                           //IsActive = slt.IsActive,
                                                           IsDelete = slt.IsDelete
                                                       };

            return iLstSysSupplier;
        }
        public object GetChemicalSupplier()
        {
            var iLstSysSupplier = (from s in _context.Sys_Supplier
                                   where s.IsActive.Equals(true) && s.IsDelete.Equals(false) && s.SupplierCategory == "Chemical"
                                   join sa in _context.Sys_SupplierAddress.Where(q => q.IsActive.Equals(true) && q.IsDelete.Equals(false))
                                   on s.SupplierID equals sa.SupplierID into temp
                                   from item in temp.DefaultIfEmpty()
                                   select new
                                   {
                                       SupplierID = s.SupplierID,
                                       SupplierCode = s.SupplierCode,
                                       SupplierName = s.SupplierName.ToUpper(),
                                       SupplierAddressID = (item.SupplierAddressID).ToString(),
                                       Address = item.Address,
                                       ContactNumber = item.ContactNumber,
                                       ContactPerson = item.ContactPerson
                                   }).ToList();



            return iLstSysSupplier;
        }
        public List<SupplierDetails> GetAllSupplier()
        {
            var AllData = (from s in _context.Sys_Supplier
                           where s.IsActive.Equals(true) && s.IsDelete.Equals(false)
                           join sa in _context.Sys_SupplierAddress.Where(q => q.IsActive.Equals(true) && q.IsDelete.Equals(false))
                           on s.SupplierID equals sa.SupplierID into temp
                           from item in temp.DefaultIfEmpty()
                           select new SupplierDetails
                           {
                               SupplierID = s.SupplierID,
                               SupplierCode = s.SupplierCode,
                               SupplierName = s.SupplierName.ToUpper(),
                               SupplierAddressID = (item.SupplierAddressID).ToString(),
                               Address = item.Address,
                               ContactNumber = item.ContactNumber,
                               ContactPerson = item.ContactPerson
                           }).ToList();


            return AllData;
        }

        public List<SupplierDetails> GetAllLocalChemicalSupplier()
        {
            var AllData = (from s in _context.Sys_Supplier
                           where s.IsActive.Equals(true) && s.IsDelete.Equals(false) && s.SupplierCategory.Equals("Chemical") && s.SupplierType.Equals("Local")
                           join sa in _context.Sys_SupplierAddress.Where(q => q.IsActive.Equals(true) && q.IsDelete.Equals(false))
                           on s.SupplierID equals sa.SupplierID into temp
                           from item in temp.DefaultIfEmpty()
                           select new SupplierDetails
                           {
                               SupplierID = s.SupplierID,
                               SupplierCode = s.SupplierCode,
                               SupplierName = s.SupplierName.ToUpper(),
                               SupplierAddressID = (item.SupplierAddressID).ToString(),
                               Address = item.Address,
                               ContactNumber = item.ContactNumber,
                               ContactPerson = item.ContactPerson
                           }).ToList();


            return AllData;
        }

        public List<SupplierDetails> GetCategoryWiseSupplier(string Category)
        {
            var AllSupplier = (from s in _context.Sys_Supplier.AsQueryable()
                               where s.SupplierCategory == (Category).ToString() && s.IsActive && !s.IsDelete

                               join sa in _context.Sys_SupplierAddress.AsQueryable().Where(x => x.IsActive && !x.IsDelete) on s.SupplierID equals sa.SupplierID into SupplierAddress
                               from sa2 in SupplierAddress.DefaultIfEmpty()
                               orderby (s.SupplierName)


                               select new SupplierDetails
                               {
                                   SupplierID = s.SupplierID,
                                   SupplierCode = s.SupplierCode,
                                   SupplierName = s.SupplierName.ToUpper(),
                                   SupplierAddressID = (sa2 == null ? null : (sa2.SupplierAddressID).ToString()),
                                   Address = (sa2 == null ? null : (sa2.Address).ToString()),
                                   ContactNumber = (sa2 == null ? null : (sa2.ContactNumber).ToString()),
                                   ContactPerson = (sa2 == null ? null : (sa2.ContactPerson).ToString()),
                               }).ToList();

            return AllSupplier;
        }
        public IEnumerable<SupplierDetails> GetAllChemicalSupplier()
        {
            var allData = (from s in _context.Sys_Supplier
                           where s.IsActive.Equals(true) && s.IsDelete.Equals(false) && s.SupplierCategory == "Chemical"
                           join sa in _context.Sys_SupplierAddress.Where(q => q.IsActive.Equals(true) && q.IsDelete.Equals(false))
                           on s.SupplierID equals sa.SupplierID into temp
                           from item in temp.DefaultIfEmpty()
                           select new SupplierDetails
                           {
                               SupplierID = s.SupplierID,
                               SupplierCode = s.SupplierCode,
                               SupplierName = s.SupplierName.ToUpper(),
                               SupplierAddressID = (item.SupplierAddressID).ToString(),
                               Address = item.Address,
                               ContactNumber = item.ContactNumber,
                               ContactPerson = item.ContactPerson
                           }).ToList().OrderBy(o=>o.SupplierName);


            return allData;
        }
        public IEnumerable<SupplierDetails> GetChemicalSupplierByType(string type)
        {
            var allData = (from s in _context.Sys_Supplier
                           where s.IsActive.Equals(true) && s.IsDelete.Equals(false) && s.SupplierCategory == "Chemical" && s.SupplierType == type
                           join sa in _context.Sys_SupplierAddress.Where(q => q.IsActive.Equals(true) && q.IsDelete.Equals(false))
                           on s.SupplierID equals sa.SupplierID into temp
                           from item in temp.DefaultIfEmpty()
                           select new SupplierDetails
                           {
                               SupplierID = s.SupplierID,
                               SupplierCode = s.SupplierCode,
                               SupplierName = s.SupplierName.ToUpper(),
                               SupplierAddressID = (item.SupplierAddressID).ToString(),
                               Address = item.Address,
                               ContactNumber = item.ContactNumber,
                               ContactPerson = item.ContactPerson
                           }).ToList().OrderBy(o=>o.SupplierName);


            return allData;
        }
        public List<SupplierDetails> GetAllChemicalSupplierList(string supplier)
        {
            var AllData = (from s in _context.Sys_Supplier
                           where s.IsActive.Equals(true) && s.IsDelete.Equals(false) && s.SupplierCategory == "Chemical" && s.SupplierName.StartsWith(supplier)
                           join sa in _context.Sys_SupplierAddress.Where(q => q.IsActive.Equals(true) && q.IsDelete.Equals(false))
                           on s.SupplierID equals sa.SupplierID into temp
                           from item in temp.DefaultIfEmpty()
                           select new SupplierDetails
                           {
                               SupplierID = s.SupplierID,
                               SupplierCode = s.SupplierCode,
                               SupplierName = s.SupplierName.ToUpper(),
                               SupplierAddressID = (item.SupplierAddressID).ToString(),
                               Address = item.Address
                               //,
                               //ContactNumber = item.ContactNumber,
                               //ContactPerson = item.ContactPerson
                           }).ToList();


            return AllData;
        }
        public List<SupplierDetails> GetSupplierList()
        {
            var AllData = (from s in _context.Sys_Supplier
                           where !s.IsDelete
                           join sa in _context.Sys_SupplierAddress on s.SupplierID equals sa.SupplierID into temp
                           from item in temp.DefaultIfEmpty()
                           select new SupplierDetails
                           {
                               SupplierID = s.SupplierID,
                               SupplierName = s.SupplierName,
                               SupplierAddressID = item.SupplierAddressID.ToString(),
                               Address = item.Address
                           }).ToList();


            return AllData;
        }
        public List<PrqSupplierBill> GetBillRefList(int supplierId)
        {
            var AllData = (from s in _context.Prq_SupplierBill.AsEnumerable()
                           where s.SupplierID == supplierId && s.RecordStatus == "NCF"
                           select new PrqSupplierBill
                           {
                               SupplierBillID = s.SupplierBillID,
                               SupplierBillNo = s.SupplierBillNo
                           }).ToList();
            return AllData;
        }
        public List<PrqPurchaseChallan> GetPurchaseChallanList()
        {
            var AllData = (from s in _context.Prq_PurchaseChallan.AsEnumerable()

                           select new PrqPurchaseChallan
                           {
                               ChallanID = s.ChallanID,
                               ChallanNo = s.ChallanNo,
                               ChallanDate = (s.ChallanDate).ToString()
                           }).ToList();

            return AllData;
        }
        public ValidationMsg Delete(SysSupplier objSysSupplier)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var supplier = _context.Sys_Supplier.FirstOrDefault(s => s.SupplierID == objSysSupplier.SupplierID);
                if (supplier != null)
                {
                    supplier.SupplierCode = objSysSupplier.SupplierCode;
                    supplier.SupplierName = objSysSupplier.SupplierName;
                    supplier.SupplierCategory = objSysSupplier.SupplierCategory;
                    supplier.SupplierType = objSysSupplier.SupplierType;
                    supplier.IsActive = Convert.ToBoolean(objSysSupplier.IsActive);
                    supplier.ModifiedOn = DateTime.Now;
                    supplier.ModifiedBy = 27;
                    supplier.IsDelete = true;
                }
                _context.SaveChanges();

                _vmMsg.Type = Enums.MessageType.Update;
                _vmMsg.Msg = "Record Deleted Successfully.";
            }
            catch (Exception ex)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to delete.";
            }
            return _vmMsg;
        }
        public string GetSupplierName(int supplierID)
        {
            var query = "SELECT SupplierName FROM [dbo].[Sys_Supplier]" +
                                "WHERE SupplierID =" + supplierID;

            var supplierName = _context.Database.SqlQuery<string>(query).ToString();
            return supplierName;
        }
        public List<SysSupplierAgent> GetSupplierAgentList()
        {
            List<Sys_Supplier> searchList = _context.Sys_Supplier.Where(m => m.SupplierType == "Local Agent" || m.SupplierType == "Foreign Agent").ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<SysSupplierAgent>();
        }
        public List<SysSupplierAgent> GetSupplierAgentSearchList(string supplier)
        {
            List<Sys_Supplier> searchList = _context.Sys_Supplier.Where(m => m.IsActive && !m.IsDelete).Where(m => m.SupplierType == "Local Agent" || m.SupplierType == "Foreign Agent").Where(m => m.SupplierName.StartsWith(supplier)).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<SysSupplierAgent>();
        }
        public List<SysSupplier> GetSupplierForSearchList()
        {
            List<Sys_Supplier> searchList = _context.Sys_Supplier.Where(m => !m.IsDelete && (m.SupplierType == "Local" || m.SupplierType == "Foreign")).ToList();
            return searchList.Select(c => SetToBussinessObjectSupplier(c)).OrderByDescending(m => m.SupplierID).ToList<SysSupplier>();
        }
        public List<SysSupplier> GetSupplierList(string supplier)
        {
            List<Sys_Supplier> searchList = _context.Sys_Supplier.Where(m => !m.IsDelete && m.SupplierName.StartsWith(supplier)).ToList();
            return searchList.Select(c => SetToBussinessObjectSupplier(c)).ToList<SysSupplier>();
        }
        public IEnumerable<string> GetSupplierListForSearch()
        {
            return _context.Sys_Supplier.Where(m => !m.IsDelete).Select(m => m.SupplierName).ToList();
        }
        public List<string> GetChemicalSupplierListForSearch()
        {
            return _context.Sys_Supplier.Where(m => !m.IsDelete && m.IsActive && m.SupplierCategory == "Chemical").Select(m => m.SupplierName).ToList();
        }
        public SysSupplier GetSupplierAddressAndAgentList(string supplierId)
        {
            SysSupplier sysSupplier = new SysSupplier();
            IList<SysSupplierAddress> supplierAddressList = new List<SysSupplierAddress>();
            IList<SysSupplierAgent> supplierAgentList = new List<SysSupplierAgent>();

            var supplierid = string.IsNullOrEmpty(supplierId) ? 0 : Convert.ToInt32(supplierId);
            List<Sys_SupplierAddress> searchList = _context.Sys_SupplierAddress.Where(m => m.SupplierID == supplierid && !m.IsDelete).ToList();
            foreach (var supplierAddress in searchList)
            {
                var supaddlist = SetToBussinessObject(supplierAddress);
                supplierAddressList.Add(supaddlist);
            }

            List<Sys_SupplierAgent> supplierAgentEnList = _context.Sys_SupplierAgent.Where(m => m.SupplierID == supplierid && !m.IsDelete).ToList();
            foreach (var supplierAgent in supplierAgentEnList)
            {
                var supAgnt = SetToBussinessObject(supplierAgent);
                supplierAgentList.Add(supAgnt);
            }

            sysSupplier.SupplierAddressList = supplierAddressList;
            sysSupplier.SupplierAgentList = supplierAgentList;
            return sysSupplier;
        }
        public SysSupplier SetToBussinessObjectSupplier(Sys_Supplier Entity)
        {
            SysSupplier Model = new SysSupplier();

            Model.SupplierID = Entity.SupplierID;
            Model.SupplierCode = Entity.SupplierCode;
            Model.SupplierName = Entity.SupplierName;
            Model.SupplierCategory = Entity.SupplierCategory;
            Model.SupplierType = Entity.SupplierType;
            Model.CountryID = Entity.CountryID;
            Model.CountryName = Entity.CountryID == null
                ? ""
                : _context.Sys_Country.Where(m => m.CountryID == Entity.CountryID).FirstOrDefault().CountryName;
            Model.IsActive = Entity.IsActive ? "Active" : "Inactive";
            var abc =
                _context.Sys_SupplierAddress.Where(
                    q => q.SupplierID == Entity.SupplierID && q.IsActive.Equals(true) && q.IsDelete.Equals(false))
                    .FirstOrDefault();
            if (abc != null)
            {
                Model.SupplierAddressID = _context.Sys_SupplierAddress.Where(q => q.SupplierID == Entity.SupplierID && q.IsActive.Equals(true) && q.IsDelete.Equals(false)).FirstOrDefault().SupplierAddressID;
                Model.Address = _context.Sys_SupplierAddress.Where(q => q.SupplierID == Entity.SupplierID && q.IsActive.Equals(true) && q.IsDelete.Equals(false)).FirstOrDefault().Address;
            }
            return Model;
        }
        public SysSupplier SetToBussinessObjectSupplierForSearch(Sys_Supplier Entity)
        {
            SysSupplier Model = new SysSupplier();

            //Model.SupplierID = Entity.SupplierID;
            //Model.SupplierCode = Entity.SupplierCode;
            Model.SupplierName = Entity.SupplierName;
            //Model.SupplierCategory = Entity.SupplierCategory;
            //Model.SupplierType = Entity.SupplierType;
            //Model.IsActive = Entity.IsActive ? "Active" : "Inactive";

            return Model;
        }
        public SysSupplierAddress SetToBussinessObject(Sys_SupplierAddress Entity)
        {
            SysSupplierAddress Model = new SysSupplierAddress();

            Model.SupplierAddressID = Entity.SupplierAddressID;
            Model.SupplierID = Entity.SupplierID;
            Model.Address = Entity.Address;
            Model.ContactPerson = Entity.ContactPerson;
            Model.ContactNumber = Entity.ContactNumber;
            Model.EmailAddress = Entity.EmailAddress;
            Model.FaxNo = Entity.FaxNo;
            Model.PhoneNo = Entity.PhoneNo;
            Model.SkypeID = Entity.SkypeID;
            Model.Website = Entity.Website;
            Model.IsActive = Entity.IsActive ? "Active" : "Inactive";

            return Model;
        }
        public SysSupplierAgent SetToBussinessObject(Sys_Supplier Entity)
        {
            SysSupplierAgent Model = new SysSupplierAgent();

            Model.SupplierID = Entity.SupplierID;
            Model.SupplierName = Entity.SupplierName;
            Model.SupplierCode = Entity.SupplierCode;
            Model.AgentType = Entity.SupplierType;
            Model.IsActive = Entity.IsActive ? "Active" : "Inactive";
            return Model;
        }
        public SysSupplierAgent SetToBussinessObject(Sys_SupplierAgent Entity)
        {
            SysSupplierAgent Model = new SysSupplierAgent();

            Model.SupplierAgentID = Entity.SupplierAgentID;
            Model.AgentID = Entity.AgentID;
            Model.SupplierID = Entity.SupplierID;
            Model.SupplierName = _context.Sys_Supplier.Where(m => m.SupplierID == Entity.AgentID).FirstOrDefault().SupplierName;
            Model.AgentType = Entity.AgentType;
            Model.IsActive = Entity.IsActive ? "Active" : "Inactive";

            return Model;
        }
        public String GetSupplierBySupplierId(int id)
        {
            Sys_Supplier data = (from temp in _context.Sys_Supplier where temp.SupplierID == id select temp).FirstOrDefault();
            return data.SupplierName;
        }
    }
}