using commercetools.Sdk.Api.Models.Products;

namespace Enterspeed.Commercetools.Integration.Api.Services;

public interface IProductVariantIdFactory
{
    Task<string> GetProductVariantIdAsync(IProductProjection productProjection, IProductVariant productVariant);
}