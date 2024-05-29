using System;

namespace desktop.Models;

public partial class Order
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
    public string Address => $"{Street}, {House}";
}
