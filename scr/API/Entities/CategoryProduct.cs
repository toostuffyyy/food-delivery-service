using System;
using System.Collections.Generic;

namespace api.Entities;

public partial class CategoryProduct
{
    public int Id { get; set; }

    public int? ParentCategoryProductId { get; set; }

    public string Name { get; set; } = null!;

    public string? ImagePath { get; set; }

    public virtual ICollection<CategoryProduct> InverseParentCategoryProduct { get; } = new List<CategoryProduct>();

    public virtual CategoryProduct? ParentCategoryProduct { get; set; }

    public virtual ICollection<Product> Products { get; } = new List<Product>();
}
