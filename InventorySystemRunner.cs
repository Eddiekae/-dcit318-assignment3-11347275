using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

// Complete Enhanced Inventory System with Main Program
public class InventorySystemProgram
{
    private static InventoryRepository<ElectronicItem> electronicRepo;
    private static InventoryRepository<GroceryItem> groceryRepo;
    private static InventoryRepository<ClothingItem> clothingRepo;
    private static InventoryRepository<BookItem> bookRepo;

    public static void Main(string[] args)
    {
        Console.WriteLine("Enhanced Inventory Management System");
        Console.WriteLine("====================================");
        
        // Initialize repositories
        electronicRepo = new InventoryRepository<ElectronicItem>("electronics.json", "electronics_log.json");
        groceryRepo = new InventoryRepository<GroceryItem>("grocery.json", "grocery_log.json");
        clothingRepo = new InventoryRepository<ClothingItem>("clothing.json", "clothing_log.json");
        bookRepo = new InventoryRepository<BookItem>("books.json", "books_log.json");
        
        // Seed sample data
        SeedSampleData();
        
        // Main menu
        while (true)
        {
            DisplayMenu();
            var choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    AddItemMenu();
                    break;
                case "2":
                    ViewAllItems();
                    break;
                case "3":
                    SearchItems();
                    break;
                case "4":
                    UpdateQuantity();
                    break;
                case "5":
                    ViewLowStock();
                    break;
                case "6":
                    ViewExpiringItems();
                    break;
                case "7":
                    ViewTransactionLogs();
                    break;
                case "8":
                    RemoveItem();
                    break;
                case "0":
                    Console.WriteLine("Thank you for using Enhanced Inventory System!");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    static void DisplayMenu()
    {
        Console.WriteLine("\n--- Main Menu ---");
        Console.WriteLine("1. Add New Item");
        Console.WriteLine("2. View All Items");
        Console.WriteLine("3. Search Items");
        Console.WriteLine("4. Update Item Quantity");
        Console.WriteLine("5. View Low Stock Items");
        Console.WriteLine("6. View Expiring Items (Grocery)");
        Console.WriteLine("7. View Transaction Logs");
        Console.WriteLine("8. Remove Item");
        Console.WriteLine("0. Exit");
        Console.Write("Enter your choice: ");
    }

    static void SeedSampleData()
    {
        try
        {
            // Add sample electronics
            electronicRepo.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 12));
            electronicRepo.AddItem(new ElectronicItem(2, "Smartphone", 25, "Samsung", 24));
            electronicRepo.AddItem(new ElectronicItem(3, "Headphones", 15, "Sony", 6));
            
            // Add sample grocery items
            groceryRepo.AddItem(new GroceryItem(1, "Milk", 50, DateTime.Now.AddDays(7)));
            groceryRepo.AddItem(new GroceryItem(2, "Bread", 30, DateTime.Now.AddDays(3)));
            groceryRepo.AddItem(new GroceryItem(3, "Eggs", 100, DateTime.Now.AddDays(14)));
            
            // Add sample clothing
            clothingRepo.AddItem(new ClothingItem(1, "T-Shirt", 20, "M", "Blue", "Cotton"));
            clothingRepo.AddItem(new ClothingItem(2, "Jeans", 15, "L", "Black", "Denim"));
            clothingRepo.AddItem(new ClothingItem(3, "Jacket", 8, "XL", "Red", "Leather"));
            
            // Add sample books
            bookRepo.AddItem(new BookItem(1, "C# Programming", 12, "978-1234567890", "John Smith", "Programming"));
            bookRepo.AddItem(new BookItem(2, "Data Structures", 8, "978-0987654321", "Jane Doe", "Computer Science"));
            bookRepo.AddItem(new BookItem(3, "Algorithms", 5, "978-1122334455", "Bob Johnson", "Computer Science"));
            
            Console.WriteLine("Sample data loaded successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding data: {ex.Message}");
        }
    }

