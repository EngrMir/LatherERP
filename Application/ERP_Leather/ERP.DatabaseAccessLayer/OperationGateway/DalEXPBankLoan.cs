using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.ComponentModel.DataAnnotations.Schema;
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
    public class DalEXPBankLoan
    {
        private readonly BLC_DEVEntities _context = new BLC_DEVEntities();
        UnitOfWork repository = new UnitOfWork();
        private ValidationMsg _vmMsg = new ValidationMsg();
        private int save;


        public DalEXPBankLoan()
        {
            _context = new BLC_DEVEntities();
            _vmMsg = new ValidationMsg();
        }

        public IEnumerable<EXPBankLoan> GetLCBankList()
        {
            string sql = @"SELECT				b.BankID BankID,
					                            ISNULL(b.BankCode,'')BankCode,
					                            ISNULL(b.BankName,'')BankName,
					                            br.BranchID BranchID,
					                            ISNULL(br.BanchCode,'')BanchCode,
					                            ISNULL(br.BranchName,'')BranchName                 
                            FROM				Sys_Bank b
                            LEFT JOIN			Sys_Branch br		ON b.BankID = br.BankID
                                                   
                            WHERE				b.BankCategory='BNK' 
											AND b.BankType='LOC' 
											AND b.IsActive='true' 
                            ORDER BY b.BankCode ASC";
            var result = _context.Database.SqlQuery<EXPBankLoan>(sql);
            return result;
        }

        public IEnumerable<EXPBankLoan> GetCIList()
        {
            string sql = @"SELECT											
			                        ISNULL(c.CIID,0)CIID,
			                        ISNULL(c.CINo,'')CINo,
			                        ISNULL(c.CIRefNo,'')CIRefNo,	
			                        ISNULL(CONVERT(VARCHAR(12),c.CIDate, 106),'')CIDate,
			                        CASE  c.RecordStatus 
						                        WHEN 'NCF' THEN 'Not Confirm'
						                        WHEN 'CNF' THEN 'Confirmed'
						                        WHEN 'CHK' THEN 'Checked'                            
			                        END		RecordStatus

                        FROM		EXP_CI c
                     
                        WHERE		c.RecordStatus='CNF'
                        ORDER BY	c.CIID DESC";
            var result = _context.Database.SqlQuery<EXPBankLoan>(sql);
            return result;
        }
        public IEnumerable<EXPBankLoan> GetHeadList()
        {
            string sql = @"SELECT											
			                            t.HeadID,
			                            ISNULL(t.HeadCode,'')HeadCode,
			                            ISNULL(t.HeadName,'')HeadName	                           
                            FROM		Sys_TransHead t              
                            WHERE		t.IsActive='true'
                            ORDER BY	t.HeadID DESC";
            var result = _context.Database.SqlQuery<EXPBankLoan>(sql);
            return result;
        }

        public ValidationMsg Save(EXPBankLoan model, int _userid, string pageUrl)//, string pageUrl
        {
            _vmMsg = new ValidationMsg();

            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        if (model.expBankLoanList.Count > 0)
                        {
                            foreach (var bankLoan in model.expBankLoanList)
                            {
                                EXP_BankLoan tblBankLoan = SetToBankLoanModelObject(bankLoan, _userid);
                                tblBankLoan.BankID = model.BankID;
                                tblBankLoan.BranchID = model.BranchID;
                                tblBankLoan.ApprovalNote = model.ApprovalNote;
                                _context.EXP_BankLoan.Add(tblBankLoan);
                                _context.SaveChanges();
                            }
                        }
                        _context.SaveChanges();
                        tx.Complete();

                        _vmMsg.Type = Enums.MessageType.Success;
                        _vmMsg.Msg = "Saved Successfully.";  
                    }
                }
            }


            catch (DbEntityValidationException e)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Save.";
            }
            return _vmMsg;
        }
        public EXP_BankLoan SetToBankLoanModelObject(EXPBankLoan model, int _userid)//int _userid
        {
            EXP_BankLoan entity = new EXP_BankLoan();
            //entity.BankLoanID = model.BankLoanID;
            entity.BankLoanNo = model.BankLoanNo;
            //entity.BankID = model.BankID;
            //entity.BranchID = model.BranchID;
            entity.LoanHead = model.LoanHead;
            entity.RefACNo = model.RefACNo;
            entity.RefACName = model.RefACName;
            var GridloanRecieveDate = model.LoanReceiveDate.Contains("/") ? model.LoanReceiveDate : Convert.ToDateTime(model.LoanReceiveDate).ToString("dd/MM/yyyy");
            entity.LoanReceiveDate = DalCommon.SetDate(GridloanRecieveDate);
            entity.LoanAmt = model.LoanAmt;
            entity.InterestPercent = model.InterestPercent;
            entity.LoanLimit = model.LoanLimit;
            entity.ReturnedAmt = model.ReturnedAmt;
            entity.BalanceAmt = model.BalanceAmt;
            entity.CIID = model.CIID;
            entity.Remarks = model.Remarks;
            entity.RecordStatus = "NCF";
            entity.RunningStatus = "RNG";
            entity.SetOn = DateTime.Now;
            entity.SetBy = _userid;
            entity.IPAddress = GetIPAddress.LocalIPAddress();

            return entity;
        }



        #region UPDATE Bank Loan DATA
        public ValidationMsg Update(EXPBankLoan model, int _userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                #region Update
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {

                        #region Save NEW Data & Update Existing Crust Leather Transfer Data

                        if (model.expBankLoanList != null)
                        {
                            foreach (EXPBankLoan bankLoan in model.expBankLoanList)
                            {
                                bankLoan.BankID = model.BankID;
                                bankLoan.BranchID = model.BranchID;
                                bankLoan.ApprovalNote = model.ApprovalNote;
                                if (bankLoan.BankLoanID == 0)
                                {                            
                                    EXP_BankLoan tblBankLoan = SetToBankLoanModelObject(bankLoan, _userid);
                                    tblBankLoan.BankID = bankLoan.BankID;
                                    tblBankLoan.BranchID = bankLoan.BranchID;
                                    tblBankLoan.ApprovalNote = bankLoan.ApprovalNote;
                                    _context.EXP_BankLoan.Add(tblBankLoan);

                                }
                                else
                                {
                                    EXP_BankLoan bankLoanEntity = SetToBankLoanModelObject(bankLoan, _userid);
                                    model.BankID = bankLoanEntity.BankID;
                                    model.BranchID = bankLoanEntity.BranchID;
                                    model.ApprovalNote = bankLoanEntity.ApprovalNote;
                                    var obBankLoan = _context.EXP_BankLoan.First(m => m.BankLoanID == bankLoan.BankLoanID);

                                    //obBankLoan.BankLoanID = model.BankLoanID;
                                    obBankLoan.BankLoanNo = bankLoanEntity.BankLoanNo;
                                    obBankLoan.CIID = bankLoanEntity.CIID;
                                    obBankLoan.LoanHead = bankLoanEntity.LoanHead;
                                    obBankLoan.RefACNo = bankLoanEntity.RefACNo;
                                    obBankLoan.RefACName = bankLoanEntity.RefACName;
                                    //var GridloanRecieveDate = bankLoanEntity.LoanReceiveDate.Contains("/") ? bankLoanEntity.LoanReceiveDate : Convert.ToDateTime(bankLoanEntity.LoanReceiveDate).ToString("dd/MM/yyyy");
                                    //obBankLoan.LoanReceiveDate = DalCommon.SetDate(GridloanRecieveDate);
                                    obBankLoan.LoanAmt = bankLoanEntity.LoanAmt;
                                    obBankLoan.InterestPercent = bankLoanEntity.InterestPercent;
                                    obBankLoan.LoanLimit = bankLoanEntity.LoanLimit;
                                    obBankLoan.ReturnedAmt = bankLoanEntity.ReturnedAmt;
                                    obBankLoan.BalanceAmt = bankLoanEntity.BalanceAmt;
                                    obBankLoan.Remarks = bankLoanEntity.Remarks;
                                    obBankLoan.RunningStatus = bankLoanEntity.RunningStatus;
                                    obBankLoan.RecordStatus = "NCF";
                                    obBankLoan.ModifiedOn = DateTime.Now;
                                    obBankLoan.ModifiedBy = _userid;
                                    obBankLoan.IPAddress = GetIPAddress.LocalIPAddress();
                                }
                            }
                        }
                        #endregion

                        _context.SaveChanges();
                        tx.Complete();

                        _vmMsg.Type = Enums.MessageType.Update;
                        _vmMsg.Msg = "Updated Successfully.";
                    }
                }

                #endregion
            }
            catch (DbEntityValidationException e)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Update.";
            }
            return _vmMsg;
        }
        #endregion

        #region SEARCH Bank Loan DATA

        public List<EXPBankLoan> SearchBankLoanDetails()
        {
            var query = @"SELECT		ebl.BankLoanID,
			                            ISNULL(ebl.BankLoanNo,'')BankLoanNo,
			                            ISNULL(ebl.LoanHead,0)LoanHead,
			                            ISNULL(th.HeadName,'')HeadName,
			                            ISNULL(ebl.CIID,0)CIID,
			                            ISNULL(ci.CINo,'')CIRefNo,
			                            ISNULL(ebl.LoanLimit,0)LoanLimit,
			                            ISNULL(ebl.RefACNo,'')RefACNo,
			                            ISNULL(ebl.RefACName,'')RefACName,
			                            CONVERT(VARCHAR(15),ebl.LoanReceiveDate, 106) LoanReceiveDate,
			                            ISNULL(ebl.LoanAmt,0)LoanAmt,
			                            ISNULL(ebl.ReturnedAmt,0)ReturnedAmt,
			                            ISNULL(ebl.BalanceAmt,0)BalanceAmt,
										ISNULL(ebl.InterestPercent,0)InterestPercent,
			                            ISNULL(ebl.Remarks,'')Remarks,
										ISNULL(ebl.RunningStatus,'')RunningStatus,
			                            CASE  ebl.RecordStatus 
					                            WHEN 'NCF' THEN 'Not Confirmed'
					                            WHEN 'CNF' THEN 'Confirmed'
					                            WHEN 'CHK' THEN 'Checked'
                                        END RecordStatus,
			                            ISNULL(ebl.ApprovalNote,'')ApprovalNote

                            FROM		EXP_BankLoan ebl
                            INNER JOIN	Sys_TransHead th ON th.HeadID=ebl.LoanHead
                            INNER JOIN	EXP_CI ci ON ci.CIID=ebl.CIID
							WHERE ebl.RunningStatus='RNG'";

            var result = _context.Database.SqlQuery<EXPBankLoan>(query).ToList();
            return result;

        }
        #endregion


        #region SEARCH Bank Loan DATA on Selected Bank

        public List<EXPBankLoan> SearchBankLoanDetailsOnSelectedBank(long BankID, long BranchID)
        {
            var query = @"SELECT		ebl.BankLoanID,
			                            ISNULL(ebl.BankID,0)BankID,
										ISNULL(ebl.BranchID,0)BranchID,
			                            ISNULL(ebl.BankLoanNo,'')BankLoanNo,
			                            ISNULL(ebl.LoanHead,0)LoanHead,
			                            ISNULL(th.HeadName,'')HeadName,
			                            ISNULL(ebl.CIID,0)CIID,
			                            ISNULL(ci.CINo,'')CIRefNo,
			                            ISNULL(ebl.LoanLimit,0)LoanLimit,
			                            ISNULL(ebl.RefACNo,'')RefACNo,
			                            ISNULL(ebl.RefACName,'')RefACName,
			                            CONVERT(VARCHAR(15),ebl.LoanReceiveDate, 106) LoanReceiveDate,
			                            ISNULL(ebl.LoanAmt,0)LoanAmt,
			                            ISNULL(ebl.ReturnedAmt,0)ReturnedAmt,
			                            ISNULL(ebl.BalanceAmt,0)BalanceAmt,
										ISNULL(ebl.InterestPercent,0)InterestPercent,
			                            ISNULL(ebl.Remarks,'')Remarks,
										ISNULL(ebl.RunningStatus,'')RunningStatus,
			                            CASE  ebl.RecordStatus 
					                            WHEN 'NCF' THEN 'Not Confirmed'
					                            WHEN 'CNF' THEN 'Confirmed'
					                            WHEN 'CHK' THEN 'Checked'
                                        END RecordStatus,
			                            ISNULL(ebl.ApprovalNote,'')ApprovalNote

                            FROM		EXP_BankLoan ebl
                            INNER JOIN	Sys_TransHead th ON th.HeadID=ebl.LoanHead
                            INNER JOIN	EXP_CI ci ON ci.CIID=ebl.CIID
							WHERE ebl.RunningStatus='RNG' AND ebl.BankID='" + BankID + "' AND ebl.BranchID='" + BranchID + "'";

            var result = _context.Database.SqlQuery<EXPBankLoan>(query).ToList();
            return result;

        }
        #endregion

        public ValidationMsg DeleteBankLoanList(int bankLoanID)
        {
            try
            {
                var cnfBillList = repository.ExpBankLoanRepository.Get().Where(ob => ob.BankLoanID == bankLoanID).ToList();
                if (cnfBillList.Count > 0)
                {
                    foreach (var cnfBill in cnfBillList)
                    {
                        repository.ExpBankLoanRepository.Delete(cnfBill);
                    }
                }
                repository.ExpBankLoanRepository.Delete(bankLoanID);

                save = repository.Save();
                if (save == 1)
                {
                    _vmMsg.Type = Enums.MessageType.Delete;
                    _vmMsg.Msg = "Deleted Successfully";
                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Delete the Record.";
            }
            return _vmMsg;
        }

        public ValidationMsg CloseLoanData(EXPBankLoan model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        if (model.expBankLoanList[0].BankLoanID != 0 && model.RecordStatus == "NCF")
                        {

                            foreach (var bankLoan in model.expBankLoanList)
                            {
                                EXP_BankLoan tblBankLoan = SetToBankLoanEntityObject(bankLoan, userid);
                                tblBankLoan.RunningStatus = "CLS";
                                _context.SaveChanges();
                            }
                        }
                        _vmMsg.Type = Enums.MessageType.Success;
                        _vmMsg.Msg = "Loan Closed Successfully.";
                    }
                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Close Loan.";
            }
            return _vmMsg;
        }


        public EXP_BankLoan SetToBankLoanEntityObject(EXPBankLoan model, int _userid)//int _userid
        {
            EXP_BankLoan entity = new EXP_BankLoan();
            entity.BankLoanID = model.BankLoanID;
            entity.BankLoanNo = model.BankLoanNo;
            //entity.BankID = model.BankID;
            //entity.BranchID = model.BranchID;
            entity.LoanHead = model.LoanHead;
            entity.RefACNo = model.RefACNo;
            entity.RefACName = model.RefACName;
            var GridloanRecieveDate = model.LoanReceiveDate.Contains("/") ? model.LoanReceiveDate : Convert.ToDateTime(model.LoanReceiveDate).ToString("dd/MM/yyyy");
            entity.LoanReceiveDate = DalCommon.SetDate(GridloanRecieveDate);
            entity.LoanAmt = model.LoanAmt;
            entity.InterestPercent = model.InterestPercent;
            entity.LoanLimit = model.LoanLimit;
            entity.ReturnedAmt = model.ReturnedAmt;
            entity.BalanceAmt = model.BalanceAmt;
            entity.CIID = model.CIID;
            entity.Remarks = model.Remarks;
            entity.RecordStatus = "NCF";
            entity.RunningStatus = "CLS";
            entity.SetOn = DateTime.Now;
            entity.SetBy = _userid;
            entity.IPAddress = GetIPAddress.LocalIPAddress();

            return entity;
        }



    }
}

