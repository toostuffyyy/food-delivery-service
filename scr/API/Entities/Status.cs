using System;
using System.Collections.Generic;

namespace api.Entities;

public partial class Status
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<OrderStatusHistory> OrderStatusHistories { get; } = new List<OrderStatusHistory>();

    public virtual ICollection<Order> Orders { get; } = new List<Order>();
}
