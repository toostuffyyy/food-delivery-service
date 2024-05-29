using System.Collections.Generic;

namespace DeliveryEatsClientMobile.Models;

public class ProductCollection
{
    public IEnumerable<Product> Product { get; set; }
    public int Count { get; set; }

    public ProductCollection(IEnumerable<Product> product, int count)
    {
        Product = product;
        Count = count;
    }
}