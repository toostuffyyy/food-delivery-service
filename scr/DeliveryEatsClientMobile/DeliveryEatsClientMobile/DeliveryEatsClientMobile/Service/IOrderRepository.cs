using System.Collections.Generic;
using System.Threading.Tasks;
using DeliveryEatsClientMobile.Models;
using Refit;

namespace DeliveryEatsClientMobile.Service;

public interface IOrderRepository
{
    [Get("/Order/GetOrdersClient/{id})")]
    public Task<List<Order>> GetOrdersClient([Authorize("Bearer")] string accessToken, [AliasAs("id")]int id);
    [Post("/Order/AddOrder")]
    public Task AddOrder([Authorize("Bearer")] string accessToken,[Body]Order order);
}