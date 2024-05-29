namespace DeliveryEatsClientMobile.Models;

public class Address
{
    public int Id { get; set; }
    public string Street { get; set; } = null!;
    public int? House { get; set; }
    public int? Apartment { get; set; }
    public int? Intercom { get; set; }
    public int? Floor { get; set; }
    public string? Comment { get; set; }
    public string? FullAddress => $"ул. {Street}, {House}";
}