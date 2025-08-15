using System;
using System.IO;

namespace GradingSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("School Grading System");
            Console.WriteLine("====================");

            try
            {
                string inputFilePath = "students.txt";
                string outputFilePath = "grading_report.txt";

                // Check if input file exists
                if (!File.Exists(inputFilePath))
                {
                    Console.WriteLine($"Creating sample input file: {inputFilePath}");
                    CreateSampleInputFile(inputFilePath);
                    Console.WriteLine("Sample file created. Please edit it with your student data and run again.");
                    return;
                }

                StudentResultProcessor processor = new StudentResultProcessor();

                Console.WriteLine($"Reading student data from: {inputFilePath}");
                var students = processor.ReadStudentsFromFile(inputFilePath);

                Console.WriteLine($"Processing {students.Count} students...");
                processor.WriteReportToFile(students, outputFilePath);

                Console.WriteLine($"Grading report generated successfully: {outputFilePath}");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Error: Input file not found - {ex.Message}");
            }
            catch (InvalidScoreFormatException ex)
            {
                Console.WriteLine($"Error: Invalid score format - {ex.Message}");
            }
            catch (MissingFieldException ex)
            {
                Console.WriteLine($"Error: Missing field in student record - {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: An unexpected error occurred - {ex.Message}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static void CreateSampleInputFile(string filePath)
        {
            string[] sampleData = {
                "101,Alice Johnson,85",
                "102,Bob Smith,72",
                "103,Carol Davis,91",
                "104,David Wilson,67",
                "105,Eve Brown,45",
                "106,Frank Miller,78",
                "107,Grace Lee,95",
                "108,Henry Taylor,58",
                "109,Ivy Clark,83",
                "110,Jack Robinson,76"
            };

            File.WriteAllLines(filePath, sampleData);
            Console.WriteLine($"Sample file created with {sampleData.Length} student records.");
        }
    }
}
