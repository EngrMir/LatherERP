using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using ERP_Leather.ActionFilters;
using System.Data.Entity.Validation;

namespace ERP_Leather.Controllers
{
    public class LCMLCRetirementController : Controller
    {
        private UnitOfWork objRepository = new UnitOfWork();
        private DalLCRetirement objDalLCRetirement = new DalLCRetirement();
        private DalSysCurrency _objCurrency = new DalSysCurrency();
        private ValidationMsg objValMssg = new ValidationMsg();
        private int _userId;

        [CheckUserAccess("LCMLCRetirement/LCMLCRetirement")]
        public ActionResult LCMLCRetirement()
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

        //********************************************* SAVE CODE ***************************************************//



        [HttpPost]
        public ActionResult SaveAndUpdate(LCMLCRetirement model)
        {
            objValMssg = model.LCRetirementID == 0 ? objDalLCRetirement.SaveLCM_LCRetirement(model, Convert.ToInt32(Session["UserID"]), "LCMLCRetirement/LCMLCRetirement") :
                                                    objDalLCRetirement.UpdateLCM_LCRetirement(model, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = objValMssg });
        }

        //********************************************* END OF SAVE CODE ***************************************************//


        //********************************************* UPDATE CODE ***************************************************//
        //[HttpPost]
        //public ActionResult Update(LCMLCRetirement dataSet)
        //{
        //    _userId = Convert.ToInt32(Session["UserID"]);
        //    objValMssg = objDalLCRetirement.UpdateLCM_LCRetirement(dataSet, _userId);
        //    return Json(new { msg = objValMssg });
        //}

        //********************************************* END OF UPDATE CODE ***************************************************//