    static void AddItemMenu()
    {
        Console.WriteLine("\n--- Add New Item ---");
        Console.WriteLine("1. Electronic Item");
        Console.WriteLine("2. Grocery Item");
        Console.WriteLine("3. Clothing Item");
        Console.WriteLine("4. Book Item");
        Console.Write("Select item type: ");
        
        var typeChoice = Console.ReadLine();
        
        try
        {
            Console.Write("Enter ID: ");
            int id = int.Parse(Console.ReadLine());
            
            Console.Write("Enter Name: ");
            string name = Console.ReadLine();
            
            Console.Write("Enter Quantity: ");
            int quantity = int.Parse(Console.ReadLine());
            
            switch (typeChoice)
            {
                case "1":
                    Console.Write("Enter Brand: ");
                    string brand = Console.ReadLine();
                    Console.Write("Enter Warranty (months): ");
                    int warranty = int.Parse(Console.ReadLine());
                    electronicRepo.AddItem(new ElectronicItem(id, name, quantity, brand, warranty));
                    break;
                case "2":
                    Console.Write("Enter Expiry Date (yyyy-MM-dd): ");
                    DateTime expiry = DateTime.Parse(Console.ReadLine());
                    groceryRepo.AddItem(new GroceryItem(id, name, quantity, expiry));
                    break;
                case "3":
                    Console.Write("Enter Size: ");
                    string size = Console.ReadLine();
                    Console.Write("Enter Color: ");
                    string color = Console.ReadLine();
                    Console.Write("Enter Material: ");
                    string material = Console.ReadLine();
                    clothingRepo.AddItem(new ClothingItem(id, name, quantity, size, color, material));
                    break;
                case "4":
                    Console.Write("Enter ISBN: ");
                    string isbn = Console.ReadLine();
                    Console.Write("Enter Author: ");
                    string author = Console.ReadLine();
                    Console.Write("Enter Genre: ");
                    string genre = Console.ReadLine();
                    bookRepo.AddItem(new BookItem(id, name, quantity, isbn, author, genre));
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    return;
            }
            
            Console.WriteLine("Item added successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding item: {ex.Message}");
        }
    }

    static void ViewAllItems()
    {
        Console.WriteLine("\n--- All Items ---");
        
        Console.WriteLine("\nElectronics:");
        foreach (var item in electronicRepo.GetAllItems())
        {
            Console.WriteLine(item);
        }
        
        Console.WriteLine("\nGrocery:");
        foreach (var item in groceryRepo.GetAllItems())
        {
            Console.WriteLine(item);
        }
        
        Console.WriteLine("\nClothing:");
        foreach (var item in clothingRepo.GetAllItems())
        {
            Console.WriteLine(item);
        }
        
        Console.WriteLine("\nBooks:");
        foreach (var item in bookRepo.GetAllItems())
        {
            Console.WriteLine(item);
        }
    }

    static void SearchItems()
    {
        Console.Write("Enter search term: ");
        string searchTerm = Console.ReadLine();
        
        Console.WriteLine("\n--- Search Results ---");
        
        var electronics = electronicRepo.SearchItems(searchTerm);
        var grocery = groceryRepo.SearchItems(searchTerm);
        var clothing = clothingRepo.SearchItems(searchTerm);
        var books = bookRepo.SearchItems(searchTerm);
        
        if (electronics.Count + grocery.Count + clothing.Count + books.Count == 0)
        {
            Console.WriteLine("No items found.");
            return;
        }
        
        DisplaySearchResults("Electronics", electronics);
        DisplaySearchResults("Grocery", grocery);
        DisplaySearchResults("Clothing", clothing);
        DisplaySearchResults("Books", books);
    }

    static void DisplaySearchResults<T>(string category, List<T> items) where T : IInventoryItem
    {
        if (items.Count > 0)
        {
            Console.WriteLine($"\n{category}:");
            foreach (var item in items)
            {
                Console.WriteLine(item);
            }
        }
    }

    static void UpdateQuantity()
    {
        Console.WriteLine("\n--- Update Quantity ---");
        Console.WriteLine("1. Electronic");
        Console.WriteLine("2. Grocery");
        Console.WriteLine("3. Clothing");
        Console.WriteLine("4. Book");
        Console.Write("Select item type: ");
        
        var typeChoice = Console.ReadLine();
        InventoryRepository<IInventoryItem> repo = null;
        
        switch (typeChoice)
        {
            case "1": repo = (InventoryRepository<IInventoryItem>)(object)electronicRepo; break;
            case "2": repo = (InventoryRepository<IInventoryItem>)(object)groceryRepo; break;
            case "3": repo = (InventoryRepository<IInventoryItem>)(object)clothingRepo; break;
            case "4": repo = (InventoryRepository<IInventoryItem>)(object)bookRepo; break;
            default: Console.WriteLine("Invalid choice."); return;
        }
        
        try
        {
            Console.Write("Enter Item ID: ");
            int id = int.Parse(Console.ReadLine());
            Console.Write("Enter New Quantity: ");
            int newQuantity = int.Parse(Console.ReadLine());
            
            switch (typeChoice)
            {
                case "1": electronicRepo.UpdateQuantity(id, newQuantity); break;
                case "2": groceryRepo.UpdateQuantity(id, newQuantity); break;
                case "3": clothingRepo.UpdateQuantity(id, newQuantity); break;
                case "4": bookRepo.UpdateQuantity(id, newQuantity); break;
            }
            
            Console.WriteLine("Quantity updated successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating quantity: {ex.Message}");
        }
    }

