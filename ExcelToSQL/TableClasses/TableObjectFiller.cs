using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using ExcelToSQL.MySQLClasses;
using ExcelToSQL.ExcelClasses;
using ExcelToSQL.CustomAttributes;

namespace ExcelToSQL.TableClasses
{
    class TableObjectFiller
    {
        private static Type _typeSQLAttr = typeof(SQLColumn);
        private static Type _typeExcelAttr = typeof(ExcelColumn);

        private ExcelEnum _excelFile;
        private SQLActor _actor;

        public TableObjectFiller(ExcelEnum excelFile, SQLActor actor)
        {
            this._excelFile = excelFile;
            this._actor = actor;
        }

        public void Fill<T>(string propertyName, ref T mainObject, ref SingleColumn fillObject)
        {
            var prop = typeof(T).GetProperty(propertyName);
            var value = fillObject.PushedValue;
            prop.SetValue(mainObject, value);
        }

        public void Fill<T>(string propertyName, ref IEnumerable<T> mainObjects, ref IEnumerable<SingleColumn> fillObjects)
        {
            var objList = mainObjects.ToList();

            foreach (T tObj in mainObjects)
            {
                SingleColumn fillObject = fillObjects.Single(o => o.RowNumber == (tObj as IMySQLTable).RowNumber);
                T mainObject = mainObjects.Single(o => o.Equals(tObj));
                Fill(propertyName,ref mainObject, ref fillObject);
            }
        }

        public void SmartFillAll<T>(ref Dictionary<string, IEnumerable<T>> mainObjects, ref Dictionary<string, Dictionary<string, IEnumerable<SingleColumn>>> singleColumns)
        {
            foreach (var sheet in singleColumns.Keys)
            {
                var objects = mainObjects[sheet];
                var singles = singleColumns[sheet];

                SmartFill(ref objects, ref singles);
            }
        }    

        public void SmartFill<T>(ref IEnumerable<T> mainObjects, ref Dictionary<string, IEnumerable<SingleColumn>> singleColumns)
        {
            var typeT = typeof(T);
            T tempInst = (T)Activator.CreateInstance(typeT);

            var missingColumns = new Dictionary<string, IEnumerable<SingleColumn>>();
            var foreignColumns = new Dictionary<string, IEnumerable<SingleColumn>>();

            foreach (var prop in singleColumns.Keys)
            {
                var attr = typeT.GetProperty(prop).GetCustomAttribute(_typeSQLAttr);
                if ((attr as SQLColumn).KeyType == Key.Foreign)
                    foreignColumns.Add(prop, singleColumns[prop]);
                else
                    missingColumns.Add(prop, singleColumns[prop]);
            }

            FillMissingColumns(ref mainObjects, ref missingColumns);
            FillForeignClass(ref mainObjects, ref foreignColumns);
        }

        public void FillMissingColumns<T>(ref IEnumerable<T> mainObjects, ref Dictionary<string, IEnumerable<SingleColumn>> singleColumns)
        {
            foreach (string prop in singleColumns.Keys)
            {
                var fillObjects = singleColumns[prop];
                Fill(prop, ref mainObjects, ref fillObjects);
            }
        }

        public void FillForeignClass<T>(ref IEnumerable<T> mainObjects, ref Dictionary<string, IEnumerable<SingleColumn>> singleColumns)
        {
            var foreignClasses = GetForeignClasses<T>(singleColumns.Keys);

            var foreignEnums = CreateForeignFillEnums<T>(foreignClasses);

            foreach (var propName in singleColumns.Keys)
            {
                var columns = singleColumns[propName];
                ReplaceSingleColumnData(foreignEnums[propName].Item2, foreignEnums[propName].Item1, ref columns);
            }

            FillMissingColumns(ref mainObjects, ref singleColumns);
        }

        private void ReplaceSingleColumnData<T>(string propertyName, IEnumerable<T> foreignFill, ref IEnumerable<SingleColumn> singleColumns)
        {
            foreach (T fill in foreignFill)
            {
                var fillPrimaryKey = GetPrimaryKey(fill);
                var fillPrimaryValue = fill.GetType().GetProperty(fillPrimaryKey).GetValue(fill);
                var fillValue = fill.GetType().GetProperty(propertyName).GetValue(fill);
                var columns = singleColumns.Where(c => c.PulledValue.Equals(fillValue));

                foreach (var col in columns)
                {
                    col.PushedValue = fillPrimaryValue;
                }
            }
        }

        private Dictionary<string, Tuple<string, string>> GetForeignClasses<T>(IEnumerable<string> propertyNames)
        {
            var foreignClasses = new Dictionary<string, Tuple<string, string>>();

            foreach (string propName in propertyNames)
            {
                var prop = typeof(T).GetProperty(propName);
                var attr = prop.GetCustomAttribute(_typeSQLAttr) as SQLColumn;
                var classInfo = new Tuple<string, string>(attr.ForeignKeyClass, attr.ForeignClassValue);

                foreignClasses.Add(propName, classInfo);
            }

            return foreignClasses;
        }

        private Dictionary<string, Tuple<IEnumerable<IMySQLTable>, string>> CreateForeignFillEnums<T>(Dictionary<string, Tuple<string, string>> foreignClasses)
        {
            T tempInst = (T)Activator.CreateInstance(typeof(T));

            var foreignEnums = new Dictionary<string, Tuple<IEnumerable<IMySQLTable>, string>>();
            var primaryKey = GetPrimaryKey(tempInst);

            foreach (var prop in foreignClasses)
            {
                IEnumerable<string> columns = new List<string>() { primaryKey, prop.Value.Item2 };
                IEnumerable<IMySQLTable> sqlSelect;

                if (TableClassHelper.TableClassExists(prop.Value.Item1, _excelFile.TableGroup, _actor.DatabaseName))
                {
                    sqlSelect = _actor.SimpleSelect(_excelFile.TableGroup, prop.Value.Item1, columns);
                }
                else
                {
                    sqlSelect = _actor.SimpleSelect(prop.Value.Item1, columns);
                }

                var foreignPair = new Tuple<IEnumerable<IMySQLTable>, string>(sqlSelect, prop.Value.Item2);

                foreignEnums.Add(prop.Key, foreignPair);
            }

            return foreignEnums;
        }

        private string GetPrimaryKey<T>(T obj)
        {
            var primaryKeyName = obj
                    .GetType()
                    .GetProperties()
                    .Single(p => p.IsDefined(_typeSQLAttr, false)
                             && (p.GetCustomAttribute(_typeSQLAttr) as SQLColumn)
                                  .KeyType == Key.Primary)
                    .Name;

            return primaryKeyName;
        }
    }
}
