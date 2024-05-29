using System;
using System.Collections.Generic;

namespace api.Entities;

public partial class Shift
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public DateTime StartDateTime { get; set; }

    public DateTime? EndDateTime { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; } = new List<Order>();
}
