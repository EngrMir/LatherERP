using System;
using System.Collections.Generic;
using System.Linq;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using System.Transactions;

namespace ERP.DatabaseAccessLayer.AppSetupGateway
{
    public class DalSysBank
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg vmMsg;
        public int BankID = 0;

        public DalSysBank()
        {
            _context = new BLC_DEVEntities();
            vmMsg = new ValidationMsg();
        }

        public ValidationMsg Save(SysBank model, int userid, string url)
        {
            vmMsg = new ValidationMsg();
            try
            {
                #region Save

                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {

                        #region Bank

                        Sys_Bank tblSysBank = SetToModelObject(model, userid, url);
                        _context.Sys_Bank.Add(tblSysBank);
                        _context.SaveChanges();

                        #endregion

                        #region Branches

                        if (model.Branches != null)
                        {
                            foreach (var branch in model.Branches)
                            {
                                branch.BankID = tblSysBank.BankID;
                                _context.Sys_Branch.Add(BranchModelConversion(branch, tblSysBank.BankID));
                                _context.SaveChanges();
                            }
                        }

                        #endregion

                        _context.SaveChanges();
                        tx.Complete();
                        BankID = tblSysBank.BankID;
                        vmMsg.ReturnId = tblSysBank.BankID;
                        vmMsg.ReturnCode = tblSysBank.BankCode;
                        vmMsg.Type = Enums.MessageType.Success;
                        vmMsg.Msg = "Saved Successfully.";
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                vmMsg.Type = Enums.MessageType.Error;
                vmMsg.Msg = "Failed to save.";
            }
            return vmMsg;
        }

        public long GetBankID()
        {
            return BankID;
        }

        public ValidationMsg Update(SysBank model, int userid)
        {
            var vmMsg = new ValidationMsg();
            try
            {
                //const string chars = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789";
                //var random = new Random();
                //var result = new string(
                //    Enumerable.Repeat(chars, 8)
                //              .Select(s => s[random.Next(s.Length)])
                //              .ToArray());
                var sysBank = _context.Sys_Bank.FirstOrDefault(s => s.BankID == model.BankID);
                if (sysBank != null)
                {
                    sysBank.BankCode = model.BankCode;
                    sysBank.BankName = model.BankName;
                    sysBank.BankSwiftCode = model.BankSwiftCode;
                    sysBank.BankBINNo = model.BankBINNo;
                    sysBank.BankType = model.BankType;
                    sysBank.IsActive = model.IsActive == "Active";
                    sysBank.ModifiedOn = DateTime.Now;
                    sysBank.ModifiedBy = userid;
                }

                #region Branches

                if (model.Branches != null)
                {
                    foreach (var branch in model.Branches)
                    {
                        if (branch.BranchID == 0)
                        {
                            branch.BankID = model.BankID;
                            Sys_Branch tblEXPCIPIItemColor = BranchModelConversion(branch, userid);
                            _context.Sys_Branch.Add(tblEXPCIPIItemColor);
                        }
                        else
                        {
                            Sys_Branch CurEntity = BranchModelConversion(branch, userid);
                            var OrgEntity = _context.Sys_Branch.First(m => m.BranchID == branch.BranchID);

                            //OrgEntity.BranchID = CurEntity.BranchID;
                            //OrgEntity.BankID = CurEntity.BankID;
                            OrgEntity.BanchCode = CurEntity.BanchCode;
                            OrgEntity.BranchName = CurEntity.BranchName;
                            OrgEntity.Address1 = CurEntity.Address1;
                            OrgEntity.Address2 = CurEntity.Address2;
                            OrgEntity.Address3 = CurEntity.Address3;
                            OrgEntity.LCLimit = CurEntity.LCLimit;
                            OrgEntity.LCOpened = CurEntity.LCOpened;
                            OrgEntity.LCBalance = CurEntity.LCBalance;
                            OrgEntity.LCMargin = CurEntity.LCMargin;
                            OrgEntity.LCMarginUsed = CurEntity.LCMarginUsed;
                            OrgEntity.LCMarginBalance = CurEntity.LCMarginBalance;
                            OrgEntity.BranchSwiftCode = CurEntity.BranchSwiftCode;
                            OrgEntity.LIMLimit = CurEntity.LIMLimit;
                            OrgEntity.LIMTaken = CurEntity.LIMTaken;
                            OrgEntity.LIMBalance = CurEntity.LIMBalance;
                            OrgEntity.IsActive = CurEntity.IsActive;// == "Active";
                            OrgEntity.ModifiedBy = userid;
                            OrgEntity.ModifiedOn = DateTime.Now;
                        }
                    }
                }

                #endregion

                //if (model.Branches != null)
                //{
                //    foreach (var branch in model.Branches)
                //    {
                //        var entity = _context.Sys_Branch.FirstOrDefault(s => s.BranchID == branch.BranchID);
                //        if (entity != null)
                //        {
                //            entity.BranchID = branch.BranchID;
                //            entity.BankID = branch.BankID;
                //            entity.BanchCode = branch.BanchCode;
                //            entity.BranchName = branch.BranchName;
                //            entity.Address1 = branch.Address1;
                //            entity.Address2 = branch.Address2;
                //            entity.Address3 = branch.Address3;
                //            entity.LCLimit = branch.LCLimit;
                //            entity.LCOpened = branch.LCOpened;
                //            entity.LCBalance = branch.LCBalance;
                //            entity.LCMargin = branch.LCMargin;
                //            entity.LCMarginUsed = branch.LCMarginUsed;
                //            entity.LCMarginBalance = branch.LCMarginBalance;
                //            entity.BranchSwiftCode = branch.BranchSwiftCode;
                //            entity.LIMLimit = branch.LIMLimit;
                //            entity.LIMTaken = branch.LIMTaken;
                //            entity.LIMBalance = branch.LIMBalance;
                //            entity.IsActive = branch.IsActive == "Active";
                //        }
                //        _context.SaveChanges();
                //    }
                //}
                _context.SaveChanges();

                vmMsg.ReturnId = sysBank.BankID;
                vmMsg.Type = Enums.MessageType.Update;
                vmMsg.Msg = "Updated Successfully.";
            }
            catch (Exception ex)
            {
                if (ex.InnerException.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    vmMsg.Type = Enums.MessageType.Error;
                    vmMsg.Msg = "Bank Code Already Exit.";
                }
                else
                {
                    vmMsg.Type = Enums.MessageType.Error;
                    vmMsg.Msg = "Failed to Update.";
                }
            }
            return vmMsg;
        }

        public IEnumerable<SysBank> GetAll()
        {
            IEnumerable<SysBank> iLstSysBank = from ss in _context.Sys_Bank
                                               select new SysBank
                                                      {
                                                          BankCode = ss.BankCode,
                                                          BankID = ss.BankID,
                                                          BankName = ss.BankName,
                                                          BankCategory = ss.BankCategory,
                                                          BankCategoryName = ss.BankCategory == "BNK" ? "Bank" : "Insurance Company",
                                                          BankType = ss.BankType,
                                                          BankTypeName = ss.BankType == "LOC" ? "Local" : "Foreign",
                                                          IsActive = ss.IsActive == true ? "Active" : "Inactive",
                                                      };

            return iLstSysBank;
        }

        public IEnumerable<SysBank> GetAllActiveBank()
        {
            IEnumerable<SysBank> iLstSysBank = from ss in _context.Sys_Bank
                                               where ss.IsActive == true
                                               select new SysBank
                                               {
                                                   BankID = ss.BankID,
                                                   BankName = ss.BankName
                                               };

            return iLstSysBank;
        }

        public ValidationMsg Delete(string BankID)
        {

            var itemid = string.IsNullOrEmpty(BankID) ? 0 : Convert.ToInt32(BankID);
            var vmMsg = new ValidationMsg();
            try
            {
                var itemList = _context.Sys_Branch.Where(m => m.BankID == itemid).ToList();

                if (itemList.Count > 0)
                {
                    vmMsg.Type = Enums.MessageType.Error;
                    vmMsg.Msg = "Child Record Found.";
                }
                else
                {
                    var sysBank = _context.Sys_Bank.FirstOrDefault(s => s.BankID == itemid);
                    _context.Sys_Bank.Remove(sysBank);
                    _context.SaveChanges();

                    vmMsg.Type = Enums.MessageType.Success;
                    vmMsg.Msg = "Deleted Successfully.";
                }
            }
            catch (Exception ex)
            {
                vmMsg.Type = Enums.MessageType.Error;
                vmMsg.Msg = "Failed to Delete.";
            }
            return vmMsg;
        }

        public Sys_Bank SetToModelObject(SysBank model, int userid, string url)
        {
            //const string chars = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789";
            //var random = new Random();
            //var result = new string(
            //    Enumerable.Repeat(chars, 8)
            //              .Select(s => s[random.Next(s.Length)])
            //              .ToArray());
            Sys_Bank entity = new Sys_Bank();
            entity.BankID = model.BankID;
            entity.BankCode = DalCommon.GetPreDefineNextCodeByUrl(url);
            entity.BankName = model.BankName;
            entity.BankCategory = model.BankCategory;
            entity.BankType = model.BankType;
            entity.BankBINNo = model.BankBINNo;
            entity.BankSwiftCode = model.BankSwiftCode;
            entity.IsActive = model.IsActive == "Active";
            entity.SetOn = model.BankID == 0 ? DateTime.Now : _context.Sys_Bank.FirstOrDefault(ob => ob.BankID == model.BankID).SetOn;
            entity.SetBy = model.BankID == 0 ? userid : _context.Sys_Bank.FirstOrDefault(ob => ob.BankID == model.BankID).SetBy;
            entity.ModifiedOn = model.BankID == 0 ? (DateTime?)null : DateTime.Now;
            entity.ModifiedBy = model.BankID == 0 ? (int?)null : userid;
            entity.IPAddress = string.Empty;
            return entity;
        }

        public Sys_Branch BranchModelConversion(SysBranch model, int bankId)
        {
            var entity = new Sys_Branch();
            entity.BranchID = model.BranchID;
            entity.BankID = bankId;
            entity.BankID = model.BankID;// bankId;
            entity.BanchCode = model.BanchCode;
            entity.BranchName = model.BranchName;
            entity.Address1 = model.Address1;
            entity.Address2 = model.Address2;
            entity.Address3 = model.Address3;
            entity.LCLimit = model.LCLimit;
            entity.LCOpened = model.LCOpened;
            entity.LCBalance = model.LCBalance;
            entity.LCMargin = model.LCMargin;
            entity.LCMarginUsed = model.LCMarginUsed;
            entity.LCMarginBalance = model.LCMarginBalance;
            entity.BranchSwiftCode = model.BranchSwiftCode;
            entity.LIMLimit = model.LIMLimit;
            entity.LIMTaken = model.LIMTaken;
            entity.LIMBalance = model.LIMBalance;
            entity.IsActive = model.IsActive == "Active";
            return entity;
        }

        public ValidationMsg DelBranch(int branchId)
        {
            var branch = _context.Sys_Branch.FirstOrDefault(ob => ob.BranchID == branchId);
            if (branch != null)
            {
                _context.Sys_Branch.Remove(branch);
                _context.SaveChanges();
                vmMsg.Type = Enums.MessageType.Delete;
                vmMsg.Msg = "Branch successfully deleted.";
            }
            else
            {
                vmMsg.Type = Enums.MessageType.Error;
                vmMsg.Msg = "Failed to delete.";
            }

            return vmMsg;
        }
    }
}
