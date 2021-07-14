using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace ExcelToSQL.ExcelClasses
{
    class ExcelConfig
    {
        public JObject FullJsonO { get; }
        public string FolderPath { get; }
        public string SheetInfoPath { get; }
        public string ConfigSheetFile { get; }
        public List<string> DatabaseNames { get; }
        public Dictionary<string,Dictionary<string,List<string>>> DbaseGroupFiles { get; }

        public ExcelConfig(string configSheet)
        {
            ConfigSheetFile = configSheet;
            FolderPath = Pathing.BasePath + "ExcelFiles/";
            SheetInfoPath = Pathing.ConfigPath + ConfigSheetFile;

            StreamReader reader = File.OpenText(SheetInfoPath);
            FullJsonO = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
            DatabaseNames = FullJsonO.Properties().Select(j => j.Name).ToList();
            DbaseGroupFiles = new Dictionary<string, Dictionary<string, List<string>>>();

            SetPropNamesFromConfig();
        }

        private void SetPropNamesFromConfig()
        {
            foreach (var name in DatabaseNames)
            {
                var groupsWithFiles = new Dictionary<string, List<string>>();

                var groupList = FullJsonO[name]
                    .ToObject<JObject>()
                    .Properties()
                    .Select(j => j.Name)
                    .ToList();

                foreach (string group in groupList)
                {
                    var fileList = FullJsonO[name][group]["Files"]
                        .ToObject<JArray>()
                        .Select(s => s.ToString())
                        .ToList();

                    groupsWithFiles.Add(group, fileList);
                }

                DbaseGroupFiles.Add(name, groupsWithFiles);
            }
        }
    }
}
