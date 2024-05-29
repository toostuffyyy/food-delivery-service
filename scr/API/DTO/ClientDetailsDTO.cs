using System.Text.Json.Serialization;
using api.Entities;

namespace api.DTO;

public class ClientDetailsDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string Password { get; set; } = null!;
    public string? ImagePath { get; set; }
    public List<AddressDTO>? Addresses { get; set; }
    
    [JsonConstructor]
    public ClientDetailsDTO() { }
    public ClientDetailsDTO(Client client)
    {
        Id = client.Id;
        Name = client.Name;
        PhoneNumber = client.PhoneNumber;
        Email = client.Email;
        Password = client.Password;
        ImagePath = client.ImagePath;
        Addresses = client.Addresses.ToList().ConvertAll(a => new AddressDTO(a));
    }
}