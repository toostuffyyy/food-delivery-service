using System.Threading.Tasks;
using DeliveryEatsClientMobile.Models;
using Refit;

namespace DeliveryEatsClientMobile.Service;

public interface IClientRepository
{
    [Get("/Client/GetInfo")]
    public Task<Client> GetInfo([Authorize("Bearer")] string accessToken);
    [Get("/Client/GetClientDetails/{id}")]
    public Task<ClientDetails> GetClientDetails([Authorize("Bearer")] string accessToken, [AliasAs("id")]int id);
    [Post("/Client/AddClient")]
    public Task AddClient([Body]ClientDetails clientDetails);
    [Put("/Client/UpdateClient")]
    public Task UpdateClient([Authorize("Bearer")] string accessToken, [Body]ClientDetails clientDetails);
    [Delete("/Client/DeleteClient/{id}")]
    public Task DeleteClient([Authorize("Bearer")] string accessToken, [AliasAs("id")] int id);
}