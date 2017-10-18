using System;
using System.Text;
using System.Web;
using System.Data;
using System.Linq;
using DatabaseUtility;
using System.Transactions;
using System.Threading.Tasks;
using System.Globalization;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.BaseModel;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.EntitiesModel.OperationModel;
using ERP.EntitiesModel.AppSetupModel;

namespace ERP.DatabaseAccessLayer.AppSetupGateway
{
    public class DalInsurance
    {
        private readonly BLC_DEVEntities _context;
        private UnitOfWork repository = new UnitOfWork();
        private ValidationMsg vmMsg;
        public int BankID = 0;
        private int save;

        public DalInsurance()
        {
            _context = new BLC_DEVEntities();
            vmMsg = new ValidationMsg();
        }


        public long GetBankID()
        {
            return BankID;
        }


        public IEnumerable<SysBank> GetInsuranceList()
        {
            string sql = @"SELECT		        b.BankID,
							                    ISNULL(b.BankCode,'')BankCode,
							                    ISNULL(b.BankName,'')BankName,
							                    ISNULL(b.BankCategory,'')BankCategory,
							                    ISNULL(b.BankType,'')BankType,
							                    br.BranchID,
							                    ISNULL(br.BanchCode,'')BanchCode,
							                    ISNULL(br.BranchName,'')BranchName,
							                    ISNULL(br.Address1,'')Address1,
							                    ISNULL(br.Address2,'')Address2,
							                    ISNULL(br.Address3,'')Address3


                            FROM				Sys_Bank b   
                            INNER JOIN			Sys_Branch br ON b.BankID=br.BankID
                            WHERE				b.BankCategory='INC' AND b.IsActive='true'
                            ORDER BY            b.BankName ASC";
        var result = _context.Database.SqlQuery<SysBank>(sql);
            return result;
        }

        public ValidationMsg Save(SysBank model, int _userid, string pageUrl)//, string pageUrl
        {
            vmMsg = new ValidationMsg();

            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        model.BankCode = DalCommon.GetPreDefineNextCodeByUrl(pageUrl);//DalCommon.GetPreDefineValue("1", "00045");
                        if (model.BankCode != null)
                        {
                            Sys_Bank tblBank = SetToBankModelObject(model, _userid);//, _userid
                            _context.Sys_Bank.Add(tblBank);
                            _context.SaveChanges();

                            if (model.Branches.Count > 0)
                            {
                                foreach (var branch in model.Branches)
                                {

                                    branch.BankID = tblBank.BankID;
                                    Sys_Branch tblBranch = SetToBranchModelObject(branch, _userid);
                                    _context.Sys_Branch.Add(tblBranch);
                                    _context.SaveChanges();
                                }
                            }
                            _context.SaveChanges();

                            tx.Complete();
                            //BankID = tblBank.BankID;

                            vmMsg.Type = Enums.MessageType.Success;
                            vmMsg.Msg = "Saved Successfully.";
                        }
                    }
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
            return vmMsg;
        }




        public Sys_Bank SetToBankModelObject(SysBank model, int _userid)//int _userid
        {
            Sys_Bank entity = new Sys_Bank();
            entity.BankID = model.BankID;
            entity.BankCode = model.BankCode;
            entity.BankName = model.BankName;
            entity.BankCategory = model.BankCategory;
            entity.BankType = model.BankType;
            entity.IsActive = model.IsActive == "Active";
            entity.SetOn = DateTime.Now;
            entity.SetBy = _userid;
            entity.IPAddress = GetIPAddress.LocalIPAddress();

            return entity;
        }


        public Sys_Branch SetToBranchModelObject(SysBranch model, int _userid)
        {
            Sys_Branch entity = new Sys_Branch();

            entity.BranchID = model.BranchID;
            entity.BankID = model.BankID;
            entity.BanchCode = model.BanchCode;
            entity.BranchName = model.BranchName;
            entity.Address1 = model.Address1;
            entity.Address2 = model.Address2;
            entity.Address3 = model.Address3;
            entity.LCBalance = model.LCBalance;
            entity.IsActive = model.IsActive == "Active";
            return entity;
        }

      

        #region UPDATE Crust Leather DATA

