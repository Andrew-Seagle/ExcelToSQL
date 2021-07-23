using ExcelToSQL.ExcelClasses;
using ExcelToSQL.MySQLClasses;
using ExcelToSQL.TableClasses;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ExcelToSQL
{
    class GroupActor
    {
        // TODO: Add logic for priority insertion
        private SQLActor _actor;
        private ExcelBuilder _builder;
        private string[] _orderArray;
        private List<Type> _classTypes;
        private string _databaseName;

        public Dictionary<string, List<ExcelEnum>> ExcelEnumCollection;

        public GroupActor(SQLActor actor, ExcelBuilder builder)
        {
            this._actor = actor;
            this._databaseName = _actor.DatabaseName;

            // TODO: Validate that builder is actually built
            this._builder = builder;
            this.ExcelEnumCollection = _builder.ExcelEnumCollection;

            var tempOrderArray = PrioritySorter.PrioritySort(_databaseName, ExcelEnumCollection);
            _orderArray = Array.ConvertAll(tempOrderArray, c => (string)c);
            //_classTypes = TableClassHelper.GroupTypeSearch(_orderArray.ToList(), _tableGroup, _databaseName);
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

            Console.WriteLine("++++++++++++++++++");

            foreach (var className in _orderArray)
            {
                Console.WriteLine(className);
            }

            Console.WriteLine("++++++++++++++++++");
        }
    }
}
