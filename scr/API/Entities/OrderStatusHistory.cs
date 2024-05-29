using System;
using System.Collections.Generic;

namespace api.Entities;

public partial class OrderStatusHistory
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int StatusId { get; set; }

    public DateTime DateTime { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Status Status { get; set; } = null!;
}
