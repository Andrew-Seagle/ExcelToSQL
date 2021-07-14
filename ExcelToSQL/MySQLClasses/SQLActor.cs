using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Data;
using ExcelToSQL.CustomAttributes;
using Dapper;
using ExcelToSQL.TableClasses;

namespace ExcelToSQL.SQLClasses
{
    public class SQLActor
    {
        private Type _typeAttr = typeof(SQLColumn);
        private SQLConnection _conn;

        public string DatabaseName { get; }

        public SQLActor(string connName, string sqlConfigFile = "SQLConfig.json")
        {
            _conn = new SQLConnection(sqlConfigFile, connName);
            DatabaseName = _conn.DatabaseName;
        }

        // TODO: Add other CRUD functions including custom select for priority insertion logic 

        public void Insert<T>(T item)
        {
            Type typeT = typeof(T);

            string tableName = (string)typeT.GetProperty("TableName").GetValue(item);
            IEnumerable<PropertyInfo> props = typeT
                .GetProperties()
                .Where(p => p.IsDefined(_typeAttr, false));
            List<Type> propTypes = props
                .Select(p => p.PropertyType)
                .ToList();
            List<string> propNames = props
                .Select(p => p.Name)
                .ToList();
            List<object> propValues = props
                .Zip(propTypes, (prop, type) => prop.GetSqlValue(item, type))
                .ToList();

            var sqlString = $"insert into {DatabaseName}.{tableName} " +
                            $"({ String.Join(", ", propNames)}) " +
                            $"values ({ String.Join(", ", propValues)});";

            Console.WriteLine(sqlString);

            // DO NOT TOUCH UNTIL ABSOLUTELY SURE
            //_conn.MySqlConnection.Execute(sqlstring);
        }

        public IEnumerable<IMySQLTable> SimpleSelect(string className, IEnumerable<string> columns = null)
        {
            var classFolder = TableClassHelper.GetClassFolder(className, DatabaseName);

            return SimpleSelect(classFolder, className, columns);
        }

        public IEnumerable<IMySQLTable> SimpleSelect(string classFolder, string className, IEnumerable<string> columns = null)
        {
            var typeT = Type.GetType($"{Pathing.TableNS}.{DatabaseName}.{classFolder}.{className}");
            var tempInst = Activator.CreateInstance(typeT);

            return SimpleSelect(tempInst, columns);
        }

        public IEnumerable<IMySQLTable> SimpleSelect<T>(T tempInstanceOfT, IEnumerable<string> columns = null)
        {
            var typeT = tempInstanceOfT.GetType();

            var tableName = (string)typeT.GetProperty("TableName")
                                         .GetValue(tempInstanceOfT);

            string columnNames = columns != null ? $"tn.{String.Join(", tn.", columns)}" : "*";

            var sqlString = $"select {columnNames} from {DatabaseName}.{tableName} as tn;";

            return _conn.MySqlConnection.Query(typeT, sqlString).Select(e => (IMySQLTable)e);
        }

        public IEnumerable<T> DapperTest<T>()
        {
            var sqlString = "select tn.id, tn.name from for_the_king.character as tn; ";

            var testEnum1 = _conn.MySqlConnection.Query<T>(sqlString);

            return testEnum1;
        }
    }
}
