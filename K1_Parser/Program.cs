using System;
using System.IO;

namespace K1_Parser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var exit = false;
            do
            {
                Console.WriteLine("K-1 Parser Program starting...");

                string filePath = "";

                do
                {
                    Console.WriteLine("\nPlease specify a K-1 PDF file path: ");

                    filePath = Console.ReadLine();

                    if (!File.Exists(filePath) || Path.GetExtension(filePath) != ".pdf")
                    {
                        Console.WriteLine($"\n{filePath} is not valid! Please try again...");
                    }
                    else
                        break;
                } while (true);


                Parser parser = new Parser();
                parser.LoadPDF(filePath);

                Console.WriteLine("\nK-1 Parser Program finished successfully!");
                Console.WriteLine("\nWould you like to parse another file?");
                Console.WriteLine("\nEnter Y for yes or N for no: ");

                var input = Console.ReadLine();
                if (input == "Y" || input == "y")
                    exit = false;
                if (input == "N" || input == "n")
                    exit = true;
            }
            while (!exit);
        }
    }
}
