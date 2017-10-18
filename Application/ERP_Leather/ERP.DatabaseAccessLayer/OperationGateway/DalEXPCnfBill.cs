using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ERP.DatabaseAccessLayer.DB;
using System.Collections.Generic;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using System.Data.Entity.Validation;


namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalEXPCnfBill
    {
        private readonly BLC_DEVEntities obEntity;
        private UnitOfWork repository;
        private ValidationMsg _vmMsg;
        public long CnfBillID = 0;
        private int save;
        public string CnfBillNo = string.Empty;
        public DalEXPCnfBill()
        {
            obEntity = new BLC_DEVEntities();
            _vmMsg = new ValidationMsg();
            repository = new UnitOfWork();
        }


        public List<EXPCnfBill> GetCnFBuyerList()
        {
            var queryString = @"SELECT		
		                                ISNULL(tbl1.BuyerID,0)BuyerID,
										ISNULL(tbl1.BuyerCode,'')BuyerCode,
										ISNULL(tbl1.BuyerName,'')BuyerName,
										ISNULL(tbl2.BuyerAddressID,'')BuyerAddressID,
		                                ISNULL(tbl2.Address,'')Address,
										ISNULL(tbl2.ContactPerson,'')ContactPerson,
										ISNULL(tbl2.ContactNumber,'')ContactNumber,
										ISNULL(tbl2.EmailAddress,'')EmailAddress,
										ISNULL(tbl2.FaxNo,'')FaxNo,
										ISNULL(tbl2.SkypeID,0)SkypeID,
		                                ISNULL(tbl2.PhoneNo,'')PhoneNo

		                                
                                FROM	 (SELECT 
				                                ISNULL(B.BuyerID,0)BuyerID,
												ISNULL(B.BuyerCode,'')BuyerCode,
				                                ISNULL(B.BuyerName,'')BuyerName
				                                FROM	Sys_Buyer B 
												WHERE	B.IsActive='true' AND B.BuyerCategory='CNF Agent' AND B.BuyerType='Foreign Agent' ) tbl1 
											LEFT JOIN  (SELECT	ISNULL(BAA.BuyerAddressID,0)BuyerAddressID, 
								                                ISNULL(BAA.BuyerID,0)BuyerID,
																ISNULL(BAA.Address,'')Address,
																ISNULL(BAA.ContactPerson,'')ContactPerson,
																ISNULL(BAA.ContactNumber,'')ContactNumber,
																ISNULL(BAA.EmailAddress,'')EmailAddress,
																ISNULL(BAA.FaxNo,'')FaxNo,
																ISNULL(BAA.SkypeID,0)SkypeID,
																ISNULL(BAA.PhoneNo,'')PhoneNo          
						                                FROM	Sys_BuyerAddress BAA 
						                                WHERE	 BAA.IsActive='true')tbl2 ON tbl1.BuyerID=tbl2.BuyerID
                                            
														ORDER BY tbl1.BuyerID DESC";
            var iChallanList = obEntity.Database.SqlQuery<EXPCnfBill>(queryString);
            return iChallanList.ToList();
        }


        public List<EXPCnfBill> GetLCList()
        {
            var queryString = @"SELECT  
                                        ISNULL(tbl2.CIID,0)CIID,
                                        ISNULL(tbl2.CINo,'')CINo,
                                        ISNULL(tbl2.CIRefNo,'')CIRefNo,
                                        ISNULL(tbl3.PLID,0)PLID,
                                        ISNULL(tbl3.PLNo,'') PLNo,
                                        ISNULL(tbl3.BalesNo,'') BalesNo,
                                        ISNULL(tbl3.TotalBales,0) TotalBales
                                                            FROM  (SELECT 
                                         ISNULL(c.CIID,0)CIID,
                                         ISNULL(c.CINo,'')CINo,
                                         ISNULL(c.CIRefNo,'')CIRefNo
                                       FROM EXP_CI c 
                                       WHERE c.RecordStatus='CNF' ) tbl2  
                                                                        LEFT JOIN  
                                         (SELECT ISNULL(PLID,0)PLID, 
                                                        ISNULL(PLNo,'')PLNo, 
                                                        ISNULL(CIID,0)CIID,
                                         ISNULL(BalesNo,'')BalesNo, 
                                         ISNULL(BaleQty,0)TotalBales 
                                                      FROM EXP_PackingList 
                                           WHERE  RecordStatus='CNF')tbl3 ON tbl2.CIID=tbl3.CIID";
            var iChallanList = obEntity.Database.SqlQuery<EXPCnfBill>(queryString);
            return iChallanList.ToList();
        }

        public ValidationMsg Save(EXP_CnFBill dataSet, int _userId, string pageUrl)
        {
            try
            {
                EXP_CnFBill obLcCnFBill = new EXP_CnFBill();

                //obLcCnFBill.LCID = dataSet.LCID;
                //obLcCnFBill.LCNo = dataSet.LCNo == null ? "" : dataSet.LCNo;//DalCommon.GetPreDefineNextCodeByUrl(pageUrl);

                obLcCnFBill.CIID = dataSet.CIID;
                obLcCnFBill.PLID = dataSet.PLID;


                obLcCnFBill.CnfBillID = dataSet.CnfBillID;
                obLcCnFBill.CnfBillNo = dataSet.CnfBillNo == null ? "" : (string)dataSet.CnfBillNo;
                obLcCnFBill.RefCnfBillNo = dataSet.RefCnfBillNo == null ? "" : (string)dataSet.RefCnfBillNo;
                obLcCnFBill.CnfBillDate = dataSet.CnfBillDate;
                obLcCnFBill.CnfBillNote = dataSet.CnfBillNote == null ? "" : (string)dataSet.CnfBillNote;

                obLcCnFBill.DutyAccountCharge = dataSet.DutyAccountCharge == null ? 0 : (decimal)dataSet.DutyAccountCharge;
                obLcCnFBill.CnfAgentID = dataSet.CnfAgentID == null ? 0 : dataSet.CnfAgentID;//obEntity.Sys_Buyer.Where(m => m.BuyerID == dataSet.CnfAgentID).FirstOrDefault().BuyerID; //dataSet.CnfAgentID; 
                obLcCnFBill.PortCharge = dataSet.PortCharge == null ? 0 : (decimal)dataSet.PortCharge;
                obLcCnFBill.NOCCharge = dataSet.NOCCharge == null ? 0 : (decimal)dataSet.NOCCharge;
                obLcCnFBill.BerthOperatorCharge = dataSet.BerthOperatorCharge == null ? 0 : (decimal)dataSet.BerthOperatorCharge;
                obLcCnFBill.AmendmentCharge = dataSet.AmendmentCharge == null ? 0 : (decimal)dataSet.AmendmentCharge;
                obLcCnFBill.AgencyCommission = dataSet.AgencyCommission == null ? 0 : (decimal)dataSet.AgencyCommission;
                obLcCnFBill.ChemicalTestCharge = dataSet.ChemicalTestCharge == null ? 0 : (decimal)dataSet.ChemicalTestCharge;
                obLcCnFBill.ExamineCharge = dataSet.ExamineCharge == null ? 0 : (decimal)dataSet.ExamineCharge;
                obLcCnFBill.SpecialDeliveryCharge = dataSet.SpecialDeliveryCharge == null ? 0 : (decimal)dataSet.SpecialDeliveryCharge;
                obLcCnFBill.ShippingCharge = dataSet.ShippingCharge == null ? 0 : (decimal)dataSet.ShippingCharge;
                obLcCnFBill.StampCharge = dataSet.StampCharge == null ? 0 : (decimal)dataSet.StampCharge;
                obLcCnFBill.TotalAmount = dataSet.TotalAmount == null ? 0 : (decimal)dataSet.TotalAmount; //(decimal)dataSet.TotalAmount;
                obLcCnFBill.BillOfEntryNo = dataSet.BillOfEntryNo == null ? "" : dataSet.BillOfEntryNo; ;
                obLcCnFBill.BillOfEntryDate = dataSet.BillOfEntryDate;
                obLcCnFBill.AssesmentValue = dataSet.AssesmentValue == null ? 0 : (decimal)dataSet.AssesmentValue;//Convert.ToDecimal(dataSet.AssesmentValue)==null? 0 :dataSet.AssesmentValue;
                obLcCnFBill.CnfBillCurrency = dataSet.CnfBillCurrency == null ? 0 : dataSet.CnfBillCurrency;
                obLcCnFBill.BillOfEntryNo = (string)dataSet.BillOfEntryNo == null ? "" : dataSet.BillOfEntryNo;
                obLcCnFBill.ExchangeCurrency = (byte)(dataSet.ExchangeCurrency) == null ? 0 : dataSet.ExchangeCurrency;
                obLcCnFBill.ExchangeRate = dataSet.ExchangeRate == null ? 0 : (decimal)dataSet.ExchangeRate;
                obLcCnFBill.ExchangeValue = dataSet.ExchangeValue == null ? 0 : (decimal)dataSet.ExchangeValue;
                obLcCnFBill.RecordStatus = "NCF";
                //obLcCnFBill.Remarks = dataSet.Remarks;


                obLcCnFBill.SetOn = DateTime.Now;
                obLcCnFBill.SetBy = 1;

                obLcCnFBill.IPAddress = GetIPAddress.LocalIPAddress();
                obEntity.EXP_CnFBill.Add(obLcCnFBill);
                int flag = obEntity.SaveChanges();
                if (flag == 1)
                {

                    _vmMsg.ReturnId = repository.LcmCnFBillRpository.Get().Last().CnfBillID;//dataSet.CnfBillID;
                    _vmMsg.ReturnCode = repository.LcmCnFBillRpository.GetByID(_vmMsg.ReturnId).CnfBillNo;
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

        public ValidationMsg Update(EXP_CnFBill dataSet, int _userId)
        {
            try
            {
                EXP_CnFBill obLcCnFBill = repository.ExpCnfBill.GetByID(dataSet.CnfBillID);

                    obLcCnFBill.CnfBillID = dataSet.CnfBillID;
                    obLcCnFBill.CnfBillNo = dataSet.CnfBillNo;
                    obLcCnFBill.RefCnfBillNo = dataSet.RefCnfBillNo;
                    obLcCnFBill.CnfBillDate = dataSet.CnfBillDate;
                    obLcCnFBill.CIID = dataSet.CIID;
                    obLcCnFBill.PLID = dataSet.PLID;

                    obLcCnFBill.BillOfEntryNo = dataSet.BillOfEntryNo;
                    obLcCnFBill.BillOfEntryDate = dataSet.BillOfEntryDate;
                    obLcCnFBill.AssesmentValue = Convert.ToDecimal(dataSet.AssesmentValue);
                    obLcCnFBill.CnfBillNote = dataSet.CnfBillNote;

                    obLcCnFBill.CnfAgentID = dataSet.CnfAgentID; //Convert.ToInt32(dataSet.CnfAgentID);
                    obLcCnFBill.DutyAccountCharge = dataSet.DutyAccountCharge;
                    obLcCnFBill.PortCharge = dataSet.PortCharge;
                    obLcCnFBill.ShippingCharge = dataSet.ShippingCharge;
                    obLcCnFBill.BerthOperatorCharge = dataSet.BerthOperatorCharge;
                    obLcCnFBill.NOCCharge = dataSet.NOCCharge;
                    obLcCnFBill.ExamineCharge = dataSet.ExamineCharge;
                    obLcCnFBill.SpecialDeliveryCharge = dataSet.SpecialDeliveryCharge;
                    obLcCnFBill.AmendmentCharge = dataSet.AmendmentCharge;
                    obLcCnFBill.ChemicalTestCharge = dataSet.ChemicalTestCharge;
                    obLcCnFBill.AgencyCommission = dataSet.AgencyCommission;
                    obLcCnFBill.StampCharge = dataSet.StampCharge;
           
                    obLcCnFBill.TotalAmount = dataSet.TotalAmount;

                    obLcCnFBill.CnfBillCurrency = dataSet.CnfBillCurrency;

                    obLcCnFBill.ExchangeCurrency = dataSet.ExchangeCurrency;
                    obLcCnFBill.ExchangeRate = dataSet.ExchangeRate;
                    obLcCnFBill.ExchangeValue = dataSet.ExchangeValue;

                    obLcCnFBill.BillOfEntryNo = dataSet.BillOfEntryNo;
                    obLcCnFBill.RecordStatus = "NCF";
                    obLcCnFBill.ModifiedOn = DateTime.Now;
                    obLcCnFBill.ModifiedBy = _userId;
                    obLcCnFBill.IPAddress = GetIPAddress.LocalIPAddress();

                    repository.ExpCnfBill.Update(obLcCnFBill);
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
                _vmMsg.Type = Enums.MessageType.Delete;
                _vmMsg.Msg = "Update Faild.";
            }
            return _vmMsg;
        }

        public ValidationMsg DeleteCnFBillList(int cnfBillID)
        {
            try
            {
                var cnfBillList = repository.ExpCnfBill.Get().Where(ob => ob.CnfBillID == cnfBillID).ToList();
                if (cnfBillList.Count > 0)
                {
                    foreach (var cnfBill in cnfBillList)
                    {
                        repository.ExpCnfBill.Delete(cnfBill);
                    }
                }
                repository.ExpCnfBill.Delete(cnfBillID);

                save = repository.Save();
                if (save == 1)
                {
                    _vmMsg.Type = Enums.MessageType.Delete;
                    _vmMsg.Msg = "Successfully deleted the record.";
                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Delete the Record.";
            }
            return _vmMsg;
        }









    }
}

