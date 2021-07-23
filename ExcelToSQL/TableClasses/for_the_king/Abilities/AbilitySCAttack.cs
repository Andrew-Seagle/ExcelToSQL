using ExcelToSQL.CustomAttributes;
using System.Collections.Generic;

namespace ExcelToSQL.TableClasses.for_the_king.Abilities
{
    class AbilitySCAttack : IMySQLTable
    {
        public static List<string> SheetNames { get; set; }

        public string TableName { get => "ability_subcategory_attack_data"; }
        public int RowNumber { get; set; }

        [SQLColumn(Key.Primary)]
        public int ID { get; set; }

        [SQLColumn(Key.Foreign, "Abilities.Ability")]
        [ExcelColumn(false, "Name")]
        public int Ability_ID { get; set; }

        [SQLColumn(Key.Foreign, "Stats.Stat")]
        [ExcelColumn(false, "Stat")]
        public int Stat_ID { get; set; }

        [SQLColumn(Key.None)]
        [ExcelColumn(true)]
        public int Rolls { get; set; }

        [SQLColumn(Key.None)]
        [ExcelColumn(true)]
        public int Stat_Modifier { get; set; }

        [SQLColumn(Key.None)]
        [ExcelColumn(true)]
        public double Damage_Modifier { get; set; }

        [SQLColumn(Key.None)]
        [ExcelColumn(true)]
        public string Damage_Type { get; set; }

        [SQLColumn(Key.Foreign, "Abilities.AbilityTarget")]
        [ExcelColumn(false, "Target")]
        public int Target_ID { get; set; }
    }
}
