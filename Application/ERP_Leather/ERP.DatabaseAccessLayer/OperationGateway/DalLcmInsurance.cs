using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.BaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalLcmInsurance
    {
        private readonly BLC_DEVEntities obEntity;
       // private UnitOfWork repository;
        private ValidationMsg _vmMsg;

        public void GetLcmInsuranceInfo()
        {
            //string sql = @"SELECT temp.InsuranceID, temp.InsuranceNo, temp.LCNo, temp.CoverNoteDate, temp.RecordStatus  FROM LCM_Insurence";
            //var result = obEntity.Database.SqlQuery<LCM_LCOpeningBank>(sql);
            //return result;
        }
    }
}
