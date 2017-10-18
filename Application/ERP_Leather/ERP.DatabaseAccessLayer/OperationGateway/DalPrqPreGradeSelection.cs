using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using System.Data;
using ERP.DatabaseAccessLayer.Utility;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalPrqPreGradeSelection
    {
        private BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;
        private UnitOfWork repository = new UnitOfWork();
        public DalPrqPreGradeSelection()
        {
            _context = new BLC_DEVEntities();
            _vmMsg = new ValidationMsg();
        }

        public ValidationMsg SaveGradeSelectionData(PreGradeSelectionData model)
        {
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        if (model.SelectionID == 0)
                        {
                            var objPreGradeSelection = new Prq_PreGradeSelection
                            {
                                SelectionType = "PGS",
                                SupplierID = model.SupplierID,
                                PurchaseID = model.PurchaseID,
                                ItemTypeID = model.ItemTypeID,
                                SupplierAddressID = model.SupplierAddressID,
                                SelectionStore = model.SelectionStore,
                                SelectionDate = Convert.ToDateTime(model.SelectionDate),
                                SelectedBy = model.SelectedBy,
                                SelectionComments = model.SelectionComments,
                                ApprovedBy = model.ApprovedBy,
                                ApproveComments = model.ApproveComments,
                                CheckedBy = model.CheckedBy,
                                CheckComments = model.CheckComments,
                                RecordStatus = "NCF"
                            };
                            _context.Prq_PreGradeSelection.Add(objPreGradeSelection);
                            
                            foreach (var gradeSelection in model.GradeSelectedList)
                            {
                                var objPreGradeSelectedData = new Prq_PreGradeSelectedData
                                {
                                    SelectionID = objPreGradeSelection.SelectionID,                                    
                                    GradeID = gradeSelection.GradeID,
                                    GradeQty = gradeSelection.GradeQty,
                                    GradeSide = gradeSelection.GradeSide,
                                    GradeArea = gradeSelection.GradeArea,
                                    UnitID = gradeSelection.UnitID,
                                    RecordStatus = "NCF",
                                    SetOn = DateTime.Now
                                 
                                };
                                _context.Prq_PreGradeSelectedData.Add(objPreGradeSelectedData);
                            }
                            _context.SaveChanges();
                        }
                        else
                        {
                            var query = new StringBuilder();
                            query.Append("Delete  From Prq_PreGradeSelectedData");
                            query.Append(" WHERE [SelectionID] = '" + model.SelectionID + "'");
                            _context.Database.ExecuteSqlCommand(query.ToString());

                            foreach (var gradeSelection in model.GradeSelectedList)
                            {
                                var objPreGradeSelectedData = new Prq_PreGradeSelectedData
                                                                {
                                                                    SelectionID = model.SelectionID,                                                                   
                                                                    GradeID = gradeSelection.GradeID,
                                                                    GradeQty = gradeSelection.GradeQty,
                                                                    GradeArea = gradeSelection.GradeArea,
                                                                    UnitID = gradeSelection.UnitID,
                                                                    GradeSide = gradeSelection.GradeSide,
                                                                    RecordStatus = "NCF",
                                                                    SetOn = DateTime.Now
                                                                };
                                _context.Prq_PreGradeSelectedData.Add(objPreGradeSelectedData);                          

                            }
                            _context.SaveChanges();
                        }

                        tx.Complete();

                        _vmMsg.Type = Enums.MessageType.Success;
                        _vmMsg.Msg = "Saved Successfully.";
                    }
                }
            }
            catch (Exception)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to save.";
            }

            return _vmMsg;
        }

        public List<GradeSelectionChallanItem> GetChallanInfoForChallanTable(string purchaseID)
        {
            var storeParam = new SqlParameter[1];
            storeParam[0] = new SqlParameter("PurchaseID", Convert.ToInt64(purchaseID));

            var iChallanInof = _context.Database.SqlQuery<GradeSelectionChallanItem>("exec UspPrqSaveItemSelectedData @PurchaseID", storeParam);
            return iChallanInof.ToList();
        }

        public List<PrqPreGradeSelectedData> GetGradeSelectionData(Int64 purchaseId, byte itemTypeID)
        {
            var queryString = "SELECT pgs.SelectionComments, pgs.CheckedBy,pgs.CheckComments,pgs.ApprovedBy,pgs.ApproveComments, pgsd.SelectionID, pgsd.GradeID,pgsd.GradeQty," +
                              " pgsd.GradeSide, pgsd.GradeArea,pgsd.UnitID,pgsd.RecordStatus,pgsd.SetOn,pgsd.SetBy, " +
                              " pgsd.IPAddress, sg.GradeName, su.UnitName, CONVERT(VARCHAR(10),pgs.SelectionDate,106) AS SelectionDate, pgs.SelectedBy " +
                              " FROM Prq_PreGradeSelectedData AS pgsd" +
                              " INNER JOIN Sys_Grade AS sg ON sg.GradeID = pgsd.GradeID" +
                              " INNER JOIN Sys_Unit AS su ON su.UnitID = pgsd.UnitID" +
                              " INNER JOIN Prq_PreGradeSelection AS pgs ON pgs.SelectionID = pgsd.SelectionID" +
                              " where pgs.PurchaseID = '" + purchaseId + "' AND pgs.ItemTypeID='" +itemTypeID + "' AND pgsd.RecordStatus = 'NCF'";

            var iGradeSelectedData = _context.Database.SqlQuery<PrqPreGradeSelectedData>(queryString);
            return iGradeSelectedData.ToList();
        }

        public ValidationMsg GradeSelectionConfirm(Int64 selectionID)
        {
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        var query = new StringBuilder();


                        query.Append("UPDATE Prq_PreGradeSelection");
                        query.Append(" SET [RecordStatus] = 'CNF'");
                        query.Append(" WHERE [selectionID] = '" + selectionID + "';");

                        query.Append("UPDATE Prq_PreGradeSelectedData");
                        query.Append(" SET [RecordStatus] = 'CNF'");
                        query.Append(" WHERE [selectionID] = '" + selectionID + "'");

                        _context.Database.ExecuteSqlCommand(query.ToString());
                    }
                    tx.Complete();
                }
                _vmMsg.Type = Enums.MessageType.Success;
                _vmMsg.Msg = "Confirm Successfully.";
            }
            catch (Exception)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Confirmation.";
            }

            return _vmMsg;
        }


        public ValidationMsg setSNR(string purchaseId)
        {
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        string[] purchaseIds = purchaseId.Split(',');
                        for (int i = 0; i < purchaseIds.Length; i++)
                        {
                            int purcid= Convert.ToInt32(purchaseIds[i]);
                            Prq_Purchase data = (from temp in _context.Prq_Purchase where temp.PurchaseID == purcid && temp.RecordStatus == "CNF" select temp).FirstOrDefault();
                            data.RecordStatus = "SNR";
                            _context.SaveChanges();
                        }
                       
                    }
                    tx.Complete();
                }
                _vmMsg.Type = Enums.MessageType.Success;
                _vmMsg.Msg = "SNR set successfully for PurchaseId ("+purchaseId+").";
            }
            catch (Exception)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Confirmation.";
            }

            return _vmMsg;
        }

    }
}
