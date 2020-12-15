using Client.Classes;
using System;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            ReadAccess(ApplicationConfigurations.PathLog);
        }

        public static void ReadAccess(string path)
        {
            string[] lines = System.IO.File.ReadAllLines(path);

            foreach (string line in lines)
            {
                // Use a tab to indent each line of the file.
                Console.WriteLine("\t" + line);
            }

        }
    }
}
