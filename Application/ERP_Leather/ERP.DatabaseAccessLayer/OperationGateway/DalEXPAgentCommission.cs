using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.AppSetupModel;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using System.Transactions;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalEXPAgentCommission
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;
        public long AgentComID = 0;
        public string AgentComNo = string.Empty;

        public DalEXPAgentCommission()
        {
            _context = new BLC_DEVEntities();
        }

        public ValidationMsg Save(EXPAgentCommission model, int userid, string pageUrl)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                #region Save

                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        model.AgentComNo = DalCommon.GetPreDefineNextCodeByUrl(pageUrl);//DalCommon.GetPreDefineValue("1", "00045");
                        if (model.AgentComNo != null)
                        {
                            #region AgentCommission

                            EXP_AgentCommission tblEXPAgentCommission = SetToModelObject(model, userid);
                            _context.EXP_AgentCommission.Add(tblEXPAgentCommission);
                            _context.SaveChanges();

                            #endregion

                            #region CIPIItemColor

                            if (model.EXPAgentCommissionBuyerCIList != null)
                            {

                                foreach (EXPAgentCommissionBuyerCI objEXPAgentCommissionBuyerCI in model.EXPAgentCommissionBuyerCIList)
                                {
                                    objEXPAgentCommissionBuyerCI.AgentComID = tblEXPAgentCommission.AgentComID;
                                    objEXPAgentCommissionBuyerCI.LocalCurrency = model.LocalCurrency;

                                    EXP_AgentCommissionBuyerCI tblEXPAgentCommissionBuyerCI = SetToModelObject(objEXPAgentCommissionBuyerCI, userid);
                                    _context.EXP_AgentCommissionBuyerCI.Add(tblEXPAgentCommissionBuyerCI);
                                }
                            }
                            #endregion

                            _context.SaveChanges();
                            tx.Complete();
                            AgentComID = tblEXPAgentCommission.AgentComID;
                            AgentComNo = model.AgentComNo;
                            _vmMsg.Type = Enums.MessageType.Success;
                            _vmMsg.Msg = "Saved Successfully.";
                        }
                        else
                        {
                            _vmMsg.Type = Enums.MessageType.Error;
                            _vmMsg.Msg = "AgentComNo Predefine Value not Found.";
                        }
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to save.";
                if (ex.InnerException.InnerException.Message.Contains("UK_EXP_AgentCommission_AgentComNo"))
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Agent Com. No Already Exist.";
                }
                if (ex.InnerException.InnerException.Message.Contains("UK_EXP_AgentCommissionBuyerCI_BuyerAgentID_CIID"))
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Same Agent Already Exist.";
                }
            }
            return _vmMsg;
        }

        public ValidationMsg Update(EXPAgentCommission model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                #region Update

                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        #region AgentCommission

                        EXP_AgentCommission CurrentEntity = SetToModelObject(model, userid);
                        var OriginalEntity = _context.EXP_AgentCommission.First(m => m.AgentComID == model.AgentComID);

                        OriginalEntity.AgentComRef = CurrentEntity.AgentComRef;
                        OriginalEntity.AgentComDate = CurrentEntity.AgentComDate;
                        //OriginalEntity.AgentComCatg = CurrentEntity.AgentComCatg;
                        //OriginalEntity.BuyerAgentID = CurrentEntity.BuyerAgentID;
                        OriginalEntity.ModifiedBy = userid;
                        OriginalEntity.ModifiedOn = DateTime.Now;

                        #endregion

                        #region AgentCommissionBuyerCI

                        if (model.EXPAgentCommissionBuyerCIList != null)
                        {
                            foreach (EXPAgentCommissionBuyerCI objEXPAgentCommissionBuyerCI in model.EXPAgentCommissionBuyerCIList)
                            {
                                if (objEXPAgentCommissionBuyerCI.AgentComCIID == 0)
                                {
                                    objEXPAgentCommissionBuyerCI.AgentComID = model.AgentComID;
                                    EXP_AgentCommissionBuyerCI tblEXPAgentCommissionBuyerCI = SetToModelObject(objEXPAgentCommissionBuyerCI, userid);
                                    _context.EXP_AgentCommissionBuyerCI.Add(tblEXPAgentCommissionBuyerCI);
                                }
                                else
                                {
                                    EXP_AgentCommissionBuyerCI CurEntity = SetToModelObject(objEXPAgentCommissionBuyerCI, userid);
                                    var OrgEntity = _context.EXP_AgentCommissionBuyerCI.First(m => m.AgentComCIID == objEXPAgentCommissionBuyerCI.AgentComCIID);

                                    OrgEntity.BuyerAgentID = CurEntity.BuyerAgentID;
                                    OrgEntity.CIID = CurEntity.CIID;
                                    OrgEntity.RefPIID = CurEntity.RefPIID;
                                    OrgEntity.CommissionPercent = CurEntity.CommissionPercent;
                                    OrgEntity.CommissionAmount = CurEntity.CommissionAmount;
                                    OrgEntity.ExchangeCurrency = CurEntity.ExchangeCurrency;
                                    OrgEntity.ExchangeRate = CurEntity.ExchangeRate;
                                    OrgEntity.ExchangeAmount = CurEntity.ExchangeAmount;
                                    OrgEntity.LocalCurrency = CurEntity.LocalCurrency;
                                    OrgEntity.LocalRate = CurEntity.LocalRate;
                                    OrgEntity.LocalAmount = CurEntity.LocalAmount;
                                    OrgEntity.ModifiedBy = userid;
                                    OrgEntity.ModifiedOn = DateTime.Now;
                                }
                            }
                        }

                        #endregion

                        _context.SaveChanges();
                        tx.Complete();
                        AgentComID = model.AgentComID;
                        AgentComNo = model.AgentComNo;
                        _vmMsg.Type = Enums.MessageType.Update;
                        _vmMsg.Msg = "Updated Successfully.";
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Update.";

                if (ex.InnerException.InnerException.Message.Contains("UK_EXP_AgentCommission_AgentComNo"))
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Agent Com. No Already Exist.";
                }
                if (ex.InnerException.InnerException.Message.Contains("UK_EXP_AgentCommissionBuyerCI_BuyerAgentID_CIID"))
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Same Agent Already Exist.";
                }
            }
            return _vmMsg;
        }

        public EXP_AgentCommission SetToModelObject(EXPAgentCommission model, int userid)
        {
            EXP_AgentCommission Entity = new EXP_AgentCommission();

            Entity.AgentComID = model.AgentComID;
            Entity.AgentComNo = model.AgentComNo;
            Entity.AgentComDate = DalCommon.SetDate(model.AgentComDate);
            Entity.AgentComRef = model.AgentComRef;

            Entity.BuyerID = Convert.ToInt32(model.BuyerID);
            Entity.CIID = model.CIID;

            Entity.CIAmount = model.CIAmount;
            Entity.ComOnAmount = model.ComOnAmount;
            Entity.CIDate = DalCommon.SetDate(model.CIDate);

            Entity.InvoiceCurrency = model.InvoiceCurrency;
            //Entity.LocalCurrency = model.LocalCurrency;
            Entity.LocalCurrency = _context.Sys_Currency.Where(m => m.CurrencyName == "TK").FirstOrDefault().CurrencyID;

            Entity.RecordStatus = "NCF";
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }

        public EXP_AgentCommissionBuyerCI SetToModelObject(EXPAgentCommissionBuyerCI model, int userid)
        {
            EXP_AgentCommissionBuyerCI Entity = new EXP_AgentCommissionBuyerCI();

            Entity.AgentComCIID = model.AgentComCIID;
            Entity.AgentComID = model.AgentComID;
            Entity.BuyerAgentID = model.BuyerAgentID;
            Entity.AgentType = model.AgentType;
            Entity.CIID = model.CIID;
            Entity.RefPIID = model.PIID;
            Entity.CommissionPercent = model.CommissionPercent;
            Entity.CommissionAmount = model.CommissionAmount;
            Entity.AITPercent = model.AITPercent;
            Entity.AITAmount = model.AITAmount;
            Entity.PayableAmount = model.PayableAmount;
            if (string.IsNullOrEmpty(model.ExchangeCurrencyName))
                Entity.ExchangeCurrency = null;
            else
                Entity.ExchangeCurrency = _context.Sys_Currency.Where(m => m.CurrencyName == model.ExchangeCurrencyName).FirstOrDefault().CurrencyID;
            Entity.ExchangeRate = model.ExchangeRate;
            Entity.ExchangeAmount = model.ExchangeAmount;
            Entity.LocalCurrency = model.LocalCurrency;
            Entity.LocalRate = model.LocalRate;
            Entity.LocalAmount = model.LocalAmount;
            Entity.RecordStatus = "NCF";
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }

        public long GetAgentComID()
        {
            return AgentComID;
        }

        public string GetAgentComNo()
        {
            return AgentComNo;
        }

        public ValidationMsg ConfirmedEXPAgentCommission(EXPAgentCommission model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        var originalEntityCI = _context.EXP_AgentCommission.First(m => m.AgentComID == model.AgentComID);
                        originalEntityCI.RecordStatus = "CNF";
                        originalEntityCI.ModifiedBy = userid;
                        originalEntityCI.ModifiedOn = DateTime.Now;

                        //if (model.EXPAgentCommissionBuyerList.Count > 0)
                        //{
                        //    foreach (EXPAgentCommissionBuyer objEXPAgentCommissionBuyer in model.EXPAgentCommissionBuyerList)
                        //    {
                        //        var originalEntityCIPIItem = _context.EXP_AgentCommissionBuyer.First(m => m.AgentComID == objEXPAgentCommissionBuyer.AgentComID && m.BuyerID == objEXPAgentCommissionBuyer.BuyerID);
                        //        originalEntityCIPIItem.RecordStatus = "CNF";
                        //        originalEntityCIPIItem.ModifiedBy = userid;
                        //        originalEntityCIPIItem.ModifiedOn = DateTime.Now;
                        //    }
                        //}

                        _context.SaveChanges();

                        tx.Complete();
                        _vmMsg.Type = Enums.MessageType.Success;
                        _vmMsg.Msg = "Confirmed Successfully.";
                    }
                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Confirmed.";
            }
            return _vmMsg;
        }

        public ValidationMsg CheckedEXPAgentCommission(EXPAgentCommission model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        var originalEntityCI = _context.EXP_AgentCommission.First(m => m.AgentComID == model.AgentComID);
                        originalEntityCI.RecordStatus = "CHK";
                        originalEntityCI.ModifiedBy = userid;
                        originalEntityCI.ModifiedOn = DateTime.Now;

                        if (model.EXPAgentCommissionBuyerList.Count > 0)
                        {
                            foreach (EXPAgentCommissionBuyer objEXPAgentCommissionBuyer in model.EXPAgentCommissionBuyerList)
                            {
                                var originalEntityCIPIItem = _context.EXP_AgentCommissionBuyer.First(m => m.AgentComID == objEXPAgentCommissionBuyer.AgentComID && m.BuyerID == objEXPAgentCommissionBuyer.BuyerID);
                                originalEntityCIPIItem.RecordStatus = "CHK";
                                originalEntityCIPIItem.ModifiedBy = userid;
                                originalEntityCIPIItem.ModifiedOn = DateTime.Now;
                            }
                        }

                        _context.SaveChanges();

                        tx.Complete();
                        _vmMsg.Type = Enums.MessageType.Success;
                        _vmMsg.Msg = "Checked Successfully.";
                    }
                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Checked.";
            }
            return _vmMsg;
        }

        public List<EXPAgentCommission> GetAgentCommissionInformation()
        {
            List<EXP_AgentCommission> searchList = _context.EXP_AgentCommission.OrderByDescending(m => m.AgentComID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<EXPAgentCommission>();
        }

        public EXPAgentCommission SetToBussinessObject(EXP_AgentCommission Entity)
        {
            EXPAgentCommission model = new EXPAgentCommission();

            model.AgentComID = Entity.AgentComID;
            model.AgentComNo = Entity.AgentComNo;
            model.AgentComDate = Convert.ToDateTime(Entity.AgentComDate).ToString("dd/MM/yyyy");
            model.AgentComRef = Entity.AgentComRef;

            model.BuyerID = Entity.BuyerID;
            model.BuyerName = model.BuyerID == null ? "" : _context.Sys_Buyer.Where(m => m.BuyerID == model.BuyerID).FirstOrDefault().BuyerName;
            model.CIID = Convert.ToInt64(Entity.CIID);
            model.CINo = Entity.CIID == null ? "" : _context.EXP_CI.Where(m => m.CIID == model.CIID).FirstOrDefault().CIRefNo;
            model.CIDate = Convert.ToDateTime(Entity.CIDate).ToString("dd/MM/yyyy");
            model.InvoiceCurrency = Entity.InvoiceCurrency;
            model.InvoiceCurrencyName = model.InvoiceCurrency == null ? "" : _context.Sys_Currency.Where(m => m.CurrencyID == model.InvoiceCurrency).FirstOrDefault().CurrencyName;
            model.CIAmount = Entity.CIAmount;
            model.ComOnAmount = Entity.ComOnAmount;
            model.LocalCurrency = Entity.LocalCurrency;

            model.RecordStatus = Entity.RecordStatus;
            if (Entity.RecordStatus == "NCF")
                model.RecordStatusName = "Not Confirmed";
            else if (Entity.RecordStatus == "CNF")
                model.RecordStatusName = "Confirmed";
            else if (Entity.RecordStatus == "CHK")
                model.RecordStatusName = "Checked";
            else
                model.RecordStatusName = "";

            return model;
        }

        public List<EXPAgentCommissionBuyerCI> GetBuyerAgentList(string CIID, string CIAmount)
        {
            var agentList = new List<EXPAgentCommissionBuyerCI>();
            var query = @"select p.LocalAgent,(select BuyerName from Sys_Buyer where BuyerID = p.LocalAgent)LocalAgentName,
                        p.ForeignAgent,(select BuyerName from Sys_Buyer where BuyerID = p.ForeignAgent)ForeignAgentName,
                        (select BuyerType from Sys_Buyer where BuyerID = p.LocalAgent)LocalAgentType,
						(select BuyerType from Sys_Buyer where BuyerID = p.ForeignAgent)ForeignAgentType,
                        p.LocalComPercent,p.ForeignComPercent,p.PIID,p.PINo from EXP_CIPI c
                        inner join EXP_LeatherPI p on c.PIID = p.PIID
                        where CIID=" + CIID + "";
            var allData = _context.Database.SqlQuery<EXPAgentCommissionBuyerCI>(query).ToList();
            foreach (var obj in allData)
            {
                var objlocAgent = new EXPAgentCommissionBuyerCI();
                if (obj.LocalAgent != null)
                {
                    objlocAgent.BuyerAgentID = obj.LocalAgent;
                    objlocAgent.BuyerAgentName = obj.LocalAgentName;
                    objlocAgent.CommissionPercent = obj.LocalComPercent;
                    if (obj.LocalAgentType == "Local Agent")
                    {
                        objlocAgent.AgentType = obj.LocalAgentType;
                        if ((obj.LocalComPercent != null) && (!string.IsNullOrEmpty(CIAmount)))
                        {
                            decimal cIAmount = Convert.ToDecimal(CIAmount);
                            objlocAgent.CommissionAmount = (obj.LocalComPercent * cIAmount) / 100;
                        }
                    }
                    objlocAgent.PIID = obj.PIID;
                    objlocAgent.PINo = obj.PINo;
                    objlocAgent.ExchangeCurrencyName = "";
                    agentList.Add(objlocAgent);
                }
                var objForAgent = new EXPAgentCommissionBuyerCI();
                if (obj.ForeignAgent != null)
                {
                    objForAgent.BuyerAgentID = obj.ForeignAgent;
                    objForAgent.BuyerAgentName = obj.ForeignAgentName;
                    objForAgent.CommissionPercent = obj.ForeignComPercent;
                    if (obj.ForeignAgentType == "Foreign Agent")
                    {
                        objForAgent.AgentType = obj.ForeignAgentType;
                        if ((obj.ForeignComPercent != null) && (!string.IsNullOrEmpty(CIAmount)))
                        {
                            decimal cIAmount = Convert.ToDecimal(CIAmount);
                            objForAgent.CommissionAmount = (obj.ForeignComPercent * cIAmount) / 100;
                        }
                    }
                    objForAgent.PIID = obj.PIID;
                    objForAgent.PINo = obj.PINo;
                    objForAgent.ExchangeCurrencyName = "";
                    agentList.Add(objForAgent);
                }
            }
            return agentList;
        }

        public List<EXPAgentCommissionBuyerCI> GetPIInfoList(string CIID)
        {
            if (!string.IsNullOrEmpty(CIID))
            {
                var query = @"select EXP_CIPI.PIID,(select PINo from EXP_LeatherPI where PIID = EXP_CIPI.PIID)PINo,
       					EXP_LeatherPI.BuyerOrderID,(select BuyerOrderNo from SLS_BuyerOrder where BuyerOrderID = EXP_LeatherPI.BuyerOrderID)BuyerOrderNo,
       					SLS_BuyerOrder.BuyerID,(select BuyerName from Sys_Buyer where BuyerID = SLS_BuyerOrder.BuyerID)BuyerName
       					from EXP_CIPI
       					inner join EXP_LeatherPI  on EXP_LeatherPI.PIID = EXP_CIPI.PIID
       					inner join SLS_BuyerOrder  on SLS_BuyerOrder.BuyerOrderID = EXP_LeatherPI.BuyerOrderID
                        where CIID=" + CIID + "";
                var allData = _context.Database.SqlQuery<EXPAgentCommissionBuyerCI>(query).ToList();
                return allData;
            }
            return null;
        }

        public List<SysBuyer> GetBuyerAgentPopupList()
        {
            var query = @"select BuyerID,BuyerName,BuyerCategory,BuyerType,
                        (select Address from Sys_BuyerAddress where BuyerID = Sys_Buyer.BuyerID and IsActive=1)Address
                        from Sys_Buyer where BuyerCategory ='Buyer Agent'";
            var allData = _context.Database.SqlQuery<SysBuyer>(query).ToList();
            return allData;
        }

        public List<EXPAgentCommission> GetCINoList()
        {
            List<EXP_CI> searchList = _context.EXP_CI.OrderByDescending(m => m.CIID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<EXPAgentCommission>();
        }

        public EXPAgentCommission SetToBussinessObject(EXP_CI Entity)
        {
            EXPAgentCommission model = new EXPAgentCommission();

            model.CIID = Entity.CIID;
            model.CINo = Entity.CIRefNo;
            model.CIDate = Convert.ToDateTime(Entity.CIDate).ToString("dd/MM/yyyy");
            model.InvoiceCurrency = Entity.CICurrency;
            model.InvoiceCurrencyName = model.InvoiceCurrency == null ? "" : _context.Sys_Currency.Where(m => m.CurrencyID == model.InvoiceCurrency).FirstOrDefault().CurrencyName;
            model.CIAmount = Entity.CIAmount;

            //model.CINo = Entity.CIID == null ? "" : _context.EXP_CI.Where(m => m.CIID == Entity.CIID).FirstOrDefault().CINo;
            if (_context.EXP_CIPI.Where(m => m.CIID == Entity.CIID).FirstOrDefault() != null)
            {
                model.PIID = _context.EXP_CIPI.Where(m => m.CIID == Entity.CIID).FirstOrDefault().PIID;// Entity.PIID;
                model.PINo = model.PIID == null ? "" : _context.EXP_LeatherPI.Where(m => m.PIID == model.PIID).FirstOrDefault().PINo;
                model.BuyerOrderID = model.PIID == null ? null : _context.EXP_LeatherPI.Where(m => m.PIID == model.PIID).FirstOrDefault().BuyerOrderID;
                model.BuyerOrderNo = model.BuyerOrderID == null ? "" : _context.SLS_BuyerOrder.Where(m => m.BuyerOrderID == model.BuyerOrderID).FirstOrDefault().BuyerOrderNo;

                model.BuyerID = model.BuyerOrderID == null ? null : _context.SLS_BuyerOrder.Where(m => m.BuyerOrderID == model.BuyerOrderID).FirstOrDefault().BuyerID;
                model.BuyerName = model.BuyerID == null ? "" : _context.Sys_Buyer.Where(m => m.BuyerID == model.BuyerID).FirstOrDefault().BuyerName;
            }
            //model.BuyerAgentID = Entity.BuyerAgentID;
            //model.AgentID = model.BuyerAgentID == null ? null : _context.Sys_BuyerAgent.Where(m => m.BuyerAgentID == model.BuyerAgentID).FirstOrDefault().AgentID;
            //model.BuyerAgentName = Entity.BuyerAgentID == null ? "" : _context.Sys_Buyer.Where(m => m.BuyerID == Entity.BuyerAgentID).FirstOrDefault().BuyerName;
            model.RecordStatus = Entity.RecordStatus;

            return model;
        }

        public List<EXPAgentCommissionBuyer> GetAgentCommissionBuyerList(string AgentComID)
        {
            if (!string.IsNullOrEmpty(AgentComID))
            {
                long? agentComID = Convert.ToInt64(AgentComID);
                List<EXP_AgentCommissionBuyer> searchList = _context.EXP_AgentCommissionBuyer.Where(m => m.AgentComID == agentComID).OrderByDescending(m => m.AgentComID).ToList();
                return searchList.Select(c => SetToBussinessObject(c)).ToList<EXPAgentCommissionBuyer>();
            }
            else
                return null;
        }

        public EXPAgentCommissionBuyer SetToBussinessObject(EXP_AgentCommissionBuyer Entity)
        {
            EXPAgentCommissionBuyer Model = new EXPAgentCommissionBuyer();

            Model.AgentComID = Entity.AgentComID;
            Model.AgentComNo = Model.AgentComID == null ? "" : _context.EXP_AgentCommission.Where(m => m.AgentComID == Model.AgentComID).FirstOrDefault().AgentComNo;
            Model.BuyerID = Entity.BuyerID;
            Model.BuyerName = Model.BuyerID == null ? "" : _context.Sys_Buyer.Where(m => m.BuyerID == Model.BuyerID).FirstOrDefault().BuyerName;

            return Model;
        }

        public List<EXPAgentCommissionBuyerCI> GetAgentCommissionBuyerCIList(string AgentComID)
        {
            long agentComID = Convert.ToInt64(AgentComID);
            //long buyerID = Convert.ToInt64(BuyerID);
            List<EXP_AgentCommissionBuyerCI> searchList = _context.EXP_AgentCommissionBuyerCI.Where(m => m.AgentComID == agentComID).OrderByDescending(m => m.AgentComID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<EXPAgentCommissionBuyerCI>();
        }

        public List<EXPAgentCommissionBuyerCI> GetAgentCommissionBuyerCIList(string AgentComID, string BuyerID)
        {
            long agentComID = Convert.ToInt64(AgentComID);
            long buyerID = Convert.ToInt64(BuyerID);
            List<EXP_AgentCommissionBuyerCI> searchList = _context.EXP_AgentCommissionBuyerCI.Where(m => m.AgentComID == agentComID).OrderByDescending(m => m.AgentComID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<EXPAgentCommissionBuyerCI>();
        }

        public EXPAgentCommissionBuyerCI SetToBussinessObject(EXP_AgentCommissionBuyerCI Entity)
        {
            EXPAgentCommissionBuyerCI Model = new EXPAgentCommissionBuyerCI();

            Model.AgentComCIID = Entity.AgentComCIID;
            Model.AgentComID = Entity.AgentComID;
            Model.BuyerAgentID = Entity.BuyerAgentID;
            Model.BuyerAgentName = Entity.BuyerAgentID == null ? "" : _context.Sys_Buyer.Where(m => m.BuyerID == Entity.BuyerAgentID).FirstOrDefault().BuyerName;
            Model.AgentType = Entity.AgentType;
            Model.CIID = Entity.CIID;
            Model.CINo = Entity.CIID == null ? "" : _context.EXP_CI.Where(m => m.CIID == Model.CIID).FirstOrDefault().CIRefNo;
            Model.PIID = Entity.RefPIID;
            Model.PINo = Model.PIID == null ? "" : _context.EXP_LeatherPI.Where(m => m.PIID == Model.PIID).FirstOrDefault().PINo;
            Model.CommissionPercent = Entity.CommissionPercent;
            Model.CommissionAmount = Entity.CommissionAmount;

            Model.AITPercent = Entity.AITPercent;
            Model.AITAmount = Entity.AITAmount;
            Model.PayableAmount = Entity.PayableAmount;

            Model.ExchangeCurrency = Entity.ExchangeCurrency;
            Model.ExchangeCurrencyName = Model.ExchangeCurrency == null ? "" : _context.Sys_Currency.Where(m => m.CurrencyID == Model.ExchangeCurrency).FirstOrDefault().CurrencyName;
            Model.ExchangeRate = Entity.ExchangeRate;
            Model.ExchangeAmount = Entity.ExchangeAmount;
            Model.LocalCurrency = Entity.LocalCurrency;
            Model.LocalRate = Entity.LocalRate;
            Model.LocalAmount = Entity.LocalAmount;
            Model.RecordStatus = Entity.RecordStatus;

            return Model;
        }

        public ValidationMsg DeletedAgentCom(long? AgentComID)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var itemList = _context.EXP_AgentCommissionBuyerCI.Where(m => m.AgentComID == AgentComID).ToList();

                if (itemList.Count > 0)
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Child Record Found.";
                }
                else
                {
                    var deleteplElement = _context.EXP_AgentCommission.First(m => m.AgentComID == AgentComID);
                    _context.EXP_AgentCommission.Remove(deleteplElement);
                    _context.SaveChanges();

                    _vmMsg.Type = Enums.MessageType.Success;
                    _vmMsg.Msg = "Deleted Successfully.";
                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Delete.";
            }
            return _vmMsg;
        }

        public ValidationMsg DeletedAgentComBuyer(long? AgentComID, int? BuyerID)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var itemList = _context.EXP_AgentCommissionBuyerCI.Where(m => m.AgentComID == AgentComID).ToList();

                if (itemList.Count > 0)
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Child Record Found.";
                }
                else
                {
                    var deleteElement = _context.EXP_AgentCommissionBuyer.First(m => m.AgentComID == AgentComID && m.BuyerID == BuyerID);
                    _context.EXP_AgentCommissionBuyer.Remove(deleteElement);
                    _context.SaveChanges();
                    _vmMsg.Type = Enums.MessageType.Success;
                    _vmMsg.Msg = "Deleted Successfully.";
                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Delete.";
            }
            return _vmMsg;
        }

        public ValidationMsg DeletedAgentComBuyerCI(long? AgentComCIID)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var deleteElement = _context.EXP_AgentCommissionBuyerCI.First(m => m.AgentComCIID == AgentComCIID);
                _context.EXP_AgentCommissionBuyerCI.Remove(deleteElement);

                _context.SaveChanges();
                _vmMsg.Type = Enums.MessageType.Success;
                _vmMsg.Msg = "Deleted Successfully.";
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Delete.";
            }
            return _vmMsg;
        }
    }
}
