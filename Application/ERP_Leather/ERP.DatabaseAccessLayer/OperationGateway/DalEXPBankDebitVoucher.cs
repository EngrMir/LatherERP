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
    public class DalEXPBankDebitVoucher
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;
        public long BDVID = 0;
        public string BDVNo = string.Empty;

        public DalEXPBankDebitVoucher()
        {
            _context = new BLC_DEVEntities();
        }

        public ValidationMsg Save(EXPBankDebitVoucher model, int userid, string pageUrl)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                #region Save

                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        model.BDVNo = DalCommon.GetPreDefineNextCodeByUrl(pageUrl);//DalCommon.GetPreDefineValue("1", "00045");
                        if (model.BDVNo != null)
                        {
                            EXP_BankDebitVoucher tblEXPCI = SetToModelObject(model, userid);
                            _context.EXP_BankDebitVoucher.Add(tblEXPCI);
                            _context.SaveChanges();

                            tx.Complete();
                            BDVID = tblEXPCI.BDVID;
                            BDVNo = model.BDVNo;
                            _vmMsg.Type = Enums.MessageType.Success;
                            _vmMsg.Msg = "Saved Successfully.";
                        }
                        else
                        {
                            _vmMsg.Type = Enums.MessageType.Error;
                            _vmMsg.Msg = "BDVNo Predefine Value not Found.";
                        }
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to save.";
                if (ex.InnerException.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Schedule Year,Month Combination Data Already Exit.";
                }
            }
            return _vmMsg;
        }

        public ValidationMsg Update(EXPBankDebitVoucher model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                #region Update

                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {

                        EXP_BankDebitVoucher CurrentEntity = SetToModelObject(model, userid);
                        var OriginalEntity = _context.EXP_BankDebitVoucher.First(m => m.BDVID == model.BDVID);

                        //OriginalEntity.BDVNo = CurrentEntity.BDVNo;
                        OriginalEntity.BankID = CurrentEntity.BankID;
                        //OriginalEntity.LCNo = CurrentEntity.LCNo.ToString();
                        OriginalEntity.BranchID = CurrentEntity.BranchID;
                        //OriginalEntity.LCID = CurrentEntity.LCID;
                        OriginalEntity.BDVNo = CurrentEntity.BDVNo;
                        OriginalEntity.BDVDate = CurrentEntity.BDVDate;
                        OriginalEntity.ExchangeCurrency = CurrentEntity.ExchangeCurrency;
                        OriginalEntity.ExchangeRate = CurrentEntity.ExchangeRate;
                        OriginalEntity.ExchangeAmount = CurrentEntity.ExchangeAmount;
                        OriginalEntity.LCMargin = CurrentEntity.LCMargin;
                        OriginalEntity.Remarks = CurrentEntity.Remarks;
                        OriginalEntity.CommissionAmt = CurrentEntity.CommissionAmt;
                        OriginalEntity.PostageAmt = CurrentEntity.PostageAmt;
                        OriginalEntity.SwiftCharge = CurrentEntity.SwiftCharge;
                        OriginalEntity.SourceTaxAmt = CurrentEntity.SourceTaxAmt;
                        OriginalEntity.VatAmt = CurrentEntity.VatAmt;
                        OriginalEntity.StationaryExpense = CurrentEntity.StationaryExpense;
                        OriginalEntity.OtherCost = CurrentEntity.OtherCost;
                        OriginalEntity.TotalAmount = CurrentEntity.TotalAmount;
                        OriginalEntity.ApprovalAdvice = CurrentEntity.ApprovalAdvice;
                        OriginalEntity.RecordStatus = "NCF";
                        OriginalEntity.ModifiedBy = userid;
                        OriginalEntity.ModifiedOn = DateTime.Now;

                        _context.SaveChanges();
                        tx.Complete();
                        BDVID = model.BDVID;
                        BDVNo = model.BDVNo;
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

                if (ex.InnerException.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Schedule Year,Month Combination Data Already Exit.";
                }
            }
            return _vmMsg;
        }

        public long GetBDVID()
        {
            return BDVID;
        }

        public string GetBDVNo()
        {
            return BDVNo;
        }

        public EXP_BankDebitVoucher SetToModelObject(EXPBankDebitVoucher model, int userid)
        {
            EXP_BankDebitVoucher Entity = new EXP_BankDebitVoucher();

            Entity.BankID = model.BankID;
            //Entity.LCNo = model.LCNo.ToString();
            //Entity.LCID = model.LCID;
            Entity.BranchID = model.BranchID;
            //Entity.LCID = model.LCID;
            Entity.BDVNo = model.BDVNo;
            Entity.BDVDate = DalCommon.SetDate(model.BDVDate);
            Entity.ExchangeCurrency = model.ExchangeCurrency;
            Entity.ExchangeRate = model.ExchangeRate;
            Entity.ExchangeAmount = model.ExchangeAmount;
            Entity.LCMargin = model.LCMargin;
            Entity.Remarks = model.Remarks;
            Entity.CommissionAmt = model.CommissionAmt;
            Entity.PostageAmt = model.PostageAmt;
            Entity.SwiftCharge = model.SwiftCharge;
            Entity.SourceTaxAmt = model.SourceTaxAmt;
            Entity.VatAmt = model.VatAmt;
            Entity.StationaryExpense = model.StationaryExpense;
            Entity.OtherCost = model.OtherCost;
            Entity.TotalAmount = model.TotalAmount;
            Entity.RecordStatus = "NCF";
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }

        public List<EXPBankDebitVoucher> GetBDVInformation()
        {
            List<EXP_BankDebitVoucher> searchList = _context.EXP_BankDebitVoucher.OrderByDescending(m => m.CIID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<EXPBankDebitVoucher>();
        }

        public EXPBankDebitVoucher SetToBussinessObject(EXP_BankDebitVoucher Entity)
        {
            EXPBankDebitVoucher Model = new EXPBankDebitVoucher();

            Model.BDVID = Entity.BDVID;
            Model.BDVNo = Entity.BDVNo;
            Model.BDVDate = Convert.ToDateTime(Entity.BDVDate).ToString("dd/MM/yyyy");
            Model.BankID = Entity.BankID;
            Model.BankName = Entity.BankID == null ? "" : _context.Sys_Bank.Where(m => m.BankID == Entity.BankID).FirstOrDefault().BankName;
            Model.LCID = Entity.LCID;
            Model.LCNo = Entity.LCID == null ? "" : _context.EXP_LCOpening.Where(m => m.LCID == Entity.LCID).FirstOrDefault().LCNo;
            Model.BranchID = Entity.BranchID;
            Model.BranchName = Entity.BranchID == null ? "" : _context.Sys_Branch.Where(m => m.BranchID == Entity.BranchID).FirstOrDefault().BranchName;
            Model.CIID = Entity.CIID;
            Model.CINo = Entity.CIID == null ? "" : _context.EXP_CI.Where(m => m.CIID == Entity.CIID).FirstOrDefault().CINo;
            Model.PIID = Entity.PIID;
            Model.PINo = Entity.PIID == null ? "" : _context.EXP_LeatherPI.Where(m => m.PIID == Entity.PIID).FirstOrDefault().PINo;
            Model.ExchangeCurrency = Entity.ExchangeCurrency;
            Model.ExchangeRate = Entity.ExchangeRate;
            Model.ExchangeAmount = Entity.ExchangeAmount;
            Model.BankCharge = Entity.BankCharge;
            Model.LCMargin = Entity.LCMargin;
            Model.CommissionAmt = Entity.CommissionAmt;
            Model.PostageAmt = Entity.PostageAmt;
            Model.SwiftCharge = Entity.SwiftCharge;
            Model.SourceTaxAmt = Entity.SourceTaxAmt;
            Model.VatAmt = Entity.VatAmt;
            Model.StationaryExpense = Entity.StationaryExpense;
            Model.OtherCost = Entity.OtherCost;
            Model.TotalAmount = Entity.TotalAmount;
            Model.Remarks = Entity.Remarks;
            Model.RecordStatus = Entity.RecordStatus;

            return Model;
        }

        public ValidationMsg ConfirmedEXPBDV(string BDVID, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        long bdvId = Convert.ToInt64(BDVID);
                        var originalEntityCI = _context.EXP_BankDebitVoucher.First(m => m.BDVID == bdvId);
                        originalEntityCI.RecordStatus = "CNF";
                        originalEntityCI.ModifiedBy = userid;
                        originalEntityCI.ModifiedOn = DateTime.Now;
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

        public ValidationMsg CheckedEXPBDV(string BDVID, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        long bdvId = Convert.ToInt64(BDVID);
                        var originalEntityCI = _context.EXP_BankDebitVoucher.First(m => m.BDVID == bdvId);
                        originalEntityCI.RecordStatus = "CHK";
                        originalEntityCI.ModifiedBy = userid;
                        originalEntityCI.ModifiedOn = DateTime.Now;
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
    }
}
