using System.Text.Json.Serialization;
using api.Entities;

namespace api.DTO;

public class AddressDTO
{
    public int Id { get; set; }
    public string Street { get; set; } = null!;
    public int House { get; set; }
    public int? Apartment { get; set; }
    public int? Intercom { get; set; }
    public int? Floor { get; set; }
    public string? Comment { get; set; }

    [JsonConstructor]
    public AddressDTO() { }
    public AddressDTO(Address address)
    {
        Id = address.Id;
        Street = address.Street;
        House = address.House;
        Apartment = address.Apartment;
        Intercom = address.Intercom;
        Floor = address.Floor;
        Comment = address.Comment;
    }
}