using ReactiveUI;

namespace DeliveryEatsClientMobile.Models;

public class Product : ReactiveObject
{
    public int Id { get; set; }
    public CategoryProduct CategoryProduct { get; set; }
    public string Unit { get; set; }
    public int UnitValue { get; set; }
    public int AvailableQuantity { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public string? ImagePath { get; set; }
    private int _count;
    public int Count
    {
        get => _count;
        set => this.RaiseAndSetIfChanged(ref _count, value);
    }
}