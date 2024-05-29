using System.Collections.Generic;
using System.Threading.Tasks;
using DeliveryEatsClientMobile.Models;
using Refit;

namespace DeliveryEatsClientMobile.Service;

public interface ICategoryProductRepository
{
    [Get("/CategoryProduct/GetCategoryProduct")]
    public Task<List<CategoryProduct>> GetCategoryProduct();
    [Get("/CategoryProduct/GetCategoryProduct/{id}")]
    public Task<CategoryProduct> GetCategoryProduct([AliasAs("id")] int Id);
    [Get("/CategoryProduct/GetCategoryProductMainPage")]
    public Task<List<CategoryProduct>> GetCategoryProductMainPage();
}