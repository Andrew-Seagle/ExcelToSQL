using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelToSQL.TableClasses
{
    class TableGroup
    {
        public string Database { get; set; }
        public string Name { get; set; }
        public Dictionary<string, List<string>> SheetsAndClasses { get; set; }
    }
}
