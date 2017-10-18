using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using ERP_Leather.ActionFilters;

namespace ERP_Leather.Controllers
{
    public class ExpBillOfLadingController : Controller
    {
        private int _userId;
        private UnitOfWork _unit;
        private ValidationMsg _validationMsg;
        private DalExpBillOfLading _dalExpBillOfLading;


        public ExpBillOfLadingController()
        {
            _userId = 0;
            _unit = new UnitOfWork();
            _validationMsg = new ValidationMsg();
            _dalExpBillOfLading = new DalExpBillOfLading();
        }

        // GET: ExpBillOfLading
        [CheckUserAccess("ExpBillOfLading/ExpBillOfLading")]
        public ActionResult ExpBillOfLading()
        {
            return View();
        }

        public ActionResult GetComInv()
        {
            var comInv = _unit.ExpCommercialInvoiceRepository.Get().ToList();
            var result = comInv.Select(inv => new
            {
                inv.CIID,
                inv.CINo,
                inv.CIRefNo,
                CIDate = string.Format("{0:dd/MM/yyyy}", inv.CIDate),
                OrdDeliveryMode = DalCommon.ReturnOrderDeliveryMode(inv.OrdDeliveryMode),
                RecordStatus = DalCommon.ReturnRecordStatus(inv.RecordStatus)
            });
            return Json(result.OrderByDescending(ob => ob.CIID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPackingListA()
        {
            var pl = _unit.ExpPackingListRepository.Get().Select(ob => new
            {
                ob.PLID,
                ob.PLNo,
                ob.CIID,
                CINo = ob.CIID == null ? "" : _unit.ExpCommercialInvoiceRepository.GetByID(ob.CIID).CINo,
                CIRefNo = ob.CIID == null ? "" : _unit.ExpCommercialInvoiceRepository.GetByID(ob.CIID).CIRefNo,
                CIDate = ob.CIID == null ? "" : string.Format("{0:dd/MM/yyyy}", _unit.ExpCommercialInvoiceRepository.GetByID(ob.CIID).CIDate),
                PLDate = string.Format("{0:dd/MM/yyyy}", ob.PLDate),
                RecordStatus = DalCommon.ReturnRecordStatus(ob.RecordStatus)
            }).ToList();
            return Json(pl.OrderByDescending(ob => ob.PLID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPackingListB(long ciid)
        {
            var pl = _unit.ExpPackingListRepository.Get().Where(ob => ob.CIID == ciid).Select(ob => new
            {
                ob.PLID,
                ob.PLNo,
                ob.CIID,
                CINo = ob.CIID == null ? "" : _unit.ExpCommercialInvoiceRepository.GetByID(ob.CIID).CINo,
                CIRefNo = ob.CIID == null ? "" : _unit.ExpCommercialInvoiceRepository.GetByID(ob.CIID).CIRefNo,
                CIDate = ob.CIID == null ? "" : string.Format("{0:dd/MM/yyyy}", _unit.ExpCommercialInvoiceRepository.GetByID(ob.CIID).CIDate),
                PLDate = string.Format("{0:dd/MM/yyyy}", ob.PLDate),
                RecordStatus = DalCommon.ReturnRecordStatus(ob.RecordStatus)
            }).ToList();
            return Json(pl.OrderByDescending(ob => ob.PLID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBol()
        {
            var bol = _unit.ExpBillOfLadingRepository.Get().ToList();
            var result = bol.Select(ob => new
            {
                ob.BLID,
                ob.BLNo,
                ob.RefBLNo,
                ob.CIID,
                CINo = ob.CIID == null ? "" : _unit.ExpCommercialInvoiceRepository.GetByID(ob.CIID).CINo,
                CIRefNo = ob.CIID == null ? "" : _unit.ExpCommercialInvoiceRepository.GetByID(ob.CIID).CIRefNo,
                BLDate = string.Format("{0:dd/MM/yyyy}", ob.BLDate),
                RecordStatus = DalCommon.ReturnRecordStatus(ob.RecordStatus)
            }).ToList();
            return Json(result.OrderByDescending(ob => ob.BLID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBolById(long id)
        {
            var bol = _unit.ExpBillOfLadingRepository.GetByID(id);
            var result = new
            {
                bol.BLID,
                bol.BLNo,
                bol.RefBLNo,
                BLDate = string.Format("{0:dd/MM/yyyy}", bol.BLDate),
                bol.CIID,
                CINo = bol.CIID == null ? "" : _unit.ExpCommercialInvoiceRepository.GetByID(bol.CIID).CINo,
                CIRefNo = bol.CIID == null ? "" : _unit.ExpCommercialInvoiceRepository.GetByID(bol.CIID).CIRefNo,
                OrdDeliveryMode = bol.CIID == null ? "" : DalCommon.ReturnOrderDeliveryMode(
                _unit.ExpCommercialInvoiceRepository.GetByID(bol.CIID).OrdDeliveryMode),
                CIDate = bol.CIID == null ? "" : string.Format("{0:dd/MM/yyyy}", _unit.ExpCommercialInvoiceRepository.GetByID(bol.CIID).CIDate),
                //bol.PLID,
                //PLNo = bol.PLID == null ? "" : _unit.ExpPackingListRepository.GetByID(bol.PLID).PLNo,
                ShippedOnBoardDate = string.Format("{0:dd/MM/yyyy}", bol.ShippedOnBoardDate),
                ExpectedArrivalTime = string.Format("{0:dd/MM/yyyy}", bol.ExpectedArrivalTime),
                bol.Shipper,
                ShipperName = bol.Shipper == null ? "" : _unit.SysBuyerRepository.GetByID(bol.Shipper).BuyerName,
                bol.ShipmentMode,
                bol.VesselName,
                bol.VoyageNo,
                bol.TransShipmentPort,
                TransShipmentPortName = bol.TransShipmentPort == null ? "" : _unit.SysPortRepository.GetByID(bol.TransShipmentPort).PortName,
                bol.ShipmentPort,
                ShipmentPortName = bol.ShipmentPort == null ? "" : _unit.SysPortRepository.GetByID(bol.ShipmentPort).PortName,
                bol.BLNote,
                bol.RecordStatus,
                Container = _unit.ExpBillOfLadingContainerRepository.Get().Where(ob => ob.BLID == bol.BLID).Select(cont => new
                {
                    cont.BLCcntID,
                    cont.BLID,
                    cont.ContainerNo,
                    cont.ContainerType,
                    cont.SealNo,
                    cont.PackageQty,
                    cont.GrossWeight,
                    cont.Measurement
                }).ToList()
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(ExpBillOfLading model)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            _validationMsg = _dalExpBillOfLading.Save(model, _userId);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Check(long id, string note)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            _validationMsg = _dalExpBillOfLading.Check(id, note, _userId);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Confrim(long id, string note)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            _validationMsg = _dalExpBillOfLading.Confirm(id, note, _userId);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckContainerNo(string cntNo)
        {
            var chkCnt = _unit.ExpBillOfLadingContainerRepository.Get().Where(ob => ob.ContainerNo == cntNo);
            return chkCnt.Any() ? Json(true, JsonRequestBehavior.AllowGet) : Json(false, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(long id, string del)
        {
            if (del == "all")
            {
                _validationMsg = _dalExpBillOfLading.DeleteAll(id);
            }
            if (del == "cont")
            {
                _validationMsg = _dalExpBillOfLading.DeleteCont(id);
            }

            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckBillNo(string billNo)
        {
            var chkBill = _unit.ExpBillOfLadingRepository.Get().Where(ob => ob.BLNo == billNo);
            return chkBill.Any() ? Json(true, JsonRequestBehavior.AllowGet) : Json(false, JsonRequestBehavior.AllowGet);
        }
    }
}