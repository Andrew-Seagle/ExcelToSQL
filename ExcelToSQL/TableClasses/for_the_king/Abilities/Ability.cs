using ExcelToSQL.CustomAttributes;
using System.Collections.Generic;

namespace ExcelToSQL.TableClasses.for_the_king.Abilities
{
    class Ability : IMySQLTable
    {
        public static List<string> SheetNames { get; set; }

        public string TableName { get => "ability"; }
        public int RowNumber { get; set; }

        [SQLColumn(Key.Primary)]
        public int ID { get; set; }

        [SQLColumn(Key.None)]
        [ExcelColumn(true)]
        public string Name { get; set; }

        [SQLColumn(Key.Foreign, "Abilities.AbilitySubcategory")]
        [ExcelColumn(true)]
        public int Subcategory_ID { get; set; }

    }
}
