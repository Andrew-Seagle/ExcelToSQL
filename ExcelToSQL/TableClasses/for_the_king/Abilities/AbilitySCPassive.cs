using ExcelToSQL.CustomAttributes;
using ExcelToSQL.MySQLClasses;
using System.Collections.Generic;

namespace ExcelToSQL.TableClasses.for_the_king.Abilities
{
    class AbilitySCPassive : IMySQLTable
    {
        public static List<string> SheetNames { get; set; }

        public string TableName { get => "ability_subcategory_passive_data"; }
        public int RowNumber { get; set; }

        [SQLColumn(Key.Primary)]
        public int ID { get; set; }

        [SQLColumn(Key.Foreign, "Abilities.Ability")]
        public int Ability_ID { get; set; }

        [SQLColumn(Key.None)]
        public string Proc_Chance { get; set; }

        [SQLColumn(Key.None)]
        public string Effect { get; set; }
    }
}
