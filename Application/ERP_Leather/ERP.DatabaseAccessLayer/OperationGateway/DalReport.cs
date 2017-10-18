using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.Emit;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.OperationModel;


namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalReport
    {
        private readonly string _connString = string.Empty;
        // private readonly string _dbProvider = string.Empty;

        public DalReport()
        {

            _connString = StrConnection.GetConnectionString();
            //_dbProvider = StrConnection.GetDatabaseProvider();
        }
        public DataTable GetLeatherStockInfo(ReportModel model)
        {
            var dt = new DataTable();
            using (var conn = new SqlConnection(_connString))
            {
                conn.Open();
                switch (model.ReportName)
                {
                    case "DailyStock":
                        GetStoreProcedureValue(model, conn, dt, "UspDateWiseStock", "noParam", model.StoreID, model.ItemTypeID, "noParam", model.DateFrom, model.DateTo);
                        break;
                    case "PurchaseNProdWiseDailyStock":
                        GetStoreProcedureValue(model, conn, dt, "UspPurchaseWiseDailyStock", "noParam", "noParam", "noParam", "noParam", model.DateFrom, model.DateTo);
                        break;
                    case "ItemWiseStock":
                    case "StoreWiseItemStock":
                        GetStoreProcedureValue(model, conn, dt, "UspStoreWiseItemStock", "noParam", model.StoreID, model.ItemTypeID, "noParam", "noParam", "noParam");
                        break;
                    case "SupplierWiseAdjustRequest":
                        GetStoreProcedureValue(model, conn, dt, "UspGetPurchaseRequestInformation", model.SupplierID, model.StoreID, model.ItemTypeID, model.LeatherTypeID, model.DateFrom, model.DateTo);
                        break;
                    case "SupplierStock":
                        GetStoreProcedureValue(model, conn, dt, "UspSupplierWiseStock", model.SupplierID, model.StoreID, model.ItemTypeID, "noParam", "noParam", "noParam");
                        break;
                    case "RawHideIssueReceiveRpt":
                        GetStoreProcedureValue(model, conn, dt, "UspGetRHIssueReceiveInfo", model.SupplierID, model.StoreID, model.ItemTypeID, model.LeatherTypeID, model.DateFrom, model.DateTo);
                        break;
                }
            }
            return dt;
        }
        private static void GetStoreProcedureValue(ReportModel model, SqlConnection conn, DataTable dt, string spName, string supplierId, string storeId, string itemtypeId, string leatherTypeId, string fromDate, string toDate)
        {
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = spName;
                cmd.CommandType = CommandType.StoredProcedure;
                if (supplierId != "noParam")
                {
                    cmd.Parameters.Add("@SupplierId", SqlDbType.Int).Value = model.SupplierID == null ? 0 : Convert.ToInt32(model.SupplierID);
                }
                if (storeId != "noParam")
                {
                    cmd.Parameters.Add("@StoreId", SqlDbType.Int).Value = model.StoreID == null
                        ? 0
                        : Convert.ToInt32(model.StoreID);
                }
                if (itemtypeId != "noParam")
                {
                    cmd.Parameters.Add("@ItemTypeId", SqlDbType.Int).Value = model.ItemTypeID == null
                        ? 0
                        : Convert.ToInt32(model.ItemTypeID);
                }
                if (leatherTypeId != "noParam")
                {
                    cmd.Parameters.Add("@LeatherTypeId", SqlDbType.Int).Value = model.LeatherTypeID == null
                        ? 0
                        : Convert.ToInt32(model.LeatherTypeID);
                }
                if (fromDate != "noParam" && toDate != "noParam")
                {
                    cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                                     ? (object)null
                                     : DalCommon.SetDate(model.DateFrom);
                    cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                                ? (object)null
                                : DalCommon.SetDate(model.DateTo);
                }
                using (var adp = new SqlDataAdapter(cmd))
                {
                    adp.Fill(dt);
                }
            }
        }
        public DataTable GetSelectionReportData(int GradeRangeId, int SupplierID, int SelectionStore, int SelectionId)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("UspGradeSelectionReport", conn);
                    cmd.Parameters.Add("@GradeRangeId", SqlDbType.Int).Value = GradeRangeId;
                    cmd.Parameters.Add("@SupplierID", SqlDbType.Int).Value = SupplierID;
                    cmd.Parameters.Add("@SelectionStore", SqlDbType.Int).Value = SelectionStore;
                    cmd.Parameters.Add("@SelectionId", SqlDbType.Int).Value = SelectionId;
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dt);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;
        }

        public DataTable GetFinalGradeSelecByWBSelectID(long storeParam)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("UspFinalGradeSelecByWBSelectID", conn);
                    cmd.Parameters.Add("@WBSelectionID", SqlDbType.BigInt).Value = storeParam;
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dt);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;
        }

        public DataTable UspFinalGradeSelectionByWBSelectionID(long storeParam)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("UspFinalGradeSelectionByWBSelectionID", conn);
                    cmd.Parameters.Add("@WBSelectionID", SqlDbType.BigInt).Value = storeParam;
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dt);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;
        }

        public DataTable GetSelectionDetailsCalculationData(int supplierID, long purchaseId)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("UspPurchaseSelectionQtyCalcualtionByPurchaseId", conn);
                    cmd.Parameters.Add("@SupplierID", SqlDbType.Int).Value = supplierID;
                    cmd.Parameters.Add("@PurchaseID", SqlDbType.Int).Value = purchaseId;
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;
        }
        public DataTable GetSelectionDetailsData(int supplierID, long purchaseId)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("UspPreGradeSelectionReportByPurchaseId", conn);
                    cmd.Parameters.Add("@SupplierID", SqlDbType.Int).Value = supplierID;
                    cmd.Parameters.Add("@PurchaseID", SqlDbType.Int).Value = purchaseId;
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        public DataTable GetSupplierBillInformation(ReportBillModel billModel)
        {
            var dt = new DataTable();
            try
            {

                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    switch (billModel.ReportName)
                    {
                        case "SupplierWiseBillSummary":
                            GetStoreProcedureValue(billModel, conn, dt, "UspGetSupplierBillInformation");
                            break;
                        case "SupplierWiseBillDetail":
                            GetStoreProcedureValue(billModel, conn, dt, "UspGetSupplierBillDetailInfo");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        private static void GetStoreProcedureValue(ReportBillModel billModel, SqlConnection conn, DataTable dt, string spName)
        {
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = spName;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@SupplierId", SqlDbType.Int).Value = billModel.SupplierID == null
                    ? 0
                    : Convert.ToInt32(billModel.SupplierID);
                cmd.Parameters.Add("@PurchaseYear", SqlDbType.NChar).Value = billModel.PurchaseYear ??
                                                                             "";
                cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (billModel.FromDate == null)
                    ? (object)null
                    : DalCommon.SetDate(billModel.FromDate);
                cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (billModel.ToDate == null)
                    ? (object)null
                    : DalCommon.SetDate(billModel.ToDate);
                using (var adp = new SqlDataAdapter(cmd))
                {
                    adp.Fill(dt);
                }
            }
        }

        public DataTable GetPurchaseInformation(PurchaseReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();

                    switch (model.ReportName)
                    {
                        case "SupplierWisePurchaseSummary":
                        case "ItemWisePurchase":
                            GetStoreProcedureValue(model, conn, dt, "UspGetSupplierWisePurchaseSummary");
                            break;
                        case "PurchaseTypeWisePurchase":
                            GetStoreProcedureValue(model, conn, dt, "UspGetPurchaseTypeWisePurchaseSummary");
                            break;
                        case "SourceWisePurchase":
                            GetStoreProcedureValue(model, conn, dt, "UspGetSourceWisePurchaseSummary");
                            break;
                        case "MonthWisePurchaseSummary":
                            GetStoreProcedureValue(model, conn, dt, "UspGetMonthlyRawHidePurchase");
                            break;
                        case "MonthlyPurchaseAndWBProduction":
                            GetStoreProcedureValue(model, conn, dt, "UspGetMonthlyRhPurchaseWBProd");
                            break;
                        default:
                            GetStoreProcedureValue(model, conn, dt, "UspGetSupplierWisePurchaseDetail");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        public DataTable GetRawHideYearlyTarget(PurchaseReportModel reportModel)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();

                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "UspRawHideYearlyTarget";
                        cmd.CommandType = CommandType.StoredProcedure;


                        cmd.Parameters.Add("@PurchaseYear", SqlDbType.NChar).Value =
                            reportModel.PurchaseYear ?? "";

                        using (var adp = new SqlDataAdapter(cmd))
                        {
                            adp.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        public DataTable GetSelectionDetailsDataBySupplierID(int supplierID)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("UspPreGradeSelectionReportBySupplierId", conn);
                    cmd.Parameters.Add("@SupplierID", SqlDbType.Int).Value = supplierID;
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        public DataTable GetGradeSelectionSummaryReportByPurchaseId(int supplierId, long purchaseID, long rangeId)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("UspPreGradeSelectionSummaryReportByPurchaseId", conn);
                    cmd.Parameters.Add("@SupplierID", SqlDbType.Int).Value = supplierId;
                    cmd.Parameters.Add("@PurchaseID", SqlDbType.BigInt).Value = purchaseID;
                    cmd.Parameters.Add("@RangeID", SqlDbType.Int).Value = rangeId;
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        public DataTable LcOpeningEntryReport(string lcno)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("UspLcmLcOpeningEntryReport", conn);
                    cmd.Parameters.Add("@LCNo", SqlDbType.NVarChar).Value = lcno;
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;
        }
        public DataTable InsuranceInfoEntryReport(string InsuranceNo)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("UspLcmInsuraceInfoEntryReport", conn);
                    cmd.Parameters.Add("@InsuranceNo", SqlDbType.NVarChar).Value = InsuranceNo;
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;
        }
        public DataTable BankDebitInfoEntryReport(string BDVNo)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("UspLcmBankDebitVoucherEntryReport", conn);
                    cmd.Parameters.Add("@BDVNo", SqlDbType.NVarChar).Value = BDVNo;
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;
        }
        public DataTable GetPurchaseWithBillInformation(ReportBillModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    switch (model.ReportName)
                    {
                        case "ItemAndSupplierWiseRHBill":
                            GetStoreProcedureForPurchaseBillValue(model, conn, dt, "UspGetPurchaseWithBillInformation");
                            break;
                        case "ItemWiseRHPurchaseBill":
                        case "StoreWiseItemPrice":
                            GetStoreProcedureForPurchaseBillValue(model, conn, dt, "UspGetStoreWisePurchaseBillInformation");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        private static void GetStoreProcedureForPurchaseBillValue(ReportBillModel model, SqlConnection conn, DataTable dt, string spName)
        {
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = spName;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@supplierID", SqlDbType.Int).Value = model.SupplierID ==
                                                                         null
                    ? 0
                    : Convert.ToInt32(model.SupplierID);
                cmd.Parameters.Add("@storeID", SqlDbType.TinyInt).Value = model.StoreID == null
                    ? 0
                    : Convert.ToByte(model.StoreID);
                cmd.Parameters.Add("@itemtypeID", SqlDbType.TinyInt).Value = model.ItemTypeID ==
                                                                             null
                    ? 0
                    : Convert.ToByte(model.ItemTypeID);
                cmd.Parameters.Add("@purchaseYear", SqlDbType.NChar).Value =
                    model.PurchaseYear ?? "";
                cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.FromDate == null)
                    ? (object)null
                    : DalCommon.SetDate(model.FromDate);
                cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.ToDate == null)
                    ? (object)null
                    : DalCommon.SetDate(model.ToDate);
                using (var adp = new SqlDataAdapter(cmd))
                {
                    adp.Fill(dt);
                }
            }
        }

        public DataTable GetSupplierRhlPurchaseWithPaymentInformation(ReportBillModel billModel)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "UspGetSupplierRHLPurchaseAndPaymentInfo";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@SupplierId", SqlDbType.Int).Value = billModel.SupplierID == null
                            ? 0
                            : Convert.ToInt32(billModel.SupplierID);
                        cmd.Parameters.Add("@PurchaseYear", SqlDbType.NChar).Value = billModel.PurchaseYear ?? "";
                        cmd.Parameters.Add("@ApproximateRate", SqlDbType.Decimal).Value = billModel.ApproximateRate ==
                                                                                          ""
                            ? 0
                            : Convert.ToDecimal(billModel.ApproximateRate);
                        cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (billModel.FromDate == null)
                            ? (object)null
                            : DalCommon.SetDate(billModel.FromDate);
                        cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (billModel.ToDate == null)
                            ? (object)null
                            : DalCommon.SetDate(billModel.ToDate);
                        using (var adp = new SqlDataAdapter(cmd))
                        {
                            adp.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        public DataTable GetChemicalStockAndIssueInformation(ReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    switch (model.ReportName)
                    {
                        case "DailyStock":
                            GetStoreProcedureValue(model, conn, dt, "UspGetChemicalDailyStock", 1);
                            break;
                        case "ChemicalItemWiseStockWithoutProd":
                        case "StoreWiseChemicalItemStock":
                            GetStoreProcedureValue(model, conn, dt, "UspGetItemwiseChemicalStockWithoutProd", 2);
                            break;
                        case "ChemicalItemWiseStock":
                            GetStoreProcedureValue(model, conn, dt, "UspGetItemwiseChemicalStock",2);
                            break;
                        case "ItemCategoryWiseChemicalStock":
                            GetStoreProcedureValue(model, conn, dt, "UspGetItemCategoryWiseChemStock", 3);
                            break;
                        case "StoreWiseSupplierStock":
                        case "SupplierWiseStoreStock":
                            GetStoreProcedureValue(model, conn, dt, "UspGetSupplierwiseChemicalStock", 1);
                            break;
                        case "ChemicalStatus":
                            GetStoreProcedureValue(model, conn, dt, "UspGetChemicalStatusFromStock", 1);
                            break;
                        case "ProductionWiseChemicalStock":
                            GetStoreProcedureValue(model, conn, dt, "UspGetProductionWiseItemStock", 2);
                            break;
                           //issue releted
                        case "ChemWiseIssueSummary":
                            GetStoreProcedureValue(model, conn, dt, "UspGetChemWiseIssueSummary", 1);
                            break;
                        case "ChemIssueDetail":
                            GetStoreProcedureValue(model, conn, dt, "UspGetChemicalIssueDetail", 1);
                            break;
                        case "ChemProdStageWiseIssueDeatil":
                            GetStoreProcedureValue(model, conn, dt, "UspGetChemicalProdStageWiseIssueDetail", 1);
                            break;
                        case "ChemWiseReceiveIssueStock":
                            GetStoreProcedureValue(model, conn, dt, "UspGetChemicalWiseReceiveIssueStock", 1);
                            break;
                        case "YearMonthChemWiseReceiveIssueStock":
                            GetStoreProcedureValue(model, conn, dt, "UspGetYearMonthWiseChemReceiveIssueStock", 1);
                            break;
                        case "DateWiseChemReceiveIssueStock":
                            GetStoreProcedureValue(model, conn, dt, "UspGetDateWiseChemReceiveIssueStock", 1);
                            break;



                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        private static void GetStoreProcedureValue(ReportModel model, SqlConnection conn, DataTable dt, string spName, int type)
        {
            using (var cmd = new SqlCommand())
            {
                switch (type)
                {
                    case 1:
                        cmd.Parameters.Add("@SupplierId", SqlDbType.Int).Value = model.SupplierID == null
                            ? 0
                            : Convert.ToInt32(model.SupplierID);
                        cmd.Parameters.Add("@ItemId", SqlDbType.Int).Value = model.ItemID == null
                            ? 0
                            : Convert.ToInt32(model.ItemID);
                        cmd.Parameters.Add("@StoreId", SqlDbType.Int).Value = model.StoreID == null
                            ? 0
                            : Convert.ToInt32(model.StoreID);
                        cmd.Parameters.Add("@ItemCategory", SqlDbType.NVarChar).Value = model.ItemCategory == null ? (object)null : model.ItemCategory;

                        cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                            ? (object)null
                            : DalCommon.SetDate(model.DateFrom);
                        cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                            ? (object)null
                            : DalCommon.SetDate(model.DateTo);
                        break;
                    case 2:
                        cmd.Parameters.Add("@ItemId", SqlDbType.Int).Value = model.ItemID == null
                            ? 0
                            : Convert.ToInt32(model.ItemID);
                        cmd.Parameters.Add("@StoreId", SqlDbType.Int).Value = model.StoreID == null
                            ? 0
                            : Convert.ToInt32(model.StoreID);
                        cmd.Parameters.Add("@ItemCategory", SqlDbType.NVarChar).Value = model.ItemCategory == null ? (object)null : model.ItemCategory;
                        break;
                    case 3:
                        cmd.Parameters.Add("@SupplierId", SqlDbType.Int).Value = model.SupplierID == null
                           ? 0
                           : Convert.ToInt32(model.SupplierID);
                        cmd.Parameters.Add("@ItemId", SqlDbType.Int).Value = model.ItemID == null
                            ? 0
                            : Convert.ToInt32(model.ItemID);
                        cmd.Parameters.Add("@StoreId", SqlDbType.Int).Value = model.StoreID == null
                            ? 0
                            : Convert.ToInt32(model.StoreID);
                        cmd.Parameters.Add("@ItemCategory", SqlDbType.NVarChar).Value = model.ItemCategory == null ? (object)null : model.ItemCategory;
                        break;
                    case 4:
                        cmd.Parameters.Add("@SupplierId", SqlDbType.Int).Value = model.SupplierID == null
                            ? 0
                            : Convert.ToInt32(model.SupplierID);
                        cmd.Parameters.Add("@ItemId", SqlDbType.Int).Value = model.ItemID == null
                            ? 0
                            : Convert.ToInt32(model.ItemID);
                        cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                            ? (object)null
                            : DalCommon.SetDate(model.DateFrom);
                        cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                            ? (object)null
                            : DalCommon.SetDate(model.DateTo);
                        break;
                    case 5:
                        cmd.Parameters.Add("@StoreId", SqlDbType.Int).Value = model.StoreID == null
                            ? 0
                            : Convert.ToInt32(model.StoreID);
                        cmd.Parameters.Add("@BuyerId", SqlDbType.Int).Value = model.BuyerID == null
                            ? 0
                            : Convert.ToInt32(model.BuyerID);

                        cmd.Parameters.Add("@ArticleChallanId", SqlDbType.Int).Value = model.ArticleChallanID == null
                            ? 0
                            : Convert.ToInt32(model.ArticleChallanID);
                        cmd.Parameters.Add("@ArticleId", SqlDbType.Int).Value = model.ArticleID == null
                            ? 0
                            : Convert.ToInt32(model.ArticleID);
                        cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                            ? (object)null
                            : DalCommon.SetDate(model.DateFrom);
                        cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                            ? (object)null
                            : DalCommon.SetDate(model.DateTo);
                        break;
                    case 6:
                        cmd.Parameters.Add("@ItemId", SqlDbType.Int).Value = model.ItemID == null
                            ? 0
                            : Convert.ToInt32(model.ItemID);
                        cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                            ? (object)null
                            : DalCommon.SetDate(model.DateFrom);
                        cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                            ? (object)null
                            : DalCommon.SetDate(model.DateTo);
                        break;
                    case 7:
                        cmd.Parameters.Add("@SupplierId", SqlDbType.Int).Value = model.SupplierID == null
                            ? 0
                            : Convert.ToInt32(model.SupplierID);
                        cmd.Parameters.Add("@ItemId", SqlDbType.Int).Value = model.ItemID == null
                            ? 0
                            : Convert.ToInt32(model.ItemID);
                        cmd.Parameters.Add("@StoreId", SqlDbType.Int).Value = model.StoreID == null
                            ? 0
                            : Convert.ToInt32(model.StoreID);

                        cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                            ? (object)null
                            : DalCommon.SetDate(model.DateFrom);
                        cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                            ? (object)null
                            : DalCommon.SetDate(model.DateTo);
                        break;


                }
                cmd.Connection = conn;
                cmd.CommandText = spName;
                cmd.CommandType = CommandType.StoredProcedure;
                using (var adp = new SqlDataAdapter(cmd))
                {
                    adp.Fill(dt);
                }
            }
        }

        public DataTable GetLcReportForChemCosting(LcReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    switch (model.ReportName)
                    {
                        case "LcReportForChemCosting":
                            using (var cmd = new SqlCommand())
                            {
                                cmd.Connection = conn;
                                cmd.CommandText = "UspLcReportForChemicalCosting";
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add("@LCID", SqlDbType.Int).Value = Convert.ToInt32(model.LcId);
                                using (var adp = new SqlDataAdapter(cmd))
                                {
                                    adp.Fill(dt);
                                }
                            }
                            break;
                        case "SupplierWiseChemicalLcCosting":
                            using (var cmd = new SqlCommand())
                            {
                                cmd.Connection = conn;
                                cmd.CommandText = "UspSupplierWiseLcChemicalCosting";
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add("@LCID", SqlDbType.Int).Value = Convert.ToInt32(model.LcId);
                                cmd.Parameters.Add("@DateType", SqlDbType.NChar).Value = "R";
                                
                                cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.FromDate == null)
                           ? (object)null
                           : DalCommon.SetDate(model.FromDate);
                                cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.ToDate == null)
                                    ? (object)null
                                    : DalCommon.SetDate(model.ToDate);
                                using (var adp = new SqlDataAdapter(cmd))
                                {
                                    adp.Fill(dt);
                                }
                            }
                            break;
                        case "SupplierWiseChemLcCostingWithLcDate":
                        case "ChemicalWiseSupplierLcCosting":
                            using (var cmd = new SqlCommand())
                            {
                                cmd.Connection = conn;
                                cmd.CommandText = "UspSupplierWiseLcChemicalCosting";
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add("@LCID", SqlDbType.Int).Value = Convert.ToInt32(model.LcId);
                                cmd.Parameters.Add("@DateType", SqlDbType.NChar).Value = "L";
                                cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.FromDate == null)
                           ? (object)null
                           : DalCommon.SetDate(model.FromDate);
                                cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.ToDate == null)
                                    ? (object)null
                                    : DalCommon.SetDate(model.ToDate);
                                using (var adp = new SqlDataAdapter(cmd))
                                {
                                    adp.Fill(dt);
                                }
                            }
                            break;
                        case "LcImportStatement":
                            using (var cmd = new SqlCommand())
                            {
                                cmd.Connection = conn;
                                cmd.CommandText = "UspLcImportStatementReport";
                                cmd.CommandType = CommandType.StoredProcedure;
                                using (var adp = new SqlDataAdapter(cmd))
                                {
                                    adp.Fill(dt);
                                }
                            }
                            break;
                        case "DeferredPaymentStatement":
                            using (var cmd = new SqlCommand())
                            {
                                cmd.Connection = conn;
                                cmd.CommandText = "UspLcDeferredPaymentStatement";
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.FromDate == null)
                            ? (object)null
                            : DalCommon.SetDate(model.FromDate);
                                cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.ToDate == null)
                                    ? (object)null
                                    : DalCommon.SetDate(model.ToDate);
                                using (var adp = new SqlDataAdapter(cmd))
                                {
                                    adp.Fill(dt);
                                }
                            }
                            break;
                        case "DeferredPaymentDueStatement":
                            using (var cmd = new SqlCommand())
                            {
                                cmd.Connection = conn;
                                cmd.CommandText = "UspLcDeferredPaymentDueStatement";
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.FromDate == null)
                            ? (object)null
                            : DalCommon.SetDate(model.FromDate);
                                cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.ToDate == null)
                                    ? (object)null
                                    : DalCommon.SetDate(model.ToDate);
                                using (var adp = new SqlDataAdapter(cmd))
                                {
                                    adp.Fill(dt);
                                }
                            }
                            break;
                        case "LcPaymentStatement":
                            using (var cmd = new SqlCommand())
                            {
                                cmd.Connection = conn;
                                cmd.CommandText = "UspLcPaymentStatement";
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.FromDate == null)
                            ? (object)null
                            : DalCommon.SetDate(model.FromDate);
                                cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.ToDate == null)
                                    ? (object)null
                                    : DalCommon.SetDate(model.ToDate);
                                using (var adp = new SqlDataAdapter(cmd))
                                {
                                    adp.Fill(dt);
                                }
                            }
                            break;
                        case "LcMonthWisePaymentStatement":
                            using (var cmd = new SqlCommand())
                            {
                                cmd.Connection = conn;
                                cmd.CommandText = "UspLcMonthWisePaymentStatement";
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.FromDate == null)
                            ? (object)null
                            : DalCommon.SetDate(model.FromDate);
                                cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.ToDate == null)
                                    ? (object)null
                                    : DalCommon.SetDate(model.ToDate);
                                using (var adp = new SqlDataAdapter(cmd))
                                {
                                    adp.Fill(dt);
                                }
                            }
                            break;
                        case "LcInsuranceBillStatement":
                            using (var cmd = new SqlCommand())
                            {
                                cmd.Connection = conn;
                                cmd.CommandText = "UspLcInsuranceBillStatement";
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.FromDate == null)
                            ? (object)null
                            : DalCommon.SetDate(model.FromDate);
                                cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.ToDate == null)
                                    ? (object)null
                                    : DalCommon.SetDate(model.ToDate);
                                using (var adp = new SqlDataAdapter(cmd))
                                {
                                    adp.Fill(dt);
                                }
                            }
                            break;
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        public DataTable FinalGradeSelectionReport(ReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "UspFinalGradeSelectionReport";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@SupplierID", SqlDbType.Int).Value = model.SupplierID == null ? 0 : Convert.ToInt32(model.SupplierID);
                        cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                            ? (object)null
                            : DalCommon.SetDate(model.DateFrom);
                        cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                            ? (object)null
                            : DalCommon.SetDate(model.DateTo);
                        cmd.Parameters.Add("@GradeRangeID", SqlDbType.Int).Value = (model.GradeRangeID == null) ? 0 : Convert.ToInt32(model.GradeRangeID);

                        using (var adp = new SqlDataAdapter(cmd))
                        {
                            adp.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        public DataTable FinalGradeSelectionReport(ReportModel model, long id = 0)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;

                        if (id != -1)
                        {
                            cmd.CommandText = "UspFinalGradeSelectionByPurchaseID";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@SupplierID", SqlDbType.Int).Value = model.SupplierID == null ? 0 : Convert.ToInt32(model.SupplierID);
                            cmd.Parameters.Add("@PurchaseID", SqlDbType.BigInt).Value = Convert.ToInt64(id);
                            cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                            ? (object)null
                            : DalCommon.SetDate(model.DateFrom);
                            cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                            ? (object)null
                            : DalCommon.SetDate(model.DateTo);
                            cmd.Parameters.Add("@GradeRangeID", SqlDbType.Int).Value = (model.GradeRangeID == null) ? 0 : Convert.ToInt32(model.GradeRangeID);
                        }
                        else
                        {
                            cmd.CommandText = "UspFinalGradeSelectionAllPurchase";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@SupplierID", SqlDbType.Int).Value = model.SupplierID == null ? 0 : Convert.ToInt32(model.SupplierID);
                            cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                             ? (object)null
                            : DalCommon.SetDate(model.DateFrom);
                            cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                            ? (object)null
                            : DalCommon.SetDate(model.DateTo);
                            cmd.Parameters.Add("@GradeRangeID", SqlDbType.Int).Value = (model.GradeRangeID == null) ? 0 : Convert.ToInt32(model.GradeRangeID);
                        }
                        using (var adp = new SqlDataAdapter(cmd))
                        {
                            adp.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }


        public DataTable FinalGradeSelectionSummaryReport(ReportModel model, long id)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;

                        cmd.CommandText = "UspFinalSelectionSummByPurchaseID";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@SupplierID", SqlDbType.Int).Value = model.SupplierID == null ? 0 : Convert.ToInt32(model.SupplierID);
                        cmd.Parameters.Add("@PurchaseID", SqlDbType.BigInt).Value = (id == null ? 0 : Convert.ToInt64(id));
                        cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                        ? (object)null
                        : DalCommon.SetDate(model.DateFrom);
                        cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                        ? (object)null
                        : DalCommon.SetDate(model.DateTo);
                        cmd.Parameters.Add("@GradeRangeID", SqlDbType.Int).Value = (model.GradeRangeID == null) ? 0 : Convert.ToInt32(model.GradeRangeID);


                        using (var adp = new SqlDataAdapter(cmd))
                        {
                            adp.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        public DataTable FinalGradeSelectionSummaryReport(ReportModel model, string id)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;

                        cmd.CommandText = "UspFinalGradeSelectionByDateRange";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                        ? (object)null
                        : DalCommon.SetDate(model.DateFrom);
                        cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                        ? (object)null
                        : DalCommon.SetDate(model.DateTo);
                        cmd.Parameters.Add("@GradeRangeID", SqlDbType.Int).Value = (model.GradeRangeID == null) ? 0 : Convert.ToInt32(model.GradeRangeID);


                        using (var adp = new SqlDataAdapter(cmd))
                        {
                            adp.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable GetFinishedProductionInfo(CrustReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    switch (model.ReportName)
                    {
                        case "ChallanWiseFndProdSummary":
                            GetStoreProcedureValue(model, conn, dt, "UspGetChallanWiseFndProduction");
                            break;
                        case "ArticleWiseFndProdSummary":
                            GetStoreProcedureValue(model, conn, dt, "UspGetArticleWiseFndProduction");
                            break;
                        case "MonthWiseFndProdSummary":
                            GetStoreProcedureValue(model, conn, dt, "UspGetMonthWiseFndProd");
                            break;
                        case "SchWiseFndProdSummary":
                            GetStoreProcedureValue(model, conn, dt, "UspGetSchWiseFndProdInfo");
                            break;
                        case "ScheduleDateWiseFndProdStatus":
                            GetStoreProcedureValue(model, conn, dt, "UspGetScheduleDateWiseFndProdStatus");
                            break;

                        case "ProdStatusWiseFndProdSummary":
                            GetStoreProcedureValue(model, conn, dt, "UspGetProdStatusWiseFndProdInfo");
                            break;
                        case "DateWiseFndStoreTransfer":
                            GetStoreProcedureValue(model, conn, dt, "UspGetDateWiseFndStoreTransfer");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        public DataTable GetCrustProductionInfo(CrustReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    switch (model.ReportName)
                    {

                        case "ChallanWiseCrustProdSummary":
                            GetStoreProcedureValue(model, conn, dt, "UspGetChallanWiseCrustProduction");
                            break;
                        case "ChallanWiseArticleCrustProdSummary":
                            GetStoreProcedureValue(model, conn, dt, "UspGetChallanWiseArticleCrustProduction");
                            break;
                        case "ArticleWiseCrustProdSummary":
                            GetStoreProcedureValue(model, conn, dt, "UspGetArticleWiseCrustProduction");
                            break;
                        case "ArticleWiseChallanCrustProdSummary":
                            GetStoreProcedureValue(model, conn, dt, "UspGetChallanWiseArticleCrustProduction");
                            break;
                        case "ScheduleDateWiseCrustProdStatus":
                            GetStoreProcedureValue(model, conn, dt, "UspGetScheduleDateWiseCrustProd");
                            break;
                        case "MonthWiseCrustProdSummary":
                            GetStoreProcedureValue(model, conn, dt, "UspGetMonthWiseCrustProd");
                            break;
                        case "SchWiseCrustProdSummary":
                            GetStoreProcedureValue(model, conn, dt, "UspGetScheduleWiseCrustProd");
                            break;
                        case "CrustProdStatusWise":
                            break;

                        case "DateWiseCrustStoreTransfer":
                            GetStoreProcedureValue(model, conn, dt, "UspGetDateWiseCrustStoreTransfer");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        private static void GetStoreProcedureValue(CrustReportModel model, SqlConnection conn, DataTable dt, string spName)
        {
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = spName;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@StoreId", SqlDbType.Int).Value = model.StoreID == null
                    ? 0
                    : Convert.ToInt32(model.StoreID);
                //cmd.Parameters.Add("@ItemTypeID", SqlDbType.TinyInt).Value = model.ItemTypeID == null
                //    ? 0
                //    : Convert.ToByte(model.ItemTypeID);
                cmd.Parameters.Add("@ScheduleYear", SqlDbType.NChar).Value = model.ScheduleYear ?? "";

                cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.FromDate == null)
                    ? (object)null
                    : DalCommon.SetDate(model.FromDate);
                cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.ToDate == null)
                    ? (object)null
                    : DalCommon.SetDate(model.ToDate);
                cmd.Parameters.Add("@ProductionStatus", SqlDbType.NVarChar).Value =
                                    model.ProductionStatus == "0" ? "" : model.ProductionStatus;
                using (var adp = new SqlDataAdapter(cmd))
                {
                    adp.Fill(dt);
                }
            }
        }
        public DataTable GetWetBlueProductionInfo(PurchaseReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    switch (model.ReportName)
                    {
                        case "ProcessWiseWBProdSummary":
                        case "ProcessAndDrumWiseWBProdSummary":
                            GetStoreProcedureValue(model, conn, dt, "UspGetWetBlueProductionInfo");
                            break;
                        case "SupplierWiseWBProdStatus":
                            GetStoreProcedureValue(model, conn, dt, "UspGetSupplierWiseWBProdStatus");
                            break;
                        case "ProdStatusWiseWBProdSummary":
                            GetStoreProcedureValue(model, conn, dt, "UspGetProductionStatusWiseWetBlueInfo");
                            break;
                        case "ScheduleDateWiseProdStatus":
                            GetStoreProcedureValue(model, conn, dt, "UspGetDateWiseWBProdStatus");
                            break;
                        case "DateWiseWBStoreTransfer":
                            GetStoreProcedureValue(model, conn, dt, "UspGetDateWiseWBStoreTransferInfo");
                            break;
                        case "MonthWiseWBProdStatus":
                            GetStoreProcedureValue(model, conn, dt, "UspGetMonthWiseWBProdStatus");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        private static void GetStoreProcedureValue(PurchaseReportModel model, SqlConnection conn, DataTable dt, string spName)
        {
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = spName;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@SupplierId", SqlDbType.Int).Value = model.SupplierID == null
                    ? 0
                    : Convert.ToInt32(model.SupplierID);
                cmd.Parameters.Add("@StoreId", SqlDbType.Int).Value = model.StoreID == null
                    ? 0
                    : Convert.ToInt32(model.StoreID);
                cmd.Parameters.Add("@ItemTypeID", SqlDbType.TinyInt).Value = model.ItemTypeID == null
                    ? 0
                    : Convert.ToByte(model.ItemTypeID);
                cmd.Parameters.Add("@PurchaseYear", SqlDbType.NChar).Value = model.PurchaseYear ?? "";

                cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                    ? (object)null
                    : DalCommon.SetDate(model.DateFrom);
                cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                    ? (object)null
                    : DalCommon.SetDate(model.DateTo);
                cmd.Parameters.Add("@PurchaseType", SqlDbType.NVarChar).Value =
                                    model.PurchaseType == "0" ? "" : model.PurchaseType;
                using (var adp = new SqlDataAdapter(cmd))
                {
                    adp.Fill(dt);
                }
            }
        }
        public DataTable GetFinishedChemConsumptionInfo(ReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    switch (model.ReportName)
                    {
                        case "SchWiseFndChemConsumptionSummary":
                            GetStoreProcedureValue(model, conn, dt, "UspGetSchWiseFndChemConsumption", 7);
                            break;
                        case "ChemicalWiseFndConsumptionSummary":
                            GetStoreProcedureValue(model, conn, dt, "UspGetChemicalWiseFndConsumption", 7);
                            break;
                        case "ChemicalWiseFndConsumptionSummaryWithoutSch":
                            GetStoreProcedureValue(model, conn, dt, "UspGetFinishChemicalWiseConsumeWithoutSch", 7);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        public DataTable GetCrustChemConsumptionInfo(ReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    switch (model.ReportName)
                    {
                        case "CrustSchWiseChemConsumptionSummary":
                            GetStoreProcedureValue(model, conn, dt, "UspGetCrustSchWiseChemConsumptionInfo", 7);
                            break;
                        case "ChemicalWiseCrustConsumptionSummary":
                            GetStoreProcedureValue(model, conn, dt, "UspGetCrustChemicalWiseConsumption", 7);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        public DataTable GetChemFrnPurcReceiveInfo(ReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    switch (model.ReportName)
                    {
                        case "ChemicalWiseFrnPurcRcvSummary":
                            GetStoreProcedureValue(model, conn, dt, "UspGetChemicalWiseForeignPurcReceive", 1);
                            break;
                        case "ChemicalWiseSuppFrnPurcRcvSummary":
                            GetStoreProcedureValue(model, conn, dt, "UspGetChemWiseSupplierForeignPurcReceive", 1);
                            break;
                        case "ChemicalWiseFrnPurcRcvDetail":
                            GetStoreProcedureValue(model, conn, dt, "UspGetChemWiseForeignPurcReceiveDetail", 1);
                            break;
                        case "SupplierWiseChemFrnPurcRcvSummary":
                            GetStoreProcedureValue(model, conn, dt, "UspGetSupplierWiseChemForeignPurcReceive", 1);
                            break;
                        case "SupplierWiseFrnPurcRcvDetail":
                            GetStoreProcedureValue(model, conn, dt, "UspGetChemicalForeignPurcReceive", 1);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        public DataTable GetChemLocPurcReceiveInfo(ReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    switch (model.ReportName)
                    {
                        case "ChemicalWiseLocPurcRcvSummary":
                            GetStoreProcedureValue(model, conn, dt, "UspGetChemicalWiseLocalPurcReceive", 1);
                            break;
                        case "ChemicalWiseSuppLocPurcRcvSummary":
                            GetStoreProcedureValue(model, conn, dt, "UspGetChemWiseSupplierLocalPurcReceive", 1);
                            break;
                        case "ChemicalWiseLocPurcRcvDetail":
                            GetStoreProcedureValue(model, conn, dt, "UspGetChemWiseLocalPurcReceiveDetail", 1);
                            break;
                        case "SupplierWiseChemLocPurcRcvSummary":
                            GetStoreProcedureValue(model, conn, dt, "UspGetSupplierWiseChemLocalPurcReceive", 1);
                            break;
                        case "SupplierWiseLocPurcRcvDetail":
                            GetStoreProcedureValue(model, conn, dt, "UspGetChemicalLocalPurcReceive", 1);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        public DataTable GetWetBlueChemConsumptionInfo(ReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    switch (model.ReportName)
                    {
                        case "WbSchWiseChemConsumptionSummary":
                            GetStoreProcedureValue(model, conn, dt, "UspGetWBProdChemConsumptionInfo", 7);
                            break;
                        case "ChemicalWiseChemConsumptionSummary":
                            GetStoreProcedureValue(model, conn, dt, "UspGetWBProdChemConsumption", 7);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable FinalGradeSelectionByItemType(ReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "UspFinalGradeSelectionByItemType";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ItemTypeID", SqlDbType.Int).Value = model.ItemTypeID == null ? 0 : Convert.ToInt32(model.ItemTypeID);

                        cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                         ? (object)null
                         : DalCommon.SetDate(model.DateFrom);
                        cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                        ? (object)null
                        : DalCommon.SetDate(model.DateTo);
                        cmd.Parameters.Add("@GradeRange", SqlDbType.Int).Value = (model.GradeRangeID == null) ? 0 : Convert.ToInt32(model.GradeRangeID);

                        using (var adp = new SqlDataAdapter(cmd))
                        {
                            adp.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        public DataTable FinalGradeSelectionByGrade(ReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "UspFinalGradeSelectionByGrade";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@GradeID", SqlDbType.Int).Value = model.GradeID == null ? 0 : Convert.ToInt32(model.GradeID);
                        cmd.Parameters.Add("@GradeRangeID", SqlDbType.Int).Value = (model.GradeRangeID == null) ? 0 : Convert.ToInt32(model.GradeRangeID);
                        cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                          ? (object)null
                          : DalCommon.SetDate(model.DateFrom);
                        cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                        ? (object)null
                        : DalCommon.SetDate(model.DateTo);


                        using (var adp = new SqlDataAdapter(cmd))
                        {
                            adp.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable ChemicalStockIssueRec(ReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "UspChemicalStoreIssueReceive";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@SupplierId", SqlDbType.Int).Value = model.SupplierID == null ? 0 : Convert.ToInt32(model.SupplierID);
                        cmd.Parameters.Add("@StoreId", SqlDbType.Int).Value = (model.StoreID == null) ? 0 : Convert.ToInt32(model.StoreID);
                        cmd.Parameters.Add("@ItemId", SqlDbType.Int).Value = (model.ItemID == null) ? 0 : Convert.ToInt32(model.ItemID);
                        cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                          ? (object)null
                          : DalCommon.SetDate(model.DateFrom);
                        cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                        ? (object)null
                        : DalCommon.SetDate(model.DateTo);
                        cmd.Parameters.Add("@FilterCategory", SqlDbType.VarChar).Value = model.ReportName;
                        using (var adp = new SqlDataAdapter(cmd))
                        {
                            adp.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable WBStockIssueAfterSelection(ReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "UspGetWBIssueToStoreAfterSelection";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@StoreID", SqlDbType.Int).Value = model.GradeID == null ? 0 : Convert.ToInt32(model.GradeID);
                        cmd.Parameters.Add("@SupplierID", SqlDbType.Int).Value = (model.GradeRangeID == null) ? 0 : Convert.ToInt32(model.GradeRangeID);
                        cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                          ? (object)null
                          : DalCommon.SetDate(model.DateFrom);
                        cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                        ? (object)null
                        : DalCommon.SetDate(model.DateTo);
                        cmd.Parameters.Add("@ReportType", SqlDbType.Text).Value = model.ReportType == null ? "DailyStockStoreWise" : Convert.ToString(model.ReportType);
                        cmd.Parameters.Add("@FilterExpression", SqlDbType.Text).Value = (model.FilterExpression == null) ? " g.GradeID = 6 " : Convert.ToString(model.FilterExpression);
                        using (var adp = new SqlDataAdapter(cmd))
                        {
                            adp.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable WBStockIssueGradeReport(ReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "UspGetWBIssueToStoreAfterSelection";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@StoreID", SqlDbType.Int).Value = model.StoreID == null ? 0 : Convert.ToInt32(model.StoreID);
                        cmd.Parameters.Add("@SupplierID", SqlDbType.Int).Value = (model.SupplierID == null) ? 0 : Convert.ToInt32(model.SupplierID);
                        cmd.Parameters.Add("@ReportType", SqlDbType.Text).Value = (model.ReportName == null) ? "" : Convert.ToString(model.ReportName);
                        cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                          ? (object)null
                          : DalCommon.SetDate(model.DateFrom);
                        cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                        ? (object)null
                        : DalCommon.SetDate(model.DateTo);
                        cmd.Parameters.Add("@FilterExpression", SqlDbType.Text).Value = model.FilterExpression;
                        using (var adp = new SqlDataAdapter(cmd))
                        {
                            adp.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable WBStockIssueStatusReport(ReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "UspGetWBIssueToStoreAfterSelection";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@StoreID", SqlDbType.Int).Value = model.StoreID == null ? 0 : Convert.ToInt32(model.StoreID);
                        cmd.Parameters.Add("@SupplierID", SqlDbType.Int).Value = (model.SupplierID == null) ? 0 : Convert.ToInt32(model.SupplierID);
                        cmd.Parameters.Add("@ReportType", SqlDbType.Text).Value = (model.ReportName == null) ? "" : Convert.ToString(model.ReportName);
                        cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                          ? (object)null
                          : DalCommon.SetDate(model.DateFrom);
                        cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                        ? (object)null
                        : DalCommon.SetDate(model.DateTo);
                        cmd.Parameters.Add("@FilterExpression", SqlDbType.Text).Value = model.FilterExpression;
                        using (var adp = new SqlDataAdapter(cmd))
                        {
                            adp.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable WBStockIssueSupplierReport(ReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "UspGetWBIssueToStoreAfterSelection";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@StoreID", SqlDbType.Int).Value = model.StoreID == null ? 0 : Convert.ToInt32(model.StoreID);
                        cmd.Parameters.Add("@SupplierID", SqlDbType.Int).Value = (model.SupplierID == null) ? 0 : Convert.ToInt32(model.SupplierID);
                        cmd.Parameters.Add("@ReportType", SqlDbType.Text).Value = (model.ReportName == null) ? "" : Convert.ToString(model.ReportName);
                        cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                          ? (object)null
                          : DalCommon.SetDate(model.DateFrom);
                        cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                        ? (object)null
                        : DalCommon.SetDate(model.DateTo);
                        cmd.Parameters.Add("@FilterExpression", SqlDbType.Text).Value = model.FilterExpression;
                        using (var adp = new SqlDataAdapter(cmd))
                        {
                            adp.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable UspGetWBRequisition(ReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "UspGetWBRequisition";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@BuyerID", SqlDbType.Int).Value = model.BuyerID == null ? 0 : Convert.ToInt32(model.BuyerID);

                        cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                          ? (object)null
                          : DalCommon.SetDate(model.DateFrom);
                        cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                        ? (object)null
                        : DalCommon.SetDate(model.DateTo);
                        cmd.Parameters.Add("@ArticalID", SqlDbType.Int).Value = model.ArticalID == null ? 0 : Convert.ToInt32(model.ArticalID);
                        cmd.Parameters.Add("@ChallanID", SqlDbType.Int).Value = model.ArticleChallanID == null ? 0 : Convert.ToInt32(model.ArticleChallanID);
                        using (var adp = new SqlDataAdapter(cmd))
                        {
                            adp.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable LocalChemPurcBillInfo(ReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    switch (model.ReportName)
                    {
                        case "SupplierWiseChemLocalBill":
                            GetStoreProcedureValue(model, conn, dt, "UspGetLocalChemBillInfo", 4);
                            break;
                        case "ChemicalWiseCrustConsumptionSummary":
                            GetStoreProcedureValue(model, conn, dt, "UspGetCrustChemicalWiseConsumption", 4);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable GetChemPurcRequisitionInfo(ReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    switch (model.ReportName)
                    {
                        case "ChemicalRequisitionWiseItems":
                            GetStoreProcedureValue(model, conn, dt, "UspGetChemRequisitionWiseItemsDetail", 1);
                            break;
                        case "StoreWiseChemicalRequisition":
                            GetStoreProcedureValue(model, conn, dt, "UspGetStoreWiseChemRequisitionSummary", 1);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        public DataTable LocalChemPurcOrderInfo(ReportModel model, string orderCategory)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Parameters.Add("@SupplierId", SqlDbType.Int).Value = model.SupplierID == null
                                       ? 0
                                       : Convert.ToInt32(model.SupplierID);
                        cmd.Parameters.Add("@ItemId", SqlDbType.Int).Value = model.ItemID == null
                            ? 0
                            : Convert.ToInt32(model.ItemID);
                        cmd.Parameters.Add("@OrderCategory", SqlDbType.NVarChar).Value = orderCategory;
                        cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                            ? (object)null
                            : DalCommon.SetDate(model.DateFrom);
                        cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                            ? (object)null
                            : DalCommon.SetDate(model.DateTo);

                        switch (model.ReportName)
                        {
                            case "ChemPurchaseOrderWiseDetail":
                                cmd.CommandText = "UspGetChemPurchaseOrderDetail";
                                break;
                            case "ChemPurchaseChemicalWiseSummary":
                                cmd.CommandText = "UspGetChemPurchaseWithChemicalWise";
                                break;
                        }
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var adp = new SqlDataAdapter(cmd))
                        {
                            adp.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable GetChemLoanReceiveRequestInfo(ReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    switch (model.ReportName)
                    {
                        case "ChemLoanReceiveRequestDetail":
                            GetStoreProcedureValue(model, conn, dt, "UspGetChemLoanReceiveRequest", 7);
                            break;
                        case "StoreWiseChemLoanReceiveRequestSummary":
                            GetStoreProcedureValue(model, conn, dt, "UspGetStoreWiseChemLoanReceiveRequest", 7);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable UspGetChemicalPI(long id, ReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Parameters.Add("@PIID", SqlDbType.BigInt).Value = id == null
                                       ? 0
                                       : Convert.ToInt64(id);

                        cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                            ? (object)null
                            : DalCommon.SetDate(model.DateFrom);
                        cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                            ? (object)null
                            : DalCommon.SetDate(model.DateTo);
                        cmd.CommandText = "UspGetChemicalPIReport";
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var adp = new SqlDataAdapter(cmd))
                        {
                            adp.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }


        public DataTable UspCommercialInvoice(long id, ReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Parameters.Add("@CIID", SqlDbType.Int).Value = id == null
                                       ? 0
                                       : Convert.ToInt32(id);

                        //cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                        //    ? (object)null
                        //    : DalCommon.SetDate(model.DateFrom);
                        //cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                        //    ? (object)null
                        //    : DalCommon.SetDate(model.DateTo);
                        cmd.CommandText = "UspCommercialInvoiceReport";
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var adp = new SqlDataAdapter(cmd))
                        {
                            adp.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        public DataTable UspPackingList(long id, ReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Parameters.Add("@PLID", SqlDbType.Int).Value = id == null
                                       ? 0
                                       : Convert.ToInt32(id);

                        //cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                        //    ? (object)null
                        //    : DalCommon.SetDate(model.DateFrom);
                        //cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                        //    ? (object)null
                        //    : DalCommon.SetDate(model.DateTo);
                        cmd.CommandText = "UspPackingListReport";
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var adp = new SqlDataAdapter(cmd))
                        {
                            adp.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable GetCrustLeatherRequisitionInfo(ReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    switch (model.ReportName)
                    {
                        case "CrustLeatherRequisitionDetail":
                            GetStoreProcedureValue(model, conn, dt, "UspGetCrustLeatherRequisitionDetail", 5);
                            break;
                        case "BuyerWiseColorCrustLeatherRequisition":
                            GetStoreProcedureValue(model, conn, dt, "UspGetBuyerWiseColorCrustRequisition", 5);
                            break;
                        case "ArticleWiseColorCrustLeatherRequisition":
                            GetStoreProcedureValue(model, conn, dt, "UspGetArticleWiseColorCrustRequisition", 5);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        public DataTable UspBillOfLadingReport(long id, ReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Parameters.Add("@BLID", SqlDbType.Int).Value = id == null
                                       ? 0
                                       : Convert.ToInt32(id);

                        //cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                        //    ? (object)null
                        //    : DalCommon.SetDate(model.DateFrom);
                        //cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                        //    ? (object)null
                        //    : DalCommon.SetDate(model.DateTo);
                        cmd.CommandText = "UspBillOfLadingReport";
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var adp = new SqlDataAdapter(cmd))
                        {
                            adp.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        public DataTable UspCNFBillReport(long id, ReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Parameters.Add("@CnfBillID", SqlDbType.Int).Value = id == null
                                       ? 0
                                       : Convert.ToInt32(id);

                        //cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                        //    ? (object)null
                        //    : DalCommon.SetDate(model.DateFrom);
                        //cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                        //    ? (object)null
                        //    : DalCommon.SetDate(model.DateTo);
                        cmd.CommandText = "UspCNFBillReport";
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var adp = new SqlDataAdapter(cmd))
                        {
                            adp.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        public DataTable UspLimInfoReport(long id, ReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Parameters.Add("@LimID", SqlDbType.Int).Value = id == null
                                       ? 0
                                       : Convert.ToInt32(id);

                        //cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                        //    ? (object)null
                        //    : DalCommon.SetDate(model.DateFrom);
                        //cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                        //    ? (object)null
                        //    : DalCommon.SetDate(model.DateTo);
                        cmd.CommandText = "UspLimInfoReport";
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var adp = new SqlDataAdapter(cmd))
                        {
                            adp.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        public DataTable UspLCRetirementReport(long id, ReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Parameters.Add("@LCRetirementID", SqlDbType.Int).Value = id == null
                                       ? 0
                                       : Convert.ToInt32(id);

                        //cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                        //    ? (object)null
                        //    : DalCommon.SetDate(model.DateFrom);
                        //cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                        //    ? (object)null
                        //    : DalCommon.SetDate(model.DateTo);
                        cmd.CommandText = "UspLCRetirementReport";
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var adp = new SqlDataAdapter(cmd))
                        {
                            adp.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }



        //************************ Export Report Regardibg Method ***************************


        public DataTable GetExportCnFBillReport(long _id, ReportModel model)
        {
            var _dataTable = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    using (var _sqlCmd = new SqlCommand())
                    {
                        _sqlCmd.Parameters.Add("@CnfBillID", SqlDbType.Int).Value = _id == null ? 0 : Convert.ToInt32(_id);

                        _sqlCmd.CommandText = "UspEXPCNFBillReport";
                        _sqlCmd.Connection = conn;
                        _sqlCmd.CommandType = CommandType.StoredProcedure;
                        using (var _sqlAdp = new SqlDataAdapter(_sqlCmd))
                        {
                            _sqlAdp.Fill(_dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dataTable;
        }

        public DataTable GetExportLCretirementReport(long _id, ReportModel model)
        {
            var _dataTable = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    using (var _sqlCmd = new SqlCommand())
                    {
                        _sqlCmd.Parameters.Add("@LCRetirementID", SqlDbType.Int).Value = _id == null ? 0 : Convert.ToInt32(_id);

                        _sqlCmd.CommandText = "UspEXPLCRetirementReport";
                        _sqlCmd.Connection = conn;
                        _sqlCmd.CommandType = CommandType.StoredProcedure;
                        using (var _sqlAdp = new SqlDataAdapter(_sqlCmd))
                        {
                            _sqlAdp.Fill(_dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dataTable;
        }

        public DataTable GetExportDeliveryChallanReport(long _id, ReportModel model)
        {
            var _dataTable = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    using (var _sqlCmd = new SqlCommand())
                    {
                        _sqlCmd.Parameters.Add("@DeliverChallanID", SqlDbType.Int).Value = _id == null ? 0 : Convert.ToInt32(_id);

                        _sqlCmd.CommandText = "UspEXPDeliveryChallanReport";
                        _sqlCmd.Connection = conn;
                        _sqlCmd.CommandType = CommandType.StoredProcedure;
                        using (var _sqlAdp = new SqlDataAdapter(_sqlCmd))
                        {
                            _sqlAdp.Fill(_dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dataTable;
        }

        //************************ END Of Export Report Regardibg Method ********************


        public DataTable GetExpPiRptInfo(ExpPiOrderReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    switch (model.ReportName)
                    {
                        case "ExpPISingleDetail":
                            GetStoreProcedureValue(model, conn, dt, "UspGetExportPIForBuyerOrder", 1);
                            break;
                        case "ExpAllPIColorLeverWiseDetail":
                            GetStoreProcedureValue(model, conn, dt, "UspGetExportAllPI", 1);
                            break;
                        case "ExpAllPIArticleLevelWiseDetail":
                            GetStoreProcedureValue(model, conn, dt, "UspGetExportAllPI", 1);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        private static void GetStoreProcedureValue(ExpPiOrderReportModel model, SqlConnection conn, DataTable dt, string spName, int type)
        {
            using (var cmd = new SqlCommand())
            {
                switch (type)
                {

                    case 1:
                        cmd.Parameters.Add("@PIId", SqlDbType.Int).Value = model.PIID == null
                            ? 0
                            : Convert.ToInt32(model.PIID);
                        cmd.Parameters.Add("@PriceLevel", SqlDbType.NVarChar).Value = model.PriceLevel;

                        cmd.Parameters.Add("@BuyerId", SqlDbType.Int).Value = model.BuyerID == null
                            ? 0
                            : Convert.ToInt32(model.BuyerID);

                        cmd.Parameters.Add("@BuyerOrderId", SqlDbType.Int).Value = model.BuyerOrderID == null
                            ? 0
                            : Convert.ToInt32(model.BuyerOrderID);
                        cmd.Parameters.Add("@ArticleId", SqlDbType.Int).Value = model.ArticleID == null
                            ? 0
                            : Convert.ToInt32(model.ArticleID);
                        cmd.Parameters.Add("@ItemTypeID", SqlDbType.Int).Value = model.ItemTypeID == null
                            ? 0
                            : Convert.ToInt32(model.ItemTypeID);
                        cmd.Parameters.Add("@LeatherStatusID", SqlDbType.Int).Value = model.LeatherStatusID == null
                            ? 0
                            : Convert.ToInt32(model.LeatherStatusID);
                        cmd.Parameters.Add("@ColorId", SqlDbType.Int).Value = model.ColorID == null
                            ? 0
                            : Convert.ToInt32(model.ColorID);
                        cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.FromDate == null)
                            ? (object)null
                            : DalCommon.SetDate(model.FromDate);
                        cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.ToDate == null)
                            ? (object)null
                            : DalCommon.SetDate(model.ToDate);
                        break;
                    case 2:
                        cmd.Parameters.Add("@PriceLevel", SqlDbType.NVarChar).Value = model.PriceLevel;
                        cmd.Parameters.Add("@BuyerId", SqlDbType.Int).Value = model.BuyerID == null
                            ? 0
                            : Convert.ToInt32(model.BuyerID);

                        cmd.Parameters.Add("@BuyerOrderId", SqlDbType.Int).Value = model.BuyerOrderID == null
                            ? 0
                            : Convert.ToInt32(model.BuyerOrderID);
                        cmd.Parameters.Add("@ArticleId", SqlDbType.Int).Value = model.ArticleID == null
                            ? 0
                            : Convert.ToInt32(model.ArticleID);
                        cmd.Parameters.Add("@ItemTypeID", SqlDbType.Int).Value = model.ItemTypeID == null
                           ? 0
                           : Convert.ToInt32(model.ItemTypeID);
                        cmd.Parameters.Add("@LeatherStatusID", SqlDbType.Int).Value = model.LeatherStatusID == null
                            ? 0
                            : Convert.ToInt32(model.LeatherStatusID);
                        cmd.Parameters.Add("@ColorId", SqlDbType.Int).Value = model.ColorID == null
                            ? 0
                            : Convert.ToInt32(model.ColorID);
                        cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.FromDate == null)
                            ? (object)null
                            : DalCommon.SetDate(model.FromDate);
                        cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.ToDate == null)
                            ? (object)null
                            : DalCommon.SetDate(model.ToDate);
                        break;
                    case 3:
                        cmd.Parameters.Add("@BuyerId", SqlDbType.Int).Value = model.BuyerID == null
                            ? 0
                            : Convert.ToInt32(model.BuyerID);

                        cmd.Parameters.Add("@BuyerOrderId", SqlDbType.Int).Value = model.BuyerOrderID == null
                            ? 0
                            : Convert.ToInt32(model.BuyerOrderID);
                        cmd.Parameters.Add("@ArticleId", SqlDbType.Int).Value = model.ArticleID == null
                            ? 0
                            : Convert.ToInt32(model.ArticleID);
                        cmd.Parameters.Add("@ArticleChallanId", SqlDbType.Int).Value = model.ArticleChallanId == null
                           ? 0
                           : Convert.ToInt32(model.ArticleChallanId);
                        cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.FromDate == null)
                            ? (object)null
                            : DalCommon.SetDate(model.FromDate);
                        cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.ToDate == null)
                            ? (object)null
                            : DalCommon.SetDate(model.ToDate);
                        break;
                    case 4:
                        cmd.Parameters.Add("@BuyerId", SqlDbType.Int).Value = model.BuyerID == null
                            ? 0
                            : Convert.ToInt32(model.BuyerID);
                        cmd.Parameters.Add("@ArticleChallanId", SqlDbType.Int).Value = model.ArticleChallanId == null
                          ? 0
                          : Convert.ToInt32(model.ArticleChallanId);

                        cmd.Parameters.Add("@ArticleId", SqlDbType.Int).Value = model.ArticleID == null
                            ? 0
                            : Convert.ToInt32(model.ArticleID);
                        cmd.Parameters.Add("@ColorId", SqlDbType.Int).Value = model.ColorID == null
                             ? 0
                             : Convert.ToInt32(model.ColorID);
                        cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.FromDate == null)
                            ? (object)null
                            : DalCommon.SetDate(model.FromDate);
                        cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.ToDate == null)
                            ? (object)null
                            : DalCommon.SetDate(model.ToDate);
                        break;
                }
                cmd.Connection = conn;
                cmd.CommandText = spName;
                cmd.CommandType = CommandType.StoredProcedure;
                using (var adp = new SqlDataAdapter(cmd))
                {
                    adp.Fill(dt);
                }
            }
        }

        public DataTable GetExpBuyerOrderRptInfo(ExpPiOrderReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    switch (model.ReportName)
                    {
                        case "ExpBuyerOrderSingleDetail":
                            GetStoreProcedureValue(model, conn, dt, "UspGetBuyerSingleOrderInfo", 2);
                            break;
                        case "ExpAllBuyerOrderColorLeverWiseDetail":
                            GetStoreProcedureValue(model, conn, dt, "UspGetBuyerAllOrderInfo", 2);
                            break;
                        case "ExpAllBuyerOrderArticleLevelWiseDetail":
                            GetStoreProcedureValue(model, conn, dt, "UspGetBuyerAllOrderInfo", 2);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable UspExpPackingListReport(long _id, ReportModel model)
        {
            var _dataTable = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    using (var _sqlCmd = new SqlCommand())
                    {
                        _sqlCmd.Parameters.Add("@PLID", SqlDbType.Int).Value = _id == null ? 0 : Convert.ToInt32(_id);

                        _sqlCmd.CommandText = "UspExpPackingListReport";
                        _sqlCmd.Connection = conn;
                        _sqlCmd.CommandType = CommandType.StoredProcedure;
                        using (var _sqlAdp = new SqlDataAdapter(_sqlCmd))
                        {
                            _sqlAdp.Fill(_dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dataTable;
        }



        public DataTable UspEXPCIReport(long _id, ReportModel model)
        {
            var _dataTable = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    using (var _sqlCmd = new SqlCommand())
                    {
                        _sqlCmd.Parameters.Add("@CIID", SqlDbType.Int).Value = _id == null ? 0 : Convert.ToInt32(_id);

                        _sqlCmd.CommandText = "UspEXPCIReport";
                        _sqlCmd.Connection = conn;
                        _sqlCmd.CommandType = CommandType.StoredProcedure;
                        using (var _sqlAdp = new SqlDataAdapter(_sqlCmd))
                        {
                            _sqlAdp.Fill(_dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dataTable;
        }




        public DataTable UspGetAllCrustLeatherIssueReport(ReportModel model)
        {
            var _dataTable = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    using (var _sqlCmd = new SqlCommand())
                    {
                        _sqlCmd.Parameters.Add("@ArticleChallanID", SqlDbType.Int).Value = (model.ArticleChallanID == null)
                         ? (object)null
                         : model.ArticleChallanID;

                        _sqlCmd.Parameters.Add("@ArticleID", SqlDbType.Int).Value = (model.ArticleID == null)
                         ? (object)null
                         : Convert.ToInt32(model.ArticleID);

                        _sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                           ? (object)null
                           : DalCommon.SetDate(model.DateFrom);
                        _sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                            ? (object)null
                            : DalCommon.SetDate(model.DateTo);
                        _sqlCmd.CommandText = "UspGetAllCrustLeatherIssueReport";


                        _sqlCmd.Connection = conn;
                        _sqlCmd.CommandType = CommandType.StoredProcedure;
                        using (var _sqlAdp = new SqlDataAdapter(_sqlCmd))
                        {
                            _sqlAdp.Fill(_dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dataTable;
        }


        public DataTable GetCrustLeatherQcRptInfo(ExpPiOrderReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    switch (model.ReportName)
                    {
                        case "CrustLeatherQcDetail":
                            GetStoreProcedureValue(model, conn, dt, "UspGetCrustLeatherQCDetail", 3);
                            break;
                        case "ExpAllBuyerOrderColorLeverWiseDetail":
                            GetStoreProcedureValue(model, conn, dt, "UspGetBuyerAllOrderInfo", 2);
                            break;
                        case "ExpAllBuyerOrderArticleLevelWiseDetail":
                            GetStoreProcedureValue(model, conn, dt, "UspGetBuyerAllOrderInfo", 2);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        public DataTable GetCrustIssueAfterQcRptInfo(ExpPiOrderReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    switch (model.ReportName)
                    {
                        case "IssueAfterCrustLeatherQcDetail":
                            GetStoreProcedureValue(model, conn, dt, "UspGetIssueAfterCrustLthrQCDetail", 3);
                            break;
                        case "ExpAllBuyerOrderColorLeverWiseDetail":
                            GetStoreProcedureValue(model, conn, dt, "UspGetBuyerAllOrderInfo", 2);
                            break;
                        case "ExpAllBuyerOrderArticleLevelWiseDetail":
                            GetStoreProcedureValue(model, conn, dt, "UspGetBuyerAllOrderInfo", 2);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        public DataTable GetFinishedLeatherQcRptInfo(ExpPiOrderReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    switch (model.ReportName)
                    {
                        case "FinishLeatherQcDetail":
                            GetStoreProcedureValue(model, conn, dt, "UspGetFinishedLeatherQCDetail", 3);
                            break;
                        case "ExpAllBuyerOrderColorLeverWiseDetail":
                            GetStoreProcedureValue(model, conn, dt, "UspGetBuyerAllOrderInfo", 2);
                            break;
                        case "ExpAllBuyerOrderArticleLevelWiseDetail":
                            GetStoreProcedureValue(model, conn, dt, "UspGetBuyerAllOrderInfo", 2);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        public DataTable GetFinishedIssueAfterQcRptInfo(ExpPiOrderReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    switch (model.ReportName)
                    {
                        case "IssueAfterFinishLeatherQcDetail":
                            GetStoreProcedureValue(model, conn, dt, "UspGetIssueAfterFinishedLthrQCDetail", 3);
                            break;
                        case "ExpAllBuyerOrderColorLeverWiseDetail":
                            GetStoreProcedureValue(model, conn, dt, "UspGetBuyerAllOrderInfo", 2);
                            break;
                        case "ExpAllBuyerOrderArticleLevelWiseDetail":
                            GetStoreProcedureValue(model, conn, dt, "UspGetBuyerAllOrderInfo", 2);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable GetCrustProductionChallanDetail(ExpPiOrderReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    switch (model.ReportName)
                    {
                        case "CrustProductionChallanDetail":
                            GetStoreProcedureValue(model, conn, dt, "UspGetCrustChallanDetailInfo", 4);
                            break;
                        case "ExpAllBuyerOrderColorLeverWiseDetail":
                            GetStoreProcedureValue(model, conn, dt, "UspGetBuyerAllOrderInfo", 2);
                            break;
                        case "ExpAllBuyerOrderArticleLevelWiseDetail":
                            GetStoreProcedureValue(model, conn, dt, "UspGetBuyerAllOrderInfo", 2);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable GetAgentCommissionDetail(ExpAgentComReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    switch (model.ReportName)
                    {
                        case "ExpAgentCommDetail":
                            GetStoreProcedureValue(model, conn, dt, "UspGetExpAgentCommissionDetail", 1);
                            break;
                        case "ExpAllBuyerOrderColorLeverWiseDetail":
                            GetStoreProcedureValue(model, conn, dt, "UspGetBuyerAllOrderInfo", 2);
                            break;
                        case "ExpAllBuyerOrderArticleLevelWiseDetail":
                            GetStoreProcedureValue(model, conn, dt, "UspGetBuyerAllOrderInfo", 2);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        private void GetStoreProcedureValue(ExpAgentComReportModel model, SqlConnection conn, DataTable dt, string spName, int type)
        {
            using (var cmd = new SqlCommand())
            {
                switch (type)
                {

                    case 1:
                        cmd.Parameters.Add("@AgentComID", SqlDbType.Int).Value = model.AgentComID == null
                            ? 0
                            : Convert.ToInt32(model.AgentComID);

                        cmd.Parameters.Add("@BuyerId", SqlDbType.Int).Value = model.BuyerID == null
                            ? 0
                            : Convert.ToInt32(model.BuyerID);

                        cmd.Parameters.Add("@AgentId", SqlDbType.Int).Value = model.AgentID == null
                            ? 0
                            : Convert.ToInt32(model.AgentID);

                        cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.FromDate == null)
                            ? (object)null
                            : DalCommon.SetDate(model.FromDate);
                        cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.ToDate == null)
                            ? (object)null
                            : DalCommon.SetDate(model.ToDate);
                        break;
                    case 2:
                        cmd.Parameters.Add("@BankId", SqlDbType.Int).Value = model.BankID == null
                            ? 0
                            : Convert.ToInt32(model.BankID);

                        cmd.Parameters.Add("@BVId", SqlDbType.Int).Value = model.BVID == null
                            ? 0
                            : Convert.ToInt32(model.BVID);

                        cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.FromDate == null)
                            ? (object)null
                            : DalCommon.SetDate(model.FromDate);
                        cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.ToDate == null)
                            ? (object)null
                            : DalCommon.SetDate(model.ToDate);
                        break;
                    case 3:
                        cmd.Parameters.Add("@FreightBillID", SqlDbType.Int).Value = model.FreightBillID == null
                            ? 0
                            : Convert.ToInt32(model.FreightBillID);

                        cmd.Parameters.Add("@BuyerId", SqlDbType.Int).Value = model.BuyerID == null
                            ? 0
                            : Convert.ToInt32(model.BuyerID);

                        cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.FromDate == null)
                            ? (object)null
                            : DalCommon.SetDate(model.FromDate);
                        cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.ToDate == null)
                            ? (object)null
                            : DalCommon.SetDate(model.ToDate);
                        break;
                    case 4:
                        cmd.Parameters.Add("@BLID", SqlDbType.Int).Value = model.BLID == null
                            ? 0
                            : Convert.ToInt32(model.BLID);

                        cmd.Parameters.Add("@BuyerId", SqlDbType.Int).Value = model.BuyerID == null
                            ? 0
                            : Convert.ToInt32(model.BuyerID);
                        cmd.Parameters.Add("@TPortID", SqlDbType.Int).Value = model.TPortID == null
                            ? 0
                            : Convert.ToInt32(model.TPortID);
                        cmd.Parameters.Add("@PortID", SqlDbType.Int).Value = model.PortID == null
                            ? 0
                            : Convert.ToInt32(model.PortID);

                        cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.FromDate == null)
                            ? (object)null
                            : DalCommon.SetDate(model.FromDate);
                        cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.ToDate == null)
                            ? (object)null
                            : DalCommon.SetDate(model.ToDate);
                        break;
                }
                cmd.Connection = conn;
                cmd.CommandText = spName;
                cmd.CommandType = CommandType.StoredProcedure;
                using (var adp = new SqlDataAdapter(cmd))
                {
                    adp.Fill(dt);
                }
            }
        }

        public DataTable GetBankVoucherDetail(ExpAgentComReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    switch (model.ReportName)
                    {
                        case "ExpBankVoucherDetail":
                            GetStoreProcedureValue(model, conn, dt, "UspGetExpBankVoucherDetail", 2);
                            break;
                        case "ExpAllBuyerOrderColorLeverWiseDetail":
                            GetStoreProcedureValue(model, conn, dt, "UspGetBuyerAllOrderInfo", 2);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable GetBillLadingDetail(ExpAgentComReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    switch (model.ReportName)
                    {
                        case "ExpBillLadingDetail":
                            GetStoreProcedureValue(model, conn, dt, "UspGetExpBillOfLadingDetail", 4);
                            break;
                        case "ExpAllBuyerOrderColorLeverWiseDetail":
                            GetStoreProcedureValue(model, conn, dt, "UspGetBuyerAllOrderInfo", 2);
                            break;
                        case "ExpAllBuyerOrderArticleLevelWiseDetail":
                            GetStoreProcedureValue(model, conn, dt, "UspGetBuyerAllOrderInfo", 2);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        public DataTable GetFreightBillDetail(ExpAgentComReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    switch (model.ReportName)
                    {
                        case "ExpFreightBillDetail":
                            GetStoreProcedureValue(model, conn, dt, "UspGetExpFreightBillDetail", 3);
                            break;
                        case "ExpFreightBillSummary":
                            GetStoreProcedureValue(model, conn, dt, "UspGetExpFreightBillDetail", 3);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        public DataTable GetChemProdRequisitionInfo(ReportModel model)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    switch (model.ReportName)
                    {
                        case "ChemProdRequisitionWiseItems":
                            GetStoreProcedureValue(model, conn, dt, "UspGetChemProdRequisitionWiseItemsDetail", 6);
                            break;
                        case "ChemProdReqItemWiseSummary":
                            GetStoreProcedureValue(model, conn, dt, "UspGetChemProdReqItemWiseSummary", 6);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        public DataTable UspGetAllFinishLeatherIssueReport(ReportModel model)
        {
            var _dataTable = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    using (var _sqlCmd = new SqlCommand())
                    {
                        _sqlCmd.Parameters.Add("@ArticleChallanID", SqlDbType.Int).Value = (model.ArticleChallanID == null)
                         ? 0 : Convert.ToInt64(model.ArticleChallanID);

                        _sqlCmd.Parameters.Add("@ArticleID", SqlDbType.Int).Value = (model.ArticleID == null)
                         ? 0
                         : Convert.ToInt32(model.ArticleID);

                        _sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                           ? (object)null
                           : DalCommon.SetDate(model.DateFrom);
                        _sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                            ? (object)null
                            : DalCommon.SetDate(model.DateTo);
                        _sqlCmd.CommandText = "UspGetAllFinishLeatherIssueReport";


                        _sqlCmd.Connection = conn;
                        _sqlCmd.CommandType = CommandType.StoredProcedure;
                        using (var _sqlAdp = new SqlDataAdapter(_sqlCmd))
                        {
                            _sqlAdp.Fill(_dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dataTable;
        }
        public DataTable UspCrustLeatherIssueByArticalChallanNo(ReportModel model)
        {
            var _dataTable = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    switch (model.ReportName)
                    {
                        case "Challan":

                            using (var _sqlCmd = new SqlCommand())
                            {
                                _sqlCmd.Parameters.Add("@ArticleChallanID", SqlDbType.Int).Value = model.ArticleChallanID;

                                _sqlCmd.Parameters.Add("@ArticleID", SqlDbType.Int).Value = model.ArticalID;

                                _sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                                    ? (object)null
                                    : DalCommon.SetDate(model.DateFrom);
                                _sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                                    ? (object)null
                                    : DalCommon.SetDate(model.DateTo);
                                _sqlCmd.Parameters.Add("@ReportName", SqlDbType.VarChar).Value = "Challan";
                                _sqlCmd.CommandText = "UspCrustLeatherIssueByArticalChallanNo";

                                _sqlCmd.Connection = conn;
                                _sqlCmd.CommandType = CommandType.StoredProcedure;
                                using (var _sqlAdp = new SqlDataAdapter(_sqlCmd))
                                {
                                    _sqlAdp.Fill(_dataTable);
                                }
                            }

                            break;
                        case "Article":
                            using (var _sqlCmd = new SqlCommand())
                            {
                                _sqlCmd.Parameters.Add("@ArticleChallanID", SqlDbType.Int).Value = model.ArticleChallanID;

                                _sqlCmd.Parameters.Add("@ArticleID", SqlDbType.Int).Value = model.ArticalID;

                                _sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                                    ? (object)null
                                    : DalCommon.SetDate(model.DateFrom);
                                _sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                                    ? (object)null
                                    : DalCommon.SetDate(model.DateTo);
                                _sqlCmd.Parameters.Add("@ReportName", SqlDbType.VarChar).Value = "Article";
                                _sqlCmd.CommandText = "UspCrustLeatherIssueByArticalChallanNo";

                                _sqlCmd.Connection = conn;
                                _sqlCmd.CommandType = CommandType.StoredProcedure;
                                using (var _sqlAdp = new SqlDataAdapter(_sqlCmd))
                                {
                                    _sqlAdp.Fill(_dataTable);
                                }
                            }

                            break;

                        case "BuyerIssue":
                            using (var _sqlCmd = new SqlCommand())
                            {
                                _sqlCmd.Parameters.Add("@BuyerID", SqlDbType.Int).Value = model.BuyerID;
                                _sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                                    ? (object)null
                                    : DalCommon.SetDate(model.DateFrom);
                                _sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                                    ? (object)null
                                    : DalCommon.SetDate(model.DateTo);

                                _sqlCmd.CommandText = "UspCrustLeatherIssueByBuyer";

                                _sqlCmd.Connection = conn;
                                _sqlCmd.CommandType = CommandType.StoredProcedure;
                                using (var _sqlAdp = new SqlDataAdapter(_sqlCmd))
                                {
                                    _sqlAdp.Fill(_dataTable);
                                }
                            }

                            break;


                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dataTable;
        }

        public DataTable UspFinishLeatherIssueByArtiChalNo(ReportModel model)
        {
            var _dataTable = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    switch (model.ReportName)
                    {
                        case "ChallanWiseIssue":

                            using (var _sqlCmd = new SqlCommand())
                            {
                                _sqlCmd.Parameters.Add("@ArticleChallanID", SqlDbType.Int).Value = model.ArticleChallanID;

                                _sqlCmd.Parameters.Add("@ArticleID", SqlDbType.Int).Value = model.ArticalID;

                                _sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                                    ? (object)null
                                    : DalCommon.SetDate(model.DateFrom);
                                _sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                                    ? (object)null
                                    : DalCommon.SetDate(model.DateTo);
                                _sqlCmd.Parameters.Add("@ReportName", SqlDbType.VarChar).Value = "ChallanWiseIssue";
                                _sqlCmd.CommandText = "UspFinishLeatherIssueByArtiChalNo";
                                _sqlCmd.Connection = conn;
                                _sqlCmd.CommandType = CommandType.StoredProcedure;
                                using (var _sqlAdp = new SqlDataAdapter(_sqlCmd))
                                {
                                    _sqlAdp.Fill(_dataTable);
                                }
                            }

                            break;
                        case "ArticleWiseIssue":
                            using (var _sqlCmd = new SqlCommand())
                            {
                                _sqlCmd.Parameters.Add("@ArticleChallanID", SqlDbType.Int).Value = model.ArticleChallanID;

                                _sqlCmd.Parameters.Add("@ArticleID", SqlDbType.Int).Value = model.ArticalID;

                                _sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                                    ? (object)null
                                    : DalCommon.SetDate(model.DateFrom);
                                _sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                                    ? (object)null
                                    : DalCommon.SetDate(model.DateTo);
                                _sqlCmd.Parameters.Add("@ReportName", SqlDbType.VarChar).Value = "ArticleWiseIssue";
                                _sqlCmd.CommandText = "UspFinishLeatherIssueByArtiChalNo";


                                _sqlCmd.Connection = conn;
                                _sqlCmd.CommandType = CommandType.StoredProcedure;
                                using (var _sqlAdp = new SqlDataAdapter(_sqlCmd))
                                {
                                    _sqlAdp.Fill(_dataTable);
                                }

                            }

                            break;

                        case "BuyerWiseIssue":
                            using (var _sqlCmd = new SqlCommand())
                            {
                                _sqlCmd.Parameters.Add("@ArticleChallanID", SqlDbType.Int).Value = model.ArticleChallanID;

                                _sqlCmd.Parameters.Add("@ArticleID", SqlDbType.Int).Value = model.ArticalID;

                                _sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                                    ? (object)null
                                    : DalCommon.SetDate(model.DateFrom);
                                _sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                                    ? (object)null
                                    : DalCommon.SetDate(model.DateTo);
                                _sqlCmd.Parameters.Add("@ReportName", SqlDbType.VarChar).Value = "BuyerWiseIssue";
                                _sqlCmd.CommandText = "UspFinishLeatherIssueByArtiChalNo";


                                _sqlCmd.Connection = conn;
                                _sqlCmd.CommandType = CommandType.StoredProcedure;
                                using (var _sqlAdp = new SqlDataAdapter(_sqlCmd))
                                {
                                    _sqlAdp.Fill(_dataTable);
                                }

                            }

                            break;
                        case "DailyIssue":
                            using (var _sqlCmd = new SqlCommand())
                            {
                                _sqlCmd.Parameters.Add("@ArticleChallanID", SqlDbType.Int).Value = model.ArticleChallanID;

                                _sqlCmd.Parameters.Add("@ArticleID", SqlDbType.Int).Value = model.ArticalID;

                                _sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                                    ? (object)null
                                    : DalCommon.SetDate(model.DateFrom);
                                _sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                                    ? (object)null
                                    : DalCommon.SetDate(model.DateTo);
                                _sqlCmd.Parameters.Add("@ReportName", SqlDbType.VarChar).Value = "DailyIssue";
                                _sqlCmd.CommandText = "UspFinishLeatherIssueByArtiChalNo";


                                _sqlCmd.Connection = conn;
                                _sqlCmd.CommandType = CommandType.StoredProcedure;
                                using (var _sqlAdp = new SqlDataAdapter(_sqlCmd))
                                {
                                    _sqlAdp.Fill(_dataTable);
                                }

                            }

                            break;

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dataTable;
            ////var _dataTable = new DataTable();
            ////try
            ////{
            ////    using (var conn = new SqlConnection(_connString))
            ////    {
            ////        conn.Open();
            ////        //switch (model.ReportName)
            ////        //{
            ////        //    //case "ChallanWiseIssue":

            ////        using (var _sqlCmd = new SqlCommand())
            ////        {
            ////            _sqlCmd.Parameters.Add("@ArticleChallanID", SqlDbType.Int).Value = model.ArticleChallanID;

            ////            _sqlCmd.Parameters.Add("@ArticleID", SqlDbType.Int).Value = model.ArticalID;

            ////            _sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
            ////                ? (object)null
            ////                : DalCommon.SetDate(model.DateFrom);
            ////            _sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
            ////                ? (object)null
            ////                : DalCommon.SetDate(model.DateTo);
            ////            _sqlCmd.CommandText = "UspFinishLeatherIssueByArtiChalNo";


            ////            _sqlCmd.Connection = conn;
            ////            _sqlCmd.CommandType = CommandType.StoredProcedure;
            ////            using (var _sqlAdp = new SqlDataAdapter(_sqlCmd))
            ////            {
            ////                _sqlAdp.Fill(_dataTable);
            ////            }
            ////        }


            ////    }
            ////}
            ////catch (Exception ex)
            ////{
            ////    throw ex;
            ////}
            ////return _dataTable;
        }

        public DataTable UspFinishLeatherStockByArtiChalNo(ReportModel model)
        {
            var _dataTable = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    switch (model.ReportName)
                    {
                        case "ChallanWiseStock":

                            using (var _sqlCmd = new SqlCommand())
                            {
                                _sqlCmd.Parameters.Add("@ArticleChallanID", SqlDbType.Int).Value = model.ArticleChallanID;

                                _sqlCmd.Parameters.Add("@ArticleID", SqlDbType.Int).Value = model.ArticalID;

                                _sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                                    ? (object)null
                                    : DalCommon.SetDate(model.DateFrom);
                                _sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                                    ? (object)null
                                    : DalCommon.SetDate(model.DateTo);
                                _sqlCmd.Parameters.Add("@ReportName", SqlDbType.VarChar).Value = "ChallanWiseStock";
                                _sqlCmd.CommandText = "UspFinishLeatherStockByArtiChalNo";
                                _sqlCmd.Connection = conn;
                                _sqlCmd.CommandType = CommandType.StoredProcedure;
                                using (var _sqlAdp = new SqlDataAdapter(_sqlCmd))
                                {
                                    _sqlAdp.Fill(_dataTable);
                                }
                            }

                            break;
                        case "ArticalWiseStock":
                            using (var _sqlCmd = new SqlCommand())
                            {
                                _sqlCmd.Parameters.Add("@ArticleChallanID", SqlDbType.Int).Value = model.ArticleChallanID;

                                _sqlCmd.Parameters.Add("@ArticleID", SqlDbType.Int).Value = model.ArticalID;

                                _sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                                    ? (object)null
                                    : DalCommon.SetDate(model.DateFrom);
                                _sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                                    ? (object)null
                                    : DalCommon.SetDate(model.DateTo);
                                _sqlCmd.Parameters.Add("@ReportName", SqlDbType.VarChar).Value = "ArticalWiseStock";
                                _sqlCmd.CommandText = "UspFinishLeatherStockByArtiChalNo";


                                _sqlCmd.Connection = conn;
                                _sqlCmd.CommandType = CommandType.StoredProcedure;
                                using (var _sqlAdp = new SqlDataAdapter(_sqlCmd))
                                {
                                    _sqlAdp.Fill(_dataTable);
                                }
                            }

                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _dataTable;

        }

        public DataTable UspCrustStockReport(ReportModel model)
        {
            var _dataTable = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();

                    using (var _sqlCmd = new SqlCommand())
                    {
                        _sqlCmd.Parameters.Add("@BuyerID", SqlDbType.Int).Value = Convert.ToInt32(model.BuyerID == null ? "0" : model.BuyerID);

                        _sqlCmd.Parameters.Add("@ArticleChallanID", SqlDbType.Int).Value = model.ArticleChallanID;

                        _sqlCmd.Parameters.Add("@ArticleID", SqlDbType.Int).Value = model.ArticalID;

                        _sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                            ? (object)null
                            : DalCommon.SetDate(model.DateFrom);
                        _sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                            ? (object)null
                            : DalCommon.SetDate(model.DateTo);
                        _sqlCmd.Parameters.Add("@ReportName", SqlDbType.VarChar).Value = (model.ReportName == null)
                           ? (object)null
                           : model.ReportName.ToString();
                        _sqlCmd.CommandText = "UspCrustStockReport";


                        _sqlCmd.Connection = conn;
                        _sqlCmd.CommandType = CommandType.StoredProcedure;
                        using (var _sqlAdp = new SqlDataAdapter(_sqlCmd))
                        {
                            _sqlAdp.Fill(_dataTable);
                        }
                    }

                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
            return _dataTable;
        }

        public DataTable UspFinishLeaterStockRpt(ReportModel model)
        {
            var _dataTable = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();

                    using (var _sqlCmd = new SqlCommand())
                    {

                        _sqlCmd.Parameters.Add("@ArticleChallanID", SqlDbType.Int).Value = model.ArticleChallanID;

                        _sqlCmd.Parameters.Add("@ArticleID", SqlDbType.Int).Value = model.ArticalID;

                        _sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = (model.DateFrom == null)
                            ? (object)null
                            : DalCommon.SetDate(model.DateFrom);
                        _sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = (model.DateTo == null)
                            ? (object)null
                            : DalCommon.SetDate(model.DateTo);

                        _sqlCmd.CommandText = "UspFinishLeaterStockRpt";


                        _sqlCmd.Connection = conn;
                        _sqlCmd.CommandType = CommandType.StoredProcedure;
                        using (var _sqlAdp = new SqlDataAdapter(_sqlCmd))
                        {
                            _sqlAdp.Fill(_dataTable);
                        }
                    }

                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
            return _dataTable;
        }
    }
}
