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
    public class BllWBRequisitionForCrusting
    {
        #region Private Variables

        private readonly DatabseManager _databseManager;
        private DalWBRequisitionForCrusting _dalObj;

        //private int _result;

        public long RequisitionItemID;

        #endregion

        #region Construction

        public BllWBRequisitionForCrusting()
        {
            _databseManager = new DatabseManager(DatabaseConfiguration.ConnectionString, DatabaseConfiguration.DatabaseProvider);

        }

        #endregion



        public ValidationMsg Save(PRDYearMonthCrustReqItem model, int userId)
        {


            var transactionOption = new TransactionOptions { Timeout = new TimeSpan(0, 0, 9999) };
            var vMsg = new ValidationMsg();
            using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOption))
            {
                try
                {
                    _dalObj = new DalWBRequisitionForCrusting();

                    RequisitionItemID = _dalObj.Save(model, userId);

                    if (RequisitionItemID > 0)
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


        public long GetRequisitionItemID()
        {
            return RequisitionItemID;
        }

    }
}
