namespace api.DTO;

public class ProductCollectionDTO
{
    public IEnumerable<ProductDTO> Product { get; set; }
    public int Count { get; set; }

    public ProductCollectionDTO(IEnumerable<ProductDTO> product, int count)
    {
        Product = product;
        Count = count;
    }
}