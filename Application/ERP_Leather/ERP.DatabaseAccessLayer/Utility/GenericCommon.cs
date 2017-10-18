using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DatabaseAccessLayer.Utility
{
    public static class GenericCommon
    {

        //Using a Generic Method
        //This is a generic method that will convert any type of DataTable to a List (the DataTable structure and List class structure should be the same).
        //The following are the two functions in which if we pass a DataTable and a user defined class. It will then return the List of that class with the DataTable data.
        //To call the preceding method, use the following syntax:

        //List< Student > studentDetails = new List< Student >();  
        //studentDetails = ConvertDataTable< Student >(dt);  
        //Change the Student class name and dt value based on your requirements. In this case the DataTable column's name and class property name should be the same otherwise this function will not work properly.
        public static List<T> ConvertDataTableToList<T>(DataTable dt)
        {
            return (from DataRow row in dt.Rows select GetItem<T>(row)).ToList();
        }

        private static T GetItem<T>(DataRow dr)
        {
            var temp = typeof(T);
            var obj = Activator.CreateInstance<T>();

            foreach (DataColumn col in dr.Table.Columns)
            {
                foreach (var pro in temp.GetProperties().Where(pro => pro.Name == col.ColumnName))
                {
                    pro.SetValue(obj, dr[col.ColumnName], null);
                }
            }
            return obj;
        }
    }
}
