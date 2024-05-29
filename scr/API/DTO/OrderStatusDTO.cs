using System.Text.Json.Serialization;
using api.Entities;

namespace api.DTO;

public class OrderStatusDTO
{
    public int Id { get; set; }
    public int StatusId { get; set; }
    
    [JsonConstructor]
    public OrderStatusDTO() { }
    
    public OrderStatusDTO(Order order)
    {
        Id = order.Id;
        StatusId = order.StatusId;
    }
}