using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.BusinessLogicLayer.OperationManager;
using ERP.EntitiesModel.OperationModel;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP_Leather.ActionFilters;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Data;
using ERP.DatabaseAccessLayer.Utility;
using System.Text;


namespace ERP_Leather.Controllers
{

    public class ReportsController : Controller
    {

        DalSysStore objStore = new DalSysStore();
        DalSysItemType objItemType = new DalSysItemType();
        DalSysLeatherType objLeatherType = new DalSysLeatherType();
        private DalReport _dalReport = new DalReport();
        private DalSysSupplier _dalSysSupplier = new DalSysSupplier();
        private DalSysGradeRange _dalSysGradeRange = new DalSysGradeRange();
        private DALLCOpening _dalLcOpening = new DALLCOpening();
        private readonly UnitOfWork _repository = new UnitOfWork();
        private const string ReportDateFormat = "dd-MMM-yy";

        [CheckUserAccess("Reports/ChemLocPurcRcvRpt")]
        public ActionResult ChemLocPurcRcvRpt()
        {
            ViewBag.Store = objStore.GetAllActiveChemicalStore();
            ViewBag.ddlItemCategory = new DalSysChemicalItem().GetAllChemicalItemCategory();
            return View();
        }
        [HttpPost]
        public ActionResult ChemLocPurcRcvRpt(ReportModel model)
        {
            ViewBag.Store = objStore.GetAllActiveChemicalStore();
            ViewBag.ddlItemCategory = new DalSysChemicalItem().GetAllChemicalItemCategory();
            var reportDocument = new ReportDocument();
            var rptPath = Server.MapPath("~/Reports");
            var dataTableObj = new DataTable();
            var reportObj = new DalReport();
            dataTableObj = reportObj.GetChemLocPurcReceiveInfo(model);
            switch (model.ReportName)
            {
                case "ChemicalWiseLocPurcRcvSummary":
                    rptPath = rptPath + "/ChemicalWiseLocPurcRcvSummary.rpt";
                    SetParamValueInChemicalPurchaseReport(model, reportDocument, rptPath, dataTableObj);
                    break;
                case "ChemicalWiseSuppLocPurcRcvSummary":
                    rptPath = rptPath + "/ChemicalWiseSuppLocPurcRcvSummary.rpt";
                    SetParamValueInChemicalPurchaseReport(model, reportDocument, rptPath, dataTableObj);
                    break;
                case "ChemicalWiseLocPurcRcvDetail":
                    rptPath = rptPath + "/ChemicalWiseLocPurcRcvDetail.rpt";
                    SetParamValueInChemicalPurchaseReport(model, reportDocument, rptPath, dataTableObj);
                    break;
                case "SupplierWiseChemLocPurcRcvSummary":
                    rptPath = rptPath + "/SupplierWiseChemLocPurcRcvSummary.rpt";
                    SetParamValueInChemicalPurchaseReport(model, reportDocument, rptPath, dataTableObj);
                    break;
                case "SupplierWiseLocPurcRcvDetail":
                    rptPath = rptPath + "/SupplierWiseLocPurcRcvDetail.rpt";
                    SetParamValueInChemicalPurchaseReport(model, reportDocument, rptPath, dataTableObj);
                    break;
            }
            reportDocument.ExportToHttpResponse(
               model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord,
               System.Web.HttpContext.Current.Response, false, "crReport");
            return View();
        }


        [CheckUserAccess("Reports/ChemFrnPurcRvcRpt")]
        public ActionResult ChemFrnPurcRvcRpt()
        {
            ViewBag.Store = objStore.GetAllActiveChemicalStore();
            ViewBag.ddlItemCategory = new DalSysChemicalItem().GetAllChemicalItemCategory();
            return View();
        }
        [HttpPost]
        public ActionResult ChemFrnPurcRvcRpt(ReportModel model)
        {
            ViewBag.Store = objStore.GetAllActiveChemicalStore();
            ViewBag.ddlItemCategory = new DalSysChemicalItem().GetAllChemicalItemCategory();
            var reportDocument = new ReportDocument();
            var rptPath = Server.MapPath("~/Reports");
            var dataTableObj = new DataTable();
            var reportObj = new DalReport();
            dataTableObj = reportObj.GetChemFrnPurcReceiveInfo(model);
            switch (model.ReportName)
            {
                case "ChemicalWiseFrnPurcRcvSummary":
                    rptPath = rptPath + "/ChemicalWiseFrnPurcRcvSummary.rpt";
                    SetParamValueInChemicalPurchaseReport(model, reportDocument, rptPath, dataTableObj);
                    break;
                case "ChemicalWiseSuppFrnPurcRcvSummary":
                    rptPath = rptPath + "/ChemicalWiseSuppFrnPurcRcvSummary.rpt";
                    SetParamValueInChemicalPurchaseReport(model, reportDocument, rptPath, dataTableObj);
                    break;
                case "ChemicalWiseFrnPurcRcvDetail":
                    rptPath = rptPath + "/ChemicalWiseFrnPurcRcvDetail.rpt";
                    SetParamValueInChemicalPurchaseReport(model, reportDocument, rptPath, dataTableObj);
                    break;
                case "SupplierWiseChemFrnPurcRcvSummary":
                    rptPath = rptPath + "/SupplierWiseChemFrnPurcRcvSummary.rpt";
                    SetParamValueInChemicalPurchaseReport(model, reportDocument, rptPath, dataTableObj);
                    break;
                case "SupplierWiseFrnPurcRcvDetail":
                    rptPath = rptPath + "/SupplierWiseFrnPurcRcvDetail.rpt";
                    SetParamValueInChemicalPurchaseReport(model, reportDocument, rptPath, dataTableObj);
                    break;
            }
            reportDocument.ExportToHttpResponse(
               model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord,
               System.Web.HttpContext.Current.Response, false, "crReport");
            return View();
        }

