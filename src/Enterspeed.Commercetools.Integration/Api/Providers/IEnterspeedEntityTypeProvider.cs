using commercetools.Sdk.Api.Models.Categories;
using commercetools.Sdk.Api.Models.Products;

namespace Enterspeed.Commercetools.Integration.Api.Providers;

public interface IEnterspeedEntityTypeProvider
{
    Task<string> GetEntityTypeAsync(IProductProjection product);
    Task<string> GetEntityTypeAsync(ICategory product);
    Task<string> GetEntityTypeAsync(IProductVariant product);
}