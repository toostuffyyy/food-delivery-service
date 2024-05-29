using System.Collections.Generic;
using System.Threading.Tasks;
using desktop.Models;
using Refit;

namespace desktop.Service;

public interface ICategoryProductRepository
{
    [Get("/CategoryProduct/GetCategoryProduct")]
    public Task<IEnumerable<CategoryProduct>> GetCategoryProduct();
}