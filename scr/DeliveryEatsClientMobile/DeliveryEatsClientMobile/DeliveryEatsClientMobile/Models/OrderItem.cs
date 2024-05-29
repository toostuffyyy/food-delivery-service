namespace DeliveryEatsClientMobile.Models;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public double Quantity { get; set; }
    public Product Product { get; set; }
}