namespace api.DTO;

public class ProductStatisticDTO
{
    public ProductDTO Product { get; set; }
    public int Count { get; set; }
    public decimal Sum { get; set; }

    public ProductStatisticDTO(ProductDTO productDto, int count, decimal sum)
    {
        Product = productDto;
        Count = count;
        Sum = sum;
    }
}