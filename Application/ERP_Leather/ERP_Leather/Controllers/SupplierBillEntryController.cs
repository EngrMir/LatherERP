using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Pipes;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.AppSetupModel;
using ERP.BusinessLogicLayer.OperationManager;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using ERP_Leather.ActionFilters;
using Microsoft.Ajax.Utilities;

namespace ERP_Leather.Controllers
{
    public class SupplierBillEntryController : Controller
    {
        private readonly BLC_DEVEntities _context;
        private DalPrqPurchaseChallanItem _dalPrqPurchaseChallanItem;
        private DalPrqPurchaseChallan _dalPrqPurchaseChallan;
        private readonly DalPrqSupplierBill _dalPrqSupplierBill;
        private readonly DalPrqSupplierBillItem _dalPrqSupplierBillItem;
        private DalPrqSupplierBillChallan _dalPrqSupplierBillChallan;
        private readonly DalSysCurrency _dalSysCurrency;
        private DalSysUnit _dalSysUnit;
        private readonly DalSysSize _dalSysSize;
        private DalSysSupplier _dalSysSupplier;
        DalPrqPurchase _objPurchase;
        ValidationMsg _validationMsg;
        private int _userId;
        private readonly UnitOfWork _unit;

        public SupplierBillEntryController()
        {
            _validationMsg = new ValidationMsg();
            _dalPrqPurchaseChallan = new DalPrqPurchaseChallan();
            _dalPrqSupplierBill = new DalPrqSupplierBill();
            _dalPrqSupplierBillItem = new DalPrqSupplierBillItem();
            _dalPrqSupplierBillChallan = new DalPrqSupplierBillChallan();
            _dalSysCurrency = new DalSysCurrency();
            _dalSysUnit = new DalSysUnit();
            _dalSysSize = new DalSysSize();
            _dalSysSupplier = new DalSysSupplier();
            _dalPrqPurchaseChallanItem = new DalPrqPurchaseChallanItem();
            _objPurchase = new DalPrqPurchase();
            _unit = new UnitOfWork();
            _context = new BLC_DEVEntities();
        }

        [CheckUserAccess("SupplierBillEntry/SupplierBillEntry")]
        public ActionResult SupplierBillEntry()
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            ViewBag.CurrencyList = _unit.CurrencyRepository.Get().ToList();
            return View();
        }

        [CheckUserAccess("SupplierBillEntry/SupplierBillApproval")]
        public ActionResult SupplierBillApproval()
        {
            ViewBag.formTiltle = "Supplier Bill Approval";
            return View();
        }

