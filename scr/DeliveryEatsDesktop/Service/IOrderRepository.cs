using System.Collections.Generic;
using System.Threading.Tasks;
using api.Models.DTO;
using desktop.Models;
using Refit;

namespace desktop.Service;

public interface IOrderRepository
{
    [Get("/Order/GetOrders")]
    public Task<OrdersCollection> GetOrders([Authorize("Bearer")] string accessToken, [Query]OwnerParameters ownerParameters);
    [Get("/Order/GetOrderDetails/{id}")]
    public Task<OrderDetails> GetOrder([Authorize("Bearer")] string accessToken, [AliasAs("id")]int id);
    [Put("/Order/UpdateStatusOrder/")]
    public Task UpdateStatusOrder([Authorize("Bearer")] string accessToken, [Body] OrderStatus orderStatus);
    [Delete("/Order/DeleteOrder/{id}")]
    public Task DeleteOrder([Authorize("Bearer")] string accessToken, [AliasAs("id")]int id);
}