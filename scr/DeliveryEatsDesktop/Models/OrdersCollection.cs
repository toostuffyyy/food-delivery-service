using System.Collections.Generic;
using System.Linq;

namespace desktop.Models;

public class OrdersCollection
{
    public IEnumerable<Order> Orders { get; set; }
    public int Count { get; set; }
}