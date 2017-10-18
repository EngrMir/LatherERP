using System;
using System.Text;
using System.Web;
using System.Data;
using System.Linq;
using DatabaseUtility;
using System.Transactions;
using System.Threading.Tasks;
using System.Globalization;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using ERP.EntitiesModel.AppSetupModel;


namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalWBIStoreSelection
    {

        private UnitOfWork repository = new UnitOfWork();
        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;
        public string WBSelectionNo = string.Empty;

        public long IssueID = 0;
        public long WBSIssueItemID = 0;
        public string IssueNo = string.Empty;
        private int stockOver = 0;
        private bool save;
        private readonly string _connString;

        public DalWBIStoreSelection()
        {
            _vmMsg = new ValidationMsg();
            _context = new BLC_DEVEntities();
            _connString = StrConnection.GetConnectionString();
        }

        public long GetIssueID()
        {
            return IssueID;
        }

        public long GetWBSIssueItemID()
        {
            return WBSIssueItemID;
        }

        public string GetIssueNo()
        {
            return IssueNo;
        }


        public IEnumerable<SysUnit> GetAllActiveLeatherUnit()
        {
            IEnumerable<SysUnit> iLstSysItemType = from sit in _context.Sys_Unit
                                                   where sit.IsActive && !sit.IsDelete && sit.UnitCategory == "Leather"
                                                   select new SysUnit
                                                   {
                                                       UnitID = sit.UnitID,
                                                       UnitName = sit.UnitName
                                                   };

            return iLstSysItemType;
        }


        //############################################### SELECTION POPUP ACCORDING STORE ##########################################

        #region SELECTION POPUP ACCORDING STORE

        public List<wbSelectionIssue> GetWBSelectionInfo(string StoreID)
        {
            if (!string.IsNullOrEmpty(StoreID))
            {
                var query =
                    @"  SELECT T1.* FROM 
                                            (SELECT		ISNULL(wbs.WBSelectionID,0)WBSelectionID,
                                                        ISNULL(wbs.WBSelectionNo,'')WBSelectionNo,
								                        CONVERT(VARCHAR(15),wbs.SelectionDate, 106) SelectionDate,
                                                        ISNULL(wbs.StoreID,0)StoreID,
								                        ISNULL(st.StoreName,'')StoreName,
                                                        --ISNULL(wbs.SelectedBy,'')SelectedBy,
                                                        ISNULL(wbs.Remarks,'')Remarks,
                                                        ISNULL(wbsi.WBSelectItemID,0)WBSelectItemID,
                                                        ISNULL(wbsi.WBSelectItemNo,'')WBSelectItemNo,
                                                        ISNULL(wbsi.SupplierID,0)SupplierID,
								                        ISNULL(s.SupplierName,'')SupplierName,
                                                        ISNULL(wbsi.PurchaseID,0)PurchaseID,
								                        ISNULL(p.PurchaseNo,'')PurchaseNo,
                                                        ISNULL(wbsi.ItemTypeID,0)ItemTypeID,
								                        ISNULL(i.ItemTypeName,'')ItemTypeName,
                                                        ISNULL(wbsi.LeatherTypeID,0)LeatherTypeID,
								                        ISNULL(l.LeatherStatusName,'')LeatherStatusName,								
								                        ISNULL(wbsi.LeatherStatusID,0)LeatherStatusID,
                                                        ISNULL(wbsi.ProductionPcs,0)ProductionPcs,
                                                        ISNULL(wbsi.ProductionSide,0)ProductionSide,
                                                        ISNULL(wbsi.ProductionArea,0)ProductionArea,
								                        ISNULL(wbsi.ProductionAreaUnit,0)ProductionAreaUnit,
								                        ISNULL(u.UnitName,'')UnitName
								                         FROM PRD_WBSelection WBS
                          INNER JOIN  PRD_WBSelectionItem WBSI  ON WBSI.WBSelectionID=WBS.WBSelectionID
                          INNER JOIN  SYS_Store st ON wbs.StoreID=st.StoreID
                          INNER JOIN  Sys_ItemType i ON wbsi.ItemTypeID=i.ItemTypeID
                          INNER JOIN  Sys_LeatherStatus l ON wbsi.LeatherStatusID=l.LeatherStatusID
                          INNER JOIN  Prq_Purchase p ON wbsi.PurchaseID=p.PurchaseID
                          INNER JOIN  Sys_Supplier s ON wbsi.SupplierID=s.SupplierID
                            INNER JOIN  Sys_Unit u ON wbsi.ProductionAreaUnit=u.UnitID
                          WHERE wbs.RecordStatus='CNF' AND WBS.RecordState='ATS'
                          ) T1

                          INNER JOIN (
                           SELECT SM.StoreID,SM.SupplierID,SM.PurchaseID,SM.ItemTypeID,SM.LeatherStatusID,SUM(SS.ClosingStockPcs)ClosingStockPcs,SUM(SS.ClosingStockSide)ClosingStockSide,SUM(SS.ClosingStockArea )ClosingStockArea FROM INV_WetBlueSelectionStock SS
                          INNER JOIN
                          (SELECT MAX(TransectionID) TransectionID, StoreID,SupplierID,PurchaseID,ItemTypeID,LeatherStatusID,GradeID FROM INV_WetBlueSelectionStock
                          GROUP BY StoreID,SupplierID,PurchaseID,ItemTypeID,LeatherStatusID,GradeID
                          ) AS SM ON SM.TransectionID=SS.TransectionID
                          GROUP BY SM.StoreID,SM.SupplierID,SM.PurchaseID,SM.ItemTypeID,SM.LeatherStatusID
                          ) AS T2
                          ON T1.StoreID=T2.StoreID AND T1.SupplierID=T2.SupplierID AND T1.PurchaseID=T2.PurchaseID AND T1.ItemTypeID=T2.ItemTypeID AND T1.LeatherStatusID=T2.LeatherStatusID
                          WHERE (T2.ClosingStockPcs > 0 OR T2.ClosingStockSide > 0 OR T2.ClosingStockArea> 0 ) AND T1.StoreID='" +
                    StoreID + "'";
                var allData = _context.Database.SqlQuery<wbSelectionIssue>(query).ToList();
                return allData.ToList();
                //List<wbSelectionIssue> searchList = allData.ToList();
                //return searchList.Select(c => SetToBussinessObject(c)).ToList<wbSelectionIssue>();
            }
            return null;
        }

        public wbSelectionIssueItem SetToBussinessObject(PRD_WBSelectionItem Entity)
        {
            wbSelectionIssueItem Model = new wbSelectionIssueItem();

            Model.WBSelectItemID = Entity.WBSelectItemID;
            Model.WBSelectItemNo = Entity.WBSelectItemNo;
            Model.WBSelectionID = Entity.WBSelectionID;
            Model.SupplierID = Entity.SupplierID;
            Model.SupplierName = Entity.SupplierID == null
                ? ""
                : _context.Sys_Supplier.Where(m => m.SupplierID == Entity.SupplierID).FirstOrDefault().SupplierName;
            Model.PurchaseID = Entity.PurchaseID;
            Model.PurchaseNo = Entity.PurchaseID == null
                ? ""
                : _context.Prq_Purchase.Where(m => m.PurchaseID == Entity.PurchaseID).FirstOrDefault().PurchaseNo;
            Model.ItemTypeID = Entity.ItemTypeID;
            Model.ItemTypeName = Entity.ItemTypeID == null
                ? ""
                : _context.Sys_ItemType.Where(m => m.ItemTypeID == Entity.ItemTypeID).FirstOrDefault().ItemTypeName;
            Model.LeatherTypeID = Entity.LeatherTypeID;
            Model.LeatherTypeName = Entity.LeatherTypeID == null
                ? ""
                : _context.Sys_LeatherType.Where(m => m.LeatherTypeID == Entity.LeatherTypeID)
                    .FirstOrDefault()
                    .LeatherTypeName;
            Model.LeatherStatusID = Entity.LeatherStatusID;
            Model.LeatherStatusName = Entity.LeatherStatusID == null
                ? ""
                : _context.Sys_LeatherStatus.Where(m => m.LeatherStatusID == Entity.LeatherStatusID)
                    .FirstOrDefault()
                    .LeatherStatusName;
            Model.ProductionPcs = Entity.ProductionPcs == null ? 0 : Entity.ProductionPcs;
            Model.ProductionSide = Entity.ProductionSide == null ? 0 : Entity.ProductionSide;
            Model.ProductionArea = Entity.ProductionArea == null ? 0 : Entity.ProductionArea;
            Model.ProductionAreaUnit = Entity.ProductionAreaUnit;
            Model.UnitName = Entity.ProductionAreaUnit == null
                ? ""
                : _context.Sys_Unit.Where(m => m.UnitID == Entity.ProductionAreaUnit).FirstOrDefault().UnitName;
            Model.WBSelectionNo = Entity.WBSelectionID == null
                ? ""
                : _context.PRD_WBSelection.Where(m => m.WBSelectionID == Entity.WBSelectionID)
                    .FirstOrDefault()
                    .WBSelectionNo;
            //Model.SelectionDate = Entity.WBSelectionID == null ? "" : Convert.ToDateTime(_context.PRD_WBSelection.Where(m => m.WBSelectionID == Entity.WBSelectionID).FirstOrDefault().SelectionDate).ToString("dd/MM/yyyy");
            Model.SelectedBy = Entity.WBSelectionID == null
                ? null
                : _context.PRD_WBSelection.Where(m => m.WBSelectionID == Entity.WBSelectionID)
                    .FirstOrDefault()
                    .SelectedBy;
            Model.SelectedByName = Model.SelectedBy == null
                ? ""
                : _context.Users.Where(m => m.UserID == Model.SelectedBy).FirstOrDefault().Title +
                  _context.Users.Where(m => m.UserID == Model.SelectedBy).FirstOrDefault().FirstName +
                  _context.Users.Where(m => m.UserID == Model.SelectedBy).FirstOrDefault().LastName;
            Model.SelectionRemarks = Entity.WBSelectionID == null
                ? ""
                : _context.PRD_WBSelection.Where(m => m.WBSelectionID == Entity.WBSelectionID).FirstOrDefault().Remarks;

            return Model;
        }

        public wbSelectionIssue SetToBussinessObject(wbSelectionIssue Entity)
        {
            wbSelectionIssue Model = new wbSelectionIssue();

            Model.WBSelectItemID = Entity.WBSelectItemID;
            Model.WBSelectItemNo = Entity.WBSelectItemNo;
            Model.WBSelectionID = Entity.WBSelectionID;
            Model.SupplierID = Entity.SupplierID;
            Model.SupplierName = Entity.SupplierID == null
                ? ""
                : _context.Sys_Supplier.Where(m => m.SupplierID == Entity.SupplierID).FirstOrDefault().SupplierName;
            Model.PurchaseID = Entity.PurchaseID;
            Model.PurchaseNo = Entity.PurchaseID == null
                ? ""
                : _context.Prq_Purchase.Where(m => m.PurchaseID == Entity.PurchaseID).FirstOrDefault().PurchaseNo;
            Model.ItemTypeID = Entity.ItemTypeID;
            Model.ItemTypeName = Entity.ItemTypeID == null
                ? ""
                : _context.Sys_ItemType.Where(m => m.ItemTypeID == Entity.ItemTypeID).FirstOrDefault().ItemTypeName;
            //Model.LeatherTypeID = Entity.LeatherTypeID;
            //Model.LeatherTypeName = Entity.LeatherTypeID == null ? "" : _context.Sys_LeatherType.Where(m => m.LeatherTypeID == Entity.LeatherTypeID).FirstOrDefault().LeatherTypeName;
            Model.LeatherStatusID = Entity.LeatherStatusID;
            Model.LeatherStatusName = Entity.LeatherStatusID == null
                ? ""
                : _context.Sys_LeatherStatus.Where(m => m.LeatherStatusID == Entity.LeatherStatusID)
                    .FirstOrDefault()
                    .LeatherStatusName;
            Model.ProductionPcs = Entity.ProductionPcs == null ? 0 : Entity.ProductionPcs;
            Model.ProductionSide = Entity.ProductionSide == null ? 0 : Entity.ProductionSide;
            Model.ProductionArea = Entity.ProductionArea == null ? 0 : Entity.ProductionArea;
            Model.ProductionAreaUnit = Entity.ProductionAreaUnit;
            Model.UnitName = Entity.ProductionAreaUnit == null
                ? ""
                : _context.Sys_Unit.Where(m => m.UnitID == Entity.ProductionAreaUnit).FirstOrDefault().UnitName;
            Model.WBSelectionNo = Entity.WBSelectionID == null
                ? ""
                : _context.PRD_WBSelection.Where(m => m.WBSelectionID == Entity.WBSelectionID)
                    .FirstOrDefault()
                    .WBSelectionNo;
            Model.SelectionDate = Entity.WBSelectionID == null
                ? ""
                : Convert.ToDateTime(
                    _context.PRD_WBSelection.Where(m => m.WBSelectionID == Entity.WBSelectionID)
                        .FirstOrDefault()
                        .SelectionDate).ToString("dd/MM/yyyy");
            //Model.SelectedBy = Entity.WBSelectionID == null ? null : _context.PRD_WBSelection.Where(m => m.WBSelectionID == Entity.WBSelectionID).FirstOrDefault().SelectedBy;
            Model.SelectedByName = Model.SelectedBy == null
                ? ""
                : _context.Users.Where(m => m.UserID == Model.SelectedBy).FirstOrDefault().Title +
                  _context.Users.Where(m => m.UserID == Model.SelectedBy).FirstOrDefault().FirstName +
                  _context.Users.Where(m => m.UserID == Model.SelectedBy).FirstOrDefault().LastName;
            Model.SelectionRemarks = Entity.WBSelectionID == null
                ? ""
                : _context.PRD_WBSelection.Where(m => m.WBSelectionID == Entity.WBSelectionID).FirstOrDefault().Remarks;
            //Model.RecordStatus = DalCommon.ReturnRecordStatus("Checked");
            Model.RecordStatus = Entity.RecordStatus;
            return Model;
        }

        public List<wbSelectionIssueGrade> GetWBSelectionGrade(string WBSelectionID, string IssueFrom, string SupplierID,
            string PurchaseID, string ItemTypeID, string LeatherStatusID)
        {
            if (!string.IsNullOrEmpty(IssueFrom))
            {
                var query = @"SELECT	inv.TransectionID,
                                        inv.GradeID,
                                        CASE    inv.SizeQtyRef 
				                                WHEN 'SizeQty1' THEN '12-15 sft'
				                                WHEN 'SizeQty2'	THEN '16-20 sft'
				                                WHEN 'SizeQty3' THEN '21-25 sft'
				                                WHEN 'SizeQty4' THEN '26-30 sft'
				                                WHEN 'SizeQty5' THEN '31-35 sft'
				                                WHEN 'SideQty' THEN 'Side'
				                                WHEN 'AreaQty' THEN 'Area'
		                                END     SizeQtyRef,
							            (SELECT GradeName FROM dbo.Sys_Grade WHERE GradeID = inv.GradeID)GradeName,
                                        ISNULL(inv.ClosingStockPcs,0)SelectedGradeQty,
							            ISNULL(inv.ClosingStockSide,0)SelectedGradeSide,
							            ISNULL(inv.ClosingStockArea,0)SelectedGradeArea,
							            ISNULL(inv.ClosingStockAreaUnit,0)SelectedAreaUnitID,
										sg.WBSelectionGradeID,
										sg.WBSelectItemID,
										ISNULL(sg.WBSelectItemNo,'') WBSelectItemNo,
										ISNULL(sg.GradeQty,0)GradeQty,
										ISNULL(sg.GradeSide,0)GradeSide,
										ISNULL(sg.GradeArea,0)GradeArea,
										sg.AreaUnitID 
                                FROM   INV_WetBlueSelectionStock inv
								INNER JOIN (SELECT	WBSelectionGradeID,
													WBSelectItemID,
													WBSelectItemNo,
													WBSelectionID,
													GradeID,
													GradeQty,
													GradeSide,
													GradeArea,
													AreaUnitID 
											 FROM	PRD_WBSelectionGrade  WHERE WBSelectionID = " + WBSelectionID +
                            ")sg ON inv.GradeID=sg.GradeID INNER JOIN (SELECT MAX(TransectionID)TransectionID,SupplierID,StoreID,PurchaseID,ItemTypeID,LeatherStatusID,GradeID,SizeQtyRef,ClosingStockSide,ClosingStockArea  FROM	 INV_WetBlueSelectionStock  GROUP BY StoreID,SupplierID,ItemTypeID,PurchaseID,LeatherStatusID,GradeID,SizeQtyRef,ClosingStockSide,ClosingStockArea) sup ON inv.TransectionID=sup.TransectionID WHERE inv.StoreID = " +
                            IssueFrom + " and inv.SupplierID = " + SupplierID + " and inv.PurchaseID = " + PurchaseID +
                            " and inv.ItemTypeID = " + ItemTypeID + " and inv.LeatherStatusID = " + LeatherStatusID +
                            " and (inv.ClosingStockPcs>0 OR inv.ClosingStockSide>0 OR inv.ClosingStockArea>0)";
                var allData = _context.Database.SqlQuery<wbSelectionIssueGrade>(query).ToList();
                List<wbSelectionIssueGrade> searchList = allData.ToList();
                return searchList.Select(c => SetToBussinessObject(c)).ToList<wbSelectionIssueGrade>();
            }
            return null;
        }

        public wbSelectionIssueGrade SetToBussinessObject(wbSelectionIssueGrade Entity)
        {
            wbSelectionIssueGrade Model = new wbSelectionIssueGrade();

            Model.WBSelectionGradeID = Entity.WBSelectionGradeID;
            Model.WBSIssueItemID = Entity.WBSIssueItemID;
            Model.WBSelectItemNo = Entity.WBSelectItemNo;
            Model.WBSIssueGradeID = Entity.WBSIssueGradeID;

            Model.WBSelectItemNo = Entity.WBSelectItemNo;
            Model.GradeID = Entity.GradeID;
            Model.GradeName = Entity.GradeID == null
                ? ""
                : _context.Sys_Grade.Where(m => m.GradeID == Entity.GradeID).FirstOrDefault().GradeName;
            Model.SizeQtyRef = Entity.SizeQtyRef == null ? "" : Entity.SizeQtyRef;
            Model.SelectedGradeQty = Entity.SelectedGradeQty == null ? 0 : Entity.SelectedGradeQty;
            Model.SelectedGradeSide = Entity.SelectedGradeSide == null ? 0 : Entity.SelectedGradeSide;
            Model.SelectedGradeArea = Entity.SelectedGradeArea == null ? 0 : Entity.SelectedGradeArea;
            Model.SelectedAreaUnitID = Entity.SelectedAreaUnitID == null ? 0 : Entity.SelectedAreaUnitID;

            //Model.SelectedUnitName = Entity.SelectedAreaUnitID == null
            //    ? ""
            //    : _context.Sys_Unit.Where(m => m.UnitID == Entity.SelectedAreaUnitID).FirstOrDefault().UnitName;
            
            //Model.SelectedUnitName =  _context.Sys_Unit.Where(m => m.UnitID == Entity.SelectedAreaUnitID).FirstOrDefault().UnitName;
            Model.IssueUnitName = Entity.AreaUnitID == null
                ? ""
                : _context.Sys_Unit.Where(m => m.UnitID == Entity.AreaUnitID).FirstOrDefault().UnitName;
            Model.GradeQty = Entity.GradeQty == null ? 0 : Entity.GradeQty;
            Model.GradeSide = Entity.GradeSide == null ? 0 : Entity.GradeSide;
            Model.GradeArea = Entity.GradeArea == null ? 0 : Entity.GradeArea;
            Model.AreaUnitID = Entity.AreaUnitID == null ? 0 : Entity.AreaUnitID;
            Model.ClosingStockAreaUnit = Entity.ClosingStockAreaUnit;
            Model.UnitName = Entity.AreaUnitID == null
                ? ""
                : _context.Sys_Unit.Where(m => m.UnitID == Entity.AreaUnitID).FirstOrDefault().UnitName;



            return Model;
        }

        //**************************************** For GRADE POP UP *******************************************
        public List<wbSelectionIssue> GetStockGradeInfo(string StoreID)
        {
            if (!string.IsNullOrEmpty(StoreID))
            {
                var query =
                    @" SELECT	inv.TransectionID,
                                        inv.SupplierID,
										s.SupplierName,
										inv.PurchaseID,
										p.PurchaseNo,
										inv.ItemTypeID,
										i.ItemTypeName,
										inv.LeatherStatusID,
										l.LeatherStatusName,
										inv.LeatherTypeID,
										u.UnitName,
										inv.GradeID,
							            (SELECT GradeName FROM dbo.Sys_Grade WHERE GradeID = inv.GradeID)GradeName,
                                        ISNULL(inv.ClosingStockPcs,0)SelectedGradeQty,
							            ISNULL(inv.ClosingStockSide,0)SelectedGradeSide,
							            ISNULL(inv.ClosingStockArea,0)SelectedGradeArea,
							            ISNULL(inv.ClosingStockAreaUnit,0)SelectedAreaUnitID,
                                        ISNULL(inv.ClosingStockAreaUnit,0)AreaUnitID
                                FROM dbo.INV_WetBlueSelectionStock inv
								INNER JOIN  SYS_Store st ON inv.StoreID=st.StoreID
								INNER JOIN  Sys_ItemType i ON inv.ItemTypeID=i.ItemTypeID
								INNER JOIN  Sys_LeatherStatus l ON inv.LeatherStatusID=l.LeatherStatusID
								INNER JOIN  Prq_Purchase p ON inv.PurchaseID=p.PurchaseID
								INNER JOIN  Sys_Supplier s ON inv.SupplierID=s.SupplierID
								INNER JOIN  Sys_Unit u ON inv.ClosingStockAreaUnit=u.UnitID
								INNER JOIN (SELECT MAX(TransectionID)TransectionID,SupplierID,StoreID,ItemTypeID,LeatherStatusID,GradeID FROM dbo.INV_WetBlueSelectionStock GROUP BY StoreID,SupplierID,ItemTypeID,PurchaseID,LeatherStatusID,GradeID) sup ON inv.TransectionID=sup.TransectionID WHERE inv.StoreID ='" +
                    StoreID + "'";
                var allData = _context.Database.SqlQuery<wbSelectionIssue>(query).ToList();
                return allData.ToList();
            }
            return null;
        }

        public List<wbSelectionIssueGrade> GetWBGradeforGrid(string IssueFrom, string SupplierID, string PurchaseID,
            string ItemTypeID, string LeatherStatusID)
        {
            if (!string.IsNullOrEmpty(IssueFrom))
            {
                var query =
                    @"SELECT    inv.TransectionID,
                                        inv.GradeID,
                                        CASE    inv.SizeQtyRef 
				                                WHEN 'SizeQty1' THEN '12-15 sft'
				                                WHEN 'SizeQty2'	THEN '16-20 sft'
				                                WHEN 'SizeQty3' THEN '21-25 sft'
				                                WHEN 'SizeQty4' THEN '26-30 sft'
				                                WHEN 'SizeQty5' THEN '31-35 sft'
				                                WHEN 'SideQty' THEN 'Side'
				                                WHEN 'AreaQty' THEN 'Area'
		                                END     SizeQtyRef,
							            (SELECT GradeName FROM dbo.Sys_Grade WHERE GradeID = inv.GradeID)GradeName,
                                        ISNULL(inv.ClosingStockPcs,0)SelectedGradeQty,
							            ISNULL(inv.ClosingStockSide,0)SelectedGradeSide,
							            ISNULL(inv.ClosingStockArea,0)SelectedGradeArea,
							            ISNULL(inv.ClosingStockAreaUnit,0)SelectedAreaUnitID,
                                        (SELECT UnitName FROM Sys_Unit WHERE UnitID = inv.ClosingStockAreaUnit)UnitName
                                FROM    INV_WetBlueSelectionStock inv
								INNER JOIN (SELECT MAX(TransectionID)TransectionID,SupplierID,StoreID,ItemTypeID,LeatherStatusID,GradeID,SizeQtyRef,ClosingStockSide,ClosingStockArea FROM INV_WetBlueSelectionStock GROUP BY StoreID,SupplierID,ItemTypeID,PurchaseID,LeatherStatusID,GradeID,SizeQtyRef,ClosingStockSide,ClosingStockArea) sup ON inv.TransectionID=sup.TransectionID WHERE inv.StoreID = " +
                    IssueFrom + " and inv.SupplierID = " + SupplierID + " and inv.PurchaseID = " + PurchaseID +
                    " and inv.ItemTypeID = " + ItemTypeID + " and inv.LeatherStatusID = " + LeatherStatusID +
                    " and (inv.ClosingStockPcs>0 OR inv.ClosingStockSide>0 OR inv.ClosingStockArea>0)";
                var allData = _context.Database.SqlQuery<wbSelectionIssueGrade>(query).ToList();
                List<wbSelectionIssueGrade> searchList = allData.ToList();
                return searchList.Select(c => SetToBussinessObjectforGridGrade(c)).ToList<wbSelectionIssueGrade>();
            }
            return null;
        }


        public wbSelectionIssueGrade SetToBussinessObjectforGridGrade(wbSelectionIssueGrade Entity)
        {
            wbSelectionIssueGrade Model = new wbSelectionIssueGrade();

            Model.WBSelectionGradeID = Entity.WBSelectionGradeID;
            Model.WBSIssueItemID = Entity.WBSIssueItemID;
            Model.WBSelectItemNo = Entity.WBSelectItemNo;
            Model.WBSIssueGradeID = Entity.WBSIssueGradeID;
            Model.WBSelectItemNo = Entity.WBSelectItemNo;
            Model.GradeID = Entity.GradeID;
            Model.GradeName = Entity.GradeID == null
                ? ""
                : _context.Sys_Grade.Where(m => m.GradeID == Entity.GradeID).FirstOrDefault().GradeName;
            Model.SelectedGradeQty = Entity.SelectedGradeQty == null ? 0 : Entity.SelectedGradeQty;
            Model.SelectedGradeSide = Entity.SelectedGradeSide == null ? 0 : Entity.SelectedGradeSide;
            Model.SelectedGradeArea = Entity.SelectedGradeArea == null ? 0 : Entity.SelectedGradeArea;
            Model.SelectedAreaUnitID = Entity.SelectedAreaUnitID == null ? 0 : Entity.SelectedAreaUnitID;
            Model.UnitName = Entity.AreaUnitID == null
                ? ""
                : _context.Sys_Unit.Where(m => m.UnitID == Entity.SelectedAreaUnitID).FirstOrDefault().UnitName;
            Model.SelectedUnitName = Entity.SelectedAreaUnitID == null
                ? ""
                : _context.Sys_Unit.Where(m => m.UnitID == Entity.SelectedAreaUnitID).FirstOrDefault().UnitName;
            Model.IssueUnitName = Entity.AreaUnitID == null
                ? ""
                : _context.Sys_Unit.Where(m => m.UnitID == Entity.AreaUnitID).FirstOrDefault().UnitName;



            return Model;
        }


        #endregion

        //#####################################################  SAVE WET BLUE DATA ################################################

        #region SAVE WET BLUE DATA

        public ValidationMsg Save(wbSelectionIssue model, int userid, string pageUrl)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        model.IssueNo = DalCommon.GetPreDefineNextCodeByUrl(pageUrl);
                        //DalCommon.GetPreDefineValue("1", "00045");
                        if (model.IssueNo != null)
                        {
                            #region WetBlueSelectionIssue

                            //model.IssueNo =
                            //    DalCommon.GetPreDefineNextCodeByUrl("WetBlueProductionSchedule/WetBlueProductionSchedule");
                            PRD_WBSellectionIssue tblWBSellectionIssue = SetToModelObject(model, userid);
                            _context.PRD_WBSellectionIssue.Add(tblWBSellectionIssue);
                            _context.SaveChanges();

                            #endregion

                            #region WetBlueSelectionIssueItem

                            model.IssueID = tblWBSellectionIssue.IssueID;

                            model.WBSelectItemNo =
                                DalCommon.GetPreDefineNextCodeByUrl("WBIStoreSelection/WBIStoreSelection");
                            PRD_WBSellectionIssueItem tblWBSellectionIssueItem = SetToIssueItemModelObject(model, userid);
                            _context.PRD_WBSellectionIssueItem.Add(tblWBSellectionIssueItem);
                            _context.SaveChanges();

                            #endregion

                            #region WetBlueSelectionIssueGrade

                            if (model.wbSelectionIssueGradeList != null)
                            {
                                foreach (
                                    wbSelectionIssueGrade objwbSelectionIssueGrade in model.wbSelectionIssueGradeList)
                                {
                                    objwbSelectionIssueGrade.WBSIssueGradeNo =
                                        DalCommon.GetPreDefineNextCodeByUrl("WBIStoreSelection/WBIStoreSelection");
                                    objwbSelectionIssueGrade.WBSIssueItemID = tblWBSellectionIssueItem.WBSIssueItemID;
                                    objwbSelectionIssueGrade.WBSelectItemNo = tblWBSellectionIssueItem.WBSelectItemNo;

                                    PRD_WBSellectionIssueGrade tblWBSellectionIssueGrade =
                                        SetToModelObject(objwbSelectionIssueGrade, userid);
                                    _context.PRD_WBSellectionIssueGrade.Add(tblWBSellectionIssueGrade);
                                    _context.SaveChanges();
                                }
                            }
                            _context.SaveChanges();

                            #endregion

                            tx.Complete();

                            IssueID = tblWBSellectionIssue.IssueID;
                            IssueNo = model.IssueNo;
                            WBSIssueItemID = tblWBSellectionIssueItem.WBSIssueItemID;


        #endregion

                            _vmMsg.Type = Enums.MessageType.Success;

                            _vmMsg.Msg = "Saved Successfully.";
                        }
                        else
                        {
                            _vmMsg.Type = Enums.MessageType.Error;
                            _vmMsg.Msg = "ScheduleNo Predefine Value not Found.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "IssueNo Data Already Exit.";
                }
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to save.";
            }
            return _vmMsg;
        }


        public PRD_WBSellectionIssue SetToModelObject(wbSelectionIssue model, int userid)
        {
            PRD_WBSellectionIssue Entity = new PRD_WBSellectionIssue();

            Entity.IssueID = model.IssueID;
            Entity.IssueNo = model.IssueNo;
            Entity.IssueDate = DalCommon.SetDate(model.IssueDate);
            Entity.IssueCategory = "AP";
            Entity.IssueType = model.IssueType;
            Entity.IssueFrom = model.IssueFrom;
            Entity.IssueTo = model.IssueTo;
            Entity.CheckNote = model.CheckNote;
            Entity.RecordStatus = "NCF";
            Entity.SetOn = DateTime.Now;
            //Entity.SetBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }

        public PRD_WBSellectionIssueItem SetToIssueItemModelObject(wbSelectionIssue model, int userid)
        {
            PRD_WBSellectionIssueItem Entity = new PRD_WBSellectionIssueItem();

            Entity.WBSIssueItemID = model.WBSIssueItemID;
            Entity.IssueID = model.IssueID;
            Entity.IssueNo = model.IssueNo;
            Entity.WBSelectItemID = model.WBSelectItemID;
            Entity.WBSelectItemNo = model.WBSelectItemNo;
            Entity.WBSelectionID = model.WBSelectionID;
            Entity.SupplierID = model.SupplierID;
            Entity.PurchaseID = model.PurchaseID;
            Entity.ItemTypeID = model.ItemTypeID;
            Entity.LeatherTypeID = model.LeatherTypeID;
            Entity.LeatherStatusID = model.LeatherStatusID;
            Entity.ProductionPcs = model.ProductionPcs == null ? 0 : model.ProductionPcs;
            Entity.ProductionSide = model.ProductionSide == null ? 0 : model.ProductionSide;
            Entity.ProductionArea = model.ProductionArea == null ? 0 : model.ProductionArea;
            Entity.ProductionAreaUnit = model.ProductionAreaUnit;
            //Entity.RecordStatus = "NCF";
            Entity.SetOn = DateTime.Now;
            //Entity.SetBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }


        public PRD_WBSellectionIssueGrade SetToModelObject(wbSelectionIssueGrade model, int userid)
        {
            PRD_WBSellectionIssueGrade Entity = new PRD_WBSellectionIssueGrade();

            Entity.WBSIssueGradeID = model.WBSIssueGradeID;
            Entity.WBSIssueGradeNo = model.WBSIssueGradeNo;
            Entity.WBSelectionGradeID = model.WBSelectionGradeID;
            Entity.WBSIssueItemID = model.WBSIssueItemID;
            Entity.WBSelectItemNo = model.WBSelectItemNo;
            Entity.GradeID = model.GradeID;
            Entity.GradeQty = model.GradeQty == null ? 0 : model.GradeQty;
            Entity.GradeSide = model.GradeSide == null ? 0 : model.GradeSide;
            Entity.GradeArea = model.GradeArea == null ? 0 : model.GradeArea;
            Entity.AreaUnitID = model.AreaUnitID == null ? 0 : model.AreaUnitID;
            Entity.SizeQtyRef = DalCommon.ConvertLeatherSizeTextToValue(model.SizeQtyRef == null ? "" : model.SizeQtyRef);//model.SizeQtyRef == null ? "" : model.SizeQtyRef;
            Entity.Remarks = model.Remarks;
            Entity.SetOn = DateTime.Now;
            //Entity.SetBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }




        //#####################################################  UPDATE WET BLUE DATA ##############################################

        #region UPDATE WET BLUE DATA

        public ValidationMsg Update(wbSelectionIssue model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                #region Update

                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        #region Wet Blue Sellection Issue

                        PRD_WBSellectionIssue IssueEntity = SetToModelObject(model, userid);
                        var OriginalEntity = _context.PRD_WBSellectionIssue.First(m => m.IssueID == model.IssueID);

                        OriginalEntity.IssueID = IssueEntity.IssueID;
                        // OriginalEntity.IssueNo = IssueEntity.IssueNo;
                        OriginalEntity.IssueDate = IssueEntity.IssueDate;
                        OriginalEntity.IssueFrom = IssueEntity.IssueFrom;
                        OriginalEntity.IssueTo = IssueEntity.IssueTo;
                        OriginalEntity.IssueCategory = IssueEntity.IssueCategory;
                        OriginalEntity.IssueType = IssueEntity.IssueType;
                        OriginalEntity.RecordStatus = IssueEntity.RecordStatus;
                        OriginalEntity.CheckNote = IssueEntity.CheckNote;


                        OriginalEntity.ModifiedBy = userid;
                        OriginalEntity.ModifiedOn = DateTime.Now;
                        _context.SaveChanges();

                        #endregion

                        #region Wet Blue Sellection Issue Item

                        PRD_WBSellectionIssueItem CurrentIssueItemEntity = SetToIssueItemModelObject(model, userid);
                        var OriginalIssueItemEntity =
                            _context.PRD_WBSellectionIssueItem.First(m => m.WBSIssueItemID == model.WBSIssueItemID);

                        //OriginalIssueItemEntity.WBSIssueItemID = CurrentIssueItemEntity.WBSIssueItemID;
                        OriginalIssueItemEntity.IssueID = CurrentIssueItemEntity.IssueID;
                        OriginalIssueItemEntity.IssueNo = CurrentIssueItemEntity.IssueNo;
                        //OriginalIssueItemEntity.WBSelectItemID = CurrentIssueItemEntity.WBSelectItemID;
                        //OriginalIssueItemEntity.WBSelectItemNo = CurrentIssueItemEntity.WBSelectItemNo;
                        //OriginalIssueItemEntity.WBSelectionID = CurrentIssueItemEntity.WBSelectionID;
                        OriginalIssueItemEntity.SupplierID = CurrentIssueItemEntity.SupplierID;
                        OriginalIssueItemEntity.PurchaseID = CurrentIssueItemEntity.PurchaseID;
                        OriginalIssueItemEntity.ItemTypeID = CurrentIssueItemEntity.ItemTypeID;
                        OriginalIssueItemEntity.LeatherStatusID = CurrentIssueItemEntity.LeatherStatusID;
                        OriginalIssueItemEntity.ProductionPcs = CurrentIssueItemEntity.ProductionPcs == null
                            ? 0
                            : CurrentIssueItemEntity.ProductionPcs;
                        OriginalIssueItemEntity.ProductionSide = CurrentIssueItemEntity.ProductionSide == null
                            ? 0
                            : CurrentIssueItemEntity.ProductionSide;
                        OriginalIssueItemEntity.ProductionArea = CurrentIssueItemEntity.ProductionArea == null
                            ? 0
                            : CurrentIssueItemEntity.ProductionArea;
                        OriginalIssueItemEntity.ProductionAreaUnit = CurrentIssueItemEntity.ProductionAreaUnit;
                        OriginalIssueItemEntity.LotNo = CurrentIssueItemEntity.LotNo;
                        OriginalIssueItemEntity.SetOn = CurrentIssueItemEntity.SetOn;
                        OriginalIssueItemEntity.SetBy = CurrentIssueItemEntity.SetBy;

                        OriginalIssueItemEntity.ModifiedBy = userid;
                        OriginalIssueItemEntity.ModifiedOn = DateTime.Now;
                        _context.SaveChanges();

                        #endregion

                        #region Save Wet Blue Selection Issue Grade List

                        if (model.wbSelectionIssueGradeList != null)
                        {
                            foreach (wbSelectionIssueGrade objwbSelectionIssueGrade in model.wbSelectionIssueGradeList)
                            {

                                PRD_WBSellectionIssueGrade CurrIssueGradeEntity =
                                    SetToModelObject(objwbSelectionIssueGrade, userid);
                                var OrgrEntity =
                                    _context.PRD_WBSellectionIssueGrade.First(
                                        m => m.WBSIssueGradeID == objwbSelectionIssueGrade.WBSIssueGradeID);

                                //OrgrEntity.ScheduleID = CurrEntity.ScheduleID;
                                //OrgrEntity.ProductionNo = CurrEntity.ProductionNo;
                                OrgrEntity.WBSIssueGradeID = CurrIssueGradeEntity.WBSIssueGradeID;
                                //OrgrEntity.WBSIssueGradeNo = CurrIssueGradeEntity.WBSIssueGradeNo;
                                //OrgrEntity.WBSIssueItemID = CurrIssueGradeEntity.WBSIssueItemID;
                                OrgrEntity.GradeID = CurrIssueGradeEntity.GradeID;
                                OrgrEntity.GradeQty = CurrIssueGradeEntity.GradeQty == null
                                    ? 0
                                    : CurrIssueGradeEntity.GradeQty;
                                OrgrEntity.GradeSide = CurrIssueGradeEntity.GradeSide == null
                                    ? 0
                                    : CurrIssueGradeEntity.GradeSide;
                                OrgrEntity.GradeArea = CurrIssueGradeEntity.GradeArea == null
                                    ? 0
                                    : CurrIssueGradeEntity.GradeArea;
                                OrgrEntity.AreaUnitID = CurrIssueGradeEntity.AreaUnitID == null
                                    ? 0
                                    : CurrIssueGradeEntity.AreaUnitID;
                                OrgrEntity.WBSelectionGradeID = CurrIssueGradeEntity.WBSelectionGradeID;
                                OrgrEntity.ModifiedBy = userid;
                                OrgrEntity.ModifiedOn = DateTime.Now;
                                OrgrEntity.IPAddress = GetIPAddress.LocalIPAddress();
                                OrgrEntity.SetOn = DateTime.Now;
                                OrgrEntity.SetBy = userid;

                            }
                        }

                        #endregion

                        _context.SaveChanges();
                        tx.Complete();

                        _vmMsg.Type = Enums.MessageType.Update;
                        _vmMsg.Msg = "Updated Successfully.";
                    }
                }

                #endregion
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Update.";
            }
            return _vmMsg;
        }


        public wbSelectionIssueGrade SetToBussinessObject(PRD_WBSelectionGrade Entity)
        {
            wbSelectionIssueGrade Model = new wbSelectionIssueGrade();

            Model.WBSelectionGradeID = Entity.WBSelectionGradeID;
            //Model.WBSIssueItemID = Entity.WBSIssueItemID;
            Model.WBSelectItemNo = Entity.WBSelectItemNo;
            Model.WBSelectionGradeID = Entity.WBSelectionGradeID;
            //Model.WBSelectItemID = Entity.WBSelectItemID;
            Model.WBSelectItemNo = Entity.WBSelectItemNo;
            Model.GradeID = Entity.GradeID;
            Model.GradeName = Entity.GradeID == null
                ? ""
                : _context.Sys_Grade.Where(m => m.GradeID == Entity.GradeID).FirstOrDefault().GradeName;
            Model.SelectedGradeQty = Entity.GradeQty == null ? 0 : Entity.GradeQty;
            Model.SelectedGradeSide = Entity.GradeSide == null ? 0 : Entity.GradeSide;
            Model.SelectedGradeArea = Entity.GradeArea == null ? 0 : Entity.GradeArea;
            Model.SelectedAreaUnitID = Entity.AreaUnitID == null ? 0 : Entity.AreaUnitID;
            Model.UnitName = Entity.AreaUnitID == null
                ? ""
                : _context.Sys_Unit.Where(m => m.UnitID == Entity.AreaUnitID).FirstOrDefault().UnitName;

            Model.GradeQty = Entity.GradeQty == null ? 0 : Entity.GradeQty;
            Model.GradeSide = Entity.GradeSide == null ? 0 : Entity.GradeSide;
            Model.GradeArea = Entity.GradeArea == null ? 0 : Entity.GradeArea;
            Model.AreaUnitID = Entity.AreaUnitID == null ? 0 : Entity.AreaUnitID;
            Model.IssueUnitName = Entity.AreaUnitID == null
                ? ""
                : _context.Sys_Unit.Where(m => m.UnitID == Entity.AreaUnitID).FirstOrDefault().UnitName;

            return Model;
        }

        #endregion

        //#####################################################  SEARCH WET BLUE DATA ##############################################

        #region SEARCH WET BLUE SELECTION DATA

        public List<wbSelectionIssue> SearchMasterInfo()
        {
            var query = @"SELECT		wbsi.IssueID, 
			                            wbsi.WBSIssueItemID,
			                            ISNULL(s.WBSelectionID,0) WBSelectionID,
			                            ISNULL(s.WBSelectionNo,'') WBSelectionNo,
			                            ISNULL(CONVERT(VARCHAR(12),
			                            s.SelectionDate, 106),'')SelectionDate,
			                            S.SelectedBy,(ISNULL(UR.FirstName,'') + ' '+ ISNULL(UR.MiddleName,'') +' '+ ISNULL(UR.LastName,'')) AS SelectedByName,
			                            ISNULL(wbs.IssueNo,'')IssueNo,
                                        CONVERT(VARCHAR(12),wbs.IssueDate, 106) IssueDate,
			                            ISNULL(wbs.IssueFrom,'') IssueFrom,
			                            ISNULL(wbs.IssueTo,'') IssueTo,
			                            IssueCategory = 'After Production',
										CASE  wbs.RecordStatus WHEN 'NCF' THEN 'Not Confirm'
                                        WHEN 'CNF' THEN 'Confirmed'
                                        WHEN 'CHK' THEN 'Checked'
                                        WHEN 'APV' THEN 'Approved'
                                        END RecordStatus,
										ISNULL(s.RecordState,'') RecordState,
			                            ISNULL(wbsi.ItemTypeID,0) ItemTypeID,
			                            ISNULL(i.ItemTypeName,'') ItemTypeName,
			                            ISNULL(wbsi.LeatherStatusID,0) LeatherStatusID,
			                            ISNULL(l.LeatherStatusName,'') LeatherStatusName,
			                            ISNULL(wbsi.SupplierID,0) SupplierID,
			                            ISNULL(su.SupplierName,'') SupplierName,
			                            ISNULL(wbsi.PurchaseID,0) PurchaseID,
			                            ISNULL(p.PurchaseNo,'') PurchaseNo,
			                            ISNULL(wbsi.ProductionPcs,0) ProductionPcs,
			                            ISNULL(wbsi.ProductionSide,0) ProductionSide,
			                            ISNULL(wbsi.ProductionArea,0) ProductionArea,
			                            ISNULL(wbsi.ProductionAreaUnit,0) ProductionAreaUnit,
			                            ISNULL(st.StoreName,'') IssueFromName, 
			                            ISNULL(st2.StoreName,'') IssueToName,
                                        ISNULL(u.UnitName,'') UnitName

                        FROM PRD_WBSellectionIssue wbs
                        LEFT JOIN PRD_WBSellectionIssueItem wbsi ON wbs.IssueID = wbsi.IssueID
                        INNER JOIN PRD_WBSelection s ON wbsi.WBSelectionID = s.WBSelectionID
                        LEFT JOIN SYS_Store st ON wbs.IssueFrom = st.StoreID
                        LEFT JOIN SYS_Store st2 ON wbs.IssueTo = st2.StoreID
                        LEFT JOIN Sys_ItemType i ON wbsi.ItemTypeID = i.ItemTypeID
                        LEFT JOIN Sys_LeatherStatus l ON wbsi.LeatherStatusID = l.LeatherStatusID
                        LEFT JOIN Sys_Supplier su ON wbsi.SupplierID = su.SupplierID
                        LEFT JOIN Prq_Purchase p ON wbsi.PurchaseID = p.PurchaseID
                        LEFT JOIN Sys_Unit u ON wbsi.ProductionAreaUnit = u.UnitID
						INNER JOIN [Security].Users UR ON UR.UserID=S.SelectedBy
                        ORDER BY wbsi.IssueID DESC";

            var result = _context.Database.SqlQuery<wbSelectionIssue>(query).ToList();
            return result;

        }

        public List<wbSelectionIssueGrade> SearchIssueInfo(string WBSIssueItemID, string IssueFrom, string SupplierID,
            string PurchaseID, string ItemTypeID, string LeatherStatusID)
        {
            var query =
                        @" SELECT    A.WBSIssueGradeID,
                                     CASE    A.SizeQtyRef 
				                                WHEN 'SizeQty1' THEN '12-15 sft'
				                                WHEN 'SizeQty2'	THEN '16-20 sft'
				                                WHEN 'SizeQty3' THEN '21-25 sft'
				                                WHEN 'SizeQty4' THEN '26-30 sft'
				                                WHEN 'SizeQty5' THEN '31-35 sft'
				                                WHEN 'SideQty' THEN 'Side'
				                                WHEN 'AreaQty' THEN 'Area'
		                             END     SizeQtyRef,
                                     G.GradeID,
                                     G.GradeName,
                                     ISNULL(B.ClosingStockPcs,0) SelectedGradeQty, 
                                     ISNULL(B.ClosingStockSide,0) SelectedGradeSide, 
                                     ISNULL(B.ClosingStockArea,0) SelectedGradeArea, 
                                     B.ClosingStockAreaUnit SelectedAreaUnitID,
                                     SU.UnitName SelectedUnitName,
                                     A.WBSelectionGradeID,
                                     A.WBSIssueItemID,
                                     A.WBSelectItemNo,
                                     ISNULL(A.GradeQty,0)GradeQty,
                                     ISNULL(A.GradeSide,0)GradeSide,
                                     ISNULL(A.GradeArea,0)GradeArea,
                                     A.AreaUnitID,
                                     A.IssueUnitName
                                FROM    (SELECT   wbs.IssueID,
                                         wbs.IssueFrom, 
                                         WII.WBSIssueItemID,
                                         WII.WBSelectItemNo,
                                         WII.SupplierID,
                                         WII.PurchaseID,
                                         WII.ItemTypeID,
                                         WII.LeatherStatusID,
                                         IG.WBSelectionGradeID, 
                                         IG.WBSIssueGradeID,
                                         IG.GradeID,
                                         IG.SizeQtyRef,
                                         IG.GradeQty,
                                         IG.GradeSide,
                                         IG.GradeArea,
                                         IG.AreaUnitID,
                                         IU.UnitName  IssueUnitName
                                     FROM   PRD_WBSellectionIssue wbs
                                     INNER JOIN  PRD_WBSellectionIssueItem WII ON  WII.IssueID=wbs.IssueID
                                     INNER JOIN  PRD_WBSellectionIssueGrade IG ON IG.WBSIssueItemID=WII.WBSIssueItemID
                                     INNER JOIN  Sys_Unit IU ON IU.UnitID=IG.AreaUnitID
                                                    WHERE   WII.WBSIssueItemID = '" + WBSIssueItemID + "') AS A INNER JOIN  Sys_Grade G ON G.GradeID=A.GradeID INNER JOIN  (SELECT inv.GradeID,inv.StoreID,inv.SupplierID,inv.PurchaseID,inv.ItemTypeID,inv.LeatherStatusID,inv.ClosingStockPcs,inv.ClosingStockSide,inv.ClosingStockArea,inv.ClosingStockAreaUnit, inv.SizeQtyRef  FROM INV_WetBlueSelectionStock inv INNER JOIN  (SELECT MAX(TransectionID)TransectionID, SupplierID,StoreID,ItemTypeID, LeatherStatusID, GradeID ,SizeQtyRef FROM INV_WetBlueSelectionStock  GROUP BY StoreID,SupplierID,PurchaseID,ItemTypeID,LeatherStatusID,GradeID,SizeQtyRef) sup  ON    inv.TransectionID=sup.TransectionID ) AS B ON    A.IssueFrom=B.StoreID AND  A.SupplierID=B.SupplierID AND A.PurchaseID=B.PurchaseID AND  A.ItemTypeID=B.ItemTypeID AND A.LeatherStatusID=B.LeatherStatusID AND A.GradeID=B.GradeID AND A.SizeQtyRef=B.SizeQtyRef INNER JOIN  Sys_Unit SU ON SU.UnitID=B.ClosingStockAreaUnit ORDER BY G.GradeName";

                var result = _context.Database.SqlQuery<wbSelectionIssueGrade>(query).ToList();
                return result;
            
        }

        #endregion


        //#####################################################  Confirm Stock Update WET BLUE DATA ###############################


        #region Confirm Stock Data with Stock Update

        //public ValidationMsg ConfirmWetBlueSelectionStock(wbSelectionIssue model, int userid, bool isSelectionLock, long wbSelectionID)
        //{
        //    _vmMsg = new ValidationMsg();

        //    try
        //    {
        //        using (var tx = new TransactionScope())
        //        {
        //            using (_context)
        //            {
        //                if (isSelectionLock)
        //                {
        //                    var wbSelection = _context.PRD_WBSelection.First(m => m.WBSelectionID == wbSelectionID);
        //                    wbSelection.RecordState = "ITS";
        //                    wbSelection.ModifiedBy = userid;
        //                    wbSelection.ModifiedOn = DateTime.Now;
        //                }

        //                if (model.wbSelectionIssueGradeList.Count > 0)
        //                {
        //                    foreach (var objwbSelectionIssueGrade in model.wbSelectionIssueGradeList)
        //                    {
        //                        #region InvWetBlueSelectionStockUpdate

        //                        var CheckSupplierSelectionStock = (from i in _context.INV_WetBlueSelectionStock.AsEnumerable()
        //                                                           where i.StoreID == model.IssueFrom
        //                                                           && i.SupplierID == model.SupplierID
        //                                                           && i.PurchaseID == model.PurchaseID
        //                                                           && i.ItemTypeID == model.ItemTypeID
        //                                                           && i.LeatherStatusID == model.LeatherStatusID
        //                                                           && i.GradeID == objwbSelectionIssueGrade.GradeID
        //                                                           select i).Any();

        //                        if (CheckSupplierSelectionStock)
        //                        {
        //                            var LastSelectionStock = (from i in _context.INV_WetBlueSelectionStock.AsEnumerable()
        //                                                      where i.StoreID == model.IssueFrom
        //                                                            && i.SupplierID == model.SupplierID
        //                                                            && i.PurchaseID == model.PurchaseID
        //                                                            && i.ItemTypeID == model.ItemTypeID
        //                                                            && i.LeatherStatusID == model.LeatherStatusID
        //                                                            && i.GradeID == objwbSelectionIssueGrade.GradeID
        //                                                      orderby i.TransectionID descending
        //                                                      select i).FirstOrDefault();

        //                            if ((LastSelectionStock.ClosingStockPcs >= objwbSelectionIssueGrade.GradeQty) && (LastSelectionStock.ClosingStockSide >= objwbSelectionIssueGrade.GradeSide) && (LastSelectionStock.ClosingStockArea >= objwbSelectionIssueGrade.GradeArea))
        //                            {
        //                                var tblSelectionStock = new INV_WetBlueSelectionStock();

        //                                tblSelectionStock.StoreID = model.IssueFrom;
        //                                tblSelectionStock.SupplierID = model.SupplierID;
        //                                tblSelectionStock.PurchaseID = model.PurchaseID;
        //                                tblSelectionStock.ItemTypeID = model.ItemTypeID;
        //                                tblSelectionStock.LeatherTypeID = 1;//Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Wet Blue").FirstOrDefault().LeatherTypeID);
        //                                tblSelectionStock.LeatherStatusID = model.LeatherStatusID;
        //                                tblSelectionStock.GradeID = objwbSelectionIssueGrade.GradeID;

        //                                tblSelectionStock.ClosingStockAreaUnit = objwbSelectionIssueGrade.AreaUnitID;

        //                                tblSelectionStock.OpeningStockPcs = LastSelectionStock.ClosingStockPcs ?? 0;
        //                                tblSelectionStock.OpeningStockSide = LastSelectionStock.ClosingStockSide ?? 0;
        //                                tblSelectionStock.OpeningStockArea = LastSelectionStock.ClosingStockArea ?? 0;

        //                                tblSelectionStock.ReceiveStockPcs = 0;
        //                                tblSelectionStock.ReceiveStockSide = 0;
        //                                tblSelectionStock.ReceiveStockArea = 0;

        //                                tblSelectionStock.IssueStockPcs = objwbSelectionIssueGrade.GradeQty ?? 0;
        //                                tblSelectionStock.IssueStockSide = objwbSelectionIssueGrade.GradeSide ?? 0;
        //                                tblSelectionStock.IssueStockArea = objwbSelectionIssueGrade.GradeArea ?? 0;

        //                                tblSelectionStock.ClosingStockPcs = (LastSelectionStock.ClosingStockPcs ?? 0) - (objwbSelectionIssueGrade.GradeQty ?? 0);
        //                                tblSelectionStock.ClosingStockSide = (LastSelectionStock.ClosingStockSide ?? 0) - (objwbSelectionIssueGrade.GradeSide ?? 0);
        //                                tblSelectionStock.ClosingStockArea = (LastSelectionStock.ClosingStockArea ?? 0) - (objwbSelectionIssueGrade.GradeArea ?? 0);
        //                                tblSelectionStock.SetBy = userid;
        //                                tblSelectionStock.SetOn = DateTime.Now;
        //                                _context.INV_WetBlueSelectionStock.Add(tblSelectionStock);
        //                                _context.SaveChanges();

        //                            }
        //                            else
        //                            {
        //                                stockOver = 1;
        //                                break;
        //                            }
        //                        #endregion

        //                            #region InvWetBlueDailyStockUpdate

        //                            var currentDate = DateTime.Now.Date;
        //                            var CheckDate = (from ds in _context.INV_WetBlueStockDaily.AsEnumerable()
        //                                             where ds.StockDate == currentDate
        //                                             && ds.StoreID == model.IssueTo
        //                                             && ds.ItemTypeID == model.ItemTypeID
        //                                             && ds.LeatherStatusID == model.LeatherStatusID
        //                                             && ds.GradeID == objwbSelectionIssueGrade.GradeID
        //                                             select ds).Any();

        //                            if (CheckDate)
        //                            {
        //                                var CurrentItem = (from ds in _context.INV_WetBlueStockDaily.AsEnumerable()
        //                                                   where ds.StockDate == currentDate
        //                                                    && ds.StoreID == model.IssueTo
        //                                                    && ds.ItemTypeID == model.ItemTypeID
        //                                                    && ds.GradeID == objwbSelectionIssueGrade.GradeID
        //                                                    && ds.LeatherStatusID == model.LeatherStatusID
        //                                                   select ds).FirstOrDefault();

        //                                CurrentItem.ReceiveStockPcs = (CurrentItem.ReceiveStockPcs ?? 0) + (objwbSelectionIssueGrade.GradeQty ?? 0);
        //                                CurrentItem.ReceiveStockSide = (CurrentItem.ReceiveStockSide ?? 0) + (objwbSelectionIssueGrade.GradeSide ?? 0);
        //                                CurrentItem.ReceiveStockArea = (CurrentItem.ReceiveStockArea ?? 0) + (objwbSelectionIssueGrade.GradeArea ?? 0);

        //                                CurrentItem.ClosingStockPcs = (CurrentItem.ClosingStockPcs ?? 0) + (objwbSelectionIssueGrade.GradeQty ?? 0);
        //                                CurrentItem.ClosingStockSide = (CurrentItem.ClosingStockSide ?? 0) + (objwbSelectionIssueGrade.GradeSide ?? 0);
        //                                CurrentItem.ClosingStockArea = (CurrentItem.ClosingStockArea ?? 0) + (objwbSelectionIssueGrade.GradeArea ?? 0);

        //                                _context.SaveChanges();
        //                            }
        //                            else
        //                            {
        //                                var PreviousRecord = (from ds in _context.INV_WetBlueStockDaily.AsEnumerable()
        //                                                      where ds.StoreID == model.IssueTo
        //                                                        && ds.ItemTypeID == model.ItemTypeID
        //                                                        && ds.GradeID == objwbSelectionIssueGrade.GradeID
        //                                                        && ds.LeatherStatusID == model.LeatherStatusID
        //                                                      orderby ds.StockDate
        //                                                      select ds).LastOrDefault();

        //                                INV_WetBlueStockDaily objStockDaily = new INV_WetBlueStockDaily();

        //                                objStockDaily.StockDate = currentDate;

        //                                objStockDaily.StoreID = model.IssueTo;
        //                                objStockDaily.ItemTypeID = model.ItemTypeID;
        //                                objStockDaily.LeatherTypeID = 1;//Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Wet Blue").FirstOrDefault().LeatherTypeID);
        //                                objStockDaily.LeatherStatusID = model.LeatherStatusID;
        //                                objStockDaily.GradeID = objwbSelectionIssueGrade.GradeID;
        //                                objStockDaily.ClosingStockArea = objwbSelectionIssueGrade.AreaUnitID;

        //                                objStockDaily.OpeningStockPcs = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockPcs);
        //                                objStockDaily.OpeningStockSide = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockSide);
        //                                objStockDaily.OpeningStockArea = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockArea);

        //                                objStockDaily.ReceiveStockPcs = (objwbSelectionIssueGrade.GradeQty ?? 0);
        //                                objStockDaily.ReceiveStockSide = (objwbSelectionIssueGrade.GradeSide ?? 0);
        //                                objStockDaily.ReceiveStockArea = (objwbSelectionIssueGrade.GradeArea ?? 0);

        //                                objStockDaily.IssueStockPcs = 0;
        //                                objStockDaily.IssueStockSide = 0;
        //                                objStockDaily.IssueStockArea = 0;

        //                                objStockDaily.ClosingStockPcs = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockPcs) + (objwbSelectionIssueGrade.GradeQty ?? 0);
        //                                objStockDaily.ClosingStockSide = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockSide) + (objwbSelectionIssueGrade.GradeSide ?? 0);
        //                                objStockDaily.ClosingStockArea = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockArea) + (objwbSelectionIssueGrade.GradeArea ?? 0);

        //                                _context.INV_WetBlueStockDaily.Add(objStockDaily);
        //                                //_context.SaveChanges();
        //                            }

        //                            #endregion

        //                            #region InvWetBlueStockItemUpdate

        //                            var CheckItemStock = (from ds in _context.INV_WetBlueStockItem.AsEnumerable()
        //                                                  where ds.StoreID == model.IssueTo
        //                                                        && ds.ItemTypeID == model.ItemTypeID
        //                                                        && ds.GradeID == objwbSelectionIssueGrade.GradeID
        //                                                        && ds.LeatherStatusID == model.LeatherStatusID
        //                                                  select ds).Any();


        //                            if (!CheckItemStock)
        //                            {
        //                                INV_WetBlueStockItem objStockItem = new INV_WetBlueStockItem();

        //                                objStockItem.StoreID = model.IssueTo;
        //                                objStockItem.ItemTypeID = model.ItemTypeID;
        //                                objStockItem.LeatherTypeID = 1;//Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Wet Blue").FirstOrDefault().LeatherTypeID);
        //                                objStockItem.LeatherStatusID = model.LeatherStatusID;
        //                                objStockItem.GradeID = objwbSelectionIssueGrade.GradeID;
        //                                objStockItem.AreaUnit = objwbSelectionIssueGrade.AreaUnitID;

        //                                objStockItem.OpeningStockPcs = 0;
        //                                objStockItem.OpeningStockSide = 0;
        //                                objStockItem.OpeningStockArea = 0;

        //                                objStockItem.ReceiveStockPcs = (objwbSelectionIssueGrade.GradeQty ?? 0);
        //                                objStockItem.ReceiveStockSide = (objwbSelectionIssueGrade.GradeSide ?? 0);
        //                                objStockItem.ReceiveStockArea = (objwbSelectionIssueGrade.GradeArea ?? 0);

        //                                objStockItem.IssueStockPcs = 0;
        //                                objStockItem.IssueStockSide = 0;
        //                                objStockItem.IssueStockArea = 0;

        //                                objStockItem.ClosingStockPcs = (objwbSelectionIssueGrade.GradeQty ?? 0);
        //                                objStockItem.ClosingStockSide = (objwbSelectionIssueGrade.GradeSide ?? 0);
        //                                objStockItem.ClosingStockArea = (objwbSelectionIssueGrade.GradeArea ?? 0);
        //                                objStockItem.SetBy = userid;
        //                                objStockItem.SetOn = DateTime.Now;

        //                                _context.INV_WetBlueStockItem.Add(objStockItem);
        //                                _context.SaveChanges();
        //                            }
        //                            else
        //                            {
        //                                var LastItemInfo = (from ds in _context.INV_WetBlueStockItem.AsEnumerable()
        //                                                    where ds.StoreID == model.IssueTo
        //                                                        && ds.ItemTypeID == model.ItemTypeID
        //                                                        && ds.GradeID == objwbSelectionIssueGrade.GradeID
        //                                                        && ds.LeatherStatusID == model.LeatherStatusID
        //                                                    orderby ds.TransectionID descending
        //                                                    select ds).FirstOrDefault();

        //                                INV_WetBlueStockItem objStockItem = new INV_WetBlueStockItem();

        //                                objStockItem.StoreID = model.IssueTo;
        //                                objStockItem.ItemTypeID = model.ItemTypeID;
        //                                objStockItem.LeatherTypeID = 1;//Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Wet Blue").FirstOrDefault().LeatherTypeID);
        //                                objStockItem.LeatherStatusID = model.LeatherStatusID;
        //                                objStockItem.GradeID = objwbSelectionIssueGrade.GradeID;
        //                                objStockItem.AreaUnit = objwbSelectionIssueGrade.AreaUnitID;

        //                                objStockItem.OpeningStockPcs = (LastItemInfo.ClosingStockPcs ?? 0);
        //                                objStockItem.OpeningStockSide = (LastItemInfo.ClosingStockSide ?? 0);
        //                                objStockItem.OpeningStockArea = (LastItemInfo.ClosingStockArea ?? 0);

        //                                objStockItem.IssueStockPcs = 0;
        //                                objStockItem.IssueStockSide = 0;
        //                                objStockItem.IssueStockArea = 0;

        //                                objStockItem.ReceiveStockPcs = (objwbSelectionIssueGrade.GradeQty ?? 0);
        //                                objStockItem.ReceiveStockSide = (objwbSelectionIssueGrade.GradeSide ?? 0);
        //                                objStockItem.ReceiveStockArea = (objwbSelectionIssueGrade.GradeArea ?? 0);

        //                                objStockItem.ClosingStockPcs = (LastItemInfo == null ? 0 : LastItemInfo.ClosingStockPcs) + (objwbSelectionIssueGrade.GradeQty ?? 0);
        //                                objStockItem.ClosingStockSide = (LastItemInfo == null ? 0 : LastItemInfo.ClosingStockSide) + (objwbSelectionIssueGrade.GradeSide ?? 0);
        //                                objStockItem.ClosingStockArea = (LastItemInfo == null ? 0 : LastItemInfo.ClosingStockArea) + (objwbSelectionIssueGrade.GradeArea ?? 0);
        //                                objStockItem.SetOn = DateTime.Now;
        //                                objStockItem.SetBy = userid;

        //                                _context.INV_WetBlueStockItem.Add(objStockItem);
        //                                //_context.SaveChanges();
        //                            }

        //                            #endregion

        //                            #region InvWetBlueStockSupplierUpdate

        //                            var CheckSupplierStock = (from ds in _context.INV_WetBlueStockSupplier.AsEnumerable()
        //                                                      where ds.SupplierID == model.SupplierID
        //                                                      && ds.PurchaseID == model.PurchaseID
        //                                                        && ds.StoreID == model.IssueTo
        //                                                        && ds.ItemTypeID == model.ItemTypeID
        //                                                        && ds.GradeID == objwbSelectionIssueGrade.GradeID
        //                                                        && ds.LeatherStatusID == model.LeatherStatusID
        //                                                      select ds).Any();

        //                            if (!CheckSupplierStock)
        //                            {
        //                                INV_WetBlueStockSupplier objStockSupplier = new INV_WetBlueStockSupplier();

        //                                objStockSupplier.StoreID = model.IssueTo;
        //                                objStockSupplier.SupplierID = model.SupplierID;
        //                                objStockSupplier.PurchaseID = model.PurchaseID;
        //                                objStockSupplier.ItemTypeID = model.ItemTypeID;
        //                                objStockSupplier.LeatherTypeID = 1;//Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Wet Blue").FirstOrDefault().LeatherTypeID);
        //                                objStockSupplier.LeatherStatusID = model.LeatherStatusID;
        //                                objStockSupplier.GradeID = objwbSelectionIssueGrade.GradeID;
        //                                objStockSupplier.AreaUnit = objwbSelectionIssueGrade.AreaUnitID;

        //                                objStockSupplier.OpeningStockPcs = 0;
        //                                objStockSupplier.OpeningStockSide = 0;
        //                                objStockSupplier.OpeningStockArea = 0;

        //                                objStockSupplier.ReceiveStockPcs = (objwbSelectionIssueGrade.GradeQty ?? 0);
        //                                objStockSupplier.ReceiveStockSide = (objwbSelectionIssueGrade.GradeSide ?? 0);
        //                                objStockSupplier.ReceiveStockArea = (objwbSelectionIssueGrade.GradeArea ?? 0);

        //                                objStockSupplier.IssueStockPcs = 0;
        //                                objStockSupplier.IssueStockSide = 0;
        //                                objStockSupplier.IssueStockArea = 0;

        //                                objStockSupplier.ClosingStockPcs = (objwbSelectionIssueGrade.GradeQty ?? 0);
        //                                objStockSupplier.ClosingStockSide = (objwbSelectionIssueGrade.GradeSide ?? 0);
        //                                objStockSupplier.ClosingStockArea = (objwbSelectionIssueGrade.GradeArea ?? 0);
        //                                objStockSupplier.SetOn = DateTime.Now;
        //                                objStockSupplier.SetBy = userid;



        //                                _context.INV_WetBlueStockSupplier.Add(objStockSupplier);
        //                                _context.SaveChanges();
        //                            }
        //                            else
        //                            {
        //                                var LastSupplierStock = (from ds in _context.INV_WetBlueStockSupplier.AsEnumerable()
        //                                                         where ds.SupplierID == model.SupplierID
        //                                                         && ds.PurchaseID == model.PurchaseID
        //                                                            && ds.StoreID == model.IssueTo
        //                                                            && ds.ItemTypeID == model.ItemTypeID
        //                                                            && ds.GradeID == objwbSelectionIssueGrade.GradeID
        //                                                            && ds.LeatherStatusID == model.LeatherStatusID
        //                                                         orderby ds.TransectionID descending
        //                                                         select ds).FirstOrDefault();

        //                                INV_WetBlueStockSupplier objStockSupplier = new INV_WetBlueStockSupplier();

        //                                objStockSupplier.StoreID = model.IssueTo;
        //                                objStockSupplier.SupplierID = model.SupplierID;
        //                                objStockSupplier.PurchaseID = model.PurchaseID;
        //                                objStockSupplier.ItemTypeID = model.ItemTypeID;
        //                                objStockSupplier.LeatherTypeID = 1;//Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Wet Blue").FirstOrDefault().LeatherTypeID);
        //                                objStockSupplier.LeatherStatusID = model.LeatherStatusID;
        //                                objStockSupplier.GradeID = objwbSelectionIssueGrade.GradeID;
        //                                objStockSupplier.AreaUnit = objwbSelectionIssueGrade.AreaUnitID;

        //                                objStockSupplier.OpeningStockPcs = (LastSupplierStock.ClosingStockPcs ?? 0);
        //                                objStockSupplier.OpeningStockSide = (LastSupplierStock.ClosingStockSide ?? 0);
        //                                objStockSupplier.OpeningStockArea = (LastSupplierStock.ClosingStockArea ?? 0);

        //                                objStockSupplier.ReceiveStockPcs = (objwbSelectionIssueGrade.GradeQty ?? 0);
        //                                objStockSupplier.ReceiveStockSide = (objwbSelectionIssueGrade.GradeSide ?? 0);
        //                                objStockSupplier.ReceiveStockArea = (objwbSelectionIssueGrade.GradeArea ?? 0);

        //                                objStockSupplier.IssueStockPcs = 0;
        //                                objStockSupplier.IssueStockSide = 0;
        //                                objStockSupplier.IssueStockArea = 0;

        //                                objStockSupplier.ClosingStockPcs = (LastSupplierStock == null ? 0 : LastSupplierStock.ClosingStockPcs) + (objwbSelectionIssueGrade.GradeQty ?? 0);
        //                                objStockSupplier.ClosingStockSide = (LastSupplierStock == null ? 0 : LastSupplierStock.ClosingStockSide) + (objwbSelectionIssueGrade.GradeSide ?? 0);
        //                                objStockSupplier.ClosingStockArea = (LastSupplierStock == null ? 0 : LastSupplierStock.ClosingStockArea) + (objwbSelectionIssueGrade.GradeArea ?? 0);
        //                                objStockSupplier.SetOn = DateTime.Now;
        //                                objStockSupplier.SetBy = userid;


        //                                _context.INV_WetBlueStockSupplier.Add(objStockSupplier);
        //                                //_context.SaveChanges();
        //                            }

        //                            #endregion

        //                            _context.SaveChanges();

        //                        }
        //                    }
        //                }

        //                if (stockOver == 0)
        //                {
        //                    var originalSelectionIssue = _context.PRD_WBSellectionIssue.First(m => m.IssueID == model.IssueID);
        //                    originalSelectionIssue.RecordStatus = "CNF";
        //                    originalSelectionIssue.ModifiedBy = userid;
        //                    originalSelectionIssue.ModifiedOn = DateTime.Now;

        //                    _vmMsg.Type = Enums.MessageType.Success;
        //                    _vmMsg.Msg = "Confirmation Successfully.";
        //                }
        //                else
        //                {
        //                    _vmMsg.Type = Enums.MessageType.Error;
        //                    _vmMsg.Msg = "Not Enough Quantity in Stock.";
        //                }
        //                _context.SaveChanges();
        //            }
        //            tx.Complete();
        //        }
        //    }
        //    catch (DbEntityValidationException e)
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        foreach (var eve in e.EntityValidationErrors)
        //        {
        //            sb.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
        //                                            eve.Entry.Entity.GetType().Name,
        //                                            eve.Entry.State));
        //            foreach (var ve in eve.ValidationErrors)
        //            {
        //                sb.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"",
        //                                            ve.PropertyName,
        //                                            ve.ErrorMessage));
        //            }
        //        }

        //        //_vmMsg.Type = Enums.MessageType.Error;
        //        //_vmMsg.Msg = "Confirmation Failed.";

        //        throw new DbEntityValidationException(sb.ToString(), e);
        //    }


        //    return _vmMsg;
        //}

        # endregion



        public ValidationMsg ConfirmWetBlueSelectionStock(long issueID, long wbSIssueItemID, int userid,
            bool isSelectionLock, long wbSelectionID)
        {
            #region Old Code


            //ValidationMsg Msg = new ValidationMsg();
            //Msg.ReturnId = 1;
            //Msg.Msg = "Confirmation Successful";
            //var currentDate = DateTime.Now.Date;

            //try
            //{
            //    using (TransactionScope Transaction = new TransactionScope())
            //    {
            //        using (var context = new BLC_DEVEntities())
            //        {

            //            var SelectionIssue = (from p in context.PRD_WBSellectionIssue.AsEnumerable()
            //                                      where p.IssueID == issueID
            //                                   select p).FirstOrDefault();

            //            var SelectionIssueItem = (from p in context.PRD_WBSellectionIssueItem.AsEnumerable()
            //                                      where p.WBSIssueItemID == wbSIssueItemID
            //                                        select p).FirstOrDefault();

            //            var SelectionIssueGradeList = (from i in context.PRD_WBSellectionIssueItem.AsEnumerable()
            //                                       join m in context.PRD_WBSellectionIssueGrade on i.WBSIssueItemID equals m.WBSIssueItemID
            //                                       where i.WBSIssueItemID == wbSIssueItemID
            //                                       select new wbSelectionIssueGrade
            //                                       {
            //                                           GradeID = m.GradeID,
            //                                           GradeQty = m.GradeQty,
            //                                           GradeSide = m.GradeSide,
            //                                           GradeArea = m.GradeArea,
            //                                           AreaUnitID = m.AreaUnitID
            //                                       }).ToList();


            //            var _LeatherTypeID = Convert.ToByte(context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Wet Blue").FirstOrDefault().LeatherTypeID);

            //            foreach (var item in SelectionIssueGradeList)
            //            {
            //                ////if (item.GradeQty == null)
            //                ////    item.GradeQty = 0;

            //                //if (item.GradeArea == null)
            //                //    item.GradeArea = 0;

            //                //if (item.GradeSide == null)
            //                //    item.GradeSide = 0;

            //                #region Reduce from wet blue selection stock
            //                var CheckSupplierSelectionStock = (from i in context.INV_WetBlueSelectionStock.AsEnumerable()
            //                                                   where i.StoreID == SelectionIssue.IssueFrom
            //                                                   && i.SupplierID == SelectionIssueItem.SupplierID
            //                                                   && i.PurchaseID == SelectionIssueItem.PurchaseID
            //                                                   && i.ItemTypeID == SelectionIssueItem.ItemTypeID
            //                                                   && i.LeatherStatusID == SelectionIssueItem.LeatherStatusID
            //                                                   && i.GradeID == item.GradeID
            //                                                   select i).Any();
            //                if (!CheckSupplierSelectionStock)
            //                {
            //                    Msg.Msg = "Item Does Not Exist in Stock";
            //                    Msg.ReturnId = 0;
            //                    //break;
            //                }
            //                else
            //                {
            //                    var FoundItem = (from i in context.INV_WetBlueSelectionStock.AsEnumerable()
            //                                     where i.StoreID == SelectionIssue.IssueFrom
            //                                           && i.SupplierID == SelectionIssueItem.SupplierID
            //                                           && i.PurchaseID == SelectionIssueItem.PurchaseID
            //                                           && i.ItemTypeID == SelectionIssueItem.ItemTypeID
            //                                           && i.LeatherStatusID == SelectionIssueItem.LeatherStatusID
            //                                           && i.GradeID == item.GradeID
            //                                     orderby i.TransectionID descending
            //                                     select i).FirstOrDefault();

            //                    if ((FoundItem.ClosingStockPcs >= item.GradeQty) && (FoundItem.ClosingStockSide >= item.GradeSide) && (FoundItem.ClosingStockArea >= item.GradeArea))
            //                    {
            //                        var tblStockSupplier = new INV_WetBlueSelectionStock();

            //                        tblStockSupplier.StoreID = SelectionIssue.IssueFrom;
            //                        tblStockSupplier.SupplierID = SelectionIssueItem.SupplierID;
            //                        tblStockSupplier.PurchaseID = SelectionIssueItem.PurchaseID;
            //                        tblStockSupplier.ItemTypeID = SelectionIssueItem.ItemTypeID;
            //                        tblStockSupplier.LeatherTypeID = _LeatherTypeID;
            //                        tblStockSupplier.LeatherStatusID = SelectionIssueItem.LeatherStatusID;
            //                        tblStockSupplier.GradeID = item.GradeID;

            //                        tblStockSupplier.ClosingStockAreaUnit = item.AreaUnitID;

            //                        tblStockSupplier.OpeningStockPcs = FoundItem.ClosingStockPcs;
            //                        tblStockSupplier.OpeningStockSide = FoundItem.ClosingStockSide;
            //                        tblStockSupplier.OpeningStockArea = FoundItem.ClosingStockArea;

            //                        tblStockSupplier.ReceiveStockPcs = 0;
            //                        tblStockSupplier.ReceiveStockSide = 0;
            //                        tblStockSupplier.ReceiveStockArea = 0;

            //                        tblStockSupplier.IssueStockPcs = item.GradeQty;
            //                        tblStockSupplier.IssueStockSide = item.GradeSide;
            //                        tblStockSupplier.IssueStockArea = item.GradeArea;

            //                        tblStockSupplier.ClosingStockPcs = FoundItem.ClosingStockPcs - item.GradeQty;
            //                        tblStockSupplier.ClosingStockSide = FoundItem.ClosingStockSide - item.GradeSide;
            //                        tblStockSupplier.ClosingStockArea = FoundItem.ClosingStockArea - item.GradeArea;
            //                        tblStockSupplier.SetBy = userid;
            //                        tblStockSupplier.SetOn = DateTime.Now;
            //                        context.INV_WetBlueSelectionStock.Add(tblStockSupplier);
            //                        // context.SaveChanges();
            //                    }
            //                    else
            //                    {
            //                        Msg.Msg = "Not Enough Quantity in Stock";
            //                        Msg.ReturnId = 0;
            //                    }

            //                }
            //                #endregion

            //                //----------------------------------------------------------------------------
            //                if (Msg.ReturnId != 0)
            //                {
            //                    #region Daily_Stock_Update

            //                    var CheckDate = (from ds in context.INV_WetBlueStockDaily.AsEnumerable()
            //                                     where ds.StockDate == currentDate
            //                                     && ds.StoreID == SelectionIssue.IssueTo
            //                                     && ds.ItemTypeID == SelectionIssueItem.ItemTypeID
            //                                     && ds.GradeID == item.GradeID
            //                                     && ds.LeatherStatusID == SelectionIssueItem.LeatherStatusID
            //                                     select ds).Any();

            //                    if (CheckDate)
            //                    {
            //                        var CurrentItem = (from ds in context.INV_WetBlueStockDaily.AsEnumerable()
            //                                           where ds.StockDate == currentDate
            //                                            && ds.StoreID == SelectionIssue.IssueTo
            //                                            && ds.ItemTypeID == SelectionIssueItem.ItemTypeID
            //                                            && ds.GradeID == item.GradeID
            //                                            && ds.LeatherStatusID == SelectionIssueItem.LeatherStatusID
            //                                           select ds).FirstOrDefault();

            //                        CurrentItem.ReceiveStockPcs = CurrentItem.ReceiveStockPcs + item.GradeQty;
            //                        CurrentItem.ReceiveStockSide = CurrentItem.ReceiveStockSide + item.GradeSide;
            //                        CurrentItem.ReceiveStockArea = CurrentItem.ReceiveStockArea + item.GradeArea;

            //                        CurrentItem.ClosingStockPcs = CurrentItem.ClosingStockPcs + item.GradeQty;
            //                        CurrentItem.ClosingStockSide = CurrentItem.ClosingStockSide + item.GradeSide;
            //                        CurrentItem.ClosingStockArea = CurrentItem.ClosingStockArea + item.GradeArea;

            //                        //context.SaveChanges();
            //                    }
            //                    else
            //                    {
            //                        var PreviousRecord = (from ds in context.INV_WetBlueStockDaily.AsEnumerable()
            //                                              where ds.StoreID == SelectionIssue.IssueTo
            //                                                && ds.ItemTypeID == SelectionIssueItem.ItemTypeID
            //                                                && ds.GradeID == item.GradeID
            //                                                && ds.LeatherStatusID == SelectionIssueItem.LeatherStatusID
            //                                              orderby ds.TransectionID descending
            //                                              select ds).FirstOrDefault();

            //                        var objStockDaily = new INV_WetBlueStockDaily();

            //                        objStockDaily.StockDate = currentDate;

            //                        objStockDaily.StoreID = SelectionIssue.IssueTo;
            //                        objStockDaily.ItemTypeID = SelectionIssueItem.ItemTypeID;
            //                        objStockDaily.LeatherTypeID = _LeatherTypeID;
            //                        objStockDaily.LeatherStatusID = SelectionIssueItem.LeatherStatusID;
            //                        objStockDaily.GradeID = item.GradeID;
            //                        objStockDaily.ClosingStockArea = item.AreaUnitID;

            //                        objStockDaily.OpeningStockPcs = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockPcs);
            //                        objStockDaily.OpeningStockSide = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockSide);
            //                        objStockDaily.OpeningStockArea = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockArea);

            //                        objStockDaily.ReceiveStockPcs = item.GradeQty;
            //                        objStockDaily.ReceiveStockSide = item.GradeSide;
            //                        objStockDaily.ReceiveStockArea = item.GradeArea;

            //                        objStockDaily.IssueStockPcs = 0;
            //                        objStockDaily.IssueStockSide = 0;
            //                        objStockDaily.IssueStockArea = 0;

            //                        objStockDaily.ClosingStockPcs = objStockDaily.OpeningStockPcs + item.GradeQty;
            //                        objStockDaily.ClosingStockSide = objStockDaily.OpeningStockSide + item.GradeSide;
            //                        objStockDaily.ClosingStockArea = objStockDaily.OpeningStockArea + item.GradeArea;

            //                        context.INV_WetBlueStockDaily.Add(objStockDaily);
            //                        //context.SaveChanges();

            //                    }

            //                    #endregion

            //                    #region Supplier_Stock_Update

            //                    var CheckSupplierStockForReceive = (from ds in context.INV_WetBlueStockSupplier.AsEnumerable()
            //                                                        where ds.SupplierID == SelectionIssueItem.SupplierID
            //                                                        && ds.PurchaseID == SelectionIssueItem.PurchaseID
            //                                                          && ds.StoreID == SelectionIssue.IssueTo
            //                                                          && ds.ItemTypeID == SelectionIssueItem.ItemTypeID
            //                                                          && ds.GradeID == item.GradeID
            //                                                          && ds.LeatherStatusID == SelectionIssueItem.LeatherStatusID
            //                                                        select ds).Any();

            //                    if (!CheckSupplierStockForReceive)
            //                    {
            //                        var objStockSupplier = new INV_WetBlueStockSupplier();

            //                        objStockSupplier.StoreID = SelectionIssue.IssueTo;
            //                        objStockSupplier.SupplierID = SelectionIssueItem.SupplierID;
            //                        objStockSupplier.PurchaseID = SelectionIssueItem.PurchaseID;
            //                        objStockSupplier.ItemTypeID = SelectionIssueItem.ItemTypeID;
            //                        objStockSupplier.LeatherTypeID = _LeatherTypeID;
            //                        objStockSupplier.LeatherStatusID = SelectionIssueItem.LeatherStatusID;
            //                        objStockSupplier.GradeID = item.GradeID;
            //                        objStockSupplier.AreaUnit = item.AreaUnitID;

            //                        objStockSupplier.OpeningStockPcs = 0;
            //                        objStockSupplier.OpeningStockSide = 0;
            //                        objStockSupplier.OpeningStockArea = 0;

            //                        objStockSupplier.ReceiveStockPcs = item.GradeQty;
            //                        objStockSupplier.ReceiveStockSide = item.GradeSide;
            //                        objStockSupplier.ReceiveStockArea = item.GradeArea;

            //                        objStockSupplier.IssueStockPcs = 0;
            //                        objStockSupplier.IssueStockSide = 0;
            //                        objStockSupplier.IssueStockArea = 0;

            //                        objStockSupplier.ClosingStockPcs = item.GradeQty;
            //                        objStockSupplier.ClosingStockSide = item.GradeSide;
            //                        objStockSupplier.ClosingStockArea = item.GradeArea;
            //                        objStockSupplier.SetOn = DateTime.Now;
            //                        objStockSupplier.SetBy = userid;



            //                        context.INV_WetBlueStockSupplier.Add(objStockSupplier);
            //                        //context.SaveChanges();
            //                    }
            //                    else
            //                    {
            //                        var LastSupplierStock = (from ds in context.INV_WetBlueStockSupplier.AsEnumerable()
            //                                                 where ds.SupplierID == SelectionIssueItem.SupplierID
            //                                                 && ds.PurchaseID == SelectionIssueItem.PurchaseID
            //                                                    && ds.StoreID == SelectionIssue.IssueTo
            //                                                    && ds.ItemTypeID == SelectionIssueItem.ItemTypeID
            //                                                    && ds.GradeID == item.GradeID
            //                                                    && ds.LeatherStatusID == SelectionIssueItem.LeatherStatusID
            //                                                 orderby ds.TransectionID descending
            //                                                 select ds).FirstOrDefault();

            //                        var objStockSupplier = new INV_WetBlueStockSupplier();

            //                        objStockSupplier.StoreID = SelectionIssue.IssueTo;
            //                        objStockSupplier.SupplierID = SelectionIssueItem.SupplierID;
            //                        objStockSupplier.PurchaseID = SelectionIssueItem.PurchaseID;
            //                        objStockSupplier.ItemTypeID = SelectionIssueItem.ItemTypeID;
            //                        objStockSupplier.LeatherTypeID = _LeatherTypeID;
            //                        objStockSupplier.LeatherStatusID = SelectionIssueItem.LeatherStatusID;
            //                        objStockSupplier.GradeID = item.GradeID;
            //                        objStockSupplier.AreaUnit = item.AreaUnitID;

            //                        objStockSupplier.OpeningStockPcs = LastSupplierStock.ClosingStockPcs;
            //                        objStockSupplier.OpeningStockSide = LastSupplierStock.ClosingStockSide;
            //                        objStockSupplier.OpeningStockArea = LastSupplierStock.ClosingStockArea;

            //                        objStockSupplier.ReceiveStockPcs = item.GradeQty;
            //                        objStockSupplier.ReceiveStockSide = item.GradeSide;
            //                        objStockSupplier.ReceiveStockArea = item.GradeArea;

            //                        objStockSupplier.IssueStockPcs = 0;
            //                        objStockSupplier.IssueStockSide = 0;
            //                        objStockSupplier.IssueStockArea = 0;

            //                        objStockSupplier.ClosingStockPcs = LastSupplierStock.ClosingStockPcs + item.GradeQty;
            //                        objStockSupplier.ClosingStockSide = LastSupplierStock.ClosingStockSide + item.GradeSide;
            //                        objStockSupplier.ClosingStockArea = LastSupplierStock.ClosingStockArea + item.GradeArea;
            //                        objStockSupplier.SetOn = DateTime.Now;
            //                        objStockSupplier.SetBy = userid;


            //                        context.INV_WetBlueStockSupplier.Add(objStockSupplier);
            //                        //context.SaveChanges();

            //                    }




            //                    #endregion

            //                    #region Item_Stock_Update

            //                    var CheckItemStockForReceive = (from ds in context.INV_WetBlueStockItem.AsEnumerable()
            //                                                    where ds.StoreID == SelectionIssue.IssueTo
            //                                                          && ds.ItemTypeID == SelectionIssueItem.ItemTypeID
            //                                                          && ds.GradeID == item.GradeID
            //                                                          && ds.LeatherStatusID == SelectionIssueItem.LeatherStatusID
            //                                                    select ds).Any();


            //                    if (!CheckItemStockForReceive)
            //                    {
            //                        var objStockItem = new INV_WetBlueStockItem();

            //                        objStockItem.StoreID = SelectionIssue.IssueTo;
            //                        objStockItem.ItemTypeID = SelectionIssueItem.ItemTypeID;
            //                        objStockItem.LeatherTypeID = _LeatherTypeID;
            //                        objStockItem.LeatherStatusID = SelectionIssueItem.LeatherStatusID;
            //                        objStockItem.GradeID = item.GradeID;
            //                        objStockItem.AreaUnit = item.AreaUnitID;

            //                        objStockItem.OpeningStockPcs = 0;
            //                        objStockItem.OpeningStockSide = 0;
            //                        objStockItem.OpeningStockArea = 0;

            //                        objStockItem.ReceiveStockPcs = item.GradeQty;
            //                        objStockItem.ReceiveStockSide = item.GradeSide;
            //                        objStockItem.ReceiveStockArea = item.GradeArea;

            //                        objStockItem.IssueStockPcs = 0;
            //                        objStockItem.IssueStockSide = 0;
            //                        objStockItem.IssueStockArea = 0;

            //                        objStockItem.ClosingStockPcs = item.GradeQty;
            //                        objStockItem.ClosingStockSide = item.GradeSide;
            //                        objStockItem.ClosingStockArea = item.GradeArea;
            //                        objStockItem.SetBy = userid;
            //                        objStockItem.SetOn = DateTime.Now;

            //                        context.INV_WetBlueStockItem.Add(objStockItem);
            //                        //context.SaveChanges();
            //                    }
            //                    else
            //                    {
            //                        var LastItemInfo = (from ds in context.INV_WetBlueStockItem
            //                                            where ds.StoreID == SelectionIssue.IssueTo
            //                                                && ds.ItemTypeID == SelectionIssueItem.ItemTypeID
            //                                                && ds.GradeID == item.GradeID
            //                                                && ds.LeatherStatusID == SelectionIssueItem.LeatherStatusID
            //                                            orderby ds.TransectionID descending
            //                                            select ds).FirstOrDefault();

            //                        var objStockItem = new INV_WetBlueStockItem();

            //                        objStockItem.StoreID = SelectionIssue.IssueTo;
            //                        objStockItem.ItemTypeID = SelectionIssueItem.ItemTypeID;
            //                        objStockItem.LeatherTypeID = _LeatherTypeID;
            //                        objStockItem.LeatherStatusID = SelectionIssueItem.LeatherStatusID;
            //                        objStockItem.GradeID = item.GradeID;
            //                        objStockItem.AreaUnit = item.AreaUnitID;

            //                        objStockItem.OpeningStockPcs = LastItemInfo.ClosingStockPcs;
            //                        objStockItem.OpeningStockSide = LastItemInfo.ClosingStockSide;
            //                        objStockItem.OpeningStockArea = LastItemInfo.ClosingStockArea ;

            //                        objStockItem.IssueStockPcs = 0;
            //                        objStockItem.IssueStockSide = 0;
            //                        objStockItem.IssueStockArea = 0;

            //                        objStockItem.ReceiveStockPcs = item.GradeQty;
            //                        objStockItem.ReceiveStockSide = item.GradeSide;
            //                        objStockItem.ReceiveStockArea = item.GradeArea;

            //                        objStockItem.ClosingStockPcs = LastItemInfo.ClosingStockPcs + item.GradeQty;
            //                        objStockItem.ClosingStockSide = LastItemInfo.ClosingStockSide + item.GradeSide;
            //                        objStockItem.ClosingStockArea = LastItemInfo.ClosingStockArea + item.GradeArea;
            //                        objStockItem.SetOn = DateTime.Now;
            //                        objStockItem.SetBy = userid;


            //                        context.INV_WetBlueStockItem.Add(objStockItem);
            //                        //context.SaveChanges();
            //                    }

            //                    #endregion
            //                }
            //                //----------------------------------------------------------------------------------
            //            }
            //            if (Msg.ReturnId == 1)
            //            {
            //                var originalSelectionIssue = context.PRD_WBSellectionIssue.First(m => m.IssueID == issueID);
            //                originalSelectionIssue.RecordStatus = "CNF";
            //                originalSelectionIssue.ModifiedBy = userid;
            //                originalSelectionIssue.ModifiedOn = DateTime.Now;
            //                if (isSelectionLock)
            //                {
            //                    var wbSelection = context.PRD_WBSelection.First(m => m.WBSelectionID == wbSelectionID);
            //                    wbSelection.RecordState = "ITS";
            //                    wbSelection.ModifiedBy = userid;
            //                    wbSelection.ModifiedOn = DateTime.Now;

            //                }

            //                context.SaveChanges();
            //                Transaction.Complete();
            //            }
            //        }

            //    }

            //    return Msg;
            //}
            //catch (Exception e)
            //{
            //    Msg.ReturnId = 0;
            //    Msg.Msg = "Confirmation Failed";
            //    return Msg;
            //}

            #endregion
            try
            {
                var dt = new DataTable();
                using (var tx = new TransactionScope())
                {
                    using (var conn = new SqlConnection(_connString))
                    {
                        conn.Open();
                        using (var cmd = new SqlCommand())
                        {
                            cmd.Connection = conn;
                            cmd.CommandText = "UspConfirmWBSelectionStock";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@IssueID", SqlDbType.BigInt).Value = issueID;
                            cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userid;
                            cmd.Parameters.Add("@IsSelectionLock", SqlDbType.Bit).Value = isSelectionLock;
                            cmd.Parameters.Add("@WbSelectionID", SqlDbType.BigInt).Value = wbSelectionID;
                            using (var adp = new SqlDataAdapter(cmd))
                            {
                                adp.Fill(dt);
                            }
                        }

                    }
                    if (dt.Rows.Count > 0)
                    {
                        if (Convert.ToBoolean(dt.Rows[0]["IsStock"]))
                        {
                            tx.Complete();
                            _vmMsg.Type = Enums.MessageType.Success;
                            _vmMsg.Msg = "Confirmation Successfully.";
                        }
                        else
                        {
                            _vmMsg.Type = Enums.MessageType.Error;
                            _vmMsg.Msg = "Not Enough Quantity In Stock.";
                            return _vmMsg;

                        }
                    }
                    else
                    {
                        _vmMsg.Type = Enums.MessageType.Error;
                        _vmMsg.Msg = "Not Enough Quantity In Stock.";
                        return _vmMsg;

                    }

                }
            }

            catch (Exception ex)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Confirmation Failed.";
            }

            return _vmMsg;
        }


        //#################################################### GET WET BLUE GRADE DATA #############################################

        #region GRADE WET BLUE DATA After Save
        public List<wbSelectionIssueGrade> GetWBSellectionIssueGradeAfterSave(string WBSIssueItemID)
        {
            var wbSIssueItemID = Convert.ToInt64(WBSIssueItemID);
            List<PRD_WBSellectionIssueGrade> searchList = _context.PRD_WBSellectionIssueGrade.Where(m => m.WBSIssueItemID == wbSIssueItemID).OrderByDescending(m => m.WBSelectionGradeID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<wbSelectionIssueGrade>();
        }

        public wbSelectionIssueGrade SetToBussinessObject(PRD_WBSellectionIssueGrade Entity)
        {
            wbSelectionIssueGrade Model = new wbSelectionIssueGrade();


            Model.WBSIssueGradeID = Entity.WBSIssueGradeID;
            Model.WBSelectionGradeID = Entity.WBSelectionGradeID;
            Model.WBSIssueItemID = Entity.WBSIssueItemID;
            Model.WBSelectItemNo = Entity.WBSelectItemNo;
            Model.WBSelectionGradeID = Entity.WBSelectionGradeID;
            Model.WBSelectItemNo = Entity.WBSelectItemNo;
            Model.GradeID = Entity.GradeID;
            Model.GradeName = Entity.GradeID == null ? "" : _context.Sys_Grade.Where(m => m.GradeID == Entity.GradeID).FirstOrDefault().GradeName;
            Model.SelectedGradeQty = Entity.GradeQty;
            Model.SelectedGradeSide = Entity.GradeSide;
            Model.SelectedGradeArea = Entity.GradeArea;
            Model.SelectedAreaUnitID = Entity.AreaUnitID;
            Model.UnitName = Entity.AreaUnitID == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.AreaUnitID).FirstOrDefault().UnitName;
            Model.GradeQty = Entity.GradeQty;
            Model.GradeSide = Entity.GradeSide;
            Model.GradeArea = Entity.GradeArea;
            Model.AreaUnitID = Entity.AreaUnitID;
            Model.IssueUnitName = Entity.AreaUnitID == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.AreaUnitID).FirstOrDefault().UnitName;

            return Model;
        }

        #endregion

        public bool DeleteItem(string _WBSIssueGradeID)
        {
            try
            {
                var RequisitionItem = (from c in _context.PRD_WBSellectionIssueGrade.AsEnumerable()
                                       where (c.WBSIssueGradeID).ToString() == _WBSIssueGradeID
                                       select c).FirstOrDefault();

                _context.PRD_WBSellectionIssueGrade.Remove(RequisitionItem);
                _context.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public ValidationMsg CheckedRecordStatus(long IssueID, string status, int userID)
        {
            int flag = 0;
            long id = (IssueID == null ? 0 : Convert.ToInt64(IssueID));
            var ob = (from temp in _context.PRD_WBSellectionIssue where temp.IssueID == IssueID select temp).FirstOrDefault();//repository.PrdWBSelectionRepository.GetByID(id);
            if (ob != null)
            {
                _vmMsg.ReturnCode = ob.RecordStatus;
                if (ob.RecordStatus == "CNF")
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Record already Confirmed.";
                }
                else if (ob.RecordStatus == "CHK")
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Data already Checked.";
                }

                else if (ob.RecordStatus == "NCF" && status == "CHK")
                {
                    // Chked              
                    ob.RecordStatus = status;
                    ob.CheckedBy = userID;
                    ob.CheckDate = DateTime.Now;
                    flag = _context.SaveChanges();
                    if (flag == 1)
                    {
                        _vmMsg.Type = Enums.MessageType.Success;
                        _vmMsg.Msg = "Checked Successfully.";
                    }
                    else
                    {
                        _vmMsg.Type = Enums.MessageType.Error;
                        _vmMsg.Msg = "Checked Faild.";
                    }
                }
                else
                {
                    if (ob.RecordStatus == "CHK")
                    {
                        _vmMsg.Type = Enums.MessageType.Error;
                        _vmMsg.Msg = "Data Already Checked ";
                    }
                    else if (ob.RecordStatus == "CNF")
                    {
                        _vmMsg.Type = Enums.MessageType.Error;
                        _vmMsg.Msg = "Data Already Confirmed";
                    }
                    else
                    {
                    }

                }
            }
            else
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Checked Faild.";
            }
            return _vmMsg;
        }

    }
}








