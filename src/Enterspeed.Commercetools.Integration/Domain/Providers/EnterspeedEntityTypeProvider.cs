using commercetools.Sdk.Api.Models.Categories;
using commercetools.Sdk.Api.Models.Products;
using Enterspeed.Commercetools.Integration.Api.Providers;

namespace Enterspeed.Commercetools.Integration.Domain.Providers;

public class EnterspeedEntityTypeProvider : IEnterspeedEntityTypeProvider
{
    public Task<string> GetEntityTypeAsync(IProductProjection product)
    {
        if (string.IsNullOrWhiteSpace(product.ProductType.Obj?.Key))
        {
            throw new ArgumentNullException(nameof(product.ProductType.Obj.Key));
        }

        return Task.FromResult(product.ProductType.Obj.Key);
    }

    public Task<string> GetEntityTypeAsync(ICategory product)
    {
        return Task.FromResult("category");
    }

    public Task<string> GetEntityTypeAsync(IProductVariant product)
    {
        return Task.FromResult("variant");
    }
}