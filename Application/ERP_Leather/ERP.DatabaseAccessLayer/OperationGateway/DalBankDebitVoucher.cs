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
using System.Web;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalBankDebitVoucher
    {
        private UnitOfWork repo; 
       
        private ValidationMsg _vmMsg;
        private SysCommonUtility _utility;
        public DalBankDebitVoucher()
        {
            _vmMsg = new ValidationMsg();
             repo = new UnitOfWork();
             _utility = new SysCommonUtility();
        }
        public ValidationMsg Save(LcmBankDebitVoucher dataSet)
        {
            try
            {
                LCM_BankDebitVoucher ob = new LCM_BankDebitVoucher();
                if (dataSet.BranchID > 0) { ob.BankID = repo.BranchRepository.GetByID(dataSet.BranchID).BankID; }

                ob.LCNo = dataSet.LCNo.ToString();
                if (dataSet.LCID > 0) { ob.LCID = Convert.ToInt32(dataSet.LCID); }
                if (dataSet.BranchID > 0) { ob.BranchID = Convert.ToInt32(dataSet.BranchID); }


                ob.BDVNo = dataSet.BDVNo.ToString();
                ob.BDVDate = DalCommon.SetDate(dataSet.BDVDate);
                if (dataSet.ExchangeCurrency > 0) { ob.ExchangeCurrency = (byte)dataSet.ExchangeCurrency; }

                ob.ExchangeRate = dataSet.ExchangeRate == null ? 0 : Convert.ToDecimal(dataSet.ExchangeRate);
                ob.ExchangeAmount = dataSet.ExchangeAmount == null ? 0 : Convert.ToDecimal(ob.ExchangeAmount);
                ob.LCMargin = dataSet.LCMargin == null ? 0 : Convert.ToDecimal(dataSet.LCMargin);
                ob.Remarks = dataSet.Remarks;
                ob.CommissionAmt = dataSet.CommissionAmt == null ? 0 : Convert.ToDecimal(dataSet.CommissionAmt);
                ob.PostageAmt = dataSet.PostageAmt == null ? 0 : Convert.ToDecimal(dataSet.PostageAmt);
                ob.SwiftCharge = dataSet.SwiftCharge == null ? 0 : Convert.ToDecimal(dataSet.SwiftCharge);
                ob.SourceTaxAmt = dataSet.SourceTaxAmt == null ? 0 : Convert.ToDecimal(dataSet.SourceTaxAmt);
                ob.VatAmt = dataSet.VatAmt == null ? 0 : Convert.ToDecimal(dataSet.VatAmt);
                ob.StationaryExpense = dataSet.StationaryExpense == null ? 0 : Convert.ToDecimal(dataSet.StationaryExpense);
                ob.OtherCost = dataSet.OtherCost == null ? 0 : Convert.ToDecimal(dataSet.OtherCost);
                ob.TotalAmount = dataSet.TotalAmount == null ? 0 : Convert.ToDecimal(dataSet.TotalAmount);
                ob.ApprovalAdvice = dataSet.ApprovalAdvice;
                ob.RecordStatus = "NCF";
                ob.SetOn = DateTime.Now;
                ob.SetBy = Convert.ToInt32(HttpContext.Current.Session["UserID"]);
                ob.ModifiedBy = Convert.ToInt32(HttpContext.Current.Session["UserID"]);
                ob.ModifiedOn = DateTime.Now;
                ob.LCAmt = dataSet.LCAmt == null ? 0 : (decimal)dataSet.LCAmt;
                ob.VatOnSwift = dataSet.VatOnSwift == null ? 0 : (decimal)dataSet.VatOnSwift;
                ob.InsuranceCost = dataSet.InsuranceCost == null ? 0 : (decimal)dataSet.InsuranceCost;
                ob.AccountNo = dataSet.AccountNo;
                ob.OpeningStampCharge = dataSet.OpeningStampCharge == null ? 0 : (decimal)dataSet.OpeningStampCharge;
                repo.LcmBankDebitVoucherRpository.Insert(ob);
                int flag = repo.Save(); // obEntity.SaveChanges();
                if (flag == 1)
                {
                    _vmMsg.ReturnId = ob.BDVID;
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

        public ValidationMsg Update(LcmBankDebitVoucher dataSet)
        {
            try
            {
                LCM_BankDebitVoucher ob = repo.LcmBankDebitVoucherRpository.GetByID(Convert.ToInt32(dataSet.BDVID));
                if (!ob.RecordStatus.Equals("CNF"))
                {

                    ob.LCNo = dataSet.LCNo.ToString();
                    // ob.LCID = Convert.ToInt32(dataSet.LCID);
                    if (dataSet.LCID > 0) { ob.LCID = Convert.ToInt32(dataSet.LCID); }
                    if (dataSet.BranchID > 0)
                    {
                        ob.BranchID = Convert.ToInt32(dataSet.BranchID);
                        ob.BankID = repo.BranchRepository.GetByID(dataSet.BranchID).BankID;
                    }
          
                    ob.BDVNo = dataSet.BDVNo.ToString();
                    ob.BDVDate = DalCommon.SetDate(dataSet.BDVDate);
                    if (dataSet.ExchangeCurrency > 0) { ob.ExchangeCurrency = (byte)dataSet.ExchangeCurrency; }
                    ob.ExchangeRate = Convert.ToDecimal(dataSet.ExchangeRate == null ? 0 : dataSet.ExchangeRate);
                    ob.ExchangeAmount = Convert.ToDecimal(dataSet.ExchangeAmount == null ? 0 : dataSet.ExchangeAmount);
                    ob.LCMargin = Convert.ToDecimal(dataSet.LCMargin == null ? 0 : dataSet.LCMargin);
                    ob.Remarks = dataSet.Remarks;
                    ob.CommissionAmt = Convert.ToDecimal(dataSet.CommissionAmt == null ? 0 : dataSet.CommissionAmt);
                    ob.PostageAmt = Convert.ToDecimal(dataSet.PostageAmt == null ? 0 : dataSet.PostageAmt);
                    ob.SwiftCharge = Convert.ToDecimal(dataSet.SwiftCharge == null ? 0 : dataSet.SwiftCharge);
                    ob.SourceTaxAmt = Convert.ToDecimal(dataSet.SourceTaxAmt == null ? 0 : dataSet.SourceTaxAmt);
                    ob.VatAmt = Convert.ToDecimal(dataSet.VatAmt == null ? 0 : dataSet.VatAmt);
                    ob.StationaryExpense = Convert.ToDecimal(dataSet.StationaryExpense == null ? 0 : dataSet.StationaryExpense);
                    ob.OtherCost = Convert.ToDecimal(dataSet.OtherCost == null ? 0 : dataSet.OtherCost);
                    ob.TotalAmount = Convert.ToDecimal(dataSet.TotalAmount == null ? 0 : dataSet.TotalAmount);
                    ob.ApprovalAdvice = dataSet.ApprovalAdvice;
                    ob.ModifiedBy = Convert.ToInt32(HttpContext.Current.Session["UserID"]);
                    ob.ModifiedOn = DateTime.Now;
                    if (dataSet.LCAmt != null) { ob.LCAmt = (decimal)dataSet.LCAmt; }
                    else { ob.LCAmt = 0; }

                    ob.VatOnSwift = Convert.ToDecimal(dataSet.VatOnSwift == null ? 0 : dataSet.VatOnSwift);
                    ob.InsuranceCost = Convert.ToDecimal(dataSet.InsuranceCost == null ? 0 : dataSet.InsuranceCost);
                    ob.AccountNo = dataSet.AccountNo;
                    ob.OpeningStampCharge = dataSet.OpeningStampCharge == null ? 0 : (decimal)dataSet.OpeningStampCharge;
                    int flag = repo.Save();
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
                //throw new Exception e;
            }
            //return Json(new { msg = _vmMsg });
            return _vmMsg;
        }

        public object GetBankDebitVoucherInfo()
        {
            var lstBankDebitInfo = (from temp in repo.LcmBankDebitVoucherRpository.Get()
                                    select new
                                    {
                                        BDVID = temp.BDVID,
                                        BDVNo = temp.BDVNo,
                                        BDVDate = Convert.ToDateTime(temp.BDVDate).ToString("dd/MM/yyyy"),
                                        LCNo = temp.LCNo,
                                        LCID = temp.LCID,
                                        LCAmt = temp.LCAmt,
                                        VatOnSwift = temp.VatOnSwift,
                                        InsuranceCost = temp.InsuranceCost,
                                        AccountNo = temp.AccountNo,
                                        RecordStatus = temp.RecordStatus
                                    }).OrderBy(o => o.BDVDate).ToList();


            var result = (from temp in lstBankDebitInfo
                          select new
                          {
                              BDVID = temp.BDVID,
                              BDVNo = temp.BDVNo,
                              AccountNo = temp.AccountNo,
                              BDVDate = temp.BDVDate,
                              LCNo = temp.LCNo,                            
                              RecordStatus = DalCommon.ReturnRecordStatus(temp.RecordStatus)
                          }).OrderByDescending(o => o.BDVID);
            //return Json(result, JsonRequestBehavior.AllowGet);
            return result;
        }

        public object SearchBankDebitVoucherByBDVNo(string search)
        {
            search = search.ToUpper();
            var result = (from temp in repo.LcmBankDebitVoucherRpository.Get(orderBy: q => q.OrderByDescending(d => d.BDVID))
                          where ((temp.BDVNo.ToUpper().StartsWith(search) || temp.BDVNo.ToUpper() == search))
                          select new
                          {
                              BDVID = temp.BDVID,
                              BDVNo = temp.BDVNo,
                              BDVDate = Convert.ToDateTime(temp.BDVDate).ToString("dd/MM/yyyy"),
                              LCNo = temp.LCNo,
                              RecordStatus = temp.RecordStatus
                          }).ToList();
            return result;
        }

        public object GetBankDebitVoucherByBDVID(string BDVID)
        {
            int id = Convert.ToInt32(BDVID);
            LCM_BankDebitVoucher dataSet = repo.LcmBankDebitVoucherRpository.GetByID(id);

            LcmBankDebitVoucher ob = new LcmBankDebitVoucher();
            if (dataSet.BranchID > 0)
            {
                ob.BankID = repo.BranchRepository.GetByID(dataSet.BranchID).BankID;
                ob.BranchName = repo.BranchRepository.GetByID(dataSet.BranchID).BranchName;
                ob.BranchID = Convert.ToInt32(dataSet.BranchID);
            }
            if (ob.BankID > 0) { ob.BankName = repo.BankRepository.GetByID(ob.BankID).BankName; }


            ob.LCNo = dataSet.LCNo;
            ob.LCID = Convert.ToInt32(dataSet.LCID);

            ob.BDVID = dataSet.BDVID;
            ob.BDVNo = dataSet.BDVNo;
            ob.BDVDate = Convert.ToDateTime(dataSet.BDVDate).ToString("dd/MM/yyyy");
            ob.ExchangeCurrency = (byte)dataSet.ExchangeCurrency;
            ob.ExchangeRate = (decimal)dataSet.ExchangeRate;
            ob.ExchangeAmount = (decimal)dataSet.ExchangeAmount;
            ob.LCMargin = (decimal)dataSet.LCMargin;
            ob.Remarks = dataSet.Remarks;
            ob.CommissionAmt = (decimal)dataSet.CommissionAmt;
            ob.PostageAmt = (decimal)dataSet.PostageAmt;
            ob.SwiftCharge = (decimal)dataSet.SwiftCharge;
            ob.SourceTaxAmt = (decimal)dataSet.SourceTaxAmt;
            ob.VatAmt = (decimal)dataSet.VatAmt;
            ob.StationaryExpense = (decimal)dataSet.StationaryExpense;
            ob.OtherCost = (decimal)dataSet.OtherCost;
            ob.TotalAmount = (ob.LCMargin + ob.CommissionAmt + ob.PostageAmt + ob.SwiftCharge + ob.SourceTaxAmt + ob.VatAmt + ob.StationaryExpense + ob.OtherCost);//(decimal)dataSet.TotalAmount;
            ob.ApprovalAdvice = dataSet.ApprovalAdvice;
            ob.LCAmt = dataSet.LCAmt;
            ob.VatOnSwift = dataSet.VatOnSwift;
            ob.InsuranceCost = dataSet.InsuranceCost;
            ob.AccountNo = dataSet.AccountNo;
            ob.RecordStatus = dataSet.RecordStatus;
            ob.OpeningStampCharge = dataSet.OpeningStampCharge;
            return ob;
        }

        public ValidationMsg ConfirmRecordStatus(string BDVID)
        {
            try
            {
                if (BDVID != "")
                {
                    LCM_BankDebitVoucher ob = repo.LcmBankDebitVoucherRpository.GetByID(Convert.ToInt32(BDVID));                  
                    ob.RecordStatus = "CNF";
                    repo.LcmBankDebitVoucherRpository.Update(ob);
                    int flag = repo.Save();
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
            }
            catch (DbEntityValidationException e)
            {
                //throw new Exception e;
            }
            return _vmMsg;
        }

        public ValidationMsg UpdateRecordStatus(string note, string BDVID)
        {
            try
            {
                if (BDVID != "")
                {
                    LCM_BankDebitVoucher ob = repo.LcmBankDebitVoucherRpository.GetByID(Convert.ToInt32(BDVID));
                    ob.ApprovalAdvice = note;
                    ob.RecordStatus = "CHK";
                    ob.CheckedBy = Convert.ToInt32(HttpContext.Current.Session["UserID"]);
                    ob.CheckDate = DateTime.Now;
                    repo.LcmBankDebitVoucherRpository.Update(ob);
                    int flag = repo.Save();
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
                // throw new Exception e;
            }
            return _vmMsg ;
        }
    }
}
