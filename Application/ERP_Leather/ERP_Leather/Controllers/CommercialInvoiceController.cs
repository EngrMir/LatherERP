using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using ERP_Leather.ActionFilters;
using Microsoft.Ajax.Utilities;
using Microsoft.Scripting.Utils;

namespace ERP_Leather.Controllers
{
    public class CommercialInvoiceController : Controller
    {
        private int _userId;
        DalSysCurrency _objCurrency;
        UnitOfWork _unit;
        ValidationMsg _validationMsg;
        private DalCommercialInvoice _dalCommercialInvoice;
        public CommercialInvoiceController()
        {
            _objCurrency = new DalSysCurrency();
            _unit = new UnitOfWork();
            _validationMsg = new ValidationMsg();
            _dalCommercialInvoice = new DalCommercialInvoice();
        }

        [CheckUserAccess("CommercialInvoice/CommercialInvoice")]
        public ActionResult CommercialInvoice()
        {

            ViewBag.CurrencyList = _objCurrency.GetAllActiveCurrency();
            return View();
        }

        public ActionResult GetLcBasicInfo()
        {
            var lcBasic = _unit.LCOpeningRepository.Get().Where(ob => /*ob.RecordStatus == "CNF" &&*/ ob.LCStatus == "RNG")
                    .ToList();

            var lstLcBasicsInfo = lcBasic.Select(item => new LcBasics
            {
                LCID = item.LCID,
                LCNo = item.LCNo,
                RecordStatus = DalCommon.ReturnRecordStatus(item.RecordStatus),
                LCDate = item.LCDate.ToString("dd-MMM-yyyy")
            }).ToList();
            return Json(lstLcBasicsInfo.OrderByDescending(ob => ob.LCID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLcInfoForSearch()
        {
            var lc = _unit.LCOpeningRepository.Get().Where(ob => /*ob.RecordStatus == "CNF" &&*/ ob.LCStatus == "RNG").Select(ob => ob.LCNo).ToList();
            return Json(lc, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLcInfoByLcNo(string lcNo)
        {
            var lc = _unit.LCOpeningRepository.Get().Where(ob => ob.LCNo == lcNo).ToList();
            var result = lc.Select(item => new LcBasics
            {
                LCID = item.LCID,
                LCNo = item.LCNo,
                LCDate = item.LCDate.ToString("dd-MMM-yyyy"),
                PINo = item.PINo
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllLCforCommercialInvoice(int lcId)
        {
            var lc = _unit.LCOpeningRepository.GetByID(lcId);
            var lcFull = new CommercialInvoiceVM
            {
                LCID = lc.LCID,
                LCNo = lc.LCNo,
                LCDate = lc.LCDate.ToString("dd-MMM-yyyy"),
                PIID = lc.PIID,
                PINo = lc.PINo,
                OrderNo = _unit.PRQ_ChemicalPIRepository.GetByID(lc.PIID).OrderNo,
                ExchangeCurrency = lc.ExchangeCurrency,
                ExchangeRate = lc.ExchangeRate,
                ExchangeValue = lc.ExchangeValue,
                Items = new List<CommercialInvoiceItemVM>()
            };
            
            var chemicalPiItems = _unit.PrqChemicalPiItemRepository.Get().Where(ob => ob.PIID == lc.PIID).ToList();
            if (chemicalPiItems.Count != 0)
            {
                foreach (var piItem in chemicalPiItems)
                {
                    var item = new CommercialInvoiceItemVM
                    {
                        ItemID = piItem.ItemID,
                        ItemName = _unit.SysChemicalItemRepository.GetByID(piItem.ItemID).ItemName,
                        PackSize = piItem.PackSize,
                        PackSizeName = piItem.PackSize == null ? "" : _unit.SysSizeRepository.GetByID(piItem.PackSize).SizeName,
                        SizeUnit = piItem.SizeUnit,
                        PackQty = piItem.PackQty,
                        CIQty = piItem.PIQty,
                        CIUnit = piItem.PIUnit,
                        CIUnitName = _unit.SysUnitRepository.GetByID(piItem.PIUnit).UnitName,
                        CIUnitPrice = Convert.ToDecimal(piItem.PIUnitPrice),
                        CITotalPrice = piItem.PITotalPrice,
                        SupplierID = piItem.SupplierID,
                        ManufacturerID = piItem.ManufacturerID
                    };
                    if (piItem.SizeUnit != null)
                    {
                        item.SizeUnitName = _unit.SysUnitRepository.GetByID(piItem.SizeUnit).UnitName;
                    }
                    if (piItem.SupplierID != null)
                    {
                        item.SupplierName = _unit.SysSupplierRepository.GetByID(piItem.SupplierID).SupplierName;
                    }
                    if (piItem.ManufacturerID != null)
                    {
                        item.ManufacturerName = _unit.SysSupplierRepository.GetByID(piItem.SupplierID).SupplierName;
                    }
                    lcFull.Items.Add(item);
                }
            }
            return Json(lcFull, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllInvoiceMin()
        {
            var commercialInvoice = _unit.CommercialInvoiceRepository.Get().ToList();

            var ciList = commercialInvoice.Select(item => new CIBacis
            {
                CIID = item.CIID,
                CINo = item.CINo,
                CIDate = string.Format("{0:dd/MM/yyyy}", item.CIDate),
                LCID = item.LCID,
                LCNo = item.LCNo,
                LCDate = _unit.LCOpeningRepository.GetByID(item.LCID).LCDate.ToString("dd/MM/yyyy"),
                SupplierName = _unit.SysSupplierRepository.GetByID(_unit.PRQ_ChemicalPIRepository.GetByID(_unit.LCOpeningRepository.GetByID(item.LCID).PIID).SupplierID).SupplierName,
                RecordStatus = DalCommon.ReturnRecordStatus(item.RecordStatus)
            }).OrderByDescending(ob=>ob.CIID).ToList();

            return Json(ciList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetInvoiceMinForSearch()
        {
            var commercialInvoice = _unit.CommercialInvoiceRepository.Get().Select( ob => ob.CINo).ToList();
            return Json(commercialInvoice, JsonRequestBehavior.AllowGet);
        }

        public ActionResult  GetInvoiceByCiNo(string ciNo)
        {
            var ci = _unit.CommercialInvoiceRepository.Get().Where(ob => ob.CINo == ciNo || (ob.CINo.StartsWith(ciNo))).ToList();
            var result = ci.Select(item => new CIBacis
            {
                CIID = item.CIID,
                CINo = item.CINo,
                CIDate = string.Format("{0:dd/MM/yyyy}", item.CIDate),
                LCID = item.LCID,
                LCNo = item.LCNo,
                LCDate = _unit.LCOpeningRepository.GetByID(item.LCID).LCDate.ToString("dd/MM/yyyy"),
                SupplierName = _unit.SysSupplierRepository.GetByID(_unit.PRQ_ChemicalPIRepository.GetByID(_unit.LCOpeningRepository.GetByID(item.LCID).PIID).SupplierID).SupplierName
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchCIByNo(string ciNo)
        {
            var result = from temp in _unit.CommercialInvoiceRepository.Get()
                         where (temp.CINo.StartsWith(ciNo) || temp.CINo == ciNo)
                         select new
                         {
                             CIID = temp.CIID,
                             CINo = temp.CINo,
                             CIDate = string.Format("{0:dd/MM/yyyy}", temp.CIDate),
                             LCID = temp.LCID,
                             LCNo = temp.LCNo,
                             LCDate = _unit.LCOpeningRepository.GetByID(temp.LCID).LCDate.ToString("dd/MM/yyyy")
                         };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCINo()
        {
            var sysCI = from temp in _unit.CommercialInvoiceRepository.Get().OrderByDescending(s => s.CIID)
                        select temp.CINo;
            return Json(sysCI, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllInvoice(int CIID)
        {
            var commercialInvoice = _unit.CommercialInvoiceRepository.GetByID(CIID);
            var CI = new CommercialInvoiceVM();
            CI.CIID = commercialInvoice.CIID;
            CI.CINo = commercialInvoice.CINo;            
            CI.CIDate = string.Format("{0:dd/MM/yyyy}", commercialInvoice.CIDate);
            CI.CIStatus = commercialInvoice.CIStatus;
            CI.CICurrency = commercialInvoice.CICurrency;
            CI.LCID = commercialInvoice.LCID;
            CI.LCNo = commercialInvoice.LCNo;
            CI.PIID = _unit.LCOpeningRepository.GetByID(commercialInvoice.LCID).PIID;
            CI.PINo = _unit.LCOpeningRepository.GetByID(commercialInvoice.LCID).PINo;            
            CI.OrderNo = _unit.PRQ_ChemicalPIRepository.GetByID(CI.PIID).OrderNo;
            CI.ExchangeCurrency = commercialInvoice.ExchangeCurrency;
            CI.ExchangeRate = commercialInvoice.ExchangeRate;
            CI.ExchangeValue = commercialInvoice.ExchangeValue;
            CI.CINote = commercialInvoice.CINote;
            CI.RecordStatus = commercialInvoice.RecordStatus;
            CI.ApprovalAdvice = commercialInvoice.ApprovalAdvice;
            CI.Items = new List<CommercialInvoiceItemVM>();
            var CiItems = _unit.CommercialInvoiceItemRepository.Get().Where(c => c.CIID == commercialInvoice.CIID).ToList();
            if (CiItems.Count > 0)
            {
                foreach (var cItem in CiItems)
                {
                    var ciItem = new CommercialInvoiceItemVM();

                    ciItem.CIItemID = cItem.CIItemID;
                    ciItem.ItemID = cItem.ItemID;
                    ciItem.ItemName = _unit.SysChemicalItemRepository.GetByID(cItem.ItemID).ItemName;
                    ciItem.PackSize = cItem.PackSize;
                    ciItem.PackSizeName = _unit.SysSizeRepository.GetByID(cItem.PackSize).SizeName;
                    ciItem.SizeUnit = cItem.SizeUnit;
                    ciItem.SizeUnitName = _unit.SysUnitRepository.GetByID(cItem.SizeUnit).UnitName;
                    ciItem.PackQty = cItem.PackQty;
                    ciItem.CIQty = cItem.CIQty;
                    ciItem.CIUnit = cItem.CIUnit;
                    ciItem.CIUnitName = _unit.SysUnitRepository.GetByID(cItem.CIUnit).UnitName;
                    ciItem.CIUnitPrice = cItem.CIUnitPrice;
                    ciItem.CITotalPrice = cItem.CITotalPrice;
                    ciItem.SupplierID = cItem.SupplierID;
                    ciItem.ManufacturerID = cItem.ManufacturerID;
                    if (cItem.ManufacturerID != null)
                    {
                        ciItem.ManufacturerName = _unit.SysSupplierRepository.GetByID(cItem.ManufacturerID).SupplierName;    
                    }
                    

                    CI.Items.Add(ciItem);
                }
            }

            return Json(CI, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllChemicalItemForSearch()
        {
            var chemItems =
                _unit.SysChemicalItemRepository.Get().Where(ob => ob.IsActive != false).Select(ob => ob.ItemName);
            return Json(chemItems, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllChemicalItemByName(string name)
        {
            var chemItems =
                _unit.SysChemicalItemRepository.Get().Where(ob => ob.ItemName == name && ob.IsActive != false).ToList();
            var result = chemItems.Select(item => new Sys_ChemicalItem
            {
                ItemID = item.ItemID, ItemName = item.ItemName, HSCode = item.HSCode
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckCiNo(string cino)
        {
            var ci = _unit.CommercialInvoiceRepository.Get().Where(ob => ob.CINo == cino);
            return Json(ci, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteCommercialInvoiceItem(int CIItemID)
        {
            var delete = _dalCommercialInvoice.DeleteCommercialInvoiceItem(CIItemID);
            if (delete == 1)
            {
                _validationMsg.Type = Enums.MessageType.Delete;
                _validationMsg.Msg = "Item has been successfully removed";
            }
            else
            {
                _validationMsg.Type = Enums.MessageType.Error;
                _validationMsg.Msg = "Failed to delete";
            }
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteCommercialInvoice(int ciid)
        {
            _validationMsg = _dalCommercialInvoice.DeleteCommercialInvoice(ciid);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(CommercialInvoiceVM model)
        {
            _userId = Convert.ToInt32(Session["UserID"]);

            _validationMsg = _dalCommercialInvoice.Save(model, _userId, "CommercialInvoice/CommercialInvoice");

            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Check(int ciid, string chkNote)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            _validationMsg = _dalCommercialInvoice.Check(ciid, _userId, chkNote);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Confirm(int ciid, string apvNote)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            _validationMsg = _dalCommercialInvoice.Confirm(ciid, _userId, apvNote);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        //public ValidationMsg ModelCheckForSave(CommercialInvoiceVM model)
        //{
        //    if (model.LCNo == null)
        //    {
        //        _validationMsg.Type = Enums.MessageType.Error;
        //        _validationMsg.Msg = "Please input LC No. to continue saving.";
        //        return _validationMsg;
        //    }
        //    foreach (var item in model.Items)
        //    {
        //        if (item.ItemID == null)
        //        {
        //            _validationMsg.Type = Enums.MessageType.Error;
        //            _validationMsg.Msg = "Please input "
        //        }
        //    }
        //}
    }

    public class LcBasics
    {
        public int LCID { get; set; }
        public string LCNo { get; set; }
        public string LCDate { get; set; }
        public string PINo { get; set; }
        public string RecordStatus { get; set; }
    }

    public class CIBacis
    {
        public int CIID { get; set; }
        public string CINo { get; set; }
        public int? LCID { get; set; }
        public string LCDate { get; set; }
        public string LCNo { get; set; }
        public string CIDate { get; set; }
        public string SupplierName { get; set; }
        public string RecordStatus { get; set; }
    }
}