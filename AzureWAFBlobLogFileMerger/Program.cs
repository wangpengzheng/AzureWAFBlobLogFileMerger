using System;
using System.IO;
using System.Text;

namespace AzureWAFBlobLogFileMerger
{
    class Program
    {
        static void Main(string[] args)
        {
        input: Console.WriteLine("Please input your working folder path. This program will only iterate json files.");
            string inputPath = Console.ReadLine();

            if (!Directory.Exists(inputPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Your input path : {inputPath} not found, please enter again.");
                Console.ResetColor();
                goto input;
            }

            try
            {
                AppendAndCopyFiles appendAndCopyFiles = new AppendAndCopyFiles(inputPath);
                var outputPath = appendAndCopyFiles.StartMigrate();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Finished, your migrated file path is :{outputPath}");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message} found.");
            }
            Console.ResetColor();
            Console.ReadLine();
        }
    }

    public class AppendAndCopyFiles
    {
        public string RootPath { get; set; }
        public string[] fileNamesWithPath;

        public AppendAndCopyFiles(string rootPath)
        {
            RootPath = rootPath;
        }

        public string StartMigrate()
        {
            string retFilePath = "";
            string[] files = Directory.GetFiles(RootPath, "*.json", SearchOption.AllDirectories);

            if (files.Length != 0)
            {
                retFilePath = RootPath + $@"\WholeJson-{Guid.NewGuid()}.json";

                Console.WriteLine($"Create file {retFilePath}");
                File.Create(retFilePath).Close();

                using (StreamWriter sw = new StreamWriter(retFilePath, true, Encoding.UTF8))
                {
                    sw.Write("[");

                    foreach (var file in files)
                    {
                        Console.WriteLine($"Reading from {file}");

                        string[] contents = File.ReadAllLines(file);
                        foreach (string line in contents)
                        {
                            var newline = line.Replace("}}", "}},");
                            sw.WriteLine(newline);
                        }
                    }

                    sw.Write("]");
                }
            }

            return retFilePath;
        }
    }
}
