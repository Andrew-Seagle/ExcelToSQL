using ExcelToSQL.CustomAttributes;
using System;
using System.Collections.Generic;

namespace ExcelToSQL.TableClasses.for_the_king.Stats
{
    class Stat : IMySQLTable
    {
        public static List<string> SheetNames { get; set; }

        public string TableName { get => "stat"; }
        public int RowNumber { get; set; }

        [SQLColumn(Key.Primary)]
        public int ID { get; set; }

        [SQLColumn(Key.None)]
        [ExcelColumn(true)]
        public string Name { get; set; }
    }
}
