using System;

namespace ExcelToSQL.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ExcelColumn : Attribute
    {
        public string ExcelColumnName { get; }
        public bool UseCurrentData { get; }

        public ExcelColumn(bool useCurrentData, string columnName = null)
        {
            this.UseCurrentData = useCurrentData;
            this.ExcelColumnName = columnName;
        }
    }
}
