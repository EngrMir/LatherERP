using System;
using System.Collections.Generic;
using System.Linq;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using System.Transactions;

namespace ERP.DatabaseAccessLayer.AppSetupGateway
{
    public class DalSysBuyer
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;

        public int BuyerID = 0;

        private int buyeraddress = 0;
        private int localAgent = 0;
        private int foreignAgent = 0;

        public DalSysBuyer()
        {
            _context = new BLC_DEVEntities();
        }

        public ValidationMsg Save(SysBuyer model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        var exitBuyerCode = _context.Sys_Buyer.Where(m => m.BuyerCode == model.BuyerCode).ToList();
                        if (exitBuyerCode.Count > 0)
                        {
                            _vmMsg.Type = Enums.MessageType.Error;
                            _vmMsg.Msg = "Buyer Code Already Exit.";
                        }
                        else
                        {
                            #region Buyer

                            model.SetBy = userid;
                            Sys_Buyer tblSysBuyer = SetToModelObject(model);
                            _context.Sys_Buyer.Add(tblSysBuyer);
                            _context.SaveChanges();

                            BuyerID = tblSysBuyer.BuyerID;

                            #endregion

                            #region Save Buyer Address

                            if (model.BuyerAddressList != null)
                            {
                                if (model.BuyerAddressList.Count > 1)
                                {
                                    buyeraddress = 1;
                                }
                                else
                                {
                                    foreach (SysBuyerAddress objSysBuyerAddress in model.BuyerAddressList)
                                    {
                                        objSysBuyerAddress.BuyerID = BuyerID;
                                        objSysBuyerAddress.SetBy = userid;
                                        objSysBuyerAddress.IsActive = string.IsNullOrEmpty(objSysBuyerAddress.IsActive) ? "Active" : objSysBuyerAddress.IsActive;
                                        Sys_BuyerAddress tblBuyerAddress = SetToModelObject(objSysBuyerAddress);
                                        _context.Sys_BuyerAddress.Add(tblBuyerAddress);
                                    }

                                    #region Save Buyer Agent

                                    if (model.BuyerAgentList != null)
                                    {
                                        foreach (SysBuyerAgent objBuyerAgent in model.BuyerAgentList)
                                        {
                                            if (objBuyerAgent.BuyerID != 0)
                                            {
                                                if (objBuyerAgent.AgentType == "Local Agent")
                                                {
                                                    ++localAgent;
                                                }
                                                else if (objBuyerAgent.AgentType == "Foreign Agent")
                                                {
                                                    ++foreignAgent;
                                                }
                                                if (localAgent == 1 || foreignAgent == 1)
                                                {
                                                    objBuyerAgent.BuyerAgentID = BuyerID;
                                                    objBuyerAgent.BuyerID = BuyerID;
                                                    objBuyerAgent.SetBy = userid;
                                                    objBuyerAgent.IsActive = string.IsNullOrEmpty(objBuyerAgent.IsActive) ? "Active" : objBuyerAgent.IsActive;
                                                    Sys_BuyerAgent tblSysBuyerAgent = SetToModelObject(objBuyerAgent);
                                                    _context.Sys_BuyerAgent.Add(tblSysBuyerAgent);
                                                }
                                            }
                                        }
                                    }

                                    #endregion

                                    #region Transaction

                                    if (buyeraddress == 1)
                                    {
                                        _vmMsg.Type = Enums.MessageType.Error;
                                        _vmMsg.Msg = "Please Enter only one Active Buyer Address.";
                                    }

                                    if (localAgent > 1 || foreignAgent > 1)
                                    {
                                        _vmMsg.Type = Enums.MessageType.Error;
                                        _vmMsg.Msg = "Please Enter Same Agent Type Only Once.";
                                    }

                                    if (buyeraddress == 0 && localAgent == 1 && foreignAgent == 1)
                                    {
                                        _context.SaveChanges();
                                        tx.Complete();
                                        _vmMsg.Type = Enums.MessageType.Success;
                                        _vmMsg.Msg = "Saved Successfully.";
                                    }
                                    if (buyeraddress == 0 && localAgent == 0 && foreignAgent == 1)
                                    {
                                        _context.SaveChanges();
                                        tx.Complete();
                                        _vmMsg.Type = Enums.MessageType.Success;
                                        _vmMsg.Msg = "Saved Successfully.";
                                    }
                                    if (buyeraddress == 0 && localAgent == 1 && foreignAgent == 0)
                                    {
                                        _context.SaveChanges();
                                        tx.Complete();
                                        _vmMsg.Type = Enums.MessageType.Success;
                                        _vmMsg.Msg = "Saved Successfully.";
                                    }
                                    if (buyeraddress == 0 && localAgent == 0 && foreignAgent == 0)
                                    {
                                        _context.SaveChanges();
                                        tx.Complete();
                                        _vmMsg.Type = Enums.MessageType.Success;
                                        _vmMsg.Msg = "Saved Successfully.";
                                    }

                                    #endregion
                                }
                            }
                            else
                            {
                                _vmMsg.Type = Enums.MessageType.Error;
                                _vmMsg.Msg = "Please Enter Buyer Address.";
                            }

                            #endregion
                        }

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

        public int GetBuyerId()
        {
            return BuyerID;
        }

        public ValidationMsg Update(SysBuyer model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        Sys_Buyer CurrentEntity = SetToModelObject(model);
                        var OriginalEntity = _context.Sys_Buyer.First(m => m.BuyerID == model.BuyerID);

                        OriginalEntity.BuyerID = CurrentEntity.BuyerID;
                        OriginalEntity.BuyerCode = CurrentEntity.BuyerCode;
                        OriginalEntity.BuyerName = CurrentEntity.BuyerName;
                        OriginalEntity.BuyerCategory = CurrentEntity.BuyerCategory;
                        OriginalEntity.BuyerType = CurrentEntity.BuyerType;
                        OriginalEntity.IsActive = CurrentEntity.IsActive;
                        OriginalEntity.ModifiedBy = userid;
                        OriginalEntity.ModifiedOn = DateTime.Now;

                        //#region Save Buyer Address

                        //if (model.BuyerAddressList != null)
                        //{
                        //    foreach (SysBuyerAddress objbuyerAddress in model.BuyerAddressList)
                        //    {
                        //        if (objbuyerAddress.BuyerAddressID == 0)
                        //        {
                        //            var exitData = _context.Sys_BuyerAddress.Where(m => m.BuyerID == model.BuyerID && m.IsActive == true).ToList();
                        //            if (exitData.Count > 0)
                        //            {
                        //                buyeraddress = 1;
                        //            }
                        //            else
                        //            {
                        //                objbuyerAddress.BuyerID = model.BuyerID;
                        //                objbuyerAddress.SetBy = userid;
                        //                objbuyerAddress.IsActive = string.IsNullOrEmpty(objbuyerAddress.IsActive) ? "Active" : objbuyerAddress.IsActive;
                        //                Sys_BuyerAddress tblPurchaseYearPeriod =
                        //                    SetToModelObject(objbuyerAddress);
                        //                _context.Sys_BuyerAddress.Add(tblPurchaseYearPeriod);
                        //                _context.SaveChanges();
                        //            }
                        //        }
                        //        else
                        //        {
                        //            if (objbuyerAddress.IsActive == "Inactive")
                        //            {
                        //                Sys_BuyerAddress CurEntity = SetToModelObject(objbuyerAddress);
                        //                var OrgEntity =
                        //                    _context.Sys_BuyerAddress.First(
                        //                        m => m.BuyerAddressID == objbuyerAddress.BuyerAddressID);

                        //                OrgEntity.BuyerAddressID = CurEntity.BuyerAddressID;
                        //                OrgEntity.BuyerID = CurEntity.BuyerID;
                        //                OrgEntity.Address = CurEntity.Address;
                        //                OrgEntity.ContactPerson = CurEntity.ContactPerson;
                        //                OrgEntity.ContactNumber = CurEntity.ContactNumber;
                        //                OrgEntity.EmailAddress = CurEntity.EmailAddress;
                        //                OrgEntity.FaxNo = CurEntity.FaxNo;
                        //                OrgEntity.PhoneNo = CurEntity.PhoneNo;
                        //                OrgEntity.SkypeID = CurEntity.SkypeID;
                        //                OrgEntity.IsActive = CurEntity.IsActive;
                        //                OrgEntity.ModifiedBy = userid;
                        //                OrgEntity.ModifiedOn = DateTime.Now;
                        //            }
                        //            else
                        //            {
                        //                var exitData = _context.Sys_BuyerAddress.Where(m => m.BuyerID == model.BuyerID && m.IsActive == true).ToList();
                        //                if (exitData.Count > 0)
                        //                {
                        //                    buyeraddress = 1;
                        //                }
                        //                else
                        //                {
                        //                    Sys_BuyerAddress CurEntity = SetToModelObject(objbuyerAddress);
                        //                    var OrgEntity =
                        //                        _context.Sys_BuyerAddress.First(
                        //                            m => m.BuyerAddressID == objbuyerAddress.BuyerAddressID);

                        //                    OrgEntity.BuyerAddressID = CurEntity.BuyerAddressID;
                        //                    OrgEntity.BuyerID = CurEntity.BuyerID;
                        //                    OrgEntity.Address = CurEntity.Address;
                        //                    OrgEntity.ContactPerson = CurEntity.ContactPerson;
                        //                    OrgEntity.ContactNumber = CurEntity.ContactNumber;
                        //                    OrgEntity.EmailAddress = CurEntity.EmailAddress;
                        //                    OrgEntity.FaxNo = CurEntity.FaxNo;
                        //                    OrgEntity.PhoneNo = CurEntity.PhoneNo;
                        //                    OrgEntity.SkypeID = CurEntity.SkypeID;
                        //                    OrgEntity.IsActive = CurEntity.IsActive;
                        //                    OrgEntity.ModifiedBy = userid;
                        //                    OrgEntity.ModifiedOn = DateTime.Now;
                        //                }
                        //            }
                        //        }
                        //    }
                        //}

                        //#endregion

                        #region Save Buyer Address

                        if (model.BuyerAddressList != null)
                        {
                            foreach (SysBuyerAddress objbuyerAddress in model.BuyerAddressList)
                            {
                                if (objbuyerAddress.BuyerAddressID == 0)
                                {
                                    var exitData = _context.Sys_BuyerAddress.Where(m => m.BuyerID == model.BuyerID && m.IsActive == true).ToList();
                                    if (exitData.Count > 0)
                                    {
                                        buyeraddress = 1;
                                    }
                                    else
                                    {
                                        objbuyerAddress.BuyerID = model.BuyerID;
                                        objbuyerAddress.SetBy = userid;
                                        objbuyerAddress.IsActive = string.IsNullOrEmpty(objbuyerAddress.IsActive) ? "Active" : objbuyerAddress.IsActive;
                                        Sys_BuyerAddress tblPurchaseYearPeriod = SetToModelObject(objbuyerAddress);
                                        _context.Sys_BuyerAddress.Add(tblPurchaseYearPeriod);
                                        _context.SaveChanges();
                                    }
                                }
                                else
                                {
                                    if (objbuyerAddress.IsActive == "Inactive")
                                    {
                                        Sys_BuyerAddress CurEntity = SetToModelObject(objbuyerAddress);
                                        var OrgEntity = _context.Sys_BuyerAddress.First(m => m.BuyerAddressID == objbuyerAddress.BuyerAddressID);

                                        OrgEntity.BuyerAddressID = CurEntity.BuyerAddressID;
                                        OrgEntity.BuyerID = CurEntity.BuyerID;
                                        OrgEntity.Address = CurEntity.Address;
                                        OrgEntity.ContactPerson = CurEntity.ContactPerson;
                                        OrgEntity.ContactNumber = CurEntity.ContactNumber;
                                        OrgEntity.EmailAddress = CurEntity.EmailAddress;
                                        OrgEntity.FaxNo = CurEntity.FaxNo;
                                        OrgEntity.PhoneNo = CurEntity.PhoneNo;
                                        OrgEntity.SkypeID = CurEntity.SkypeID;
                                        OrgEntity.IsActive = CurEntity.IsActive;
                                        OrgEntity.ModifiedBy = userid;
                                        OrgEntity.ModifiedOn = DateTime.Now;
                                    }
                                    else
                                    {
                                        var exitData = _context.Sys_BuyerAddress.Where(m => m.BuyerID == model.BuyerID && m.IsActive == true).ToList();
                                        if ((exitData.Count > 0) && (model.BuyerAddressList.Count > 1))
                                        //if (exitData.Count > 0)
                                        {
                                            buyeraddress = 1;
                                        }
                                        else
                                        {
                                            Sys_BuyerAddress CurEntity = SetToModelObject(objbuyerAddress);
                                            var OrgEntity = _context.Sys_BuyerAddress.First(m => m.BuyerAddressID == objbuyerAddress.BuyerAddressID);

                                            OrgEntity.BuyerAddressID = CurEntity.BuyerAddressID;
                                            OrgEntity.BuyerID = CurEntity.BuyerID;
                                            OrgEntity.Address = CurEntity.Address;
                                            OrgEntity.ContactPerson = CurEntity.ContactPerson;
                                            OrgEntity.ContactNumber = CurEntity.ContactNumber;
                                            OrgEntity.EmailAddress = CurEntity.EmailAddress;
                                            OrgEntity.FaxNo = CurEntity.FaxNo;
                                            OrgEntity.PhoneNo = CurEntity.PhoneNo;
                                            OrgEntity.SkypeID = CurEntity.SkypeID;
                                            OrgEntity.IsActive = CurEntity.IsActive;
                                            OrgEntity.ModifiedBy = userid;
                                            OrgEntity.ModifiedOn = DateTime.Now;
                                        }
                                    }
                                }
                            }
                        }

                        #endregion

                        #region Save Buyer Agent

                        if (model.BuyerAgentList != null)
                        {
                            foreach (SysBuyerAgent objBuyerAgent in model.BuyerAgentList)
                            {
                                if (objBuyerAgent.BuyerAgentID == 0)
                                {
                                    if (objBuyerAgent.BuyerID != 0)
                                    {
                                        if (objBuyerAgent.AgentType == "Local Agent")
                                        {
                                            ++localAgent;
                                            var exitData = _context.Sys_BuyerAgent.Where(m => m.BuyerID == model.BuyerID && m.IsActive == true && m.AgentType == "Local Agent").ToList();
                                            if (exitData.Count > 0)
                                            {
                                                ++localAgent;
                                            }
                                        }
                                        else if (objBuyerAgent.AgentType == "Foreign Agent")
                                        {
                                            ++foreignAgent;
                                            var exitData = _context.Sys_BuyerAgent.Where(m => m.BuyerID == model.BuyerID && m.IsActive == true && m.AgentType == "Foreign Agent").ToList();
                                            if (exitData.Count > 0)
                                            {
                                                ++foreignAgent;
                                            }
                                        }
                                        if (localAgent == 1 || foreignAgent == 1)
                                        {
                                            objBuyerAgent.BuyerID = model.BuyerID;
                                            objBuyerAgent.SetBy = userid;
                                            objBuyerAgent.IsActive = string.IsNullOrEmpty(objBuyerAgent.IsActive) ? "Active" : objBuyerAgent.IsActive;
                                            Sys_BuyerAgent tblSysBuyerAgent = SetToModelObject(objBuyerAgent);
                                            _context.Sys_BuyerAgent.Add(tblSysBuyerAgent);
                                        }
                                    }
                                }
                                else
                                {
                                    if (objBuyerAgent.AgentType == "Local Agent")
                                    {
                                        ++localAgent;
                                    }
                                    else if (objBuyerAgent.AgentType == "Foreign Agent")
                                    {
                                        ++foreignAgent;
                                    }
                                    if (localAgent == 1 || foreignAgent == 1)
                                    {
                                        Sys_BuyerAgent CurEntity = SetToModelObject(objBuyerAgent);
                                        var OrgEntity =
                                            _context.Sys_BuyerAgent.First(
                                                m => m.BuyerAgentID == objBuyerAgent.BuyerAgentID);

                                        OrgEntity.BuyerAgentID = CurEntity.BuyerAgentID;
                                        OrgEntity.AgentID = CurEntity.AgentID;
                                        OrgEntity.BuyerID = CurEntity.BuyerID;
                                        OrgEntity.AgentType = CurEntity.AgentType;
                                        OrgEntity.IsActive = CurEntity.IsActive;
                                        OrgEntity.ModifiedBy = userid;
                                        OrgEntity.ModifiedOn = DateTime.Now;
                                    }
                                }
                            }
                        }

                        #endregion

                        if (buyeraddress == 1)
                        {
                            _vmMsg.Type = Enums.MessageType.Error;
                            _vmMsg.Msg = "Please Enter only one Active Buyer Address.";
                        }

                        if (localAgent > 1 || foreignAgent > 1)
                        {
                            _vmMsg.Type = Enums.MessageType.Error;
                            _vmMsg.Msg = "Please Enter Same Agent Type Only Once.";
                        }

                        if (buyeraddress == 0 && localAgent == 1 && foreignAgent == 1)
                        {
                            _context.SaveChanges();
                            tx.Complete();
                            _vmMsg.Type = Enums.MessageType.Update;
                            _vmMsg.Msg = "Updated Successfully.";
                        }

                        if (buyeraddress == 0 && localAgent == 0 && foreignAgent == 1)
                        {
                            _context.SaveChanges();
                            tx.Complete();
                            _vmMsg.Type = Enums.MessageType.Update;
                            _vmMsg.Msg = "Updated Successfully.";
                        }

                        if (buyeraddress == 0 && localAgent == 1 && foreignAgent == 0)
                        {
                            _context.SaveChanges();
                            tx.Complete();
                            _vmMsg.Type = Enums.MessageType.Update;
                            _vmMsg.Msg = "Updated Successfully.";
                        }
                        if (buyeraddress == 0 && localAgent == 0 && foreignAgent == 0)
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
                    _vmMsg.Msg = "Buyer Code Already Exit..";
                }
                else
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Failed to Update.";
                }
            }
            return _vmMsg;
        }

        public Sys_Buyer SetToModelObject(SysBuyer model)
        {
            Sys_Buyer Entity = new Sys_Buyer();

            Entity.BuyerID = model.BuyerID;
            Entity.BuyerCode = model.BuyerCode;
            Entity.BuyerName = model.BuyerName;
            Entity.BuyerCategory = model.BuyerCategory;
            Entity.BuyerType = model.BuyerType;
            Entity.IsActive = model.IsActive == "Active";
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = model.SetBy;
            Entity.IPAddress = string.Empty;
            return Entity;
        }

        public Sys_BuyerAddress SetToModelObject(SysBuyerAddress model)
        {
            Sys_BuyerAddress Entity = new Sys_BuyerAddress();

            Entity.BuyerAddressID = model.BuyerAddressID;
            Entity.BuyerID = model.BuyerID;
            Entity.Address = model.Address;
            Entity.ContactPerson = model.ContactPerson;
            Entity.ContactNumber = model.ContactNumber;
            Entity.EmailAddress = model.EmailAddress;
            Entity.FaxNo = model.FaxNo;
            Entity.PhoneNo = model.PhoneNo;
            Entity.SkypeID = model.SkypeID;
            Entity.IsActive = model.IsActive == "Active";
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = model.SetBy;
            Entity.IPAddress = string.Empty;
            return Entity;
        }

        public Sys_BuyerAgent SetToModelObject(SysBuyerAgent model)
        {
            Sys_BuyerAgent Entity = new Sys_BuyerAgent();

            Entity.BuyerAgentID = model.BuyerAgentID;
            Entity.AgentID = model.AgentID;
            Entity.BuyerID = model.BuyerID;
            Entity.AgentType = model.AgentType;
            Entity.IsActive = model.IsActive == "Active";
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = model.SetBy;
            Entity.IPAddress = string.Empty;
            return Entity;
        }

        public ValidationMsg Delete(string buyerId)
        {
            var buyerid = string.IsNullOrEmpty(buyerId) ? 0 : Convert.ToInt32(buyerId);
            var vmMsg = new ValidationMsg();
            try
            {
                var buyerAddressList = _context.Sys_BuyerAddress.Where(s => s.BuyerID == buyerid && s.IsActive == true).ToList();
                var buyerAgentList = _context.Sys_BuyerAgent.Where(s => s.BuyerID == buyerid && s.IsActive == true).ToList();

                if (buyerAddressList.Count > 0 || buyerAgentList.Count > 0)
                {
                    vmMsg.Type = Enums.MessageType.Error;
                    vmMsg.Msg = "Child Record Found.";
                }
                else
                {
                    var sysBuyer = _context.Sys_Buyer.FirstOrDefault(s => s.BuyerID == buyerid);
                    _context.Sys_Buyer.Remove(sysBuyer);
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

        public ValidationMsg DeletedAddress(string buyerAddressId)
        {
            var buyeraddressid = string.IsNullOrEmpty(buyerAddressId) ? 0 : Convert.ToInt32(buyerAddressId);
            var vmMsg = new ValidationMsg();
            try
            {
                var sysBuyerAddress = _context.Sys_BuyerAddress.FirstOrDefault(s => s.BuyerAddressID == buyeraddressid);
                _context.Sys_BuyerAddress.Remove(sysBuyerAddress);
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

        public ValidationMsg DeletedAgent(string buyerAgentId)
        {
            var buyeragentid = string.IsNullOrEmpty(buyerAgentId) ? 0 : Convert.ToInt32(buyerAgentId);
            var vmMsg = new ValidationMsg();
            try
            {
                var sysBuyerAgent = _context.Sys_BuyerAgent.FirstOrDefault(s => s.BuyerAgentID == buyeragentid);
                _context.Sys_BuyerAgent.Remove(sysBuyerAgent);
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

        public IEnumerable<SysBuyer> GetAll()
        {
            IEnumerable<SysBuyer> iLstSysBuyer = from slt in _context.Sys_Buyer
                                                 //where slt.IsActive == true
                                                 select new SysBuyer
                                                 {
                                                     BuyerID = slt.BuyerID,
                                                     BuyerCode = slt.BuyerCode,
                                                     BuyerName = slt.BuyerName,
                                                     BuyerCategory = slt.BuyerCategory,
                                                     BuyerType = slt.BuyerType,
                                                 };

            return iLstSysBuyer;
        }

        //public List<BuyerDetails> GetAllBuyer()
        //{
        //    var AllData = (from s in _context.Sys_Buyer
        //                   where s.IsActive.Equals(true) && s.IsDelete.Equals(false)
        //                   join sa in _context.Sys_BuyerAddress.Where(q => q.IsActive.Equals(true) && q.IsDelete.Equals(false))
        //                   on s.BuyerID equals sa.BuyerID into temp
        //                   from item in temp.DefaultIfEmpty()
        //                   select new BuyerDetails
        //                   {
        //                       BuyerID = s.BuyerID,
        //                       BuyerCode = s.BuyerCode,
        //                       BuyerName = s.BuyerName.ToUpper(),
        //                       BuyerAddressID = (item.BuyerAddressID).ToString(),
        //                       Address = item.Address,
        //                       ContactNumber = item.ContactNumber,
        //                       ContactPerson = item.ContactPerson
        //                   }).ToList();


        //    return AllData;
        //}

        //public List<BuyerDetails> GetBuyerList()
        //{
        //    var AllData = (from s in _context.Sys_Buyer
        //                   where !s.IsDelete
        //                   join sa in _context.Sys_BuyerAddress on s.BuyerID equals sa.BuyerID into temp
        //                   from item in temp.DefaultIfEmpty()
        //                   select new BuyerDetails
        //                   {
        //                       BuyerID = s.BuyerID,
        //                       BuyerName = s.BuyerName,
        //                       BuyerAddressID = item.BuyerAddressID.ToString(),
        //                       Address = item.Address
        //                   }).ToList();


        //    return AllData;
        //}

        //public List<PrqBuyerBill> GetBillRefList(int buyerId)
        //{
        //    var AllData = (from s in _context.Prq_BuyerBill.AsEnumerable()
        //                   where s.BuyerID == buyerId && s.RecordStatus == "NCF"
        //                   select new PrqBuyerBill
        //                   {
        //                       BuyerBillID = s.BuyerBillID,
        //                       BuyerBillNo = s.BuyerBillNo
        //                   }).ToList();
        //    return AllData;
        //}

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

        public string GetBuyerName(int buyerID)
        {
            var query = "SELECT BuyerName FROM [dbo].[Sys_Buyer]" +
                                "WHERE BuyerID =" + buyerID;

            var buyerName = _context.Database.SqlQuery<string>(query).ToString();
            return buyerName;
        }

        public List<SysBuyerAgent> GetBuyerAgentList()
        {
            List<Sys_Buyer> searchList = _context.Sys_Buyer.Where(m => m.BuyerType == "Local Agent" || m.BuyerType == "Foreign Agent").ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<SysBuyerAgent>();
        }

        public List<SysBuyerAgent> GetBuyerAgentSearchList(string buyer)
        {
            List<Sys_Buyer> searchList = _context.Sys_Buyer.Where(m => m.IsActive == true).Where(m => m.BuyerType == "Local Agent" || m.BuyerType == "Foreign Agent").Where(m => m.BuyerName.StartsWith(buyer)).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<SysBuyerAgent>();
        }

        public List<SysBuyer> GetBuyerForSearchList()
        {
            List<Sys_Buyer> searchList = _context.Sys_Buyer.Where(m => m.BuyerCategory == "Buyer" || m.BuyerCategory == "Forwarder" || m.BuyerCategory == "Shipper").ToList();
            return searchList.Select(c => SetToBussinessObjectBuyer(c)).ToList<SysBuyer>();
        }

        public List<SysBuyer> GetBuyerList(string buyer)
        {
            List<Sys_Buyer> searchList = _context.Sys_Buyer.Where(m => m.BuyerName.StartsWith(buyer)).ToList();
            return searchList.Select(c => SetToBussinessObjectBuyer(c)).ToList<SysBuyer>();
        }

        public List<string> GetBuyerListForSearch()
        {
            return _context.Sys_Buyer.Select(m => m.BuyerName).ToList();
        }

        public List<string> GetBuyerAgentListForSearch()
        {
            return _context.Sys_Buyer.Where(m => m.BuyerCategory == "Buyer Agent").Select(m => m.BuyerName).ToList();
        }

        public SysBuyer GetBuyerAddressAndAgentList(string buyerId)
        {
            SysBuyer sysBuyer = new SysBuyer();
            IList<SysBuyerAddress> buyerAddressList = new List<SysBuyerAddress>();
            IList<SysBuyerAgent> buyerAgentList = new List<SysBuyerAgent>();

            var buyerid = string.IsNullOrEmpty(buyerId) ? 0 : Convert.ToInt32(buyerId);
            List<Sys_BuyerAddress> searchList = _context.Sys_BuyerAddress.Where(m => m.BuyerID == buyerid).ToList();
            foreach (var buyerAddress in searchList)
            {
                var supaddlist = SetToBussinessObject(buyerAddress);
                buyerAddressList.Add(supaddlist);
            }

            List<Sys_BuyerAgent> buyerAgentEnList = _context.Sys_BuyerAgent.Where(m => m.BuyerID == buyerid).ToList();
            foreach (var buyerAgent in buyerAgentEnList)
            {
                var supAgnt = SetToBussinessObject(buyerAgent);
                buyerAgentList.Add(supAgnt);
            }

            sysBuyer.BuyerAddressList = buyerAddressList;
            sysBuyer.BuyerAgentList = buyerAgentList;
            return sysBuyer;
        }

        public List<SysBuyer> GetAllActiveBuyer()
        {
            List<Sys_Buyer> searchList = _context.Sys_Buyer.Where(m => m.IsActive == true).ToList();
            return searchList.Select(c => SetToBussinessObjectBuyer(c)).ToList<SysBuyer>();
        }
        public List<SysBuyer> GetAllActiveBuyerByCategory()
        {
            List<Sys_Buyer> searchList = _context.Sys_Buyer.Where(m => m.IsActive == true && m.BuyerCategory == "Buyer").ToList();
            return searchList.Select(c => SetToBussinessObjectBuyer(c)).ToList<SysBuyer>();
        }

        public SysBuyer SetToBussinessObjectBuyer(Sys_Buyer Entity)
        {
            SysBuyer Model = new SysBuyer();

            Model.BuyerID = Entity.BuyerID;
            Model.BuyerCode = Entity.BuyerCode;
            Model.BuyerName = Entity.BuyerName;
            Model.BuyerCategory = Entity.BuyerCategory;
            Model.BuyerType = Entity.BuyerType;
            Model.IsActive = Entity.IsActive == true ? "Active" : "Inactive";
            Model.Address = _context.Sys_BuyerAddress.Where(q => q.BuyerID == Entity.BuyerID && q.IsActive == true).FirstOrDefault() == null ? null : _context.Sys_BuyerAddress.Where(q => q.BuyerID == Entity.BuyerID && q.IsActive == true).FirstOrDefault().Address;

            return Model;
        }

        public SysBuyer SetToBussinessObjectBuyerForSearch(Sys_Buyer Entity)
        {
            SysBuyer Model = new SysBuyer();

            Model.BuyerID = Entity.BuyerID;
            Model.BuyerCode = Entity.BuyerCode;
            Model.BuyerName = Entity.BuyerName;
            Model.BuyerCategory = Entity.BuyerCategory;
            Model.BuyerType = Entity.BuyerType;
            Model.IsActive = Entity.IsActive == true ? "Active" : "Inactive";

            return Model;
        }

        public SysBuyerAddress SetToBussinessObject(Sys_BuyerAddress Entity)
        {
            SysBuyerAddress Model = new SysBuyerAddress();

            Model.BuyerAddressID = Entity.BuyerAddressID;
            Model.BuyerID = Entity.BuyerID;
            Model.Address = Entity.Address;
            Model.ContactPerson = Entity.ContactPerson;
            Model.ContactNumber = Entity.ContactNumber;
            Model.EmailAddress = Entity.EmailAddress;
            Model.FaxNo = Entity.FaxNo;
            Model.PhoneNo = Entity.PhoneNo;
            Model.SkypeID = Entity.SkypeID;
            Model.IsActive = Entity.IsActive == true ? "Active" : "Inactive";

            return Model;
        }

        public SysBuyerAgent SetToBussinessObject(Sys_Buyer Entity)
        {
            SysBuyerAgent Model = new SysBuyerAgent();

            Model.BuyerID = Entity.BuyerID;
            Model.BuyerName = Entity.BuyerName;
            Model.BuyerCode = Entity.BuyerCode;
            Model.AgentType = Entity.BuyerType;
            Model.IsActive = Entity.IsActive == true ? "Active" : "Inactive";
            return Model;
        }

        public SysBuyerAgent SetToBussinessObject(Sys_BuyerAgent Entity)
        {
            SysBuyerAgent Model = new SysBuyerAgent();

            Model.BuyerAgentID = Entity.BuyerAgentID;
            Model.AgentID = Entity.AgentID;
            Model.BuyerID = Entity.BuyerID;
            Model.BuyerName = _context.Sys_Buyer.Where(m => m.BuyerID == Entity.AgentID).FirstOrDefault().BuyerName;
            Model.AgentType = Entity.AgentType;
            Model.IsActive = Entity.IsActive == true ? "Active" : "Inactive";

            return Model;
        }

        public String GetBuyerByBuyerId(int id)
        {
            Sys_Buyer data = (from temp in _context.Sys_Buyer where temp.BuyerID == id select temp).FirstOrDefault();
            return data.BuyerName;
        }
    }
}
