using api.Entities;

namespace api.DTO;

public class ProductDetailsDTO
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
    public CategoryProductDTO CategoryProduct { get; set; } = null!;
    public PromotionDTO? Promotion { get; set; }
    public List<ParameterProductDTO> ParameterProducts { get; set; } = new ();
    public List<ProductImageDTO> ProductImages { get; set; } = new ();
    
    public ProductDetailsDTO(Product product)
    {
        Id = product.Id;
        CategoryProductId = product.CategoryProductId;
        BrandId = product.BrandId;
        PackagingId = product.PackagingId;
        PromotionId = product.PromotionId;
        UnitValue = product.UnitValue;
        Unit = product.Unit.Name;
        Name = product.Name;
        ExpirationDate = product.ExpirationDate;
        AvailableQuantity = product.AvailableQuantity;
        Price = product.Price;
        Proteins = product.Proteins;
        Fats = product.Fats;
        Carbohydrates = product.Carbohydrates;
        Kcal = product.Kcal;
        StorageConditions = product.StorageConditions;
        Composition = product.Composition;
        Description = product.Description;
        Brand = product.Brand?.Name;
        Packaging = product.Packaging?.Name;
        CategoryProduct = new CategoryProductDTO(product.CategoryProduct);
        Promotion = product.Promotion != null ? new PromotionDTO(product.Promotion) : null;
        ParameterProducts = product.ParameterProducts.ToList().ConvertAll(a => new ParameterProductDTO(a));
        ProductImages = product.ProductImages.Any()
            ? product.ProductImages.ToList().ConvertAll(a => new ProductImageDTO(a))
            : [new ProductImageDTO(new ProductImage { ProductId = product.Id, ImagePath = null })];

    }
}