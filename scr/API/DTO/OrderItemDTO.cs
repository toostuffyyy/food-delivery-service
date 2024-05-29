using System.Text.Json.Serialization;
using api.Entities;

namespace api.DTO;

public class OrderItemDTO
{
    [JsonConstructor]
    public OrderItemDTO() { }
    
    public OrderItemDTO(OrderItem orderItem)
    {
        ProductId = orderItem.ProductId;
        OrderId = orderItem.OrderId;
        Quantity = orderItem.Quantity;
        Product = new ProductDTO(orderItem.Product);
    }

    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public ProductDTO Product { get; set; }
}