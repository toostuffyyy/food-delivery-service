using System;
using System.Collections.Generic;

namespace api.Entities;

public partial class Address
{
    public int Id { get; set; }

    public int ClientId { get; set; }

    public string Street { get; set; } = null!;

    public int House { get; set; }

    public int? Apartment { get; set; }

    public int? Intercom { get; set; }

    public int? Floor { get; set; }

    public string? Comment { get; set; }

    public virtual Client Client { get; set; } = null!;
}
