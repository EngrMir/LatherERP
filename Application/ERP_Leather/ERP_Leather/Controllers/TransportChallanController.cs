using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using ERP_Leather.ActionFilters;

namespace ERP_Leather.Controllers
{
    public class TransportChallanController : Controller
    {
        private int _userId;
        private readonly UnitOfWork _unit;
        private ValidationMsg _validationMsg;
        private readonly DalExpTransportChallan _dalExpTransportChallan;
        private readonly BLC_DEVEntities _context;

        public TransportChallanController()
        {
            _userId = 0;
            _unit = new UnitOfWork();
            _context = new BLC_DEVEntities();
            _validationMsg = new ValidationMsg();
            _dalExpTransportChallan = new DalExpTransportChallan();
        }
        // GET: TransportChallan
        [CheckUserAccess("TransportChallan/TransportChallan")]
        public ActionResult TransportChallan()
        {
            return View();
        }

        public ActionResult Save(ExpTransportChallan model)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            _validationMsg = _dalExpTransportChallan.Save(model, _userId);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Confirm(long id, string note)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            _validationMsg = _dalExpTransportChallan.Confirm(id, _userId, note);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(long id)
        {
            _validationMsg = _dalExpTransportChallan.Delete(id);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteTrnsChlnCi(long id, long id2)
        {
            _validationMsg = _dalExpTransportChallan.DeleteTrnsChlnCi(id, id2);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDlvryChln()
        {
            var dlvryChlns = _unit.EXPDeliveryChallanRepository.Get().ToList();
            var result = dlvryChlns.Select(chln => new
            {
                chln.DeliverChallanID,
                chln.DeliverChallanNo,
                chln.DeliverChallanRef,
                DeliverChallanDate = string.Format("{0:dd/MM/yyyy}",chln.DeliverChallanDate),
                RecordStatus  = DalCommon.ReturnRecordStatus(chln.RecordStatus)
            }).ToList();
            return Json(result.OrderByDescending(ob => ob.DeliverChallanID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTransChln()
        {
            var transChlns = _context.EXP_TransportChallan.ToList();
            var result = transChlns.Select(chln => new
            {
                chln.TransportChallanID,
                chln.TransportChallanNo,
                chln.TransportChallanRef,
                TransportChallanDate = string.Format("{0:dd/MM/yyyy}",chln.TransportChallanDate),
                RecordStatus = DalCommon.ReturnRecordStatus(chln.RecordStatus)
            }).ToList();
            return Json(result.OrderByDescending(ob => ob.TransportChallanID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTransChlnById(long id)
        {
            var chln = _dalExpTransportChallan.GetTranportChallan(id);
            return Json(chln, JsonRequestBehavior.AllowGet);
        }
    }
}