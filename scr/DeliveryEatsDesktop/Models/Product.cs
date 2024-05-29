namespace desktop.Models;

public class Product
{
    public int Id { get; set; }
    public CategoryProduct CategoryProduct { get; set; }
    public string Unit { get; set; }
    public int UnitValue { get; set; }
    public int AvailableQuantity { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public string? ImagePath { get; set; }
}