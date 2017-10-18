using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using ERP_Leather.ActionFilters;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ERP_Leather.Controllers
{
    public class LCRetirementController : Controller
    {
        private UnitOfWork objRepository = new UnitOfWork();
        private DalLCRetirement objDalLCRetirement = new DalLCRetirement();
        private DalSysCurrency _objCurrency=new DalSysCurrency();
        private ValidationMsg objValMssg = new ValidationMsg();
        private int _userId;
        // private decimal value;

        [CheckUserAccess("LCRetirement/LCRetirement")]
        public ActionResult LCRetirement()
        {
            ViewBag.Currency = new SelectList(objRepository.CurrencyRepository.Get(filter: ob => ob.IsActive == true), "CurrencyID", "CurrencyName");
            ViewBag.CurrencyList = _objCurrency.GetAllActiveCurrency();
            ViewBag.ExchangeCurrencyList = _objCurrency.GetAllActiveCurrency();
            return View();
        }


        public ActionResult GetLCNoList()
        {
            var lcList = objDalLCRetirement.GetLCNoList();
            return Json(lcList, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult Save(LCMLCRetirement dataSet)
        {
            _userId = Convert.ToInt32(Session["UserID"]);

            objValMssg = objDalLCRetirement.SaveLCM_LCRetirement(dataSet, _userId, "LCRetirement/LCRetirement");
            return Json(new { msg = objValMssg });
        }

        [HttpPost]
        public ActionResult Update(LCMLCRetirement dataSet)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            objValMssg = objDalLCRetirement.UpdateLCM_LCRetirement(dataSet, _userId);
            return Json(new { msg = objValMssg });
        }




        // ##################### Search Start ##############


        public ActionResult GetLcmRetirementInfo()
        {
            var listLcmRetirementInfo = (from temp in objRepository.LcmRetirementRpository.Get().AsEnumerable()
                                         join temp2 in objRepository.LCOpeningRepository.Get() on temp.LCID equals temp2.LCID
                                         join temp3 in objRepository.PRQ_ChemicalPIRepository.Get() on temp2.PIID equals temp3.PIID
                                         join temp4 in objRepository.SysSupplierRepository.Get() on temp3.SupplierID equals temp4.SupplierID

                                  select new
                                  {
                                      LCRetirementID = temp.LCRetirementID,
                                      LCRetirementNo = temp.LCRetirementNo,
                                      LCRetirementDate = temp.LCRetirementDate == null ? " " : Convert.ToDateTime(temp.LCRetirementDate).ToString("dd/MM/yyyy"),//temp.LCRetirementDate,
                                      LCID = temp.LCID,
                                      LCNo = temp.LCNo,
                                      LCDate = temp2.LCDate == null ? " " : Convert.ToDateTime(temp2.LCDate).ToString("dd/MM/yyyy"),
                                      SupplierID = temp4.SupplierName,
                                      SupplierName = temp4.SupplierName,
                                      BillValue = temp.BillValue,
                                      LessMargin = temp.LessMargin,
                                      ExchangeCurrency = temp.ExchangeCurrency,
                                      LCRCurrency=temp.LCRCurrency,
                                      RecordStatus = DalCommon.ReturnRecordStatus(temp.RecordStatus),//temp.RecordStatus,


                                  }).OrderByDescending(ob => ob.LCRetirementID); //obLCOpeningDAL.GetLCOpeningInfo();
            return Json(listLcmRetirementInfo, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchLcmRetirementInfoByLCNo(string search)//InsuranceNo
        {
            var lcInfo = from temp in objRepository.LcmRetirementRpository.Get().Where(ob => ob.LCNo.StartsWith(search) || ob.LCNo == search)
                         select new { temp.LCRetirementID, temp.LCID, temp.LCNo, temp.LCRetirementNo };

            return Json(lcInfo, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAutoCompleteData()
        {
            var data = objRepository.LcmRetirementRpository.Get().Select(ob => ob.LCNo);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLcmRetirementByLCRetirementID(string LCRetirementID)
        {

            int id = Convert.ToInt32(LCRetirementID);
            LCM_LCRetirement dataSet = objRepository.LcmRetirementRpository.GetByID(id);

            LCMLCRetirement ob = new LCMLCRetirement();

            ob.LCNo = dataSet.LCNo;
            ob.LCID = Convert.ToInt32(dataSet.LCID);
            ob.LCRetirementID = dataSet.LCRetirementID;
            ob.LCRetirementNo = dataSet.LCRetirementNo;
            ob.LCRetirementDate = dataSet.LCRetirementDate==null ? "":Convert.ToDateTime(dataSet.LCRetirementDate).ToString("dd/MM/yyyy");
            ob.BillValue = dataSet.BillValue;

            ob.LCRCurrency = dataSet.LCRCurrency;
            ob.ExchangeRate = dataSet.ExchangeRate == null ? 0 : dataSet.ExchangeRate;
            ob.ExchangeCurrency = dataSet.ExchangeCurrency;

            if (dataSet.ExchangeValue == null)
            {
                ob.ExchangeValue = 0;
            }
            else
            {
                ob.ExchangeValue = (decimal)dataSet.ExchangeValue;
            }

            if (dataSet.LessMargin == null)
            {
                ob.LessMargin = 0;
            }
            else
            {
                ob.LessMargin = (decimal)dataSet.LessMargin;
            }

            ob.InterestPersent =dataSet.InterestPersent;
            ob.InterestAmount = (decimal)dataSet.InterestAmount;


            if (dataSet.OtherCharge == null)
            {
                ob.OtherCharge = 0;
            }
            else
            {
                ob.OtherCharge = dataSet.OtherCharge;
            }

            if (dataSet.BankCommissionAmt == null)
            {
                ob.BankCommissionAmt = 0;
            }
            else
            {
                ob.BankCommissionAmt = (decimal)dataSet.BankCommissionAmt;
            }


            if (dataSet.SwiftCharge == null)
            {
                ob.SwiftCharge = 0;
            }
            else
            {
                ob.SwiftCharge = (decimal)dataSet.SwiftCharge;
            }


            if (dataSet.TotalAmount == null)
            {
                ob.TotalAmount = 0;
            }
            else
            {
                ob.TotalAmount = (decimal)dataSet.TotalAmount;
            }


            if (dataSet.GrandTotal == null)
            {
                ob.GrandTotal = 0;
            }
            else
            {
                ob.GrandTotal = (decimal)dataSet.GrandTotal;
            }

            ob.RecordStatus = dataSet.RecordStatus;
            ob.Remarks = dataSet.Remarks;

            return Json(ob, JsonRequestBehavior.AllowGet);

        }

        // ##################### Search END ##############

        public ActionResult DeleteLCRetirementID(int lcRetirementID)
        {
            objValMssg = objDalLCRetirement.DeleteLCRetirementID(lcRetirementID);//DeletePackingList(cnfBillID);
            return Json(objValMssg, JsonRequestBehavior.AllowGet);
        }



        public ActionResult ConfirmRecordStatus(string lcRetirementID)
        {
            try
            {
                if (lcRetirementID != "")
                {
                    LCM_LCRetirement ob = objRepository.LcmRetirementRpository.GetByID(Convert.ToInt32(lcRetirementID));
                    if (ob.RecordStatus == "NCF")
                    {
                        ob.RecordStatus = "CNF";
                        //ob.CheckedBy = Convert.ToInt32(Session["UserID"]);
                        //ob.CheckDate = DateTime.Now;
                        objRepository.LcmRetirementRpository.Update(ob);
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










    }
}