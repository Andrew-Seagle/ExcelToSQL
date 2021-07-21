using System;
using System.Collections.Generic;
using System.Linq;

namespace ExcelToSQL.TableClasses
{
    static class TableClassHelper
    {
        public static void SetSheetNames(string databaseName, string classFolder, Dictionary<string, string[]> sheetClasses)
        {
            foreach (KeyValuePair<string, string[]> pair in sheetClasses)
            {
                foreach (string className in pair.Value)
                {
                    var type = Type.GetType($"{Pathing.TableNS}.{databaseName}.{classFolder}.{className}");
                    var obj = Activator.CreateInstance(type);
                    var property = type.GetProperty("SheetNames");

                    var currentList = property.GetValue(obj) ?? new List<string>();
                    property.PropertyType
                            .GetMethod("Add")
                            .Invoke(currentList, new[] { pair.Key });

                    property.SetValue(obj, currentList);
                }
            }
        }

        // TODO: Validate if classes of same name exist >>>

        public static bool TableClassExists(string className, string classFolder, string databaseName, bool throwException = false)
        {
            var type = Type.GetType($"{Pathing.TableNS}.{databaseName}.{classFolder}.{className}");
            var exists = type != null;

            if (throwException && !exists)
                throw new Exception(); // TODO: Fill out exception

            return exists;
        }

        // TODO: Validate before single check
        public static Type IntensiveTypeSearch(string className, string databaseName = "", string interfaceName = "IMySQLTable")
        {
            var interfaceType = Type.GetType($"{Pathing.TableNS}.{interfaceName}");

            var classTypes = interfaceType.Assembly.GetTypes();
            var classType = classTypes.Where(t => t.FullName != null)
                                      .Where(t => t.FullName.Contains(databaseName))
                                      .Where(t => t.FullName.Contains(className))
                                      .OrderBy(t => t.FullName.Substring(t.FullName.LastIndexOf('.') + 1).Length)
                                      .First();

            return classType;
        }

        public static string GetClassFolder(string className, string databaseName = "", string interfaceName = "IMySQLTable")
        {
            var type = IntensiveTypeSearch(className, databaseName, interfaceName);
            var classNS = type.Namespace;
            var classFolder = classNS.Substring(classNS.LastIndexOf('.') + 1);

            //TableClassExists(className, classFolder, databaseName, true);

            return classFolder;
        }
    }
}
