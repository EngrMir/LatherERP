using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using DatabaseUtility;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.OperationModel;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.BaseModel;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalEXPLCRetirement
    {
        private readonly BLC_DEVEntities obEntity;
        private UnitOfWork repository;
        private ValidationMsg _vmMsg;
        private int save;

        public DalEXPLCRetirement()
        {
            obEntity = new BLC_DEVEntities();
            _vmMsg = new ValidationMsg();
            repository = new UnitOfWork();
        }



        public List<EXPLCRetirement> GetLCNoList()
        {
            var queryString = @"SELECT		ISNULL(lc.LCID,0)LCID,
			                                ISNULL(lc.LCNo,'')LCNo,   
			                                ISNULL(lc.LCDate,'')LCDate,
			                                ISNULL(lc.LCAmount,0) LCAmount,
			                                --ISNULL(bdv.LCMargin,0)LCMargin,
			                                ISNULL(cipi.ExchangeValue,0)ExchangeValueInvoice,
			                                ISNULL(cipi.ExchangeRate,0)ExchangeRateInvoice,
			                                ISNULL(ci.CIID,0) CIID,
			                                ISNULL(ci.CINo,'') CINo,
			                                ci.CIDate
			                                --ISNULL((ci.ExchangeValue/NULLIF(ci.ExchangeRate),0),0) CITotalValue,
			                                --ISNULL((cipi.ExchangeValue/ISNULL(NULLIF(cipi.ExchangeRate,0),1)),0)CITotalValue
                                FROM		EXP_LCOpening lc
			                                INNER JOIN EXP_CIPI cipi ON lc.LCID=cipi.LCID
			                                INNER JOIN EXP_CI ci ON cipi.CIID=ci.CIID			                                
			                                --INNER JOIN EXP_BankDebitVoucher bdv ON ci.CIID=bdv.CIID 
                                ORDER BY	lc.LCID DESC";
            var lcList = obEntity.Database.SqlQuery<EXPLCRetirement>(queryString);
            return lcList.ToList();
        }
        public ValidationMsg SaveLCM_LCRetirement(EXP_LCRetirement dataSet, int _userId, string url)
        {
            try
            {
                EXP_LCRetirement obLCRetirement = new EXP_LCRetirement();
                obLCRetirement.LCRetirementID = dataSet.LCRetirementID;
                obLCRetirement.LCRetirementNo = DalCommon.GetPreDefineNextCodeByUrl(url);
                obLCRetirement.LCRetirementDate = dataSet.LCRetirementDate;
                obLCRetirement.LCNo = dataSet.LCNo;
                obLCRetirement.LCID = dataSet.LCID;
                //obLCRetirement.BillValue = (decimal)dataSet.BillValue;
                //EXP_CIPI ExchangeValueInvoice = repository.EXPCommercialInvoicePIRepository.Get(filter: dat => dat.LCID == dataSet.LCID).FirstOrDefault();
                //EXP_CIPI ExchangeRateInvoice = repository.EXPCommercialInvoicePIRepository.Get(filter: dat => dat.LCID == dataSet.LCID).FirstOrDefault();
                obLCRetirement.BillValue = dataSet.BillValue;//(ExchangeValueInvoice.ExchangeValue / ExchangeRateInvoice.ExchangeRate) == null ? 0 : (decimal)(ExchangeValueInvoice.ExchangeValue / ExchangeRateInvoice.ExchangeRate);

                //obLcLimInfo.OtherCharges = dataSet.OtherCharges == null ? 0 : (decimal)dataSet.OtherCharges;

                obLCRetirement.InterestPersent = dataSet.InterestPersent == null ? 0 : (decimal)dataSet.InterestPersent;
                obLCRetirement.InterestAmount = dataSet.InterestAmount == null ? 0 : (decimal)dataSet.InterestAmount;

                obLCRetirement.BankCommissionAmt = dataSet.BankCommissionAmt == null ? 0 : (decimal)dataSet.BankCommissionAmt;
                obLCRetirement.OtherCharge = dataSet.OtherCharge == null ? 0 : (decimal)dataSet.OtherCharge;
                obLCRetirement.SwiftCharge = dataSet.SwiftCharge == null ? 0 : (decimal)dataSet.SwiftCharge;
                obLCRetirement.TotalAmount = dataSet.TotalAmount == null ? 0 : (decimal)dataSet.TotalAmount;

                obLCRetirement.LCRCurrency = (byte)dataSet.LCRCurrency;
                obLCRetirement.Remarks = dataSet.Remarks == null ? "" : dataSet.Remarks;

                obLCRetirement.ExchangeCurrency = (byte)(dataSet.ExchangeCurrency);
                //obLCRetirement.ExchangeRate = (decimal)dataSet.ExchangeRate == null ? 0 : (decimal)dataSet.ExchangeRate;
                if (obLCRetirement.ExchangeRate == null)
                {
                    dataSet.ExchangeRate = 0;
                }
                else 
                {
                    obLCRetirement.ExchangeRate = (decimal)dataSet.ExchangeRate;
                }
                obLCRetirement.ExchangeValue = (decimal)dataSet.ExchangeValue == null ? 0 : (decimal)dataSet.ExchangeValue;
                obLCRetirement.RecordStatus = "NCF";

                obLCRetirement.SetOn = DateTime.Now;
                obLCRetirement.SetBy = _userId;
                obLCRetirement.IPAddress = GetIPAddress.LocalIPAddress();

                obEntity.EXP_LCRetirement.Add(obLCRetirement);
                int flag = obEntity.SaveChanges();
                if (flag == 1)
                {

                    _vmMsg.ReturnId = repository.LcmRetirementRpository.Get().Last().LCRetirementID;
                    _vmMsg.ReturnCode = dataSet.LCRetirementNo;//repository.LcmRetirementRpository.GetByID(_vmMsg.ReturnId).LCRetirementNo;
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
        public ValidationMsg UpdateLCM_LCRetirement(EXP_LCRetirement dataSet, int _userId)
        {
            try
            {
                EXP_LCRetirement obLCRetirement = repository.EXPLCRetirementRepository.GetByID(Convert.ToInt32(dataSet.LCRetirementID));


                obLCRetirement.LCRetirementNo = dataSet.LCRetirementNo;
                obLCRetirement.LCRetirementDate = DalCommon.SetDate(dataSet.LCRetirementDate.ToString());
                obLCRetirement.LCNo = dataSet.LCNo;
                obLCRetirement.LCID = dataSet.LCID;


                obLCRetirement.BillValue = (decimal)dataSet.BillValue;
                obLCRetirement.InterestPersent = (decimal)dataSet.InterestPersent;
                obLCRetirement.InterestAmount = (decimal)dataSet.InterestAmount;

                obLCRetirement.BankCommissionAmt = dataSet.BankCommissionAmt == null ? 0 : (decimal)dataSet.BankCommissionAmt;
                obLCRetirement.SwiftCharge = dataSet.SwiftCharge == null ? 0 : (decimal)dataSet.SwiftCharge;
                obLCRetirement.OtherCharge = dataSet.OtherCharge == null ? 0 : (decimal)dataSet.OtherCharge;//(decimal)dataSet.BankCommissionAmt;
                obLCRetirement.TotalAmount = (decimal)dataSet.TotalAmount;

                obLCRetirement.LCRCurrency = (byte)dataSet.LCRCurrency;
                obLCRetirement.Remarks = dataSet.Remarks;
                obLCRetirement.RecordStatus = "NCF";
                obLCRetirement.ExchangeRate = (decimal)dataSet.ExchangeRate == null ? 0 : (decimal)dataSet.ExchangeRate;
                obLCRetirement.ExchangeCurrency = (byte)(dataSet.ExchangeCurrency);
                obLCRetirement.ExchangeValue = dataSet.ExchangeValue == null ? 0 : (decimal)dataSet.ExchangeValue;//(decimal)dataSet.ExchangeValue;

                obLCRetirement.ModifiedOn = DateTime.Now;
                obLCRetirement.ModifiedBy = _userId;
                obLCRetirement.IPAddress = GetIPAddress.LocalIPAddress();

                repository.EXPLCRetirementRepository.Update(obLCRetirement);
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


                //_vmMsg.Type = Enums.MessageType.Error;
                //_vmMsg.Msg = "Data already confirmed. You can't update this.";

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
        
        public ValidationMsg DeleteLCRetirementID(int lcRetirementID)
        {
            try
            {
                var lcRetirementList = repository.EXPLCRetirementRepository.Get().Where(ob => ob.LCRetirementID == lcRetirementID).ToList();
                if (lcRetirementList.Count > 0)
                {
                    foreach (var lc in lcRetirementList)
                    {
                        repository.EXPLCRetirementRepository.Delete(lc);
                    }
                }
                repository.EXPLCRetirementRepository.Delete(lcRetirementID);

                save = repository.Save();
                if (save == 1)
                {
                    _vmMsg.Type = Enums.MessageType.Delete;
                    _vmMsg.Msg = "Deleted successfully .";
                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to delete the record.";
            }
            return _vmMsg;
        }

    }
}