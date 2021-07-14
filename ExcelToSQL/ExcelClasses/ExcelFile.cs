using ExcelToSQL.TableClasses;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExcelToSQL.ExcelClasses
{
    class ExcelFile
    {
        private JArray _fileSheetData;
        private JObject _fileJObject;
        private Dictionary<string, string[]> _sheetClasses;
        private ExcelConfig _config;
        private string _dbName;
        private IEnumerable<JProperty> _jProperties;

        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string TableGroup { get; set; }
        public List<string> SheetNames { get; set; }

        //TODO: Validate that file exists!
        //TODO: Validate proper config if being added manually
        public ExcelFile(ExcelConfig excelConfig, string dbName, string file)
        {
            _config = excelConfig;
            _dbName = dbName;

            FilePath = _config.FolderPath + file;
            FileName = file.Remove(file.LastIndexOf('.'));

            SetJsonVariables();
            ExtractSheetsAndClasses();
            CheckGroupName();

            TableClassHelper.SetSheetNames(dbName, TableGroup, _sheetClasses);
        }

        private void SetJsonVariables()
        {
            var jTokens = _config.FullJsonO[_dbName]
                .Where(p => (p as JProperty)
                    .Value["Files"]
                    .Select(i => i.ToString())
                    .Contains(FileName));

            _jProperties = jTokens.Cast<JProperty>();
            _fileJObject = new JObject(_jProperties);
            TableGroup = (_jProperties.First()).Name;
            _fileSheetData = (JArray)_fileJObject[TableGroup]["Sheets"];
        }

        private void ExtractSheetsAndClasses()
        {
            SheetNames = _fileSheetData.Select(s => (string)s["SheetName"]).ToList();

            _sheetClasses = new Dictionary<string, string[]>();
            foreach (string sheet in SheetNames)
            {
                _sheetClasses.Add(sheet, _fileSheetData
                                        .Where(s => (string)s["SheetName"] == sheet)
                                        .Select(s => s["Classes"])
                                        .Children()
                                        .Select(s => (string)s)
                                        .ToArray());
            }
        }

        private void CheckGroupName()
        {
            bool sheetMatch = SheetNames.Select(s => s.ToLower())
                                        .Contains(TableGroup.ToLower());
            bool classMatch = _sheetClasses.SelectMany(s => s.Value)
                                           .Select(s => s.ToLower())
                                           .Contains(TableGroup.ToLower());

            if (sheetMatch || classMatch)
            {
                string whatMatched = sheetMatch ? "sheet" : "class";

                throw new NotSupportedException($"Group name '{TableGroup}' " +
                    $"cannot be the same as a {whatMatched} name used in that " +
                    $"group. Please edit config file: {_config.SheetInfoPath}");
            }
        }
    }
}
