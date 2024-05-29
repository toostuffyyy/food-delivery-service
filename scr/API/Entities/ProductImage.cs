using System;
using System.Collections.Generic;

namespace api.Entities;

public partial class ProductImage
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string ImagePath { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
