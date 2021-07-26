using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExcelToSQL.MySQLClasses
{
    public class Database
    {
        public string DbName { get; set; }
        public string ProjectName { get; set; }
        public string AssemblyFN { get; set; }
        public string TablesNamespace { get; set; }
        public IDbConnection MySqlConnection { get; set; }
        public SQLActor SQLActor { get; set; }

        public static void AddMe()
        {
            var assem = Assembly.GetCallingAssembly();
            var assemName = assem.FullName;
            Console.WriteLine(assemName);

            var filePath = @"C:\Programming\repos\ExcelToSQL\ExcelToSQL\ConfigFiles\MySQLDatabases.json";

            JObject videogameRatings = new JObject(
                new JProperty("Halo", 9),
                new JProperty("Starcraft", 9),
                new JProperty("Call of Duty", 7.5));

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
            {
                using (StreamReader reader = new StreamReader(fileStream))
                using (StreamWriter writer = new StreamWriter(fileStream))
                {
                    var fullJsonO = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
                    fullJsonO.Add("Testing2", videogameRatings);
                    fileStream.SetLength(0);
                    fullJsonO.WriteTo(new JsonTextWriter(writer));
                }
            }
            

            var textString = new StringBuilder(File.ReadAllText(@"C:\Programming\repos\ExcelToSQL\ExcelToSQL\ConfigFiles\MySQLDatabases.json"));

            var insertPos = textString.Length - 2;

            Formatter.JsonPrettyAppend(textString.ToString(), videogameRatings.ToString());

            textString.Insert(insertPos, $", \"Testing\": {videogameRatings}");

            var jsonString = JToken.Parse(textString.ToString()).ToString();

            File.WriteAllText(@"C:\Programming\repos\ExcelToSQL\ExcelToSQL\ConfigFiles\MySQLDatabases.json", jsonString);
        }
    }
}
