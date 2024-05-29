using System;

namespace DeliveryEatsClientMobile.Models;

public class Promotion
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public int Discount { get; set; }
    public string? Description { get; set; }
}