using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using System.Data.Entity.Validation;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalEXPLCOpening
    {
        private readonly BLC_DEVEntities obEntity;
        private UnitOfWork repository;
        private ValidationMsg _vmMsg;
        public DalEXPLCOpening()
        {
            obEntity = new BLC_DEVEntities();
            _vmMsg = new ValidationMsg();
            repository = new UnitOfWork();
        }
        public IEnumerable<EXPLCOpening> GetPIInfo()
        {
            string sql = @"SELECT 
					                ISNULL(p.PIID,0)PIID,
					                ISNULL(p.PINo,0)PINo,
									ISNULL(p.PICurrency,0)PICurrency,
									ISNULL(c.CurrencyName,'')PICurrencyName,
					                ISNULL(CONVERT(varchar(11),p.PIDate,106),'')PIDate,
					                ISNULL(p.BeneficiaryAddressID,0)BeneficiaryAddressID,
									ISNULL(sa.Address,0)Address,
									ISNULL(p.PIBeneficiary,0)PIBeneficiary, 
									ISNULL(s.SupplierName,0)BeneficiaryName, 
					                p.RecordStatus
			                FROM	EXP_LeatherPI p
							LEFT JOIN Sys_Supplier s ON p.PIBeneficiary=s.SupplierID 
							LEFT JOIN Sys_Currency c ON p.PICurrency=c.CurrencyID 
							LEFT JOIN Sys_SupplierAddress sa ON p.BeneficiaryAddressID=sa.SupplierAddressID 

			                WHERE	p.RecordStatus='CNF' 
							ORDER BY p.PIID DESC";
            var result = obEntity.Database.SqlQuery<EXPLCOpening>(sql);
            return result;
        }
        public IEnumerable<EXPLCOpening> GetLCBankList(string bankCategory, string bankType)
        {
            string sql = @"SELECT			b.BankID,
											b.BankCode,
											(b.BankName +' , '+ br.BranchName) BankName, 
											br.BranchID,
											ISNULL(br.BanchCode,'')BranchCode,
											ISNULL(br.BranchName,'') BranchName,
											c.CurrencyID,
											ISNULL(c.CurrencyName,'') CurrencyName,	
											ISNULL(br.Address1,'') Address,
											ISNULL(br.LCLimit,0) LCLimit,
											ISNULL(b.BankBINNo,'') BankBINNo 
								FROM Sys_Bank b
								INNER JOIN Sys_Branch br ON b.BankID = br.BankID
								
								LEFT JOIN Sys_Currency c ON br.LCCurrency = c.CurrencyID
		
	                        WHERE 
			                        b.BankCategory='" + bankCategory + "' AND b.BankType ='" + bankType + "' AND b.IsActive='true' ORDER BY b.BankID";
            var result = obEntity.Database.SqlQuery<EXPLCOpening>(sql);
            return result;
        }




        public IEnumerable<EXPLCOpening> GetAdvisingBankList(string bankCategory, string bankType)
        {
            string sql = @"SELECT			b.BankID,
											b.BankCode,
											(b.BankName +' , '+ br.BranchName) BankName, 
											br.BranchID,
											ISNULL(br.BanchCode,'')BanchCode,
											ISNULL(br.BranchName,'') BranchName,
											br.BranchSwiftCode BranchSwiftCode,
											c.CurrencyID,
											ISNULL(c.CurrencyName,'') CurrencyName,	
											ISNULL(br.Address1,'') Address,
											ISNULL(br.LCLimit,0) LCLimit,
											ISNULL(b.BankBINNo,'') BankBINNo 
								FROM Sys_Bank b
								INNER JOIN Sys_Branch br ON b.BankID = br.BankID
								
								LEFT JOIN Sys_Currency c ON br.LCCurrency = c.CurrencyID
									WHERE b.BankCategory='" + bankCategory + "' AND b.BankType ='" + bankType + "' AND b.IsActive='true'";
            var result = obEntity.Database.SqlQuery<EXPLCOpening>(sql);
            return result;
        }


        public IEnumerable<EXPLCOpening> GetPIDetailsInfo(string piNo)
        {
            string sql = @"SELECT	pi.PIID,
		                            ISNULL(pi.PINo,'')PINo,
		                            CASE pi.PICategory
				                            WHEN 'CO' THEN 'Contact Order'
				                            WHEN 'PI' THEN 'Proforma Invoice'
				                            WHEN 'IO' THEN 'Indent Order'
		                            END PICategory,
		                            ISNULL(pi.PICurrency,0)PICurrency,
		                            C.CurrencyName PICurrencyName,	
		                            CONVERT(varchar(15),PIDate,106)PIDate,
		                            ISNULL(pi.LocalAgent,0)LocalAgent,
		                            (SELECT BuyerName FROM Sys_Buyer B WHERE B.BuyerID=pi.LocalAgent )LocalAgentName,
		                            ISNULL(pi.ForeignAgent,0)ForeignAgent,
		                            (SELECT BuyerName FROM Sys_Buyer B WHERE B.BuyerID=pi.ForeignAgent ) ForeignAgentName,
		                            ISNULL(pi.BuyerOrderID,0)BuyerOrderID,
		                            ISNULL(BO.BuyerOrderNo,'')BuyerOrderNo,
		                            ISNULL(pi.BuyerBank,0)BuyerBank,
		                            B1.BankName BuyerBankName,
		                            ISNULL(pi.BuyerBankAccount,'')BuyerBankAccount,
		                            ISNULL(pi.BeneficiaryBank,0)BeneficiaryBank,
		                            B2.BankName SellerBankName,		
		                            ISNULL(pi.BankAccount,'')BankAccount,
		
		                            CASE pi.PaymentMode
				                            WHEN 'ST' THEN 'Sight'
				                            WHEN 'DF' THEN 'Defered'				
		                            END PaymentMode,
		                            ISNULL(pi.PortofLoading,0)PortofLoading,
		                            P1.PortName LoadingPortName,
		                            ISNULL(pi.PortofDischarge,0)PortofDischarge,
		                            P2.PortName DischargePortName,
		                            ISNULL(pi.OfferValidityDays,0)OfferValidityDays 
		
                            FROM EXP_LeatherPI pi
                            LEFT JOIN SLS_BuyerOrder BO ON pi.BuyerOrderID=BO.BuyerOrderID

                            LEFT JOIN Sys_Branch BR1 ON pi.BuyerBank=BR1.BranchID
                            LEFT JOIN Sys_Bank B1 ON BR1.BankID=B1.BankID
                            LEFT JOIN Sys_Branch BR2 ON pi.BeneficiaryBank=BR2.BranchID
                            LEFT JOIN Sys_Bank B2 ON BR2.BankID=B2.BankID
                            LEFT JOIN Sys_Port P1 ON pi.PortofLoading=P1.PortID
                            LEFT JOIN Sys_Port P2 ON pi.PortofDischarge=P2.PortID
                            LEFT JOIN Sys_Currency C ON pi.PICurrency=C.CurrencyID
	                        WHERE 
			                        pi.PINo='" + piNo + "' ORDER BY pi.PIID DESC";
            var result = obEntity.Database.SqlQuery<EXPLCOpening>(sql);
            return result;
        }


        public ValidationMsg Save(EXPLCOpening dataSet, int _userId)
        {
            try
            {
                EXP_LCOpening objLcOpening = new EXP_LCOpening();

                objLcOpening.LCID = dataSet.LCID;
                objLcOpening.LCNo = dataSet.LCNo;//DalCommon.GetPreDefineNextCodeByUrl(pageUrl);
                objLcOpening.LCAmount = dataSet.LCAmount;
                objLcOpening.LCDate = DalCommon.SetDate(dataSet.LCDate);

                objLcOpening.LCCurrencyID = dataSet.LCCurrencyID;


                objLcOpening.PIID = dataSet.PIID == null ? 0 : dataSet.PIID;

                objLcOpening.ExpiryDate = DalCommon.SetDate(dataSet.ExpiryDate);
                objLcOpening.ExpiryPlace = dataSet.ExpiryPlace == null ? "" : dataSet.ExpiryPlace;
                objLcOpening.LastShipmentDate = DalCommon.SetDate(dataSet.LastShipmentDate);

                objLcOpening.Remarks = dataSet.Remarks;
                objLcOpening.LCState = "LCOP";
                objLcOpening.LCStatus = "GDSP";
                objLcOpening.RecordStatus = "NCF";
                objLcOpening.RunningStatus = "LCOP";
                objLcOpening.SetOn = DateTime.Now;
                objLcOpening.SetBy = _userId;
                objLcOpening.IPAddress = GetIPAddress.LocalIPAddress();

                //objLcOpening.RefLCNo = dataSet.RefLCNo == null ? "" : dataSet.RefLCNo;
                //objLcOpening.IssueDate = DalCommon.SetDate(dataSet.IssueDate);
                //objLcOpening.ExchangeCurrency = dataSet.ExchangeCurrency == null ? 0 : dataSet.ExchangeCurrency;
                //objLcOpening.ExchangeRate = dataSet.ExchangeRate == null ? 0 : (decimal)dataSet.ExchangeRate;
                //objLcOpening.ExchangeValue = dataSet.ExchangeValue == null ? 0 : (decimal)dataSet.ExchangeValue;
                //objLcOpening.NNDSendingTime = dataSet.NNDSendingTime == null ? 0 : (int)dataSet.NNDSendingTime;
                //objLcOpening.LCANo = dataSet.LCANo == null ? "" : dataSet.LCANo;
                //objLcOpening.IRCNo = dataSet.IRCNo == null ? "" : dataSet.IRCNo;
                //objLcOpening.VatRegNo = dataSet.VatRegNo == null ? "" : dataSet.VatRegNo;
                //objLcOpening.BINNo = dataSet.BINNo == null ? "" : dataSet.BINNo;
                //objLcOpening.ICNNo = dataSet.ICNNo == null ? "" : dataSet.ICNNo;
                //objLcOpening.TINNo = dataSet.ICNNo == null ? "" : dataSet.TINNo;
                //objLcOpening.ICNDate = DalCommon.SetDate(dataSet.ICNDate);
                //if (dataSet.BranchID != 0)
                //{
                //    objLcOpening.LCOpeningBank = dataSet.BranchID;
                //}
                //else
                //{
                //    objLcOpening.LCOpeningBank = 0;
                //}
                //if (dataSet.AdvisingBank != 0)
                //{
                //    objLcOpening.AdvisingBank = dataSet.AdvisingBank;
                //}
                //if (dataSet.Beneficiary != 0)
                //{
                //    objLcOpening.Beneficiary = dataSet.PIBeneficiary;
                //}

                //if (dataSet.BeneficiaryAddressID != 0)
                //{
                //    objLcOpening.BeneficiaryAddressID = dataSet.BeneficiaryAddressID;
                //}
                //objLcOpening.LCReviceNo = dataSet.LCReviceNo;
                //objLcOpening.LCReviceDate = DateTime.Now;

                obEntity.EXP_LCOpening.Add(objLcOpening);
                int flag = obEntity.SaveChanges();
                if (flag == 1)
                {

                    _vmMsg.ReturnId = objLcOpening.LCID;
                    _vmMsg.ReturnCode = objLcOpening.LCNo;
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

        public ValidationMsg Update(EXP_LCOpening dataSet, int _userId)
        {
            try
            {
                EXP_LCOpening obLcOpening = repository.ExpLCOpening.GetByID(dataSet.LCID);


                    obLcOpening.LCID = dataSet.LCID;
                    obLcOpening.LCNo = dataSet.LCNo;
                    obLcOpening.LCDate = dataSet.LCDate;
                    obLcOpening.LCAmount = dataSet.LCAmount;
                    obLcOpening.LCCurrencyID = dataSet.LCCurrencyID;

                    obLcOpening.PIID = dataSet.PIID;

                    obLcOpening.ExpiryDate = dataSet.ExpiryDate;
                    obLcOpening.ExpiryPlace = dataSet.ExpiryPlace;
                    obLcOpening.LastShipmentDate = dataSet.LastShipmentDate;
                    obLcOpening.Remarks = dataSet.Remarks;



                    obLcOpening.RecordStatus = "NCF";
                    obLcOpening.RunningStatus = dataSet.RunningStatus == null ? "" : dataSet.RunningStatus;
                    obLcOpening.LCReviceDate = DateTime.Now;
                    obLcOpening.LCState = dataSet.LCState == null ? "" : dataSet.LCState;
                    obLcOpening.LCStatus = dataSet.LCStatus == null ? "" : dataSet.LCStatus;


                    obLcOpening.ModifiedOn = DateTime.Now;
                    obLcOpening.ModifiedBy = _userId;
                    obLcOpening.IPAddress = GetIPAddress.LocalIPAddress();
                    





                    repository.ExpLCOpening.Update(obLcOpening);
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

        public ValidationMsg ConfirmRecordStatus(string lcId, int userId)
        {
            try
            {
                EXP_LCOpening ob = repository.ExpLCOpening.GetByID(Convert.ToInt32(lcId));
                if (lcId != "" && ob.RecordStatus == "NCF")
                {
                        ob.RecordStatus = "CNF";
                        ob.LCStatus = "RNG";
                        ob.LCState = "ODDN";
   
                        ob.ApproveDate = DateTime.Now;
                        repository.ExpLCOpening.Update(ob);
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
