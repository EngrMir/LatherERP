using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DatabaseAccessLayer.Utility
{
    public class ExecuteSqlStatement
    {
        protected static int ExecuteProcedure(string strCon, SqlCommand objCmd)
        {
            try
            {
                using (var conn = new SqlConnection(strCon))
                {
                    conn.Open();
                    objCmd.Connection = conn;
                    return objCmd.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

