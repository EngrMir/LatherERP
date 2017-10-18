using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using ERP_Leather.ActionFilters;

namespace ERP_Leather.Controllers
{
    public class PackingListController : Controller
    {
        private readonly DalSysUnit _objSysUnit;
        private UnitOfWork _unit;
        private ValidationMsg _validationMsg;
        private DalLcmPackingList _dalLcmPackingList;
        private int _userId;
        public PackingListController()
        {
            _objSysUnit = new DalSysUnit();
            _unit = new UnitOfWork();
            _validationMsg = new ValidationMsg();
            _dalLcmPackingList = new DalLcmPackingList();
        }

        // GET: PackingList
        [CheckUserAccess("PackingList/PackingList")]
        public ActionResult PackingList()
        {
            ViewBag.UnitList = _objSysUnit.GetAllActiveLeatherChemical();
            return View();
        }

        public ActionResult DeletePackingListPacksItem(int plPackID)
        {
            _validationMsg = _dalLcmPackingList.DeletePackingListPacks(plPackID);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeletePackingListItem(int plItemID)
        {
            _validationMsg = _dalLcmPackingList.DeletePackingListItem(plItemID);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeletePackingList(int plid)
        {
            _validationMsg = _dalLcmPackingList.DeletePackingList(plid);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Save(PackingListVM model)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            _validationMsg = _dalLcmPackingList.Save(model, _userId, "PackingList/PackingList");

            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Check(int plid, string checkNote)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            _validationMsg = _dalLcmPackingList.Check(plid, _userId, checkNote);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Confirm(int plid, string confirmNote)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            _validationMsg = _dalLcmPackingList.Confirm(plid, _userId, confirmNote);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCIItemByCIID(int ciid)
        {
            var ciItems = _unit.CommercialInvoiceItemRepository.Get().Where(ob => ob.CIID == ciid).ToList();
            var plItemList = new List<PackingListItemVM>();
            foreach (var item in ciItems)
            {
                var pushItem = new PackingListItemVM();
                pushItem.ItemID = item.ItemID;
                pushItem.ItemName = _unit.SysChemicalItemRepository.GetByID(item.ItemID).ItemName;
                pushItem.HsCode = _unit.SysChemicalItemRepository.GetByID(item.ItemID).HSCode;
                pushItem.PackSize = item.PackSize;
                if (item.PackSize != null)
                {
                    pushItem.PackSizeName = _unit.SysSizeRepository.GetByID(item.PackSize).SizeName;
                }
                pushItem.SizeUnit = item.SizeUnit;
                if (item.SizeUnit != null)
                {
                    pushItem.SizeUnitName = _unit.SysUnitRepository.GetByID(item.SizeUnit).UnitName;
                }
                pushItem.PackQty = item.PackQty;
                pushItem.PlQty = item.CIQty;
                pushItem.PlUnit = item.CIUnit;
                pushItem.PlUnitName = _unit.SysUnitRepository.GetByID(item.CIUnit).UnitName;
                plItemList.Add(pushItem);
            }
            return Json(plItemList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllPackingListMin()
        {
            var packingLists = _unit.PackingListRepository.Get().ToList();
            var packingListsShow = packingLists.Select(item => new PackingListMin
            {
                Plid = item.PLID,
                PlNo = item.PLNo,
                PlDate = string.Format("{0:dd/MM/yyyy}", item.PLDate),
                LcNo = item.LCNo,
                LcDate = string.Format("{0:dd/MM/yyyy}", _unit.LCOpeningRepository.GetByID(item.LCID).LCDate),
                SupplierName = _unit.SysSupplierRepository.GetByID(_unit.PRQ_ChemicalPIRepository.GetByID(_unit.LCOpeningRepository.GetByID(item.LCID).PIID).SupplierID).SupplierName,
                CiNo = item.CINo,
                RecordStatus = DalCommon.ReturnRecordStatus(item.RecordStatus)
            }).ToList();
            return Json(packingListsShow.OrderByDescending(ob => ob.Plid), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPackingListById(int plid)
        {
            var packingList = _unit.PackingListRepository.GetByID(plid);
            var result = new PackingListVM
            {
                Plid = packingList.PLID,
                PlNo = packingList.PLNo,
                PlDate = string.Format("{0:dd/MM/yyyy}", packingList.PLDate),
                Lcid = packingList.LCID,
                LcNo = packingList.LCNo,
                Ciid = packingList.CIID,
                CiNo = packingList.CINo,
                NetWeight = packingList.PLNetWeight,
                NetWeightUnit = packingList.NetWeightUnit,
                GrossWeight = packingList.PLGrossWeight,
                GrossWeightUnit = packingList.GrossWeightUnit,
                PlNote = packingList.PLNote,
                RecordStatus = packingList.RecordStatus,
                packingListItem = new List<PackingListItemVM>(),
                packingListPacks = new List<PackingListPacksVM>()
            };
            if (packingList.LCID != null)
            {
                result.LcDate = _unit.LCOpeningRepository.GetByID(packingList.LCID).LCDate.ToString("dd/MM/yyyy");
            }
            if (packingList.CIID != null)
            {
                result.CiDate = string.Format("{0:dd/MM/yyyy}",
                    _unit.CommercialInvoiceRepository.GetByID(packingList.CIID).CIDate);
            }
            var packingListItems = _unit.PackingListItemRepository.Get().Where(ob => ob.PLID == plid).ToList();
            if (packingListItems.Count > 0)
            {
                foreach (var item in packingListItems)
                {
                    var packingListItem = new PackingListItemVM();
                    packingListItem.PlItemID = item.PLItemID;
                    packingListItem.Plid = item.PLID;
                    packingListItem.ItemID = item.ItemID;
                    if (item.ItemID != null)
                    {
                        packingListItem.ItemName = _unit.SysChemicalItemRepository.GetByID(item.ItemID).ItemName;
                        packingListItem.HsCode = _unit.SysChemicalItemRepository.GetByID(item.ItemID).HSCode;
                    }
                    packingListItem.PackSize = item.PackSize;
                    if (item.PackSize != null)
                    {
                        packingListItem.PackSizeName = _unit.SysSizeRepository.GetByID(item.PackSize).SizeName;
                    }
                    packingListItem.SizeUnit = item.SizeUnit;
                    if (item.SizeUnit != null)
                    {
                        packingListItem.SizeUnitName = _unit.SysUnitRepository.GetByID(item.SizeUnit).UnitName;
                    }
                    packingListItem.PackQty = item.PackQty;
                    packingListItem.PlQty = item.PLQty;
                    packingListItem.PlUnit = item.PLUnit;
                    if (item.PLUnit != null)
                    {
                        packingListItem.PlUnitName = _unit.SysUnitRepository.GetByID(item.PLUnit).UnitName;
                    }
                    packingListItem.SupplierID = item.SupplierID;
                    if (item.SupplierID != null)
                    {
                        packingListItem.SupplierName = _unit.SysSupplierRepository.GetByID(item.SupplierID).SupplierName;
                    }
                    packingListItem.ManufacturerID = item.ManufacturerID;
                    if (item.ManufacturerID != null)
                    {
                        packingListItem.ManufacturerName = _unit.SysSupplierRepository.GetByID(item.ManufacturerID).SupplierName;
                    }                    
                    result.packingListItem.Add(packingListItem);
                }
            }
            var packingListPacks = _unit.PackingListPacksRepository.Get().Where(ob => ob.PLID == plid).ToList();
            if (packingListPacks.Count > 0)
            {
                foreach (var pack in packingListPacks)
                {
                    var packingListPack = new PackingListPacksVM();
                    packingListPack.PlPackID = pack.PLPackID;
                    packingListPack.Plid = pack.PLID;
                    packingListPack.PackUnit = pack.PackUnit;
                    if (pack.PackUnit != null)
                    {
                        packingListPack.PackUnitName = _unit.SysUnitRepository.GetByID(pack.PackUnit).UnitName;
                    }
                    packingListPack.PackQty = pack.PackQty;
                    result.packingListPacks.Add(packingListPack);
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllPackingListForSearch()
        {
            var pl = _unit.PackingListRepository.Get().Select(ob => ob.PLNo).ToList();
            return Json(pl, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPackingListByPlNo(string plno)
        {
            var pl = _unit.PackingListRepository.Get().Where(ob => ob.PLNo == plno).ToList();
            var result = pl.Select(item => new PackingListMin
            {
                Plid = item.PLID,
                PlNo = item.PLNo,
                PlDate = string.Format("{0:dd/MM/yyyy}", item.PLDate),
                LcNo = item.LCNo,
                CiNo = item.CINo
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCiNo(long plid)
        {
            var cino = _unit.PackingListRepository.GetByID(plid);
            var result = new
            {
                cino.CIID,
                cino.CINo
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DelPckLstCont(long plid)
        {
            _validationMsg = _dalLcmPackingList.DelPckLstCont(plid);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }
    }

    public class PackingListMin
    {
        public int Plid { get; set; }
        public string PlNo { get; set; }
        public string PlDate { get; set; }
        public string LcNo { get; set; }
        public string LcDate { get; set; }
        public string SupplierName { get; set; }
        public string CiNo { get; set; }
        public string RecordStatus { get; set; }
    }
}