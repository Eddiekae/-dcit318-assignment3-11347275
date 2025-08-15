using System;
using Question5InventorySystem;

public class Q5_TestRunner
{
    public static void Main(string[] args)
    {
        Console.WriteLine("=== Question 5: Immutable Inventory System Test ===");
        Console.WriteLine("Testing the new inventory system with C# records and file operations...");
        Console.WriteLine();
        
        // Run the Question5 inventory system
        Q5Program.Main(args);
    }
}
