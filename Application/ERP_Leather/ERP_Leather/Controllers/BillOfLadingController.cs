using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using ERP_Leather.ActionFilters;

namespace ERP_Leather.Controllers
{
    public class BillOfLadingController : Controller
    {
        private int _save;
        private int _userId;
        private UnitOfWork _unit;
        private ValidationMsg _validationMsg;
        private DalLcmBillOfLading _dalLcmBillOfLading;
        public BillOfLadingController()
        {
            _save = 0;
            _unit = new UnitOfWork();
            _validationMsg = new ValidationMsg();
            _dalLcmBillOfLading = new DalLcmBillOfLading();
        }
        // GET: BillOfLading
        [CheckUserAccess("BillOfLading/BillOfLading")]
        public ActionResult BillOfLading()
        {
            return View();
        }

        public ActionResult GetComInv()
        {
            var comInvs = _unit.CommercialInvoiceRepository.Get().Select(comInv => new
            {
                comInv.CIID, comInv.CINo, comInv.LCID, comInv.LCNo,
                CIDate = string.Format("{0:dd/MM/yyyy}", comInv.CIDate),
                _unit.LCOpeningRepository.GetByID(comInv.LCID).PINo,
                RecordStatus = DalCommon.ReturnRecordStatus(comInv.RecordStatus)
            }).ToList();
            return Json(comInvs.OrderByDescending(ob => ob.CIID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLcInfo()
        {
            var lcInfo = _unit.LCOpeningRepository.Get().Where(ob => ob.RecordStatus == "CNF" && ob.LCStatus == "RNG").ToList();
            var allLcInfoMin = lcInfo.Select(info => new LcInfoMin
            {
                Lcid = info.LCID, LcNo = info.LCNo, LcDate = info.LCDate.ToString("dd/mm/yyyy"), PiNo = info.PINo
            }).ToList();
            return Json(allLcInfoMin, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetShipperInfo()
        {
            var shipperInfo = _unit.SysBuyerRepository.Get().Where(ob => ob.BuyerCategory == "Shipper").ToList();
            var allShipperInfo = shipperInfo.Select(info => new ShipperInfo
            {
                Id=info.BuyerID, Code = info.BuyerCode, Name = info.BuyerName, Type = info.BuyerType
            }).ToList();
            return Json(allShipperInfo, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetShipperForSearch()
        {
            var shipper =
                _unit.SysBuyerRepository.Get()
                    .Where(ob => ob.BuyerCategory == "Chemical")
                    .Select(ob => ob.BuyerName)
                    .ToList();
            return Json(shipper, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetShipperByName(string name)
        {
            var shipper =
                _unit.SysBuyerRepository.Get()
                    .Where(ob => ob.BuyerName == name && ob.BuyerCategory == "Chemical")
                    .ToList();
            var result = shipper.Select(item => new ShipperInfo
            {
                Id = item.BuyerID, Code = item.BuyerCode, Name = item.BuyerName, Type = item.BuyerType
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPortInfo()
        {
            var portsInfo = _unit.SysPortRepository.Get().Where(ob => ob.IsActive == true).ToList();
            var allPortInfo = portsInfo.Select(info => new PortInfoMin
            {
                Id = info.PortID, Code = info.PortCode, Name = info.PortName, Address = info.PortAdress
            }).ToList();
            return Json(allPortInfo, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPortForSearch()
        {
            var ports = _unit.SysPortRepository.Get().Where(ob => ob.IsActive == true).Select(ob => ob.PortName);
            return Json(ports, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPortByName(string name)
        {
            var ports = _unit.SysPortRepository.Get().Where(ob => ob.PortName == name && ob.IsActive == true).ToList();
            var result = ports.Select(item => new PortInfoMin
            {
                Id = item.PortID, Code = item.PortCode, Name = item.PortName, Address = item.PortAdress
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBillOfLadingMin()
        {
            var blEntityMin = _unit.BillOfLadingRepository.Get().ToList();
            var blModelMin = blEntityMin.Select(item => new BillOfLadingMin
            {
                Blid = item.BLID, BlNo = item.BLNo, BlDate = string.Format("{0:dd/MM/yyyy}", item.BLDate), LcNo = item.LCNo,
                LcDate = string.Format("{0:dd/MM/yyyy}", _unit.LCOpeningRepository.GetByID(item.LCID).LCDate),
                SupplierName = _unit.SysSupplierRepository.GetByID(_unit.PRQ_ChemicalPIRepository.GetByID(_unit.LCOpeningRepository.GetByID(item.LCID).PIID).SupplierID).SupplierName,
                PiNo = _unit.LCOpeningRepository.GetByID(item.LCID).PINo,
                CiNo = item.CIID == null ? "" : _unit.CommercialInvoiceRepository.GetByID(item.CIID).CINo,
                RecordStatus = DalCommon.ReturnRecordStatus(item.RecordStatus)
            }).ToList();
            return Json(blModelMin.OrderByDescending(ob => ob.Blid), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBillOfLadingForSearch()
        {
            var bl = _unit.BillOfLadingRepository.Get().Select(ob => ob.BLNo).ToList();
            return Json(bl, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBillOfLadingByBlno(string blno)
        {
            var bl = _unit.BillOfLadingRepository.Get().Where(ob => ob.BLNo == blno).ToList();
            var result = bl.Select(item => new BillOfLadingMin
            {
                Blid = item.BLID, BlNo = item.BLNo, BlDate = string.Format("{0:dd/MM/yyyy}", item.BLDate), LcNo = item.LCNo,
                PiNo = _unit.LCOpeningRepository.GetByID(item.LCID).PINo
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBillOfLadingById(int blid)
        {
            var blEntity = _unit.BillOfLadingRepository.GetByID(blid);
            var blModel = new LcmBillOfLadingVM();
            blModel.Blid = blEntity.BLID;
            blModel.BlNo = blEntity.BLNo;
            blModel.BlDate = string.Format("{0:dd/MM/yyyy}",blEntity.BLDate);
            blModel.Lcid = blEntity.LCID;
            blModel.LcNo = blEntity.LCNo;
            blModel.CIID = blEntity.CIID;
            blModel.CINo = blEntity.CINo;
            blModel.ShippedOnBoardDate = string.Format("{0:dd/MM/yyyy}", blEntity.ShippedOnBoardDate);
            blModel.ExpectedArrivalTime = string.Format("{0:dd/MM/yyyy}", blEntity.ExpectedArrivalTime);
            blModel.Shipper = blEntity.Shipper;
            blModel.ShipmentMode = blEntity.ShipmentMode;
            blModel.VesselName = blEntity.VesselName;
            blModel.VoyageNo = blEntity.VoyageNo;
            blModel.TransShipmentPort = blEntity.TransShipmentPort;
            blModel.ShipmentPort = blEntity.ShipmentPort;
            blModel.RecordStatus = blEntity.RecordStatus;
            blModel.Containers = new List<LcmBillOfLadingContainerVM>();
            if (blEntity.LCID != null)
            {
                blModel.PiNo = _unit.LCOpeningRepository.GetByID(blEntity.LCID).PINo;
            }
            if (blEntity.Shipper != null)
            {
                blModel.ShipperName = _unit.SysBuyerRepository.GetByID(blEntity.Shipper).BuyerName;    
            }
            if (blEntity.TransShipmentPort != null)
            {
                blModel.TransShipmentPortName = _unit.SysPortRepository.GetByID(blEntity.TransShipmentPort).PortName;    
            }
            if (blEntity.ShipmentPort != null)
            {
                blModel.ShipmentPortName = _unit.SysPortRepository.GetByID(blEntity.ShipmentPort).PortName;
            }
            var blcEntity = _unit.BillOfLadingContainerRepository.Get().Where(ob => ob.BLID == blid).ToList();
            foreach (var entity in blcEntity)
            {
                var blcModel = new LcmBillOfLadingContainerVM();
                blcModel.BlcCntId = entity.BLCcntID;
                blcModel.Blid = entity.BLID;
                blcModel.ContainerNo = entity.ContainerNo;
                blcModel.ContainerType = entity.ContainerType;
                blcModel.SealNo = entity.SealNo;
                blcModel.PackageQty = entity.PackageQty;
                blcModel.GrossWeight = entity.GrossWeight;
                blcModel.WeightUnit = entity.WeightUnit;
                if (entity.WeightUnit != null)
                {
                    blcModel.WeightUnitName = _unit.SysUnitRepository.GetByID(entity.WeightUnit).UnitName;
                }
                blModel.Containers.Add(blcModel);
            }
            return Json(blModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteBillOfLadingContainer(int blcCntId)
        {
            _validationMsg = _dalLcmBillOfLading.DeleteBillOfLadingContainer(blcCntId);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteBillOfLading(int blcCntId)
        {
            _validationMsg = _dalLcmBillOfLading.DeleteBillOfLading(blcCntId);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(LcmBillOfLadingVM model)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            _validationMsg = _dalLcmBillOfLading.Save(model, _userId, "BillOfLading/BillOfLading");

            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Check(int blid, string checkNote)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            _validationMsg = _dalLcmBillOfLading.Check(blid, _userId, checkNote);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Confirm(int blid, string confirmNote)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            _validationMsg = _dalLcmBillOfLading.Confirm(blid, _userId, confirmNote);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }
    }

    public class LcInfoMin
    {
        public int Lcid { get; set; }
        public string LcNo { get; set; }
        public string LcDate { get; set; }
        public string PiNo { get; set; }
    }

    public class ShipperInfo
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class PortInfoMin
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public class BillOfLadingMin
    {
        public int Blid { get; set; }
        public string BlNo { get; set; }
        public string BlDate { get; set; }
        public string LcNo { get; set; }
        public string LcDate { get; set; }
        public string SupplierName { get; set; }
        public string PiNo { get; set; }
        public string CiNo { get; set; }
        public string RecordStatus { get; set; }
    }
}