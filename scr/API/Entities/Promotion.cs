using System;
using System.Collections.Generic;

namespace api.Entities;

public partial class Promotion
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime StartDateTime { get; set; }

    public DateTime EndDateTime { get; set; }

    public int Discount { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Product> Products { get; } = new List<Product>();
}
