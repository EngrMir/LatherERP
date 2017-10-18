using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseUtility;
using ERP.EntitiesModel.BaseModel;

namespace ERP.DatabaseAccessLayer.Utility
{
   public static  class StrConnection
   {

       public static  string GetConnectionString()
       {
           return  DatabaseConfiguration.ConnectionString;
               
       }
       public static string GetDatabaseProvider()
       {
           return DatabaseConfiguration.DatabaseProvider;

       }
   
    }
}
