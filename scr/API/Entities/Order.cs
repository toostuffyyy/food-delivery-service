using System;
using System.Collections.Generic;

namespace api.Entities;

public partial class Order
{
    public int Id { get; set; }

    public int StatusId { get; set; }

    public int ClientId { get; set; }

    public decimal MinSum { get; set; }

    public decimal PriceDelivery { get; set; }

    public decimal PriceAssembly { get; set; }

    public DateTime StartDateTime { get; set; }

    public DateTime? EndDateTime { get; set; }

    public string Street { get; set; } = null!;

    public int House { get; set; }

    public int Apartment { get; set; }

    public int? Intercom { get; set; }

    public int? Floor { get; set; }

    public string? Comment { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<OrderStatusHistory> OrderStatusHistories { get; } = new List<OrderStatusHistory>();

    public virtual Pay? Pay { get; set; }

    public virtual Review? Review { get; set; }

    public virtual Status Status { get; set; } = null!;

    public virtual ICollection<Shift> Shifts { get; } = new List<Shift>();
    
    public decimal Sum => OrderItems.Sum(a => a.Quantity * a.Product.Price);
}
