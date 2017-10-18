using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DALLCOpening
    {
        private readonly BLC_DEVEntities obEntity;     
        private UnitOfWork repository;
        private ValidationMsg _vmMsg;
        public DALLCOpening()
        {
            obEntity = new BLC_DEVEntities();
            _vmMsg = new ValidationMsg();
            repository = new UnitOfWork();        
        }
        public object GetBeneficiaryInfo()
        {
            var result = from temp in obEntity.Sys_Supplier
                         join temp2 in obEntity.Sys_SupplierAddress on
                           temp.SupplierID equals temp2.SupplierID
                         where temp.IsActive == true && temp2.IsActive == true
                         select new { 
                             BuyerID = temp.SupplierID, 
                             BuyerName = temp.SupplierName, 
                             BuyerAddressID = temp2.SupplierAddressID, 
                             Address =temp2.Address
                         };
            return result;
        }

        public object SearchBenificiaryByBenificiary(string buyer)
        {
            var result = from temp in obEntity.Sys_Supplier
                         join temp2 in obEntity.Sys_SupplierAddress on
                           temp.SupplierID equals temp2.SupplierID
                         where temp.IsActive == true && temp2.IsActive == true && (temp.SupplierName.StartsWith(buyer) || temp.SupplierName == buyer)
                         select new {
                             BuyerID = temp.SupplierID,
                             BuyerName = temp.SupplierName,
                             BuyerAddressID = temp2.SupplierAddressID,
                             Address = temp2.Address
                         };
            return result;
        }

        public IEnumerable<ChemicalPI> GetPIInfo()
        {
            string sql = @"SELECT PIID,PINo,ISNULL(br.BranchID,'') BranchID,CONVERT(varchar(11),PIDate,103)  PIDate,ISNULL(br.BranchSwiftCode,'') BankSwiftCode,ISNULL(p.BeneficiaryBank,0) BeneficiaryBankID, ISNULL(p.AdvisingBank,0) AdvisingBankID,ISNULL(br2.BranchName,'') AdvisingBankName,ISNULL(br.BranchName,'') BeneficiaryBankName, ISNULL(PIBeneficiary,0) Beneficiary,
                            ISNULL(s.SupplierName,'') BeneficiaryName, ISNULL(p.BeneficiaryAddressID,0) BeneficiaryAddressID, (sa.SupplierAddressID) SupplierAddressID,ISNULL(sa.Address,'') BenificiaryAddress, p.PICurrency,p.PaymentTerm,
                            p.PaymentMode,p.DeferredDays,p.CountryOforigin,p.BeneficiaryBank,p.DeliveryTerm,p.PortofDischarge,p.DeliveryMode, p.PortofLoading,p.Transshipment,p.PartialShipment,
                            p.GoodDelivery,p.ShippingMark,p.ExpectedShipmentTime,p.Packing,p.PreShipmentIns,p.OfferValidityDays,ISNULL(p.ExchangeValue,0) ExchangeValue,ISNULL(p.ExchangeRate,0) ExchangeRate, (ISNULL(p.ExchangeValue,0)/ ISNULL((NULLIF(p.ExchangeRate,0)),1)) ActualPrice
                            FROM PRQ_ChemicalPI p
                            LEFT JOIN Sys_Branch br ON  br.BranchID = p.BeneficiaryBank 
                            LEFT JOIN Sys_Branch br2 ON  br2.BranchID = p.AdvisingBank 
                            LEFT JOIN Sys_Bank b ON br.BankID = b.BankID
                            LEFT JOIN Sys_Supplier s ON s.SupplierID = p.PIBeneficiary
                            LEFT JOIN Sys_SupplierAddress sa ON s.SupplierID =  sa.SupplierID
                            ";
            var result = obEntity.Database.SqlQuery<ChemicalPI>(sql).OrderByDescending(ob=>ob.PIID);
            return result;
        }
        
        public IEnumerable<LCM_LCOpeningBank> GetLCBankList(string bankCategory, string bankType)
        {
            string sql = @"SELECT b.BankID,br.BranchSwiftCode BankCode,(b.BankName +' , '+ br.BranchName) BankName, br.BranchID,ISNULL(br.BranchName,'') BranchName,ISNULL(br.LCLimit,0) LCLimit,ISNULL(c.CurrencyName,'') CurrencyName,ISNULL(b.BankBINNo,'') BankBINNo FROM Sys_Bank b
                            INNER JOIN Sys_Branch br ON b.BankID = br.BankID
                            LEFT JOIN Sys_Currency c ON br.LCCurrency = c.CurrencyID
                            WHERE b.BankCategory='" + bankCategory + "' AND b.BankType ='" + bankType + "' AND b.IsActive='true' ";
            var result = obEntity.Database.SqlQuery<LCM_LCOpeningBank>(sql);
            return result;
        }
        public IEnumerable<LCM_LCOpeningBank> GetLcAdvBenfBankList(string bankCategory)
        {
            string sql = @"SELECT b.BankID,br.BranchSwiftCode BankCode,(b.BankName +' , '+ br.BranchName) BankName, br.BranchID,ISNULL(br.BranchName,'') BranchName,ISNULL(br.LCLimit,0) LCLimit,ISNULL(c.CurrencyName,'') CurrencyName,ISNULL(b.BankBINNo,'') BankBINNo FROM Sys_Bank b
                            INNER JOIN Sys_Branch br ON b.BankID = br.BankID
                            LEFT JOIN Sys_Currency c ON br.LCCurrency = c.CurrencyID
                            WHERE b.BankCategory='" + bankCategory + "' AND (b.BankType ='ADV' OR b.BankType ='BNF') AND b.IsActive='true' ";
            var result = obEntity.Database.SqlQuery<LCM_LCOpeningBank>(sql);
            return result;
        }

        public IEnumerable<GetLCList> GetLCList()
        {
            string sql = @"SELECT lc.LCID,lc.LCNo,ISNULL(s.SupplierName,'') SupplierName,CONVERT( varchar(15),ISNULL(lc.LCDate,''),106) LCDate,
                            ISNULL(lc.LCAmount,0) LCAmount,ISNULL(lc.LCMargin,0) LCMargin,ISNULL(bd.ExchangeRate,0) ExchangeRate,ISNULL(bd.ExchangeAmount,0)ExchangeAmount
                            FROM LCM_LCOpening lc
                            LEFT JOIN LCM_BankDebitVoucher bd ON lc.LCID= bd.LCID
                            LEFT JOIN PRQ_ChemicalPI p ON lc.PIID = p.PIID
                            LEFT JOIN Sys_Supplier s ON p.SupplierID = s.SupplierID
                            WHERE lc.RecordStatus ='CNF' ORDER BY lc.LCID DESC";
            var result = obEntity.Database.SqlQuery<GetLCList>(sql);
            return result;
        }

        public ValidationMsg SaveLCOpeningInfo(LcmLcOpening dataSet, int UserId)
        {
            try 
            { 
                LCM_LCOpening obLcOpening = new LCM_LCOpening();
                obLcOpening.LCNo = dataSet.LCNo;
                obLcOpening.LCDate =DalCommon.SetDate(dataSet.LCDate);
                obLcOpening.LCAmount = dataSet.LCAmount;
                
                obLcOpening.ExchangeRate = dataSet.ExchangeRate;
                obLcOpening.ExchangeValue = Convert.ToDecimal(dataSet.LCAmount * dataSet.ExchangeRate);
                if (dataSet.LCAmountCurrency !=0)
                { 
                    obLcOpening.LCCurrencyID = (byte)dataSet.LCAmountCurrency;
                }
                if (dataSet.ExchangeCurrency !=0)
                { 
                    obLcOpening.ExchangeCurrency =(byte)dataSet.ExchangeCurrency;
                }
                obLcOpening.IssueDate = DalCommon.SetDate(dataSet.IssueDate);
                obLcOpening.ExpiryDate = DalCommon.SetDate(dataSet.ExpiryDate);
                obLcOpening.ExpiryPlace = dataSet.ExpiryPlace;
                obLcOpening.NNDSendingTime = dataSet.NNDSendingTime;
                obLcOpening.LCANo = dataSet.LCANo;
                obLcOpening.IRCNo = dataSet.IRCNo;
                obLcOpening.VatRegNo = dataSet.VatRegNo;
                obLcOpening.TINNo = dataSet.TINNo;
                obLcOpening.BINNo = dataSet.BINNo;
                obLcOpening.ICNNo = dataSet.ICNNo;
                obLcOpening.ICNDate = DalCommon.SetDate(dataSet.ICNDate);
                obLcOpening.LastShipmentDate = DalCommon.SetDate(dataSet.LastShipmentDate);
                if (dataSet.BranchID != 0)
                { 
                    obLcOpening.LCOpeningBank = dataSet.BranchID;
                }
                if (dataSet.AdvisingBank != 0)
                { 
                    obLcOpening.AdvisingBank = dataSet.AdvisingBank;
                }
                if (dataSet.Beneficiary !=null)
                { 
                 obLcOpening.Beneficiary = Convert.ToInt32(dataSet.Beneficiary);
                }
                if (dataSet.BeneficiaryAddressID !=0)
                { 
                    obLcOpening.BeneficiaryAddressID = dataSet.BeneficiaryAddressID;
                }
                if (!string.IsNullOrEmpty(dataSet.PINo))
                {
                    obLcOpening.PIID = dataSet.PIID;
                    obLcOpening.PINo = dataSet.PINo;
                }
                

                obLcOpening.LCReviceNo = dataSet.LCReviceNo;
                obLcOpening.LCReviceDate = DateTime.Now;
                obLcOpening.LCState = "LCOP";
                obLcOpening.LCStatus = dataSet.LCStatus;
                obLcOpening.RecordStatus = "NCF";
                obLcOpening.RunningStatus = "LCOP";           
                obLcOpening.ApprovalAdvice=dataSet.ApprovalAdvice;
                obLcOpening.SetOn = DateTime.Now;
                obLcOpening.LCMargin = dataSet.LCMargin;
                obLcOpening.SetBy = UserId;
                obLcOpening.ModifiedOn= DateTime.Now;
                obLcOpening.ModifiedBy = UserId;
                obLcOpening.IPAddress = GetIPAddress.LocalIPAddress();                
        //   ##############        "": "", "": "", "": "", "": "", "": "",
                if (!string.IsNullOrEmpty(dataSet.InsuranceID.ToString())) { obLcOpening.InsuranceID = Convert.ToInt32(dataSet.InsuranceID); }               
                if (!string.IsNullOrEmpty(dataSet.PICurrency.ToString()))  { obLcOpening.PICurrency = (byte)dataSet.PICurrency; }               
                obLcOpening.PaymentTerm = dataSet.PaymentTerm;
                obLcOpening.PaymentMode = dataSet.PaymentMode;
                obLcOpening.DeferredDays = dataSet.DeferredDays;
                if (!string.IsNullOrEmpty(dataSet.CountryOforigin.ToString())) { obLcOpening.CountryOforigin = Convert.ToInt32(dataSet.CountryOforigin); }
                if (!string.IsNullOrEmpty(dataSet.BeneficiaryBank.ToString())) { obLcOpening.BeneficiaryBank = Convert.ToInt32(dataSet.BeneficiaryBank); }
                
                obLcOpening.DeliveryTerm = dataSet.DeliveryTerm;
                if (!string.IsNullOrEmpty(dataSet.PortofDischarge.ToString())) { obLcOpening.PortofDischarge = Convert.ToInt32(dataSet.PortofDischarge); }
               
                obLcOpening.DeliveryMode = dataSet.DeliveryMode;
                if (!string.IsNullOrEmpty(dataSet.PortofLoading.ToString())) { obLcOpening.PortofLoading = Convert.ToInt32(dataSet.PortofLoading); }
            
                obLcOpening.Transshipment = dataSet.Transshipment;
                obLcOpening.PartialShipment = dataSet.PartialShipment;
                obLcOpening.GoodDelivery = dataSet.GoodDelivery;
                obLcOpening.ShippingMark = dataSet.ShippingMark;
                if (!string.IsNullOrEmpty(dataSet.ExpectedShipmentTime)) { obLcOpening.ExpectedShipmentTime = Convert.ToInt32(dataSet.ExpectedShipmentTime); }
              
                obLcOpening.Packing = dataSet.Packing;
                obLcOpening.PreShipmentIns = dataSet.PreShipmentIns;
                if (!string.IsNullOrEmpty(dataSet.OfferValidityDays.ToString())) { obLcOpening.OfferValidityDays = Convert.ToInt32(dataSet.OfferValidityDays); }
                obLcOpening.PresentationDays = Convert.ToInt32(dataSet.PresentationDays);
                obLcOpening.ConfirmStatus = dataSet.ConfirmStatus;
                
                obEntity.LCM_LCOpening.Add(obLcOpening);
                int flag = obEntity.SaveChanges();
                if (flag ==1)
                {
                    _vmMsg.ReturnId = obLcOpening.LCID; 
                    _vmMsg.Type = Enums.MessageType.Success;
                    _vmMsg.Msg = "Saved Successfully.";
                }
                else
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Saved Faild.";
                }
           }
            catch (DbEntityValidationException e)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var eve in e.EntityValidationErrors)
                {
                    sb.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                                    eve.Entry.Entity.GetType().Name,
                                                    eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        sb.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                                    ve.PropertyName,
                                                    ve.ErrorMessage));
                    }
                }
                throw new DbEntityValidationException(sb.ToString(), e);
            }
            return _vmMsg;
        }

        public ValidationMsg UpdateLCOpeningInfo(LcmLcOpening dataSet, int UserID)
        {
            try
            {
                LCM_LCOpening obLcOpening = repository.LCOpeningRepository.GetByID(Convert.ToInt32(dataSet.LCID));
                if (!obLcOpening.RecordStatus.Equals("CNF"))
                {

                    obLcOpening.LCNo = dataSet.LCNo;
                    obLcOpening.LCDate = DalCommon.SetDate(dataSet.LCDate.ToString());
                    obLcOpening.LCAmount = dataSet.LCAmount;                    
                    obLcOpening.ExchangeRate = dataSet.ExchangeRate;
                    obLcOpening.ExchangeValue = Convert.ToDecimal(dataSet.LCAmount * dataSet.ExchangeRate);
                    if (dataSet.LCAmountCurrency > 0) { obLcOpening.LCCurrencyID = (byte)dataSet.LCAmountCurrency; }
                    if (dataSet.ExchangeCurrency > 0) { obLcOpening.ExchangeCurrency = (byte)dataSet.ExchangeCurrency; }             
                   
                   
                    obLcOpening.IssueDate = Convert.ToDateTime(DalCommon.SetDate(dataSet.IssueDate));
                    obLcOpening.ExpiryDate = DalCommon.SetDate(dataSet.ExpiryDate);
                    obLcOpening.ExpiryPlace = dataSet.ExpiryPlace;
                   obLcOpening.NNDSendingTime = dataSet.NNDSendingTime;
                   obLcOpening.ApprovalAdvice = dataSet.ApprovalAdvice;
                    obLcOpening.LCANo = dataSet.LCANo;
                    obLcOpening.IRCNo = dataSet.IRCNo;
                    obLcOpening.VatRegNo = dataSet.VatRegNo;
                    obLcOpening.TINNo = dataSet.TINNo;
                    obLcOpening.BINNo = dataSet.BINNo;
                    obLcOpening.ICNNo = dataSet.ICNNo;
                    obLcOpening.ICNDate = DalCommon.SetDate(dataSet.ICNDate);
                    obLcOpening.LastShipmentDate = DalCommon.SetDate(dataSet.LastShipmentDate);
                    if (dataSet.BranchID>0) { obLcOpening.LCOpeningBank = dataSet.BranchID; }
                    if (dataSet.AdvisingBank > 0) { obLcOpening.AdvisingBank = dataSet.AdvisingBank; }
                    
                    if (!string.IsNullOrEmpty(dataSet.Beneficiary)) { obLcOpening.Beneficiary = Convert.ToInt32(dataSet.Beneficiary); }
              
                    if (!string.IsNullOrEmpty(dataSet.PINo))
                    {
                        obLcOpening.PIID = dataSet.PIID;
                        obLcOpening.PINo = dataSet.PINo;
                    }
                    
                    obLcOpening.ModifiedOn = DateTime.Now;
                    obLcOpening.ModifiedBy = UserID;
                    obLcOpening.IPAddress = GetIPAddress.LocalIPAddress();
                    obLcOpening.LCMargin = dataSet.LCMargin;
                    //   ##############        "": "", "": "", "": "", "": "", "": "",
                    if (!string.IsNullOrEmpty(dataSet.InsuranceID.ToString())) { obLcOpening.InsuranceID = Convert.ToInt32(dataSet.InsuranceID); }
                    if (!string.IsNullOrEmpty(dataSet.PICurrency.ToString())) { obLcOpening.PICurrency = (byte)dataSet.PICurrency; }
                    obLcOpening.PaymentTerm = dataSet.PaymentTerm;
                    obLcOpening.PaymentMode = dataSet.PaymentMode;
                    obLcOpening.DeferredDays = dataSet.DeferredDays;
                    if (!string.IsNullOrEmpty(dataSet.CountryOforigin.ToString())) { obLcOpening.CountryOforigin = Convert.ToInt32(dataSet.CountryOforigin); }
                    if (!string.IsNullOrEmpty(dataSet.BeneficiaryBank.ToString())) { obLcOpening.BeneficiaryBank = Convert.ToInt32(dataSet.BeneficiaryBank); }

                    obLcOpening.DeliveryTerm = dataSet.DeliveryTerm;
                    if (!string.IsNullOrEmpty(dataSet.PortofDischarge.ToString())) { obLcOpening.PortofDischarge = Convert.ToInt32(dataSet.PortofDischarge); }

                    obLcOpening.DeliveryMode = dataSet.DeliveryMode;
                    if (!string.IsNullOrEmpty(dataSet.PortofLoading.ToString())) { obLcOpening.PortofLoading = Convert.ToInt32(dataSet.PortofLoading); }

                    obLcOpening.Transshipment = dataSet.Transshipment;
                    obLcOpening.PartialShipment = dataSet.PartialShipment;
                    obLcOpening.GoodDelivery = dataSet.GoodDelivery;
                    obLcOpening.ShippingMark = dataSet.ShippingMark;
                    if (!string.IsNullOrEmpty(dataSet.ExpectedShipmentTime)) { obLcOpening.ExpectedShipmentTime = Convert.ToInt32(dataSet.ExpectedShipmentTime); }

                    obLcOpening.Packing = dataSet.Packing;
                    obLcOpening.PreShipmentIns = dataSet.PreShipmentIns;
                    if (!string.IsNullOrEmpty(dataSet.OfferValidityDays.ToString())) { obLcOpening.OfferValidityDays = Convert.ToInt32(dataSet.OfferValidityDays); }
                    
                    obLcOpening.PresentationDays = Convert.ToInt32(dataSet.PresentationDays);
                    obLcOpening.ConfirmStatus = dataSet.ConfirmStatus;

                    repository.LCOpeningRepository.Update(obLcOpening);
                    int flag = repository.Save();
                    if (flag == 1)
                    {
                        _vmMsg.Type = Enums.MessageType.Success;
                        _vmMsg.Msg = "Update Successfully.";
                    }
                    else
                    {
                        _vmMsg.Type = Enums.MessageType.Error;
                        _vmMsg.Msg = "Saved Faild.";
                    }
                }
                else
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Data already confirmed. You can't update this.";
                }
            }
            catch (DbEntityValidationException e)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var eve in e.EntityValidationErrors)
                {
                    sb.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                                    eve.Entry.Entity.GetType().Name,
                                                    eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        sb.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                                    ve.PropertyName,
                                                    ve.ErrorMessage));
                    }
                }
                throw new DbEntityValidationException(sb.ToString(), e);
            }
            return _vmMsg;
        }

        public ValidationMsg UpdateRecordStatus(string note, string lcId, int userId)
        {
            try
            {
                if ( lcId != "")
                {
                    LCM_LCOpening ob = repository.LCOpeningRepository.GetByID(Convert.ToInt32(lcId));
                    ob.ApprovalAdvice = note;
                    ob.RecordStatus = "CHK";
                    ob.CheckedBy = userId;
                    ob.CheckDate = DateTime.Now;
                    repository.LCOpeningRepository.Update(ob);
                    int flag = repository.Save();
                    if (flag == 1)
                    {
                        _vmMsg.Type = Enums.MessageType.Success;
                        _vmMsg.Msg = "Checked Successfully.";
                    }
                    else
                    {
                        _vmMsg.Type = Enums.MessageType.Error;
                        _vmMsg.Msg = "Checked Faild.";
                    }
                }
                else
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Data Save First Before Checked.";
                }
            }
            catch (DbEntityValidationException e)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var eve in e.EntityValidationErrors)
                {
                    sb.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                                    eve.Entry.Entity.GetType().Name,
                                                    eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        sb.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                                    ve.PropertyName,
                                                    ve.ErrorMessage));
                    }
                }
                throw new DbEntityValidationException(sb.ToString(), e);
            }
            return _vmMsg;
        }

        public ValidationMsg UpdateConfRecordStatus(string lcId, string approvalAdvice, int userId)
        {
            try
            {
                if (lcId != "")
                {
                    LCM_LCOpening ob = repository.LCOpeningRepository.GetByID(Convert.ToInt32(lcId));
                    if (ob.RecordStatus == "NCF")
                    {
                        ob.ApprovalAdvice = approvalAdvice.ToString().Trim();
                        ob.RecordStatus = "CNF";
                        ob.LCStatus = "RNG";
                        ob.ApprovedBy = userId;
                        ob.ApproveDate = DateTime.Now;
                        repository.LCOpeningRepository.Update(ob);
                        int flag = repository.Save();
                        if (flag == 1)
                        {
                            _vmMsg.Type = Enums.MessageType.Success;
                            _vmMsg.Msg = "Confirmed Successfully.";
                        }
                        else
                        {
                            _vmMsg.Type = Enums.MessageType.Error;
                            _vmMsg.Msg = "Confirmed Faild.";
                        }
                    }
                    else
                    {
                        _vmMsg.Type = Enums.MessageType.Error;
                        _vmMsg.Msg = "Data Save First.";
                    }
                }
                else
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Data Save First.";
                }
            }
            catch (DbEntityValidationException e)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var eve in e.EntityValidationErrors)
                {
                    sb.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                                    eve.Entry.Entity.GetType().Name,
                                                    eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        sb.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                                    ve.PropertyName,
                                                    ve.ErrorMessage));
                    }
                }
                throw new DbEntityValidationException(sb.ToString(), e);
            }
            return _vmMsg;
        }

        public List<LcmLcOpening> GetLcOpeningReportData(String sql)
       {
           var result=  obEntity.Database.SqlQuery<LCM_LCOpening>(sql);
           
            List<LcmLcOpening> lst = new List<LcmLcOpening>();
             foreach (var item in result)
             {
                 LcmLcOpening ob = new LcmLcOpening();
                 ob.LCID = item.LCID;
                 ob.LCNo = item.LCNo;
                 ob.LCDate =item.LCDate.ToString("dd/MM/yyyy");
                 ob.LCAmount = item.LCAmount;
                 ob.RecordStatus = item.RecordStatus;
                 lst.Add(ob);
             }
             return lst;
       }

        public ValidationMsg SaveLcmLcFile(LcmLcFile model)
        {
            LCM_LCFile obLcFile = new LCM_LCFile();
         
            obLcFile.LCFileNo = model.LCFileNo;
            obLcFile.LCFileOpenDate = DateTime.Now;
            obLcFile.LCFileStatus = model.LCFileStatus;
            obLcFile.LCFileClosingDate = model.LCFileClosingDate;
            obLcFile.LCID = Convert.ToInt32(model.LCID);
            obLcFile.LCNo = model.LCNo;
            obLcFile.Remarks = model.Remarks;
            obLcFile.RecordStatus = model.RecordStatus;
            if (model.CheckedBy != null) { obLcFile.CheckedBy = Convert.ToInt32(model.CheckedBy); }
            if (obLcFile.CheckDate != null) { obLcFile.CheckDate = Convert.ToDateTime(model.CheckDate); }
            obLcFile.CheckComments = model.CheckComments;
            if (model.ApprovedBy != null) { obLcFile.ApprovedBy = Convert.ToInt32(model.ApprovedBy); }
            if (model.ApproveDate != null) { obLcFile.ApproveDate = Convert.ToDateTime(model.ApproveDate); }
            obLcFile.ApprovalComments = model.ApprovalComments;
            if (model.SetOn != null) { obLcFile.SetOn = Convert.ToDateTime(model.SetOn); }
            if (model.SetBy != null) { obLcFile.SetBy = Convert.ToInt32(model.SetBy); }
            obLcFile.ModifiedOn = DateTime.Now;
            obLcFile.ModifiedBy = model.ModifiedBy;
            obLcFile.IPAddress = GetIPAddress.LocalIPAddress();
            try
            {
                repository.LcmLcFileRepository.Insert(obLcFile);
                //repository.Save();
                _vmMsg.Type = Enums.MessageType.Success;
                _vmMsg.Msg = "Saved Successfully.";
                
            }
            catch (Exception ex)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Saved Faild.";
            }
            return _vmMsg;
        }
    }
}