        public JsonResult GetChallanList(string supplierId)
        {
            var challanList = _dalPrqSupplierBill.GetChallanListInfo(supplierId);
            var items = challanList;
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveSupplierBill(PrqSupplierBill billModel, List<PrqSupplierBillItem> billItemModel, List<PrqSupplierBillChallan> billChallanModel)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            _validationMsg = _dalPrqSupplierBill.SaveSupplierBill(billModel, billItemModel, billChallanModel, _userId, "SupplierBillEntry/SupplierBillEntry");
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCurrencyList()
        {
            var currency = _dalSysCurrency.GetAllActiveCurrency();
            var currencyInfos = currency.Select(curInf => new CurrencyInfos
            {
                CurrencyID = curInf.CurrencyID,
                CurrencyName = curInf.CurrencyName
            }).ToList();

            return new JsonResult { Data = currencyInfos };
        }

        public ActionResult ConfirmBill(long billId, string comment)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            _validationMsg = _dalPrqSupplierBill.ConfirmBill(billId, comment, _userId);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BillApproval(PrqSupplierBill billModel, IEnumerable<PrqSupplierBillItem> billItemModel)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            if (!string.IsNullOrEmpty(billModel.SupplierBillID.ToString(CultureInfo.InvariantCulture)))
            {
                _validationMsg = _dalPrqSupplierBill.ApproveBill(billModel, billItemModel, _userId);
            }
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSupplierBill(int supplierId)
        {
            //var supplierBills = _unit.SupplierBillRepository.Get().Where(ob => ob.SupplierID == supplierId && ob.IsDelete == false).ToList();
            //var result = supplierBills.Select(bill => new
            //{
            //    bill.SupplierBillID,
            //    bill.SupplierBillNo,
            //    bill.SupplierID,
            //    SupplierName = bill.SupplierID == 0 ? "" : _unit.SysSupplierRepository.GetByID(bill.SupplierID).SupplierName,
            //    BillDate = string.Format("{0:dd/MM/yyyy}", bill.BillDate),
            //    bill.PurchaseYear,
            //    RecordStatus = DalCommon.ReturnRecordStatus(bill.RecordStatus)
            //}).ToList();
            var result = _dalPrqSupplierBill.GetBills(supplierId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBills()
        {
            //var supplierBills = _unit.SupplierBillRepository.Get().Where(ob => ob.IsDelete == false).ToList();
            //var result = supplierBills.Select(bill => new
            //{
            //    bill.SupplierBillID,
            //    bill.SupplierBillNo,
            //    bill.SupplierID,
            //    SupplierName = bill.SupplierID == 0 ? "" : _unit.SysSupplierRepository.GetByID(bill.SupplierID).SupplierName,
            //    BillDate = string.Format("{0:dd/MM/yyyy}", bill.BillDate),
            //    bill.PurchaseYear,
            //    RecordStatus = DalCommon.ReturnRecordStatus(bill.RecordStatus)
            //}).ToList();
            //var result1 = result.OrderBy(ob => ob.SupplierName).ThenByDescending(ob => ob.SupplierBillID);
            var result1 = _dalPrqSupplierBill.GetBills(0);
            return Json(result1, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBillById(long billId)
        {
            var bill = new PrqSupplierBill();
            var ritems = new List<PrqSupplierBillItem>();
            var rchallans = new List<PrqSupplierBillChallan>();
            var supplierBill = _unit.SupplierBillRepository.GetByID(billId);
            if (supplierBill != null)
            {
                bill.SupplierBillID = supplierBill.SupplierBillID;
                bill.SupplierBillNo = supplierBill.SupplierBillNo;
                bill.SupplierBillRefNo = supplierBill.SupplierBillRefNo;
                bill.SupplierID = supplierBill.SupplierID;
                bill.SupplierCode = _unit.SysSupplierRepository.GetByID(supplierBill.SupplierID).SupplierCode;
                bill.SupplierName = _unit.SysSupplierRepository.GetByID(supplierBill.SupplierID).SupplierName;
                bill.SupplierAddressID = supplierBill.SupplierAddressID;
                bill.SupplierAddress = _unit.SupplierAddressRepository.GetByID(supplierBill.SupplierAddressID).Address;
                bill.PurchaseID = supplierBill.PurchaseID;
                bill.PurchaseNo = supplierBill.PurchaseID == null
                    ? ""
                    : _unit.PrqPurchaseRepository.GetByID(supplierBill.PurchaseID).PurchaseNo;
                bill.BillCategory = supplierBill.BillCategory;
                bill.BillType = supplierBill.BillType;
                bill.BillDate = string.Format("{0:dd/MM/yyyy}", supplierBill.BillDate);
                bill.PurchaseYear = supplierBill.PurchaseYear;
                bill.Remarks = supplierBill.Remarks;
                bill.TotalQty = supplierBill.TotalQty;
                bill.TotalRejectQty = supplierBill.TotalRejectQty;
                bill.AvgPrice = supplierBill.ApprovedPrice;
                bill.ApprovedPrice = supplierBill.ApprovedPrice;
                bill.OtherCost = supplierBill.OtherCost;
                bill.DiscountAmt = supplierBill.DiscountAmt;
                bill.DiscountPercent = supplierBill.DiscountPercent;
                bill.ApprovedAmt = supplierBill.ApprovedAmt;
                bill.TotalAmt = supplierBill.TotalAmt;
                bill.PayableAmt = supplierBill.PayableAmt;
                bill.CheckComment = supplierBill.CheckComment;
                bill.ApproveComment = supplierBill.ApproveComment;
                bill.RecordStatus = supplierBill.RecordStatus;
                var challans =
                _unit.SupplierBillChallanRepository.Get()
                    .Where(ob => ob.SupplierBillID == supplierBill.SupplierBillID && ob.IsDelete == false)
                    .ToList();
                if (challans.Count > 0)
                {
                    foreach (var c in challans)
                    {
                        var rchallan = new PrqSupplierBillChallan();
                        rchallan.ChallanID = c.ChallanID;
                        rchallan.ChallanNo = c.S_ChallanRef;
                        rchallan.ChallanDate = string.Format("{0:dd/MM/yyyy}",
                            _unit.PrqPurchaseChallanRepo.GetByID(c.ChallanID).ChallanDate);
                        rchallan.Quantity =
                            _unit.PurchaseChallanItemRepository.Get()
                                .Where(ob => ob.ChallanID == c.ChallanID)
                                .Sum(ob => ob.ChallanQty);
                        rchallans.Add(rchallan);
                    }
                }
                var items =
                    _unit.SupplierBillItemRepository.Get()
                        .Where(ob => ob.SupplierBillID == supplierBill.SupplierBillID && ob.IsDelete == false)
                        .ToList();
                if (items.Count > 0)
                {
                    foreach (var item in items)
                    {
                        var ritem = new PrqSupplierBillItem();
                        ritem.BillItemID = item.BillItemID;
                        ritem.ItemTypeID = item.ItemType;
                        ritem.ItemTypeName = _unit.SysItemTypeRepository.GetByID(item.ItemType).ItemTypeName;
                        ritem.ItemSizeID = item.ItemSize;
                        ritem.ItemSizeName = _unit.SysSizeRepository.GetByID(item.ItemSize).SizeName;
                        ritem.ItemQty = item.ItemQty;
                        ritem.RejectQty = item.RejectQty;
                        ritem.ItemRate = item.ItemRate;
                        ritem.ApproveRate = item.ApproveRate;
                        ritem.Amount = item.Amount;
                        ritem.AmtUnit = item.AmtUnit ?? 0;
                        ritem.AvgArea = item.AvgArea;
                        ritem.AreaUnit = item.AreaUnit;
                        ritem.AreaUnitName = item.AreaUnit == null ? "" : _unit.SysUnitRepository.GetByID(item.AreaUnit).UnitName;
                        ritem.Remarks = item.Remarks;
                        ritems.Add(ritem);
                    }

                }

            }
            return Json(new { bill, rchallans, ritems }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllSize()
        {
            var size = _dalSysSize.GetAllActiveItemSize();
            var sizes = size.Select(svm => new SizeVM
            {
                SizeID = svm.SizeID,
                SizeName = svm.SizeName
            }).ToList();
            return Json(sizes, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetItemListForChallan(string ChallanID)
        {
            var itemList = _dalPrqSupplierBillItem.GetItemListForChallan(ChallanID);
            return Json(itemList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetChallans(long purchaseId)
        {
            var challans = _unit.PurchaseChallanRepository.Get().Where(ob => ob.PurchaseID == purchaseId).ToList();
            var result = challans.Select(challan => new
            {
                challan.ChallanID,
                challan.ChallanNo,
                ChallanDate = string.Format("{0:dd/MM/yyyy}", challan.ChallanDate),
                Quantity = _unit.PurchaseChallanItemRepository.Get().Where(ob => ob.ChallanID == challan.ChallanID).Sum(ob => ob.ChallanQty),
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetChallanById(long challanId)
        {
            var challan = _unit.PurchaseChallanRepository.GetByID(challanId);
            var result = new
            {
                challan.ChallanID,
                challan.ChallanNo,
                ChallanDate = string.Format("{0:dd/MM/yyyy}", challan.ChallanDate),
                Quantity = _unit.PurchaseChallanItemRepository.Get().Where(ob => ob.ChallanID == challan.ChallanID).Sum(ob => ob.ChallanQty),
                Items = _unit.PurchaseChallanItemRepository.Get().Where(ob => ob.ChallanID == challan.ChallanID).Select(item => new
                {
                    item.ItemTypeID,
                    ItemTypeName = item.ItemTypeID == 0 ? "" : _unit.SysItemTypeRepository.GetByID(item.ItemTypeID).ItemTypeName,
                    item.ItemSizeID,
                    ItemSizeName = item.ItemSizeID == 0 ? "" : _unit.SysSizeRepository.GetByID(item.ItemSizeID).SizeName,
                    ItemQty = item.ChallanQty,
                }).ToList()
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]


        public ActionResult Delete(long id, long billId, string del)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            if (del == "all")
            {
                _validationMsg = _dalPrqSupplierBill.Delete(billId, _userId);
            }
            if (del == "challan")
            {
                _validationMsg = _dalPrqSupplierBill.DeleteChallan(id, billId);
            }
            if (del == "item")
            {
                _validationMsg = _dalPrqSupplierBill.DeleteItem(id, _userId);
            }
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPurchase(int supplierId)
        {
            var purc = _dalPrqSupplierBill.GetPurchaseListBySupplier(supplierId);
            return Json(purc, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPurchaseById(long purchaseId)
        {
            var purchase = _unit.PrqPurchaseRepository.GetByID(purchaseId);
            var result = new
            {
                purchase.PurchaseID,
                purchase.PurchaseNo,
                purchase.PurchaseType,
                purchase.PurchaseYear,
                Challans = _unit.PrqPurchaseChallanRepo.Get().Where(ob => ob.PurchaseID == purchase.PurchaseID).Select(
                challan => new
                {
                    challan.ChallanID,
                    challan.ChallanNo,
                    ChallanDate = string.Format("{0:dd/MM/yyyy}", challan.ChallanDate),
                    Quantity = _unit.PurchaseChallanItemRepository.Get().Where(ob => ob.ChallanID == challan.ChallanID).Sum(ob => ob.ChallanQty),
                    Items = _unit.PurchaseChallanItemRepository.Get().Where(ob => ob.ChallanID == challan.ChallanID).Select(item => new
                    {
                        item.ItemTypeID,
                        ItemTypeName = item.ItemTypeID == 0 ? "" : _unit.SysItemTypeRepository.GetByID(item.ItemTypeID).ItemTypeName,
                        item.ItemSizeID,
                        ItemSizeName = item.ItemSizeID == 0 ? "" : _unit.SysSizeRepository.GetByID(item.ItemSizeID).SizeName,
                        ItemQty = item.ChallanQty,
                    }).ToList()
                }).ToList()
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSuppliers()
        {
            var dummy = (from s in _context.Sys_Supplier
                         where s.IsActive.Equals(true) && s.IsDelete.Equals(false)
                         join sa in _context.Sys_SupplierAddress.Where(q => q.IsActive.Equals(true) && q.IsDelete.Equals(false))
                         on s.SupplierID equals sa.SupplierID into temp
                         from item in temp.DefaultIfEmpty()
                         select new SupplierDetails
                         {
                             SupplierID = s.SupplierID,
                             SupplierCode = s.SupplierCode,
                             SupplierName = s.SupplierName.ToUpper(),
                             SupplierAddressID = (item.SupplierAddressID).ToString(),
                             Address = item.Address,
                             ContactNumber = item.ContactNumber,
                             ContactPerson = item.ContactPerson
                         }).ToList();

            var result = dummy.OrderBy(ob => ob.SupplierCode);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSuppliersForSearch()
        {
            var suppliers =
                _unit.SysSupplierRepository.Get()
                    .Where(ob => ob.IsDelete == false && ob.IsActive == true)
                    .Select(ob => ob.SupplierName)
                    .ToList();
            return Json(suppliers, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSupplierByName(string name)
        {
            var supplier = (from s in _context.Sys_Supplier
                            where s.IsActive.Equals(true) && s.IsDelete.Equals(false) && s.SupplierName.Equals(name)
                            join sa in _context.Sys_SupplierAddress.Where(q => q.IsActive.Equals(true) && q.IsDelete.Equals(false))
                                on s.SupplierID equals sa.SupplierID into temp
                            from item in temp.DefaultIfEmpty()
                            select new SupplierDetails
                            {
                                SupplierID = s.SupplierID,
                                SupplierCode = s.SupplierCode,
                                SupplierName = s.SupplierName.ToUpper(),
                                SupplierAddressID = (item.SupplierAddressID).ToString(),
                                Address = item.Address,
                                ContactNumber = item.ContactNumber,
                                ContactPerson = item.ContactPerson
                            });
            return Json(supplier, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetItemsByBillId(long billId)
        {
            var challans = _unit.SupplierBillChallanRepository.Get().Where(ob => ob.SupplierBillID == billId).ToList();
            var items = _unit.SupplierBillItemRepository.Get().Where(ob => ob.SupplierBillID == billId).ToList();

            var rChallans = challans.Select(challan => new
            {
                challan.ChallanID,
                ChallanNo = challan.S_ChallanRef,
                ChallanDate = string.Format("{0:dd/MM/yyyy}",
                    _unit.PrqPurchaseChallanRepo.GetByID(challan.ChallanID).ChallanDate),
                Quantity =
                    _unit.PurchaseChallanItemRepository.Get()
                        .Where(ob => ob.ChallanID == challan.ChallanID)
                        .Sum(ob => ob.ChallanQty)
            }).ToList();

            var rItems = items.Select(item => new
            {
                item.BillItemID,
                ItemTypeID = item.ItemType, _unit.SysItemTypeRepository.GetByID(item.ItemType).ItemTypeName,
                ItemSizeID = item.ItemSize,
                ItemSizeName = _unit.SysSizeRepository.GetByID(item.ItemSize).SizeName, item.ItemQty, item.RejectQty, item.ItemRate,
                item.ApproveRate, item.Amount,
                AmtUnit = item.AmtUnit ?? 0, item.AvgArea, item.AreaUnit,
                AreaUnitName = item.AreaUnit == null ? "" : _unit.SysUnitRepository.GetByID(item.AreaUnit).UnitName, item.Remarks,
            }).ToList();
            return Json(new {rChallans, rItems}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBillListForSearch()
        {
            var bills =
                _unit.SupplierBillRepository.Get()
                    .Where(ob => ob.IsDelete == false)
                    .Select(ob => ob.SupplierBillNo)
                    .ToList();
            return Json(bills, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBillForSearch(string billNo)
        {
            var bills = _context.Prq_SupplierBill.Where(m => m.IsDelete == false && m.SupplierBillNo.StartsWith(billNo)).ToList();
            var result = bills.Select(bill => new
            {
                bill.SupplierBillID,
                bill.SupplierBillNo,
                bill.SupplierID,
                _unit.SysSupplierRepository.GetByID(bill.SupplierID).SupplierName,
                BillDate = string.Format("{0:dd/mm/yyyy}",bill.BillDate),
                bill.PurchaseYear,
                RecordStatus = DalCommon.ReturnRecordStatus(bill.RecordStatus)
            }).ToList();

            return Json(result.OrderBy(ob => ob.SupplierName), JsonRequestBehavior.AllowGet);
        }
    }

    public class CurrencyInfos
    {
        public int CurrencyID { get; set; }
        public string CurrencyName { get; set; }
    }

    
}