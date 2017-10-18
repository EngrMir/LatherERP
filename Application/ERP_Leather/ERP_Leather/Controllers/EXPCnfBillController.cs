using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using ERP_Leather.ActionFilters;

namespace ERP_Leather.Controllers
{
    public class EXPCnfBillController : Controller
    {
        DalSysStore objDalStore = new DalSysStore();
        private DalEXPCnfBill objDalEXPCnfBill;
        private ValidationMsg _vmMsg;
        private UnitOfWork repository;
        private int _userId;

        public EXPCnfBillController()
        {
            _vmMsg = new ValidationMsg();
            objDalEXPCnfBill = new DalEXPCnfBill();
            repository = new UnitOfWork();
        }


        [CheckUserAccess("EXPCnfBill/EXPCnfBill")]
        public ActionResult EXPCnfBill()
        {
            ViewBag.Currency = new SelectList(repository.CurrencyRepository.Get(filter: ob => ob.IsActive == true), "CurrencyID", "CurrencyName");
            ViewBag.formTiltle = "Export C&F Bill Documentation";
            return View();
        }

        public ActionResult GetCnFBuyerList()
        {
            var bankList = objDalEXPCnfBill.GetCnFBuyerList();
            return Json(bankList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLCNoList()
        {
            var result = objDalEXPCnfBill.GetLCList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region Start EXPCnfBill Save & Update
        [HttpPost]
        public ActionResult Save(EXP_CnFBill dataSet)
        {
            _userId = Convert.ToInt32(Session["UserID"]);

            _vmMsg = objDalEXPCnfBill.Save(dataSet, _userId, "EXPCnfBill/EXPCnfBill");
            return Json(new { msg = _vmMsg });
        }

        [HttpPost]
        public ActionResult Update(EXP_CnFBill dataSet)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            _vmMsg = objDalEXPCnfBill.Update(dataSet, _userId);
            return Json(new { msg = _vmMsg });
        }
        #endregion



        // ##################### Search Start #############
        public ActionResult GetCnFBillInfo()
        {
            var listCnFBillInfo = (from temp in repository.ExpCnfBill.Get().AsEnumerable()
                                   join temp2 in repository.ExpCommercialInvoiceRepository.Get() on temp.CIID equals temp2.CIID
                                   join temp3 in repository.ExpPackingListRepository.Get() on temp.PLID equals temp3.PLID
                                   join temp4 in repository.SysBuyerRepository.Get() on temp.CnfAgentID equals temp4.BuyerID

                                   select new
                                   {
                                       CnfBillID = temp.CnfBillID,
                                       CnfBillNo = temp.CnfBillNo,
                                       CnfAgentID = temp.CnfAgentID,
                                       CIID = temp2.CIID,
                                       CINo = temp2.CINo,
                                       PLID = temp3.PLID,
                                       PLNo = temp3.PLNo,
                                       AgentCode = temp4.BuyerCode,
                                       AgentName = temp4.BuyerName,                             
                                       RecordStatus = DalCommon.ReturnRecordStatus(temp.RecordStatus),

                                   }).OrderByDescending(ob => ob.CnfBillID); 
            return Json(listCnFBillInfo, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchLcmCnFBillInfoByLCNo(string search)//InsuranceNo
        {
            var lcInfo =    (from temp in repository.ExpCnfBill.Get().Where(ob => ob.CnfBillNo.StartsWith(search) || ob.CnfBillNo == search)
                            join temp2 in repository.ExpCommercialInvoiceRepository.Get() on temp.CIID equals temp2.CIID
                            join temp3 in repository.ExpPackingListRepository.Get() on temp.PLID equals temp3.PLID
                            join temp4 in repository.SysBuyerRepository.Get() on temp.CnfAgentID equals temp4.BuyerID
                            join temp5 in repository.EXPCommercialInvoicePIRepository.Get() on temp2.CIID equals temp5.CIID
                            join temp6 in repository.ExpLCOpening.Get() on temp5.LCID equals temp6.LCID
                            select new
                                   {
                                       CnfBillID = temp.CnfBillID,
                                       CnfBillNo = temp.CnfBillNo,
                                       CnfAgentID = temp.CnfAgentID == null ? 0 : temp.CnfAgentID,
                                       LCID = temp6.LCID == null ? 0 : temp6.LCID,
                                       LCNo = temp6.LCNo,
                                       CIID = temp2.CIID == null ? 0 : temp2.CIID,
                                       CINo = temp2.CINo,
                                       PLID = temp3.PLID == null ? 0 : temp3.PLID,
                                       PLNo = temp3.PLNo,
                                       BuyerCode = temp4.BuyerCode,
                                       BuyerName = temp4.BuyerName,                             
                                       RecordStatus = DalCommon.ReturnRecordStatus(temp.RecordStatus),

                                   }).OrderByDescending(ob => ob.CnfBillID); 
            return Json(lcInfo, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAutoCompleteData()
        {
            var data = repository.ExpCnfBill.Get().Select(ob => ob.CnfBillNo);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCnFBillInfoByCnfBillID(string CnfBillID)
        {
            int id = Convert.ToInt32(CnfBillID);
            EXP_CnFBill dataSet = repository.ExpCnfBill.GetByID(id);
            EXPCnfBill ob = new EXPCnfBill();

             //ob.LCID = LC.LC//dataSet.LCID;
            //ob.LCNo = repository.EXPCommercialInvoicePIRepository.Get(filter: m => m.CIID == dataSet.CIID).FirstOrDefault().LCN;//dataSet.LCNo;

            ob.CIID = dataSet.CIID;
            ob.CIRefNo = repository.ExpCommercialInvoiceRepository.Get(filter: m => m.CIID == dataSet.CIID).FirstOrDefault().CIRefNo;//dataSet.CINo;

            ob.BalesNo = repository.ExpPackingListRepository.Get(filter: m => m.CIID == (dataSet.CIID == null ? 0 : dataSet.CIID)).FirstOrDefault().BalesNo;//dataSet.CINo;
            ob.TotalBales = repository.ExpPackingListRepository.Get(filter: m => m.CIID == (dataSet.CIID == null ? 0 : dataSet.CIID)).FirstOrDefault().BaleQty;//dataSet.CINo;
            
            ob.PLID = dataSet.PLID;
            ob.PLNo = repository.ExpPackingListRepository.Get(filter: m => m.PLID == dataSet.PLID).FirstOrDefault().PLNo;
            ob.CnfBillID = dataSet.CnfBillID;
            ob.CnfBillNo = dataSet.CnfBillNo;//
            ob.RefCnfBillNo = dataSet.RefCnfBillNo;
            ob.Address = repository.BuyerAddressRepository.Get(filter: m => m.BuyerID == dataSet.CnfAgentID).FirstOrDefault().Address;
            ob.EmailAddress = repository.BuyerAddressRepository.Get(filter: m => m.BuyerID == dataSet.CnfAgentID).FirstOrDefault().EmailAddress;
            ob.ContactPerson = repository.BuyerAddressRepository.Get(filter: m => m.BuyerID == dataSet.CnfAgentID).FirstOrDefault().ContactPerson;
            ob.ContactNumber = repository.BuyerAddressRepository.Get(filter: m => m.BuyerID == dataSet.CnfAgentID).FirstOrDefault().ContactNumber;
            ob.PhoneNo = repository.BuyerAddressRepository.Get(filter: m => m.BuyerID == dataSet.CnfAgentID).FirstOrDefault().PhoneNo;

            ob.CnfBillID = dataSet.CnfBillID;
            ob.CnfBillDate = Convert.ToDateTime(dataSet.CnfBillDate).ToString("dd/MM/yyyy");
            ob.CnfBillNote = dataSet.CnfBillNote == null ? "" : dataSet.CnfBillNote;

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
           // ob.BLVerifyCharge = (decimal)dataSet.BLVerifyCharge == null ? 0 : (decimal)dataSet.BLVerifyCharge;

            ob.TotalAmount = (decimal)dataSet.TotalAmount == null ? 0 : (decimal)dataSet.TotalAmount;
            ob.BillOfEntryNo = dataSet.BillOfEntryNo;
            ob.BillOfEntryDate = Convert.ToDateTime(dataSet.BillOfEntryDate).ToString("dd/MM/yyyy");
            ob.AssesmentValue = Convert.ToDecimal(dataSet.AssesmentValue);
            ob.CnfBillCurrency = dataSet.CnfBillCurrency;
            ob.BillOfEntryNo = (string)dataSet.BillOfEntryNo;
            ob.ExchangeCurrency = (byte)(dataSet.ExchangeCurrency);
            ob.ExchangeRate = (byte)dataSet.ExchangeRate;
            ob.ExchangeValue = (decimal)dataSet.ExchangeValue;
            ob.RecordStatus = dataSet.RecordStatus;
            //ob.Remarks = dataSet.Remarks;
            ob.BuyerID = repository.SysBuyerRepository.GetByID(dataSet.CnfAgentID).BuyerID;
            ob.BuyerCode = repository.SysBuyerRepository.GetByID(dataSet.CnfAgentID).BuyerCode;
            ob.BuyerName = repository.SysBuyerRepository.GetByID(dataSet.CnfAgentID).BuyerName;
     



            return Json(ob, JsonRequestBehavior.AllowGet);

        }

        // ##################### Search END ##############


        public ActionResult ConfirmRecordStatus(string cnfBillID)
        {
            try
            {
                if (cnfBillID != "")
                {
                    EXP_CnFBill ob = repository.ExpCnfBill.GetByID(Convert.ToInt32(cnfBillID));
                    if (ob.RecordStatus == "NCF")
                    {
                        ob.RecordStatus = "CNF";
                        repository.ExpCnfBill.Update(ob);
                        int flag = repository.Save();
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
                    else
                    {
                        _vmMsg.Type = Enums.MessageType.Error;
                        _vmMsg.Msg = "Record Status has not found.";
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
            return Json(new { msg = _vmMsg });
        }


        public ActionResult DeleteCnFBillList(int cnfBillID)
        {
            _vmMsg = objDalEXPCnfBill.DeleteCnFBillList(cnfBillID);//DeletePackingList(cnfBillID);
            return Json(_vmMsg, JsonRequestBehavior.AllowGet);
        }
    }
}