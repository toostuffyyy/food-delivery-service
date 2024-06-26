﻿using System;
using System.Collections.Generic;

namespace api.Entities;

public partial class OrderItem
{
    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
