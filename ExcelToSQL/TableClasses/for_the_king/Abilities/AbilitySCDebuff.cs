using ExcelToSQL.CustomAttributes;
using ExcelToSQL.SQLClasses;
using System.Collections.Generic;

namespace ExcelToSQL.TableClasses.for_the_king.Abilities
{
    class AbilitySCDebuff : IMySQLTable
    {
        public static List<string> SheetNames { get; set; }

        public string TableName { get => "ability_subcategory_debuff_data"; }
        public int RowNumber { get; set; }

        [SQLColumn(Key.Primary)]
        public int ID { get; set; }

        [SQLColumn(Key.Foreign, "Ability")]
        public int Ability_ID { get; set; }

        [SQLColumn(Key.Foreign, "Stat")]
        public int Stat_ID { get; set; }

        [SQLColumn(Key.None)]
        public int Rolls { get; set; }

        [SQLColumn(Key.None)]
        public int Stat_Modifier { get; set; }

        [SQLColumn(Key.Foreign, "Target")]
        public int Target_ID { get; set; }
    }
}
