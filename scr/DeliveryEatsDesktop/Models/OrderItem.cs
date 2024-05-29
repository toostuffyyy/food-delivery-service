using desktop.Models;

namespace api.Models.DTO;

public class OrderItem
{
    public double Quantity { get; set; }
    public decimal SumProduct => (decimal)Quantity * Product.Price;
    public Product Product { get; set; }
}