using ExcelToSQL.CustomAttributes;
using ExcelToSQL.MySQLClasses;
using System.Collections.Generic;

namespace ExcelToSQL.TableClasses.for_the_king.Abilities
{
    class AbilitySCActive
    {
        public static List<string> SheetNames { get; set; }

        public string TableName { get => "ability_subcategory_active_data"; }
        public int RowNumber { get; set; }

        [SQLColumn(Key.Primary)]
        public int ID { get; set; }

        [SQLColumn(Key.Foreign, "Abilities.Ability")]
        public int Ability_ID { get; set; }

        [SQLColumn(Key.Foreign, "Stats.Stat")]
        public int Stat_ID { get; set; }

        [SQLColumn(Key.None)]
        public int Rolls { get; set; }

        [SQLColumn(Key.None)]
        public string Effect { get; set; }
    }
}
