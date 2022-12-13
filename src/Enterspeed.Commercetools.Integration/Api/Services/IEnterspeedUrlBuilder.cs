using commercetools.Sdk.Api.Models.Categories;
using commercetools.Sdk.Api.Models.Products;

namespace Enterspeed.Commercetools.Integration.Api.Services;

public interface IEnterspeedUrlBuilder
{
    Task<Dictionary<string, string>> BuildLocalizedUrlsAsync(ICategory category, List<ICategoryReference>? ancestors);

    Task<Dictionary<string, string>> BuildLocalizedUrlsAsync(IProductProjection product);
}