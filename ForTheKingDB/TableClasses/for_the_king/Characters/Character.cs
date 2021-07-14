using ExcelToSQL.CustomAttributes;
using System.Collections.Generic;

namespace ExcelToSQL.TableClasses.for_the_king.Characters
{
    class Character : IMySQLTable
    {
        public static List<string> SheetNames { get; set; }

        public string TableName { get => "character"; }
        public int RowNumber { get; set; }

        [SQLColumn(Key.Primary)]
        public int ID { get; set; }

        [SQLColumn(Key.None)]
        public string Name { get; set; }
    }
}
