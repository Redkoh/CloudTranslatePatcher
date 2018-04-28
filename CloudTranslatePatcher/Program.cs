using System;
using System.Collections.Generic;
using System.IO;

namespace CloudTranslatePatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var projectPath = @"E:\Bazis\salonportal\app.cloud\";

            List<string> dirs = GetDirictories(new string[] { projectPath });
            Console.WriteLine("*******************************************************************");
            Console.WriteLine("The number of directories is {0}.", dirs.Count);

            List<string> files = new List<string>();
            foreach (string dir in dirs)
            {
                files.AddRange(Directory.GetFiles(dir, "ru.ts"));
            }

            Console.WriteLine("*******************************************************************");
            Console.WriteLine("The number of files is {0}.", files.Count);

            string resFilePath = Path.Combine(projectPath, "resRU.json");
            WriteJSONToFile(resFilePath, files);

            Console.WriteLine("File " + resFilePath + " created");

            Console.ReadKey();
        }

        static List<string> GetDirictories(string[] paths)
        {
            List<string> i18n = new List<string>();

            foreach (var item in paths)
            {
                Console.WriteLine("check " + item);

                if (Directory.GetDirectories(item).Length > 0)
                {
                    i18n.AddRange(Directory.GetDirectories(item, "i18n"));

                    i18n.AddRange(GetDirictories(Directory.GetDirectories(item)));
                }
            }

            return i18n;
        }

        static void WriteJSONToFile(string resultPath, List<string> files)
        {
            if (files == null)
            {
                throw new ArgumentNullException(nameof(files));
            }

            if (!File.Exists(resultPath))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(resultPath))
                {
                    foreach (var item in files)
                    {
                        // Open the file to read from.
                        using (StreamReader sr = File.OpenText(item))
                        {
                            int i = 0;
                            string s = "";
                            List<string> json = new List<string>();
                            while ((s = sr.ReadLine()) != null)
                            {
                                if (i > 2)
                                    json.Add(s);

                                i++;
                            }

                            short closeCount = 0;
                            for (int j = json.Count - 1; j >= 0; j--)
                            {
                                if (json[j].IndexOf('}') != -1)
                                    closeCount++;

                                if (closeCount < 3)
                                    json.RemoveAt(j);
                                else
                                {
                                    json[json.Count - 1] += ',';
                                    break;
                                }
                            }

                            sw.WriteLine("//**" + item);
                            foreach (String str in json)
                                sw.WriteLine(str);
                        }
                    }
                }
            }            
        }
    }
}
