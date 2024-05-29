using System.Collections.Generic;

namespace DeliveryEatsClientMobile.Models;

public class ClientDetails
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string Password { get; set; } = null!;
    public string? ImagePath { get; set; }
    public List<Address>? Addresses { get; set; }
}