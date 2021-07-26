using Microsoft.Extensions.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

namespace ExcelToSQL.MySQLClasses
{
    public class SQLConnection
    {
        private IConfigurationRoot _config;
        private string _connString;

        public string DatabaseName { get; }
        public IDbConnection MySqlConnection { get; set; }

        public SQLConnection(string jsonFile, string connectionName)
        {
            _config = new ConfigurationBuilder()
                      .SetBasePath(Pathing.ConfigPath)
                      .AddJsonFile(jsonFile)
                      .Build();

            _connString = _config.GetConnectionString(connectionName);
            System.Console.WriteLine(_connString);
            MySqlConnection = new MySqlConnection(_connString);
            DatabaseName = MySqlConnection.Database;
        }
    }
}
