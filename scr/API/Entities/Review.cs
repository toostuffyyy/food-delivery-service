using System;
using System.Collections.Generic;

namespace api.Entities;

public partial class Review
{
    public int OrderId { get; set; }

    public int ClientId { get; set; }

    public int Rating { get; set; }

    public DateTime CreateDateTime { get; set; }

    public string? Comment { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
