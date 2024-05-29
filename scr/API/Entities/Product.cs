using System;
using System.Collections.Generic;

namespace api.Entities;

public partial class Product
{
    public int Id { get; set; }

    public int CategoryProductId { get; set; }

    public int? BrandId { get; set; }

    public int? PackagingId { get; set; }

    public int? PromotionId { get; set; }

    public int UnitId { get; set; }

    public int UnitValue { get; set; }

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

    public virtual Brand? Brand { get; set; }

    public virtual CategoryProduct CategoryProduct { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; } = new List<OrderItem>();

    public virtual Packaging? Packaging { get; set; }

    public virtual ICollection<ParameterProduct> ParameterProducts { get; } = new List<ParameterProduct>();

    public virtual ICollection<ProductImage> ProductImages { get; } = new List<ProductImage>();

    public virtual Promotion? Promotion { get; set; }

    public virtual Unit Unit { get; set; } = null!;
}
