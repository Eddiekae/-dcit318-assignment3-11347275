using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GradingSystem
{
    public class StudentResultProcessor
    {
        public List<Student> ReadStudentsFromFile(string inputFilePath)
        {
            List<Student> students = new List<Student>();

            try
            {
                using (StreamReader reader = new StreamReader(inputFilePath))
                {
                    string line;
                    int lineNumber = 0;

                    while ((line = reader.ReadLine()) != null)
                    {
                        lineNumber++;
                        line = line.Trim();

                        if (string.IsNullOrEmpty(line))
                            continue;

                        string[] fields = line.Split(',');

                        if (fields.Length != 3)
                        {
                            throw new MissingFieldException(
                                $"Line {lineNumber}: Expected 3 fields (ID, FullName, Score) but found {fields.Length}");
                        }

                        string idStr = fields[0].Trim();
                        string fullName = fields[1].Trim();
                        string scoreStr = fields[2].Trim();

                        if (string.IsNullOrEmpty(idStr) || string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(scoreStr))
                        {
                            throw new MissingFieldException(
                                $"Line {lineNumber}: Missing required field(s)");
                        }

                        if (!int.TryParse(idStr, out int id))
                        {
                            throw new InvalidScoreFormatException(
                                $"Line {lineNumber}: Invalid ID format '{idStr}'. ID must be an integer.");
                        }

                        if (!int.TryParse(scoreStr, out int score))
                        {
                            throw new InvalidScoreFormatException(
                                $"Line {lineNumber}: Invalid score format '{scoreStr}'. Score must be an integer.");
                        }

                        if (score < 0 || score > 100)
                        {
                            throw new InvalidScoreFormatException(
                                $"Line {lineNumber}: Score {score} is out of valid range (0-100)");
                        }

                        students.Add(new Student
                        {
                            Id = id,
                            FullName = fullName,
                            Score = score
                        });
                    }
                }
            }
            catch (FileNotFoundException)
            {
                throw;
            }
            catch (IOException ex)
            {
                throw new Exception($"Error reading file: {ex.Message}", ex);
            }

            return students;
        }

        public void WriteReportToFile(List<Student> students, string outputFilePath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(outputFilePath))
                {
                    writer.WriteLine("STUDENT GRADING REPORT");
                    writer.WriteLine("======================");
                    writer.WriteLine($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    writer.WriteLine($"Total Students: {students.Count}");
                    writer.WriteLine();

                    foreach (Student student in students)
                    {
                        string grade = student.GetGrade();
                        writer.WriteLine($"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {grade}");
                    }

                    writer.WriteLine();
                    writer.WriteLine("GRADE SUMMARY");
                    writer.WriteLine("=============");

                    int aCount = students.Count(s => s.GetGrade() == "A");
                    int bCount = students.Count(s => s.GetGrade() == "B");
                    int cCount = students.Count(s => s.GetGrade() == "C");
                    int dCount = students.Count(s => s.GetGrade() == "D");
                    int fCount = students.Count(s => s.GetGrade() == "F");

                    writer.WriteLine($"Grade A: {aCount} students");
                    writer.WriteLine($"Grade B: {bCount} students");
                    writer.WriteLine($"Grade C: {cCount} students");
                    writer.WriteLine($"Grade D: {dCount} students");
                    writer.WriteLine($"Grade F: {fCount} students");
                }
            }
            catch (IOException ex)
            {
                throw new Exception($"Error writing report: {ex.Message}", ex);
            }
        }
    }
}
