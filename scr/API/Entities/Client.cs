using System;
using System.Collections.Generic;

namespace api.Entities;

public partial class Client
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string Password { get; set; } = null!;

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExp { get; set; }

    public string? ImagePath { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual ICollection<Order> Orders { get; } = new List<Order>();

    public virtual ICollection<Review> Reviews { get; } = new List<Review>();
}
