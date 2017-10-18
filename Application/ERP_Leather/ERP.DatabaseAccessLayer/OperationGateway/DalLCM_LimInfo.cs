using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using ERP.EntitiesModel.AppSetupModel;
using System;
using System.Transactions;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalLCM_LimInfo
    {

        private readonly BLC_DEVEntities obEntity = new BLC_DEVEntities();
        UnitOfWork repository = new UnitOfWork();
        private ValidationMsg _vmMsg = new ValidationMsg();
        private int save;
        //private readonly BLC_DEVEntities obEntity;

        public DalLCM_LimInfo()
        {
            obEntity = new BLC_DEVEntities();
            _vmMsg = new ValidationMsg();

        }



        public IEnumerable<LCMLimInfo> GetLCBankList()
        {
            string sql = @"SELECT				ISNULL(b.BankID,0)BankID,
					                            ISNULL(b.BankCode,'')BankCode,
					                            ISNULL(b.BankName,'')BankName,
					                            ISNULL(b.BankType,'') BankType,
					                            ISNULL(br.BranchID,0)BranchID,
					                            ISNULL(br.BanchCode,'')BanchCode,
					                            ISNULL(br.BranchName,'')BranchName,
					                            
					                            ISNULL(br.LIMLimit,0) LimLimit,
					                            CASE  lcm.RecordStatus 
										                            WHEN 'NCF' THEN 'Not Confirm'
                                                                    WHEN 'CNF' THEN 'Confirmed'
                                                                    WHEN 'CHK' THEN 'Rejected'
                                                                    WHEN 'APV' THEN 'Approved'
					                            END		RecordStatus,
					                            
					                            ISNULL(lcm.LCID,0)LCID ,
					                            ISNULL(c.CurrencyName,0)CurrencyName,
					                            ISNULL(lcm.LCNo,0)LCNo
                            FROM				Sys_Bank b
                            LEFT JOIN			Sys_Branch br		ON b.BankID = br.BankID
                            LEFT JOIN			Sys_Currency c		ON br.LCCurrency = c.CurrencyID
                            LEFT JOIN			LCM_LCOpening lcm	ON br.BranchID = lcm.LCOpeningBank
                            WHERE				b.BankCategory='BNK' AND b.BankType='LOC'  AND b.IsActive='true' AND br.IsActive='true' 
                            ORDER BY b.BankCode,b.BankName ASC";
            var result = obEntity.Database.SqlQuery<LCMLimInfo>(sql);
            return result;
        }
        public ValidationMsg SaveLCM_LimInfo(LCM_LimInfo dataSet, string url, int _userId)
        {
            try
            {

                LCM_LimInfo obLcLimInfo = new LCM_LimInfo();
                Sys_Branch branch = new Sys_Branch();

                obLcLimInfo.LCID = dataSet.LCID;
                obLcLimInfo.LCNo = dataSet.LCNo;
                obLcLimInfo.LimID = dataSet.LimID;
                obLcLimInfo.LimNo = dataSet.LimNo;
                obLcLimInfo.LimDate = dataSet.LimDate;
                obLcLimInfo.LimLimit = dataSet.LimLimit == null ? 0 : (decimal)dataSet.LimLimit;
                obLcLimInfo.LimBalance = dataSet.LimBalance == null ? 0 : (decimal)dataSet.LimBalance;




                
                obLcLimInfo.LimBankID = dataSet.LimBankID;
                obLcLimInfo.LimBranchID = dataSet.LimBranchID;
                obLcLimInfo.LoanAmount = dataSet.LoanAmount == null ? 0 : (decimal)dataSet.LoanAmount;
                obLcLimInfo.InterestPersent = dataSet.InterestPersent == null ? 0 : (decimal)dataSet.InterestPersent;
                obLcLimInfo.InterestAmount = dataSet.InterestAmount == null ? 0 : (decimal)dataSet.InterestAmount;
                obLcLimInfo.AmountToBePaid = dataSet.AmountToBePaid == null ? 0 : (decimal)dataSet.AmountToBePaid;
                obLcLimInfo.AdjustmentTime = dataSet.AdjustmentTime;
                obLcLimInfo.OtherCharges = dataSet.OtherCharges == null ? 0 : (decimal)dataSet.OtherCharges;
                obLcLimInfo.AcceptanceCommission = dataSet.AcceptanceCommission == null ? 0 : (decimal)dataSet.AcceptanceCommission;
                obLcLimInfo.HandlingCharge = dataSet.HandlingCharge == null ? 0 : (decimal)dataSet.HandlingCharge;
                obLcLimInfo.TotalAmountToBePaid = dataSet.TotalAmountToBePaid == null ? 0 : (decimal)dataSet.TotalAmountToBePaid;
                obLcLimInfo.PaidAmount = dataSet.PaidAmount == null ? 0 : (decimal)dataSet.PaidAmount;
                obLcLimInfo.LimCurrency = (byte)dataSet.LimCurrency;

                obLcLimInfo.ExchangeCurrency = (byte)(dataSet.ExchangeCurrency);
                obLcLimInfo.ExchangeRate = dataSet.ExchangeRate == null ? 0 : (decimal)dataSet.ExchangeRate;//dataSet.ExchangeRate;
                obLcLimInfo.ExchangeValue = dataSet.ExchangeValue == null ? 0 : (decimal)dataSet.ExchangeValue;//dataSet.ExchangeValue;
                obLcLimInfo.RecordStatus = "NCF";

                obLcLimInfo.LCMarginTransfer = dataSet.LCMarginTransfer == null ? 0 : (decimal)dataSet.LCMarginTransfer;
                obLcLimInfo.LimMarginTrans = dataSet.LimMarginTrans == null ? 0 : (decimal)dataSet.LimMarginTrans;
                obLcLimInfo.LimMarginTransPercnt = dataSet.LimMarginTransPercnt == null ? 0 : (decimal)dataSet.LimMarginTransPercnt;
                obLcLimInfo.InterestCalcDate = dataSet.InterestCalcDate;
                obLcLimInfo.CalcInterestAmt = dataSet.CalcInterestAmt == null ? 0 : (decimal)dataSet.CalcInterestAmt;
                obLcLimInfo.ExciseDuty = dataSet.ExciseDuty == null ? 0 : (decimal)dataSet.ExciseDuty;
                obLcLimInfo.TransCash = dataSet.TransCash == null ? 0 : (decimal)dataSet.TransCash;
                obLcLimInfo.CalcAmtToPaid = dataSet.CalcAmtToPaid == null ? 0 : (decimal)dataSet.CalcAmtToPaid;
                obLcLimInfo.LimAdjustDr = dataSet.LimAdjustDr == null ? 0 : (decimal)dataSet.LimAdjustDr;
                obLcLimInfo.LimAdjustCr = dataSet.LimAdjustCr == null ? 0 : (decimal)dataSet.LimAdjustCr;
                obLcLimInfo.TotalCalcAmtToPaid = dataSet.TotalCalcAmtToPaid == null ? 0 : (decimal)dataSet.TotalCalcAmtToPaid;

                obLcLimInfo.Remarks = dataSet.Remarks;

                obLcLimInfo.SetOn = DateTime.Now;
                obLcLimInfo.SetBy = _userId;
                obLcLimInfo.IPAddress = GetIPAddress.LocalIPAddress();
                obEntity.LCM_LimInfo.Add(obLcLimInfo);
                int flag = obEntity.SaveChanges();
                if (flag == 1)
                {

                    //_vmMsg.ReturnId = repository.LimInfoRepository.Get().Last().LimID;
                    //_vmMsg.ReturnCode = dataSet.LimNo; //repository.LimInfoRepository.GetByID(_vmMsg.ReturnId).LimNo;
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
        public ValidationMsg UpdateLCM_LimInfo(LCM_LimInfo dataSet, int _userId)
        {
            try
            {
                LCM_LimInfo obLcLimInfo = repository.LimInfoRepository.GetByID(Convert.ToInt32(dataSet.LimID));
                if (obLcLimInfo.RecordStatus != "CNF")
                {
                    obLcLimInfo.LimID = dataSet.LimID;
                    obLcLimInfo.LimNo = dataSet.LimNo;
                    obLcLimInfo.LimDate = dataSet.LimDate;
                    obLcLimInfo.LimLimit = dataSet.LimLimit;
                    obLcLimInfo.LimBalance = dataSet.LimBalance;
                    obLcLimInfo.LimBankID = dataSet.LimBankID;
                    obLcLimInfo.LimBranchID = dataSet.LimBranchID;
                    obLcLimInfo.LoanAmount = dataSet.LoanAmount;
                    obLcLimInfo.InterestPersent = dataSet.InterestPersent;
                    obLcLimInfo.InterestAmount = dataSet.InterestAmount;
                    obLcLimInfo.AmountToBePaid = dataSet.AmountToBePaid;
                    obLcLimInfo.AdjustmentTime = dataSet.AdjustmentTime;
                    obLcLimInfo.OtherCharges = dataSet.OtherCharges;
                    obLcLimInfo.AcceptanceCommission = dataSet.AcceptanceCommission;
                    obLcLimInfo.HandlingCharge = dataSet.HandlingCharge;
                    obLcLimInfo.TotalAmountToBePaid = dataSet.TotalAmountToBePaid;
                    obLcLimInfo.PaidAmount = dataSet.PaidAmount;
                    obLcLimInfo.LimCurrency = dataSet.LimCurrency;
                    obLcLimInfo.ExchangeCurrency = dataSet.ExchangeCurrency;
                    obLcLimInfo.ExchangeRate = dataSet.ExchangeRate;
                    obLcLimInfo.ExchangeValue = dataSet.ExchangeValue;



                    obLcLimInfo.LCMarginTransfer = dataSet.LCMarginTransfer == null ? 0 : dataSet.LCMarginTransfer;
                    obLcLimInfo.LimMarginTrans = dataSet.LimMarginTrans == null ? 0 : dataSet.LimMarginTrans;
                    obLcLimInfo.LimMarginTransPercnt = dataSet.LimMarginTransPercnt == null ? 0 : dataSet.LimMarginTransPercnt;
                    obLcLimInfo.InterestCalcDate = dataSet.InterestCalcDate;
                    obLcLimInfo.CalcInterestAmt = dataSet.CalcInterestAmt == null ? 0 : dataSet.CalcInterestAmt;
                    obLcLimInfo.ExciseDuty = dataSet.ExciseDuty == null ? 0 : dataSet.ExciseDuty;
                    obLcLimInfo.TransCash = dataSet.TransCash == null ? 0 : dataSet.TransCash;
                    obLcLimInfo.CalcAmtToPaid = dataSet.CalcAmtToPaid == null ? 0 : dataSet.CalcAmtToPaid;
                    obLcLimInfo.LimAdjustDr = dataSet.LimAdjustDr == null ? 0 : dataSet.LimAdjustDr;
                    obLcLimInfo.LimAdjustCr = dataSet.LimAdjustCr == null ? 0 : dataSet.LimAdjustCr;
                    obLcLimInfo.TotalCalcAmtToPaid = dataSet.TotalCalcAmtToPaid == null ? 0 : dataSet.TotalCalcAmtToPaid;





                    obLcLimInfo.Remarks = dataSet.Remarks;
                    obLcLimInfo.ModifiedOn = DateTime.Now;
                    obLcLimInfo.ModifiedBy = _userId;
                    obLcLimInfo.IPAddress = GetIPAddress.LocalIPAddress();

                    repository.LimInfoRepository.Update(obLcLimInfo);
                    int flag = repository.Save();
                    if (flag == 1)
                    {

                        _vmMsg.Type = Enums.MessageType.Success;
                        _vmMsg.Msg = "Updated Successfully.";
                    }
                    else
                    {
                        _vmMsg.Type = Enums.MessageType.Error;
                        _vmMsg.Msg = "Update Faild.";
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
        public ValidationMsg DeleteLimInfoList(int limID)
        {
            try
            {
                var limInfoList = repository.LimInfoRepository.Get().Where(ob => ob.LimID == limID).ToList();
                if (limInfoList.Count > 0)
                {
                    foreach (var pack in limInfoList)
                    {
                        repository.LimInfoRepository.Delete(pack);
                    }
                }
                repository.LimInfoRepository.Delete(limID);

                save = repository.Save();
                if (save == 1)
                {
                    _vmMsg.Type = Enums.MessageType.Delete;
                    _vmMsg.Msg = "Successfully deleted the record.";
                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to delete the record.";
            }
            return _vmMsg;
        }



        public ValidationMsg ConfirmLimInformation(int userid, string limID,string branchID)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    if (limID != "")
                    {
                        LCM_LimInfo obLimInfo = repository.LimInfoRepository.GetByID(Convert.ToInt32(limID));
                        if (obLimInfo.RecordStatus == "NCF")
                        {
                           

                            obLimInfo.RecordStatus = "CNF";
                            obLimInfo.SetBy = userid;
                            obLimInfo.SetOn = DateTime.Now;
                            repository.LimInfoRepository.Update(obLimInfo);

                            Sys_Branch obBranch = repository.BranchRepository.GetByID(Convert.ToInt32(branchID));
                            obBranch.LIMBalance = obLimInfo.LimLimit - obLimInfo.LoanAmount;
                            repository.BranchRepository.Update(obBranch);

                            int flag = repository.Save();
                            if (flag == 1)
                            {
                                _vmMsg.Type = Enums.MessageType.Success;
                                _vmMsg.Msg = "Confirmed Faild. ";
                            }
                            else
                            {
                                _vmMsg.Type = Enums.MessageType.Error;
                                _vmMsg.Msg = "Confirmed Successfully.";
                            }
                        }
                        else
                        {
                            _vmMsg.Type = Enums.MessageType.Error;
                            _vmMsg.Msg = "Data has Already Confirmed.";
                        }
                    }

                    else
                    {
                        _vmMsg.Type = Enums.MessageType.Error;
                        _vmMsg.Msg = "Data Save First.";
                    }

                    tx.Complete();
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




    }
}
