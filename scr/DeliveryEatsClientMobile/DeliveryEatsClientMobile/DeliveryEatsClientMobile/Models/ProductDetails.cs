using System.Collections.Generic;
using ReactiveUI;

namespace DeliveryEatsClientMobile.Models;

public class ProductDetails : ReactiveObject
{
    public int Id { get; set; }
    public int CategoryProductId { get; set; }
    public int? BrandId { get; set; }
    public int? PackagingId { get; set; }
    public int? PromotionId { get; set; }
    public int UnitValue { get; set; }
    public string Unit { get; set; }
    public string Name { get; set; } = null!;
    public int? ExpirationDate { get; set; }
    public int AvailableQuantity { get; set; }
    public decimal Price { get; set; }
    public double? Proteins { get; set; }
    public double? Fats { get; set; }
    public double? Carbohydrates { get; set; }
    public double? Kcal { get; set; }
    public string? StorageConditions { get; set; }
    public string? Composition { get; set; }
    public string? Description { get; set; }
    public string? Brand { get; set; }
    public string? Packaging { get; set; }
    public CategoryProduct CategoryProduct { get; set; } = null!;
    public Promotion? Promotion { get; set; }
    public List<ParameterProduct> ParameterProducts { get; set; } = new ();
    public List<ProductImage> ProductImages { get; set; } = new ();
    private int _count;
    public int Count
    {
        get => _count;
        set => this.RaiseAndSetIfChanged(ref _count, value);
    }
}