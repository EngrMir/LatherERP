using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.BaseModel
{
    public static class DatabaseConfiguration
    {
       
        public static string ConnectionString { get; set; }

        public static string ServerName { get; set; }

        public static string DatabaseName { get; set; }

        public static string UserName { get; set; }

        public static string Password { get; set; }

        public static string DatabaseProvider { get; set; }

        public static int DefaultTimeOut { get; set; }

        public static string BackupLocation { get; set; }
    }
}
