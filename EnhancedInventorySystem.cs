using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

// Marker Interface for Inventory Items
public interface IInventoryItem
{
    int Id { get; }
    string Name { get; }
    int Quantity { get; set; }
    string ItemType { get; }
}

// Custom Exceptions
public class DuplicateItemException : Exception
{
    public DuplicateItemException(string message) : base(message) { }
}

public class ItemNotFoundException : Exception
{
    public ItemNotFoundException(string message) : base(message) { }
}

public class InvalidQuantityException : Exception
{
    public InvalidQuantityException(string message) : base(message) { }
}

// Base Item Class for JSON Serialization
public abstract class InventoryItem : IInventoryItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public abstract string ItemType { get; }
}

// Product Classes
public class ElectronicItem : InventoryItem
{
    public string Brand { get; set; }
    public int WarrantyMonths { get; set; }
    
    [JsonIgnore]
    public override string ItemType => "Electronic";

    public ElectronicItem() { }

    public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        Brand = brand;
        WarrantyMonths = warrantyMonths;
    }

    public override string ToString()
    {
        return $"[ELECTRONIC] ID: {Id}, Name: {Name}, Quantity: {Quantity}, Brand: {Brand}, Warranty: {WarrantyMonths} months";
    }
}

public class GroceryItem : InventoryItem
{
    public DateTime ExpiryDate { get; set; }
    
    [JsonIgnore]
    public override string ItemType => "Grocery";

    public GroceryItem() { }

    public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        ExpiryDate = expiryDate;
    }

    public override string ToString()
    {
        return $"[GROCERY] ID: {Id}, Name: {Name}, Quantity: {Quantity}, Expiry: {ExpiryDate.ToShortDateString()}";
    }
}

public class ClothingItem : InventoryItem
{
    public string Size { get; set; }
    public string Color { get; set; }
    public string Material { get; set; }
    
    [JsonIgnore]
    public override string ItemType => "Clothing";

    public ClothingItem() { }

    public ClothingItem(int id, string name, int quantity, string size, string color, string material)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        Size = size;
        Color = color;
        Material = material;
    }

    public override string ToString()
    {
        return $"[CLOTHING] ID: {Id}, Name: {Name}, Quantity: {Quantity}, Size: {Size}, Color: {Color}, Material: {Material}";
    }
}

public class BookItem : InventoryItem
{
    public string ISBN { get; set; }
    public string Author { get; set; }
    public string Genre { get; set; }
    
    [JsonIgnore]
    public override string ItemType => "Book";

    public BookItem() { }

    public BookItem(int id, string name, int quantity, string isbn, string author, string genre)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        ISBN = isbn;
        Author = author;
        Genre = genre;
    }

    public override string ToString()
    {
        return $"[BOOK] ID: {Id}, Name: {Name}, Quantity: {Quantity}, ISBN: {ISBN}, Author: {Author}, Genre: {Genre}";
    }
}

// Transaction Log Entry
public class TransactionLog
{
    public DateTime Timestamp { get; set; }
    public string Action { get; set; }
    public string ItemType { get; set; }
    public int ItemId { get; set; }
    public string ItemName { get; set; }
    public int QuantityChange { get; set; }
    public int NewQuantity { get; set; }
    public string Details { get; set; }

    public override string ToString()
    {
        return $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Action} - {ItemType} ID:{ItemId} '{ItemName}' - Change: {QuantityChange}, New Qty: {NewQuantity} - {Details}";
    }
}

// Generic Inventory Repository with Persistence
public class InventoryRepository<T> where T : IInventoryItem
{
    private Dictionary<int, T> _items = new Dictionary<int, T>();
    private List<TransactionLog> _transactionLogs = new List<TransactionLog>();
    private string _dataFile;
    private string _logFile;

    public InventoryRepository(string dataFile, string logFile)
    {
        _dataFile = dataFile;
        _logFile = logFile;
        LoadData();
    }

    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id))
        {
            throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
        }
        _items.Add(item.Id, item);
        
        LogTransaction("ADD", item, item.Quantity, item.Quantity, $"Added new {item.ItemType}");
        SaveData();
    }

    public T GetItemById(int id)
    {
        if (!_items.TryGetValue(id, out T item))
        {
            throw new ItemNotFoundException($"Item with ID {id} not found.");
        }
        return item;
    }

    public void RemoveItem(int id)
    {
        if (!_items.TryGetValue(id, out T item))
        {
            throw new ItemNotFoundException($"Cannot remove. Item with ID {id} not found.");
        }
        
        _items.Remove(id);
        LogTransaction("REMOVE", item, -item.Quantity, 0, $"Removed {item.ItemType}");
        SaveData();
    }

    public List<T> GetAllItems()
    {
        return new List<T>(_items.Values);
    }

    public List<T> SearchItems(string searchTerm)
    {
        return _items.Values
            .Where(item => item.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public List<T> FilterByQuantityRange(int minQuantity, int maxQuantity)
    {
        return _items.Values
            .Where(item => item.Quantity >= minQuantity && item.Quantity <= maxQuantity)
            .ToList();
    }

    public void UpdateQuantity(int id, int newQuantity)
    {
        if (newQuantity < 0)
        {
            throw new InvalidQuantityException($"Quantity cannot be negative. Attempted to set {newQuantity}.");
        }

        var item = GetItemById(id);
        int oldQuantity = item.Quantity;
        item.Quantity = newQuantity;
        
        LogTransaction("UPDATE", item, newQuantity - oldQuantity, newQuantity, $"Updated quantity for {item.ItemType}");
        SaveData();
    }

    private void LogTransaction(string action, IInventoryItem item, int quantityChange, int newQuantity, string details)
    {
        _transactionLogs.Add(new TransactionLog
        {
            Timestamp = DateTime.Now,
            Action = action,
            ItemType = item.ItemType,
            ItemId = item.Id,
            ItemName = item.Name,
            QuantityChange = quantityChange,
            NewQuantity = newQuantity,
            Details = details
        });
        
        SaveTransactionLog();
    }

    private void SaveData()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(_items.Values.ToList(), options);
            File.WriteAllText(_dataFile, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving data: {ex.Message}");
        }
    }

    private void LoadData()
    {
        try
        {
            if (File.Exists(_dataFile))
            {
                string json = File.ReadAllText(_dataFile);
                var items = JsonSerializer.Deserialize<List<T>>(json);
                foreach (var item in items)
                {
                    _items[item.Id] = item;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading data: {ex.Message}");
        }
    }

    private void SaveTransactionLog()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(_transactionLogs, options);
            File.WriteAllText(_logFile, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving transaction log: {ex.Message}");
        }
    }

    public List<TransactionLog> GetTransactionLogs()
    {
        return new List<TransactionLog>(_transactionLogs);
    }

    public List<T> GetLowStockItems(int threshold)
    {
        return _items.Values.Where(item => item.Quantity <= threshold).ToList();
    }

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
