using System;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;
using DatabaseUtility;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using System.Reflection;
using System.Collections.Generic;

namespace ERP.BusinessLogicLayer.OperationManager
{
    public class BllPrqPurchase
    {
        #region Private Variables

        private readonly DatabseManager _databseManager;
        private DalPrqPurchase _dalPrqPurchase;

        private int _result;

        public long purchaseID;

        #endregion

        #region Construction

        public BllPrqPurchase()
        {
            _databseManager = new DatabseManager(DatabaseConfiguration.ConnectionString, DatabaseConfiguration.DatabaseProvider);

        }

        #endregion


        public ValidationMsg SavePurchaseReceiveWithChallanItem(PurchaseReceive purchaseReceive, int userId)
        {


            var transactionOption = new TransactionOptions { Timeout = new TimeSpan(0, 0, 9999) };
            var vMsg = new ValidationMsg();
            using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOption))
            {
                try
                {
                    _dalPrqPurchase = new DalPrqPurchase();

                    purchaseID = _dalPrqPurchase.SavePurchaseInformation(purchaseReceive, userId);

                    //GetpurchaseID(purchaseID);

                    if (purchaseID > 0)
                    {

                        transactionScope.Complete();

                        vMsg.Type = Enums.MessageType.Success;
                        vMsg.Msg = "Saved successfully.";
                    }
                    else
                    {
                        vMsg.Type = Enums.MessageType.Error;
                        vMsg.Msg = "Failed to save.";

                    }
                }

                catch (Exception)
                {
                    vMsg.Type = Enums.MessageType.Error;
                    vMsg.Msg = "Failed to save.";
                }

            }
            return vMsg;
        }

        public ValidationMsg UpdatePurchaseReceiveWithChallanItem(PurchaseReceive purchaseReceive, int userId)
        {


            var transactionOption = new TransactionOptions { Timeout = new TimeSpan(0, 0, 9999) };
            var vMsg = new ValidationMsg();
            using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOption))
            {
                try
                {
                    _dalPrqPurchase = new DalPrqPurchase();

                    var UpdateStatus = _dalPrqPurchase.UpdatePurchaseInformation(purchaseReceive, userId);

                    if (UpdateStatus > 0)
                    {

                        transactionScope.Complete();

                        vMsg.Type = Enums.MessageType.Update;
                        vMsg.Msg = "Updated successfully.";
                    }
                    else
                    {
                        vMsg.Type = Enums.MessageType.Error;
                        vMsg.Msg = "Failed to Update.";

                    }
                }

                catch (Exception)
                {
                    vMsg.Type = Enums.MessageType.Error;
                    vMsg.Msg = "Failed to Update.";
                }

            }
            return vMsg;
        }

        //public ValidationMsg ConfirmPurchase(string purchaseNumber, string confirmComment)
        //{


        //    var transactionOption = new TransactionOptions { Timeout = new TimeSpan(0, 0, 9999) };
        //    var vMsg = new ValidationMsg();
        //    using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOption))
        //    {
        //        try
        //        {
        //            _dalPrqPurchase = new DalPrqPurchase();

        //            var ConfirmStatus = _dalPrqPurchase.ConfirmPurchase(purchaseNumber, confirmComment);

        //            if (ConfirmStatus > 0)
        //            {

        //                transactionScope.Complete();

        //                vMsg.Type = Enums.MessageType.Update;
        //                vMsg.Msg = "Confirmation successful.";
        //            }
        //            else
        //            {
        //                vMsg.Type = Enums.MessageType.Error;
        //                vMsg.Msg = "Confirmation Failed.";

        //            }
        //        }

        //        catch (Exception)
        //        {
        //            vMsg.Type = Enums.MessageType.Error;
        //            vMsg.Msg = "Confirmation Failed.";
        //        }

        //    }
        //    return vMsg;
        //}

        public long GetpurchaseID()
        {
            return purchaseID;
        }

        //public ValidationMsg SavePurchaseReceiveWithChallanItems(PurchaseReceive purchaseReceive)
        //{



        //    DataTable dtItem = new DataTable();
        //    dtItem.Columns.Add("ItemTypeID", typeof(int));
        //    dtItem.Columns.Add("SizeID", typeof(int));
        //    dtItem.Columns.Add("UnitID", typeof(int));
        //    dtItem.Columns.Add("Description", typeof(string));
        //    dtItem.Columns.Add("ChallanQty", typeof(int));
        //    dtItem.Columns.Add("ReceiveQty", typeof(int));
        //    dtItem.Columns.Add("Remark", typeof(string));

        //    DataRow dr;
        //    foreach (var item in purchaseReceive.ChallanItemList)
        //    {
        //        dr = dtItem.NewRow();
        //        dr["ItemTypeID"] = item.ItemTypeID;
        //        dr["SizeID"] = item.ItemSizeID;
        //        dr["UnitID"] = item.UnitID;
        //        dr["Description"] = item.Description;
        //        dr["ChallanQty"] = item.ChallanQty;
        //        dr["ReceiveQty"] = item.ReceiveQty;
        //        dr["Remark"] = item.Remark;

        //        dtItem.Rows.Add(dr);
        //    }
        //    var challan = new PrqPurchaseChallan();
        //    foreach (var c in purchaseReceive.ChallanList)
        //    {
        //        challan.ChallanNo = c.ChallanNo;
        //        challan.ChallanCategory = c.ChallanCategory;
        //        challan.ChallanDate = c.ChallanDate;
        //        challan.LocationID = c.LocationID;
        //        challan.ReceiveStore = c.ReceiveStore;
        //        challan.ChallanNote = c.ChallanNote;
        //        challan.SourceID = c.SourceID;
        //    }

        //    var transactionOption = new TransactionOptions { Timeout = new TimeSpan(0, 0, 9999) };
        //    var vMsg = new ValidationMsg();
        //    using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOption))
        //    {
        //        try
        //        {
        //            //_databseManager.Open();
        //            _dalPrqPurchase = new DalPrqPurchase(_databseManager);



        //            if (_dalPrqPurchase.SavePurchaseInformationWithSP(dtItem, purchaseReceive, challan, strCon: DatabaseConfiguration.ConnectionString) > 0)
        //            {
        //                transactionScope.Complete();

        //                vMsg.Type = Enums.MessageType.Success;
        //                vMsg.Msg = "Save successfully.";
        //            }
        //            else
        //            {
        //                vMsg.Type = Enums.MessageType.Error;
        //                vMsg.Msg = "Failed to save.";

        //            }
        //        }
        //        catch (SqlException sqlException)
        //        {
        //            vMsg.Type = Enums.MessageType.Error;
        //            vMsg.Msg = "Failed to save.";
        //        }
        //        catch (Exception exception)
        //        {
        //            vMsg.Type = Enums.MessageType.Error;
        //            vMsg.Msg = "Failed to save.";
        //        }
        //        finally
        //        {
        //            //_databseManager.Close();
        //        }
        //    }

        //    return vMsg;
        //}
        //public ValidationMsg GetSupplierList()
        //{

        //    var transactionOption = new TransactionOptions { Timeout = new TimeSpan(0, 0, 9999) };
        //    var vMsg = new ValidationMsg();
        //    using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOption))
        //    {
        //        try
        //        {
        //            _databseManager.Open();
        //            _dalPrqPurchase = new DalPrqPurchase(_databseManager);

        //            if (_dalPrqPurchase.GetSupplierList() > 0)
        //            {
        //                transactionScope.Complete();

        //                vMsg.Type = Enums.MessageType.Success;
        //                vMsg.Msg = "Save successfully.";
        //            }
        //            else
        //            {
        //                vMsg.Type = Enums.MessageType.Error;
        //                vMsg.Msg = "Failed to save.";

        //            }
        //        }
        //        catch (SqlException sqlException)
        //        {
        //            vMsg.Type = Enums.MessageType.Error;
        //            vMsg.Msg = "Failed to save.";
        //        }
        //        catch (Exception exception)
        //        {
        //            vMsg.Type = Enums.MessageType.Error;
        //            vMsg.Msg = "Failed to save.";
        //        }
        //        finally
        //        {
        //            _databseManager.Close();
        //        }
        //    }

        //    return vMsg;
        //}
    }


}
