using System;
using System.Collections.Generic;

namespace api.Entities;

public partial class ParameterProduct
{
    public int ProductId { get; set; }

    public int ParameterId { get; set; }

    public string Value { get; set; } = null!;

    public virtual Parameter Parameter { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
