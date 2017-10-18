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
    public class BllChemicalConsumptionForFinishing
    {
        #region Private Variables

        private readonly DatabseManager _databseManager;
        private DalChemicalConsumptionForFinishing _dalChemicalConsumption;

        private int _result;

        public long CLProductionID;

        #endregion

        #region Construction

        public BllChemicalConsumptionForFinishing()
        {
            _databseManager = new DatabseManager(DatabaseConfiguration.ConnectionString, DatabaseConfiguration.DatabaseProvider);

        }

        #endregion



        public ValidationMsg Save(ChemicalConsumptionForCrusting model, int userId, string pageUrl)
        {


            var transactionOption = new TransactionOptions { Timeout = new TimeSpan(0, 0, 99999) };
            var vMsg = new ValidationMsg();


            using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOption))
            {
                try
                {
                    _dalChemicalConsumption = new DalChemicalConsumptionForFinishing();

                    CLProductionID = _dalChemicalConsumption.Save(model, userId, pageUrl);

                    //GetpurchaseID(purchaseID);

                    if (CLProductionID > 0)
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

        public ValidationMsg Update(ChemicalConsumptionForCrusting model, int userId)
        {


            var transactionOption = new TransactionOptions { Timeout = new TimeSpan(0, 0, 9999) };
            var vMsg = new ValidationMsg();
            using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOption))
            {
                try
                {
                    _dalChemicalConsumption = new DalChemicalConsumptionForFinishing();

                    var UpdateStatus = _dalChemicalConsumption.Update(model, userId);

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

        public long GetProductionID()
        {
            return CLProductionID;
        }
    }
}
