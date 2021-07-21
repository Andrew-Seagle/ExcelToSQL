using ExcelToSQL.CustomAttributes;
using ExcelToSQL.ExcelClasses;
using ExcelToSQL.TableClasses;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TopologicalSorter;

namespace ExcelToSQL.MySQLClasses
{
    public static class PrioritySorter
    {
        internal static object[] PrioritySort(string databaseName, Dictionary<string, List<ExcelEnum>> excelEnumCollection)
        {
            var classList = GetAllClassNames(excelEnumCollection);
            var foreignClassDict = GetAllForeignClasses(databaseName, classList);

            var sortedArray = TSorter.Sort(foreignClassDict, false);

            return sortedArray;
        }

        private static Dictionary<object, List<object>>GetAllForeignClasses(string databaseName, List<string> classList)
        {
            var fClassDict = new Dictionary<object, List<object>>();
            var _typeSQLAttr = typeof(SQLColumn);

            foreach (var className in classList)
            {
                var fClassList = new List<object>();

                var classType = TableClassHelper.IntensiveTypeSearch(className, databaseName);

                foreach (var prop in classType.GetProperties())
                {
                    var attr = prop.GetCustomAttribute(_typeSQLAttr) as SQLColumn;
                    if (attr != null)
                        fClassList.Add((object)attr.ForeignKeyClass);
                }

                fClassDict.Add((object)className, fClassList);
            }

            return fClassDict;
        }

        private static List<string> GetAllClassNames(Dictionary<string, List<ExcelEnum>> excelEnumCollection)
        {
            var tempClassList = new List<string>();

            foreach (var entry in excelEnumCollection)
            {
                foreach (var excelEnum in entry.Value)
                {
                    foreach (var className in excelEnum.ClassNames)
                    {
                        tempClassList.Add(className);
                    }
                }
            }

            return tempClassList.Distinct().ToList();
        }
    }
}