        public ValidationMsg Update(SysBank model, int _userid)
        {
            vmMsg = new ValidationMsg();
            try
            {
             #region Update

                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        #region Crust Leather Transfer

                        Sys_Bank bank = SetToBankModelObject(model, _userid);
                        var OriginalEntity = _context.Sys_Bank.First(m => m.BankID == model.BankID);


                        OriginalEntity.BankCode = bank.BankCode;
                        OriginalEntity.BankName = bank.BankName;
                        OriginalEntity.BankCategory = bank.BankCategory;
                        OriginalEntity.BankType = bank.BankType;
                        OriginalEntity.IsActive = bank.IsActive;
                        OriginalEntity.ModifiedOn = DateTime.Now;
                        OriginalEntity.ModifiedBy = _userid;
                        _context.SaveChanges();
                        #endregion

                        #region Save NEW Data & Update Existing Crust Leather Transfer Data

                        if (model.Branches != null)
                        {
                            foreach (SysBranch branch in model.Branches)
                            {
                                branch.BankID = model.BankID;


                                if (branch.BranchID == 0)
                                {
                                    branch.BankID = bank.BankID;
                                    Sys_Branch tblBranch = SetToBranchModelObject(branch, _userid);
                                    _context.Sys_Branch.Add(tblBranch);
                                    



                                    //INV_CLTransferTo tblQCSelection = SetToCLTransferToModelObject(objCLTransferToItem, userid);
                                    //_context.INV_CLTransferTo.Add(tblQCSelection);
                                }
                                else
                                {

                                    Sys_Branch branchEntity = SetToBranchModelObject(branch, _userid);
                                    var OriginalIssueItemEntity = _context.Sys_Branch.First(m => m.BranchID == branch.BranchID);

                                    OriginalIssueItemEntity.BranchID = branchEntity.BranchID;
                                    OriginalIssueItemEntity.BankID = branchEntity.BankID;
                                    OriginalIssueItemEntity.BanchCode = branchEntity.BanchCode;
                                    OriginalIssueItemEntity.BranchName = branchEntity.BranchName;
                                    OriginalIssueItemEntity.Address1 = branchEntity.Address1;
                                    OriginalIssueItemEntity.Address2 = branchEntity.Address2;
                                    OriginalIssueItemEntity.Address3 = branchEntity.Address3;
                                    OriginalIssueItemEntity.LCBalance = branchEntity.LCBalance;
                                    // OriginalIssueItemEntity.IsActive = branchEntity.IsActive == "Active";
                                    OriginalIssueItemEntity.SetOn = branchEntity.SetOn;
                                    OriginalIssueItemEntity.SetBy = branchEntity.SetBy;


                                    OriginalIssueItemEntity.ModifiedBy = _userid;
                                    OriginalIssueItemEntity.ModifiedOn = DateTime.Now;
                                    OriginalIssueItemEntity.IPAddress = GetIPAddress.LocalIPAddress();
                                    OriginalIssueItemEntity.SetOn = DateTime.Now;
                                    OriginalIssueItemEntity.SetBy = _userid;

                                }

                                
                            }
                        }
                        #endregion
    
                        _context.SaveChanges();
                        tx.Complete();
                        BankID = model.BankID;
                        //CLTransferNo = model.CLTransferNo;
                        vmMsg.Type = Enums.MessageType.Update;
                        vmMsg.Msg = "Updated Successfully.";
                    }
                }

               #endregion
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
                vmMsg.Type = Enums.MessageType.Error;
                vmMsg.Msg = "Failed to Update.";
            }
            return vmMsg;
        }
        #endregion
        public ValidationMsg Delete(string bankID)
        {
            try
            {
                using (var tx = new TransactionScope())
                {
                    Sys_Bank delete = repository.BankRepository.GetByID(Convert.ToInt32(bankID));
                    if (delete != null)
                    {
                        var deleteBranch = repository.BranchRepository.Get().Where(ob => ob.BankID == delete.BankID).ToList();
                        foreach (var item in deleteBranch)
                        {
                            Sys_Branch deleteBranchItem = repository.BranchRepository.GetByID(Convert.ToInt32(item.BranchID));


                            repository.BranchRepository.Delete(deleteBranchItem);
                            save = repository.Save();


                        }
                        repository.BankRepository.Delete(delete);
                        save = repository.Save();
                    }

                    tx.Complete();

                    vmMsg.Type = Enums.MessageType.Delete;
                    vmMsg.Msg = "Deleted Sucessfully";

                }
            }
            catch 
            {
                vmMsg.Type = Enums.MessageType.Error;
                vmMsg.Msg = "Failed to delete.";
            }
            return vmMsg;
        }


    }
}