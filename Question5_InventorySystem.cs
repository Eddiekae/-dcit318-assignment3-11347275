using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

// Marker Interface for Inventory Entities
public interface IInventoryEntity
{
    int Id { get; }
}

// Immutable Inventory Item Record
public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

// Generic Inventory Logger for managing inventory items
public class InventoryLogger<T> where T : IInventoryEntity
{
    private List<T> _log = new List<T>();
    private string _filePath;

    public InventoryLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Add(T item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));
        
        _log.Add(item);
    }

    public List<T> GetAll()
    {
        return new List<T>(_log);
    }

    public void SaveToFile()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(_log, options);
            File.WriteAllText(_filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving to file: {ex.Message}");
        }
    }

    public void LoadFromFile()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine($"File {_filePath} not found. Starting with empty inventory.");
                return;
            }

            string json = File.ReadAllText(_filePath);
            if (!string.IsNullOrEmpty(json))
            {
                var items = JsonSerializer.Deserialize<List<T>>(json);
                if (items != null)
                {
                    _log = items;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading from file: {ex.Message}");
        }
    }
}

// Integration Layer - Inventory Application
public class InventoryApp
{
    private InventoryLogger<InventoryItem> _logger;

    public InventoryApp(string filePath)
    {
        _logger = new InventoryLogger<InventoryItem>(filePath);
    }

    public void SeedSampleData()
    {
        Console.WriteLine("Seeding sample inventory data...");
        
        var items = new List<InventoryItem>
        {
            new InventoryItem(1, "Laptop Computer", 10, DateTime.Now.AddDays(-30)),
            new InventoryItem(2, "Wireless Mouse", 25, DateTime.Now.AddDays(-15)),
            new InventoryItem(3, "USB-C Cable", 50, DateTime.Now.AddDays(-7)),
            new InventoryItem(4, "Mechanical Keyboard", 8, DateTime.Now.AddDays(-45)),
            new InventoryItem(5, "Monitor Stand", 15, DateTime.Now.AddDays(-3))
        };

        foreach (var item in items)
        {
            _logger.Add(item);
        }
        
        Console.WriteLine($"Added {items.Count} sample items to inventory.");
    }

    public void SaveData()
    {
        Console.WriteLine("Saving inventory data to file...");
        _logger.SaveToFile();
        Console.WriteLine("Data saved successfully.");
    }

    public void LoadData()
    {
        Console.WriteLine("Loading inventory data from file...");
        _logger.LoadFromFile();
        Console.WriteLine("Data loaded successfully.");
    }

    public void PrintAllItems()
    {
        var items = _logger.GetAll();
        
        if (items.Count == 0)
        {
            Console.WriteLine("No items found in inventory.");
            return;
        }

        Console.WriteLine("\n=== Current Inventory ===");
        Console.WriteLine($"Total Items: {items.Count}");
        Console.WriteLine(new string('-', 80));
        
        foreach (var item in items)
        {
            Console.WriteLine($"ID: {item.Id,-5} | Name: {item.Name,-25} | Quantity: {item.Quantity,4} | Added: {item.DateAdded:yyyy-MM-dd}");
        }
        
        Console.WriteLine(new string('-', 80));
    }

    public int GetItemCount()
    {
        return _logger.GetAll().Count;
    }
}

// Main Program Entry Point
public class Question5_InventoryProgram
{
    public static void Main(string[] args)
    {
        Console.WriteLine("=== Question 5: Immutable Inventory Management System ===");
        Console.WriteLine("This system demonstrates C# records, generics, and file operations.");
        Console.WriteLine();
        
        string inventoryFile = "question5_inventory.json";
        var app = new InventoryApp(inventoryFile);
        
        Console.WriteLine("Step 1: Seeding sample data...");
        app.SeedSampleData();
        
        Console.WriteLine("\nStep 2: Displaying initial data...");
        app.PrintAllItems();
        
        Console.WriteLine("Step 3: Saving data to file...");
        app.SaveData();
        
        Console.WriteLine("\nStep 4: Clearing memory (simulating new session)...");
        app = new InventoryApp(inventoryFile);
        
        Console.WriteLine("Step 5: Loading data from file...");
        app.LoadData();
        
        Console.WriteLine("Step 6: Displaying recovered data...");
        app.PrintAllItems();
        
        Console.WriteLine("\n=== System Test Complete ===");
        Console.WriteLine($"Final item count: {app.GetItemCount()}");
        
        // Wait for user input before closing
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
