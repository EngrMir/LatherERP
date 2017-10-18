using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.BusinessLogicLayer.OperationManager;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.OperationModel;

namespace ERP_Leather.Controllers
{
    public class CommonController : Controller
    {
        DalSysSupplier objSupplier = new DalSysSupplier();
        BllPrqPurchase objPurchase = new BllPrqPurchase();
        UnitOfWork unit = new UnitOfWork();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetSupplierList()
        {
            var supplier = objSupplier.GetAllSupplier();
            return Json(supplier, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSupplierListSearchById(string supplier)
        {
            supplier = supplier.ToUpper();
            var supplierData = objSupplier.GetAllSupplier().Where(ob => ob.SupplierName.StartsWith(supplier)).ToList();
            return Json(supplierData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetLocationList()
        {
            DalSysLocation objLocation = new DalSysLocation();
            var locations = objLocation.GetAllActiveLocation();
            return Json(locations, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSourceList()
        {
            DalSysSource objSource = new DalSysSource();

            var AllData = objSource.GetAllActiveSource();
            return Json(AllData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllItemSize()
        {
            DalSysSize objSource = new DalSysSize();

            var items = objSource.GetAllActiveItemSize();
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllStore()
        {
            DalSysStore objStore = new DalSysStore();

            var allData = objStore.GetAllActiveStore();
            return Json(allData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllUnit()
        {
            DalSysUnit objUnit = new DalSysUnit();

            var allData = objUnit.GetAllActiveUnit();
            return Json(allData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllLeatherUnit()
        {
            var objUnit = new DalSysUnit();

            var allData = objUnit.GetAllActiveLeatherUnit();
            return Json(allData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllItemType()
        {
            DalSysItemType objSource = new DalSysItemType();

            var allData = objSource.GetAllActiveItemTypeLeather();
            return Json(allData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPaymentTypeList()
        {
            DalSysSource objSource = new DalSysSource();

            var AllData = objSource.GetAllActiveSource();
            return Json(AllData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPaymentMethodList()
        {
            DalSysSize objSource = new DalSysSize();

            var items = objSource.GetAllActiveItemSize();
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllActiveCurrency()
        {
            DalSysCurrency objSource = new DalSysCurrency();

            var allData = objSource.GetAllActiveCurrency();
            return Json(allData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLCOpeningBankList()
        {
            var bank = unit.BankRepository.Get().Where(ob => ob.BankCategory == "BANK" && ob.BankType == "LOC");
            List<LCM_LCOpeningBank> lstBank = new List<LCM_LCOpeningBank>();
            foreach (var item in bank)
            {
                LCM_LCOpeningBank ob = new LCM_LCOpeningBank();
                ob.BankID = item.BankID;
                ob.BankName = item.BankName;
                ob.BankCode = item.BankCode;
                lstBank.Add(ob);
            }
            return Json(lstBank, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchLCOpeningBankByBank(string bank)
        {
            bank = bank.ToUpper();
            var bankData = unit.BankRepository.Get().Where(ob => ob.BankName.StartsWith(bank) && ob.BankCategory == "BANK" && ob.BankType == "LOC").ToList();
            List<LCM_LCOpeningBank> lstBank = new List<LCM_LCOpeningBank>();
            foreach (var item in bankData)
            {
                LCM_LCOpeningBank ob = new LCM_LCOpeningBank();
                ob.BankID = item.BankID;
                ob.BankName = item.BankName;
                ob.BankCode = item.BankCode;
                lstBank.Add(ob);
            }
            return Json(lstBank, JsonRequestBehavior.AllowGet);
        }

	}
}