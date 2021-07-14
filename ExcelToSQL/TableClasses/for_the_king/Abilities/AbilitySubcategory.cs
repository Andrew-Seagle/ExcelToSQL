using ExcelToSQL.CustomAttributes;
using System.Collections.Generic;

namespace ExcelToSQL.TableClasses.for_the_king.Abilities
{
    class AbilitySubcategory : IMySQLTable
    {
        public static List<string> SheetNames { get; set; }

        public string TableName { get => "ability_subcategory"; }
        public int RowNumber { get; set; }

        [SQLColumn(Key.Primary)]
        public int ID { get; set; }

        [SQLColumn(Key.None)]
        public string Name { get; set; }

        [SQLColumn(Key.Foreign, "AbilityCategory")]
        [ExcelColumn(false, "Category")]
        public int Category_ID { get; set; }
    }
}
