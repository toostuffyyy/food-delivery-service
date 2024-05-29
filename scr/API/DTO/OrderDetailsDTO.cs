using System.Text.Json.Serialization;
using api.Entities;

namespace api.DTO;

public class OrderDetailsDTO
{
    [JsonConstructor]
    public OrderDetailsDTO() { }
    
    public OrderDetailsDTO(Order order)
    {
        Id = order.Id;
        StatusId = order.StatusId;
        ClientId = order.ClientId;
        Sum = order.Sum;
        MinSum = order.MinSum;
        PriceDelivery = order.PriceDelivery;
        PriceAssembly = order.PriceAssembly;
        Street = order.Street;
        House = order.House;
        Apartment = order.Apartment;
        Intercom = order.Intercom;
        Floor = order.Floor;
        Status = order.Status.Name;
        StartDateTime = order.StartDateTime;
        EndDateTime = order.EndDateTime;
        Client = new ClientDTO(order.Client);
        Review = order.Review != null ? new ReviewDTO(order.Review) : null;
        OrderItems = order.OrderItems.ToList().ConvertAll(a => new OrderItemDTO(a));
    }
    
    public int Id { get; set; }
    public int StatusId { get; set; }
    public int ClientId { get; set; }
    public decimal Sum { get; set; }
    public decimal? MinSum { get; set; }
    public decimal? PriceDelivery { get; set; }
    public decimal? PriceAssembly { get; set; }
    public string Street { get; set; } = null!;
    public int House { get; set; }
    public int Apartment { get; set; }
    public int? Intercom { get; set; }
    public int? Floor { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
    public string Status { get; set; }
    public ClientDTO Client { get; set; }
    public ReviewDTO? Review { get; set; }
    public ICollection<OrderItemDTO> OrderItems { get; set; }
}