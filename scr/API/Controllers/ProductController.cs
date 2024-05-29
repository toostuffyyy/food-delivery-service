using System.Linq.Expressions;
using System.Text.Json;
using api.Context;
using api.DTO;
using api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private DeliveryServiceContext _context;
    public ProductController(DeliveryServiceContext context)
    {
        _context = context;
    }
    
    [HttpGet("GetProducts")]
    public async Task<ActionResult<ProductCollectionDTO>> GetProducts([FromQuery]OwnerParameters ownerParameters)
    {
        try
        {
            IQueryable<Product> productsQuery = _context.Products
                .Include(p => p.CategoryProduct)
                .Include(p => p.ProductImages)
                .Include(p => p.Unit);
            // Применение фильтра по строке поиска.
            if (!string.IsNullOrEmpty(ownerParameters.SearchString))
            {
                string search = ownerParameters.SearchString.ToLower();
                productsQuery = productsQuery.Where(a => a.Name.ToLower().Contains(search));
            }
            // Применение фильтров.
            if (!string.IsNullOrEmpty(ownerParameters.Filters))
            {
                // Получаем идентификаторы категорий второго уровня, которые являются родителями для фильтруемой категории
                var parentCategoryIds = await _context.CategoryProducts
                    .Where(cp => cp.ParentCategoryProductId == int.Parse(ownerParameters.Filters))
                    .Select(cp => cp.Id)
                    .ToListAsync();

                productsQuery = productsQuery.Where(a => parentCategoryIds.Contains(a.CategoryProductId));
            }
            // Применение сортировки.
            if (!string.IsNullOrEmpty(ownerParameters.Sorts))
            {
                List<SortingDTO>? sortParameters = JsonSerializer.Deserialize<List<SortingDTO>>(ownerParameters.Sorts);
                IOrderedQueryable<Product> sortedProducts = null;
                foreach (var sort in sortParameters)
                {
                    sortedProducts = sortedProducts == null
                        ? sort.Direction
                            ? productsQuery.OrderByDescending(GetSortingProperty(sort.NameColumn))
                            : productsQuery.OrderBy(GetSortingProperty(sort.NameColumn))
                        : sort.Direction
                            ? sortedProducts.ThenByDescending(GetSortingProperty(sort.NameColumn))
                            : sortedProducts.ThenBy(GetSortingProperty(sort.NameColumn));
                }
                productsQuery = sortedProducts ?? productsQuery;
            }
            // Проверка, есть ли поиск или фильтры. Если нет, применяем пагинацию.
            if (string.IsNullOrEmpty(ownerParameters.SearchString) && string.IsNullOrEmpty(ownerParameters.Filters))
            {
                // Загрузка категорий первого уровня.
                var topLevelCategoryIds = await _context.CategoryProducts
                    .Where(cp => cp.ParentCategoryProductId == null)
                    .Select(cp => cp.Id)
                    .ToListAsync();

                var productsByTopCategory = new List<ProductDTO>();
                foreach (var categoryId in topLevelCategoryIds)
                {
                    var categoryProducts = await productsQuery
                        .Where(p => p.CategoryProduct.ParentCategoryProductId == categoryId || p.CategoryProduct.Id == categoryId)
                        .Take(ownerParameters.SizePage)
                        .ToListAsync();
                    productsByTopCategory.AddRange(categoryProducts.ConvertAll(p => new ProductDTO(p)));
                }

                return Ok(new ProductCollectionDTO(productsByTopCategory, await productsQuery.CountAsync()));
            }
            var products = await productsQuery.ToListAsync();
            return Ok(new ProductCollectionDTO(products.ConvertAll(p => new ProductDTO(p)), products.Count));
        }
        catch (JsonException)
        {
            return BadRequest();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    
    [HttpGet("GetProductDetails/{id}")]
    public async Task<ActionResult<ProductDetailsDTO>> GetProductDetails(int id)
    {
        try
        {
            var productDetails = await _context.Products
                .Include(a => a.CategoryProduct)
                .Include(a => a.Brand)
                .Include(a => a.Unit)
                .Include(a => a.Packaging)
                .Include(a => a.ProductImages)
                .Include(a => a.Promotion)
                .Include(a => a.ParameterProducts)
                    .ThenInclude(b => b.Parameter)
                .FirstOrDefaultAsync(a => a.Id == id);
            
            if (productDetails == null)
                return BadRequest();
            
            return Ok(new ProductDetailsDTO(productDetails));
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    
    private static Expression<Func<Product, object>> GetSortingProperty(string sortName)
    {
        return sortName?.ToLower() switch
        {
            "nameproduct" => product => product.Name,
            "price" => product => product.Price,
            _ => product => product.Id
        };
    }
}