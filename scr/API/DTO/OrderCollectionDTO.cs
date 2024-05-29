namespace api.DTO;

public class OrderCollectionDTO
{
    public IEnumerable<OrderDTO> Orders { get; set; }
    public int Count { get; set; }
    
    public OrderCollectionDTO(IEnumerable<OrderDTO> orders, int count)
    {
        Orders = orders;
        Count = count;
    }
}