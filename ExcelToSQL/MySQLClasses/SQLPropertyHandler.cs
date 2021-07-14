using System;
using System.Linq;
using System.Reflection;

namespace ExcelToSQL.SQLClasses
{
    public static class SQLPropertyHandler
    {
        public static object GetSqlValue(this PropertyInfo propertyInfo, object obj, Type type)
        {
            var value = propertyInfo.GetValue(obj);
            SqlSafe(ref value, type);
            return value;
        }

        private static void SqlSafe(ref object val, Type type)
        {
            switch (type.Name)
            {
                case "String":
                    string str = (string)Convert.ChangeType(val, type);
                    var chars = str.Replace("\\", "\\\\")
                                   .Replace("\"", "\\\"")
                                   .Replace("\'", "\\\'")
                                   .Append('\"')
                                   .Prepend('\"')
                                   .ToArray();
                    val = new String(chars);
                    break;

                //TODO: Add sql safe parsing for other data types
            }
        }
    }
}
