using System.Text.Json.Serialization;
using api.Entities;

namespace api.DTO;

public class ProductDTO
{
    [JsonConstructor]
    public ProductDTO() { }
    public ProductDTO(Product product)
    {
        Id = product.Id;
        Name = product.Name;
        Price = product.Price;
        Unit = product.Unit.Name;
        AvailableQuantity = product.AvailableQuantity;
        UnitValue = product.UnitValue;
        ImagePath = product.ProductImages.ToList().Count() > 0 ? product.ProductImages.ToList()[0].ImagePath : null;
        CategoryProduct = new CategoryProductDTO(product.CategoryProduct);
    }
    
    public int Id { get; set; }
    public string Unit { get; set; }
    public int UnitValue { get; set; }
    public int AvailableQuantity { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public string? ImagePath { get; set; }
    public CategoryProductDTO CategoryProduct { get; set; }
}