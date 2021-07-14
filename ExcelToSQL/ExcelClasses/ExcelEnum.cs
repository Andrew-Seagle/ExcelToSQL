using ExcelToEnumerable;
using ExcelToSQL.CustomAttributes;
using ExcelToSQL.TableClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExcelToSQL.ExcelClasses
{
    class ExcelEnum
    {
        private static Type _typeSQLAttr = typeof(SQLColumn);
        private static Type _typeExcelAttr = typeof(ExcelColumn);

        private ExcelFile _file { get; }

        public string FileName { get; }
        public string TableGroup { get; }

        public ExcelEnum(ExcelFile file)
        {
            this._file = file;
            FileName = _file.FileName;
            TableGroup = _file.TableGroup;
        }

        public IEnumerable<T> CreateEnum<T>(string sheetName, string columnName = "") where T : new()
        {
            var enumerable = _file.FilePath
                    .ExcelToEnumerable<T>(x => x.UsingSheet(sheetName)
                                                .Property(p => (p as IMySQLTable).TableName)
                                                .Ignore()
                                                .Property(p => (p as SingleColumn).PulledValue)
                                                .UsesColumnNamed(columnName)
                                                .Property(p => (p as IMySQLTable).RowNumber)
                                                .MapsToRowNumber()
                                                .IgnoreColumsWithoutMatchingProperties()
                                                .AllPropertiesOptionalByDefault());

            return enumerable;
        }

        public Dictionary<string, IEnumerable<T>> CreateAllEnum<T>() where T : new()
        {
            var type = typeof(T);
            T tempInst = (T)Activator.CreateInstance(type);

            var sheetListProp = type.GetProperty("SheetNames");
            var sheetNames = sheetListProp.GetValue(tempInst) as List<string>;

            sheetNames = sheetNames.Intersect(_file.SheetNames).ToList();
            var allEnumerables = new Dictionary<string, IEnumerable<T>>();

            foreach(string sheet in sheetNames)
            {
                var tempEnumerable = CreateEnum<T>(sheet);
                allEnumerables.Add(sheet, tempEnumerable);
            }

            return allEnumerables;
        }

        public Dictionary<string, Dictionary<string, IEnumerable<SingleColumn>>> CreateAllSingleEnums<T>()
        {
            var typeT = typeof(T);
            T tempInst = (T)Activator.CreateInstance(typeT);

            var sheetListProp = typeT.GetProperty("SheetNames");
            var sheetNames = sheetListProp.GetValue(tempInst);

            var propNames = typeT
                .GetProperties()
                .Where(p => p.IsDefined(_typeSQLAttr, false)
                         && p.IsDefined(_typeExcelAttr, false)
                         && !(p.GetCustomAttribute(_typeExcelAttr) as ExcelColumn)
                               .UseCurrentData)
                .Select(p => p.Name);

            var allSingleColumns = new Dictionary<string, Dictionary<string, IEnumerable<SingleColumn>>>();

            foreach (string sheet in sheetNames as List<string>)
            {
                var singleColumns = CreateSingleEnums<T>(sheet, propNames);
                allSingleColumns.Add(sheet, singleColumns);
            }

            return allSingleColumns;
        }

        public Dictionary<string, IEnumerable<SingleColumn>> CreateSingleEnums<T>(string sheetName, IEnumerable<string> propNames)
        {
            var singleColumns = new Dictionary<string, IEnumerable<SingleColumn>>();

            foreach (var prop in propNames)
            {
                var tempCol = (typeof(T).GetProperty(prop).GetCustomAttribute(_typeExcelAttr) as ExcelColumn).ExcelColumnName;
                tempCol = tempCol.Length == 0 ? prop : tempCol;
                var tempEnum = CreateEnum<SingleColumn>(sheetName, tempCol);
                singleColumns.Add(prop, tempEnum);
            }

            return singleColumns;
        }
    }
}
