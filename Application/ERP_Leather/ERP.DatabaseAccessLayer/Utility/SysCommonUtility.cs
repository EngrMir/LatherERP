using ERP.DatabaseAccessLayer.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DatabaseAccessLayer.Utility
{
    public class SysCommonUtility
    {
        private readonly string _connString = string.Empty;
        public SysCommonUtility()
        {
            _connString = StrConnection.GetConnectionString();
        }

        private BLC_DEVEntities _context = new BLC_DEVEntities();

        // Check Specific Input Field Validity
        public bool IsValid(string tbl, string fld, string value, string type)
        {
            DataTable dt = new DataTable();
            string sql = "";
       
            if (type.Equals("int"))
            {
                sql = "SELECT * FROM " + tbl + " WHERE " + fld + " = " + value;
            }
            else
                sql = "SELECT * FROM " + tbl + " WHERE " + fld + " = '" + value + "'";
            
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);                   
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dt);
                }
                if (dt.Rows.Count > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return false;
        }
       
    }
}
