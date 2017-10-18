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
using Microsoft.Ajax.Utilities;

namespace ERP_Leather.Controllers
{
    public class CrustedLeatherIssueFromStoreController : Controller
    {
        private readonly UnitOfWork _unit;
        private int _userId;
        private ValidationMsg _validationMsg;
        private readonly BLC_DEVEntities _context;
        private readonly DalInvCrustedLeatherIssueFromStore _dalInvCrustedLeatherIssueFrom;
        private readonly DalSysStore _dalSysStore;
        public CrustedLeatherIssueFromStoreController()
        {
            _unit = new UnitOfWork();
            _context = new BLC_DEVEntities();
            _validationMsg = new ValidationMsg();
            _dalInvCrustedLeatherIssueFrom = new DalInvCrustedLeatherIssueFromStore();
            _dalSysStore = new DalSysStore();
        }
        [CheckUserAccess("CrustedLeatherIssueFromStore/CrustedLeatherIssueFromStore")]
        public ActionResult CrustedLeatherIssueFromStore()
        {
            ViewBag.Store = _unit.StoreRepository.Get().Where(ob => ob.StoreCategory == "Leather" && ob.StoreType == "CR Production").Select(ob => new
            {
                ob.StoreID,
                ob.StoreName
            }).ToList();

            return View();
        }
        public ActionResult GetIssue()
        {
            var issues = _unit.CrustLeatherIssue.Get().Where(ob => ob.IssueCategory != "IAQC").Select(issue => new
            {
                issue.CrustLeatherIssueID,
                issue.CrustLeatherIssueNo,
                CrustLeatherIssueDate = string.Format("{0:dd/MM/yyyy}", issue.CrustLeatherIssueDate),
                RecordStatus = DalCommon.ReturnRecordStatus(issue.RecordStatus)
            }).ToList();
            return Json(issues.OrderByDescending(ob => ob.CrustLeatherIssueID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStoreByTrans()
        {
            var stores = _dalSysStore.GetAllActiveCrustStore();
            return Json(stores.OrderBy(ob => ob.StoreName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStoreByProd()
        {
            var stores = _dalSysStore.GetAllActiveFinishedStore();
            return Json(stores.OrderBy(ob => ob.StoreName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBuyerByStore(int storeId, string qcLabel)
        {


            var buyers = _dalInvCrustedLeatherIssueFrom.GetByStoreId(storeId, qcLabel);

            return Json(buyers, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOrderByBuyer(byte storeId, long buyerId)
        {
            var result = _dalInvCrustedLeatherIssueFrom.GetOrderByBuyer(storeId, buyerId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetColorByComb(byte storeId, string qcLabel, int? buyerId, long? buyerOrderId, int? articleId, long? articleChallanId, byte? itemTypeId, byte? leatherTypeId, byte? leatherStatusId)
        {
            var colors = _dalInvCrustedLeatherIssueFrom.GetColorByComb(storeId, qcLabel, buyerId, buyerOrderId, articleId, articleChallanId, itemTypeId, leatherTypeId, leatherStatusId);
            var result = new List<InvCrustLeatherIssueColor>();

            foreach (var color in colors)
            {
                var item = new InvCrustLeatherIssueColor();
                item.CrustLeatherIssueID = color.CrustLeatherIssueID;
                item.CrustLeatherIssueItemID = color.CrustLeatherIssueItemID;
                item.CrustLeatherIssueColorID = color.CrustLeatherIssueColorID;
                item.ArticleColorNo = color.ArticleColorNo;
                item.ColorID = color.ColorID;
                item.ColorName = color.ColorName;
                ////item.GradeID = color.GradeID;
                ////item.GradeName = color.GradeName;
                item.GradeRange = color.GradeRange;
                item.ClosingStockPcs = color.ClosingStockPcs;
                item.ClosingStockSide = color.ClosingStockSide;
                item.ClosingStockArea = color.ClosingStockArea;
                item.IssuePcs = color.ClosingStockPcs;
                item.IssueSide = color.ClosingStockSide ;
                item.IssueArea = color.ClosingStockArea;
                item.SideArea = color.SideArea;
                item.AreaUnit = color.AreaUnit;
                item.AreaUnitName = color.AreaUnitName;
                item.CrustQCLabel = color.CrustQCLabel;
                result.Add(item);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetColorsGrade(byte storeId, int buyerId, long orderId, int articleId, byte itemTypeId, byte leatherTypeId, byte leatherStatusId)
        {
            var colorsGrades =
                _context.INV_CrustBuyerStock.Where(
                    ob =>
                        ob.StoreID == storeId && ob.BuyerID == buyerId && ob.BuyerOrderID == orderId && ob.ArticleID == articleId &&
                        ob.ItemTypeID == itemTypeId && ob.LeatherTypeID == leatherTypeId &&
                        ob.LeatherStatusID == leatherStatusId && ob.ClosingStockPcs > 0).Max();
            var result = new
            {
                colorsGrades.ColorID,
                _unit.SysColorRepository.GetByID(colorsGrades.ColorID).ColorName,
                colorsGrades.GradeID,
                _unit.SysGrade.GetByID(colorsGrades.GradeID).GradeName,
                colorsGrades.ClosingStockPcs,
                colorsGrades.ClosingStockSide,
                colorsGrades.ClosingStockArea
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetIssueById(long issueId)
        {
            //var result = _dalInvCrustedLeatherIssueFrom.GetIssueById(issueId);


            var issue = _unit.CrustLeatherIssue.GetByID(issueId);
            var result = new InvCrustLeatherIssue();
            result.CrustLeatherIssueID = issue.CrustLeatherIssueID;
            result.CrustLeatherIssueNo = issue.CrustLeatherIssueNo;
            result.CrustLeatherIssueDate = string.Format("{0:dd/MM/yyyy}",issue.CrustLeatherIssueDate);
            result.IssueCategory = issue.IssueCategory;
            result.IssueFor = issue.IssueFor;
            result.IssueFrom = issue.IssueFrom;
            result.IssueTo = issue.IssueTo;
            result.RecordStatus = issue.RecordStatus;
            result.CheckNote = issue.CheckNote;
            result.IssueNote = issue.IssueNote;
            result.Items = new List<InvCrustLeatherIssueItem>();
            var items =
                _unit.CrustLeatherIssueItem.Get()
                    .Where(ob => ob.CrustLeatherIssueID == result.CrustLeatherIssueID)
                    .ToList();
            if (items.Count > 0)
            {
                foreach (var item in items)
                {
                    var x = new InvCrustLeatherIssueItem();
                    x.CrustLeatherIssueID = item.CrustLeatherIssueID;
                    x.CrustLeatherIssueItemID = item.CrustLeatherIssueItemID;
                    x.RequisitionDateID = item.RequisitionDateID;
                    x.RequisitionNo = item.RequisitionDateID == null ? "" :  _unit.PrdYearMonthFinishReqDate.GetByID(item.RequisitionDateID).RequisitionNo;
                    x.BuyerID = item.BuyerID;
                    x.BuyerName = item.BuyerID == null ? "" : _unit.SysBuyerRepository.GetByID(item.BuyerID).BuyerName;
                    x.BuyerOrderID = item.BuyerOrderID;
                    x.BuyerOrderNo = item.BuyerOrderID == null
                        ? ""
                        : _unit.SlsBuyerOrederRepository.GetByID(item.BuyerOrderID).BuyerOrderNo;
                    x.ArticleID = item.ArticleID;
                    x.ArticleName = item.ArticleID == null
                        ? ""
                        : _unit.ArticleRepository.GetByID(item.ArticleID).ArticleName;
                    x.ArticleNo = item.ArticleID == null
                        ? ""
                        : _unit.ArticleRepository.GetByID(item.ArticleID).ArticleNo;
                    x.ArticleChallanNo = item.ArticleChallanNo;
                    x.ArticleChallanID = item.ArticleChallanID;
                    x.ItemTypeID = item.ItemTypeID;
                    x.ItemTypeName = item.ItemTypeID == null
                        ? ""
                        : _unit.SysItemTypeRepository.GetByID(item.ItemTypeID).ItemTypeName;
                    x.LeatherTypeID = item.LeatherTypeID;
                    x.LeatherTypeName = item.LeatherTypeID == null
                        ? ""
                        : _unit.SysLeatherTypeRepository.GetByID(item.LeatherTypeID).LeatherTypeName;
                    x.LeatherStatusID = item.LeatherStatusID;
                    x.LeatherStatusName = item.LeatherStatusID == null
                        ? ""
                        : _unit.SysLeatherStatusRepo.GetByID(item.LeatherStatusID).LeatherStatusName;
                    x.CrustQCLabel = item.CrustQCLabel;
                    x.Colors = new List<InvCrustLeatherIssueColor>();
                    var colors =
                        _unit.CrustLeatherIssueColor.Get()
                            .Where(ob => ob.CrustLeatherIssueItemID == x.CrustLeatherIssueItemID)
                            .ToList();
                    if (colors.Count > 0)
                    {
                        foreach (var color in colors)
                        {
                            var y = new InvCrustLeatherIssueColor();
                            y.CrustLeatherIssueID = color.CrustLeatherIssueID;
                            y.CrustLeatherIssueItemID = color.CrustLeatherIssueItemID;
                            y.CrustLeatherIssueColorID = color.CrustLeatherIssueColorID;
                            y.ArticleColorNo = color.ArticleColorNo;
                            y.ColorID = color.ColorID;
                            y.ColorName = color.ColorID == null
                                ? ""
                                : _unit.SysColorRepository.GetByID(color.ColorID).ColorName;
                            y.GradeRange = color.GradeRange;
                            //y.GradeID = color.GradeID;
                            //y.GradeName = color.GradeID == null ? "" : _unit.SysGrade.GetByID(color.GradeID).GradeName;
                            var clsngStkPcs = _context.INV_CrustBuyerStock.Where(
                                ob =>
                                    ob.StoreID == issue.IssueFrom && ob.BuyerID == item.BuyerID &&
                                    ob.BuyerOrderID == item.BuyerOrderID && ob.ArticleID == item.ArticleID &&
                                    ob.ItemTypeID == item.ItemTypeID && ob.LeatherTypeID == item.LeatherTypeID &&
                                    ob.LeatherStatusID == item.LeatherStatusID && ob.ColorID == color.ColorID
                                    && ob.CrustQCLabel == color.CrustQCLabel &&
                                    ob.ArticleChallanNo == item.ArticleChallanNo)
                                .OrderByDescending(m => m.TransectionID)
                                .FirstOrDefault();
                            y.ClosingStockPcs = clsngStkPcs == null ? 0 : clsngStkPcs.ClosingStockPcs;
                            var clsngStkSide = _context.INV_CrustBuyerStock.Where(
                                ob =>
                                    ob.StoreID == issue.IssueFrom && ob.BuyerID == item.BuyerID &&
                                    ob.BuyerOrderID == item.BuyerOrderID && ob.ArticleID == item.ArticleID &&
                                    ob.ItemTypeID == item.ItemTypeID && ob.LeatherTypeID == item.LeatherTypeID &&
                                    ob.LeatherStatusID == item.LeatherStatusID && ob.ColorID == color.ColorID &&
                                    ob.CrustQCLabel == color.CrustQCLabel &&
                                    ob.ArticleChallanNo == item.ArticleChallanNo)
                                .OrderByDescending(m => m.TransectionID)
                                .FirstOrDefault();
                            y.ClosingStockSide = clsngStkSide == null ? 0 : clsngStkSide.ClosingStockSide;
                            var clsngStkArea = _context.INV_CrustBuyerStock.Where(
                                ob =>
                                    ob.StoreID == issue.IssueFrom && ob.BuyerID == item.BuyerID &&
                                    ob.BuyerOrderID == item.BuyerOrderID && ob.ArticleID == item.ArticleID &&
                                    ob.ItemTypeID == item.ItemTypeID && ob.LeatherTypeID == item.LeatherTypeID &&
                                    ob.LeatherStatusID == item.LeatherStatusID && ob.ColorID == color.ColorID &&
                                    ob.CrustQCLabel == color.CrustQCLabel &&
                                    ob.ArticleChallanNo == item.ArticleChallanNo)
                                .OrderByDescending(m => m.TransectionID)
                                .FirstOrDefault();
                            y.ClosingStockArea = clsngStkArea == null ? 0 : clsngStkArea.ClosingStockArea;
                            y.IssuePcs = color.IssuePcs;
                            y.IssueSide = color.IssueSide;
                            y.IssueArea = color.IssueArea;
                            y.SideArea = color.SideArea;
                            y.AreaUnit = color.AreaUnit;
                            y.AreaUnitName = color.AreaUnit == null
                                ? ""
                                : _unit.SysUnitRepository.GetByID(color.AreaUnit).UnitName;
                            y.CrustQCLabel = color.CrustQCLabel;
                            x.Colors.Add(y);
                        }
                    }
                    result.Items.Add(x);
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProdFloor()
        {
            var floors =
                _unit.StoreRepository.Get()
                    .Where(ob => ob.StoreType == "FN Production" && ob.StoreCategory == "Production")
                    .Select(floor => new
                    {
                        floor.StoreID,
                        floor.StoreName
                    }).ToList();
            return Json(floors.OrderBy(ob => ob.StoreName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRequisition(string year, string month, byte? floor)
        {
            var req = _dalInvCrustedLeatherIssueFrom.GetReq(year, month, "CRR", floor);

            return Json(req, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetReqById(long reqDateId, byte? issueFrom, byte? issueTo, string qcLabel)
        {
            var req = _dalInvCrustedLeatherIssueFrom.GetReqItemNColors(reqDateId, issueFrom, issueTo, qcLabel);
            return Json(req, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(InvCrustLeatherIssue model)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            _validationMsg = _dalInvCrustedLeatherIssueFrom.Save(model, _userId, "CrustedLeatherIssueFromStore/CrustedLeatherIssueFromStore");
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(long id, string del)
        {
            if (del == "all")
            {
                _validationMsg = _dalInvCrustedLeatherIssueFrom.DeleteAll(id);
            }
            if (del == "item")
            {
                _validationMsg = _dalInvCrustedLeatherIssueFrom.DeleteItem(id);
            }
            if (del == "color")
            {
                _validationMsg = _dalInvCrustedLeatherIssueFrom.DeleteColor(id);
            }
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Check(long issueId, string comment)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            _validationMsg = _dalInvCrustedLeatherIssueFrom.Check(issueId, _userId, comment);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Issue(long crustId)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            _validationMsg = _dalInvCrustedLeatherIssueFrom.Confirm(crustId, _userId);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }
    }
}