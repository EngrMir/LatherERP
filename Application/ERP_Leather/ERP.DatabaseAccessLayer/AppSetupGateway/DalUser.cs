using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;

namespace ERP.DatabaseAccessLayer.AppSetupGateway
{
   public  class DalUser
    {

        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;

        public DalUser()
        {
            _context = new BLC_DEVEntities();
            _vmMsg = new ValidationMsg();
        }

        public IEnumerable<UserViewModel> GetUserInformation(int? userID, string loginID)
        {
            var query = new StringBuilder();

            query.Append("SELECT U.UserID, U.UserCode, U.UserType, U.Title, U.FirstName, ISNULL(U.MiddleName,''), U.LastName,");
            query.Append(" U.DesignationID, U.Email, U.Phone, U.Mobile, U.IsActive, U.SupervisorUserID, U.Description,");
            query.Append(" CONVERT(NVARCHAR(12), U.SetOn, 106) AS SetOn, U.SetBy,");
            query.Append(" D.Name AS DesignationTitle,");
            query.Append(" UCI.LoginID, UCI.Password, CONVERT(NVARCHAR(12),UCI.LastPasswordChangedDate, 106) AS LastPasswordChangedDate, UCI.IsPasswordAccepted, UCI.IsLocked,");
            query.Append(" CONVERT(NVARCHAR(12),UCI.SetOn, 106) AS UCI_SetOn, UCI.SetBy AS UCI_SetBy");
            query.Append(" FROM Security.Users AS U LEFT JOIN");
            query.Append(" Security.Designations AS D ON U.DesignationID = D.DesignationID INNER JOIN");
            query.Append(" Security.UserCredentialInformation AS UCI ON U.UserID = UCI.UserID");
            query.Append(" WHERE 1=1");

            if (userID != null)
            {
                query.Append(" AND U.UserID = {0}");
            }
            if (!string.IsNullOrWhiteSpace(loginID))
            {
                query.Append(" AND UCI.LoginID = {1}");
            }

            return _context.Database.SqlQuery<UserViewModel>(query.ToString(), userID, loginID);
        }

        public IEnumerable<User> GetAllUser() 
        {
            return _context.Users.Where(ob =>ob.IsActive && !ob.IsDelete).ToList();
        }

        public IEnumerable<UserViewModel> GetAllUserExceptSuper()
        {
            var query = new StringBuilder();

            query.Append("SELECT U.UserID, U.UserCode, U.UserType, U.Title, U.FirstName, ISNULL(U.MiddleName,'') MiddleName, U.LastName,");
            query.Append(" U.DesignationID, U.Email, U.Phone, U.Mobile, U.IsActive, U.SupervisorUserID, U.Description,");
            query.Append(" D.Name AS DesignationTitle ");
            query.Append(" FROM Security.Users AS U LEFT JOIN");
            query.Append(" Security.Designations AS D ON U.DesignationID = D.DesignationID INNER JOIN");
            query.Append(" Security.UserCredentialInformation AS UCI ON U.UserID = UCI.UserID");
            query.Append(" WHERE U.UserID !=1");
            query.Append(" ORDER BY U.FirstName");
            return _context.Database.SqlQuery<UserViewModel>(query.ToString());
            
        }

       public int ChangePassword(int userId, string password)
       {
           using (_context)
           {
               var objUpdate = _context.UserCredentialInformations.FirstOrDefault(u => u.UserID == userId);
               if (objUpdate == null) return 0;
               objUpdate.IsPasswordAccepted = true;
               objUpdate.Password = password;
               _context.SaveChanges();
           }
           return 1;
       }
    }
}
