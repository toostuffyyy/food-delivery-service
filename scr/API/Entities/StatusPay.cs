using System;
using System.Collections.Generic;

namespace api.Entities;

public partial class StatusPay
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Pay> Pays { get; } = new List<Pay>();
}
