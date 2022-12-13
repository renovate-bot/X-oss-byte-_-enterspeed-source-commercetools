using commercetools.Sdk.Api.Models.Categories;
using commercetools.Sdk.Api.Models.Products;

namespace Enterspeed.Commercetools.Integration.Api.Repositories;

public interface ICommercetoolsRepository
{
    Task<ICategory?> GetCategoryByIdAsync(string categoryId);
    Task<IProductProjection?> GetProductByIdAsync(string productId);

    Task<List<IProductProjection>> GetAllProductsAsync();
    Task<List<ICategory>> GetAllCategoriesAsync();
}