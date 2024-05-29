using System;
using System.Collections.Generic;

namespace api.Entities;

public partial class Pay
{
    public int OrderId { get; set; }

    public int StatusPayId { get; set; }

    public int TypePayId { get; set; }

    public DateTime DateTime { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual StatusPay StatusPay { get; set; } = null!;

    public virtual TypePay TypePay { get; set; } = null!;
}
