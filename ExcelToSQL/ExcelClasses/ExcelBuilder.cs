using ExcelToSQL.TableClasses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExcelToSQL.ExcelClasses
{
    class ExcelBuilder
    {
        private Dictionary<string, ExcelConfig> _configNicknames;
        private Dictionary<ExcelConfig, List<ExcelFile>> _fileCollection;
        private Dictionary<string, List<ExcelEnum>> _excelEnumCollection;
        private string _databaseName;
        private string _defaultConfig;
        private IEnumerable<Tuple<IEnumerable<IMySQLTable>, IEnumerable<SingleColumn>>> _classPairs;

        public ExcelBuilder(string databaseName)
        {
            _databaseName = databaseName;
            _fileCollection = new Dictionary<ExcelConfig, List<ExcelFile>>();
            _configNicknames = new Dictionary<string, ExcelConfig>();
            _excelEnumCollection = new Dictionary<string, List<ExcelEnum>>();
        }

        public Dictionary<string,string> GetAllConfigNames()
        {
            var configNames = new Dictionary<string, string>();

            foreach (var pair in _configNicknames)
            {
                configNames.Add(pair.Key, pair.Value.ConfigSheetFile);
            }

            return configNames;
        }

        public Dictionary<string, List<string>> GetAllFileNames()
        {
            var fileNames = new Dictionary<string, List<string>>();

            foreach (var pair in _configNicknames)
            {
                var fileList = _fileCollection[pair.Value]
                    .Select(f => f.FileName)
                    .ToList();

                fileNames.Add(pair.Key, fileList);
            }

            return fileNames;
        }

        public ExcelBuilder AddConfig(string configFile)
        {
            var config = new ExcelConfig(configFile);

            AddConfig(config);

            return this;
        }

        public ExcelBuilder AddConfig(ExcelConfig config)
        {
            string lazyName = "Config ";
            lazyName += (_fileCollection.Count + 1).ToString();

            AddConfig(config, lazyName);

            return this;
        }

        public ExcelBuilder AddConfig(string configFile, string configNickname)
        {
            var config = new ExcelConfig(configFile);

            AddConfig(config, configNickname);

            return this;
        }

        public ExcelBuilder AddConfig(ExcelConfig config, string configNickname)
        {
            var files = new List<ExcelFile>();

            _fileCollection.Add(config, files);
            _configNicknames.Add(configNickname, config);

            if (_fileCollection.Count == 1)
                _defaultConfig = configNickname;

            return this;
        }

        public ExcelBuilder ChangeDefaultConfig(string configNickname)
        {
            // TODO: Check validity
            _defaultConfig = configNickname;

            return this;
        }

        public ExcelBuilder AddFile(ExcelFile file)
        {
            AddFile(file, _defaultConfig);

            return this;
        }

        public ExcelBuilder AddFile(string excelFile)
        {

            AddFile(excelFile, _defaultConfig);

            return this;
        }
        
        public ExcelBuilder AddFile(string excelFile, ExcelConfig config)
        {
            if (!excelFile.Contains('.'))
                excelFile += Pathing.ExcelExten;
            
            var file = new ExcelFile(config, _databaseName, excelFile);

            AddFile(file, config);

            return this;
        }

        public ExcelBuilder AddFile(string excelFile, string configName)
        {
            var config = FindConfig(configName);

            AddFile(excelFile, config);

            return this;
        }

        public ExcelBuilder AddFile(ExcelFile file, string configName)
        {
            var config = FindConfig(configName);

            AddFile(file, config);

            return this;
        }

        public ExcelBuilder AddFile(ExcelFile file, ExcelConfig config)
        {
            if (IsFileAlreadyAdded(file, config))
                throw new Exception("Cannot assign a file twice to " +
                    "the same config sheet in an excel builder.");

            _fileCollection[config].Add(file);

            return this;
        }

        public ExcelBuilder AutoAddFiles(string tableGroup)
        {
            AutoAddFiles(tableGroup, _defaultConfig);

            return this;
        }

        public ExcelBuilder AutoAddFiles(string tableGroup, string configName)
        {
            var config = FindConfig(configName);

            AutoAddFiles(tableGroup, config);

            return this;
        }

        public ExcelBuilder AutoAddFiles(string tableGroup, ExcelConfig config)
        {
            var groupsWithFiles = config.DbaseGroupFiles[_databaseName];
            var files = groupsWithFiles[tableGroup];

            foreach (string file in files)
            {
                AddFile(file, config);
            }

            return this;
        }

        public ExcelBuilder AutoAddAllFiles()
        {
            foreach (var config in _fileCollection.Keys)
            {
                var groups = config.DbaseGroupFiles[_databaseName].Keys;

                foreach (var group in groups)
                {
                    AutoAddFiles(group, config);
                }
            }

            return this;
        }

        public void Build(GroupActor groupActor)
        {
            // TODO: Add final validation?
            CreateExcelEnumsForFiles();

        }

        private void CreateExcelEnumsForFiles()
        {
            foreach (var config in _fileCollection.Keys)
            {
                var configName = _configNicknames.Single(c => c.Value.Equals(config)).Key;
                var excelEnums = CreateExcelEnums(_fileCollection[config]);

                _excelEnumCollection.Add(configName, excelEnums);
            }
        }

        private List<ExcelEnum> CreateExcelEnums(List<ExcelFile> excelFiles)
        {
            var excelEnums = new List<ExcelEnum>();

            foreach (var file in excelFiles)
            {
                excelEnums.Add(new ExcelEnum(file));
            }

            return excelEnums;
        }

        // TODO: Figure out if this is needed
        private bool IsFileAlreadyAdded(string fileName, string configName)
        {
            var config = FindConfig(configName);
            var files = _fileCollection[config].Select(f => f.FileName);

            return files.Contains(fileName);
        }

        private bool IsFileAlreadyAdded(ExcelFile file, ExcelConfig config)
        {
            return _fileCollection[config].Contains(file);
        }

        private ExcelConfig FindConfig(string configName)
        {
            var keys = _fileCollection.Keys;
            ExcelConfig config;

            if (_configNicknames.ContainsKey(configName))
                config = _configNicknames[configName];
            else if (keys.Any(k => k.ConfigSheetFile == configName))
                config = keys.Single(k => k.ConfigSheetFile == configName);
            else
                throw new Exception(); // TODO: Fill out exception

            return config;
        }
    }
}
