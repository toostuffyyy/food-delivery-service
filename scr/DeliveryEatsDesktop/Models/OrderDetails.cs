using System;
using System.Collections.Generic;

using api.Models.DTO;

namespace desktop.Models;

public class OrderDetails
{
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
    public string Address => $"ул. {Street}, д. {House}, кв. {Apartment}";
    public DateTime StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
    public string Status { get; set; }
    public Client Client { get; set; }
    public Review? Review { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; }
}