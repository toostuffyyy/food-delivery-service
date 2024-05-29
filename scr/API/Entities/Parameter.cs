using System;
using System.Collections.Generic;

namespace api.Entities;

public partial class Parameter
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<ParameterProduct> ParameterProducts { get; } = new List<ParameterProduct>();
}
