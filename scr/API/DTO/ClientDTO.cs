using System.Text.Json.Serialization;
using api.Entities;

namespace api.DTO;

public class ClientDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? ImagePath { get; set; }
    public List<AddressDTO>? Addresses { get; set; }

    [JsonConstructor]
    public ClientDTO() { }
    public ClientDTO(Client client)
    {
        Id = client.Id;
        Name = client.Name;
        ImagePath = client.ImagePath;
        Addresses = client.Addresses.ToList().ConvertAll(a => new AddressDTO(a));
    }
}