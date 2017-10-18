using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.BaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
  
    public class DalLcmBankDebitVoucher
    {
        private readonly BLC_DEVEntities obEntity;     
        private UnitOfWork repository;
        private ValidationMsg _vmMsg;
        public DalLcmBankDebitVoucher()
        {
            obEntity = new BLC_DEVEntities();
            _vmMsg = new ValidationMsg();
            repository = new UnitOfWork();        
        }

//        public IEnumerable<DalLcmBankDebitVoucher> GetBankDebitVoucherInfo()
//        {
//            string sql = @"SELECT b.BankID,br.BranchSwiftCode BankCode,b.BankName, br.BranchID,br.BranchName, br.LCLimit,br.LCMargin,c.CurrencyName FROM Sys_Bank b
//                            INNER JOIN Sys_Branch br ON b.BankID = br.BankID
//                            LEFT JOIN Sys_Currency c ON br.LCCurrency = c.CurrencyID
//                            WHERE b.BankCategory='" + bankCategory + "' AND b.BankType ='" + bankType + "' AND b.IsActive='true' AND br.IsActive='true'";
//            var result = obEntity.Database.SqlQuery<LCM_LCOpeningBank>(sql);
//            return result;
//        }
    }
}
