using ExcelToSQL.TableClasses;
using System;
using ExcelToSQL.ExcelClasses;
using ExcelToSQL.MySQLClasses;
using System.Linq;
using ExcelToSQL.TableClasses.for_the_king.Abilities;
using System.Collections.Generic;
using ExcelToSQL.TableClasses.for_the_king.Characters;
using TopologicalSorter;

namespace ExcelToSQL
{
    class Program
    {
        static void Main(string[] args)
        {
            var keyArray = new object[] { "two", "three", "five", "seven", "eight", "nine", "ten", "eleven" };

            var arraytest = new int[,] {
                {0,0,0,0,0,0,0,1},
                {0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0},
                {0,1,0,1,0,0,0,0},
                {0,0,0,0,1,0,0,1},
                {0,1,0,0,0,0,0,1},
                {0,0,1,1,0,0,0,0},
            };

            var final = TSorter.Sort(arraytest,keyArray,false);

            var config = new ExcelConfig("ExcelFilesSheetInfo.json");
            var abilsEx = new ExcelFile(config, "for_the_king", "FTKAbilitiesSQLTables.xlsx");

            var excelEnum = new ExcelEnum(abilsEx);

            var singleColTesting = excelEnum.CreateEnum<SingleColumn>("Passive", "Name");

            //var abilities = excelEnum.CreateEnum<Ability>();
            //var passives = excelEnum.CreateEnum<AbilitySCPassive>();
            //var actives = excelEnum.CreateEnum<AbilitySCActive>();
            var attacks = excelEnum.CreateAllEnum<AbilitySCAttack>();
            //var buffs = excelEnum.CreateEnum<AbilitySCBuff>();
            //var debuffs = excelEnum.CreateEnum<AbilitySCDebuff>();
            var subcategories = excelEnum.CreateAllEnum<AbilitySubcategory>();
            var scSingles = excelEnum.CreateAllSingleEnums<AbilitySubcategory>();
            var atSingles = excelEnum.CreateAllSingleEnums<AbilitySCAttack>();



            foreach (var col in singleColTesting)
            {
                Console.WriteLine(col.PulledValue);
            }
            Console.WriteLine();

            var sqlActor = new SQLActor("for_the_king");
            var filler = new TableObjectFiller(excelEnum, sqlActor);



            var testStrings = new List<string>();
            testStrings.Add("id");
            testStrings.Add("name");

            var sqlSelectTest = sqlActor.SimpleSelect("Characters", "Character", testStrings);

            foreach (var character in sqlSelectTest.Cast<Character>())
            {
                Console.WriteLine(character.ID + " - " + character.Name);
            }

            Console.WriteLine();

            // Command only prints. The actual execute is currently commented out.
            filler.SmartFillAll(ref attacks, ref atSingles);
            int num = 1;
            foreach (var item in attacks["Attack"])
            {
                Console.Write($"Action {num++}:\nTo MySQL => ");
                sqlActor.Insert(item);
                Console.WriteLine("\n------------------------------------------------\n");
            }


            var testConfig = new ExcelConfig("ExcelFilesSheetTesting.json");
            var testFile = new ExcelFile(testConfig, "for_the_king", "Testing.test");

            var build = new ExcelBuilder("for_the_king");
            build.AddConfig("ExcelFilesSheetInfo.json", "FTK Abilities")
                 .AddConfig(testConfig)
                 .AutoAddFiles("Abilities")
                 .AddFile(testFile, "Config 2")
                 .Build();

            var bob = new ExcelBuilder("for_the_king");
            bob.AddConfig(config)
               .AutoAddAllFiles()
               .Build();

            var groupBuild = new ExcelBuilder("for_the_king");
            groupBuild.AddConfig(testConfig, "Test Config")
                      .AddConfig("ExcelFilesSheetInfo.json")
                      .AutoAddAllFiles()
                      .Build();

            var groupActor = new GroupActor(sqlActor, groupBuild);

            Console.WriteLine("--------");
            groupActor.TestMethod();
            Console.WriteLine("--------");

            var builderList = new List<ExcelBuilder>();
            builderList.Add(build);
            builderList.Add(bob);

            foreach (var builder in builderList)
            {
                DisplayConfigsAndFiles(builder);
            }
        }

        public static void DisplayConfigsAndFiles(ExcelBuilder builder)
        {
            Console.WriteLine();
            var configNames = builder.GetAllConfigNames();

            foreach (var pair in configNames)
            {
                Console.WriteLine($"Nickname: {pair.Key} --- File name: {pair.Value}");
            }

            var fileNames = builder.GetAllFileNames();

            foreach (var pair in fileNames)
            {
                Console.WriteLine($"\nConfig Name: {pair.Key}");

                foreach (var file in pair.Value)
                {
                    Console.WriteLine($"     --File: {file}");
                }
            }
        }
    }
}