    static void ViewLowStock()
    {
        Console.Write("Enter low stock threshold: ");
        int threshold = int.Parse(Console.ReadLine());
        
        Console.WriteLine("\n--- Low Stock Items ---");
        
        var lowStockElectronics = electronicRepo.GetLowStockItems(threshold);
        var lowStockGrocery = groceryRepo.GetLowStockItems(threshold);
        var lowStockClothing = clothingRepo.GetLowStockItems(threshold);
        var lowStockBooks = bookRepo.GetLowStockItems(threshold);
        
        DisplaySearchResults("Electronics", lowStockElectronics);
        DisplaySearchResults("Grocery", lowStockGrocery);
        DisplaySearchResults("Clothing", lowStockClothing);
        DisplaySearchResults("Books", lowStockBooks);
    }

    static void ViewExpiringItems()
    {
        Console.Write("Enter days ahead to check: ");
        int days = int.Parse(Console.ReadLine());
        
        Console.WriteLine("\n--- Expiring Grocery Items ---");
        var expiring = groceryRepo.GetExpiringItems(days);
        
        if (expiring.Count == 0)
        {
            Console.WriteLine("No expiring items found.");
            return;
        }
        
        foreach (var item in expiring)
        {
            Console.WriteLine(item);
        }
    }

    static void ViewTransactionLogs()
    {
        Console.WriteLine("\n--- Transaction Logs ---");
        
        Console.WriteLine("\nElectronics:");
        foreach (var log in electronicRepo.GetTransactionLogs())
        {
            Console.WriteLine(log);
        }
        
        Console.WriteLine("\nGrocery:");
        foreach (var log in groceryRepo.GetTransactionLogs())
        {
            Console.WriteLine(log);
        }
        
        Console.WriteLine("\nClothing:");
        foreach (var log in clothingRepo.GetTransactionLogs())
        {
            Console.WriteLine(log);
        }
        
        Console.WriteLine("\nBooks:");
        foreach (var log in bookRepo.GetTransactionLogs())
        {
            Console.WriteLine(log);
        }
    }

    static void RemoveItem()
    {
        Console.WriteLine("\n--- Remove Item ---");
        Console.WriteLine("1. Electronic");
        Console.WriteLine("2. Grocery");
        Console.WriteLine("3. Clothing");
        Console.WriteLine("4. Book");
        Console.Write("Select item type: ");
        
        var typeChoice = Console.ReadLine();
        
        try
        {
            Console.Write("Enter Item ID: ");
            int id = int.Parse(Console.ReadLine());
            
            switch (typeChoice)
            {
                case "1": electronicRepo.RemoveItem(id); break;
                case "2": groceryRepo.RemoveItem(id); break;
                case "3": clothingRepo.RemoveItem(id); break;
                case "4": bookRepo.RemoveItem(id); break;
                default: Console.WriteLine("Invalid choice."); return;
            }
            
            Console.WriteLine("Item removed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing item: {ex.Message}");
        }
    }
}

// Complete the missing method from EnhancedInventorySystem.cs
public partial class InventoryRepository<T> where T : IInventoryItem
{
    public List<GroceryItem> GetExpiringItems(int daysAhead)
    {
        if (typeof(T) != typeof(GroceryItem))
            return new List<GroceryItem>();

        DateTime cutoffDate = DateTime.Now.AddDays(daysAhead);
        
        if (_items.Values is IEnumerable<GroceryItem> groceryItems)
        {
            return groceryItems.Where(item => item.ExpiryDate <= cutoffDate).ToList();
        }
        
        return new List<GroceryItem>();
    }
}
