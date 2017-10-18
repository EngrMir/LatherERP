using System;
using System.Collections.Generic;
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

namespace ERP_Leather.Controllers
{
    public class ChemLocalPurcBillEntController : Controller
    {
        private int _userId;
        private readonly UnitOfWork _unit;
        private readonly DalSysCurrency _objCurrency;
        private ValidationMsg _validationMsg;
        private readonly DalPrqChemLocalPurcBillEnt _chemLocalPurcBillEnt;
        private readonly BLC_DEVEntities _context;
        public ChemLocalPurcBillEntController()
        {
            _chemLocalPurcBillEnt = new DalPrqChemLocalPurcBillEnt();
            _objCurrency = new DalSysCurrency();
            _unit = new UnitOfWork();
            _validationMsg = new ValidationMsg();
            _context = new BLC_DEVEntities();
        }

        [CheckUserAccess("ChemLocalPurcBillEnt/ChemLocalPurcBillEnt")]
        public ActionResult ChemLocalPurcBillEnt()
        {
            ViewBag.CurrencyList = _objCurrency.GetAllActiveCurrency();
            return View();
        }

        public ActionResult GetLocalPurchaseOrder()
        {
            var lpo = _unit.ChemicalPurchaseOrderRepository.Get().Where(ob => ob.OrderCategory == "LPO").ToList();
            var result = lpo.Select(order => new OrderMin
            {
                OrderID = order.OrderID,
                OrderNo = order.OrderNo,
                OrderDate = string.Format("{0:dd/MM/yyyy}", order.OrderDate),
                OrderType = order.OrderType
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLocalPurchaseReceive(int supplierId)
        {
            var rpo = _unit.ChemLocalPurcRecvPoRepository.Get().ToList();
            var ords =
                _context.PRQ_ChemFrgnPurcOrdr.Where(ob => ob.SupplierID == supplierId && ob.OrderCategory == "LPO").ToList();
            var pos = new List<PO>();
            if (ords.Count > 0)
            {
                foreach (var ord in ords)
                {
                    var qry = _context.PRQ_ChemLocalPurcRecvPO.Where(ob => ob.OrderID == ord.OrderID).ToList();
                    if (qry.Count > 0)
                    {
                        foreach (var qy in qry)
                        {
                            var po = new PO();
                            po.POReceiveID = qy.POReceiveID;
                            po.OrderID = qy.OrderID;
                            po.OrderNo = qy.OrderNo;
                            po.OrderDate = qy.OrderID == null
                                ? ""
                                : string.Format("{0:dd/MM/yyyy}",
                                    _unit.ChemicalPurchaseOrderRepository.GetByID(po.OrderID).OrderDate);
                            po.ReceiveID = qy.ReceiveID;
                            po.ReceiveNo = qy.ReceiveID == null
                                ? ""
                                : _unit.ChemicalLocalPurchaseReceiveRepository.GetByID(po.ReceiveID).ReceiveNo;
                            po.ReceiveDate = qy.ReceiveID == null
                                ? ""
                                : string.Format("{0:dd/MM/yyyy}",
                                    _unit.ChemicalLocalPurchaseReceiveRepository.GetByID(po.ReceiveID).ReceiveDate);
                            po.ReceiveType = qy.ReceiveID == null
                                ? ""
                                : _unit.ChemicalLocalPurchaseReceiveRepository.GetByID(po.ReceiveID).ReceiveType;
                            pos.Add(po);
                        }    
                    }
                    
                }    
            }
            
            //var result = rpo.Select(po => new
            //{
            //    po.POReceiveID,
            //    po.OrderID,
            //    po.OrderNo,
            //    OrderDate = po.OrderID == null ? "" : string.Format("{0:dd/MM/yyyy}",_unit.ChemicalPurchaseOrderRepository.GetByID(po.OrderID).OrderDate),
            //    po.ReceiveID,
            //    _unit.ChemicalLocalPurchaseReceiveRepository.GetByID(po.ReceiveID).ReceiveNo,
            //    ReceiveDate =  string.Format("{0:dd/MM/yyyy}", _unit.ChemicalLocalPurchaseReceiveRepository.GetByID(po.ReceiveID).ReceiveDate),
            //    _unit.ChemicalLocalPurchaseReceiveRepository.GetByID(po.ReceiveID).ReceiveType
            //    //ReceiveID = receive.ReceiveID, ReceiveNo = receive.ReceiveNo, ReceiveDate = string.Format("{0:dd/MM/yyyy}", receive.ReceiveDate), 
            //    //ReceiveType = receive.ReceiveType
            //});
            return Json(pos.OrderByDescending(ob => ob.POReceiveID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPurchaseReceiveItems(long receiveId, int orderId)
        {
            var rItems = _unit.ChemicalLocalPurchaseReceiveItemRepository.Get()
                .Where(ob => ob.ReceiveID == receiveId).Select(item => new PrqChemLocalPurcBillItem
                {
                    ItemId = item.ItemID, 
                    SupplierId = item.SupplierID, 
                    PackSize = item.PackSize, 
                    PackSizeName = item.PackSize == null ? "" : _unit.SysSizeRepository.GetByID(item.PackSize).SizeName,
                    SizeUnit = item.SizeUnit, 
                    SizeUnitName = item.SizeUnit == null ? "" : _unit.SysUnitRepository.GetByID(item.SizeUnit).UnitName,
                    PackQty = item.PackQty,
                    ReceiveQty = item.ReceiveQty, 
                    UnitId = item.UnitID, 
                    ReceiveID = item.ReceiveID, 
                    OrderID = orderId,
                    ItemName = item.ItemID == null || item.ItemID == 0 ? "" : _unit.SysChemicalItemRepository.GetByID(item.ItemID).ItemName,
                    ManufacturerId = item.ManufacturerID == null || item.ManufacturerID == 0 ? null : item.ManufacturerID,
                    //PackSizeName = item.PackSize == null || item.PackSize == 0 ? "" : _unit.SysSizeRepository.GetByID(item.PackSize).SizeName,
                    UnitName = item.ItemID == null || item.ItemID == 0 ? "" : _unit.SysUnitRepository.GetByID(item.UnitID).UnitName,
                    ManufacturerName = item.ManufacturerID == null || item.ManufacturerID == 0 ? "" : _unit.SysSupplierRepository.GetByID(item.ManufacturerID).SupplierName
                }).ToList();
            return Json(rItems, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBills()
        {
            var bills = _unit.ChemicalLocalPurchaseBillRepository.Get()
                .Select(bill => new LocalPurchaseBillMin
                {
                    BillID = bill.BillID,
                    BillNo = bill.BillNo,
                    BillDate = string.Format("{0:dd/MM/yyyy}", bill.BillDate),
                    SupplierName = bill.SupplierID == null ?
                    "" : _unit.SysSupplierRepository.GetByID(bill.SupplierID).SupplierName,
                    SupplierBillNo = bill.SupplierBillNo, 
                    RecordStatus = DalCommon.ReturnRecordStatus(bill.RecordStatus)
                }).ToList();
            return Json(bills.OrderByDescending(ob => ob.BillID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBillById(long billId)
        {
            var bill = _unit.ChemicalLocalPurchaseBillRepository.GetByID(billId);
            var result = new PrqChemLocalPurcBill
            {
                BillId = bill.BillID, BillNo = bill.BillNo, BillDate = string.Format("{0:dd/MM/yyyy}", bill.BillDate), PurchaseYear = bill.PurchaseYear,
                SupplierId = bill.SupplierID, SupplierName = _unit.SysSupplierRepository.GetByID(bill.SupplierID).SupplierName, SupplierAddressId = bill.SupplierAddressID,
                SupplierAddress = _unit.SupplierAddressRepository.GetByID(bill.SupplierAddressID).Address, SupplierBillNo = bill.SupplierBillNo,
                SupBillDate = string.Format("{0:dd/MM/yyyy}", bill.SupBillDate), BillAmt = bill.BillAmt, VatAmt = bill.VatAmt,DiscountAmt = bill.DiscountAmt,
                DiscountPercent = bill.DiscountPercent, PayableAmt = bill.PayableAmt, ExchangCurrency = bill.ExchangCurrency, ExchangRate = bill.ExchangRate,
                Currency = bill.Currency, ExchangValue = bill.ExchangValue, CheckComments = bill.CheckComments, ApprovalComments = bill.ApprovalComments,
                RecordStatus = bill.RecordStatus, References = _unit.ChemicalLocalPurchaseBillReferenceRepository.Get().Where(ob => ob.BillID == bill.BillID)
                .Select(reff => new PrqChemLocalPurcBillRef
                {
                    BillRefId = reff.BillRefID, BillId = reff.BillID, OrderID = reff.OrderID, ReceiveID = reff.ReceiveID,
                    OrderNo = _unit.ChemicalPurchaseOrderRepository.GetByID(reff.OrderID).OrderNo,
                    OrderDate = string.Format("{0:dd/MM/yyyy}", _unit.ChemicalPurchaseOrderRepository.GetByID(reff.OrderID).OrderDate),
                    ReceiveNo = _unit.ChemicalLocalPurchaseReceiveRepository.GetByID(reff.ReceiveID).ReceiveNo,
                    ReceiveDate = string.Format("{0:dd/MM/yyyy}", _unit.ChemicalLocalPurchaseReceiveRepository.GetByID(reff.ReceiveID).ReceiveDate),
                    Items = _unit.ChemicalLocalPurchaseBillItemRepository.Get().Where(ob => ob.BillRefID == reff.BillRefID)
                    .Select(item => new PrqChemLocalPurcBillItem
                    {
                        BillItemId = item.BillItemID, BillRefId = item.BillRefID, ItemId = item.ItemID, SupplierId = item.SupplierID, PackSize = item.PackSize,
                        SizeUnit = item.SizeUnit, PackQty = item.PackQty, ReceiveQty = item.ReceiveQty, UnitId = item.UnitID, UnitPrice = item.UnitPrice,
                        TotalPrice = item.TotalPrice, ManufacturerId = item.ManufacturerID,
                        ItemName = item.ItemID == null ? "" : _unit.SysChemicalItemRepository.GetByID(item.ItemID).ItemName,
                        PackSizeName = item.PackSize == null ? "" : _unit.SysSizeRepository.GetByID(item.PackSize).SizeName,
                        SizeUnitName = item.SizeUnit == null ? "" : _unit.SysUnitRepository.GetByID(item.SizeUnit).UnitName,
                        UnitName = item.UnitID == null ? "" : _unit.SysUnitRepository.GetByID(item.UnitID).UnitName,
                        ManufacturerName = item.ManufacturerID == null ? "" : _unit.SysSupplierRepository.GetByID(item.ManufacturerID).SupplierName
                    }).ToList()
                }).ToList(),
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(PrqChemLocalPurcBill model)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            const string url = "ChemLocalPurcBillEnt/ChemLocalPurcBillEnt";
            //model.OtherCost = 0;
            _validationMsg = _chemLocalPurcBillEnt.Save(model, _userId, url);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Check(int billId, string comment)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            _validationMsg = _chemLocalPurcBillEnt.Check(billId, _userId, comment);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Confirm(int billId, string comment)
        {
            _validationMsg = _chemLocalPurcBillEnt.Confirm(billId, comment);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckRefBeforeDelete(int billRefId)
        {
            var flag = 0;
            var items =
                _unit.ChemicalLocalPurchaseBillItemRepository.Get().Where(ob => ob.BillRefID == billRefId).ToList();
            if (items.Count > 0)
            {
                flag = 1;
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteReference(int billRefId)
        {
            _validationMsg = _chemLocalPurcBillEnt.DeleteReference(billRefId);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteItem(int billItemId)
        {
            _validationMsg = _chemLocalPurcBillEnt.DeleteItem(billItemId);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteBill(int billId)
        {
            _validationMsg = _chemLocalPurcBillEnt.DeleteBill(billId);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBillListForSearch(string billNo)
        {
            var billList = _chemLocalPurcBillEnt.GeBillListForSearch(billNo);
            return Json(billList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBillList(string billNo)
        {
            var sysBill = new List<LocalPurchaseBillMin>();
            var billList = _chemLocalPurcBillEnt.GetBillList(billNo);

            foreach (var bill in billList)
            {
                var x = new LocalPurchaseBillMin();
                x.BillID = bill.BillId;
                x.BillDate = bill.BillDate;
                x.BillNo = bill.BillNo;
                x.SupplierName = bill.SupplierName;
                x.SupplierBillNo = bill.SupplierBillNo;
                x.RecordStatus = bill.RecordStatus;
                sysBill.Add(x);
            }
            return Json(sysBill.OrderByDescending(ob => ob.BillID), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSupplierListForSearch(string supplier)
        {
            var supplierList = _chemLocalPurcBillEnt.GeSupplierListForSearch(supplier);
            return Json(supplierList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSupplierList(string supplier)
        {
            var supplierList = _chemLocalPurcBillEnt.GetSupplierList(supplier);
            return Json(supplierList.OrderByDescending(ob => ob.SupplierName), JsonRequestBehavior.AllowGet);
        }
    }

    public class OrderMin
    {
        public int OrderID { get; set; }
        public string OrderNo { get; set; }
        public string OrderType { get; set; }
        public string OrderDate { get; set; }
    }

    public class LocalPurchaseReceiveMin
    {
        public long ReceiveID { get; set; }
        public string ReceiveNo { get; set; }
        public string ReceiveDate { get; set; }
        public string ReceiveType { get; set; }
    }

    public class LocalPurchaseBillMin
    {
        public long BillID { get; set; }
        public string BillNo { get; set; }
        public string BillDate { get; set; }
        public string SupplierName { get; set; }
        public string SupplierBillNo { get; set; }
        public string RecordStatus { get; set; }
        
    }

    public class PO
    {
        public long POReceiveID { get; set; }
        public int? OrderID { get; set; }
        public string OrderNo { get; set; }
        public string OrderDate { get; set; }
        public long? ReceiveID { get; set; }
        public string ReceiveNo { get; set; }
        public string ReceiveDate { get; set; }
        public string ReceiveType { get; set; }
    }
}