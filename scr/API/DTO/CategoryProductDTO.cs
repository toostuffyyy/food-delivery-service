using System.Text.Json.Serialization;
using api.Entities;

namespace api.DTO;

public class CategoryProductDTO
{
    [JsonConstructor]
    public CategoryProductDTO() { }
    public CategoryProductDTO(CategoryProduct categoryProduct)
    {
        Id = categoryProduct.Id;
        ParentCategoryProductId = categoryProduct.ParentCategoryProductId;
        Name = categoryProduct.Name;
        ImagePath = categoryProduct.ImagePath;
    }
    public int Id { get; set; }
    public int? ParentCategoryProductId { get; set; }
    public string Name { get; set; } = null!;
    public string? ImagePath { get; set; }
}