using api.Entities;

namespace api.DTO;

public class OrderStatusHistoryDTO
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int StatusId { get; set; }
    public DateTime DateTime { get; set; }

    public OrderStatusHistoryDTO(OrderStatusHistory orderStatusHistory)
    {
        Id = orderStatusHistory.Id;
        OrderId = orderStatusHistory.OrderId;
        StatusId = orderStatusHistory.StatusId;
        DateTime = orderStatusHistory.DateTime;
    }
}