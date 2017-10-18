using System;
using System.Globalization;
using System.Web;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DatabaseUtility;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.OperationModel;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using System.Transactions;
using System.Linq;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalBankVoucher
    {
        private readonly BLC_DEVEntities _context;

        public DalBankVoucher()
        {
            _context = new BLC_DEVEntities();
        }
        public List<ExpBankVoucher> GetTransactionInfoForSearch()
        {

            using (_context)
            {
                var Data = (from t in _context.EXP_BankVoucher.AsEnumerable()

                            join b in _context.Sys_Bank on t.BankID equals b.BankID into Banks
                            from b in Banks.DefaultIfEmpty()

                            join br in _context.Sys_Branch on t.BranchID equals br.BranchID into Branches
                            from br in Branches.DefaultIfEmpty()

                            join ci in _context.EXP_CI on (t.CIID==null?null:t.CIID) equals ci.CIID into CIs
                            from ci in CIs.DefaultIfEmpty()

                            orderby t.BVID descending
                            select new ExpBankVoucher
                            {
                                BVID = t.BVID,
                                RefBVNo = t.RefBVNo,
                                BVDate = (Convert.ToDateTime(t.BVDate)).ToString("dd'/'MM'/'yyyy"),
                                BVType = (t.BVType=="DR"? "Debit": "Credit"),
                                BankID = t.BankID,
                                BankName= b==null?null:b.BankName,
                                BranchID = t.BranchID,
                                BranchName = br == null ? null : br.BranchName,
                                ACNo = t.ACNo,
                                ACName = t.ACName,
                                Remarks= t.Remarks,
                                RecordStatus = DalCommon.ReturnRecordStatus(t.RecordStatus)
                            }).ToList();

                return Data;
            }
        }

        public List<SysBank> GetCategoryTypeWiseBankList(string _Category, string _Type)
        {
            using(_context)
            {
                var Data = (from b in _context.Sys_Bank
                            where b.BankCategory == _Category & b.BankType == _Type
                            select new SysBank
                            {
                                BankID = b.BankID,
                                BankCode = b.BankCode,
                                BankName = b.BankName
                            }).ToList();

                return Data;
            }
        }

        public List<sysBranch> GetBranchListForSpecificBank(long _BankID)
        {
            using(_context)
            {
                var Data = (from b in _context.Sys_Branch
                            where b.BankID == _BankID
                            select new sysBranch
                            {
                                BranchID = b.BranchID,
                                BranchName = b.BranchName
                            }).ToList();

                return Data;
            }
        }

        public List<EXPCI> GetCIList()
        {
            using (_context)
            {
                var Data = (from b in _context.EXP_CI.AsEnumerable()

                            orderby b.CIID descending

                            select new EXPCI
                            {
                                CIID = b.CIID,
                                CIRefNo = b.CIRefNo,
                                CIDate = (Convert.ToDateTime(b.CIDate)).ToString("dd'/'MM'/'yyyy")
                            }).ToList();

                return Data;
            }
        }



        public long Save(ExpBankVoucher model, int userId, string pageUrl)
        {
            long CurrentBVID = 0;
            

            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    using (_context)
                    {
                        var AutoBVNo = DalCommon.GetPreDefineNextCodeByUrl(pageUrl);

                        //Random r = new Random();
                        //var AutoBVNo = r.Next();

                        if (AutoBVNo != null)
                        {
                            #region New_Voucher_Insert

                        EXP_BankVoucher objVoucher = new EXP_BankVoucher();

                            
                            objVoucher.BVNo= AutoBVNo.ToString();
                            objVoucher.RefBVNo = model.RefBVNo;
                            objVoucher.BVDate = DalCommon.SetDate(model.BVDate);
                            objVoucher.BVType = model.BVType;

                            objVoucher.BankID = model.BankID;
                            objVoucher.BranchID = model.BranchID;
                            objVoucher.ACName = model.ACName;
                            objVoucher.ACNo = model.ACNo;
                            objVoucher.Remarks = model.Remarks;

                            objVoucher.RecordStatus = "NCF";
                            objVoucher.SetBy = userId;
                            objVoucher.SetOn = DateTime.Now;

                            _context.EXP_BankVoucher.Add(objVoucher);
                            _context.SaveChanges();
                            CurrentBVID = objVoucher.BVID;
                            #endregion

                            }

                            #region Item Insert
                            if (model.EXPBankVoucherDtl != null)
                            {
                                foreach (var item in model.EXPBankVoucherDtl)
                                {
                                    EXP_BankVoucherDtl objItem = new EXP_BankVoucherDtl();

                                    objItem.BVID = CurrentBVID;
                                    objItem.TransSL = item.TransSL;
                                   
                                    objItem.Narration = item.Narration;
                                    objItem.TransHead = item.TransHead;
                                    objItem.BVCRAmt = item.BVCRAmt;
                                    objItem.BVDRAmt = item.BVDRAmt;
                                        
                                    if(item.CurrencyName!=null)
                                        objItem.Currency = DalCommon.GetCurrencyID(item.CurrencyName);

                                    if (item.ExchangeCurrencyName != null)
                                        objItem.ExchangeCurrency = DalCommon.GetCurrencyID(item.ExchangeCurrencyName);

                                    objItem.ExchangeRate = item.ExchangeRate;
                                    objItem.ExchangeAmount = item.ExchangeAmount;
                                    objItem.CIID = item.CIID;


                                  
                                    objItem.SetOn = DateTime.Now;
                                    objItem.SetBy = userId;

                                    _context.EXP_BankVoucherDtl.Add(objItem);
                                    //_context.SaveChanges();
                                }

                            }
                            #endregion

                            _context.SaveChanges();
                        
                    }
                    transaction.Complete();
                }
                catch (Exception e)
                {
                    CurrentBVID = 0;
                    return CurrentBVID;
                }

            }
            return CurrentBVID;
        }

        public int Update(ExpBankVoucher model, int userId)
        {
            try
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    using (_context)
                    {

                        #region Transaction_Informaiton_Update
                        var objVoucher = (from p in _context.EXP_BankVoucher
                                          where p.BVID == model.BVID
                                          select p).FirstOrDefault();


                        objVoucher.RefBVNo = model.RefBVNo;
                        objVoucher.BVDate = DalCommon.SetDate(model.BVDate);
                        objVoucher.BVType = model.BVType;

                        objVoucher.BankID = model.BankID;
                        objVoucher.BranchID = model.BranchID;
                        objVoucher.ACName = model.ACName;
                        objVoucher.ACNo = model.ACNo;
                        objVoucher.CIID = model.CIID;

                        #endregion

                        #region Update voucher Items
                        if (model.EXPBankVoucherDtl != null)
                        {
                            foreach (var item in model.EXPBankVoucherDtl)
                            {

                                var checkVoucherItem = (from i in _context.EXP_BankVoucherDtl
                                                        where i.BVDTLID == item.BVDTLID
                                                        select i).Any();

                                #region New_Item_Insertion
                                if (!checkVoucherItem)
                                {
                                    EXP_BankVoucherDtl objItem = new EXP_BankVoucherDtl();

                                    objItem.BVID = model.BVID;
                                    objItem.TransSL = item.TransSL;

                                    objItem.Narration = item.Narration;

                                    objItem.TransHead = item.TransHead;

                                    objItem.BVCRAmt = item.BVCRAmt;
                                    objItem.BVDRAmt = item.BVDRAmt;


                                    if (item.CurrencyName != null)
                                        objItem.Currency = DalCommon.GetCurrencyID(item.CurrencyName);

                                    if (item.ExchangeCurrencyName != null)
                                        objItem.ExchangeCurrency = DalCommon.GetCurrencyID(item.ExchangeCurrencyName);
                                    objItem.ExchangeRate = item.ExchangeRate;
                                    objItem.ExchangeAmount = item.ExchangeAmount;
                                    objItem.CIID = item.CIID;

                                    objItem.SetOn = DateTime.Now;
                                    objItem.SetBy = userId;

                                    _context.EXP_BankVoucherDtl.Add(objItem);

                                }
                                #endregion

                                #region Existing_Item_Update
                                else
                                {
                                    var objItem = (from i in _context.EXP_BankVoucherDtl
                                                     where i.BVDTLID == item.BVDTLID
                                                     select i).FirstOrDefault();

                                    objItem.BVID = model.BVID;
                                    objItem.TransSL = item.TransSL;

                                    objItem.Narration = item.Narration;

                                    objItem.TransHead = item.TransHead;
                                    objItem.BVCRAmt = item.BVCRAmt;
                                    objItem.BVDRAmt = item.BVDRAmt;
                                    
                                    if (item.CurrencyName != null)
                                        objItem.Currency = DalCommon.GetCurrencyID(item.CurrencyName);

                                    if (item.ExchangeCurrencyName != null)
                                        objItem.ExchangeCurrency = DalCommon.GetCurrencyID(item.ExchangeCurrencyName);
                                    objItem.ExchangeRate = item.ExchangeRate;
                                    objItem.ExchangeAmount = item.ExchangeAmount;
                                    objItem.CIID = item.CIID;

                                    objItem.ModifiedOn = DateTime.Now;
                                    objItem.ModifiedBy = userId;
                                }
                                #endregion
                            }
                        }

                        #endregion

                        _context.SaveChanges();
                    }

                    transaction.Complete();


                }
                return 1;
            }
            catch (Exception e)
            {
                return 0;
            }

        }

        public List<EXPBankVoucherDtl> GetVouchertemList(long BVID)
        {
            using(var context= new BLC_DEVEntities())
            {
                var Data = (from vd in context.EXP_BankVoucherDtl
                            where vd.BVID == BVID

                            join c in context.Sys_Currency on (vd.Currency == null ? null : vd.Currency) equals c.CurrencyID into Currencies
                            from c in Currencies.DefaultIfEmpty()

                            join ec in context.Sys_Currency on (vd.ExchangeCurrency == null ? null : vd.ExchangeCurrency) equals ec.CurrencyID into ExchangeCurrencies
                            from ec in ExchangeCurrencies.DefaultIfEmpty()

                            join th in context.Sys_TransHead on (vd.TransHead == null ? null : vd.TransHead) equals th.HeadID into TransHeads
                            from th in TransHeads.DefaultIfEmpty()

                            join ci in context.EXP_CI on (vd.CIID == null ? null : vd.CIID) equals ci.CIID into CIs
                            from ci in CIs.DefaultIfEmpty()


                            select new EXPBankVoucherDtl
                            {
                                BVDTLID = vd.BVDTLID,
                                TransSL = vd.TransSL,
                                Narration = vd.Narration,
                                HeadID = vd.TransHead,
                                HeadName = (th == null ? null : th.HeadName),
                                BVDRAmt= vd.BVDRAmt,
                                BVCRAmt= vd.BVCRAmt,
                                CurrencyName = (c == null ? null : c.CurrencyName),
                                ExchangeCurrencyName = (ec == null ? null : ec.CurrencyName),
                                ExchangeRate = vd.ExchangeRate,
                                ExchangeAmount = vd.ExchangeAmount,
                                CIID = vd.CIID,
                                CIRefNo = ci == null ? "" : ci.CIRefNo
                            }).ToList();

                return Data;
            }
        }

        public bool DeleteVoucherItem(string _BVDTLID)
        {
            try
            {
                var VoucherItem = (from c in _context.EXP_BankVoucherDtl.AsEnumerable()
                                       where (c.BVDTLID).ToString() == _BVDTLID
                                       select c).FirstOrDefault();

                _context.EXP_BankVoucherDtl.Remove(VoucherItem);
                _context.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool DeleteVoucher(string _BVID)
        {
            try
            {
                var Voucher = (from r in _context.EXP_BankVoucher.AsEnumerable()
                               where r.BVID.ToString() == _BVID
                               select r).FirstOrDefault();

                _context.EXP_BankVoucher.Remove(Voucher);
                _context.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }


        public bool ConfirmVoucher(long _BVID, string _CheckComment)
        {
            try
            {
                using (TransactionScope Transaction = new TransactionScope())
                {
                    using (_context)
                    {


                        var VoucherInfo = (from p in _context.EXP_BankVoucher
                                           where p.BVID == _BVID
                                           select p).FirstOrDefault();

                        VoucherInfo.CheckNote = _CheckComment;

                        VoucherInfo.RecordStatus = "CNF";

                        _context.SaveChanges();

                    }
                    Transaction.Complete();
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }




    }
}
