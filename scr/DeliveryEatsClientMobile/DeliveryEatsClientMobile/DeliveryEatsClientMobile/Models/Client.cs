using System.Collections.Generic;
using ReactiveUI;

namespace DeliveryEatsClientMobile.Models;

public class Client : ReactiveObject
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? ImagePath { get; set; }
    public List<Address>? Addresses { get; set; }
    private Address _selectedAddress;
    public Address SelectedAddress
    {
        get => _selectedAddress;
        set => this.RaiseAndSetIfChanged(ref _selectedAddress, value);
    }
}