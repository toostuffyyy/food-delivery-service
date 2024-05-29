using System;
using System.Collections.ObjectModel;
using ReactiveUI;

namespace DeliveryEatsClientMobile.Models;

public class Order : ReactiveObject
{
    public int Id { get; set; }
    public int StatusId { get; set; }
    public int? ManagerId { get; set; }
    public int ClientId { get; set; }
    public int? CourierId { get; set; }
    public string Street { get; set; } = null!;
    public int House { get; set; }
    public int? Apartment { get; set; }
    public int? Intercom { get; set; }
    public int? Floor { get; set; }
    public string Status { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
    public ObservableCollection<Product> Products { get; set; }
    private decimal _sum;
    public decimal Sum
    {
        get => _sum;
        set => this.RaiseAndSetIfChanged(ref _sum, value);
    }
}