using api.Entities;

namespace api.DTO;

public class OrderDTO
{
    public int Id { get; set; }
    public string Status { get; set; } 
    public string Street { get; set; } = null!;
    public int House { get; set; }
    public int? Apartment { get; set; }
    public int? Intercom { get; set; }
    public int? Floor { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
    public decimal Sum { get; set; }

    public OrderDTO(Order order)
    {
        Id = order.Id;
        Status = order.Status.Name;
        Street = order.Street;
        House = order.House;
        Apartment = order.Apartment;
        Intercom = order.Intercom;
        Floor = order.Floor;
        StartDateTime = order.StartDateTime;
        EndDateTime = order.EndDateTime;
        Sum = order.Sum;
    }
}