        private void SetParamValueInChemicalPurchaseReport(ReportModel model, ReportDocument reportDocument, string rptPath, DataTable dataTableObj)
        {
            reportDocument.Load(rptPath);
            reportDocument.SetDataSource(dataTableObj);
            reportDocument.Refresh();
            reportDocument.SetParameterValue("SupplierName", model.SupplierName ?? "");
            reportDocument.SetParameterValue("ItemName",
                model.ItemID != null
                    ? _repository.SysChemicalItemRepository.GetByID(Convert.ToInt32(model.ItemID)).ItemName
                    : "");
            reportDocument.SetParameterValue("StoreName",
                model.StoreID != null ? _repository.StoreRepository.GetByID(Convert.ToByte(model.StoreID)).StoreName : "");

            reportDocument.SetParameterValue("FromDate1",
            model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
            reportDocument.SetParameterValue("ToDate1",
            model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
        }

        [CheckUserAccess("Reports/ChemProdRequisitionRpt")]
        public ActionResult ChemProdRequisitionRpt()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChemProdRequisitionRpt(ReportModel model)
        {

            var reportDocument = new ReportDocument();
            var rptPath = Server.MapPath("~/Reports");
            var dataTableObj = new DataTable();
            var reportObj = new DalReport();
            dataTableObj = reportObj.GetChemProdRequisitionInfo(model);
            switch (model.ReportName)
            {
                case "ChemProdRequisitionWiseItems":
                    rptPath = rptPath + "/ChemProdRequisitionWiseItems.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("ItemName",
                        model.ItemID != null
                            ? _repository.SysChemicalItemRepository.GetByID(Convert.ToInt32(model.ItemID)).ItemName
                            : "");
                    reportDocument.SetParameterValue("FromDate1",
                    model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
                case "ChemProdReqItemWiseSummary":
                    rptPath = rptPath + "/ChemProdReqItemWiseSummary.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("ItemName",
                       model.ItemID != null
                           ? _repository.SysChemicalItemRepository.GetByID(Convert.ToInt32(model.ItemID)).ItemName
                           : "");
                    reportDocument.SetParameterValue("FromDate1",
                    model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
            }
            reportDocument.ExportToHttpResponse(
               model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord,
               System.Web.HttpContext.Current.Response, false, "crReport");
            return View();
        }

        [CheckUserAccess("Reports/ExpFreightBillRpt")]
        public ActionResult ExpFreightBillRpt()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ExpFreightBillRpt(ExpAgentComReportModel model)
        {
            ReportDocument reportDocument = new ReportDocument();
            string rptPath = Server.MapPath("~/Reports");

            var reportObj = new DalReport();
            DataTable dataTableObj = reportObj.GetFreightBillDetail(model);
            switch (model.ReportName)
            {
                case "ExpFreightBillDetail":
                    rptPath = rptPath + "/ExpFreightBillDetail.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("BuyerName", model.BuyerName ?? "");
                    reportDocument.SetParameterValue("FreightBillNo", model.FreightBillNo ?? "");
                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
                case "ExpFreightBillSummary":

                    rptPath = rptPath + "/ExpFreightBillSummary.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("BuyerName", model.BuyerName ?? "");
                    reportDocument.SetParameterValue("FreightBillNo", model.FreightBillNo ?? "");
                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;

            }
            reportDocument.ExportToHttpResponse(
               model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord,
               System.Web.HttpContext.Current.Response, false, "crReport");
            return View();
        }

        [CheckUserAccess("Reports/ExpBillLadingRpt")]
        public ActionResult ExpBillLadingRpt()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ExpBillLadingRpt(ExpAgentComReportModel model)
        {
            ReportDocument reportDocument = new ReportDocument();
            string rptPath = Server.MapPath("~/Reports");

            var reportObj = new DalReport();
            DataTable dataTableObj = reportObj.GetBillLadingDetail(model);
            switch (model.ReportName)
            {
                case "ExpBillLadingDetail":
                    rptPath = rptPath + "/ExpBillLadingDetail.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("BuyerName", model.BuyerName ?? "");
                    reportDocument.SetParameterValue("BLNo", model.BLNo ?? "");
                    reportDocument.SetParameterValue("TPortName", model.TPortName ?? "");
                    reportDocument.SetParameterValue("PortName", model.PortName ?? "");
                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
                case "ExpAllBuyerOrderColorLeverWiseDetail":

                    rptPath = rptPath + "/CrustProductionChallanDetail.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("BuyerName", model.BuyerName ?? "");
                    reportDocument.SetParameterValue("BLNo", model.BLNo ?? "");
                    reportDocument.SetParameterValue("TPortName", model.TPortName ?? "");
                    reportDocument.SetParameterValue("PortName", model.PortName ?? "");
                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;

            }
            reportDocument.ExportToHttpResponse(
               model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord,
               System.Web.HttpContext.Current.Response, false, "crReport");
            return View();
        }


        [CheckUserAccess("Reports/BankVoucherRpt")]
        public ActionResult BankVoucherRpt()
        {
            return View();
        }
        [HttpPost]
        public ActionResult BankVoucherRpt(ExpAgentComReportModel model)
        {
            ReportDocument reportDocument = new ReportDocument();
            string rptPath = Server.MapPath("~/Reports");

            var reportObj = new DalReport();
            DataTable dataTableObj = reportObj.GetBankVoucherDetail(model);
            switch (model.ReportName)
            {
                case "ExpBankVoucherDetail":
                    rptPath = rptPath + "/ExpBankVoucherDetail.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("BankName", model.BankName ?? "");
                    reportDocument.SetParameterValue("BVNo", model.BVNo ?? "");
                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
                case "ExpAllBuyerOrderColorLeverWiseDetail":

                    rptPath = rptPath + "/CrustProductionChallanDetail.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("BankName", model.BankName ?? "");
                    reportDocument.SetParameterValue("BVNo", model.BVNo ?? "");
                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;

            }
            reportDocument.ExportToHttpResponse(
               model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord,
               System.Web.HttpContext.Current.Response, false, "crReport");
            return View();
        }

        [CheckUserAccess("Reports/ExpAgentCommRpt")]
        public ActionResult ExpAgentCommRpt()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ExpAgentCommRpt(ExpAgentComReportModel model)
        {
            ReportDocument reportDocument = new ReportDocument();
            string rptPath = Server.MapPath("~/Reports");

            var reportObj = new DalReport();
            DataTable dataTableObj = reportObj.GetAgentCommissionDetail(model);
            switch (model.ReportName)
            {
                case "ExpAgentCommDetail":
                    rptPath = rptPath + "/ExpAgentCommDetail.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("BuyerName", model.BuyerName ?? "");
                    reportDocument.SetParameterValue("AgentComNo", model.AgentComNo ?? "");
                    reportDocument.SetParameterValue("AgentName", model.AgentName ?? "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
                case "ExpAllBuyerOrderColorLeverWiseDetail":

                    rptPath = rptPath + "/CrustProductionChallanDetail.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("BuyerName", model.BuyerName ?? "");
                    reportDocument.SetParameterValue("AgentComNo", model.AgentComNo ?? "");
                    reportDocument.SetParameterValue("AgentName", model.AgentName ?? "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));

                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;

            }
            reportDocument.ExportToHttpResponse(
               model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord,
               System.Web.HttpContext.Current.Response, false, "crReport");
            return View();
        }

        [CheckUserAccess("Reports/CrustChallanPrepRpt")]
        public ActionResult CrustChallanPrepRpt()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CrustChallanPrepRpt(ExpPiOrderReportModel model)
        {
            ReportDocument reportDocument = new ReportDocument();
            string rptPath = Server.MapPath("~/Reports");

            var reportObj = new DalReport();
            DataTable dataTableObj = reportObj.GetCrustProductionChallanDetail(model);
            switch (model.ReportName)
            {
                case "CrustProductionChallanDetail":
                    rptPath = rptPath + "/CrustProductionChallanDetail.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("BuyerName", model.BuyerName ?? "");
                    reportDocument.SetParameterValue("ArticleChallanNo", model.ArticleChallanNo ?? "");
                    reportDocument.SetParameterValue("ArticleName", model.ArticleName ?? "");
                    reportDocument.SetParameterValue("ColorName", model.ColorName ?? "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
                case "ExpAllBuyerOrderColorLeverWiseDetail":

                    rptPath = rptPath + "/CrustProductionChallanDetail.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("BuyerName", model.BuyerName ?? "");
                    reportDocument.SetParameterValue("ArticleChallanNo", model.ArticleChallanNo ?? "");
                    reportDocument.SetParameterValue("ArticleName", model.ArticleName ?? "");
                    reportDocument.SetParameterValue("ColorName", model.ColorName ?? "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;

            }
            reportDocument.ExportToHttpResponse(
               model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord,
               System.Web.HttpContext.Current.Response, false, "crReport");
            return View();
        }


        [CheckUserAccess("Reports/FinishedIssueAfterQcRpt")]
        public ActionResult FinishedIssueAfterQcRpt()
        {
            ViewBag.Store = objStore.GetAllActiveFinishProductionStore();
            return View();
        }
        [HttpPost]
        public ActionResult FinishedIssueAfterQcRpt(ExpPiOrderReportModel model)
        {
            ViewBag.Store = objStore.GetAllActiveFinishProductionStore();
            ReportDocument reportDocument = new ReportDocument();
            string rptPath = Server.MapPath("~/Reports");

            var reportObj = new DalReport();
            DataTable dataTableObj = reportObj.GetFinishedIssueAfterQcRptInfo(model);
            switch (model.ReportName)
            {
                case "IssueAfterFinishLeatherQcDetail":
                    rptPath = rptPath + "/IssueAfterFinishLeatherQcDetail.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("BuyerName", model.BuyerName ?? "");
                    reportDocument.SetParameterValue("BuyerOrderNo", model.BuyerOrderNo ?? "");
                    reportDocument.SetParameterValue("ArticleChallanNo", model.ArticleChallanNo ?? "");
                    reportDocument.SetParameterValue("ArticleName", model.ArticleName ?? "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
                case "BuyerWiseColorCrustLeatherRequisition":

                    rptPath = rptPath + "/BuyerWiseColorCrustLeatherRequisition.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("BuyerName", model.BuyerName ?? "");
                    reportDocument.SetParameterValue("BuyerOrderNo", model.BuyerOrderNo ?? "");
                    reportDocument.SetParameterValue("ArticleChallanNo", model.ArticleChallanNo ?? "");
                    reportDocument.SetParameterValue("ArticleName", model.ArticleName ?? "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
                case "ArticleWiseColorCrustLeatherRequisition":

                    rptPath = rptPath + "/ArticleWiseColorCrustLeatherRequisition.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("BuyerName", model.BuyerName ?? "");
                    reportDocument.SetParameterValue("BuyerOrderNo", model.BuyerOrderNo ?? "");
                    reportDocument.SetParameterValue("ArticleChallanNo", model.ArticleChallanNo ?? "");
                    reportDocument.SetParameterValue("ArticleName", model.ArticleName ?? "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;


            }
            reportDocument.ExportToHttpResponse(
               model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord,
               System.Web.HttpContext.Current.Response, false, "crReport");
            return View();
        }

        [CheckUserAccess("Reports/FinishedLeatherQcRpt")]
        public ActionResult FinishedLeatherQcRpt()
        {
            ViewBag.Store = objStore.GetAllActiveFinishProductionStore();
            return View();
        }
        [HttpPost]
        public ActionResult FinishedLeatherQcRpt(ExpPiOrderReportModel model)
        {
            //ViewBag.Store = objStore.GetAllActiveFinishProductionStore();
            ReportDocument reportDocument = new ReportDocument();
            string rptPath = Server.MapPath("~/Reports");

            var reportObj = new DalReport();
            DataTable dataTableObj = reportObj.GetFinishedLeatherQcRptInfo(model);
            switch (model.ReportName)
            {
                case "FinishLeatherQcDetail":
                    rptPath = rptPath + "/FinishLeatherQcDetail.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("BuyerName", model.BuyerName ?? "");
                    reportDocument.SetParameterValue("BuyerOrderNo", model.BuyerOrderNo ?? "");
                    reportDocument.SetParameterValue("ArticleChallanNo", model.ArticleChallanNo ?? "");
                    reportDocument.SetParameterValue("ArticleName", model.ArticleName ?? "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
                case "BuyerWiseColorCrustLeatherRequisition":

                    rptPath = rptPath + "/BuyerWiseColorCrustLeatherRequisition.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("BuyerName", model.BuyerName ?? "");
                    reportDocument.SetParameterValue("BuyerOrderNo", model.BuyerOrderNo ?? "");
                    reportDocument.SetParameterValue("ArticleChallanNo", model.ArticleChallanNo ?? "");
                    reportDocument.SetParameterValue("ArticleName", model.ArticleName ?? "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
                case "ArticleWiseColorCrustLeatherRequisition":

                    rptPath = rptPath + "/ArticleWiseColorCrustLeatherRequisition.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("BuyerName", model.BuyerName ?? "");
                    reportDocument.SetParameterValue("BuyerOrderNo", model.BuyerOrderNo ?? "");
                    reportDocument.SetParameterValue("ArticleChallanNo", model.ArticleChallanNo ?? "");
                    reportDocument.SetParameterValue("ArticleName", model.ArticleName ?? "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;


            }
            reportDocument.ExportToHttpResponse(
               model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord,
               System.Web.HttpContext.Current.Response, false, "crReport");
            return View();
        }

        [CheckUserAccess("Reports/CrustIssueAfterQcRpt")]
        public ActionResult CrustIssueAfterQcRpt()
        {
            //ViewBag.Store = objStore.GetAllActiveFinishProductionStore();
            return View();
        }
        [HttpPost]
        public ActionResult CrustIssueAfterQcRpt(ExpPiOrderReportModel model)
        {
            //ViewBag.Store = objStore.GetAllActiveFinishProductionStore();
            ReportDocument reportDocument = new ReportDocument();
            string rptPath = Server.MapPath("~/Reports");

            var reportObj = new DalReport();
            DataTable dataTableObj = reportObj.GetCrustIssueAfterQcRptInfo(model);
            switch (model.ReportName)
            {

                case "IssueAfterCrustLeatherQcDetail":

                    rptPath = rptPath + "/IssueAfterCrustLeatherQcDetail.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("BuyerName", model.BuyerName ?? "");
                    reportDocument.SetParameterValue("BuyerOrderNo", model.BuyerOrderNo ?? "");
                    reportDocument.SetParameterValue("ArticleChallanNo", model.ArticleChallanNo ?? "");
                    reportDocument.SetParameterValue("ArticleName", model.ArticleName ?? "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
                case "BuyerWiseColorCrustLeatherRequisition":

                    rptPath = rptPath + "/BuyerWiseColorCrustLeatherRequisition.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("BuyerName", model.BuyerName ?? "");
                    reportDocument.SetParameterValue("BuyerOrderNo", model.BuyerOrderNo ?? "");
                    reportDocument.SetParameterValue("ArticleChallanNo", model.ArticleChallanNo ?? "");
                    reportDocument.SetParameterValue("ArticleName", model.ArticleName ?? "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
                case "ArticleWiseColorCrustLeatherRequisition":

                    rptPath = rptPath + "/ArticleWiseColorCrustLeatherRequisition.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("BuyerName", model.BuyerName ?? "");
                    reportDocument.SetParameterValue("BuyerOrderNo", model.BuyerOrderNo ?? "");
                    reportDocument.SetParameterValue("ArticleChallanNo", model.ArticleChallanNo ?? "");
                    reportDocument.SetParameterValue("ArticleName", model.ArticleName ?? "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;


            }
            reportDocument.ExportToHttpResponse(
               model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord,
               System.Web.HttpContext.Current.Response, false, "crReport");
            return View();
        }


        [CheckUserAccess("Reports/CrustLeatherQcRpt")]
        public ActionResult CrustLeatherQcRpt()
        {
            //ViewBag.Store = objStore.GetAllActiveFinishProductionStore();
            return View();
        }
        [HttpPost]
        public ActionResult CrustLeatherQcRpt(ExpPiOrderReportModel model)
        {
            //ViewBag.Store = objStore.GetAllActiveFinishProductionStore();
            ReportDocument reportDocument = new ReportDocument();
            string rptPath = Server.MapPath("~/Reports");

            var reportObj = new DalReport();
            DataTable dataTableObj = reportObj.GetCrustLeatherQcRptInfo(model);
            switch (model.ReportName)
            {

                case "CrustLeatherQcDetail":
                    rptPath = rptPath + "/CrustLeatherQcDetail.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("BuyerName", model.BuyerName ?? "");
                    reportDocument.SetParameterValue("BuyerOrderNo", model.BuyerOrderNo ?? "");
                    reportDocument.SetParameterValue("ArticleChallanNo", model.ArticleChallanNo ?? "");
                    reportDocument.SetParameterValue("ArticleName", model.ArticleName ?? "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
                case "BuyerWiseColorCrustLeatherRequisition":

                    rptPath = rptPath + "/BuyerWiseColorCrustLeatherRequisition.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("BuyerName", model.BuyerName ?? "");
                    reportDocument.SetParameterValue("BuyerOrderNo", model.BuyerOrderNo ?? "");
                    reportDocument.SetParameterValue("ArticleChallanNo", model.ArticleChallanNo ?? "");
                    reportDocument.SetParameterValue("ArticleName", model.ArticleName ?? "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
                case "ArticleWiseColorCrustLeatherRequisition":

                    rptPath = rptPath + "/ArticleWiseColorCrustLeatherRequisition.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("BuyerName", model.BuyerName ?? "");
                    reportDocument.SetParameterValue("BuyerOrderNo", model.BuyerOrderNo ?? "");
                    reportDocument.SetParameterValue("ArticleChallanNo", model.ArticleChallanNo ?? "");
                    reportDocument.SetParameterValue("ArticleName", model.ArticleName ?? "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;


            }
            reportDocument.ExportToHttpResponse(
               model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord,
               System.Web.HttpContext.Current.Response, false, "crReport");
            return View();
        }

        [CheckUserAccess("Reports/BuyerOrderRpt")]
        public ActionResult BuyerOrderRpt()
        {
            ViewBag.Store = objStore.GetAllActiveFinishProductionStore();
            return View();
        }
        [HttpPost]
        public ActionResult BuyerOrderRpt(ExpPiOrderReportModel model)
        {
            ReportDocument reportDocument = new ReportDocument();
            string rptPath = Server.MapPath("~/Reports");

            var reportObj = new DalReport();
            DataTable dataTableObj = reportObj.GetExpBuyerOrderRptInfo(model);
            switch (model.ReportName)
            {

                case "ExpBuyerOrderSingleDetail":

                    rptPath = rptPath + "/ExpBuyerOrderSingleDetail.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    //reportDocument.SetParameterValue("BuyerName", model.BuyerName ?? "");
                    ////reportDocument.SetParameterValue("BuyerOrderNo", model.BuyerOrderNo ?? "");
                    //reportDocument.SetParameterValue("ArticleName", model.ArticleName ?? "");

                    //reportDocument.SetParameterValue("FromDate1",
                    //model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    //reportDocument.SetParameterValue("ToDate1",
                    //model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
                case "ExpAllBuyerOrderColorLeverWiseDetail":

                    rptPath = rptPath + "/ExpAllBuyerOrderColorLeverWiseDetail.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("BuyerName", model.BuyerName ?? "");
                    //reportDocument.SetParameterValue("BuyerOrderNo", model.BuyerOrderNo ?? "");
                    reportDocument.SetParameterValue("ArticleName", model.ArticleName ?? "");
                    reportDocument.SetParameterValue("Item",
                        model.ItemTypeID != null
                            ? _repository.SysItemTypeRepository.GetByID(Convert.ToInt32(model.ItemTypeID != null
                        )).ItemTypeName
                            : "");
                    reportDocument.SetParameterValue("Color",
                        model.ColorID != null ? _repository.SysColorRepository.GetByID(Convert.ToByte(model.ColorID)).ColorName : "");
                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
                case "ExpAllBuyerOrderArticleLevelWiseDetail":

                    rptPath = rptPath + "/ExpAllBuyerOrderArticleLevelWiseDetail.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("BuyerName", model.BuyerName ?? "");
                    //reportDocument.SetParameterValue("BuyerOrderNo", model.BuyerOrderNo ?? "");
                    reportDocument.SetParameterValue("ArticleName", model.ArticleName ?? "");
                    reportDocument.SetParameterValue("Item",
                        model.ItemTypeID != null
                            ? _repository.SysItemTypeRepository.GetByID(Convert.ToInt32(model.ItemTypeID != null
                        )).ItemTypeName
                            : "");
                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;

            }
            reportDocument.ExportToHttpResponse(
               model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord,
               System.Web.HttpContext.Current.Response, false, "crReport");
            return View();
        }


        [CheckUserAccess("Reports/ExpPiRpt")]
        public ActionResult ExpPiRpt()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ExpPiRpt(ExpPiOrderReportModel model)
        {
            ReportDocument reportDocument = new ReportDocument();
            string rptPath = Server.MapPath("~/Reports");

            var reportObj = new DalReport();
            DataTable dataTableObj = reportObj.GetExpPiRptInfo(model);
            switch (model.ReportName)
            {

                case "ExpPISingleDetail":

                    rptPath = rptPath + "/ExpPISingleDetail.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("PINo", model.PINo ?? "");
                    reportDocument.SetParameterValue("BuyerName", model.BuyerName ?? "");
                    //reportDocument.SetParameterValue("BuyerOrderNo", model.BuyerOrderNo ?? "");
                    reportDocument.SetParameterValue("ArticleName", model.ArticleName ?? "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
                case "ExpAllPIColorLeverWiseDetail":

                    rptPath = rptPath + "/ExpAllPIColorLeverWiseDetail.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    //reportDocument.SetParameterValue("PINo", model.PINo ?? "");
                    reportDocument.SetParameterValue("BuyerName", model.BuyerName ?? "");
                    //reportDocument.SetParameterValue("BuyerOrderNo", model.BuyerOrderNo ?? "");
                    reportDocument.SetParameterValue("ArticleName", model.ArticleName ?? "");
                    reportDocument.SetParameterValue("Item",
                       model.ItemTypeID != null
                           ? _repository.SysItemTypeRepository.GetByID(Convert.ToInt32(model.ItemTypeID != null
                       )).ItemTypeName
                           : "");
                    reportDocument.SetParameterValue("Color",
                        model.ColorID != null ? _repository.SysColorRepository.GetByID(Convert.ToByte(model.ColorID)).ColorName : "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
                case "ExpAllPIArticleLevelWiseDetail":

                    rptPath = rptPath + "/ExpAllPIArticleLevelWiseDetail.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    //reportDocument.SetParameterValue("PINo", model.PINo ?? "");
                    reportDocument.SetParameterValue("BuyerName", model.BuyerName ?? "");
                    //reportDocument.SetParameterValue("BuyerOrderNo", model.BuyerOrderNo ?? "");
                    reportDocument.SetParameterValue("ArticleName", model.ArticleName ?? "");
                    reportDocument.SetParameterValue("Item",
                        model.ItemTypeID != null
                            ? _repository.SysItemTypeRepository.GetByID(Convert.ToInt32(model.ItemTypeID != null
                        )).ItemTypeName
                            : "");
                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;

            }
            reportDocument.ExportToHttpResponse(
               model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord,
               System.Web.HttpContext.Current.Response, false, "crReport");
            return View();
        }


        [CheckUserAccess("Reports/CrustLeatherRequisitionRpt")]
        public ActionResult CrustLeatherRequisitionRpt()
        {
            ViewBag.Store = objStore.GetAllActiveFinishProductionStore();
            return View();
        }
        [HttpPost]
        public ActionResult CrustLeatherRequisitionRpt(ReportModel model)
        {
            ViewBag.Store = objStore.GetAllActiveFinishProductionStore();
            ReportDocument reportDocument = new ReportDocument();
            string rptPath = Server.MapPath("~/Reports");

            var reportObj = new DalReport();
            DataTable dataTableObj = reportObj.GetCrustLeatherRequisitionInfo(model);
            switch (model.ReportName)
            {

                case "CrustLeatherRequisitionDetail":

                    rptPath = rptPath + "/CrustLeatherRequisitionDetail.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("BuyerName", model.BuyerName ?? "");
                    reportDocument.SetParameterValue("ArticleChallanNo", model.ArticleChallanNo ?? "");
                    reportDocument.SetParameterValue("ArticleName", model.ArticleName ?? "");
                    reportDocument.SetParameterValue("StoreName",
                        model.StoreID != null ? _repository.StoreRepository.GetByID(Convert.ToByte(model.StoreID)).StoreName : "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
                case "BuyerWiseColorCrustLeatherRequisition":

                    rptPath = rptPath + "/BuyerWiseColorCrustLeatherRequisition.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("BuyerName", model.BuyerName ?? "");
                    reportDocument.SetParameterValue("ArticleChallanNo", model.ArticleChallanNo ?? "");
                    reportDocument.SetParameterValue("ArticleName", model.ArticleName ?? "");
                    reportDocument.SetParameterValue("StoreName",
                        model.StoreID != null ? _repository.StoreRepository.GetByID(Convert.ToByte(model.StoreID)).StoreName : "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
                case "ArticleWiseColorCrustLeatherRequisition":

                    rptPath = rptPath + "/ArticleWiseColorCrustLeatherRequisition.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("BuyerName", model.BuyerName ?? "");
                    reportDocument.SetParameterValue("ArticleChallanNo", model.ArticleChallanNo ?? "");
                    reportDocument.SetParameterValue("ArticleName", model.ArticleName ?? "");
                    reportDocument.SetParameterValue("StoreName",
                        model.StoreID != null ? _repository.StoreRepository.GetByID(Convert.ToByte(model.StoreID)).StoreName : "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;


            }
            reportDocument.ExportToHttpResponse(
               model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord,
               System.Web.HttpContext.Current.Response, false, "crReport");
            return View();
        }

        [CheckUserAccess("Reports/ChemLoanRcvReqRpt")]
        public ActionResult ChemLoanRcvReqRpt()
        {
            ViewBag.Store = objStore.GetAllActiveChemicalStore();
            return View();
        }
        [HttpPost]
        public ActionResult ChemLoanRcvReqRpt(ReportModel model)
        {
            ViewBag.Store = objStore.GetAllActiveChemicalStore();
            ReportDocument reportDocument = new ReportDocument();
            string rptPath = Server.MapPath("~/Reports");

            var reportObj = new DalReport();
            DataTable dataTableObj = reportObj.GetChemLoanReceiveRequestInfo(model);
            switch (model.ReportName)
            {
                case "ChemLoanReceiveRequestDetail":
                    rptPath = rptPath + "/ChemLoanReceiveRequestDetail.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("SupplierName", model.SupplierName ?? "");
                    reportDocument.SetParameterValue("ItemName",
                        model.ItemID != null
                            ? _repository.SysChemicalItemRepository.GetByID(Convert.ToInt32(model.ItemID)).ItemName
                            : "");
                    reportDocument.SetParameterValue("StoreName",
                        model.StoreID != null ? _repository.StoreRepository.GetByID(Convert.ToByte(model.StoreID)).StoreName : "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
                case "StoreWiseChemLoanReceiveRequestSummary":

                    rptPath = rptPath + "/StoreWiseChemLoanReceiveRequestSummary.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("SupplierName", model.SupplierName ?? "");
                    reportDocument.SetParameterValue("ItemName",
                        model.ItemID != null
                            ? _repository.SysChemicalItemRepository.GetByID(Convert.ToInt32(model.ItemID)).ItemName
                            : "");
                    reportDocument.SetParameterValue("StoreName",
                        model.StoreID != null ? _repository.StoreRepository.GetByID(Convert.ToByte(model.StoreID)).StoreName : "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;

            }
            reportDocument.ExportToHttpResponse(
               model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord,
               System.Web.HttpContext.Current.Response, false, "crReport");
            return View();
        }

        [CheckUserAccess("Reports/ForeignChemPurcRpt")]
        public ActionResult ForeignChemPurcRpt()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForeignChemPurcRpt(ReportModel model)
        {
            var reportDocument = new ReportDocument();
            var rptPath = Server.MapPath("~/Reports");
            var dataTableObj = new DataTable();
            var reportObj = new DalReport();
            dataTableObj = reportObj.LocalChemPurcOrderInfo(model, "FPO");
            switch (model.ReportName)
            {
                case "ChemPurchaseOrderWiseDetail":
                    rptPath = rptPath + "/ChemPurchaseOrderWiseDetail.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("RptTitle", "Foreign Chemical Purchase Order Wise Detail");
                    reportDocument.SetParameterValue("SupplierName", model.SupplierName ?? "");
                    reportDocument.SetParameterValue("ItemName",
                        model.ItemID != null
                            ? _repository.SysChemicalItemRepository.GetByID(Convert.ToInt32(model.ItemID)).ItemName
                            : "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
                case "ChemPurchaseChemicalWiseSummary":
                    rptPath = rptPath + "/ChemPurchaseOrderChemicalWiseSummary.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("RptTitle", "Chemical Name Wise Foreign Chemical Purchase Order Summary");
                    reportDocument.SetParameterValue("SupplierName", model.SupplierName ?? "");
                    reportDocument.SetParameterValue("ItemName",
                        model.ItemID != null
                            ? _repository.SysChemicalItemRepository.GetByID(Convert.ToInt32(model.ItemID)).ItemName
                            : "");
                    reportDocument.SetParameterValue("FromDate1",
                    model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
            }
            reportDocument.ExportToHttpResponse(
               model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord,
               System.Web.HttpContext.Current.Response, false, "crReport");
            return View();
        }

        [CheckUserAccess("Reports/LocalChemPurcRpt")]
        public ActionResult LocalChemPurcRpt()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LocalChemPurcRpt(ReportModel model)
        {
            var reportDocument = new ReportDocument();
            var rptPath = Server.MapPath("~/Reports");
            var dataTableObj = new DataTable();
            var reportObj = new DalReport();
            dataTableObj = reportObj.LocalChemPurcOrderInfo(model, "LPO");
            switch (model.ReportName)
            {
                case "ChemPurchaseOrderWiseDetail":
                    rptPath = rptPath + "/ChemPurchaseOrderWiseDetail.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("RptTitle", "Local Chemical Purchase Order Wise Detail");
                    reportDocument.SetParameterValue("SupplierName", model.SupplierName ?? "");
                    reportDocument.SetParameterValue("ItemName",
                        model.ItemID != null
                            ? _repository.SysChemicalItemRepository.GetByID(Convert.ToInt32(model.ItemID)).ItemName
                            : "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
                case "ChemPurchaseChemicalWiseSummary":
                    rptPath = rptPath + "/ChemPurchaseOrderChemicalWiseSummary.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("RptTitle", "Local Name Wise Foreign Chemical Purchase Order Summary");
                    reportDocument.SetParameterValue("SupplierName", model.SupplierName ?? "");
                    reportDocument.SetParameterValue("ItemName",
                        model.ItemID != null
                            ? _repository.SysChemicalItemRepository.GetByID(Convert.ToInt32(model.ItemID)).ItemName
                            : "");
                    reportDocument.SetParameterValue("FromDate1",
                    model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
            }
            reportDocument.ExportToHttpResponse(
               model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord,
               System.Web.HttpContext.Current.Response, false, "crReport");
            return View();
        }


        [CheckUserAccess("Reports/ChemPurcRequisition")]
        public ActionResult ChemPurcRequisition()
        {
            ViewBag.Store = objStore.GetAllActiveChemicalStore();
            ViewBag.ItemType = objItemType.GetAllActiveItemTypeLeather();
            return View();
        }
        [HttpPost]
        public ActionResult ChemPurcRequisition(ReportModel model)
        {
            ViewBag.Store = objStore.GetAllActiveChemicalStore();
            ViewBag.ItemType = objItemType.GetAllActiveItemTypeLeather();

            var reportDocument = new ReportDocument();
            var rptPath = Server.MapPath("~/Reports");
            var dataTableObj = new DataTable();
            var reportObj = new DalReport();
            dataTableObj = reportObj.GetChemPurcRequisitionInfo(model);
            switch (model.ReportName)
            {
                case "ChemicalRequisitionWiseItems":
                    rptPath = rptPath + "/ChemicalRequisitionWiseItems.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("SupplierName", model.SupplierName ?? "");
                    reportDocument.SetParameterValue("ItemName",
                        model.ItemID != null
                            ? _repository.SysChemicalItemRepository.GetByID(Convert.ToInt32(model.ItemID)).ItemName
                            : "");
                    reportDocument.SetParameterValue("StoreName",
                        model.StoreID != null ? _repository.StoreRepository.GetByID(Convert.ToByte(model.StoreID)).StoreName : "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
                case "StoreWiseChemicalRequisition":
                    rptPath = rptPath + "/StoreWiseChemicalRequisition.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("SupplierName", model.SupplierName ?? "");
                    reportDocument.SetParameterValue("ItemName",
                        model.ItemID != null
                            ? _repository.SysChemicalItemRepository.GetByID(Convert.ToInt32(model.ItemID)).ItemName
                            : "");
                    reportDocument.SetParameterValue("StoreName",
                        model.StoreID != null ? _repository.StoreRepository.GetByID(Convert.ToByte(model.StoreID)).StoreName : "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
            }
            reportDocument.ExportToHttpResponse(
               model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord,
               System.Web.HttpContext.Current.Response, false, "crReport");
            return View();
        }

        [CheckUserAccess("Reports/LocalChemPurcBill")]
        public ActionResult LocalChemPurcBill()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LocalChemPurcBill(ReportModel model)
        {
            var reportDocument = new ReportDocument();
            var rptPath = Server.MapPath("~/Reports");
            var dataTableObj = new DataTable();
            var reportObj = new DalReport();
            dataTableObj = reportObj.LocalChemPurcBillInfo(model);
            switch (model.ReportName)
            {
                case "SupplierWiseChemLocalBill":
                    rptPath = rptPath + "/SupplierWiseChemLocalBill.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("SupplierName", model.SupplierName ?? "");
                    reportDocument.SetParameterValue("ItemName",
                        model.ItemID != null
                            ? _repository.SysChemicalItemRepository.GetByID(Convert.ToInt32(model.ItemID)).ItemName
                            : "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
                case "ChemicalWiseCrustConsumptionSummary":
                    rptPath = rptPath + "/ChemicalWiseCrustChemConsumption.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("SupplierName", model.SupplierName ?? "");
                    reportDocument.SetParameterValue("ItemName",
                        model.ItemID != null
                            ? _repository.SysChemicalItemRepository.GetByID(Convert.ToInt32(model.ItemID)).ItemName
                            : "");
                    reportDocument.SetParameterValue("FromDate1",
                    model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
            }
            reportDocument.ExportToHttpResponse(
               model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord,
               System.Web.HttpContext.Current.Response, false, "crReport");
            return View();
        }


        [CheckUserAccess("Reports/FinishedChemConsumeRpt")]
        public ActionResult FinishedChemConsumeRpt()
        {
            ViewBag.Store = objStore.GetAllActiveFinishProductionStore();
            //ViewBag.ItemType = objItemType.GetAllActiveItemTypeLeather();
            return View();
        }
        [HttpPost]
        public ActionResult FinishedChemConsumeRpt(ReportModel model)
        {
            ViewBag.Store = objStore.GetAllActiveFinishProductionStore();
            //ViewBag.ItemType = objItemType.GetAllActiveItemTypeLeather();
            var reportDocument = new ReportDocument();
            var rptPath = Server.MapPath("~/Reports");
            var dataTableObj = new DataTable();
            var reportObj = new DalReport();
            dataTableObj = reportObj.GetFinishedChemConsumptionInfo(model);
            switch (model.ReportName)
            {
                case "SchWiseFndChemConsumptionSummary":
                    rptPath = rptPath + "/SchWiseFndChemConsumptionSummary.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("SupplierName", model.SupplierName ?? "");
                    reportDocument.SetParameterValue("ItemName",
                        model.ItemID != null
                            ? _repository.SysChemicalItemRepository.GetByID(Convert.ToInt32(model.ItemID)).ItemName
                            : "");
                    reportDocument.SetParameterValue("StoreName",
                        model.StoreID != null ? _repository.StoreRepository.GetByID(Convert.ToByte(model.StoreID)).StoreName : "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
                case "ChemicalWiseFndConsumptionSummary":
                    rptPath = rptPath + "/ChemicalWiseFndConsumptionSummary.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("SupplierName", model.SupplierName ?? "");
                    reportDocument.SetParameterValue("ItemName",
                        model.ItemID != null
                            ? _repository.SysChemicalItemRepository.GetByID(Convert.ToInt32(model.ItemID)).ItemName
                            : "");
                    reportDocument.SetParameterValue("StoreName",
                        model.StoreID != null ? _repository.StoreRepository.GetByID(Convert.ToByte(model.StoreID)).StoreName : "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
                case "ChemicalWiseFndConsumptionSummaryWithoutSch":
                    rptPath = rptPath + "/ChemicalWiseFndConsumptionSummaryWithoutSch.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("SupplierName", model.SupplierName ?? "");
                    reportDocument.SetParameterValue("ItemName",
                        model.ItemID != null
                            ? _repository.SysChemicalItemRepository.GetByID(Convert.ToInt32(model.ItemID)).ItemName
                            : "");
                    reportDocument.SetParameterValue("StoreName",
                        model.StoreID != null ? _repository.StoreRepository.GetByID(Convert.ToByte(model.StoreID)).StoreName : "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;

            }
            reportDocument.ExportToHttpResponse(
               model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord,
               System.Web.HttpContext.Current.Response, false, "crReport");
            return View();
        }


        [CheckUserAccess("Reports/FinishedProdRpt")]
        public ActionResult FinishedProdRpt()
        {
            ViewBag.Store = objStore.GetAllActiveFinishProductionStore();
            return View();
        }
        [HttpPost]
        public ActionResult FinishedProdRpt(CrustReportModel model)
        {
            ViewBag.Store = objStore.GetAllActiveFinishProductionStore();
            var reportDocument = new ReportDocument();
            var rptPath = Server.MapPath("~/Reports");
            var dataTableObj = new DataTable();
            var reportObj = new DalReport();
            dataTableObj = reportObj.GetFinishedProductionInfo(model);
            switch (model.ReportName)
            {
                case "ChallanWiseFndProdSummary":
                    rptPath = rptPath + "/ChallanWiseFndProd.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("StoreName",
                      model.StoreID != null
                          ? _repository.StoreRepository.GetByID(Convert.ToInt32(model.StoreID)).StoreName
                          : "");
                    reportDocument.SetParameterValue("ProductionStatus1", DalCommon.ReturnProductionStatus(model.ProductionStatus ?? ""));
                    reportDocument.SetParameterValue("ScheduleYear1", model.ScheduleYear ?? "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
                case "ArticleWiseFndProdSummary":
                    rptPath = rptPath + "/ArticleWiseFndProd.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("StoreName",
                       model.StoreID != null
                           ? _repository.StoreRepository.GetByID(Convert.ToInt32(model.StoreID)).StoreName
                           : "");
                    reportDocument.SetParameterValue("ProductionStatus1", DalCommon.ReturnProductionStatus(model.ProductionStatus ?? ""));
                    reportDocument.SetParameterValue("ScheduleYear1", model.ScheduleYear ?? "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
                case "MonthWiseFndProdSummary":
                    rptPath = rptPath + "/MonthWiseFndProd.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("StoreName",
                      model.StoreID != null
                          ? _repository.StoreRepository.GetByID(Convert.ToInt32(model.StoreID)).StoreName
                          : "");
                    reportDocument.SetParameterValue("ProductionStatus1", DalCommon.ReturnProductionStatus(model.ProductionStatus ?? ""));
                    reportDocument.SetParameterValue("ScheduleYear1", model.ScheduleYear ?? "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;

                case "ScheduleDateWiseFndProdStatus":
                    rptPath = rptPath + "/ScheduleDateWiseFndProdDetail.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("StoreName",
                      model.StoreID != null
                          ? _repository.StoreRepository.GetByID(Convert.ToInt32(model.StoreID)).StoreName
                          : "");
                    reportDocument.SetParameterValue("ProductionStatus1", DalCommon.ReturnProductionStatus(model.ProductionStatus ?? ""));
                    reportDocument.SetParameterValue("ScheduleYear1", model.ScheduleYear ?? "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
                case "SchWiseFndProdSummary":
                    rptPath = rptPath + "/SchWiseFndProdSummary.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("StoreName",
                      model.StoreID != null
                          ? _repository.StoreRepository.GetByID(Convert.ToInt32(model.StoreID)).StoreName
                          : "");
                    reportDocument.SetParameterValue("ProductionStatus1", DalCommon.ReturnProductionStatus(model.ProductionStatus ?? ""));
                    reportDocument.SetParameterValue("ScheduleYear1", model.ScheduleYear ?? "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
                case "FndProdStatusWise":
                    rptPath = rptPath + "/FndProdStatusWise.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("StoreName",
                      model.StoreID != null
                          ? _repository.StoreRepository.GetByID(Convert.ToInt32(model.StoreID)).StoreName
                          : "");
                    reportDocument.SetParameterValue("ProductionStatus1", DalCommon.ReturnProductionStatus(model.ProductionStatus ?? ""));
                    reportDocument.SetParameterValue("ScheduleYear1", model.ScheduleYear ?? "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;

                case "DateWiseFndStoreTransfer":
                    rptPath = rptPath + "/DateWiseFndStoreTransfer.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("StoreName",
                       model.StoreID != null
                           ? _repository.StoreRepository.GetByID(Convert.ToInt32(model.StoreID)).StoreName
                           : "");
                    reportDocument.SetParameterValue("ProductionStatus1", DalCommon.ReturnProductionStatus(model.ProductionStatus ?? ""));
                    reportDocument.SetParameterValue("ScheduleYear1", model.ScheduleYear ?? "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
            }
            reportDocument.ExportToHttpResponse(
               model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord,
               System.Web.HttpContext.Current.Response, false, "crReport");
            return View();
        }

        [CheckUserAccess("Reports/CrustChemConsumeRpt")]
        public ActionResult CrustChemConsumeRpt()
        {
            ViewBag.Store = objStore.GetAllActiveWetBlueProductionStore();
            ViewBag.ItemType = objItemType.GetAllActiveItemTypeLeather();
            return View();
        }
        [HttpPost]
        public ActionResult CrustChemConsumeRpt(ReportModel model)
        {
            ViewBag.Store = objStore.GetAllActiveWetBlueProductionStore();
            ViewBag.ItemType = objItemType.GetAllActiveItemTypeLeather();
            var reportDocument = new ReportDocument();
            var rptPath = Server.MapPath("~/Reports");
            var dataTableObj = new DataTable();
            var reportObj = new DalReport();
            dataTableObj = reportObj.GetCrustChemConsumptionInfo(model);
            switch (model.ReportName)
            {
                case "CrustSchWiseChemConsumptionSummary":
                    rptPath = rptPath + "/SchWiseCrustChemConsumption.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("SupplierName", model.SupplierName ?? "");
                    reportDocument.SetParameterValue("ItemName",
                        model.ItemID != null
                            ? _repository.SysChemicalItemRepository.GetByID(Convert.ToInt32(model.ItemID)).ItemName
                            : "");
                    reportDocument.SetParameterValue("StoreName",
                        model.StoreID != null ? _repository.StoreRepository.GetByID(Convert.ToByte(model.StoreID)).StoreName : "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
                case "ChemicalWiseCrustConsumptionSummary":
                    rptPath = rptPath + "/ChemicalWiseCrustChemConsumption.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("SupplierName", model.SupplierName ?? "");
                    reportDocument.SetParameterValue("ItemName",
                        model.ItemID != null
                            ? _repository.SysChemicalItemRepository.GetByID(Convert.ToInt32(model.ItemID)).ItemName
                            : "");
                    reportDocument.SetParameterValue("StoreName",
                        model.StoreID != null ? _repository.StoreRepository.GetByID(Convert.ToByte(model.StoreID)).StoreName : "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
            }
            reportDocument.ExportToHttpResponse(
               model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord,
               System.Web.HttpContext.Current.Response, false, "crReport");
            return View();
        }


        [CheckUserAccess("Reports/CrustProdRpt")]
        public ActionResult CrustProdRpt()
        {
            ViewBag.Store = objStore.GetAllActiveWetBlueProductionStore();
            return View();
        }
        [HttpPost]
        public ActionResult CrustProdRpt(CrustReportModel model)
        {
            ViewBag.Store = objStore.GetAllActiveWetBlueProductionStore();
            var reportDocument = new ReportDocument();
            var rptPath = Server.MapPath("~/Reports");
            var dataTableObj = new DataTable();
            var reportObj = new DalReport();
            dataTableObj = reportObj.GetCrustProductionInfo(model);
            switch (model.ReportName)
            {
                case "ChallanWiseCrustProdSummary":
                    rptPath = rptPath + "/ChallanWiseCrustProd.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("StoreName",
                      model.StoreID != null
                          ? _repository.StoreRepository.GetByID(Convert.ToInt32(model.StoreID)).StoreName
                          : "");
                    reportDocument.SetParameterValue("ProductionStatus1", DalCommon.ReturnProductionStatus(model.ProductionStatus ?? ""));
                    reportDocument.SetParameterValue("ScheduleYear1", model.ScheduleYear ?? "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
                case "ChallanWiseArticleCrustProdSummary":
                    rptPath = rptPath + "/ChallanWiseArticleCrustProd.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("StoreName",
                      model.StoreID != null
                          ? _repository.StoreRepository.GetByID(Convert.ToInt32(model.StoreID)).StoreName
                          : "");
                    reportDocument.SetParameterValue("ProductionStatus1", DalCommon.ReturnProductionStatus(model.ProductionStatus ?? ""));
                    reportDocument.SetParameterValue("ScheduleYear1", model.ScheduleYear ?? "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
                case "ArticleWiseCrustProdSummary":
                    rptPath = rptPath + "/ArticleWiseCrustProd.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("StoreName",
                       model.StoreID != null
                           ? _repository.StoreRepository.GetByID(Convert.ToInt32(model.StoreID)).StoreName
                           : "");
                    reportDocument.SetParameterValue("ProductionStatus1", DalCommon.ReturnProductionStatus(model.ProductionStatus ?? ""));
                    reportDocument.SetParameterValue("ScheduleYear1", model.ScheduleYear ?? "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
                case "ArticleWiseChallanCrustProdSummary":
                    rptPath = rptPath + "/ArticleWiseChallanCrustProd.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("StoreName",
                       model.StoreID != null
                           ? _repository.StoreRepository.GetByID(Convert.ToInt32(model.StoreID)).StoreName
                           : "");
                    reportDocument.SetParameterValue("ProductionStatus1", DalCommon.ReturnProductionStatus(model.ProductionStatus ?? ""));
                    reportDocument.SetParameterValue("ScheduleYear1", model.ScheduleYear ?? "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
                case "MonthWiseCrustProdSummary":
                    rptPath = rptPath + "/MonthWiseCrustProd.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("StoreName",
                      model.StoreID != null
                          ? _repository.StoreRepository.GetByID(Convert.ToInt32(model.StoreID)).StoreName
                          : "");
                    reportDocument.SetParameterValue("ProductionStatus1", DalCommon.ReturnProductionStatus(model.ProductionStatus ?? ""));
                    reportDocument.SetParameterValue("ScheduleYear1", model.ScheduleYear ?? "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;

                case "ScheduleDateWiseCrustProdStatus":
                    rptPath = rptPath + "/ScheduleDateWiseCrustProdDetail.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("StoreName",
                      model.StoreID != null
                          ? _repository.StoreRepository.GetByID(Convert.ToInt32(model.StoreID)).StoreName
                          : "");
                    reportDocument.SetParameterValue("ProductionStatus1", DalCommon.ReturnProductionStatus(model.ProductionStatus ?? ""));
                    reportDocument.SetParameterValue("ScheduleYear1", model.ScheduleYear ?? "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
                case "SchWiseCrustProdSummary":
                    rptPath = rptPath + "/SchWiseCrustProdSummary.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("StoreName",
                      model.StoreID != null
                          ? _repository.StoreRepository.GetByID(Convert.ToInt32(model.StoreID)).StoreName
                          : "");
                    reportDocument.SetParameterValue("ProductionStatus1", DalCommon.ReturnProductionStatus(model.ProductionStatus ?? ""));
                    reportDocument.SetParameterValue("ScheduleYear1", model.ScheduleYear ?? "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
                case "CrustProdStatusWise":
                    rptPath = rptPath + "/CrustProdStatusWise.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("StoreName",
                      model.StoreID != null
                          ? _repository.StoreRepository.GetByID(Convert.ToInt32(model.StoreID)).StoreName
                          : "");
                    reportDocument.SetParameterValue("ProductionStatus1", DalCommon.ReturnProductionStatus(model.ProductionStatus ?? ""));
                    reportDocument.SetParameterValue("ScheduleYear1", model.ScheduleYear ?? "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;

                case "DateWiseCrustStoreTransfer":
                    rptPath = rptPath + "/DateWiseCrustStoreTransfer.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("StoreName",
                       model.StoreID != null
                           ? _repository.StoreRepository.GetByID(Convert.ToInt32(model.StoreID)).StoreName
                           : "");
                    reportDocument.SetParameterValue("ProductionStatus1", DalCommon.ReturnProductionStatus(model.ProductionStatus ?? ""));
                    reportDocument.SetParameterValue("ScheduleYear1", model.ScheduleYear ?? "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
            }
            reportDocument.ExportToHttpResponse(
               model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord,
               System.Web.HttpContext.Current.Response, false, "crReport");
            return View();
        }


        [CheckUserAccess("Reports/WbChemConsumeReport")]
        public ActionResult WbChemConsumeReport()
        {
            ViewBag.Store = objStore.GetAllActiveProductionStore();
            ViewBag.ItemType = objItemType.GetAllActiveItemTypeLeather();
            return View();
        }
        [HttpPost]
        public ActionResult WbChemConsumeReport(ReportModel model)
        {
            ViewBag.Store = objStore.GetAllActiveProductionStore();
            ViewBag.ItemType = objItemType.GetAllActiveItemTypeLeather();
            var reportDocument = new ReportDocument();
            var rptPath = Server.MapPath("~/Reports");
            var dataTableObj = new DataTable();
            var reportObj = new DalReport();
            dataTableObj = reportObj.GetWetBlueChemConsumptionInfo(model);
            switch (model.ReportName)
            {
                case "WbSchWiseChemConsumptionSummary":
                    rptPath = rptPath + "/WbSchWiseChemConsumptionRpt.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("SupplierName", model.SupplierName ?? "");
                    reportDocument.SetParameterValue("ItemName",
                        model.ItemID != null
                            ? _repository.SysChemicalItemRepository.GetByID(Convert.ToInt32(model.ItemID)).ItemName
                            : "");
                    reportDocument.SetParameterValue("StoreName",
                        model.StoreID != null ? _repository.StoreRepository.GetByID(Convert.ToByte(model.StoreID)).StoreName : "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
                case "ChemicalWiseChemConsumptionSummary":
                    rptPath = rptPath + "/ChemicalWiseChemConsumptionSummary.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("SupplierName", model.SupplierName ?? "");
                    reportDocument.SetParameterValue("ItemName",
                        model.ItemID != null
                            ? _repository.SysChemicalItemRepository.GetByID(Convert.ToInt32(model.ItemID)).ItemName
                            : "");
                    reportDocument.SetParameterValue("StoreName",
                        model.StoreID != null ? _repository.StoreRepository.GetByID(Convert.ToByte(model.StoreID)).StoreName : "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
            }
            reportDocument.ExportToHttpResponse(
               model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord,
               System.Web.HttpContext.Current.Response, false, "crReport");
            return View();
        }


        [CheckUserAccess("Reports/WbProdReport")]
        public ActionResult WbProdReport()
        {
            ViewBag.Store = objStore.GetAllActiveProductionStore();
            ViewBag.ProcessList = _repository.SysProductionProces.Get().Where(a => a.IsActive.Equals(true) && a.ProcessCategory.Equals("WB")).ToList().Select(ob => new
            {
                ob.ProcessID,
                ob.ProcessName
            }).ToList();
            return View();
        }
        [HttpPost]
        public ActionResult WbProdReport(PurchaseReportModel model)
        {
            ViewBag.Store = objStore.GetAllActiveProductionStore();
            var reportDocument = new ReportDocument();
            var rptPath = Server.MapPath("~/Reports");
            var dataTableObj = new DataTable();
            var reportObj = new DalReport();
            dataTableObj = reportObj.GetWetBlueProductionInfo(model);
            switch (model.ReportName)
            {
                case "ProcessAndDrumWiseWBProdSummary":
                    rptPath = rptPath + "/ProcessAndDrumWiseWBProdSummaryRpt.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("ProcessName",
                        model.ItemTypeID != null
                            ? _repository.SysProductionProces.GetByID(Convert.ToInt32(model.ItemTypeID)).ProcessName
                            : "");
                    reportDocument.SetParameterValue("ProductionStatus", model.PurchaseType ?? "");
                    reportDocument.SetParameterValue("ScheduleYear", model.PurchaseYear ?? "");

                    reportDocument.SetParameterValue("FromDate1",
                    model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
                case "SupplierWiseWBProdStatus":
                    rptPath = rptPath + "/SupplierWiseWBProdStatus.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("SupplierName", model.SupplierName ?? "");
                    reportDocument.SetParameterValue("ScheduleYear", model.PurchaseYear ?? "");
                    reportDocument.SetParameterValue("FromDate1",
                    model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
                case "ProdStatusWiseWBProdSummary":
                    rptPath = rptPath + "/ProdStatusWiseWBProdSummary.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("SupplierName", model.SupplierName ?? "");
                    reportDocument.SetParameterValue("ScheduleYear", model.PurchaseYear ?? "");
                    reportDocument.SetParameterValue("FromDate1",
                    model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
                case "ProcessWiseWBProdSummary":
                    rptPath = rptPath + "/ProcessWiseWBProdSummaryRpt.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("ProcessName",
                       model.ItemTypeID != null
                           ? _repository.SysProductionProces.GetByID(Convert.ToInt32(model.ItemTypeID)).ProcessName
                           : "");
                    reportDocument.SetParameterValue("ProductionStatus", model.PurchaseType ?? "");
                    reportDocument.SetParameterValue("ScheduleYear", model.PurchaseYear ?? "");
                    reportDocument.SetParameterValue("FromDate1",
                    model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
                case "ScheduleDateWiseProdStatus":
                    rptPath = rptPath + "/ScheduleDateWiseProdStatus.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("SupplierName", model.SupplierName ?? "");
                    reportDocument.SetParameterValue("ScheduleYear", model.PurchaseYear ?? "");
                    reportDocument.SetParameterValue("FromDate1",
                    model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
                case "DateWiseWBStoreTransfer":
                    rptPath = rptPath + "/DateWiseWBStoreTransferRpt.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("ProductionStatus", model.PurchaseType ?? "");
                    reportDocument.SetParameterValue("ScheduleYear", model.PurchaseYear ?? "");
                    reportDocument.SetParameterValue("FromDate1",
                    model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
                case "MonthWiseWBProdStatus":
                    rptPath = rptPath + "/MonthWiseWBProdStatus.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("SupplierName", model.SupplierName ?? "");
                    reportDocument.SetParameterValue("ScheduleYear", model.PurchaseYear ?? "");
                    reportDocument.SetParameterValue("FromDate1",
                    model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
            }
            reportDocument.ExportToHttpResponse(
               model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord,
               System.Web.HttpContext.Current.Response, false, "crReport");
            return View();
        }
        [CheckUserAccess("Reports/ChemLcReport")]
        public ActionResult ChemLcReport()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChemLcReport(LcReportModel model)
        {
            ReportDocument reportDocument = new ReportDocument();
            string rptPath = Server.MapPath("~/Reports");
            var reportObj = new DalReport();
            var dataTableObj = reportObj.GetLcReportForChemCosting(model);
            switch (model.ReportName)
            {
                case "LcReportForChemCosting":

                    rptPath = rptPath + "/LcReportForChemCosting.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("LcNo", model.LcNo ?? "");
                    break;
                case "SupplierWiseChemicalLcCosting":
                    rptPath = rptPath + "/LcSupplierWiseChemicalCosting.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("LcNo", model.LcNo ?? "");
                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
                case "SupplierWiseChemLcCostingWithLcDate":
                    rptPath = rptPath + "/LcSupplierWiseChemLcCostingWithLcDate.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("LcNo", model.LcNo ?? "");
                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
                    
                case "ChemicalWiseSupplierLcCosting":
                    rptPath = rptPath + "/LcChemicalWiseSupplierCosting.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("LcNo", model.LcNo ?? "");
                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
                case "LcImportStatement":
                    rptPath = rptPath + "/LcImportStatementRpt.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    break;
                case "DeferredPaymentStatement":
                    rptPath = rptPath + "/LcDeferredPaymentStatement.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
                case "DeferredPaymentDueStatement":
                    rptPath = rptPath + "/LcDeferredPaymentDueStatement.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
                case "LcPaymentStatement":
                    rptPath = rptPath + "/LcPaymentStatement.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
                case "LcMonthWisePaymentStatement":
                    rptPath = rptPath + "/LcMonthWisePaymentStatement.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
                case "LcInsuranceBillStatement":
                    rptPath = rptPath + "/LcInsuranceBillStatement.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("FromDate1",
                    model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
            }
            reportDocument.ExportToHttpResponse(
               model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord,
               System.Web.HttpContext.Current.Response, false, "crReport");
            return View();
        }


        [CheckUserAccess("Reports/StockReport")]
        public ActionResult StockReport()
        {
            ViewBag.Store = objStore.GetAllActiveStore();
            ViewBag.ItemType = objItemType.GetAllActiveItemTypeLeather();
            ViewBag.LeatherType = objLeatherType.GetAllActiveLeatherType();
            return View();
        }

        [HttpPost]
        public ActionResult StockReport(ReportModel model)
        {
            ViewBag.Store = objStore.GetAllActiveStore();
            ViewBag.ItemType = objItemType.GetAllActiveItemTypeLeather();
            ViewBag.LeatherType = objLeatherType.GetAllActiveLeatherType();
            ReportDocument reportDocument = new ReportDocument();
            string rptPath = Server.MapPath("~/Reports");
            var reportObj = new DalReport();
            var dataTableObj = reportObj.GetLeatherStockInfo(model);
            switch (model.ReportName)
            {
                case "StoreWiseItemStock":
                    rptPath = rptPath + "/StoreWiseItemStock.rpt";
                    SetParamValueInReport(model, reportDocument, rptPath, dataTableObj);
                    break;
                case "ItemWiseStock":
                    rptPath = rptPath + "/ItemWiseStock.rpt";
                    SetParamValueInReport(model, reportDocument, rptPath, dataTableObj);
                    break;
                case "DailyStock":
                    rptPath = rptPath + "/EveryDateWiseStockReport.rpt";
                    SetParamValueInReport(model, reportDocument, rptPath, dataTableObj);
                    break;
                case "PurchaseNProdWiseDailyStock":
                    rptPath = rptPath + "/PurchaseNProdWiseDailyStockRpt.rpt";
                    SetParamValueInReport(model, reportDocument, rptPath, dataTableObj);
                    break;

                case "SupplierStock":
                    rptPath = rptPath + "/SupplierWiseStock.rpt";
                    SetParamValueInReport(model, reportDocument, rptPath, dataTableObj);
                    break;
                case "SupplierWiseAdjustRequest":
                    rptPath = rptPath + "/SupplierWiseAdjustRequest.rpt";
                    SetParamValueInReport(model, reportDocument, rptPath, dataTableObj);
                    break;
                case "RawHideIssueReceiveRpt":
                    rptPath = rptPath + "/RawHideIssueReceiveRpt.rpt";
                    SetParamValueInReport(model, reportDocument, rptPath, dataTableObj);
                    break;

            }
            reportDocument.ExportToHttpResponse(
                model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord,
                System.Web.HttpContext.Current.Response, false, "crReport");

            return View();
        }

        private void SetParamValueInReport(ReportModel model, ReportDocument reportDocument, string rptPath,
            DataTable dataTableObj)
        {
            reportDocument.Load(rptPath);
            reportDocument.SetDataSource(dataTableObj);
            reportDocument.Refresh();
            reportDocument.SetParameterValue("SupplierName", model.SupplierName ?? "");
            reportDocument.SetParameterValue("leathertype",
                model.LeatherTypeID != null
                    ? _repository.SysLeatherTypeRepository.GetByID(Convert.ToByte(model.LeatherTypeID)).LeatherTypeName
                    : "");
            reportDocument.SetParameterValue("itemtype",
                model.ItemTypeID != null
                    ? _repository.SysItemTypeRepository.GetByID(Convert.ToByte(model.ItemTypeID)).ItemTypeName
                    : "");
            reportDocument.SetParameterValue("StoreName",
                model.StoreID != null ? _repository.StoreRepository.GetByID(Convert.ToByte(model.StoreID)).StoreName : "");
            reportDocument.SetParameterValue("FromDate1",
                model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
            reportDocument.SetParameterValue("ToDate1",
                model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
        }

        [HttpGet]
        public ActionResult GradeReport()
        {
            ViewBag.Store = objStore.GetAllActiveStore();
            ViewBag.ItemType = objItemType.GetAllActiveItemTypeLeather();
            ViewBag.LeatherType = objLeatherType.GetAllActiveLeatherType();


            return View();
        }

        [CheckUserAccess("Reports/GradeSelectionReport")]
        public ActionResult GradeSelectionReport()
        {
            ViewBag.Message = "";
            ViewBag.ddStore = new SelectList(objStore.GetAllActiveStore().ToList(), "StoreID", "StoreName");
            ViewBag.ddGradeRange = new SelectList(_dalSysGradeRange.GetAll(), "GradeRangeID", "GradeRangeName");
            return View();
        }

        [CheckUserAccess("Reports/GradeSelectionDetailReport")]
        public ActionResult GradeSelectionDetailReport()
        {
            ViewBag.Message = "";
            ViewBag.ddSupplier = new SelectList(_dalSysSupplier.GetAll().OrderBy(x => x.SupplierName).ToList(), "SupplierID", "SupplierName");
            ViewBag.ddGradeRange = new SelectList(_dalSysGradeRange.GetAll(), "GradeRangeID", "GradeRangeName");
            return View();
        }

        [HttpPost]
        public ActionResult GradeSelectionDetailReport(FormCollection collection)
        {
            try
            {
                ViewBag.ddSupplier = new SelectList(_dalSysSupplier.GetAll().OrderBy(x => x.SupplierName).ToList(), "SupplierID", "SupplierName");
                ViewBag.ddGradeRange = new SelectList(_dalSysGradeRange.GetAll(), "GradeRangeID", "GradeRangeName");
                int val, supplierID = 0, rangeId = 0;
                long purchaseId = 0;
                if ((Int32.TryParse(Request.Form["ddGradeRange"], out val)))
                {
                    var obj = Request.Form["ddPurchase"];
                    if (obj.Equals("-- Select All --"))
                    {
                        purchaseId = 0;
                    }
                    else
                    {
                        purchaseId = Convert.ToInt64(Request.Form["ddPurchase"] ?? "0");
                    }

                    rangeId = Convert.ToInt32(Request.Form["ddGradeRange"] ?? "0");
                    supplierID = Convert.ToInt32(Request.Form["ddSupplier"] ?? "0");
                    String supplier = _dalSysSupplier.GetSupplierBySupplierId(supplierID);
                    if (purchaseId == 0)
                    {
                        ReportDocument reportDocument = new ReportDocument();
                        string rptPath = Server.MapPath("~/Reports");
                        DataTable DataTableObj = new DataTable();
                        var _dalObj = new DalReport();
                        DataTableObj = _dalObj.GetGradeSelectionSummaryReportByPurchaseId(supplierID, purchaseId, rangeId);
                        rptPath = rptPath + "/PreGradeSelectionSummaryReportByPurchaseId.rpt";
                        reportDocument.Load(rptPath);
                        reportDocument.SetDataSource(DataTableObj);
                        reportDocument.Refresh();

                        reportDocument.SetParameterValue("SupplierName", _repository.SysSupplierRepository.GetByID(supplierID).SupplierName);
                        reportDocument.SetParameterValue("PurchaseNo", _repository.PrqPurchaseRepository.GetByID(purchaseId).PurchaseNo);
                        reportDocument.SetParameterValue("GradeRangeName", _repository.SysGradeRangeRepository.GetByID(rangeId).GradeRangeName);


                        reportDocument.ExportToHttpResponse(
                            Request.Form["ReportType"].ToString().Equals("PDF")
                                ? ExportFormatType.PortableDocFormat
                                : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, "crReport");
                    }
                    else
                    {
                        ReportDocument reportDocument = new ReportDocument();
                        string rptPath = Server.MapPath("~/Reports");
                        DataTable DataTableObj = new DataTable();
                        var _dalObj = new DalReport();
                        DataTableObj = _dalObj.GetGradeSelectionSummaryReportByPurchaseId(0, purchaseId, rangeId);
                        rptPath = rptPath + "/PreGradeSelectionSummaryReportByPurchaseId.rpt";
                        reportDocument.Load(rptPath);
                        reportDocument.SetDataSource(DataTableObj);
                        reportDocument.Refresh();
                        string supp = _repository.SysSupplierRepository.GetByID(supplierID).SupplierName;
                        string pur = _repository.PrqPurchaseRepository.GetByID(purchaseId).PurchaseNo;
                        string nam = _repository.SysGradeRangeRepository.GetByID(rangeId).GradeRangeName;
                        reportDocument.SetParameterValue("SupplierName", supp == null ? "N/A" : supp);
                        reportDocument.SetParameterValue("PurchaseNo", pur == null ? "N/A" : pur);
                        reportDocument.SetParameterValue("GradeRangeName", nam == null ? "N/A" : nam);

                        reportDocument.ExportToHttpResponse(
                            Request.Form["ReportType"].ToString().Equals("PDF")
                                ? ExportFormatType.PortableDocFormat
                                : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, "crReport");

                    }

                    return View();
                }

                if ((Int32.TryParse(Request.Form["ddPurchase"], out val)))
                {
                    purchaseId = Convert.ToInt64(Request.Form["ddPurchase"] ?? "0");
                    supplierID = Convert.ToInt32(Request.Form["ddSupplier"] ?? "0");

                    ReportDocument reportDocument = new ReportDocument();
                    string rptPath = Server.MapPath("~/Reports");
                    DataTable DataTableObj = new DataTable();
                    var _dalObj = new DalReport();
                    DataTableObj = _dalObj.GetSelectionDetailsData(supplierID, purchaseId);
                    rptPath = rptPath + "/PreGradeSelectionDetailsContainer.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(DataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("PurchaseNo", _repository.PrqPurchaseRepository.GetByID(purchaseId).PurchaseNo);
                    reportDocument.ExportToHttpResponse(
                        Request.Form["ReportType"].ToString().Equals("PDF")
                            ? ExportFormatType.PortableDocFormat
                            : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, "crReport");
                    return View();
                }
                else
                {
                    supplierID = Convert.ToInt32(Request.Form["ddSupplier"] ?? "0");
                    ReportDocument reportDocument = new ReportDocument();
                    string rptPath = Server.MapPath("~/Reports");
                    DataTable DataTableObj = new DataTable();
                    var _dalObj = new DalReport();
                    DataTableObj = _dalObj.GetSelectionDetailsDataBySupplierID(supplierID);
                    rptPath = rptPath + "/SupplierWisePurchaseAndSelectionDetails.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(DataTableObj);
                    reportDocument.Refresh();
                    reportDocument.ExportToHttpResponse(
                        Request.Form["ReportType"].ToString().Equals("PDF")
                            ? ExportFormatType.PortableDocFormat
                            : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, "crReport");
                    return View();
                }

            }
            catch (Exception ex)
            {

            }

            return View();
        }
        //public ActionResult GradeSelectionDetailReport(FormCollection collection)
        //{
        //    try
        //    {
        //        ViewBag.ddSupplier = new SelectList(_dalSysSupplier.GetAll().OrderBy(x => x.SupplierName).ToList(), "SupplierID", "SupplierName");
        //        ViewBag.ddGradeRange = new SelectList(_dalSysGradeRange.GetAll(), "GradeRangeID", "GradeRangeName");
        //        int val, supplierID = 0, rangeId = 0;
        //        long purchaseId = 0;
        //        if ((Int32.TryParse(Request.Form["ddGradeRange"], out val)))
        //        {
        //            var obj = Request.Form["ddPurchase"];
        //            if (obj.Equals("-- Select All --"))
        //            {
        //                purchaseId = 0;
        //            }
        //            else
        //            {
        //                purchaseId = Convert.ToInt64(Request.Form["ddPurchase"] ?? "0");
        //            }

        //            rangeId = Convert.ToInt32(Request.Form["ddGradeRange"] ?? "0");
        //            supplierID = Convert.ToInt32(Request.Form["ddSupplier"] ?? "0");
        //            String supplier = _dalSysSupplier.GetSupplierBySupplierId(supplierID);
        //            if (purchaseId == 0)
        //            {
        //                ReportDocument reportDocument = new ReportDocument();
        //                string rptPath = Server.MapPath("~/Reports");
        //                DataTable DataTableObj = new DataTable();
        //                var _dalObj = new DalReport();
        //                DataTableObj = _dalObj.GetGradeSelectionSummaryReportByPurchaseId(supplierID, purchaseId, rangeId);
        //                rptPath = rptPath + "/PreGradeSelectionSummaryReportByPurchaseId.rpt";
        //                reportDocument.Load(rptPath);
        //                reportDocument.SetDataSource(DataTableObj);
        //                reportDocument.Refresh();

        //                reportDocument.SetParameterValue("SupplierName", _repository.SysSupplierRepository.GetByID(supplierID).SupplierName);
        //                reportDocument.SetParameterValue("PurchaseNo","All");
        //                reportDocument.SetParameterValue("GradeRangeName", _repository.SysGradeRangeRepository.GetByID(rangeId).GradeRangeName);


        //                reportDocument.ExportToHttpResponse(
        //                    Request.Form["ReportType"].ToString().Equals("PDF")
        //                        ? ExportFormatType.PortableDocFormat
        //                        : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, "crReport");
        //            }
        //            else
        //            {
        //                ReportDocument reportDocument = new ReportDocument();
        //                string rptPath = Server.MapPath("~/Reports");
        //                DataTable DataTableObj = new DataTable();
        //                var _dalObj = new DalReport();
        //                DataTableObj = _dalObj.GetGradeSelectionSummaryReportByPurchaseId(0, purchaseId, rangeId);
        //                rptPath = rptPath + "/PreGradeSelectionSummaryReportByPurchaseId.rpt";
        //                reportDocument.Load(rptPath);
        //                reportDocument.SetDataSource(DataTableObj);
        //                reportDocument.Refresh();
        //                string supp = _repository.SysSupplierRepository.GetByID(supplierID).SupplierName;
        //                string pur = _repository.PrqPurchaseRepository.GetByID(purchaseId).PurchaseNo;
        //                string nam = _repository.SysGradeRangeRepository.GetByID(rangeId).GradeRangeName;
        //                reportDocument.SetParameterValue("SupplierName", supp == null ? "N/A" : supp);
        //                reportDocument.SetParameterValue("PurchaseNo", pur == null ? "N/A" : pur);
        //                reportDocument.SetParameterValue("GradeRangeName", nam == null ? "N/A" : nam);

        //                reportDocument.ExportToHttpResponse(
        //                    Request.Form["ReportType"].ToString().Equals("PDF")
        //                        ? ExportFormatType.PortableDocFormat
        //                        : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, "crReport");

        //            }

        //            return View();
        //        }

        //        if ((Int32.TryParse(Request.Form["ddPurchase"], out val)))
        //        {
        //            purchaseId = Convert.ToInt64(Request.Form["ddPurchase"] ?? "0");
        //            supplierID = Convert.ToInt32(Request.Form["ddSupplier"] ?? "0");

        //            ReportDocument reportDocument = new ReportDocument();
        //            string rptPath = Server.MapPath("~/Reports");
        //            DataTable DataTableObj = new DataTable();
        //            var _dalObj = new DalReport();
        //            DataTableObj = _dalObj.GetSelectionDetailsData(supplierID, purchaseId);
        //            rptPath = rptPath + "/PreGradeSelectionDetailsContainer.rpt";
        //            reportDocument.Load(rptPath);
        //            reportDocument.SetDataSource(DataTableObj);
        //            reportDocument.Refresh();
        //            reportDocument.SetParameterValue("PurchaseNo", _repository.PrqPurchaseRepository.GetByID(purchaseId).PurchaseNo);
        //            reportDocument.ExportToHttpResponse(
        //                Request.Form["ReportType"].ToString().Equals("PDF")
        //                    ? ExportFormatType.PortableDocFormat
        //                    : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, "crReport");
        //            return View();
        //        }
        //        else
        //        {
        //            supplierID = Convert.ToInt32(Request.Form["ddSupplier"] ?? "0");
        //            ReportDocument reportDocument = new ReportDocument();
        //            string rptPath = Server.MapPath("~/Reports");
        //            DataTable DataTableObj = new DataTable();
        //            var _dalObj = new DalReport();
        //            DataTableObj = _dalObj.GetSelectionDetailsDataBySupplierID(supplierID);
        //            rptPath = rptPath + "/SupplierWisePurchaseAndSelectionDetails.rpt";
        //            reportDocument.Load(rptPath);
        //            reportDocument.SetDataSource(DataTableObj);
        //            reportDocument.Refresh();
        //            reportDocument.ExportToHttpResponse(
        //                Request.Form["ReportType"].ToString().Equals("PDF")
        //                    ? ExportFormatType.PortableDocFormat
        //                    : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, "crReport");
        //            return View();
        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //    return View();
        //}

        [HttpPost]
        public ActionResult GradeSelectionReport(FormCollection collection)
        {
            try
            {
                ViewBag.ddStore = new SelectList(objStore.GetAllActiveStore().ToList(), "StoreID", "StoreName");
                ViewBag.ddGradeRange = new SelectList(_dalSysGradeRange.GetAll(), "GradeRangeID", "GradeRangeName");
                int val, gradeRangeId = 0, supplierID = 0, selectionStore = 0, selectionId = 0;
                if ((Int32.TryParse(Request.Form["ddGradeRange"], out val)) && (Int32.TryParse(Request.Form["ddSuppliers"], out val)) && (Int32.TryParse(Request.Form["ddStore"], out val)) && (Int32.TryParse(Request.Form["ddSelection"], out val)))
                {
                    gradeRangeId = Convert.ToInt32(Request.Form["ddGradeRange"]);
                    supplierID = Convert.ToInt32(Request.Form["ddSuppliers"]);
                    selectionStore = Convert.ToInt32(Request.Form["ddStore"]);
                    selectionId = Convert.ToInt32(Request.Form["ddSelection"]);

                    var store = objStore.StoreGetByID(selectionStore);
                    ReportDocument reportDocument = new ReportDocument();
                    string rptPath = Server.MapPath("~/Reports");
                    DataTable DataTableObj = new DataTable();
                    var _dalObj = new DalReport();
                    DataTableObj = _dalObj.GetSelectionReportData(gradeRangeId, supplierID, selectionStore, selectionId);
                    rptPath = rptPath + "/PreSelectionReport.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(DataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("Stock", store[0].StoreName);

                    reportDocument.ExportToHttpResponse(
                        Request.Form["ReportType"].ToString().Equals("PDF")
                            ? ExportFormatType.PortableDocFormat
                            : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, "crReport");
                }
                else
                {
                    return View();
                }

            }
            catch (Exception ex)
            {

            }


            return View();
        }

        [CheckUserAccess("Reports/BillReport")]
        public ActionResult BillReport()
        {
            return View();
        }

        [CheckUserAccess("Reports/BillReport")]
        [HttpPost]
        public ActionResult BillReport(ReportBillModel billModel)
        {
            ReportDocument reportDocument = new ReportDocument();
            string rptPath = Server.MapPath("~/Reports");

            var reportObj = new DalReport();
            DataTable dataTableObj = reportObj.GetSupplierBillInformation(billModel);
            switch (billModel.ReportName)
            {
                case "SupplierWiseBillSummary":

                    rptPath = rptPath + "/SupplierWiseBillSummary.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("SupplierName", billModel.SupplierName ?? "");
                    reportDocument.SetParameterValue("purchaseYear1", billModel.PurchaseYear ?? "");
                    reportDocument.SetParameterValue("FromDate1", billModel.FromDate == null ? "" : DalCommon.SetDate(billModel.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1", billModel.ToDate == null ? "" : DalCommon.SetDate(billModel.ToDate).ToString(ReportDateFormat));
                    break;
                case "SupplierWiseBillDetail":
                    rptPath = rptPath + "/SupplierWiseBillDetail.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("SupplierName", billModel.SupplierName ?? "");
                    reportDocument.SetParameterValue("PurchaseYear1", billModel.PurchaseYear ?? "");
                    reportDocument.SetParameterValue("FromDate1", billModel.FromDate == null ? "" : DalCommon.SetDate(billModel.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1", billModel.ToDate == null ? "" : DalCommon.SetDate(billModel.ToDate).ToString(ReportDateFormat));

                    break;
            }
            reportDocument.ExportToHttpResponse(
               billModel.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord,
               System.Web.HttpContext.Current.Response, false, "crReport");
            return View();
        }


        [HttpGet]
        [CheckUserAccess("Reports/PurchaseReport")]
        public ActionResult PurchaseReport()
        {
            ViewBag.Store = objStore.GetAllActiveStore();
            ViewBag.ItemList = objItemType.GetAllActiveItemTypeLeather();
            return View();
        }
        [HttpPost]
        public ActionResult PurchaseReport(PurchaseReportModel purchaseModel)
        {
            ViewBag.Store = objStore.GetAllActiveStore();
            ViewBag.ItemList = objItemType.GetAllActiveItemTypeLeather();
            var reportDocument = new ReportDocument();
            var rptPath = Server.MapPath("~/Reports");
            var reportObj = new DalReport();
            var dataTableObj = reportObj.GetPurchaseInformation(purchaseModel);
            switch (purchaseModel.ReportName)
            {
                case "SupplierWisePurchaseSummary":
                    rptPath = rptPath + "/SupplierWisePurchaseSummary.rpt";
                    SetParamValueInReport(purchaseModel, reportDocument, rptPath, dataTableObj);
                    break;
                case "SupplierWisePurchaseDetail":
                    rptPath = rptPath + "/SupplierWisePurchaseDetail.rpt";
                    SetParamValueInReport(purchaseModel, reportDocument, rptPath, dataTableObj);
                    break;
                case "StoreWisePurchase":
                    rptPath = rptPath + "/StoreWisePurchase.rpt";
                    SetParamValueInReport(purchaseModel, reportDocument, rptPath, dataTableObj);
                    break;
                case "PurchaseTypeWisePurchase":
                    rptPath = rptPath + "/PurchaseTypeWisePurchase.rpt";
                    SetParamValueInReport(purchaseModel, reportDocument, rptPath, dataTableObj);
                    break;
                case "SourceWisePurchase":
                    rptPath = rptPath + "/LocationWisePurchase.rpt";
                    SetParamValueInReport(purchaseModel, reportDocument, rptPath, dataTableObj);
                    break;
                case "ItemWisePurchase":
                    rptPath = rptPath + "/ItemTypeWisePurchaseRpt.rpt";
                    SetParamValueInReport(purchaseModel, reportDocument, rptPath, dataTableObj);
                    break;
                case "MonthWisePurchaseSummary":
                    rptPath = rptPath + "/MonthWisePurchaseSummary.rpt";
                    SetParamValueInReport(purchaseModel, reportDocument, rptPath, dataTableObj);
                    break;
                //case "MonthlyPurchaseAndWBProduction":
                //    rptPath = rptPath + "/MonthlyPurchaseAndWBProduction.rpt";
                    //SetParamValueInReport(purchaseModel, reportDocument, rptPath, dataTableObj);
                case "MonthlyPurchaseAndWBProduction":
                    rptPath = rptPath + "/MonthlyPurchaseAndWBProduction.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("SupplierName", purchaseModel.SupplierName ?? "");
                    reportDocument.SetParameterValue("ScheduleYear", purchaseModel.PurchaseYear ?? "");
                    reportDocument.SetParameterValue("FromDate1",
                    purchaseModel.DateFrom == null ? "" : DalCommon.SetDate(purchaseModel.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1",
                    purchaseModel.DateTo == null ? "" : DalCommon.SetDate(purchaseModel.DateTo).ToString(ReportDateFormat));
                    break;
                    
            }
            reportDocument.ExportToHttpResponse(
               purchaseModel.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord,
               System.Web.HttpContext.Current.Response, false, "crReport");
            return View();
        }

        private void SetParamValueInReport(PurchaseReportModel purchaseModel, ReportDocument reportDocument, string rptPath,
            DataTable dataTableObj)
        {
            reportDocument.Load(rptPath);
            reportDocument.SetDataSource(dataTableObj);
            reportDocument.Refresh();

            reportDocument.SetParameterValue("SupplierName", purchaseModel.SupplierName ?? "");
            reportDocument.SetParameterValue("purchaseYear", purchaseModel.PurchaseYear ?? "");
            reportDocument.SetParameterValue("purchaseType", purchaseModel.PurchaseType == "0" ? "" : purchaseModel.PurchaseType);
            reportDocument.SetParameterValue("StoreName",
                purchaseModel.StoreID != null
                    ? _repository.StoreRepository.GetByID(Convert.ToByte(purchaseModel.StoreID)).StoreName
                    : "");
            reportDocument.SetParameterValue("Item",
                purchaseModel.ItemTypeID != null
                    ? _repository.SysItemTypeRepository.GetByID(Convert.ToByte(purchaseModel.ItemTypeID)).ItemTypeName
                    : "");
            reportDocument.SetParameterValue("FromDate1",
                purchaseModel.DateFrom == null ? "" : DalCommon.SetDate(purchaseModel.DateFrom).ToString(ReportDateFormat));
            reportDocument.SetParameterValue("ToDate1",
                purchaseModel.DateTo == null ? "" : DalCommon.SetDate(purchaseModel.DateTo).ToString(ReportDateFormat));
        }
        [CheckUserAccess("Reports/RawHideYearlyTargetRpt")]
        public ActionResult RawHideYearlyTargetRpt()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RawHideYearlyTargetRpt(PurchaseReportModel reportModel)
        {
            var reportDocument = new ReportDocument();
            var rptPath = Server.MapPath("~/Reports");
            var reportObj = new DalReport();
            var dataTableObj = reportObj.GetRawHideYearlyTarget(reportModel);
            switch (reportModel.ReportName)
            {
                case "YearWiseTargetDatail":
                    rptPath = rptPath + "/RawHideYearlyTarget.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("PurchaseYear1", reportModel.PurchaseYear ?? "");

                    break;
                case "ItemWiseTargetSummary":
                    rptPath = rptPath + "/RawHideYearlyTargetItemwise.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("PurchaseYear1", reportModel.PurchaseYear ?? "");
                    break;
            }
            reportDocument.ExportToHttpResponse(
            reportModel.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord,
            System.Web.HttpContext.Current.Response, false, "crReport");

            return View();
        }

        [CheckUserAccess("Reports/LcOpeningEntryReport")]
        public ActionResult LcOpeningEntryReport()
        {
            ViewBag.ddSupplier = new SelectList(_dalSysSupplier.GetAll().ToList(), "SupplierID", "SupplierName");
            return View();
        }
        [HttpPost]
        public ActionResult GetLCOpeningData(string dateFrom, string dateTo, string amountFrom, string amountTo, string supplier)
        {
            //dateFrom = DalCommon.SetDate(dateFrom);
            //dateTo = DalCommon.SetDate(dateTo);

            StringBuilder sb = new StringBuilder(); String a = "", b = "", c = "";

            if (amountFrom != "" && amountTo != "")
            {
                a = "(LCAmount >='" + amountFrom + "' AND LCAmount <='" + amountTo + "' )";
                sb.Append(a);
            }
            if (dateTo != "" && dateFrom != "")
            {
                if (sb.ToString() == "")
                {
                    b = "(LCDate >='" + DalCommon.SetDate(dateFrom) + "' AND LCDate <='" + DalCommon.SetDate(dateTo) + "' )";
                    sb.Append(b);
                }
                else
                {
                    b = "AND (LCDate >='" + DalCommon.SetDate(dateFrom) + "' AND LCDate <='" + DalCommon.SetDate(dateTo) + "' )";
                    sb.Append(b);
                }
            }
            if (supplier != "")
            {
                if (sb.ToString() == "")
                {
                    c = " PIID IN(SELECT PIID FROM PRQ_ChemicalPI WHERE SupplierID='" + supplier + "')";
                }
                else
                {
                    c = " AND PIID IN(SELECT PIID FROM PRQ_ChemicalPI WHERE SupplierID='" + supplier + "')";
                }
                sb.Append(c);
            }
            String sql = "";
            if (a == "" && b == "" && c == "")
            {
                sql = @"SELECT * FROM LCM_LCOpening";
            }
            else
            {
                sql = @"SELECT * FROM LCM_LCOpening WHERE " + sb.ToString();
            }

            List<LcmLcOpening> lst = _dalLcOpening.GetLcOpeningReportData(sql);
            return Json(lst);
        }

        [HttpPost]
        public ActionResult LcOpeningEntryReport(string LCNo)
        {
            ViewBag.ddSupplier = new SelectList(_dalSysSupplier.GetAll().ToList(), "SupplierID", "SupplierName");
            if (LCNo != null)
            {
                ReportDocument reportDocument = new ReportDocument();
                string rptPath = Server.MapPath("~/Reports");
                var dataTableObj = new DataTable();
                var reportObj = new DalReport();

                dataTableObj = reportObj.LcOpeningEntryReport(LCNo);

                rptPath = rptPath + "/LcOpeningEntryReport.rpt";
                reportDocument.Load(rptPath);
                reportDocument.SetDataSource(dataTableObj);
                reportDocument.Refresh();


                reportDocument.ExportToHttpResponse(
                                 Request.Form["ReportType"].ToString().Equals("PDF")
                                     ? ExportFormatType.PortableDocFormat
                                     : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, "crReport");
            }
            return View("LcOpeningEntryReport");
        }

        [CheckUserAccess("Reports/InsuranceInfoEntryReport")]
        public ActionResult InsuranceInfoEntryReport()
        {
            ViewBag.ddSupplier = new SelectList(_dalSysSupplier.GetAll().ToList(), "SupplierID", "SupplierName");
            return View();
        }
        [HttpPost]
        public ActionResult GetInsuranceInfoData()
        {
            var result = _repository.LcmInsuranceRpository.Get().ToList();
            List<Lcm_Insurence> lst = new List<Lcm_Insurence>();
            foreach (var item in result)
            {
                Lcm_Insurence ob = new Lcm_Insurence();
                ob.InsuranceID = item.InsuranceID;
                ob.InsuranceNo = item.InsuranceNo;
                ob.InsuranceChallanNo = item.InsuranceChallanNo;
                ob.InsurancChallanDate = Convert.ToDateTime(item.InsurancChallanDate).ToString("dd/MM/yyyy");
                ob.RecordStatus = item.RecordStatus;
                lst.Add(ob);
            }
            return Json(lst);
        }

        [HttpPost]
        public ActionResult InsuranceInfoEntryReport(string insuranceNo)
        {
            if (insuranceNo != null)
            {
                ReportDocument reportDocument = new ReportDocument();
                string rptPath = Server.MapPath("~/Reports");
                var dataTableObj = new DataTable();
                var reportObj = new DalReport();

                dataTableObj = reportObj.InsuranceInfoEntryReport(insuranceNo);

                rptPath = rptPath + "/InsuraceInfoEntryReport.rpt";
                reportDocument.Load(rptPath);
                reportDocument.SetDataSource(dataTableObj);
                reportDocument.Refresh();


                reportDocument.ExportToHttpResponse(
                                 Request.Form["ReportType"].ToString().Equals("PDF")
                                     ? ExportFormatType.PortableDocFormat
                                     : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, "crReport");
            }
            return View("InsuranceInfoEntryReport");
        }

        [CheckUserAccess("Reports/BankDebitInfoEntryReport")]
        public ActionResult BankDebitInfoEntryReport()
        {
            ViewBag.ddSupplier = new SelectList(_dalSysSupplier.GetAll().ToList(), "SupplierID", "SupplierName");
            return View();
        }
        [HttpPost]
        public ActionResult GetBankDebitInfoData()
        {
            var result = _repository.LcmBankDebitVoucherRpository.Get().ToList();
            List<LcmBankDebitVoucher> lst = new List<LcmBankDebitVoucher>();
            foreach (var item in result)
            {
                LcmBankDebitVoucher ob = new LcmBankDebitVoucher();
                ob.BDVID = item.BDVID;
                ob.BDVNo = item.BDVNo;
                ob.BDVDate = Convert.ToDateTime(item.BDVDate).ToString("dd/MM/yyyy");
                ob.LCNo = item.LCNo;
                ob.RecordStatus = item.RecordStatus;
                lst.Add(ob);
            }
            return Json(lst);
        }

        [HttpPost]
        public ActionResult BankDebitInfoEntryReport(string bdvNo)
        {
            if (bdvNo != null)
            {
                ReportDocument reportDocument = new ReportDocument();
                string rptPath = Server.MapPath("~/Reports");
                var dataTableObj = new DataTable();
                var reportObj = new DalReport();

                dataTableObj = reportObj.BankDebitInfoEntryReport(bdvNo);

                rptPath = rptPath + "/BankDebitInfoEntryReport.rpt";
                reportDocument.Load(rptPath);
                reportDocument.SetDataSource(dataTableObj);
                reportDocument.Refresh();
                reportDocument.SetParameterValue("BDVNo2", bdvNo.ToString());
                //  reportDocument.SetParameterValue("supplier", supplier.ToString());
                reportDocument.ExportToHttpResponse(
                                 Request.Form["ReportType"].ToString().Equals("PDF")
                                     ? ExportFormatType.PortableDocFormat
                                     : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, "crReport");
            }
            return View("BankDebitInfoEntryReport");
        }






        [CheckUserAccess("Reports/PurchaseWithBillReport")]
        public ActionResult PurchaseWithBillReport()
        {
            ViewBag.Store = objStore.GetAllActiveStore();
            ViewBag.ItemType = objItemType.GetAllActiveItemTypeLeather();


            return View();
        }
        [HttpPost]
        public ActionResult PurchaseWithBillReport(ReportBillModel purchaseModel)
        {


            ViewBag.Store = objStore.GetAllActiveStore();
            ViewBag.ItemType = objItemType.GetAllActiveItemTypeLeather();
            var reportDocument = new ReportDocument();
            var rptPath = Server.MapPath("~/Reports");
            var reportObj = new DalReport();
            var dataTableObj = reportObj.GetPurchaseWithBillInformation(purchaseModel);
            switch (purchaseModel.ReportName)
            {
                case "ItemAndSupplierWiseRHBill":
                    rptPath = rptPath + "/ItemAndSupplierWiseRHBill.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("PurchaseYear1", purchaseModel.PurchaseYear ?? "");
                    reportDocument.SetParameterValue("FromDate1", purchaseModel.FromDate == null ? "" : DalCommon.SetDate(purchaseModel.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1", purchaseModel.ToDate == null ? "" : DalCommon.SetDate(purchaseModel.ToDate).ToString(ReportDateFormat));
                    break;
                case "ItemWiseRHPurchaseBill":
                    rptPath = rptPath + "/ItemWiseRHPurchaseBill.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);//
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("PurchaseYear1", purchaseModel.PurchaseYear ?? "");
                    reportDocument.SetParameterValue("FromDate1", purchaseModel.FromDate == null ? "" : DalCommon.SetDate(purchaseModel.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1", purchaseModel.ToDate == null ? "" : DalCommon.SetDate(purchaseModel.ToDate).ToString(ReportDateFormat));
                    break;
                case "StoreWiseItemPrice":
                    rptPath = rptPath + "/StoreWiseItemPrice.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("PurchaseYear1", purchaseModel.PurchaseYear ?? "");
                    reportDocument.SetParameterValue("FromDate1", purchaseModel.FromDate == null ? "" : DalCommon.SetDate(purchaseModel.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1", purchaseModel.ToDate == null ? "" : DalCommon.SetDate(purchaseModel.ToDate).ToString(ReportDateFormat));
                    break;
            }
            reportDocument.ExportToHttpResponse(
           purchaseModel.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord,
           System.Web.HttpContext.Current.Response, false, "crReport");
            return View();
        }
        [CheckUserAccess("Reports/RHLPurchaseWithPaymentRpt")]
        public ActionResult RHLPurchaseWithPaymentRpt()
        {
            ViewBag.Store = objStore.GetAllActiveStore();
            ViewBag.ItemType = objItemType.GetAllActiveItemTypeLeather();
            return View();
        }
        [HttpPost]
        public ActionResult RHLPurchaseWithPaymentRpt(ReportBillModel model)
        {
            ViewBag.Store = objStore.GetAllActiveStore();
            ViewBag.ItemType = objItemType.GetAllActiveItemTypeLeather();
            ReportDocument reportDocument = new ReportDocument();
            string rptPath = Server.MapPath("~/Reports");

            var reportObj = new DalReport();
            DataTable dataTableObj = reportObj.GetSupplierRhlPurchaseWithPaymentInformation(model);
            switch (model.ReportName)
            {
                case "SupplierWiseRHLPurchaseAndPaymentSummary":

                    rptPath = rptPath + "/SupplierWiseRHLPurchaseAndPayment.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("SupplierName", model.SupplierName ?? "");
                    reportDocument.SetParameterValue("AppRate", model.ApproximateRate == null ? "" : Convert.ToDecimal(model.ApproximateRate).ToString("##.###"));
                    reportDocument.SetParameterValue("PurchaseYear1", model.PurchaseYear ?? "");
                    reportDocument.SetParameterValue("FromDate1", model.FromDate == null ? "" : DalCommon.SetDate(model.FromDate).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1", model.ToDate == null ? "" : DalCommon.SetDate(model.ToDate).ToString(ReportDateFormat));
                    break;
            }
            reportDocument.ExportToHttpResponse(
               model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord,
               System.Web.HttpContext.Current.Response, false, "crReport");
            return View();
        }
        [CheckUserAccess("Reports/ChemicalIssueRpt")]
        public ActionResult ChemicalIssueRpt()
        {
            ViewBag.Store = objStore.GetAllActiveChemicalStore();
            ViewBag.ddlItemCategory = new DalSysChemicalItem().GetAllChemicalItemCategory();
            return View();
        }
        [HttpPost]
        public ActionResult ChemicalIssueRpt(ReportModel model)
        {
            ViewBag.Store = objStore.GetAllActiveChemicalStore();
            ViewBag.ddlItemCategory = new DalSysChemicalItem().GetAllChemicalItemCategory();
            ReportDocument reportDocument = new ReportDocument();
            string rptPath = Server.MapPath("~/Reports");

            var reportObj = new DalReport();
            DataTable dataTableObj = reportObj.GetChemicalStockAndIssueInformation(model);
            switch (model.ReportName)
            {
                case "ChemWiseIssueSummary":
                    rptPath = rptPath + "/ChemWiseIssueSummary.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("SupplierName", model.SupplierName ?? "");
                    reportDocument.SetParameterValue("StoreName1", model.StoreID != null ? _repository.StoreRepository.GetByID(Convert.ToByte(model.StoreID)).StoreName : "");
                    reportDocument.SetParameterValue("ItemName1", model.ItemID != null ? _repository.SysChemicalItemRepository.GetByID(Convert.ToInt32(model.ItemID)).ItemName : "");
                    reportDocument.SetParameterValue("FromDate1", model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1", model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
                case "ChemIssueDetail":
                    rptPath = rptPath + "/ChemIssueDetail.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("SupplierName", model.SupplierName ?? "");
                    reportDocument.SetParameterValue("StoreName1", model.StoreID != null ? _repository.StoreRepository.GetByID(Convert.ToByte(model.StoreID)).StoreName : "");
                    reportDocument.SetParameterValue("ItemName1", model.ItemID != null ? _repository.SysChemicalItemRepository.GetByID(Convert.ToInt32(model.ItemID)).ItemName : "");
                    reportDocument.SetParameterValue("FromDate1", model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1", model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
                case "ChemProdStageWiseIssueDeatil":
                    rptPath = rptPath + "/ChemProdStageWiseIssueDeatil.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("SupplierName", model.SupplierName ?? "");
                    reportDocument.SetParameterValue("StoreName1", model.StoreID != null ? _repository.StoreRepository.GetByID(Convert.ToByte(model.StoreID)).StoreName : "");
                    reportDocument.SetParameterValue("ItemName1", model.ItemID != null ? _repository.SysChemicalItemRepository.GetByID(Convert.ToInt32(model.ItemID)).ItemName : "");
                    reportDocument.SetParameterValue("FromDate1", model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1", model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
                case "ChemWiseReceiveIssueStock":
                    rptPath = rptPath + "/ChemWiseReceiveIssueStock.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("SupplierName", model.SupplierName ?? "");
                    reportDocument.SetParameterValue("StoreName1", model.StoreID != null ? _repository.StoreRepository.GetByID(Convert.ToByte(model.StoreID)).StoreName : "");
                    reportDocument.SetParameterValue("ItemName1", model.ItemID != null ? _repository.SysChemicalItemRepository.GetByID(Convert.ToInt32(model.ItemID)).ItemName : "");
                    reportDocument.SetParameterValue("FromDate1", model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1", model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
                case "YearMonthChemWiseReceiveIssueStock":
                    rptPath = rptPath + "/YearMonthChemWiseRcvIssueStock.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("SupplierName", model.SupplierName ?? "");
                    reportDocument.SetParameterValue("StoreName1", model.StoreID != null ? _repository.StoreRepository.GetByID(Convert.ToByte(model.StoreID)).StoreName : "");
                    reportDocument.SetParameterValue("ItemName1", model.ItemID != null ? _repository.SysChemicalItemRepository.GetByID(Convert.ToInt32(model.ItemID)).ItemName : "");
                    reportDocument.SetParameterValue("FromDate1", model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1", model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
                case "DateWiseChemReceiveIssueStock":
                    rptPath = rptPath + "/DateWiseChemReceiveIssueStock.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("SupplierName", model.SupplierName ?? "");
                    reportDocument.SetParameterValue("StoreName1", model.StoreID != null ? _repository.StoreRepository.GetByID(Convert.ToByte(model.StoreID)).StoreName : "");
                    reportDocument.SetParameterValue("ItemName1", model.ItemID != null ? _repository.SysChemicalItemRepository.GetByID(Convert.ToInt32(model.ItemID)).ItemName : "");
                    reportDocument.SetParameterValue("FromDate1", model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1", model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
            }
            reportDocument.ExportToHttpResponse(
               model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord,
               System.Web.HttpContext.Current.Response, false, "crReport");
            return View();
        }

        [CheckUserAccess("Reports/ChemStockRpt")]
        public ActionResult ChemStockRpt()
        {
            ViewBag.Store = objStore.GetAllActiveChemicalStore();
            ViewBag.ddlItemCategory = new DalSysChemicalItem().GetAllChemicalItemCategory();
            return View();
        }
        [HttpPost]
        public ActionResult ChemStockRpt(ReportModel model)
        {
            ViewBag.Store = objStore.GetAllActiveChemicalStore();
            ViewBag.ddlItemCategory = new DalSysChemicalItem().GetAllChemicalItemCategory();
            ReportDocument reportDocument = new ReportDocument();
            string rptPath = Server.MapPath("~/Reports");

            var reportObj = new DalReport();
            DataTable dataTableObj = reportObj.GetChemicalStockAndIssueInformation(model);
            switch (model.ReportName)
            {
                case "DailyStock":
                    rptPath = rptPath + "/ChemcalDailyStock.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("StoreName1", model.StoreID != null ? _repository.StoreRepository.GetByID(Convert.ToByte(model.StoreID)).StoreName : "");
                    reportDocument.SetParameterValue("ItemName1", model.ItemID != null ? _repository.SysChemicalItemRepository.GetByID(Convert.ToInt32(model.ItemID)).ItemName : "");
                    reportDocument.SetParameterValue("FromDate1", model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1", model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
                case "ChemicalItemWiseStock":
                    rptPath = rptPath + "/ChemItemWiseStockRpt.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("StoreName1", model.StoreID != null ? _repository.StoreRepository.GetByID(Convert.ToByte(model.StoreID)).StoreName : "");
                    reportDocument.SetParameterValue("ItemName1", model.ItemID != null ? _repository.SysChemicalItemRepository.GetByID(Convert.ToInt32(model.ItemID)).ItemName : "");
                    //reportDocument.SetParameterValue("FromDate1", model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    //reportDocument.SetParameterValue("ToDate1", model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
                case "ChemicalItemWiseStockWithoutProd":
                    rptPath = rptPath + "/ChemicalItemWiseStockWithoutProd.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("StoreName1", model.StoreID != null ? _repository.StoreRepository.GetByID(Convert.ToByte(model.StoreID)).StoreName : "");
                    reportDocument.SetParameterValue("ItemName1", model.ItemID != null ? _repository.SysChemicalItemRepository.GetByID(Convert.ToInt32(model.ItemID)).ItemName : "");
                    //reportDocument.SetParameterValue("FromDate1", model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    //reportDocument.SetParameterValue("ToDate1", model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
                    
                case "StoreWiseChemicalItemStock":
                    rptPath = rptPath + "/StoreWiseItemStockRpt.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("StoreName1", model.StoreID != null ? _repository.StoreRepository.GetByID(Convert.ToByte(model.StoreID)).StoreName : "");
                    reportDocument.SetParameterValue("ItemName1", model.ItemID != null ? _repository.SysChemicalItemRepository.GetByID(Convert.ToInt32(model.ItemID)).ItemName : "");
                    //reportDocument.SetParameterValue("FromDate1", model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    //reportDocument.SetParameterValue("ToDate1", model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
                case "ItemCategoryWiseChemicalStock":
                    rptPath = rptPath + "/ItemCategoryWiseChemRpt.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("SupplierName", model.SupplierName ?? "");
                    reportDocument.SetParameterValue("StoreName1", model.StoreID != null ? _repository.StoreRepository.GetByID(Convert.ToByte(model.StoreID)).StoreName : "");
                    reportDocument.SetParameterValue("ItemName1", model.ItemID != null ? _repository.SysChemicalItemRepository.GetByID(Convert.ToInt32(model.ItemID)).ItemName : "");
                    //reportDocument.SetParameterValue("FromDate1", model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    //reportDocument.SetParameterValue("ToDate1", model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
                case "StoreWiseSupplierStock":
                    rptPath = rptPath + "/StoreWiseSupplierChemStock.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("SupplierName", model.SupplierName ?? "");
                    reportDocument.SetParameterValue("StoreName1", model.StoreID != null ? _repository.StoreRepository.GetByID(Convert.ToByte(model.StoreID)).StoreName : "");
                    reportDocument.SetParameterValue("ItemName1", model.ItemID != null ? _repository.SysChemicalItemRepository.GetByID(Convert.ToInt32(model.ItemID)).ItemName : "");
                    reportDocument.SetParameterValue("FromDate1", model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1", model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
                case "SupplierWiseStoreStock":
                    rptPath = rptPath + "/SupplierWiseStoreChemStock.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("SupplierName", model.SupplierCode ?? "");
                    reportDocument.SetParameterValue("StoreName1", model.StoreID != null ? _repository.StoreRepository.GetByID(Convert.ToByte(model.StoreID)).StoreName : "");
                    reportDocument.SetParameterValue("ItemName1", model.ItemID != null ? _repository.SysChemicalItemRepository.GetByID(Convert.ToInt32(model.ItemID)).ItemName : "");
                    reportDocument.SetParameterValue("FromDate1", model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1", model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
                case "ChemicalStatus":
                    rptPath = rptPath + "/WBChemicalStatus.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("SupplierName", model.SupplierName ?? "");
                    reportDocument.SetParameterValue("StoreName1", model.StoreID != null ? _repository.StoreRepository.GetByID(Convert.ToByte(model.StoreID)).StoreName : "");
                    reportDocument.SetParameterValue("ItemName1", model.ItemName ?? "");
                    reportDocument.SetParameterValue("FromDate1", model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("ToDate1", model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;
                case "ProductionWiseChemicalStock":
                    rptPath = rptPath + "/ProductionWiseChemicalStock.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("StoreName1", model.StoreID != null ? _repository.StoreRepository.GetByID(Convert.ToByte(model.StoreID)).StoreName : "");
                    reportDocument.SetParameterValue("ItemName1", model.ItemName ?? "");
                    reportDocument.SetParameterValue("ICategory", model.ItemCategory ?? "");
                    //reportDocument.SetParameterValue("FromDate1", model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                   // reportDocument.SetParameterValue("ToDate1", model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    break;

            }
            reportDocument.ExportToHttpResponse(
               model.ReportType == "PDF" ? ExportFormatType.PortableDocFormat : ExportFormatType.ExcelRecord,
               System.Web.HttpContext.Current.Response, false, "crReport");
            return View();
        }
        [CheckUserAccess("Reports/FinalGradeSelectionReport")]
        public ActionResult FinalGradeSelectionReport()
        {
            ViewBag.Message = "";
            ViewBag.SupplierID = new SelectList(_repository.SysSupplierRepository.Get(filter: ob => ob.SupplierCategory == "Leather" && ob.IsActive).OrderBy(x => x.SupplierName).ToList(), "SupplierID", "SupplierName");
            ViewBag.GradeRangeID = new SelectList(_dalSysGradeRange.GetAll(), "GradeRangeID", "GradeRangeName");
            return View();
        }

        [HttpPost]
        public ActionResult FinalGradeSelectionReport(ReportModel model)
        {
            ViewBag.Message = "";
            ViewBag.SupplierID = new SelectList(_repository.SysSupplierRepository.Get(filter: ob => ob.SupplierCategory == "Leather" && ob.IsActive).OrderBy(x => x.SupplierName).ToList(), "SupplierID", "SupplierName");
            ViewBag.GradeRangeID = new SelectList(_dalSysGradeRange.GetAll(), "GradeRangeID", "GradeRangeName");

            if (!string.IsNullOrEmpty(model.GradeRangeID))
            {
                ReportDocument reportDocument = new ReportDocument();
                string rptPath = Server.MapPath("~/Reports");
                var dataTableObj = new DataTable();
                var reportObj = new DalReport();
                var PurchaseID = Request.Form["ddPurchase"] == null ? "-- Select One --" : Request.Form["ddPurchase"];

                if (string.IsNullOrEmpty(model.SupplierID) && Request.Form["ReportName"].ToString().Equals("DateRange"))
                {
                    //All Supplier Purchase Report
                    dataTableObj = _dalReport.FinalGradeSelectionSummaryReport(model, "DateRange");
                    rptPath = rptPath + "/FinalGradeSelectionByDateRange.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("Title", "Grade Selection Report " + model.DateFrom + " To " + model.DateTo + "");
                    reportDocument.ExportToHttpResponse(
                   Request.Form["ReportType"].ToString().Equals("PDF")
                       ? ExportFormatType.PortableDocFormat
                       : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, "crReport");
                    return View();
                }

                if (PurchaseID.ToString().Equals("-- Select One --"))
                {
                    // Specific Supplier Purchase Report
                    dataTableObj = _dalReport.FinalGradeSelectionReport(model);
                    rptPath = rptPath + "/FinalGradeSelectionContainer.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    reportDocument.SetParameterValue("datefrom1", model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                    reportDocument.SetParameterValue("dateto1", model.DateTo == null ? "All" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                }
                else
                {
                    if (PurchaseID.ToString().Equals("alldetails"))
                    {
                        dataTableObj = _dalReport.FinalGradeSelectionReport(model, -1);
                        rptPath = rptPath + "/FinalGradeSelectionAllPurchaseContainer.rpt";
                        reportDocument.Load(rptPath);
                        reportDocument.SetDataSource(dataTableObj);
                        reportDocument.Refresh();
                        reportDocument.SetParameterValue("Title", "Purchase Wise Grade Selection Report " + model.DateFrom + " To " + model.DateTo + "");

                        //reportDocument.SetParameterValue("datefrom1", model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                        //reportDocument.SetParameterValue("dateto1", model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));

                    }
                    else if (PurchaseID.ToString().Equals("all"))
                    {
                        // Specific Supplier Purchase Report
                        dataTableObj = _dalReport.FinalGradeSelectionReport(model);
                        rptPath = rptPath + "/FinalGradeSelectionContainer.rpt";
                        reportDocument.Load(rptPath);
                        reportDocument.SetDataSource(dataTableObj);
                        reportDocument.Refresh();
                        reportDocument.SetParameterValue("datefrom1", model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                        reportDocument.SetParameterValue("dateto1", model.DateTo == null ? "All" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                    }
                    else
                    {
                        if (Request.Form["ReportName"].ToString().Equals("PurchaseSelectionSummary"))
                        {
                            dataTableObj = _dalReport.FinalGradeSelectionSummaryReport(model, Convert.ToInt64(PurchaseID));
                        }
                        else if (Request.Form["ReportName"].ToString().Equals("PurchaseSelectionDetails"))
                        {
                            dataTableObj = _dalReport.FinalGradeSelectionReport(model, Convert.ToInt64(PurchaseID));
                        }
                        else { return View(); }
                        rptPath = rptPath + "/FinalGradeSelectionByPurchaseContainer.rpt";
                        reportDocument.Load(rptPath);
                        reportDocument.SetDataSource(dataTableObj);
                        reportDocument.Refresh();
                        //reportDocument.SetParameterValue("datefrom1", model.DateFrom == null ? "" : DalCommon.SetDate(model.DateFrom).ToString(ReportDateFormat));
                        //reportDocument.SetParameterValue("dateto1", model.DateTo == null ? "" : DalCommon.SetDate(model.DateTo).ToString(ReportDateFormat));
                        reportDocument.SetParameterValue("Title", "Purchase Wise Grade Selection Report");
                    }
                }
                reportDocument.ExportToHttpResponse(
                    Request.Form["ReportType"].ToString().Equals("PDF")
                        ? ExportFormatType.PortableDocFormat
                        : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, "crReport");

            }
            return View();
        }

        public ActionResult FinalGradeSelectionByItemType()
        {
            ViewBag.ItemTypeID = new SelectList(_repository.SysItemTypeRepository.Get(filter: ob => ob.IsActive), "ItemTypeID", "ItemTypeName");
            ViewBag.GradeRangeID = new SelectList(_dalSysGradeRange.GetAll(), "GradeRangeID", "GradeRangeName");
            return View();
        }
        [HttpPost]
        public ActionResult FinalGradeSelectionByItemType(ReportModel model)
        {
            ViewBag.Message = "";
            ViewBag.ItemTypeID = new SelectList(_repository.SysItemTypeRepository.Get(filter: ob => ob.IsActive), "ItemTypeID", "ItemTypeName");
            ViewBag.GradeRangeID = new SelectList(_dalSysGradeRange.GetAll(), "GradeRangeID", "GradeRangeName");


            ReportDocument reportDocument = new ReportDocument();
            string rptPath = Server.MapPath("~/Reports");
            var dataTableObj = new DataTable();
            var reportObj = new DalReport();
            var ItemTypeID = Request.Form["ItemTypeID"] == null ? "-- Select One --" : Request.Form["ItemTypeID"];
            if (ItemTypeID.ToString().Equals(""))
            {
                return View();
            }
            else
            {
                dataTableObj = _dalReport.FinalGradeSelectionByItemType(model);
                rptPath = rptPath + "/FinalGradeSelectionByItemTypeContainer.rpt";
                reportDocument.Load(rptPath);
                reportDocument.SetDataSource(dataTableObj);
                reportDocument.Refresh();
                reportDocument.SetParameterValue("Title", "Final Grade Selection For Item Type - " + _repository.SysItemTypeRepository.GetByID(model.ItemTypeID == null ? 0 : Convert.ToInt32(model.ItemTypeID)).ItemTypeName);
            }
            reportDocument.ExportToHttpResponse(
                Request.Form["ReportType"].ToString().Equals("PDF")
                    ? ExportFormatType.PortableDocFormat
                    : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, "crReport");


            return View();
        }

        [CheckUserAccess("Reports/FinalGradeSelectionReport")]
        public ActionResult FinalGradeSelectionByGrade()
        {
            ViewBag.ItemTypeID = new SelectList(_repository.SysItemTypeRepository.Get(filter: ob => ob.IsActive), "ItemTypeID", "ItemTypeName");
            ViewBag.GradeID = new SelectList(_repository.SysGrade.Get(filter: ob => ob.IsActive), "GradeID", "GradeName");
            ViewBag.GradeRangeID = new SelectList(_dalSysGradeRange.GetAll(), "GradeRangeID", "GradeRangeName");
            return View();
        }
        [HttpPost]
        public ActionResult FinalGradeSelectionByGrade(ReportModel model)
        {
            ViewBag.Message = "";
            ViewBag.ItemTypeID = new SelectList(_repository.SysItemTypeRepository.Get(filter: ob => ob.IsActive), "ItemTypeID", "ItemTypeName");
            ViewBag.GradeID = new SelectList(_repository.SysGrade.Get(filter: ob => ob.IsActive), "GradeID", "GradeName");
            ViewBag.GradeRangeID = new SelectList(_dalSysGradeRange.GetAll(), "GradeRangeID", "GradeRangeName");


            ReportDocument reportDocument = new ReportDocument();
            string rptPath = Server.MapPath("~/Reports");
            var dataTableObj = new DataTable();
            var reportObj = new DalReport();
            var GradeRangeID = Request.Form["GradeRangeID"] == null ? "-- Select One --" : Request.Form["GradeRangeID"];

            if (string.IsNullOrEmpty(model.ItemTypeID))
            {
                dataTableObj = _dalReport.FinalGradeSelectionByGrade(model);
                rptPath = rptPath + "/FinalGradeSelectionByGrade.rpt";
                reportDocument.Load(rptPath);
                reportDocument.SetDataSource(dataTableObj);
                reportDocument.Refresh();
                if (model.GradeRangeID == "")
                {
                    if (string.IsNullOrEmpty(model.DateFrom) && string.IsNullOrEmpty(model.DateTo))
                    {
                        reportDocument.SetParameterValue("Title", "Final Grade Selection For Grade" + _repository.SysGrade.GetByID(model.GradeID == null ? 0 : Convert.ToInt32(model.GradeID)).GradeName + " (All).");
                    }
                    else
                    {
                        reportDocument.SetParameterValue("Title", "Final Grade Selection For Grade" + _repository.SysGrade.GetByID(model.GradeID == null ? 0 : Convert.ToInt32(model.GradeID)).GradeName + "(" + model.DateFrom + " to " + model.DateTo + ")");
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(model.DateFrom) && string.IsNullOrEmpty(model.DateTo))
                    {
                        reportDocument.SetParameterValue("Title", "Final Grade Selection For Grade Range (All).");
                    }
                    else
                    {
                        reportDocument.SetParameterValue("Title", "Final Grade Selection For Grade Range " + "(" + model.DateFrom + " to " + model.DateTo + ")");
                    }
                }
            }
            else
            {
                dataTableObj = _dalReport.FinalGradeSelectionByItemType(model);
                rptPath = rptPath + "/FinalGradeSelectionByItemTypeContainer.rpt";
                reportDocument.Load(rptPath);
                reportDocument.SetDataSource(dataTableObj);
                reportDocument.Refresh();
                if (string.IsNullOrEmpty(model.DateFrom) && string.IsNullOrEmpty(model.DateTo))
                {
                    reportDocument.SetParameterValue("Title", "Final Grade Selection For Item Type - " + _repository.SysItemTypeRepository.GetByID(model.ItemTypeID == null ? 0 : Convert.ToInt32(model.ItemTypeID)).ItemTypeName + " (All).");
                }
                else
                {
                    reportDocument.SetParameterValue("Title", "Final Grade Selection For Item Type - " + _repository.SysItemTypeRepository.GetByID(model.ItemTypeID == null ? 0 : Convert.ToInt32(model.ItemTypeID)).ItemTypeName + " (" + model.DateFrom + " to " + model.DateTo + ")");
                }

            }

            reportDocument.ExportToHttpResponse(
                Request.Form["ReportType"].ToString().Equals("PDF")
                    ? ExportFormatType.PortableDocFormat
                    : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, "crReport");
            return View();
        }



        public ActionResult WbStockReport()
        {
            ViewBag.Store = objStore.GetAllActiveStore();
            ViewBag.ItemType = objItemType.GetAllActiveItemTypeLeather();
            ViewBag.LeatherType = objLeatherType.GetAllActiveLeatherType();
            return View();
        }
        [CheckUserAccess("Reports/ChemicalStockIssueRec")]
        public ActionResult ChemicalStockIssueRec()
        {
            ViewBag.Store = _repository.StoreRepository.Get(filter: ob => ob.IsActive == true && ob.IsDelete == false && ob.StoreCategory == "Chemical");
            ViewBag.ItemType = from t in _repository.SysChemicalItemRepository.Get(filter: ob => ob.IsActive == true).OrderBy(ob => ob.ItemName)
                               select new
                               {
                                   ItemTypeID = t.ItemID,
                                   ItemTypeName = t.ItemName
                               };
            return View();
        }
        [HttpPost]
        public ActionResult ChemicalStockIssueRec(ReportModel model)
        {
            ViewBag.Message = "";
            ViewBag.Store = _repository.StoreRepository.Get(filter: ob => ob.IsActive == true && ob.IsDelete == false && ob.StoreCategory == "Chemical");

            ReportDocument reportDocument = new ReportDocument();
            string rptPath = Server.MapPath("~/Reports");
            var dataTableObj = new DataTable();
            var reportObj = new DalReport();
            if (model.ReportName == "Filter")
            {
                rptPath = rptPath + "/ChemicalStockIssueRec.rpt";
                dataTableObj = _dalReport.ChemicalStockIssueRec(model);
            }
            else
            {
                rptPath = rptPath + "/ChemicalStockIssueRecByStore.rpt";
                dataTableObj = _dalReport.ChemicalStockIssueRec(model);
            }

            reportDocument.Load(rptPath);
            reportDocument.SetDataSource(dataTableObj);
            reportDocument.Refresh();
            if (string.IsNullOrEmpty(model.DateFrom) || string.IsNullOrEmpty(model.DateTo))
            {
                reportDocument.SetParameterValue("Title", "Chemical Stock Issue Receive - All");
            }
            else
            {
                reportDocument.SetParameterValue("Title", "Chemical Stock Issue Receive " + model.DateFrom + " To " + model.DateTo);
            }

            reportDocument.ExportToHttpResponse(
                Request.Form["ReportType"].ToString().Equals("PDF")
                    ? ExportFormatType.PortableDocFormat
                    : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, "crReport");

            return View();
        }

        [CheckUserAccess("Reports/WBStockIssueReport")]
        public ActionResult WBStockIssueReport()
        {
            ViewBag.Message = "";
            ViewBag.StoreID = new SelectList((from t in (_repository.StoreRepository.Get(filter: ob => ob.IsActive == true && ob.IsDelete == false && ob.StoreType == "Wet Blue")).ToList() select new { t.StoreID, t.StoreName }), "StoreID", "StoreName");
            //ViewBag.SupplierID = new SelectList(_repository.SysSupplierRepository.Get(filter: ob => ob.SupplierCategory == "Leather" && ob.IsActive).OrderBy(x => x.SupplierName).ToList(), "SupplierID", "SupplierName");
            // ViewBag.GradeRangeID = new SelectList(_repository.SysGrade.Get(), "GradeID", "GradeName");
            return View();

        }
        [HttpPost]
        public ActionResult WBStockIssueReport(ReportModel model)
        {
            ViewBag.Message = "";
            //ViewBag.SupplierID = new SelectList(_repository.SysSupplierRepository.Get(filter: ob => ob.SupplierCategory == "Leather" && ob.IsActive).OrderBy(x => x.SupplierName).ToList(), "SupplierID", "SupplierName");
            ViewBag.StoreID = new SelectList((from t in (_repository.StoreRepository.Get(filter: ob => ob.IsActive == true && ob.IsDelete == false && ob.StoreType == "Wet Blue")).ToList() select new { t.StoreID, t.StoreName }), "StoreID", "StoreName");
            ReportDocument reportDocument = new ReportDocument();
            string rptPath = Server.MapPath("~/Reports");
            var dataTableObj = new DataTable();
            var reportObj = new DalReport();
            if (model.ReportName == "GradeWise")
            {
                rptPath = rptPath + "/WBStockIssueGradeReport.rpt";
                dataTableObj = _dalReport.WBStockIssueGradeReport(model);
            }
            else if (model.ReportName == "LeatherStatusWise")
            {
                rptPath = rptPath + "/WBStockIssueLeatherStatusReport.rpt";
                dataTableObj = _dalReport.WBStockIssueStatusReport(model);
            }
            else
            {
                rptPath = rptPath + "/WBStockIssueSupplierReport.rpt";
                dataTableObj = _dalReport.WBStockIssueSupplierReport(model);
            }

            reportDocument.Load(rptPath);
            reportDocument.SetDataSource(dataTableObj);
            reportDocument.Refresh();
            //if (string.IsNullOrEmpty(model.DateFrom) || string.IsNullOrEmpty(model.DateTo))
            //{
            //    reportDocument.SetParameterValue("Title", "Chemical Stock Issue Receive - All");
            //}
            //else
            //{
            //    reportDocument.SetParameterValue("Title", "Chemical Stock Issue Receive " + model.DateFrom + " To " + model.DateTo);
            //}

            reportDocument.ExportToHttpResponse(
                Request.Form["ReportType"].ToString().Equals("PDF")
                    ? ExportFormatType.PortableDocFormat
                    : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, "crReport");

            return View();
        }
        [CheckUserAccess("Reports/WBStockRequisitionReport")]
        public ActionResult WBStockRequisitionReport()
        {
            return View();

        }
        [HttpPost]
        public ActionResult WBStockRequisitionReport(ReportModel model)
        {
            ReportDocument reportDocument = new ReportDocument();
            string rptPath = Server.MapPath("~/Reports");
            var dataTableObj = new DataTable();
            if (model.ReportName.Equals("DateWise"))
            {
                rptPath = rptPath + "/WBStockRequisitionByDateReport.rpt";
            }
            else
            {
                rptPath = rptPath + "/WBStockRequisitionReport.rpt";
            }
            dataTableObj = _dalReport.UspGetWBRequisition(model);
            reportDocument.Load(rptPath);
            reportDocument.SetDataSource(dataTableObj);
            reportDocument.Refresh();

            if (string.IsNullOrEmpty(model.BuyerName))
            {
                model.BuyerName = "All";
            }

            if (string.IsNullOrEmpty(model.DateFrom) || string.IsNullOrEmpty(model.DateTo))
            {
                reportDocument.SetParameterValue("Title", "Buyer- " + model.BuyerName + ": All WB Stock Requisition.");
            }
            else
            {
                reportDocument.SetParameterValue("Title", "Buyer:" + model.BuyerName + ": WB Stock Requisition From " + model.DateFrom + " To " + model.DateTo + "");
            }
            reportDocument.ExportToHttpResponse(
                Request.Form["ReportType"].ToString().Equals("PDF")
                    ? ExportFormatType.PortableDocFormat
                    : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, "crReport");

            return View();
        }

        [CheckUserAccess("Reports/ChemicalPI")]
        public ActionResult ChemicalPI()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChemicalPI(ReportModel model)
        {
            long id = 0;
            if (Request.Form["PIID"] != null)
            {
                id = Convert.ToInt64(Request.Form["PIID"]);
                ReportDocument reportDocument = new ReportDocument();
                string rptPath = Server.MapPath("~/Reports");
                var dataTableObj = new DataTable();
                var reportObj = new DalReport();

                dataTableObj = reportObj.UspGetChemicalPI(id, model);

                rptPath = rptPath + "/ChemicalPIContainer.rpt";
                reportDocument.Load(rptPath);
                reportDocument.SetDataSource(dataTableObj);
                reportDocument.Refresh();
                //reportDocument.SetParameterValue("BDVNo2", bdvNo.ToString());
                //  reportDocument.SetParameterValue("supplier", supplier.ToString());
                reportDocument.ExportToHttpResponse(
                                 Request.Form["ReportType"].ToString().Equals("PDF")
                                     ? ExportFormatType.PortableDocFormat
                                     : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, "crReport");
            }




            return View("ChemicalPI");
        }

        [CheckUserAccess("Reports/CommercialInvoice")]
        public ActionResult CommercialInvoice()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CommercialInvoice(ReportModel model)
        {
            int id = 0;
            if (Request.Form["CIID"] != null)
            {
                id = Convert.ToInt32(Request.Form["CIID"]);
                ReportDocument reportDocument = new ReportDocument();
                string rptPath = Server.MapPath("~/Reports");
                var dataTableObj = new DataTable();
                var reportObj = new DalReport();

                dataTableObj = reportObj.UspCommercialInvoice(id, model);

                rptPath = rptPath + "/CommercialInvoice.rpt";
                reportDocument.Load(rptPath);
                reportDocument.SetDataSource(dataTableObj);
                reportDocument.Refresh();
                //reportDocument.SetParameterValue("BDVNo2", bdvNo.ToString());
                //  reportDocument.SetParameterValue("supplier", supplier.ToString());
                reportDocument.ExportToHttpResponse(
                                 Request.Form["ReportType"].ToString().Equals("PDF")
                                     ? ExportFormatType.PortableDocFormat
                                     : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, "crReport");
            }




            return View("CommercialInvoice");
        }
        //  [CheckUserAccess("Reports/PackingList")]
        public ActionResult PackingList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PackingList(ReportModel model)
        {
            int id = 0;
            if (Request.Form["PLID"] != null)
            {
                id = Convert.ToInt32(Request.Form["PLID"]);
                ReportDocument reportDocument = new ReportDocument();
                string rptPath = Server.MapPath("~/Reports");
                var dataTableObj = new DataTable();
                var reportObj = new DalReport();

                dataTableObj = reportObj.UspPackingList(id, model);

                rptPath = rptPath + "/PackingListReportContainer.rpt";
                reportDocument.Load(rptPath);
                reportDocument.SetDataSource(dataTableObj);
                reportDocument.Refresh();
                //reportDocument.SetParameterValue("BDVNo2", bdvNo.ToString());
                //  reportDocument.SetParameterValue("supplier", supplier.ToString());
                reportDocument.ExportToHttpResponse(
                                 Request.Form["ReportType"].ToString().Equals("PDF")
                                     ? ExportFormatType.PortableDocFormat
                                     : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, "crReport");
            }
            return View("PackingList");
        }
        [CheckUserAccess("Reports/BillOfLading")]
        public ActionResult BillOfLading()
        {
            return View();
        }
        [HttpPost]
        public ActionResult BillOfLading(ReportModel model)
        {
            int id = 0;
            if (Request.Form["BLID"] != null)
            {
                id = Convert.ToInt32(Request.Form["BLID"]);
                ReportDocument reportDocument = new ReportDocument();
                string rptPath = Server.MapPath("~/Reports");
                var dataTableObj = new DataTable();
                var reportObj = new DalReport();

                dataTableObj = reportObj.UspBillOfLadingReport(id, model);

                rptPath = rptPath + "/BillOfLadingContainer.rpt";
                reportDocument.Load(rptPath);
                reportDocument.SetDataSource(dataTableObj);
                reportDocument.Refresh();
                //reportDocument.SetParameterValue("BDVNo2", bdvNo.ToString());
                //  reportDocument.SetParameterValue("supplier", supplier.ToString());
                reportDocument.ExportToHttpResponse(
                                 Request.Form["ReportType"].ToString().Equals("PDF")
                                     ? ExportFormatType.PortableDocFormat
                                     : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, "crReport");
            }
            return View("BillOfLading");
        }

        [CheckUserAccess("Reports/CNFBillReport")]
        public ActionResult CNFBillReport()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CNFBillReport(ReportModel model)
        {
            int id = 0;
            if (Request.Form["CnfBillID"] != null)
            {
                id = Convert.ToInt32(Request.Form["CnfBillID"]);
                ReportDocument reportDocument = new ReportDocument();
                string rptPath = Server.MapPath("~/Reports");
                var dataTableObj = new DataTable();
                var reportObj = new DalReport();

                dataTableObj = reportObj.UspCNFBillReport(id, model);

                rptPath = rptPath + "/CNFBillReport.rpt";
                reportDocument.Load(rptPath);
                reportDocument.SetDataSource(dataTableObj);
                reportDocument.Refresh();
                //reportDocument.SetParameterValue("BDVNo2", bdvNo.ToString());
                //  reportDocument.SetParameterValue("supplier", supplier.ToString());
                reportDocument.ExportToHttpResponse(
                                 Request.Form["ReportType"].ToString().Equals("PDF")
                                     ? ExportFormatType.PortableDocFormat
                                     : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, "crReport");
            }
            return View("CNFBillReport");
        }

        [CheckUserAccess("Reports/LimInfoReport")]
        public ActionResult LimInfoReport()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LimInfoReport(ReportModel model)
        {
            int id = 0;
            if (Request.Form["LimID"] != null)
            {
                id = Convert.ToInt32(Request.Form["LimID"]);
                ReportDocument reportDocument = new ReportDocument();
                string rptPath = Server.MapPath("~/Reports");
                var dataTableObj = new DataTable();
                var reportObj = new DalReport();

                dataTableObj = reportObj.UspLimInfoReport(id, model);

                rptPath = rptPath + "/LimInfoReport.rpt";
                reportDocument.Load(rptPath);
                reportDocument.SetDataSource(dataTableObj);
                reportDocument.Refresh();
                //reportDocument.SetParameterValue("BDVNo2", bdvNo.ToString());
                //  reportDocument.SetParameterValue("supplier", supplier.ToString());
                reportDocument.ExportToHttpResponse(
                                 Request.Form["ReportType"].ToString().Equals("PDF")
                                     ? ExportFormatType.PortableDocFormat
                                     : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, "crReport");
            }
            return View("LimInfoReport");
        }

        [CheckUserAccess("Reports/LCRetirementReport")]
        public ActionResult LCRetirementReport()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LCRetirementReport(ReportModel model)
        {
            int id = 0;
            if (Request.Form["LCRetirementID"] != null)
            {
                id = Convert.ToInt32(Request.Form["LCRetirementID"]);
                ReportDocument reportDocument = new ReportDocument();
                string rptPath = Server.MapPath("~/Reports");
                var dataTableObj = new DataTable();
                var reportObj = new DalReport();

                dataTableObj = reportObj.UspLCRetirementReport(id, model);

                rptPath = rptPath + "/LCRetirementReport.rpt";
                reportDocument.Load(rptPath);
                reportDocument.SetDataSource(dataTableObj);
                reportDocument.Refresh();
                //reportDocument.SetParameterValue("BDVNo2", bdvNo.ToString());
                //  reportDocument.SetParameterValue("supplier", supplier.ToString());
                reportDocument.ExportToHttpResponse(
                                 Request.Form["ReportType"].ToString().Equals("PDF")
                                     ? ExportFormatType.PortableDocFormat
                                     : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, "crReport");
            }
            return View("ChemicalPI");
        }


        //*************************** Export Report Regarding Method **********************

        [CheckUserAccess("Reports/EXPCNFBillReport")]
        public ActionResult EXPCNFBillReport()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EXPCNFBillReport(ReportModel model)
        {
            int id = 0;
            if (Request.Form["CnfBillID"] != null)
            {
                id = Convert.ToInt32(Request.Form["CnfBillID"]);
                ReportDocument reportDocument = new ReportDocument();
                string rptPath = Server.MapPath("~/Reports");
                var dataTableObj = new DataTable();
                var reportObj = new DalReport();

                dataTableObj = reportObj.GetExportCnFBillReport(id, model);

                rptPath = rptPath + "/EXPCNFBillReport.rpt";
                reportDocument.Load(rptPath);
                reportDocument.SetDataSource(dataTableObj);
                reportDocument.Refresh();
                //reportDocument.SetParameterValue("BDVNo2", bdvNo.ToString());
                //  reportDocument.SetParameterValue("supplier", supplier.ToString());
                reportDocument.ExportToHttpResponse(
                                 Request.Form["ReportType"].ToString().Equals("PDF")
                                     ? ExportFormatType.PortableDocFormat
                                     : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, "crReport");
            }
            return View("EXPCNFBillReport");
        }

        //[CheckUserAccess("Reports/EXPLCRetirement")]
        public ActionResult EXPLCRetirement()
        {
            return View();
        }
        [HttpPost]
        public ActionResult EXPLCRetirement(ReportModel model)
        {
            int id = 0;
            if (Request.Form["[LCRetirementID]"] != null)
            {
                id = Convert.ToInt32(Request.Form["[LCRetirementID]"]);
                ReportDocument reportDocument = new ReportDocument();
                string rptPath = Server.MapPath("~/Reports");
                var dataTableObj = new DataTable();
                var reportObj = new DalReport();

                dataTableObj = reportObj.GetExportLCretirementReport(id, model);

                rptPath = rptPath + "/EXPLCRetirementReport.rpt";
                reportDocument.Load(rptPath);
                reportDocument.SetDataSource(dataTableObj);
                reportDocument.Refresh();

                reportDocument.ExportToHttpResponse(
                                 Request.Form["ReportType"].ToString().Equals("PDF")
                                     ? ExportFormatType.PortableDocFormat
                                     : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, "crReport");
            }
            return View("EXPLCRetirement");
        }

        //[CheckUserAccess("Reports/EXPDeliveryChallan")]
        public ActionResult EXPDeliveryChallan()
        {
            return View();
        }
        [HttpPost]
        public ActionResult EXPDeliveryChallan(ReportModel model)
        {
            int id = 0;
            if (Request.Form["DeliverChallanID"] != null)
            {
                id = Convert.ToInt32(Request.Form["DeliverChallanID"]);
                ReportDocument reportDocument = new ReportDocument();
                string rptPath = Server.MapPath("~/Reports");
                var dataTableObj = new DataTable();
                var reportObj = new DalReport();

                dataTableObj = reportObj.GetExportDeliveryChallanReport(id, model);

                rptPath = rptPath + "/EXPDeliveryChallanReport.rpt";
                reportDocument.Load(rptPath);
                reportDocument.SetDataSource(dataTableObj);
                reportDocument.Refresh();

                reportDocument.ExportToHttpResponse(
                                 Request.Form["ReportType"].ToString().Equals("PDF")
                                     ? ExportFormatType.PortableDocFormat
                                     : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, "crReport");
            }
            return View("EXPDeliveryChallan");
        }


        //************************************ END Export Report Regarding Method ****************
        public ActionResult ExpPackingListRpt()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ExpPackingListRpt(ReportModel model)
        {
            int id = 0;
            if (Request.Form["PLID"] != null)
            {
                id = Convert.ToInt32(Request.Form["PLID"]);
                ReportDocument reportDocument = new ReportDocument();
                string rptPath = Server.MapPath("~/Reports");
                var dataTableObj = new DataTable();
                var reportObj = new DalReport();

                dataTableObj = reportObj.UspExpPackingListReport(id, model);

                rptPath = rptPath + "/ExpPackingListRpt.rpt";
                reportDocument.Load(rptPath);
                reportDocument.SetDataSource(dataTableObj);
                reportDocument.Refresh();

                reportDocument.ExportToHttpResponse(
                                 Request.Form["ReportType"].ToString().Equals("PDF")
                                     ? ExportFormatType.PortableDocFormat
                                     : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, "crReport");
            }
            return View("ExpPackingListRpt");
        }


        //************************************ END Export Report Regarding Method ****************

        //************************************ Start Crust Lather Report Regarding Method ****************
        public ActionResult CrustLeatherIssueRpt()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CrustLeatherIssueRpt(ReportModel model)
        {
            ReportDocument reportDocument = new ReportDocument();
            string rptPath = Server.MapPath("~/Reports");
            var dataTableObj = new DataTable();
            var reportObj = new DalReport();
            if (model.ReportName.Equals("Challan"))
            {
                model.ArticalID = "0";
                dataTableObj = reportObj.UspCrustLeatherIssueByArticalChallanNo(model);
                rptPath = rptPath + "/CrustLeatherIssueByChalArtRpt.rpt";
                reportDocument.Load(rptPath);
                reportDocument.SetDataSource(dataTableObj);
                reportDocument.Refresh();
                if (string.IsNullOrEmpty(model.DateFrom) && string.IsNullOrEmpty(model.DateTo))
                {
                    reportDocument.SetParameterValue("Title", "Crusted Leather Challan Wise Issue Report.");
                }
                else
                { reportDocument.SetParameterValue("Title", "Crusted Leather Challan Wise Issue Report " + model.DateFrom + " To " + model.DateTo + ""); }

            }
            else if (model.ReportName.Equals("BuyerIssue"))
            {
                model.ArticalID = "0";
                dataTableObj = reportObj.UspCrustLeatherIssueByArticalChallanNo(model);
                rptPath = rptPath + "/CrustLeatherIssueByBuyer.rpt";
                reportDocument.Load(rptPath);
                reportDocument.SetDataSource(dataTableObj);
                reportDocument.Refresh();
                if (string.IsNullOrEmpty(model.DateFrom) && string.IsNullOrEmpty(model.DateTo))
                {
                    reportDocument.SetParameterValue("Title", "Crusted Leather Buyer Wise Issue Report.");
                }
                else
                { reportDocument.SetParameterValue("Title", "Crusted Leather Challan Buyer Issue Report " + model.DateFrom + " To " + model.DateTo + ""); }

            }
            else if (model.ReportName.Equals("ChallanStock"))
            {
         
                dataTableObj = reportObj.UspCrustStockReport(model);
                rptPath = rptPath + "/CrustLeatherStockByChalArtRpt.rpt";
                reportDocument.Load(rptPath);
                reportDocument.SetDataSource(dataTableObj);
                reportDocument.Refresh();
                if (string.IsNullOrEmpty(model.DateFrom) && string.IsNullOrEmpty(model.DateTo))
                {
                    reportDocument.SetParameterValue("Title", "Crusted Leather Challan Wise Stock Report.");
                }
                else
                { reportDocument.SetParameterValue("Title", "Crusted Leather Challan Wise Stock Report " + model.DateFrom + " To " + model.DateTo + ""); }

            }
            else if (model.ReportName.Equals("ArticleStock"))
            {
      
                dataTableObj = reportObj.UspCrustStockReport(model);
                rptPath = rptPath + "/CrustLeatherStockByChalArtRpt.rpt";
                reportDocument.Load(rptPath);
                reportDocument.SetDataSource(dataTableObj);
                reportDocument.Refresh();
                if (string.IsNullOrEmpty(model.DateFrom) && string.IsNullOrEmpty(model.DateTo))
                {
                    reportDocument.SetParameterValue("Title", "Crusted Leather Article Wise Stock Report.");
                }
                else
                { reportDocument.SetParameterValue("Title", "Crusted Leather Article Wise Issue Report " + model.DateFrom + " To " + model.DateTo + ""); }

            }
            else if (model.ReportName.Equals("Article"))
            {
                model.ArticleChallanNo = "";
                dataTableObj = reportObj.UspCrustLeatherIssueByArticalChallanNo(model);
                rptPath = rptPath + "/CrustLeatherIssueByChalArtRpt.rpt";
                reportDocument.Load(rptPath);
                reportDocument.SetDataSource(dataTableObj);
                reportDocument.Refresh();
                if (string.IsNullOrEmpty(model.DateFrom) && string.IsNullOrEmpty(model.DateTo))
                {
                    reportDocument.SetParameterValue("Title", "Crusted Leather Issue Article wise Report.");
                }
                else
                { reportDocument.SetParameterValue("Title", "Crusted Leather Issue Report " + model.DateFrom + " To " + model.DateTo + ""); }

            }
            else if (model.ReportName.Equals("Daily"))
            {
                model.ArticleChallanNo = "";
                dataTableObj = reportObj.UspCrustStockReport(model);
                rptPath = rptPath + "/DailyCrustStock.rpt";
                reportDocument.Load(rptPath);
                reportDocument.SetDataSource(dataTableObj);
                reportDocument.Refresh();
                if (string.IsNullOrEmpty(model.DateFrom) && string.IsNullOrEmpty(model.DateTo))
                {
                    reportDocument.SetParameterValue("Title", "Crusted Leather Daily Stock Report.");
                }
                else
                { reportDocument.SetParameterValue("Title", "Crusted Leather Daily Stock Report " + model.DateFrom + " To " + model.DateTo + ""); }

            }
            else if (model.ReportName.Equals("Buyer"))
            {
                model.ArticleChallanNo = "";
                dataTableObj = reportObj.UspCrustStockReport(model);
                rptPath = rptPath + "/CrustBuyerWiseStockRpt.rpt";
                reportDocument.Load(rptPath);
                reportDocument.SetDataSource(dataTableObj);
                reportDocument.Refresh();
                if (string.IsNullOrEmpty(model.BuyerName))
                {
                    reportDocument.SetParameterValue("Title", "Crusted Leather Buyer Wise Stock.");
                }
                else
                { reportDocument.SetParameterValue("Title", "Crusted Leather Buyer Wise Stock Report for Buyer:" + model.BuyerName); }

            }
            else
            {
                dataTableObj = reportObj.UspGetAllCrustLeatherIssueReport(model);
                rptPath = rptPath + "/CrustLeatherIssueRpt.rpt";
                reportDocument.Load(rptPath);
                reportDocument.SetDataSource(dataTableObj);
                reportDocument.Refresh();
                if (string.IsNullOrEmpty(model.DateFrom) && string.IsNullOrEmpty(model.DateTo))
                {
                    reportDocument.SetParameterValue("Title", "Crusted Leather Issue Report.");
                }
                else
                { reportDocument.SetParameterValue("Title", "Crusted Leather Issue Report " + model.DateFrom + " To " + model.DateTo + ""); }

            }



            reportDocument.ExportToHttpResponse(
                             Request.Form["ReportType"].ToString().Equals("PDF")
                                 ? ExportFormatType.PortableDocFormat
                                 : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, "crReport");

            return View("CrustLeatherIssueRpt");
        }
        //************************************ END Crust Lather Report Regarding Method ****************

        //************************************ START Finish Leather Issue Report Regarding Method ****************
        public ActionResult FinishedLeatherIssueRpt()
        {
            return View();
        }
        [HttpPost]
        public ActionResult FinishedLeatherIssueRpt(ReportModel model)
        {
            ReportDocument reportDocument = new ReportDocument();
            string rptPath = Server.MapPath("~/Reports");
            var dataTableObj = new DataTable();
            var reportObj = new DalReport();


            switch (model.ReportName)
            {
                case "AllIssues":
                    dataTableObj = reportObj.UspGetAllFinishLeatherIssueReport(model);
                    rptPath = rptPath + "/FinishedLeatherIssueRpt.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    if (string.IsNullOrEmpty(model.DateFrom))
                    {
                        reportDocument.SetParameterValue("Title", "All Finished Leather Issue Report.");
                    }
                    else
                    {
                        reportDocument.SetParameterValue("Title", "Finished Leather Issue Report " + model.DateFrom + " To " + model.DateTo + "");
                    }
                    break;
                case "ChallanWiseIssue":
                  
                    dataTableObj = reportObj.UspFinishLeatherIssueByArtiChalNo(model);
                    rptPath = rptPath + "/FinishLeatherIssueByArtiChalan.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    if (string.IsNullOrEmpty(model.DateFrom))
                    {
                        reportDocument.SetParameterValue("Title", "Finished Leather Challan Wise Issue.");
                    }
                    else
                    {
                        reportDocument.SetParameterValue("Title", "Finished Leather Challan Wise Issue " + model.DateFrom + " To " + model.DateTo + "");
                    }
                    break;
                case "ArticleWiseIssue":
                   
                    dataTableObj = reportObj.UspFinishLeatherIssueByArtiChalNo(model);
                    rptPath = rptPath + "/FinishLeatherIssueByArtiChalan.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    if (string.IsNullOrEmpty(model.DateFrom))
                    {
                        reportDocument.SetParameterValue("Title", "Finished Leather Article Wise Issue.");
                    }
                    else
                    {
                        reportDocument.SetParameterValue("Title", "Finished Leather Article Wise Issue " + model.DateFrom + " To " + model.DateTo + "");
                    }
                    break;

                case "BuyerWiseIssue":
                   
                    dataTableObj = reportObj.UspFinishLeatherIssueByArtiChalNo(model);
                    rptPath = rptPath + "/FinishLeatherIssueByBuyer.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    if (string.IsNullOrEmpty(model.DateFrom))
                    {
                        reportDocument.SetParameterValue("Title", "Finished Leather Buyer Wise Issue.");
                    }
                    else
                    {
                        reportDocument.SetParameterValue("Title", "Finished Leather Buyer Wise Issue " + model.DateFrom + " To " + model.DateTo + "");
                    }
                    break;
                case "DailyIssue":

                    dataTableObj = reportObj.UspFinishLeatherIssueByArtiChalNo(model);
                    rptPath = rptPath + "/FinishLeatherIssueByDaily.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    if (string.IsNullOrEmpty(model.DateFrom))
                    {
                        reportDocument.SetParameterValue("Title", "Finished Leather Daily Issue.");
                    }
                    else
                    {
                        reportDocument.SetParameterValue("Title", "Finished Leather Daily Issue " + model.DateFrom + " To " + model.DateTo + "");
                    }
                    break;
                case "ChallanWiseStock":

                    dataTableObj = reportObj.UspFinishLeatherStockByArtiChalNo(model);
                    rptPath = rptPath + "/FinishLeatherStockByArticalChalan.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    if (string.IsNullOrEmpty(model.DateFrom))
                    {
                        reportDocument.SetParameterValue("Title", "Finished Leather Challan Wise Stock.");
                    }
                    else
                    {
                        reportDocument.SetParameterValue("Title", "Finished Leather Challan Wise Stock " + model.DateFrom + " To " + model.DateTo + "");
                    }
                    break;
                case "ArticalWiseStock":

                    dataTableObj = reportObj.UspFinishLeatherStockByArtiChalNo(model);
                    rptPath = rptPath + "/FinishLeatherStockByArticalChalan.rpt";
                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    if (string.IsNullOrEmpty(model.DateFrom))
                    {
                        reportDocument.SetParameterValue("Title", "Finished Leather Artical Wise Stock.");
                    }
                    else
                    {
                        reportDocument.SetParameterValue("Title", "Finished Leather Artical Wise Stock " + model.DateFrom + " To " + model.DateTo + "");
                    }
                    break;
                case "DailyStock":
               
                    dataTableObj = reportObj.UspFinishLeaterStockRpt(model);
                    if (Convert.ToInt32(model.ArticleChallanID) > 0)
                    {
                        rptPath = rptPath + "/FinishLeatherDailyGroupStock.rpt";
                    }
                    else
                    { rptPath = rptPath + "/FinishLeatherDailyStock.rpt"; }

                    reportDocument.Load(rptPath);
                    reportDocument.SetDataSource(dataTableObj);
                    reportDocument.Refresh();
                    if (string.IsNullOrEmpty(model.DateFrom))
                    {
                        reportDocument.SetParameterValue("Title", "Title: Finished Leather Daily Stock Report.");
                    }
                    else
                    {
                        reportDocument.SetParameterValue("Title", "Title: Finished Leather Daily Stock Report " + model.DateFrom + " To " + model.DateTo + "");
                    }
                    break;


            }

            reportDocument.ExportToHttpResponse(
                             Request.Form["ReportType"].ToString().Equals("PDF")
                                 ? ExportFormatType.PortableDocFormat
                                 : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, "crReport");

            return View("FinishedLeatherIssueRpt");
        }
        //************************************ END Finish Leather Issue Report Regarding Method ****************

        //************************************ Export Commercial Invoice Report ****************
        [CheckUserAccess("Reports/EXPCIReports")]
        public ActionResult EXPCIReports()
        {
            return View();
        }
        [HttpPost]
        public ActionResult EXPCIReports(ReportModel model)
        {
            int id = 0;
            if (Request.Form["CIID"] != null)
            {
                id = Convert.ToInt32(Request.Form["CIID"]);
                ReportDocument reportDocument = new ReportDocument();
                string rptPath = Server.MapPath("~/Reports");
                var dataTableObj = new DataTable();
                var reportObj = new DalReport();

                dataTableObj = reportObj.UspEXPCIReport(id, model);

                rptPath = rptPath + "/EXPCIReport.rpt";
                reportDocument.Load(rptPath);
                reportDocument.SetDataSource(dataTableObj);
                reportDocument.Refresh();

                reportDocument.ExportToHttpResponse(
                                 Request.Form["ReportType"].ToString().Equals("PDF")
                                     ? ExportFormatType.PortableDocFormat
                                     : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, "crReport");
            }
            return View("EXPCIReports");
        }


        //************************************ END Export Commercial Invoice Report ****************


        //************************************ Crust Leather Report ****************

        ////[HttpPost]
        ////public ActionResult CrustLeatherDailyStock(ReportModel model)
        ////{
        ////    int id = 0;
        ////    if (Request.Form["CIID"] != null)
        ////    {
        ////        id = Convert.ToInt32(Request.Form["CIID"]);
        ////        ReportDocument reportDocument = new ReportDocument();
        ////        string rptPath = Server.MapPath("~/Reports");
        ////        var dataTableObj = new DataTable();
        ////        var reportObj = new DalReport();

        ////        dataTableObj = reportObj.UspEXPCIReport(id, model);

        ////        rptPath = rptPath + "/EXPCIReport.rpt";
        ////        reportDocument.Load(rptPath);
        ////        reportDocument.SetDataSource(dataTableObj);
        ////        reportDocument.Refresh();

        ////        reportDocument.ExportToHttpResponse(
        ////                         Request.Form["ReportType"].ToString().Equals("PDF")
        ////                             ? ExportFormatType.PortableDocFormat
        ////                             : ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, "crReport");
        ////    }
        ////    return View("EXPCIReports");
        ////}


        //************************************ Crust Leather Report ****************





    }
}
