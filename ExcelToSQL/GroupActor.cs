using ExcelToSQL.ExcelClasses;
using ExcelToSQL.SQLClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExcelToSQL
{
    class GroupActor
    {
        // TODO: Add logic for priority insertion
        private SQLActor _actor;

        public Dictionary<string, List<ExcelEnum>> ExcelEnumCollection;

        public GroupActor(SQLActor actor)
        {
            this._actor = actor;
        }

        public void GroupFill()
        {

        }

        public void TestMethod()
        {
            foreach (var configName in ExcelEnumCollection.Keys)
            {
                Console.WriteLine(configName);

                foreach (var exEnum in ExcelEnumCollection[configName])
                {
                    Console.WriteLine($"   --- {exEnum.FileName}");
                }
            }
        }
    }
}
