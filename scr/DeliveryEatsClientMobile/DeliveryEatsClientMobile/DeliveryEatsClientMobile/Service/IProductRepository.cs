using System.Threading.Tasks;
using DeliveryEatsClientMobile.Models;
using desktop.Models;
using Refit;

namespace DeliveryEatsClientMobile.Service;

public interface IProductRepository
{
    [Get("/Product/GetProducts")]
    public Task<ProductCollection> GetProducts([Query]OwnerParameters ownerParameters);
    [Get("/Product/GetProductDetails/{id}")]
    public Task<ProductDetails> GetProductDetails([AliasAs("id")]int productId);
}