using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using ERP_Leather.ActionFilters;


namespace ERP_Leather.Controllers
{
    public class CnFBillsController : Controller
    {
        private UnitOfWork objRepository = new UnitOfWork();
        private DalLCMCnFBillInfo objDalLCMCnFBillInfo = new DalLCMCnFBillInfo();
        private ValidationMsg objValMssg = new ValidationMsg();
        private int _userId;




        [CheckUserAccess("CnFBills/CnFBills")]
        public ActionResult CnFBills()
        {

            ViewBag.Currency = new SelectList(objRepository.CurrencyRepository.Get(filter: ob => ob.IsActive == true), "CurrencyID", "CurrencyName");
            ViewBag.formTiltle = "C&F Bill Documentation";
            return View();
        }

        public ActionResult GetCnFBuyerList()
        {
            var bankList = objDalLCMCnFBillInfo.GetCnFBuyerList();
            return Json(bankList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLCNoList()
        {
            //var listLCNo = from temp in objRepository.LCOpeningRepository.Get().OrderByDescending(ob=>ob.LCID) select new { temp.LCID, temp.LCNo, temp.PIID, temp.PINo }; //obLCOpeningDAL.GetLCOpeningInfo();
            //return Json(listLCNo, JsonRequestBehavior.AllowGet);
            var result = from temp in objRepository.CommercialInvoiceRepository.Get().AsEnumerable()
                         join temp2 in objRepository.LCOpeningRepository.Get() on temp.LCID equals temp2.LCID
                         select new
                         {
                             LCID = temp.LCID,
                             LCNo = temp.LCNo,
                             PIID = temp2.PIID,
                             PINo = temp2.PINo,
                             CIID=temp.CIID,
                             CINo = temp.CINo
                             
                         };
            return Json(result, JsonRequestBehavior.AllowGet);



        }


        [HttpPost]
        public ActionResult Save(CnFBill dataSet)
        {
            _userId = Convert.ToInt32(Session["UserID"]);

            objValMssg = objDalLCMCnFBillInfo.SaveLCM_CnFBill(dataSet, _userId, "CnFBills/CnFBills");
            return Json(new { msg = objValMssg });
        }

        [HttpPost]
        public ActionResult Update(CnFBill dataSet)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            objValMssg = objDalLCMCnFBillInfo.UpdateLCM_CnFBill(dataSet, _userId);
            return Json(new { msg = objValMssg });
        }



        // ##################### Search Start ##############
        public ActionResult GetCnFBillInfo()
        {
            var listCnFBillInfo = (from temp in objRepository.LcmCnFBillRpository.Get().AsEnumerable()
                                  join temp2 in objRepository.SysBuyerRepository.Get() on temp.CnfAgentID equals temp2.BuyerID
                                  select new { 
                                      
                                    CnfBillNo = temp.CnfBillNo,
                                    CnfAgentID = temp.CnfAgentID,
                                    LCID = temp.LCID,
                                    LCNo = temp.LCNo,
                                    BuyerCode = temp2.BuyerCode,
                                    BuyerName = temp2.BuyerName,
                                    CnfBillID = temp.CnfBillID,//temp.LCNo,
                                    RecordStatus = DalCommon.ReturnRecordStatus(temp.RecordStatus),

                                      }).OrderByDescending(ob => ob.CnfBillID); //obLCOpeningDAL.GetLCOpeningInfo();
            return Json(listCnFBillInfo, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchLcmCnFBillInfoByLCNo(string search)//InsuranceNo
        {
            var lcInfo = from temp in objRepository.LcmCnFBillRpository.Get().Where(ob => ob.LCNo.StartsWith(search) || ob.LCNo == search)
                         select new { temp.CnfAgentID, temp.CnfBillNo, temp.LCNo, temp.LCID, temp.BillOfEntryNo,temp.RecordStatus};

            return Json(lcInfo, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetAutoCompleteData()
        {
            var data = objRepository.LcmCnFBillRpository.Get().Select(ob => ob.LCNo);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCnFBillInfoByCnfBillID(string CnfBillID)
        {
            int id = Convert.ToInt32(CnfBillID);
            LCM_CnfBill dataSet = objRepository.LcmCnFBillRpository.GetByID(id);

            CnFBill ob = new CnFBill();

            ob.LCID = dataSet.LCID;
            ob.LCNo = dataSet.LCNo;
            ob.CIID = dataSet.CIID;
            ob.CINo = dataSet.CINo;
            ob.CnfBillID = dataSet.CnfBillID;
            ob.CnfBillDate = dataSet.CnfBillDate == null ? "" : Convert.ToDateTime(dataSet.CnfBillDate).ToString("dd/MM/yyyy");
            ob.CnfBillNote = dataSet.CnfBillNote == null ? "" : dataSet.CnfBillNote;
            ob.CnfBillNo = dataSet.CnfBillNo;
            ob.RefCnfBillNo = dataSet.RefCnfBillNo;
            ob.Address = objRepository.BuyerAddressRepository.Get(filter: m => m.BuyerID == dataSet.CnfAgentID).FirstOrDefault().Address;
            ob.EmailAddress = objRepository.BuyerAddressRepository.Get(filter: m => m.BuyerID == dataSet.CnfAgentID).FirstOrDefault().EmailAddress;
            ob.ContactPerson = objRepository.BuyerAddressRepository.Get(filter: m => m.BuyerID == dataSet.CnfAgentID).FirstOrDefault().ContactPerson;
            ob.ContactNumber = objRepository.BuyerAddressRepository.Get(filter: m => m.BuyerID == dataSet.CnfAgentID).FirstOrDefault().ContactNumber;
            ob.PhoneNo = objRepository.BuyerAddressRepository.Get(filter: m => m.BuyerID == dataSet.CnfAgentID).FirstOrDefault().PhoneNo;


            ob.DutyAccountCharge = (decimal)dataSet.DutyAccountCharge == null ? 0 : (decimal)dataSet.DutyAccountCharge;
            ob.CnfAgentID = dataSet.CnfAgentID == null ? 0 : dataSet.CnfAgentID;
            ob.PortCharge = (decimal)dataSet.PortCharge == null ? 0 : (decimal)dataSet.PortCharge;
            ob.NOCCharge = (decimal)dataSet.NOCCharge == null ? 0 : (decimal)dataSet.NOCCharge;
            ob.BerthOperatorCharge = (decimal)dataSet.BerthOperatorCharge == null ? 0 : (decimal)dataSet.BerthOperatorCharge;
            ob.AmendmentCharge = (decimal)dataSet.AmendmentCharge == null ? 0 : (decimal)dataSet.AmendmentCharge;
            ob.AgencyCommission = dataSet.AgencyCommission == null ? 0 : (decimal)dataSet.AgencyCommission;
            ob.ChemicalTestCharge = (decimal)dataSet.ChemicalTestCharge == null ? 0 : (decimal)dataSet.ChemicalTestCharge;
            ob.ExamineCharge = (decimal)dataSet.ExamineCharge == null ? 0 : (decimal)dataSet.ExamineCharge;
            ob.SpecialDeliveryCharge = (decimal)dataSet.SpecialDeliveryCharge == null ? 0 : (decimal)dataSet.SpecialDeliveryCharge;
            ob.ShippingCharge = (decimal)dataSet.ShippingCharge == null ? 0 : (decimal)dataSet.ShippingCharge;
            ob.StampCharge = (decimal)dataSet.StampCharge == null ? 0 : (decimal)dataSet.StampCharge;
            //ob.BLVerifyCharge = (decimal)dataSet.BLVerifyCharge == null ? 0 : (decimal)dataSet.BLVerifyCharge;OtherCharge

            if (dataSet.BLVerifyCharge != 0)
            {
                ob.BLVerifyCharge = dataSet.BLVerifyCharge;
            }
            else
            {
                ob.BLVerifyCharge = 0;
            }
            
            
            
            
            if (dataSet.OtherCharge != 0)
            {
                ob.OtherCharge = dataSet.OtherCharge;
            }
            else
            {
                ob.OtherCharge = 0;
            }

            ob.TotalAmount = (decimal)dataSet.TotalAmount == null ? 0 : (decimal)dataSet.TotalAmount; 
            ob.BillOfEntryNo = dataSet.BillOfEntryNo;
            ob.BillOfEntryDate = dataSet.BillOfEntryDate == null ? "" : Convert.ToDateTime(dataSet.BillOfEntryDate).ToString("dd/MM/yyyy");
            ob.AssesmentValue = Convert.ToDecimal(dataSet.AssesmentValue);
            ob.CnfBillCurrency = dataSet.CnfBillCurrency;
            ob.BillOfEntryNo = (string)dataSet.BillOfEntryNo;
            ob.ExchangeCurrency = (byte)(dataSet.ExchangeCurrency);
            ob.ExchangeRate = (byte)dataSet.ExchangeRate;
            ob.ExchangeValue = (decimal)dataSet.ExchangeValue;
            ob.RecordStatus = dataSet.RecordStatus;
            ob.Remarks = dataSet.Remarks;
            ob.BuyerID = objRepository.SysBuyerRepository.GetByID(dataSet.CnfAgentID).BuyerID;
            ob.BuyerCode = objRepository.SysBuyerRepository.GetByID(dataSet.CnfAgentID).BuyerCode;
            ob.BuyerName = objRepository.SysBuyerRepository.GetByID(dataSet.CnfAgentID).BuyerName;
            ob.PINo = objRepository.LCOpeningRepository.GetByID(dataSet.LCID).PINo;

            return Json(ob, JsonRequestBehavior.AllowGet);

        }

        // ##################### Search END ##############


        public ActionResult UpdateRecordStatus(string remarks, string cnfBillID)
        {
            try
            {
                if ((remarks != "" && cnfBillID != ""))
                {
                    LCM_CnfBill ob = objRepository.LcmCnFBillRpository.GetByID(Convert.ToInt32(cnfBillID));
                    // ob.ApprovalAdvice = note;
                    ob.RecordStatus = "NCF";
                    objRepository.LcmCnFBillRpository.Update(ob);
                    int flag = objRepository.Save();
                    if (flag == 1)
                    {
                        objValMssg.Type = Enums.MessageType.Success;
                        objValMssg.Msg = "Confirmed Successfully.";
                    }
                    else
                    {
                        objValMssg.Type = Enums.MessageType.Error;
                        objValMssg.Msg = "Confirm Faild.";
                    }
                }
                else
                {
                    objValMssg.Type = Enums.MessageType.Error;
                    objValMssg.Msg = "Data Save First Before Checked.";
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
            return Json(new { msg = objValMssg });
        }

        public ActionResult ConfirmRecordStatus(string cnfBillID)
        {
            try
            {
                if (cnfBillID != "")
                {
                    LCM_CnfBill ob = objRepository.LcmCnFBillRpository.GetByID(Convert.ToInt32(cnfBillID));
                    if (ob.RecordStatus == "NCF")
                    {
                        ob.RecordStatus = "CNF";
                        //ob.CheckedBy = Convert.ToInt32(Session["UserID"]);
                        //ob.CheckDate = DateTime.Now;
                        objRepository.LcmCnFBillRpository.Update(ob);
                        int flag = objRepository.Save();
                        if (flag == 1)
                        {
                            objValMssg.Type = Enums.MessageType.Success;
                            objValMssg.Msg = "Confirmed Successfully.";
                        }
                        else
                        {
                            objValMssg.Type = Enums.MessageType.Error;
                            objValMssg.Msg = "Confirmed Faild.";
                        }
                    }
                    else
                    {
                        objValMssg.Type = Enums.MessageType.Error;
                        objValMssg.Msg = "Record Status is Empty.";
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
            return Json(new { msg = objValMssg });
        }


        public ActionResult DeleteCnFBillList(int cnfBillID)
        {
            objValMssg = objDalLCMCnFBillInfo.DeleteCnFBillList(cnfBillID);//DeletePackingList(cnfBillID);
            return Json(objValMssg, JsonRequestBehavior.AllowGet);
        }





	}
}