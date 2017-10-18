using System;
using System.Collections.Generic;
using System.Linq;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;

namespace ERP.DatabaseAccessLayer.AppSetupGateway
{
    public class DalSysBranch
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg vmMsg;
        public int BranchID = 0;

        public DalSysBranch()
        {
            _context = new BLC_DEVEntities();
        }

        public ValidationMsg Save(SysBranch model, int userid)
        {
            var vmMsg = new ValidationMsg();
            try
            {
                Sys_Branch tblSysBranch = SetToModelObject(model, userid);
                _context.Sys_Branch.Add(tblSysBranch);
                _context.SaveChanges();
                BranchID = tblSysBranch.BranchID;
                vmMsg.Type = Enums.MessageType.Success;
                vmMsg.Msg = "Saved Successfully.";
            }
            catch (Exception ex)
            {
                if (ex.InnerException.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    vmMsg.Type = Enums.MessageType.Error;
                    vmMsg.Msg = "Banch Code Already Exit..";
                }
                else
                {
                    vmMsg.Type = Enums.MessageType.Error;
                    vmMsg.Msg = "Failed to Save.";
                }
            }
            return vmMsg;
        }

        public long GetBranchID()
        {
            return BranchID;
        }

        public ValidationMsg Update(SysBranch model, int userid)
        {
            var vmMsg = new ValidationMsg();
            try
            {
                var sysBranch = _context.Sys_Branch.FirstOrDefault(s => s.BranchID == model.BranchID);
                if (sysBranch != null)
                {
                    sysBranch.BanchCode = model.BanchCode;
                    sysBranch.BranchName = model.BranchName;
                    sysBranch.BankID = model.BankID;
                    sysBranch.LCLimit = model.LCLimit;
                    sysBranch.LCOpened = model.LCOpened;
                    sysBranch.LCBalance = model.LCBalance;
                    sysBranch.LCMargin = model.LCMargin;
                    sysBranch.LCMarginUsed = model.LCMarginUsed;
                    sysBranch.LCMarginBalance = model.LCMarginBalance;
                    sysBranch.BranchSwiftCode = model.BranchSwiftCode;
                    sysBranch.LIMLimit = model.LIMLimit;
                    sysBranch.LIMTaken = model.LIMTaken;
                    sysBranch.LIMBalance = model.LIMBalance;
                    sysBranch.Address1 = model.Address1;
                    sysBranch.Address2 = model.Address2;
                    sysBranch.Address3 = model.Address3;
                    sysBranch.IsActive = model.IsActive == "Active";
                    sysBranch.ModifiedOn = DateTime.Now;
                    sysBranch.ModifiedBy = userid;
                }
                _context.SaveChanges();

                vmMsg.Type = Enums.MessageType.Update;
                vmMsg.Msg = "Updated Successfully.";
            }
            catch (Exception ex)
            {
                if (ex.InnerException.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    vmMsg.Type = Enums.MessageType.Error;
                    vmMsg.Msg = "Banch Code Already Exit.";
                }
                else
                {
                    vmMsg.Type = Enums.MessageType.Error;
                    vmMsg.Msg = "Failed to Update.";
                }
            }
            return vmMsg;
        }

        public IEnumerable<SysBranch> GetAll()
        {
            IEnumerable<SysBranch> iLstSysBranch = from ss in _context.Sys_Branch
                                                   select new SysBranch
                                                          {
                                                              BanchCode = ss.BanchCode,
                                                              BranchID = ss.BranchID,
                                                              BranchName = ss.BranchName,
                                                              LCLimit = ss.LCLimit,
                                                              LCOpened = ss.LCOpened,
                                                              LCBalance = ss.LCBalance,
                                                              BankID = ss.BankID,
                                                              BankName = _context.Sys_Bank.Where(m => m.BankID == ss.BankID).FirstOrDefault() != null ? _context.Sys_Bank.Where(m => m.BankID == ss.BankID).FirstOrDefault().BankName : null,
                                                              LCMargin = ss.LCMargin,
                                                              LCMarginUsed = ss.LCMarginUsed,
                                                              LCMarginBalance = ss.LCMarginBalance,
                                                              BranchSwiftCode = ss.BranchSwiftCode,
                                                              LIMLimit = ss.LIMLimit,
                                                              LIMTaken = ss.LIMTaken,
                                                              LIMBalance = ss.LIMBalance,
                                                              Address1 = ss.Address1,
                                                              Address2 = ss.Address2,
                                                              Address3 = ss.Address3,
                                                              IsActive = ss.IsActive == true ? "Active" : "Inactive",
                                                          };

            return iLstSysBranch;
        }

