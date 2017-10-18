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
    public class DalLCRetirement
    {

        private readonly BLC_DEVEntities obEntity;
        private UnitOfWork repository;
        private ValidationMsg _vmMsg;
        private int save;

        public DalLCRetirement()
        {
            obEntity = new BLC_DEVEntities();
            _vmMsg = new ValidationMsg();
            repository = new UnitOfWork();
        }
        public List<LCMLCRetirement> GetLCNoList()
        {
            var queryString = @"SELECT	lc.LCID,
                                        lc.LCNo,    
										CONVERT(VARCHAR(15),lc.LCDate, 106) LCDate,
										ISNULL(lc.LCAmount,0) LCAmount,		
										CIT.CIID,
										ISNULL(CIT.CINo,'') CINo,
										CIT.CITotalPrice CITotalValue,
										ISNULL(bdv.LCMargin,0)LessMargin
                                 FROM	     LCM_LCOpening lc 
											 INNER JOIN 
											 (
												SELECT		CI.CIID,
															CI.CINo, 
															CI.LCID,
															SUM(I.CITotalPrice)CITotalPrice  
												FROM		LCM_CommercialInvoice  CI
												INNER JOIN	LCM_CommercialInvoiceItem I 
												ON			I.CIID=CI.CIID 

								GROUP BY					CI.CIID,
															CI.LCID,
															CI.CINo
												) CIT 				
												ON CIT.LCID=lc.LCID
								INNER JOIN			LCM_BankDebitVoucher bdv 
												ON lc.LCID=bdv.LCID 
		                        ORDER BY			lc.LCID DESC";
            var lcList = obEntity.Database.SqlQuery<LCMLCRetirement>(queryString);
            return lcList.ToList();
        }
        public ValidationMsg SaveLCM_LCRetirement(LCMLCRetirement model, int _userId, string url)
        {
            try
            {
                LCM_LCRetirement objDBModel = new LCM_LCRetirement();

                objDBModel.LCRetirementNo = DalCommon.GetPreDefineNextCodeByUrl(url);
                objDBModel.LCRetirementDate = model.LCRetirementDate == null ? (DateTime?)null : DalCommon.SetDate(model.LCRetirementDate);//model.LCRetirementDate;
                objDBModel.LCNo = model.LCNo;
                objDBModel.LCID = model.LCID;
                objDBModel.BillValue = (decimal)model.BillValue == null ? 0 : (decimal)model.BillValue; ;
                objDBModel.InterestPersent = model.InterestPersent == null ? 0 : (decimal)model.InterestPersent;
                objDBModel.InterestAmount = model.InterestAmount == null ? 0 : (decimal)model.InterestAmount;
                objDBModel.BankCommissionAmt = model.BankCommissionAmt == null ? 0 : (decimal)model.BankCommissionAmt;
                objDBModel.OtherCharge = model.OtherCharge == null ? 0 : (decimal)model.OtherCharge;
                objDBModel.SwiftCharge = model.SwiftCharge == null ? 0 : (decimal)model.SwiftCharge;
                objDBModel.TotalAmount = model.TotalAmount == null ? 0 : (decimal)model.TotalAmount;
                objDBModel.GrandTotal = model.GrandTotal == null ? 0 : (decimal)model.GrandTotal;
                objDBModel.LessMargin = model.LessMargin == null ? 0 : (decimal)model.LessMargin;
                objDBModel.LCRCurrency = model.LCRCurrency;
                objDBModel.Remarks = model.Remarks == null ? "" : model.Remarks;
                objDBModel.ExchangeCurrency = model.ExchangeCurrency;
                objDBModel.ExchangeRate = (decimal)model.ExchangeRate == null ? 0 : (decimal)model.ExchangeRate;
                objDBModel.ExchangeValue = (decimal)model.ExchangeValue == null ? 0 : (decimal)model.ExchangeValue;
                objDBModel.RecordStatus = "NCF";
                objDBModel.SetOn = DateTime.Now;
                objDBModel.SetBy = _userId;
                objDBModel.IPAddress = GetIPAddress.LocalIPAddress();

                obEntity.LCM_LCRetirement.Add(objDBModel);
                int flag = obEntity.SaveChanges();
                if (flag == 1)
                {

                    _vmMsg.ReturnId = objDBModel.LCRetirementID;//repository.LcmRetirementRpository.Get().Last().LCRetirementID;
                    _vmMsg.ReturnCode = objDBModel.LCRetirementNo;//repository.LcmRetirementRpository.GetByID(_vmMsg.ReturnId).LCRetirementNo;

                    _vmMsg.Type = Enums.MessageType.Success;
                    _vmMsg.Msg = "Saved Successfully.";
                }
                else
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = " Error occured";
                }
            }
            catch (DbEntityValidationException e)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to save";
            }
            return _vmMsg;
        }
        public ValidationMsg UpdateLCM_LCRetirement(LCMLCRetirement model, int _userId)
        {
            try
            {
                LCM_LCRetirement obLCRetirement = repository.LcmRetirementRpository.GetByID(Convert.ToInt32(model.LCRetirementID));



                obLCRetirement.LCRetirementDate = model.LCRetirementDate == null ? (DateTime?)null : DalCommon.SetDate(model.LCRetirementDate); //model.LCRetirementDate == null ? (DateTime?)null : DalCommon.SetDate(model.LCRetirementDate);
                obLCRetirement.LCNo = model.LCNo;
                obLCRetirement.LCID = model.LCID;


                obLCRetirement.BillValue = model.BillValue;
                obLCRetirement.InterestPersent = model.InterestPersent;
                obLCRetirement.InterestAmount = model.InterestAmount;

                obLCRetirement.BankCommissionAmt = model.BankCommissionAmt == null ? 0 : model.BankCommissionAmt;
                obLCRetirement.SwiftCharge = model.SwiftCharge == null ? 0 : model.SwiftCharge;
                obLCRetirement.OtherCharge = model.OtherCharge == null ? 0 : model.OtherCharge;//model.BankCommissionAmt;
                obLCRetirement.TotalAmount = model.TotalAmount == null ? 0 : model.TotalAmount; ;
                obLCRetirement.LessMargin = model.LessMargin == null ? 0 : model.LessMargin; ;
                obLCRetirement.GrandTotal = model.GrandTotal == null ? 0 : model.GrandTotal; ;

                obLCRetirement.LCRCurrency = model.LCRCurrency;
                obLCRetirement.Remarks = model.Remarks;
                obLCRetirement.RecordStatus = "NCF";
                obLCRetirement.ExchangeCurrency = (model.ExchangeCurrency);
                obLCRetirement.ExchangeRate = model.ExchangeRate == null ? 0 : model.ExchangeRate;// model.ExchangeRate;
                obLCRetirement.ExchangeValue = model.ExchangeValue == null ? 0 : model.ExchangeValue;//(decimal)model.ExchangeValue;

                obLCRetirement.ModifiedOn = DateTime.Now;
                obLCRetirement.ModifiedBy = _userId;
                obLCRetirement.IPAddress = GetIPAddress.LocalIPAddress();

                repository.LcmRetirementRpository.Update(obLCRetirement);
                int flag = repository.Save();
                if (flag == 1)
                {
                    _vmMsg.Type = Enums.MessageType.Success;
                    _vmMsg.Msg = "Updated Successfully.";
                    _vmMsg.ReturnId = obLCRetirement.LCRetirementID;
                    _vmMsg.ReturnCode = obLCRetirement.LCRetirementNo;
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
                var lcRetirementList = repository.LcmRetirementRpository.Get().Where(ob => ob.LCRetirementID == lcRetirementID).ToList();
                if (lcRetirementList.Count > 0)
                {
                    foreach (var lc in lcRetirementList)
                    {
                        repository.LcmRetirementRpository.Delete(lc);
                    }
                }
                repository.LcmRetirementRpository.Delete(lcRetirementID);

                save = repository.Save();
                if (save == 1)
                {
                    _vmMsg.Type = Enums.MessageType.Delete;
                    _vmMsg.Msg = "Deleted Successfully .";
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
