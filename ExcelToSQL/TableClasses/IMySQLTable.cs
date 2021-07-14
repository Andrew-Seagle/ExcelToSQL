namespace ExcelToSQL.TableClasses
{
    public interface IMySQLTable
    {
        public string TableName { get; }
        public int RowNumber { get; set; }
        public int ID { get; set; }
        
    }
}
