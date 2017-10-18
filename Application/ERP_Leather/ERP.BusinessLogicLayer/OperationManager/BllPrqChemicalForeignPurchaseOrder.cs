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
    public class BllPrqChemicalForeignPurchaseOrder
    {
         #region Private Variables

        private readonly DatabseManager _databseManager;
        private DalChemicalForeignPurchaseOrder _dalChemicalForeignPurchaseOrder;

        private int _result;

        public long OrderID;

        #endregion

        #region Construction

        public BllPrqChemicalForeignPurchaseOrder()
        {
            _databseManager = new DatabseManager(DatabaseConfiguration.ConnectionString, DatabaseConfiguration.DatabaseProvider);

        }

        #endregion

        public ValidationMsg Save(ChemicalForeignPurchaseOrder model, int userId, string pageUrl)
        {


            var transactionOption = new TransactionOptions { Timeout = new TimeSpan(0, 0, 99999) };
            var vMsg = new ValidationMsg();


            using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOption))
            {
                    try
                    {
                        _dalChemicalForeignPurchaseOrder = new DalChemicalForeignPurchaseOrder();

                        OrderID = _dalChemicalForeignPurchaseOrder.Save(model, userId, pageUrl);

                        //GetpurchaseID(purchaseID);

                        if (OrderID > 0)
                        {

                            transactionScope.Complete();

                            vMsg.Type = Enums.MessageType.Success;
                            vMsg.Msg = "Saved successfully.";
                            //vMsg.ReturnId = _dalChemicalForeignPurchaseOrder.GetOrderNo(OrderID);
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

        public ValidationMsg Update(ChemicalForeignPurchaseOrder model, int userId)
        {


            var transactionOption = new TransactionOptions { Timeout = new TimeSpan(0, 0, 9999) };
            var vMsg = new ValidationMsg();
            using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOption))
            {
                try
                {
                    _dalChemicalForeignPurchaseOrder = new DalChemicalForeignPurchaseOrder();

                    var UpdateStatus = _dalChemicalForeignPurchaseOrder.UpdateChemicalPurchaseOrderInformation(model, userId);

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

        public long GetpurchaseID()
        {
            return OrderID;
        }

    }
}
