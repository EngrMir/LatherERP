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
    public class DalLCMCnFBillInfo
    {


        private readonly BLC_DEVEntities obEntity;
        private UnitOfWork repository;
        private ValidationMsg _vmMsg;
        private int save;


        public DalLCMCnFBillInfo()
        {
            obEntity = new BLC_DEVEntities();
            _vmMsg = new ValidationMsg();
            repository = new UnitOfWork();
        }


        public List<CnFBill> GetCnFBuyerList()
        {
            var queryString = @"SELECT 
	                                b.BuyerID,
	                                b.BuyerCode,
	                                b.BuyerName, 
	                                b.BuyerCategory,
	                                b.BuyerType,
                                    ISNULL(ba.BuyerAddressID,0)BuyerAddressID,
	                                ISNULL(ba.Address,0)Address,
	                                ISNULL(ba.ContactPerson,0)ContactPerson,
	                                ISNULL(ba.ContactNumber,0)ContactNumber,
	                                ISNULL(ba.EmailAddress,0)EmailAddress,
	                                ISNULL(ba.PhoneNo,0)PhoneNo
                                FROM Sys_Buyer b
	                                INNER JOIN Sys_BuyerAddress ba ON b.BuyerID = ba.BuyerID
	                               
	                              
                                WHERE  --b.IsActive='true'
	                                   --AND ba.IsActive='true'
                                        b.BuyerCategory='CNF Agent'
									   --AND b.BuyerType='Foreign Agent'
                                ORDER BY b.BuyerID DESC";
            var iChallanList = obEntity.Database.SqlQuery<CnFBill>(queryString);
            return iChallanList.ToList();
        }

        public ValidationMsg SaveLCM_CnFBill(CnFBill dataSet, int _userId, string pageUrl)
        {
            try
            {
                LCM_CnfBill obLcCnFBill = new LCM_CnfBill();

                obLcCnFBill.LCID = dataSet.LCID;
                obLcCnFBill.LCNo = dataSet.LCNo == null ? "" : dataSet.LCNo;//DalCommon.GetPreDefineNextCodeByUrl(pageUrl);

                obLcCnFBill.CIID = dataSet.CIID;
                obLcCnFBill.CINo = dataSet.CINo == null ? "" : dataSet.CINo;


                obLcCnFBill.CnfBillID = dataSet.CnfBillID;
                //obLcCnFBill.CnfBillNo = dataSet.CnfBillNo==null?"":dataSet.CnfBillNo;
                obLcCnFBill.CnfBillNo = DalCommon.GetPreDefineNextCodeByUrl(pageUrl);
                obLcCnFBill.RefCnfBillNo = dataSet.RefCnfBillNo == null ? "" : dataSet.RefCnfBillNo;
                obLcCnFBill.CnfBillDate = dataSet.CnfBillDate == null ? (DateTime?)null : DalCommon.SetDate(dataSet.CnfBillDate);
                obLcCnFBill.CnfBillNote = dataSet.CnfBillNote == null ? "" : (string)dataSet.CnfBillNote;

                obLcCnFBill.DutyAccountCharge = dataSet.DutyAccountCharge == null ? 0 : (decimal)dataSet.DutyAccountCharge;
                obLcCnFBill.CnfAgentID = dataSet.CnfAgentID == null ? 0 : obEntity.Sys_Buyer.Where(m => m.BuyerID == dataSet.CnfAgentID).FirstOrDefault().BuyerID; //dataSet.CnfAgentID; 
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
                obLcCnFBill.BLVerifyCharge = dataSet.BLVerifyCharge == null ? 0 : (decimal)dataSet.BLVerifyCharge;//OtherCharge
                obLcCnFBill.OtherCharge = dataSet.OtherCharge == null ? 0 : (decimal)dataSet.OtherCharge;//
                obLcCnFBill.TotalAmount = dataSet.TotalAmount == null ? 0 : (decimal)dataSet.TotalAmount; //(decimal)dataSet.TotalAmount;
                obLcCnFBill.BillOfEntryNo = dataSet.BillOfEntryNo == null ? "" : dataSet.BillOfEntryNo; ;
                obLcCnFBill.BillOfEntryDate = dataSet.BillOfEntryDate == null ? (DateTime ?)null : DalCommon.SetDate(dataSet.BillOfEntryDate);
                obLcCnFBill.AssesmentValue = dataSet.AssesmentValue == null ? 0 : (decimal)dataSet.AssesmentValue;//Convert.ToDecimal(dataSet.AssesmentValue)==null? 0 :dataSet.AssesmentValue;
                obLcCnFBill.CnfBillCurrency = dataSet.CnfBillCurrency == null ? 0 : dataSet.CnfBillCurrency;
                obLcCnFBill.BillOfEntryNo = (string)dataSet.BillOfEntryNo == null ? "" : dataSet.BillOfEntryNo;
                obLcCnFBill.ExchangeCurrency = (byte)(dataSet.ExchangeCurrency) == null ? 0 : dataSet.ExchangeCurrency;
                obLcCnFBill.ExchangeRate = dataSet.ExchangeRate == null ? 0 : (decimal)dataSet.ExchangeRate;
                obLcCnFBill.ExchangeValue = dataSet.ExchangeValue == null ? 0 : (decimal)dataSet.ExchangeValue;
                obLcCnFBill.RecordStatus = "NCF";
                obLcCnFBill.Remarks = dataSet.Remarks == null ? "" : dataSet.Remarks;


                obLcCnFBill.SetOn = DateTime.Now;

                obLcCnFBill.SetBy = 1;
                obLcCnFBill.ModifiedOn = DateTime.Now;
                obLcCnFBill.ModifiedBy = _userId;
                obLcCnFBill.IPAddress = GetIPAddress.LocalIPAddress();
                obEntity.LCM_CnfBill.Add(obLcCnFBill);
                int flag = obEntity.SaveChanges();
                if (flag == 1)
                {

                    //_vmMsg.ReturnId =  repository.LcmCnFBillRpository.Get().Last().CnfBillID;//dataSet.CnfBillID;
                    _vmMsg.ReturnCode = obLcCnFBill.CnfBillNo;
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

        public ValidationMsg SaveLCM_CnFBill(LCM_CnfBill dataSet, int _userId, string pageUrl)
        {
            return _vmMsg;
        }
        public ValidationMsg UpdateLCM_CnFBill(LCM_CnfBill dataSet, int _userId)
        {
            return _vmMsg;
        }
        public ValidationMsg UpdateLCM_CnFBill(CnFBill dataSet, int _userId)
        {
            try
            {
                LCM_CnfBill obLcCnFBill = repository.LcmCnFBillRpository.GetByID(dataSet.CnfBillID);

                if (!obLcCnFBill.RecordStatus.Equals("CNF"))
                {
                    obLcCnFBill.LCID = dataSet.LCID;
                    obLcCnFBill.LCNo = dataSet.LCNo;

                    obLcCnFBill.CIID = dataSet.CIID;
                    obLcCnFBill.CINo = dataSet.CINo;

                    obLcCnFBill.CnfBillID = dataSet.CnfBillID;
                    obLcCnFBill.RefCnfBillNo = dataSet.RefCnfBillNo;
                    obLcCnFBill.CnfBillNo = dataSet.CnfBillNo;
                    obLcCnFBill.CnfBillDate = DalCommon.SetDate(dataSet.CnfBillDate); 
                    obLcCnFBill.CnfBillNote = dataSet.CnfBillNote;

                    obLcCnFBill.DutyAccountCharge = dataSet.DutyAccountCharge;
                    obLcCnFBill.CnfAgentID = dataSet.CnfAgentID; //Convert.ToInt32(dataSet.CnfAgentID);
                    obLcCnFBill.PortCharge = dataSet.PortCharge;
                    obLcCnFBill.NOCCharge = dataSet.NOCCharge;
                    obLcCnFBill.BerthOperatorCharge = dataSet.BerthOperatorCharge;
                    obLcCnFBill.AmendmentCharge = dataSet.AmendmentCharge;
                    obLcCnFBill.AgencyCommission = dataSet.AgencyCommission;
                    obLcCnFBill.ChemicalTestCharge = dataSet.ChemicalTestCharge;
                    obLcCnFBill.ExamineCharge = dataSet.ExamineCharge;
                    obLcCnFBill.SpecialDeliveryCharge = dataSet.SpecialDeliveryCharge;
                    obLcCnFBill.ShippingCharge = dataSet.ShippingCharge;
                    obLcCnFBill.StampCharge = dataSet.StampCharge;
                    obLcCnFBill.BLVerifyCharge = dataSet.BLVerifyCharge;
                    obLcCnFBill.OtherCharge = dataSet.OtherCharge;

                    obLcCnFBill.TotalAmount = dataSet.TotalAmount;
                    obLcCnFBill.BillOfEntryNo = dataSet.BillOfEntryNo;
                    obLcCnFBill.BillOfEntryDate = DalCommon.SetDate(dataSet.BillOfEntryDate);
                    obLcCnFBill.AssesmentValue = Convert.ToDecimal(dataSet.AssesmentValue);
                    obLcCnFBill.CnfBillCurrency = dataSet.CnfBillCurrency;
                    obLcCnFBill.BillOfEntryNo = dataSet.BillOfEntryNo;
                    obLcCnFBill.ExchangeCurrency = dataSet.ExchangeCurrency;
                    obLcCnFBill.ExchangeRate = dataSet.ExchangeRate;
                    obLcCnFBill.ExchangeValue = dataSet.ExchangeValue;
                    obLcCnFBill.Remarks = dataSet.Remarks;


                    obLcCnFBill.ModifiedOn = DateTime.Now;
                    obLcCnFBill.ModifiedBy = _userId;
                    obLcCnFBill.IPAddress = GetIPAddress.LocalIPAddress();

                    repository.LcmCnFBillRpository.Update(obLcCnFBill);
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
                else
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Data already confirmed. You can't update this.";
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

        public ValidationMsg DeleteCnFBillList(int cnfBillID)
        {
            try
            {
                var cnfBillList = repository.LcmCnFBillRpository.Get().Where(ob => ob.CnfBillID == cnfBillID).ToList();
                if (cnfBillList.Count > 0)
                {
                    foreach (var cnfBill in cnfBillList)
                    {
                        repository.LcmCnFBillRpository.Delete(cnfBill);
                    }
                }

                var cnfBillListItems = repository.LcmCnFBillRpository.Get().Where(ob => ob.CnfBillID == cnfBillID).ToList();
                if (cnfBillListItems.Count > 0)
                {
                    foreach (var item in cnfBillListItems)
                    {
                        repository.LcmCnFBillRpository.Delete(item);
                    }
                }
                repository.LcmCnFBillRpository.Delete(cnfBillID);

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
                _vmMsg.Msg = "Failed to delete the record.";
            }
            return _vmMsg;
        }


    }
}
