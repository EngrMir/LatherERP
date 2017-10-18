using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseUtility;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using System.Transactions;

namespace ERP.BusinessLogicLayer.OperationManager
{
    public  class BllStockAdjust
    {
        #region Private Variables

        private readonly DatabseManager _databseManager;
        private DalInvLeatherStockAdjustRequest _dalInvLeatherStockAdjust;
        private int _result;
        private string _connectionString = string.Empty;
        public int currentRequestID;
        #endregion

        #region Construction

        public BllStockAdjust()
        {
            _databseManager = new DatabseManager(DatabaseConfiguration.ConnectionString, DatabaseConfiguration.DatabaseProvider);
            _connectionString = DatabaseConfiguration.ConnectionString;
        }

        #endregion


        public ValidationMsg SaveAdjustItemRequest(InvLeatherStockAdjustModel model, int userID)
        {


            var transactionOption = new TransactionOptions { Timeout = new TimeSpan(0, 0, 9999) };
            var vMsg = new ValidationMsg();
            using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOption))
            {
                try
                {
                    DalInvLeatherStockAdjustItem objStockItem = new DalInvLeatherStockAdjustItem();

                    currentRequestID = objStockItem.SaveAdjustItemRequest(model, userID);

                    if (currentRequestID > 0)
                    {

                        transactionScope.Complete();

                        vMsg.Type = Enums.MessageType.Success;
                        vMsg.Msg = "Save successfully.";
                    }
                    else
                    {
                        vMsg.Type = Enums.MessageType.Error;
                        vMsg.Msg = "Failed to save.";

                    }
                }

                catch (Exception e)
                {
                    vMsg.Type = Enums.MessageType.Error;
                    vMsg.Msg = "Failed to save.";
                }

            }
            return vMsg;
       }

        public int GetRequestID()
        {
            return currentRequestID;
        }

        public ValidationMsg UpdateAdjustItemRequest(InvLeatherStockAdjustModel model, int userID)
        {


            var transactionOption = new TransactionOptions { Timeout = new TimeSpan(0, 0, 9999) };
            var vMsg = new ValidationMsg();
            using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOption))
            {
                try
                {
                    var _dalStockAdjustItem = new DalInvLeatherStockAdjustItem();

                    currentRequestID = _dalStockAdjustItem.UpdateAdjustItemRequest(model, userID);

                    if (currentRequestID > 0)
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

                catch (Exception e)
                {
                    vMsg.Type = Enums.MessageType.Error;
                    vMsg.Msg = "Failed to Update.";
                }

            }
            return vMsg;
        }


        public ValidationMsg UpdateAdjustmentRequestWithItemValue(InvLeatherStockAdjustModel model)
        {


            var transactionOption = new TransactionOptions { Timeout = new TimeSpan(0, 0, 9999) };
            var vMsg = new ValidationMsg();
            using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOption))
            {
                try
                {
                    var _dalStockAdjustItem = new DalInvLeatherStockAdjustItem();

                    var CheckUpdateStatus = _dalStockAdjustItem.UpdateAdjustmentRequestWithItemValue(model);

                    if (CheckUpdateStatus > 0)
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

   }
}
