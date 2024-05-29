using api.Entities;

namespace api.DTO;

public class ProductImageDTO
{
    public int ProductId { get; set; }
    public string ImagePath { get; set; } = null!;

    public ProductImageDTO(ProductImage productImage)
    {
        ProductId = productImage.ProductId;
        ImagePath = productImage.ImagePath;
    }
}