using System;

namespace ExcelToSQL.CustomAttributes
{
    public enum Key
    {
        Primary,
        Foreign,
        None
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class SQLColumn : Attribute
    {
        public Key KeyType { get; }
        public string ForeignKeyClass { get; }
        public string ForeignClassValue { get; }

        public SQLColumn(Key keyType, string foreignClass = "", string valueUsedFromForeignClass = "Name") 
        {
            if (keyType != Key.Foreign && foreignClass.Length > 0)
                throw new ArgumentException(
                    "A column cannot have a foreign key class name " +
                    "without being a foreign key.");

            if (keyType == Key.Foreign && foreignClass.Length == 0)
                throw new ArgumentException(
                    "A column designated as a foreign key must have " +
                    "a foreign key class name.");

            // TODO: Complete validation
            //if (foreignClass.Length > 0)

            this.KeyType = keyType;
            this.ForeignKeyClass = foreignClass;
            this.ForeignClassValue = valueUsedFromForeignClass;
        }

        // TODO: Add logic needed for priority insertion
    }
}
