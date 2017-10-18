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
using Microsoft.Ajax.Utilities;

namespace ERP_Leather.Controllers
{


    public class FinishedLeatherIssueFromStoreController : Controller
    {
        private readonly UnitOfWork _unit;
        private int _userId;
        private ValidationMsg _validationMsg;
        private readonly BLC_DEVEntities _context;
        private readonly DalInvFinishedLeatherIssueFromStore _dalInvFinishedLeatherIssueFromStore;

        public FinishedLeatherIssueFromStoreController()
        {
            _unit = new UnitOfWork();
            _context = new BLC_DEVEntities();
            _validationMsg = new ValidationMsg();
            _dalInvFinishedLeatherIssueFromStore = new DalInvFinishedLeatherIssueFromStore();
        }
        public ActionResult FinishedLeatherIssueFromStore()
        {
            return View();
        }

        public ActionResult GetArticlesByParam(byte storeId, int buyerId, long orderId)
        {
            var articles =
                _unit.FinishBuyerStock.Get()
                    .Where(ob => ob.StoreID == storeId && ob.BuyerID == buyerId && ob.BuyerOrderID == orderId)
                    .ToList();
            var result = articles.Select(article => new
            {
                article.ArticleID,
                article.ArticleNo,
                _unit.ArticleRepository.GetByID(article.ArticleID).ArticleName,
                article.ArticleChallanNo
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStoreByTrans()
        {
            var stores =
                _unit.StoreRepository.Get()
                    .Where(ob => (ob.StoreType == "Finish") && ob.IsActive && !ob.IsDelete)
                    .Select(store => new
                    {
                        store.StoreID,
                        store.StoreName
                    }).ToList();
            return Json(stores.OrderBy(ob => ob.StoreName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStoreByPack()
        {
            var stores =
                _unit.StoreRepository.Get()
                    .Where(ob => (ob.StoreType == "Packing") && ob.IsActive && !ob.IsDelete)
                    .Select(store => new
                    {
                        store.StoreID,
                        store.StoreName
                    }).ToList();
            return Json(stores.OrderBy(ob => ob.StoreName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBuyerByStore(int storeId, string qclabel)
        {
            var result = new object();
            if (qclabel == " Passed")
            {
                result = _dalInvFinishedLeatherIssueFromStore.GetBuyerPass(storeId);
            }
            if (qclabel == " Failed")
            {
                result = _dalInvFinishedLeatherIssueFromStore.GetBuyerFail(storeId);
            }
            if (qclabel == " All")
            {
                result = _dalInvFinishedLeatherIssueFromStore.GetBuyerAll(storeId);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOrderByBuyer(byte storeId, long buyerId)
        {
            var result = _dalInvFinishedLeatherIssueFromStore.GetOrderByBuyer(storeId, buyerId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetColorByComb(byte storeId, long buyerId, long orderId, byte itemTypeId, byte leatherTypeId, byte leatherStatusID, int articleId)
        {
            var result = _dalInvFinishedLeatherIssueFromStore.GetColorByComb(storeId, buyerId, orderId, itemTypeId, leatherTypeId, leatherStatusID, articleId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetIssueById(long issueId)
        {
            var issue = _unit.FinishLeatherIssue.GetByID(issueId);
            //var x = new InvFinishLeatherIssue();
            var result = new InvFinishLeatherIssue();

            result.FinishLeatherIssueID = issue.FinishLeatherIssueID;
            result.FinishLeatherIssueNo = issue.FinishLeatherIssueNo;
            result.FinishLeatherIssueDate = string.Format("{0:dd/MM/yyyy}", issue.FinishLeatherIssueDate);
            result.IssueCategory = issue.IssueCategory;
            result.IssueFor = issue.IssueFor;
            result.IssueFrom = issue.IssueFrom;
            result.IssueTo = issue.IssueTo;
            result.CheckNote = issue.CheckNote;
            result.IssueNote = issue.IssueNote;
            result.RecordStatus = issue.RecordStatus;
            result.Items = new List<InvFinishedLeatherIssueItem>();
            var items =
                _unit.FinishLeatherIssueItem.Get()
                    .Where(ob => ob.FinishLeatherIssueID == issue.FinishLeatherIssueID)
                    .ToList();
            if (items.Count > 0)
            {
                foreach (var item in items)
                {
                    var y = new InvFinishedLeatherIssueItem();
                    y.FinishLeatherIssueItemID = item.FinishLeatherIssueItemID;
                    y.FinishLeatherIssueID = item.FinishLeatherIssueID;
                    y.RequisitionDateID = item.RequisitionDateID;
                    y.BuyerID = item.BuyerID;
                    y.BuyerName = item.BuyerID == null
                        ? ""
                        : _unit.SysBuyerRepository.GetByID(item.BuyerID).BuyerName;
                    y.BuyerOrderID = item.BuyerOrderID;
                    y.BuyerOrderNo = item.BuyerOrderID == null
                        ? ""
                        : _unit.SlsBuyerOrederRepository.GetByID(item.BuyerOrderID).BuyerOrderNo;
                    y.ArticleID = item.ArticleID;
                    y.ArticleNo = item.ArticleNo;
                    y.ArticleChallanNo = item.ArticleChallanNo;
                    y.ArticleName = item.ArticleID == null
                        ? ""
                        : _unit.ArticleRepository.GetByID(item.ArticleID).ArticleName;
                    y.ItemTypeID = item.ItemTypeID;
                    y.ItemTypeName = item.ItemTypeID == null
                        ? ""
                        : _unit.SysItemTypeRepository.GetByID(item.ItemTypeID).ItemTypeName;
                    y.LeatherTypeID = item.LeatherTypeID;
                    y.LeatherTypeName = item.LeatherTypeID == null
                        ? ""
                        : _unit.SysLeatherTypeRepository.GetByID(item.LeatherTypeID).LeatherTypeName;
                    y.LeatherStatusID = item.LeatherStatusID;
                    y.LeatherStatusName = item.LeatherStatusID == null
                        ? ""
                        : _unit.SysLeatherStatusRepo.GetByID(item.LeatherStatusID).LeatherStatusName;
                    y.Colors = new List<InvFinishedLeatherIssueColor>();
                    var colors =
                        _unit.FinishLeatherIssueColor.Get()
                            .Where(ob => ob.FinishLeatherIssueItemID == item.FinishLeatherIssueItemID)
                            .ToList();
                    if (colors.Count > 0)
                    {
                        foreach (var color in colors)
                        {
                            var z = new InvFinishedLeatherIssueColor();
                            z.FinishLeatherIssueColorID = color.FinishLeatherIssueColorID;
                            z.FinishLeatherIssueItemID = color.FinishLeatherIssueItemID;
                            z.FinishLeatherIssueID = color.FinishLeatherIssueID;
                            z.ColorID = color.ColorID;
                            z.ColorName = color.ColorID == null
                                ? ""
                                : _unit.SysColorRepository.GetByID(color.ColorID).ColorName;
                            //z.GradeID = color.GradeID;
                            //z.GradeName = color.GradeID == null ? "" : _unit.SysGrade.GetByID(color.GradeID).GradeName;
                            var stock =
                                _unit.FinishBuyerStock.Get()
                                    .Where(
                                        ob =>
                                            ob.BuyerID == item.BuyerID && ob.BuyerOrderID == item.BuyerOrderID &&
                                            ob.ArticleID == item.ArticleID && ob.ItemTypeID == item.ItemTypeID &&
                                            ob.LeatherTypeID == item.LeatherTypeID &&
                                            ob.LeatherStatusID == item.LeatherStatusID && ob.ColorID == color.ColorID &&
                                            ob.GradeID == color.GradeID && ob.ClosingStockPcs > 0).OrderByDescending(ob => ob.TransectionID)
                                    .FirstOrDefault();
                            z.ClosingStockPcs = stock == null ? 0 : stock.ClosingStockPcs;
                            z.ClosingStockSide = stock == null ? 0 : stock.ClosingStockSide;
                            z.ClosingStockArea = stock == null ? 0 : stock.ClosingStockArea;
                            z.IssuePcs = color.IssuePcs;
                            z.IssueSide = color.IssueSide;
                            z.IssueArea = color.IssueArea;
                            z.SideArea = color.SideArea;
                            z.AreaUnit = color.AreaUnit;
                            z.UnitName = color.AreaUnit == null
                                ? ""
                                : _unit.SysUnitRepository.GetByID(color.AreaUnit).UnitName;
                            z.FinishQCLabel = color.FinishQCLabel;
                            y.Colors.Add(z);
                        }
                    }
                    result.Items.Add(y);
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFinishLeatherIssues()
        {
            var issues = _unit.FinishLeatherIssue.Get().Select(issue => new
            {
                issue.FinishLeatherIssueID,
                issue.FinishLeatherIssueNo,
                FinishLeatherIssueDate = string.Format("{0:dd/MM/yyyy}", issue.FinishLeatherIssueDate),
                RecordStatus = DalCommon.ReturnRecordStatus(issue.RecordStatus)
            }).ToList();
            return Json(issues.OrderBy(ob => ob.FinishLeatherIssueDate), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(InvFinishLeatherIssue model)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            _validationMsg = _dalInvFinishedLeatherIssueFromStore.Save(model, _userId, "FinishedLeatherIssueFromStore/FinishedLeatherIssueFromStore");
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Check(long issueId, string comment)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            _validationMsg = _dalInvFinishedLeatherIssueFromStore.Check(issueId, _userId, comment);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Issue(InvFinishLeatherIssue model)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            _validationMsg = _dalInvFinishedLeatherIssueFromStore.Confirm(model, _userId);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(long id, string del)
        {
            if (del == "all")
            {
                _validationMsg = _dalInvFinishedLeatherIssueFromStore.DeleteAll(id);
            }
            if (del == "item")
            {
                _validationMsg = _dalInvFinishedLeatherIssueFromStore.DeleteItem(id);
            }
            if (del == "color")
            {
                _validationMsg = _dalInvFinishedLeatherIssueFromStore.DeleteColor(id);
            }
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }
    }

    public class BuyerEx
    {
        int? BuyerID { get; set; }
        string BuyerCode { get; set; }
        string BuyerName { get; set; }
        string BuyerType { get; set; }
        int BuyerAddressId { get; set; }
        string Address { get; set; }
    }
}