using ExcelToSQL.CustomAttributes;
using ExcelToSQL.ExcelClasses;
using ExcelToSQL.TableClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TopologicalSorter;

namespace ExcelToSQL.MySQLClasses
{
    public static class PrioritySorter
    {
        internal static string[] PrioritySort(string databaseName, Dictionary<string, List<ExcelEnum>> excelEnumCollection)
        {
            var classList = GetAllClassNames(excelEnumCollection);
            var foreignClassDict = GetAllForeignClasses(databaseName, classList);
            var convertedFClassDict = DictConvertForSort(foreignClassDict);

            var sortedArray = TSorter.Sort(convertedFClassDict, false);

            return Array.ConvertAll(sortedArray, c => (string)c);
        }

        private static Dictionary<Type, List<string>> GetAllForeignClasses(string databaseName, List<Tuple<string, string>> classList)
        {
            var fClassDict = new Dictionary<Type, List<string>>();
            var _typeSQLAttr = typeof(SQLColumn);

            var classTypes = new List<Type>();

            foreach (var pair in classList)
            {
                var classType = Type.GetType($"{Pathing.TableNS}.{databaseName}.{pair.Item1}.{pair.Item2}");
                classTypes.Add(classType);
            }

            foreach (var classType in classTypes)
            {
                var fClassList = new List<string>();

                foreach (var prop in classType.GetProperties())
                {
                    var attr = prop.GetCustomAttribute(_typeSQLAttr) as SQLColumn;
                    if (attr != null && attr.ForeignKeyClass.Any())
                        fClassList.Add(attr.ForeignKeyClass);
                }

                fClassDict.Add(classType, fClassList);
            }

            return fClassDict;
        }

        private static Dictionary<object, List<object>> DictConvertForSort(Dictionary<Type, List<string>> oldDict)
        {
            var newDict = new Dictionary<object, List<object>>();

            foreach (var entry in oldDict)
            {
                var oldKeyNS = entry.Key.Namespace;
                var newKeyTGroup = oldKeyNS.Substring(oldKeyNS.LastIndexOf('.') + 1);

                var newKey = $"{newKeyTGroup}.{entry.Key.Name}";
                var newValue = new List<object>();

                foreach (var className in entry.Value)
                {
                    newValue.Add((object)className);
                }

                newDict.Add(newKey, newValue);
            }

            return newDict;
        }

        private static List<Tuple<string, string>> GetAllClassNames(Dictionary<string, List<ExcelEnum>> excelEnumCollection)
        {
            var tempClassList = new List<Tuple<string, string>>();

            foreach (var entry in excelEnumCollection)
            {
                foreach (var excelEnum in entry.Value)
                {
                    var tableGroup = excelEnum.TableGroup;

                    foreach (var className in excelEnum.ClassNames)
                    {
                        tempClassList.Add(new Tuple<string, string>(tableGroup, className));
                    }
                }
            }

            return tempClassList.Distinct().ToList();
        }
    }
}
