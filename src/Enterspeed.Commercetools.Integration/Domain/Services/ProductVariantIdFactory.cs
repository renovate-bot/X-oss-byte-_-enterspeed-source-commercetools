using commercetools.Sdk.Api.Models.Products;
using Enterspeed.Commercetools.Integration.Api.Services;

namespace Enterspeed.Commercetools.Integration.Domain.Services;

class ProductVariantIdFactory : IProductVariantIdFactory
{
    public Task<string> GetProductVariantIdAsync(IProductProjection productProjection, IProductVariant productVariant)
    {
        return Task.FromResult($"{productProjection.Id}-{productVariant.Id}");
    }
}