        //public IEnumerable<SysBranch> GetAllActiveBranch()
        //{
        //    IEnumerable<SysBranch> iLstSysBranch = from ss in _context.Sys_Branch
        //                                           where ss.IsActive
        //                                           select new SysBranch
        //                                           {
        //                                               BranchID = ss.BranchID,
        //                                               BranchName = ss.BranchName
        //                                           };

        //    return iLstSysBranch;
        //}

        //public ValidationMsg Delete(string BranchID, int userid)
        public ValidationMsg Delete(string BranchID)
        {
            var itemid = string.IsNullOrEmpty(BranchID) ? 0 : Convert.ToInt32(BranchID);
            var vmMsg = new ValidationMsg();
            try
            {
                var sysBranch = _context.Sys_Branch.FirstOrDefault(s => s.BranchID == itemid);
                _context.Sys_Branch.Remove(sysBranch);
                _context.SaveChanges();

                vmMsg.Type = Enums.MessageType.Success;
                vmMsg.Msg = "Deleted Successfully.";
            }
            catch (Exception ex)
            {
                vmMsg.Type = Enums.MessageType.Error;
                vmMsg.Msg = "Failed to Delete.";
            }
            return vmMsg;
        }
        public Sys_Branch SetToModelObject(SysBranch model, int userid)
        {
            Sys_Branch entity = new Sys_Branch();

            entity.BranchID = model.BranchID;
            entity.BanchCode = model.BanchCode;
            entity.BranchName = model.BranchName;
            entity.BankID = model.BankID;
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
            entity.Address1 = model.Address1;
            entity.Address2 = model.Address2;
            entity.Address3 = model.Address3;
            entity.IsActive = model.IsActive == "Active";
            entity.SetOn = DateTime.Now;
            entity.SetBy = userid;
            entity.IPAddress = string.Empty;

            return entity;
        }


        public List<SysBranch> GetCategoryTypeWiseBranchNameWithBank(string _Category, string _Type)
        {
            using (var context= new BLC_DEVEntities())
            {
                var Data = (from b in context.Sys_Bank.AsEnumerable()
                            where b.BankCategory == _Category && b.BankType == _Type

                            join br in context.Sys_Branch on b.BankID equals br.BankID

                            select new SysBranch
                            {
                                BranchID = Convert.ToInt16((br == null ? null : (br.BranchID).ToString())),
                                BranchName = ( b.BankName +  "," + (br == null ? null : br.BranchName) )
                            }).ToList();

                return Data;
            }

        }

        public List<SysBank> GetBeneficiaryAdvisingBankList()
        {
            using (var context = new BLC_DEVEntities())
            {
                var Data = (from b in context.Sys_Bank.AsEnumerable()
                            where b.BankCategory == "BNK" && (b.BankType == "BNF" || b.BankType == "ADV")

                            join br in context.Sys_Branch on b.BankID equals br.BankID

                            orderby br.BranchName
                            select new SysBank
                            {
                                BranchID = Convert.ToInt16((br == null ? null : (br.BranchID).ToString())),
                                BranchName = (br == null ? null : br.BranchName + "," + b.BankName),
                                BankName = b.BankName,
                                BankType = b.BankType == "BNF" ? "Beneficiary" : "Advising"
                            }).ToList();

                return Data;
            }

        }


    }
}
