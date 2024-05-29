using System.Collections.Generic;

namespace DeliveryEatsClientMobile.Models;

public class GroupProduct
{
    public int Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<Product> Products { get; set; }
}