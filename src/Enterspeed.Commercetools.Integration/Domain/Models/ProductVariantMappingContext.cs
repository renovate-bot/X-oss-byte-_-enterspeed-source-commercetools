using commercetools.Sdk.Api.Models.Products;

namespace Enterspeed.Commercetools.Integration.Domain.Models;

public class ProductVariantMappingContext
{
    public ProductVariantMappingContext(IProductProjection product, IProductVariant variant)
    {
        Product = product;
        Variant = variant;
    }

    public IProductProjection Product{ get; set; }
    public IProductVariant Variant { get; set; }
}