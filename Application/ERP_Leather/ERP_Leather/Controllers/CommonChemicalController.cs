using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.OperationModel;

namespace ERP_Leather.Controllers
{
    public class CommonChemicalController : Controller
    {
        private UnitOfWork _unit;
        private int _userId;
        private readonly DalSysSupplier _objSupplier;


        public CommonChemicalController()
        {
            _unit = new UnitOfWork();
            _objSupplier = new DalSysSupplier();
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetAllActiveLeatherChemical()
        {
            var objUnit = new DalSysUnit();

            var allData = objUnit.GetAllActiveLeatherChemical();
            return Json(allData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllActiveChemicalItem()
        {
            var allData = new DalSysChemicalItem().GetAllActiveChemicalItem();
            return Json(allData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllChemicalSupplier()
        {
            var allData = new DalSysSupplier().GetAllChemicalSupplier();
            return Json(allData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLocalChemicalSupplierList()
        {
            var supplier = _objSupplier.GetAllLocalChemicalSupplier();
            return Json(supplier, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetChemicalSupplierByType(string supplierType)
        {
            var type = supplierType == "LP" ? "Local" : "Foreign";
            var allData = new DalSysSupplier().GetChemicalSupplierByType(type);
            return Json(allData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetChemicalSupplierByTypeForSearch(string supplierType)
        {
            var type = supplierType == "LP" ? "Local" : "Foreign";
            var allData = new DalSysSupplier().GetChemicalSupplierByType(type).Select(s => s.SupplierName);
            return Json(allData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllUnitForChemical()
        {
            var chemicalUnit = _unit.SysUnitRepository.Get().Where(ob => ob.UnitCategory == "Chemical" && ob.IsDelete != true).ToList();
            var lstChemicalUnit = chemicalUnit.Select(item => new SysUnit { UnitID = item.UnitID, UnitName = item.UnitName }).ToList();
            return Json(lstChemicalUnit, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllChemicalItemType()
        {
            var chemicalItemType = _unit.SysChemicalItemRepository.Get().Where(ob => ob.IsActive != false).ToList();
            var lstChemicalItemType = chemicalItemType.Select(item => new SysChemicalItem { ItemID = item.ItemID, ItemName = item.ItemName, HSCode = item.HSCode, ItemCategory = DalCommon.ReturnChemicalItemCategory(item.ItemCategory) }).ToList();
            return Json(lstChemicalItemType.OrderBy(ob => ob.ItemName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllChemicalUnit()
        {
            var chemicalUnit = _unit.SysUnitRepository.Get().Where(ob => ob.UnitCategory == "ChemicalPack" && ob.IsActive && ob.IsDelete == false).ToList();
            var lstChemicalUnit =
                chemicalUnit.Select(item => new Sys_Unit { UnitID = item.UnitID, UnitName = item.UnitName }).ToList();
            return Json(lstChemicalUnit, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCommonChemicalUnit()
        {
            var chemicalUnit = _unit.SysUnitRepository.Get().Where(ob => (ob.UnitCategory == "Chemical" || ob.UnitCategory == "Common") && ob.IsActive && ob.IsDelete == false).ToList();
            var lstChemicalUnit =
                chemicalUnit.Select(item => new Sys_Unit { UnitID = item.UnitID, UnitName = item.UnitName }).ToList();
            return Json(lstChemicalUnit, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllManufacturer()
        {
            var manufactutrer = _unit.SysSupplierRepository.Get().Where(ob => ob.SupplierCategory == "Chemical" && ob.SupplierType == "Manufacturer").ToList();
            var lstManufacturer =
                manufactutrer.Select(
                    item => new SysSupplier { SupplierID = item.SupplierID, SupplierName = item.SupplierName, SupplierCode = item.SupplierCode, SupplierType = item.SupplierType }).ToList();
            return Json(lstManufacturer, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllSize()
        {
            var sizes = _unit.SysSizeRepository.Get().Where(ob => ob.SizeCategory == "ChemicalPack").ToList();
            var sizeList = sizes.Select(item => new SysSize { SizeID = item.SizeID, SizeName = item.SizeName }).ToList();
            return Json(sizeList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllPackUnitForChemical()
        {
            var chemicalUnit = _unit.SysUnitRepository.Get().Where(ob => ob.UnitCategory == "ChemicalPack" || ob.UnitCategory == "Common" && ob.IsActive && !ob.IsDelete).ToList();
            var lstChemicalUnit =
                chemicalUnit.Select(item => new PackUnit { PackUnitID = item.UnitID, PackUnitName = item.UnitName }).ToList();
            return Json(lstChemicalUnit, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllPackSizeForChemical()
        {
            var packSizeUnit = _unit.SysSizeRepository.Get().Where(ob => ob.SizeCategory == "ChemicalPack" && ob.IsActive && !ob.IsDelete).OrderBy(o=>Convert.ToInt32(o.SizeName)).ToList();
            var lstPackSizeUnit =
                packSizeUnit.Select(item => new SysSize() { SizeID = item.SizeID, SizeName = item.SizeName }).ToList();
            return Json(lstPackSizeUnit, JsonRequestBehavior.AllowGet);
            //var data = new DalChemicalForeignPurchaseOrder().GetAllPackSizeForChemical();
            //return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetChemicalItemsForSearch()
        {
            var chemItems =
                _unit.SysChemicalItemRepository.Get()
                    .Where(ob => ob.IsActive == true)
                    .Select(ob => new
                    {
                        ob.ItemID,
                        ob.ItemName,
                        ob.HSCode,
                        ItemCategory = DalCommon.ReturnChemicalItemCategory(ob.ItemCategory)
                    }).ToList();
            return Json(chemItems, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetChemicalUnit()
        {
            var chemUnit =
                _unit.SysUnitRepository.Get()
                    .Where(ob => ob.UnitCategory == "Chemical" && ob.IsActive && !ob.IsDelete)
                    .Select(unit => new { unit.UnitID, unit.UnitName })
                    .ToList();
            return Json(chemUnit, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetChemicalPackUnit()
        {
            var chemPackUnit =
                _unit.SysUnitRepository.Get()
                .Where(ob => ob.UnitCategory == "ChemicalPack" && ob.IsActive && !ob.IsDelete)
                .Select(unit => new { unit.UnitID, unit.UnitName })
                .ToList();
            return Json(chemPackUnit, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetManufacturer()
        {
            var result =
                _unit.SysSupplierRepository.Get()
                    .Where(ob => ob.SupplierType == "Manufacturer" && ob.IsActive && !ob.IsDelete)
                    .Select(sup => new { sup.SupplierID, sup.SupplierCode, sup.SupplierName });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLocalPurchaseBill()
        {
            var bills = _unit.ChemicalLocalPurchaseBillRepository.Get().Where(ob => ob.RecordStatus == "CNF")
                 .Select(bill => new LocalPurchaseBillMin
                 {
                     BillID = bill.BillID,
                     BillNo = bill.BillNo,
                     BillDate = string.Format("{0:dd/MM/yyyy}", bill.BillDate),
                     SupplierName = _unit.SysSupplierRepository.GetByID(bill.SupplierID).SupplierName,
                     SupplierBillNo = bill.SupplierBillNo
                 }).ToList();
            return Json(bills, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLocalPurchaseBillBySup(int supplierId)
        {
            var bills = _unit.ChemicalLocalPurchaseBillRepository.Get().Where(ob => ob.RecordStatus == "CNF" && ob.SupplierID == supplierId)
                 .Select(bill => new LocalPurchaseBillMin
                 {
                     BillID = bill.BillID,
                     BillNo = bill.BillNo,
                     BillDate = string.Format("{0:dd/MM/yyyy}", bill.BillDate),
                     SupplierName = _unit.SysSupplierRepository.GetByID(bill.SupplierID).SupplierName,
                     SupplierBillNo = bill.SupplierBillNo
                 }).ToList();
            return Json(bills, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllWetBlueBuyer()
        {
            var result =
                _unit.SysBuyerRepository.Get()
                    .Where(ob => ob.BuyerCategory == "Buyer" && ob.IsActive == true)
                    .Select(b => new { b.BuyerID, b.BuyerCode, b.BuyerName }).OrderBy(o => o.BuyerID).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllUnitForThickness()
        {
            var thicknessUnit =
                _unit.SysUnitRepository.Get()
                .Where(ob => ob.UnitCategory == "Thickness" && ob.IsActive && !ob.IsDelete)
                .Select(unit => new { unit.UnitID, unit.UnitName })
                .ToList();
            return Json(thicknessUnit, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetChemLcList()
        {
            var result =
                            _unit.LCOpeningRepository.Get()
                //.Where(ob => ob.RecordStatus=="CNF")
                                .Select(b => new LcmLcOpening
                                {
                                    LCID = b.LCID,
                                    LCNo = b.LCNo,
                                    LCDate = string.Format("{0:dd-MMM-yyyy}", b.LCDate),
                                    RecordStatus = DalCommon.ReturnOrderStatus(b.RecordStatus)
                                }
                                ).ToList().OrderByDescending(s => s.LCID);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllActiveArticleItem()
        {
            var bills = _unit.ArticleRepository.Get().Where(ob => ob.IsActive == true)
                 .Select(a => new SysArticle()
                 {
                     ArticleID = a.ArticleID,
                     ArticleNo = a.ArticleNo,
                     ArticleName = a.ArticleName,
                     ArticleChallanNo = a.ArticleChallanNo ?? "",
                     ArticleColor = a.ArticleColor,
                     ColorName = a.ArticleColor != null ? _unit.SysColorRepository.GetByID(a.ArticleColor).ColorName : ""
                 }).ToList();
            return Json(bills, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetGrade()
        {
            var grades = _unit.SysGrade.Get().Where(ob => ob.IsActive && !ob.IsDelete)
                .Select(grade => new
                {
                    grade.GradeID,
                    grade.GradeName,
                    grade.Description
                }
                ).ToList();

            return Json(grades.OrderBy(ob => ob.GradeName), JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetBuyer()
        {
            var buyers = _unit.SysBuyerRepository.Get().Where(ob => ob.IsActive == true && ob.BuyerCategory == "Buyer")
                .Select(buyer => new
                {
                    buyer.BuyerID,
                    buyer.BuyerCode,
                    buyer.BuyerName,
                    buyer.BuyerType,
                    BuyerAddressId = _unit.BuyerAddressRepository.Get().FirstOrDefault(ob => ob.BuyerID == buyer.BuyerID).BuyerAddressID,
                    _unit.BuyerAddressRepository.Get().FirstOrDefault(ob => ob.BuyerID == buyer.BuyerID).Address
                }).ToList();
            return Json(buyers.OrderBy(ob => ob.BuyerName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuyerOrder(long buyerId)
        {
            var orders = _unit.SlsBuyerOrederRepository.Get().Where(ob => ob.BuyerID == buyerId)
                .Select(order => new
                {
                    order.BuyerOrderID,
                    order.BuyerOrderNo,
                    BuyerOrderDate = string.Format("{0:dd-mm-yyyy}", order.BuyerOrderDate),
                    order.BuyerOrderStatus,
                    BuyerName = order.BuyerID == null ? "" : _unit.SysBuyerRepository.GetByID(order.BuyerID).BuyerName,
                    RecordStatus = DalCommon.ReturnRecordStatus(order.RecordStatus)
                }).ToList();
            return Json(orders.OrderByDescending(ob => ob.BuyerOrderID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetGrades()
        {
            var grades = _unit.SysGrade.Get().Where(ob => ob.IsActive && !ob.IsDelete).Select(grade => new
            {
                grade.GradeID,
                grade.GradeName,
                grade.Description
            }).ToList();

            return Json(grades.OrderBy(ob => ob.GradeName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetArticles()
        {
            var articles =
                _unit.ArticleRepository.Get()
                    .Where(ob => ob.ArticleCategory == "LTHR" && ob.IsActive == true)
                    .Select(article => new
                    {
                        article.ArticleID,
                        article.ArticleNo,
                        article.ArticleName,
                        article.ArticleChallanNo
                    }).ToList();
            return Json(articles.OrderBy(ob => ob.ArticleName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetColors()
        {
            var colors =
                _unit.SysColorRepository.Get().Where(ob => ob.IsActive == true)
                .Select(color => new
                {
                    color.ColorID,
                    color.ColorCode,
                    color.ColorName
                }).ToList();
            return Json(colors.OrderBy(ob => ob.ColorCode), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetIssue()
        {
            var issues = _unit.CrustLeatherIssue.Get().Select(issue => new
            {
                issue.CrustLeatherIssueID,
                issue.CrustLeatherIssueNo,
                CrustLeatherIssueDate = string.Format("{0:dd/MM/yyyy}", issue.CrustLeatherIssueDate),
                RecordStatus = DalCommon.ReturnRecordStatus(issue.RecordStatus)
            }).ToList();
            return Json(issues.OrderBy(ob => ob.CrustLeatherIssueDate), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUnitsForLeather()
        {
            var units =
                _unit.SysUnitRepository.Get().Where(ob => ob.UnitCategory == "Leather").Select(unit => new
                {
                    unit.UnitID,
                    unit.UnitName
                }).ToList();
            return Json(units.OrderBy(ob => ob.UnitName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetShipperInfo()
        {
            var shipperInfo = _unit.SysBuyerRepository.Get().Where(ob => ob.BuyerCategory == "Forwarder").ToList();
            var allShipperInfo = shipperInfo.Select(info => new
            {
                info.BuyerID,
                info.BuyerCode,
                info.BuyerName,
                info.BuyerType
            }).ToList();
            return Json(allShipperInfo, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPortInfo()
        {
            var portsInfo = _unit.SysPortRepository.Get().Where(ob => ob.IsActive == true).ToList();
            var allPortInfo = portsInfo.Select(info => new
            {
                info.PortID,
                info.PortCode,
                info.PortName,
                info.PortAdress
            }).ToList();
            return Json(allPortInfo, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllCountry()
        {
            var countries = _unit.SysCountryRepository.Get().Where(ob => ob.IsActive == true).ToList();
            var result = countries.Select(cntry => new
            {
                cntry.CountryID,
                cntry.CountryCode,
                cntry.CountryName
            }).ToList();
            return Json(result.OrderBy(ob => ob.CountryName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllBuyer()
        {
            var buyers = _unit.SysBuyerRepository.Get().Where(ob => ob.IsActive == true).ToList();
            var result = new List<SysBuyer>();
            foreach (var buyer in buyers)
            {
                var x = new SysBuyer();
                x.BuyerID = buyer.BuyerID;
                x.BuyerCode = buyer.BuyerCode;
                x.BuyerName = buyer.BuyerName;
                x.BuyerType = buyer.BuyerType;
                var add = _unit.BuyerAddressRepository.Get().FirstOrDefault(ob => ob.BuyerID == buyer.BuyerID && ob.IsActive == true);
                x.Address = add == null ? "" : add.Address;
                result.Add(x);
            }
            return Json(result.OrderBy(ob => ob.BuyerName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllBanks()
        {
            var banks = _unit.BankRepository.Get().Where(ob => ob.BankCategory == "BNK" && ob.IsActive == true).ToList();
            var result = banks.Select(bank => new
            {
                bank.BankID,
                bank.BankCode,
                bank.BankName,
                bank.BankSwiftCode,
                bank.BankBINNo,
                BankType = DalCommon.ReturnBankType(bank.BankType),
                bank.IsActive
            }).ToList().OrderBy(ob => ob.BankName);
            //}).ToList().OrderByDescending(m => m.BankID);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBranchByBank(int bankId)
        {
            var branches = _unit.BranchRepository.Get().Where(ob => ob.BankID == bankId).ToList();
            var result = branches.Select(branch => new
            {
                branch.BranchID,
                branch.BankID,
                branch.BanchCode,
                branch.BranchName,
                branch.Address1,
                branch.Address2,
                branch.Address3,
                branch.LCLimit,
                branch.LCOpened,
                branch.LCBalance,
                branch.LCMargin,
                branch.LCMarginUsed,
                branch.LCMarginBalance,
                branch.BranchSwiftCode,
                branch.LIMLimit,
                branch.LIMTaken,
                branch.LIMBalance,
                IsActive = branch.IsActive == true ? "Active" : "Inactive"
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllBranches()
        {
            var branches = _unit.BranchRepository.Get().Where(ob => ob.IsActive == true).ToList();
            var result = new List<SysBranch>();
            foreach (var branch in branches)
            {
                var x = new SysBranch();
                x.BranchID = branch.BranchID;
                x.BanchCode = branch.BanchCode;
                x.BranchName = branch.BranchName;
                x.Address1 = branch.Address1;
                x.Address2 = branch.Address2;
                x.Address3 = branch.Address3;
                result.Add(x);
            }
            return Json(result.OrderBy(ob => ob.BranchName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ArticleChallanList()
        {
            var data = new DalWetBlueIssue().GetAllActiveChallanArticleWithoutBuyer();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ArticleList()
        {
            var data = new DalWetBlueIssue().GetAllActiveArticle();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }

}