        //********************************************* SEARCH CODE ***************************************************//
        public ActionResult SearchWindowOfLCRetirement()
        {
            var searchList = (from temp in objRepository.LcmRetirementRpository.Get().AsEnumerable()
                              join temp2 in objRepository.CurrencyRepository.Get() on temp.LCRCurrency equals temp2.CurrencyID
                              select new
                              {
                                  LCRetirementID = temp.LCRetirementID,
                                  LCRetirementNo = temp.LCRetirementNo,
                                  LCRetirementDate = temp.LCRetirementDate == null ? " " : Convert.ToDateTime(temp.LCRetirementDate).ToString("dd/MM/yyyy"),
                                  LCID = temp.LCID,
                                  LCNo = temp.LCNo,
                                  BillValue = temp.BillValue,
                                  LCRCurrency = temp.LCRCurrency,
                                  CurrencyName = temp2.CurrencyName,
                                  GrandTotal = temp.GrandTotal,
                                  RecordStatus = DalCommon.ReturnRecordStatus(temp.RecordStatus)

                              }).OrderByDescending(o => o.LCRetirementID);
            return Json(searchList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchLCByLCNo(string search)
        {
            var lcInfo = (from temp in objRepository.LcmRetirementRpository.Get().AsEnumerable()
                          join temp2 in objRepository.CurrencyRepository.Get() on temp.LCRCurrency equals temp2.CurrencyID
                          select new
                          {
                              LCRetirementID = temp.LCRetirementID,
                              LCRetirementNo = temp.LCRetirementNo,
                              LCRetirementDate = temp.LCRetirementDate,
                              LCID = temp.LCID,
                              LCNo = temp.LCNo,
                              BillValue = temp.BillValue,
                              LCRCurrency = temp.LCRCurrency,
                              CurrencyName = temp2.CurrencyName,
                              GrandTotal = temp.GrandTotal,
                              RecordStatus = DalCommon.ReturnRecordStatus(temp.RecordStatus)

                          }).Where(ob => (ob.LCRetirementNo.StartsWith(search) || ob.LCRetirementNo == search)).OrderByDescending(o => o.LCRetirementID);

            return Json(lcInfo, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BtnSearchLCByLCNo(string search)
        {
            var lcInfo = (from temp in objRepository.LcmRetirementRpository.Get().AsEnumerable()
                          join temp2 in objRepository.CurrencyRepository.Get() on temp.LCRCurrency equals temp2.CurrencyID
                          select new
                          {
                              LCRetirementID = temp.LCRetirementID,
                              LCRetirementNo = temp.LCRetirementNo,
                              LCRetirementDate = temp.LCRetirementDate,
                              LCID = temp.LCID,
                              LCNo = temp.LCNo,
                              BillValue = temp.BillValue,
                              LCRCurrency = temp.LCRCurrency,
                              CurrencyName = temp2.CurrencyName,
                              GrandTotal = temp.GrandTotal,
                              RecordStatus = DalCommon.ReturnRecordStatus(temp.RecordStatus)

                          }).Where(ob => (ob.LCRetirementNo.StartsWith(search) || ob.LCRetirementNo == search)).OrderByDescending(o => o.LCRetirementID);

            return Json(lcInfo, JsonRequestBehavior.AllowGet);
        }


        public ActionResult SearchAllDataOfLcRetirement(string LCRetirementID)
        {
            int lcRetirementId = Convert.ToInt32(LCRetirementID);
            var lcInfo = objRepository.LcmRetirementRpository.GetByID(lcRetirementId);
            LCMLCRetirement ob = new LCMLCRetirement();
            if (lcInfo != null)
            {
                ob.LCRetirementID = lcInfo.LCRetirementID;
                ob.LCRetirementNo = lcInfo.LCRetirementNo;
                ob.LessMargin = lcInfo.LessMargin;
                ob.LCRetirementDate = lcInfo.LCRetirementDate == null ? " " : Convert.ToDateTime(lcInfo.LCRetirementDate).ToString("dd/MM/yyyy");//temp.LCRetirementDate == null ? " " : Convert.ToDateTime(temp.LCRetirementDate).ToString("dd/MM/yyyy")
                ob.LCID = lcInfo.LCID;
                ob.LCNo = lcInfo.LCNo;
                var id = objRepository.CurrencyRepository.GetByID(lcInfo.LCRCurrency).CurrencyID;
                string currency = "";
                if (id != null)
                {
                    currency = objRepository.CurrencyRepository.GetByID(id).CurrencyName;
                }
                ob.CurrencyName = currency;//lcInfo.Sys_Currency.CurrencyName;
                ob.ExchangeRate = lcInfo.ExchangeRate;
                ob.ExchangeCurrency = lcInfo.ExchangeCurrency;
                ob.InterestAmount = lcInfo.InterestAmount;
                ob.InterestPersent = lcInfo.InterestPersent;
                ob.BillValue = lcInfo.BillValue;
                ob.Remarks = lcInfo.Remarks;
                ob.RecordStatus = lcInfo.RecordStatus;


                if (lcInfo.ExchangeValue == null)
                {
                    ob.ExchangeValue = 0;
                }
                else
                {
                    ob.ExchangeValue = (decimal)lcInfo.ExchangeValue;
                }

                if (lcInfo.LessMargin == null)
                {
                    ob.LessMargin = 0;
                }
                else
                {
                    ob.LessMargin = (decimal)lcInfo.LessMargin;
                }


                if (lcInfo.OtherCharge == null)
                {
                    ob.OtherCharge = 0;
                }
                else
                {
                    ob.OtherCharge = lcInfo.OtherCharge;
                }

                if (lcInfo.BankCommissionAmt == null)
                {
                    ob.BankCommissionAmt = 0;
                }
                else
                {
                    ob.BankCommissionAmt = lcInfo.BankCommissionAmt;
                }


                if (lcInfo.SwiftCharge == null)
                {
                    ob.SwiftCharge = 0;
                }
                else
                {
                    ob.SwiftCharge = lcInfo.SwiftCharge;
                }


                if (lcInfo.TotalAmount == null)
                {
                    ob.TotalAmount = 0;
                }
                else
                {
                    ob.TotalAmount = lcInfo.TotalAmount;
                }


                if (lcInfo.GrandTotal == null)
                {
                    ob.GrandTotal = 0;
                }
                else
                {
                    ob.GrandTotal = lcInfo.GrandTotal;
                }
            }
            return Json(ob, JsonRequestBehavior.AllowGet);
        }



        //******************************************END OF SEARCH CODE ************************************************//


        //******************************************DELETE CODE ************************************************//
        public ActionResult DeleteLCRetirementID(int lcRetirementID)
        {
            objValMssg = objDalLCRetirement.DeleteLCRetirementID(lcRetirementID);//DeletePackingList(cnfBillID);
            return Json(objValMssg, JsonRequestBehavior.AllowGet);
        }

        //******************************************END OF DELETE CODE ************************************************//


        //****************************************** CONFIRM RETIREMENT DATA *******************************************//

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

        //****************************************** END OF CONFIRM RETIREMENT DATA ************************************//

    }
}