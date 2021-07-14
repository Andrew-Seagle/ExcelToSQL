using System;

namespace ExcelToSQL.TableClasses
{
    class SingleColumn : IMySQLTable
    {
        public object PulledValue { get; set; }
        public object PushedValue { get; set; }

        public string TableName { get => throw new Exception(); }
        public int RowNumber { get; set; }

        public int ID { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
